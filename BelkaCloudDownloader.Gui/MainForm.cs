namespace BelkaCloudDownloader.Gui
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Properties;
    using Utils;
    using TaskStatus = Utils.TaskStatus;

    /// <summary>
    /// Small GUI meant for testing of BelkaCloudDownloader library.
    /// </summary>
    public sealed partial class MainForm : Form
    {
        /// <summary>
        /// A list of available cloud plugins.
        /// </summary>
        private readonly IList<CloudPlugin> plugins = new List<CloudPlugin> { new GoogleCloudPlugin() };

        /// <summary>
        /// Holds and serializes on disk currently selected downloading settings.
        /// </summary>
        private SettingsWrapper settings = new SettingsWrapper();

        /// <summary>
        /// Maps a downloader task to its last estimation. Used to quickly lookup estimation when progress of
        /// a task changed.
        /// </summary>
        private Dictionary<AbstractTask, int> subtaskEstimations = new Dictionary<AbstractTask, int>();

        /// <summary>
        /// Cancellation token source used to cancel downloading operation.
        /// </summary>
        private CancellationTokenSource cancellationTokenSource;

        /// <summary>
        /// State of a form.
        /// </summary>
        private State state = State.Ready;

        /// <summary>
        /// Last known status of a downloader. Used in progress reporting.
        /// </summary>
        private Status lastStatus;

        /// <summary>
        /// Last known information about current task. Used in progress reporting.
        /// </summary>
        private string lastInfo = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            this.InitializeComponent();

            if (string.IsNullOrEmpty(this.directorySelector.Directory))
            {
                this.downloadOrCancelButton.Enabled = false;
            }

            this.directorySelector.DirectoryChanged += this.OnTargetDirectoryChanged;

            foreach (var plugin in this.plugins)
            {
                foreach (var protocol in plugin.Protocols)
                {
                    protocol.Settings = this.settings;
                }

                var pluginRadioButton = new RadioButton { Text = plugin.CloudName };
                pluginRadioButton.CheckedChanged += this.OnPluginRadioButtonCheckedChanged;
                pluginRadioButton.Tag = plugin;
                this.selectCloudLayout.Controls.Add(pluginRadioButton);
                if (plugin.CloudName == this.settings.CloudName)
                {
                    pluginRadioButton.Checked = true;
                }
            }

            if ((this.settings.CloudName == null) && (this.selectCloudLayout.Controls.Count > 0))
            {
                (this.selectCloudLayout.Controls[0] as RadioButton).Checked = true;
            }
        }

        /// <summary>
        /// State of a form.
        /// </summary>
        private enum State
        {
            /// <summary>
            /// Form is waiting for user input.
            /// </summary>
            Ready,

            /// <summary>
            /// Application is downloading and can cancel downloading process.
            /// </summary>
            Downloading
        }

        /// <summary>
        /// Helper method that produces human-readable message from status report.
        /// </summary>
        /// <param name="status">Information about status change.</param>
        /// <returns>Human-readable status message.</returns>
        private static string StatusToString(Status status)
        {
            switch (status)
            {
                case Status.Initialized:
                    return "Ready.";
                case Status.Authenticating:
                    return "Authenticating...";
                case Status.Done:
                    return "Done.";
                case Status.DownloadingFiles:
                    return "Downloading files...";
                case Status.ListingFiles:
                    return "Listing files...";
                case Status.DownloadingMetaInformation:
                    return "Downloading meta information...";
                case Status.Error:
                    return "Error!";
                case Status.DownloadingAccountInfo:
                    return "Downloading account info...";
                case Status.DownloadingCalendar:
                    return "Downloading calendar...";
                case Status.DownloadingGeolocationData:
                    return "Downloading geolocation data...";
                default:
                    throw new ArgumentOutOfRangeException(nameof(status));
            }
        }

        /// <summary>
        /// Registers GUI handlers for events in downloader.
        /// </summary>
        /// <param name="downloader">Downloader object that needs its events to be connected and reported
        ///     by GUI.</param>
        private void ConnectDownloader(AbstractDownloader downloader)
        {
            downloader.EstimationChanged += (s, args) =>
                {
                    if (this.state != State.Ready)
                    {
                        this.overallProgressBar.Maximum = this.Estimate();
                    }
                };

            downloader.ProgressChanged += (s, args) =>
                {
                    if (this.state != State.Ready)
                    {
                        this.overallProgressBar.Value = args.Progress;
                        this.statusLabel.Text = this.StatusText(this.lastStatus, this.lastInfo);
                    }
                };

            downloader.CurrentOperationEstimationChanged += (s, args) =>
                {
                    this.subtaskEstimations[args.Task] = args.Estimation;
                    this.currentOperationProgressBar.Maximum = args.Estimation;
                };

            downloader.CurrentOperationProgressChanged += (s, args) =>
                {
                    if (!this.subtaskEstimations.ContainsKey(args.Task))
                    {
                        throw new InternalErrorException(
                            $"Have no estimation for task {args.Task.Info}, but it is reporting progress.");
                    }

                    this.currentOperationProgressBar.Maximum = this.subtaskEstimations[args.Task];
                    this.currentOperationProgressBar.Value = args.Progress;

                    if ((this.state == State.Ready) || (args.Info.Length == 0))
                    {
                        return;
                    }

                    this.lastInfo = args.Info;
                    this.statusLabel.Text = this.StatusText(this.lastStatus, args.Info);
                };

            // TODO: Display preliminary results.
            ////protocol.PreliminaryResultsReady += (s, args) => { };

            downloader.StatusChanged += (s, args) =>
            {
                if (this.state == State.Ready)
                {
                    return;
                }

                this.lastStatus = args.Status;
                this.lastInfo = string.Empty;
                this.statusLabel.Text = this.StatusText(args.Status);
            };
        }

        /// <summary>
        /// Returns text for status bar based on current status and additional info.
        /// </summary>
        /// <param name="status">Downloader status.</param>
        /// <param name="currentOperationInfo">Additional information about current operation.</param>
        /// <returns>Human-readable string with current status and progress info.</returns>
        private string StatusText(Status status, string currentOperationInfo = "") =>
            $"({this.overallProgressBar.Value}/{this.overallProgressBar.Maximum}) {MainForm.StatusToString(status)}"
                + (currentOperationInfo.Length == 0
                        ? string.Empty
                        : $" | ({this.currentOperationProgressBar.Value}/{this.currentOperationProgressBar.Maximum}) "
                          + $"{currentOperationInfo}");

        /// <summary>
        /// Handler for CheckedChanged event of plugin selection radio buttons.
        /// </summary>
        /// <param name="sender">Object that raised an event.</param>
        /// <param name="e">Event arguments.</param>
        private void OnPluginRadioButtonCheckedChanged(object sender, EventArgs e)
        {
            foreach (var radioButton in this.selectCloudLayout.Controls
                .Cast<object>().Select(control => control as RadioButton))
            {
                Debug.Assert(radioButton != null, "selectCloudLayout must contain only radio buttons");
                if (!radioButton.Checked)
                {
                    continue;
                }

                this.authenticationMethodLayout.Controls.Clear();
                this.capabilitiesLayout.Controls.Clear();
                var cloud = radioButton.Tag as CloudPlugin;
                Debug.Assert(cloud != null, "Radio buttons in selectCloudLayout must have a tag of type CloudPlugin.");

                this.settings.CloudName = cloud.CloudName;

                var authenticationMethodsSet = new HashSet<string>();

                foreach (var authenticationMethod in cloud.Protocols.SelectMany(
                    protocol => protocol.AuthenticationMethods))
                {
                    authenticationMethodsSet.Add(authenticationMethod);
                }

                foreach (var authenticationMethod in authenticationMethodsSet)
                {
                    var authenticationRadioButton = new RadioButton { Text = authenticationMethod };
                    authenticationRadioButton.CheckedChanged += this.OnAuthenticationRadioButtonCheckedChanged;
                    authenticationRadioButton.Tag = cloud;
                    authenticationRadioButton.Width = 180;
                    this.authenticationMethodLayout.Controls.Add(authenticationRadioButton);

                    if (authenticationMethod == this.settings.GetAuthenticationMethod())
                    {
                        authenticationRadioButton.Checked = true;
                    }
                }

                if ((this.settings.GetAuthenticationMethod() == null)
                    && (this.authenticationMethodLayout.Controls.Count > 0))
                {
                    ((RadioButton)this.authenticationMethodLayout.Controls[0]).Checked = true;
                }

                return;
            }
        }

        /// <summary>
        /// Handler for CheckedChanged event of authentication methods radio buttons.
        /// </summary>
        /// <param name="sender">Object that raised an event.</param>
        /// <param name="e">Event arguments.</param>
        private void OnAuthenticationRadioButtonCheckedChanged(object sender, EventArgs e)
        {
            foreach (var radioButton in this.authenticationMethodLayout.Controls
                .Cast<object>().Select(control => control as RadioButton))
            {
                Debug.Assert(radioButton != null, "authenticationMethodLayout must contain only radio buttons");
                if (!radioButton.Checked)
                {
                    continue;
                }

                var authenticationMethod = radioButton.Text;
                this.settings.SetAuthenticationMethod(authenticationMethod);
                var cloud = radioButton.Tag as CloudPlugin;
                Debug.Assert(
                    cloud != null,
                    "Radio buttons in authenticationMethodLayout must have a tag of type CloudPlugin.");

                this.capabilitiesLayout.Controls.Clear();
                this.authenticationInfoPanel.Controls.Clear();

                var storedCapabilities = this.settings.GetCapabilities(authenticationMethod);

                foreach (var protocol in cloud.Protocols)
                {
                    if (!protocol.AuthenticationMethods.Contains(authenticationMethod))
                    {
                        continue;
                    }

                    foreach (var capability in protocol.Capabilities)
                    {
                        var capabilityCheckBox = new CheckBox
                        {
                            Text = capability,
                            Checked = true,
                            Width = 200,
                            Tag = authenticationMethod
                        };

                        capabilityCheckBox.CheckedChanged += this.CapabilityCheckBoxOnCheckedChanged;
                        this.capabilitiesLayout.Controls.Add(capabilityCheckBox);
                        capabilityCheckBox.Checked = storedCapabilities.Contains(capability);
                    }
                }

                var authenticationControl = cloud.Protocols
                    .First(p => p.AuthenticationMethods.Contains(authenticationMethod))
                    .AuthenticationControl(authenticationMethod);

                authenticationControl.Dock = DockStyle.Fill;
                this.authenticationInfoPanel.Controls.Add(authenticationControl);
            }
        }

        /// <summary>
        /// Handler for changing of check state of one of the capabilities checkbox.
        /// </summary>
        /// <param name="sender">Checkbox that sent an event.</param>
        /// <param name="eventArgs">Event arguments.</param>
        private void CapabilityCheckBoxOnCheckedChanged(object sender, EventArgs eventArgs)
        {
            var checkBox = sender as CheckBox;
            Debug.Assert(
                checkBox != null,
                "Capability CheckBox shal be a sender for CapabilityCheckBoxOnCheckedChanged.");
            var authenticationMethod = (string)checkBox.Tag;
            this.settings.SetCapability(authenticationMethod, checkBox.Text, checkBox.Checked);
        }

        /// <summary>
        /// Handler for "Download" button, which also works as "Cancel" when the program is downloading.
        /// </summary>
        /// <param name="sender">Button that sends the signal.</param>
        /// <param name="args">Event arguments.</param>
        private async void OnDownloadOrCancelButtonClick(object sender, EventArgs args)
        {
            if (this.state == State.Ready)
            {
                this.resultsViewer.Clear();
                this.currentOperationProgressBar.Value = 0;
                this.overallProgressBar.Value = 0;
                this.state = State.Downloading;
                this.downloadOrCancelButton.Text = Resources.MainFormOnDownloadOrCancelButtonClickCancel;
                this.cancellationTokenSource = new CancellationTokenSource();

                var currentExecutedProtocols = new Downloader(this.cancellationTokenSource.Token);
                this.SelectedProtocols().ToList().ForEach(p => currentExecutedProtocols.AddSubDownloader(p));
                currentExecutedProtocols.Log = DownloaderGlobalLog.Log;

                this.ConnectDownloader(currentExecutedProtocols);

                var tasks = new List<Task<object>>();

                var results = new List<object>();

                this.overallProgressBar.Maximum = this.Estimate();
                this.currentOperationProgressBar.Maximum = 0;

                try
                {
                    tasks.AddRange(
                        this.SelectedProtocols()
                            .Select(
                                protocol =>
                                protocol.Execute(
                                    this.directorySelector.Directory,
                                    this.authenticationInfoPanel.Controls[0],
                                    this.SelectedCapabilities().ToList(),
                                    this.cancellationTokenSource.Token)));

                    foreach (var task in tasks)
                    {
                        results.Add(await task);
                    }

                    this.resultsViewer.DisplayResults(results);
                }
                catch (TaskCanceledException)
                {
                    this.statusLabel.Text = Resources.MainFormOnDownloadOrCancelButtonClickTaskCancelled;
                    this.overallProgressBar.Value = 0;
                    this.currentOperationProgressBar.Value = 0;
                }
                catch (Exception e)
                {
                    DownloaderGlobalLog.Log.LogError("Exception was unhandled by downloader, aborting download.", e);
                    this.cancellationTokenSource.Cancel();
                    this.statusLabel.Text = this.StatusText(Status.Error, "Unhandled exception, see log for details.");
                }

                this.state = State.Ready;
                if (this.overallProgressBar.Value != 0)
                {
                    this.overallProgressBar.Value = this.overallProgressBar.Maximum;
                }

                if (!this.cancellationTokenSource.Token.IsCancellationRequested)
                {
                    this.ReportTaskStatus(currentExecutedProtocols);
                }

                this.SelectedProtocols().ToList().ForEach(p => p.Clear());
                currentExecutedProtocols.Clear();

                this.downloadOrCancelButton.Text = Resources.MainFormOnDownloadOrCancelButtonClickDownload;
                this.downloadOrCancelButton.Enabled = true;
            }
            else
            {
                this.cancellationTokenSource.Cancel();
                this.downloadOrCancelButton.Enabled = false;
            }
        }

        /// <summary>
        /// Prints status of a downloader in status bar.
        /// </summary>
        /// <param name="downloader">Downloader whose status shall be reported.</param>
        private void ReportTaskStatus(AbstractDownloader downloader)
        {
            this.statusLabel.Text = this.StatusText(downloader.Status);
            switch (downloader.TaskStatus)
            {
                case TaskStatus.Failure:
                    this.statusLabel.Text += Resources.MainFormDownloadingFailed;
                    break;
                case TaskStatus.Errors:
                    this.statusLabel.Text += Resources.MainFormDownloadingComletedWithErrors;
                    break;
                case TaskStatus.Success:
                    this.statusLabel.Text += Resources.MainFormDownloadingCompletedSuccessfully;
                    break;
            }
        }

        /// <summary>
        /// Gets an estimation of time required by currently selected protocols in abstract work units.
        /// </summary>
        /// <returns>Estimation of time required by currently selected protocols (in abstract work units).</returns>
        private int Estimate()
            => this.SelectedProtocols().Aggregate(0, (current, protocol) => current + protocol.Estimation);

        /// <summary>
        /// Returns a collection of currently selected protocols.
        /// </summary>
        /// <returns>A collection of protocols selected by user.</returns>
        private IEnumerable<Protocol> SelectedProtocols()
        {
            var authenticationMethodRadioButton = this.authenticationMethodLayout.Controls
                .Cast<RadioButton>().FirstOrDefault(rb => rb.Checked);
            var authenticationMethod = authenticationMethodRadioButton?.Text;
            var cloud = authenticationMethodRadioButton?.Tag as CloudPlugin;
            if (cloud == null)
            {
                yield break;
            }

            foreach (var protocol in cloud.Protocols.Where(
                    protocol => protocol.AuthenticationMethods.Contains(authenticationMethod)
                            && protocol.Capabilities.Intersect(this.SelectedCapabilities()).Any()))
            {
                yield return protocol;
            }
        }

        /// <summary>
        /// Returns a list of selected pieces of information to download.
        /// </summary>
        /// <returns>A list of selected capability names taken from protocols.</returns>
        private IEnumerable<string> SelectedCapabilities() =>
            this.capabilitiesLayout.Controls.Cast<CheckBox>().Where(cb => cb.Checked).Select(cb => cb.Text);

        /// <summary>
        /// Handler for changes in a field where folder with downloaded files shall be entered.
        /// </summary>
        /// <param name="sender">Control that sent a signal.</param>
        /// <param name="e">Event arguments.</param>
        private void OnTargetDirectoryChanged(object sender, EventArgs e)
            => this.downloadOrCancelButton.Enabled = !string.IsNullOrEmpty(this.directorySelector.Directory);
    }
}
