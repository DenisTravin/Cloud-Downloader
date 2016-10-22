using System.ComponentModel;
using System.Windows.Forms;

namespace BelkaCloudDownloader.Gui
{
    sealed partial class DirectorySelector
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.openInExplorerButton = new System.Windows.Forms.Button();
            this.browseButton = new System.Windows.Forms.Button();
            this.directoryForDownloadedFilesTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // openInExplorerButton
            // 
            this.openInExplorerButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.openInExplorerButton.Location = new System.Drawing.Point(839, 1);
            this.openInExplorerButton.Name = "openInExplorerButton";
            this.openInExplorerButton.Size = new System.Drawing.Size(103, 23);
            this.openInExplorerButton.TabIndex = 26;
            this.openInExplorerButton.Text = "Open in Explorer";
            this.openInExplorerButton.UseVisualStyleBackColor = true;
            this.openInExplorerButton.Click += new System.EventHandler(this.OnOpenInExplorerButtonClick);
            // 
            // browseButton
            // 
            this.browseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseButton.Location = new System.Drawing.Point(758, 1);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(75, 23);
            this.browseButton.TabIndex = 25;
            this.browseButton.Text = "Browse";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.OnBrowseButtonClick);
            // 
            // directoryForDownloadedFilesTextBox
            // 
            this.directoryForDownloadedFilesTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.directoryForDownloadedFilesTextBox.Location = new System.Drawing.Point(143, 3);
            this.directoryForDownloadedFilesTextBox.Name = "directoryForDownloadedFilesTextBox";
            this.directoryForDownloadedFilesTextBox.Size = new System.Drawing.Size(609, 20);
            this.directoryForDownloadedFilesTextBox.TabIndex = 24;
            this.directoryForDownloadedFilesTextBox.TextChanged += new System.EventHandler(this.OnDirectoryForDownloadedFilesTextBoxTextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "Directory to download files:";
            // 
            // DirectorySelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.openInExplorerButton);
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.directoryForDownloadedFilesTextBox);
            this.Controls.Add(this.label1);
            this.Name = "DirectorySelector";
            this.Size = new System.Drawing.Size(945, 27);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button openInExplorerButton;
        private Button browseButton;
        private TextBox directoryForDownloadedFilesTextBox;
        private Label label1;
    }
}
