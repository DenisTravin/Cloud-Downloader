namespace BelkaCloudDownloader.Gui
{
    using System;
    using System.Diagnostics;
    using System.Windows.Forms;
    using Properties;

    /// <summary>
    /// Control that allows to select a directory where downloaded artifacts shall be stored.
    /// </summary>
    public sealed partial class DirectorySelector : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DirectorySelector"/> class.
        /// </summary>
        public DirectorySelector()
        {
            this.InitializeComponent();
            this.directoryForDownloadedFilesTextBox.Text = Settings.Default.DirectoryToDownloadFiles;
        }

        /// <summary>
        /// Event that is emitted when selected directory is changed.
        /// </summary>
        public event EventHandler<EventArgs> DirectoryChanged;

        /// <summary>
        /// Gets path to a currently selected directory.
        /// </summary>
        public string Directory => this.directoryForDownloadedFilesTextBox.Text;

        /// <summary>
        /// Handler for "Browse" button. Opens folder selection dialog to select a folder where downloaded files
        /// shall be stored.
        /// </summary>
        /// <param name="sender">Button that sent a signal.</param>
        /// <param name="e">Event arguments.</param>
        private void OnBrowseButtonClick(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.ShowDialog();
                this.directoryForDownloadedFilesTextBox.Text = folderDialog.SelectedPath;
                Settings.Default.DirectoryToDownloadFiles = folderDialog.SelectedPath;
            }

            Settings.Default.Save();
        }

        /// <summary>
        /// Handler for an event that opens a target directory in file browser.
        /// </summary>
        /// <param name="sender">Object that sent an event.</param>
        /// <param name="e">Event parameters, ignored here.</param>
        private void OnOpenInExplorerButtonClick(object sender, EventArgs e)
            => Process.Start(this.directoryForDownloadedFilesTextBox.Text);

        /// <summary>
        /// Handler for changes in text box with target directory.
        /// </summary>
        /// <param name="sender">Object that sent an event.</param>
        /// <param name="e">Event parameters, ignored here.</param>
        private void OnDirectoryForDownloadedFilesTextBoxTextChanged(object sender, EventArgs e)
            => this.DirectoryChanged?.Invoke(this, EventArgs.Empty);
    }
}
