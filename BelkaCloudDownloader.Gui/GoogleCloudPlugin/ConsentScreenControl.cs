namespace BelkaCloudDownloader.Gui
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using global::Google.Apis.Auth.OAuth2.Responses;
    using Timer = System.Timers.Timer;

    /// <summary>
    /// Provides a button to open OAuth 2.0 consent screen in browser and a text box with Refresh Token received
    /// with user consent.
    /// </summary>
    public sealed partial class ConsentScreenControl : UserControl
    {
        /// <summary>
        /// Text that is shown on button when control is ready to begin authentication.
        /// </summary>
        private const string ReadyText = "Authenticate using browser consent screen (or stored authentication info)";

        /// <summary>
        /// Text that is shown on button when user is already authenticated and Refresh Token is ready.
        /// </summary>
        private const string DoneText = "Done!";

        /// <summary>
        /// Text that is shown on button when user denied consent.
        /// </summary>
        private const string AccessDeniedText = "Access denied by user";

        /// <summary>
        /// Text that is shown on button when user interacts with a browser.
        /// </summary>
        private const string CancelText = "Cancel";

        /// <summary>
        /// Method that actually opens required consent screen. Protocol-specific.
        /// </summary>
        private readonly Func<CancellationToken, Task<string>> openConsentScreenDelegate;

        /// <summary>
        /// Timer that is used to revert button to the Ready state after showing the error.
        /// </summary>
        private readonly Timer cooldownTimer = new Timer(2000);

        /// <summary>
        /// Cancellation token source that is used to abort authentication.
        /// </summary>
        private CancellationTokenSource cancellationTokenSource;

        /// <summary>
        /// Current state of a control - ready to begin authentication, authenticating, done authentication.
        /// </summary>
        private State state = State.Ready;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsentScreenControl"/> class.
        /// </summary>
        /// <param name="openConsentScreenDelegate">A function that asynchronously opens consent screen in browser.
        /// It allows different protocols reuse this control to open their (possibly, different) consent screens.</param>
        public ConsentScreenControl(Func<CancellationToken, Task<string>> openConsentScreenDelegate)
        {
            this.InitializeComponent();

            this.openConsentScreenDelegate = openConsentScreenDelegate;
            this.cooldownTimer.Elapsed += (timerSender, timerArgs) =>
            {
                this.state = State.Ready;
                if (this.openConsentScreenButton.InvokeRequired)
                {
                    this.openConsentScreenButton.Invoke(
                        new Action<string>(s => this.openConsentScreenButton.Text = s),
                        ConsentScreenControl.ReadyText);
                }
                else
                {
                    this.openConsentScreenButton.Text = ConsentScreenControl.ReadyText;
                }

                this.cooldownTimer.Stop();
            };
        }

        /// <summary>
        /// State of the consent screen control - not opened, opened, got consent.
        /// </summary>
        private enum State
        {
            /// <summary>
            /// Consent screen was not opened, so this control awaits user input.
            /// </summary>
            Ready,

            /// <summary>
            /// User is supposedly interacting with the browser now.
            /// </summary>
            Authenticating,

            /// <summary>
            /// Got user consent and received Refresh Token.
            /// </summary>
            Done
        }

        /// <summary>
        /// Gets Refresh Token received with user consent.
        /// </summary>
        public string RefreshToken { get; private set; }

        /// <summary>
        /// Handler for a click on "Open consent screen" button, asynchronously launches browser.
        /// </summary>
        /// <param name="sender">Object who sent an event.</param>
        /// <param name="e">Event arguments (ignored here).</param>
        private async void OnOpenConsentScreenButtonClick(object sender, EventArgs e)
        {
            switch (this.state)
            {
                case State.Ready:
                    this.refreshTokenBox.Text = string.Empty;
                    this.cancellationTokenSource = new CancellationTokenSource();
                    try
                    {
                        var task = this.openConsentScreenDelegate(this.cancellationTokenSource.Token);
                        this.state = State.Authenticating;
                        this.openConsentScreenButton.Text = ConsentScreenControl.CancelText;
                        this.RefreshToken = await task;
                        /* TODO: Due to known bug in Google API (https://github.com/google/google-api-dotnet-client/issues/508) cancellation will not work
                          if user closes browser page. For now authentication task will become zombie, in future we shall find a workaround for this
                          (maybe implement our own OAuth consent mechanism). */
                        this.state = State.Done;
                        this.openConsentScreenButton.Text = ConsentScreenControl.DoneText;
                        this.openConsentScreenButton.Enabled = false;
                        this.refreshTokenBox.Text = this.RefreshToken;
                    }
                    catch (TokenResponseException ex)
                    {
                        if (ex.Error.Error == "access_denied")
                        {
                            this.openConsentScreenButton.Text = ConsentScreenControl.AccessDeniedText;
                            this.cooldownTimer.Start();
                        }
                        else
                        {
                            throw;
                        }
                    }

                    break;
                case State.Authenticating:
                    this.cancellationTokenSource.Cancel();
                    this.state = State.Ready;
                    this.openConsentScreenButton.Text = ConsentScreenControl.ReadyText;
                    break;
                case State.Done:
                    break;
            }
        }
    }
}
