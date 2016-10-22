using System.ComponentModel;
using System.Windows.Forms;

namespace BelkaCloudDownloader.Gui
{
    sealed partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.downloadOrCancelButton = new System.Windows.Forms.Button();
            this.overallProgressBar = new System.Windows.Forms.ProgressBar();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.currentOperationProgressBar = new System.Windows.Forms.ProgressBar();
            this.authenticationInfoPanel = new System.Windows.Forms.GroupBox();
            this.selectCloudPanel = new System.Windows.Forms.GroupBox();
            this.selectCloudLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.selectAuthenticationMethodPanel = new System.Windows.Forms.GroupBox();
            this.authenticationMethodLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.capabilitiesBox = new System.Windows.Forms.GroupBox();
            this.capabilitiesLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.resultsViewer = new BelkaCloudDownloader.Gui.ResultsViewer();
            this.directorySelector = new BelkaCloudDownloader.Gui.DirectorySelector();
            this.statusBar.SuspendLayout();
            this.selectCloudPanel.SuspendLayout();
            this.selectAuthenticationMethodPanel.SuspendLayout();
            this.capabilitiesBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // downloadOrCancelButton
            // 
            this.downloadOrCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.downloadOrCancelButton.Location = new System.Drawing.Point(8, 656);
            this.downloadOrCancelButton.Name = "downloadOrCancelButton";
            this.downloadOrCancelButton.Size = new System.Drawing.Size(1143, 43);
            this.downloadOrCancelButton.TabIndex = 0;
            this.downloadOrCancelButton.Text = "Download";
            this.downloadOrCancelButton.UseVisualStyleBackColor = true;
            this.downloadOrCancelButton.Click += new System.EventHandler(this.OnDownloadOrCancelButtonClick);
            // 
            // overallProgressBar
            // 
            this.overallProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.overallProgressBar.Location = new System.Drawing.Point(9, 598);
            this.overallProgressBar.Name = "overallProgressBar";
            this.overallProgressBar.Size = new System.Drawing.Size(1142, 23);
            this.overallProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.overallProgressBar.TabIndex = 1;
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusBar.Location = new System.Drawing.Point(0, 702);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(1158, 22);
            this.statusBar.TabIndex = 8;
            this.statusBar.Text = "statusBar";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // currentOperationProgressBar
            // 
            this.currentOperationProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.currentOperationProgressBar.Location = new System.Drawing.Point(9, 627);
            this.currentOperationProgressBar.Name = "currentOperationProgressBar";
            this.currentOperationProgressBar.Size = new System.Drawing.Size(1142, 23);
            this.currentOperationProgressBar.TabIndex = 9;
            // 
            // authenticationInfoPanel
            // 
            this.authenticationInfoPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.authenticationInfoPanel.Location = new System.Drawing.Point(516, 13);
            this.authenticationInfoPanel.Name = "authenticationInfoPanel";
            this.authenticationInfoPanel.Size = new System.Drawing.Size(635, 111);
            this.authenticationInfoPanel.TabIndex = 15;
            this.authenticationInfoPanel.TabStop = false;
            this.authenticationInfoPanel.Text = "Authentication info:";
            // 
            // selectCloudPanel
            // 
            this.selectCloudPanel.Controls.Add(this.selectCloudLayout);
            this.selectCloudPanel.Location = new System.Drawing.Point(8, 13);
            this.selectCloudPanel.Name = "selectCloudPanel";
            this.selectCloudPanel.Size = new System.Drawing.Size(140, 111);
            this.selectCloudPanel.TabIndex = 16;
            this.selectCloudPanel.TabStop = false;
            this.selectCloudPanel.Text = "Select a cloud:";
            // 
            // selectCloudLayout
            // 
            this.selectCloudLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.selectCloudLayout.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.selectCloudLayout.Location = new System.Drawing.Point(3, 16);
            this.selectCloudLayout.Name = "selectCloudLayout";
            this.selectCloudLayout.Size = new System.Drawing.Size(134, 92);
            this.selectCloudLayout.TabIndex = 0;
            // 
            // selectAuthenticationMethodPanel
            // 
            this.selectAuthenticationMethodPanel.Controls.Add(this.authenticationMethodLayout);
            this.selectAuthenticationMethodPanel.Location = new System.Drawing.Point(154, 13);
            this.selectAuthenticationMethodPanel.Name = "selectAuthenticationMethodPanel";
            this.selectAuthenticationMethodPanel.Size = new System.Drawing.Size(356, 111);
            this.selectAuthenticationMethodPanel.TabIndex = 17;
            this.selectAuthenticationMethodPanel.TabStop = false;
            this.selectAuthenticationMethodPanel.Text = "Select authentication method:";
            // 
            // authenticationMethodLayout
            // 
            this.authenticationMethodLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.authenticationMethodLayout.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.authenticationMethodLayout.Location = new System.Drawing.Point(3, 16);
            this.authenticationMethodLayout.Name = "authenticationMethodLayout";
            this.authenticationMethodLayout.Size = new System.Drawing.Size(350, 92);
            this.authenticationMethodLayout.TabIndex = 0;
            // 
            // capabilitiesBox
            // 
            this.capabilitiesBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.capabilitiesBox.Controls.Add(this.capabilitiesLayout);
            this.capabilitiesBox.Location = new System.Drawing.Point(8, 127);
            this.capabilitiesBox.Name = "capabilitiesBox";
            this.capabilitiesBox.Size = new System.Drawing.Size(1143, 78);
            this.capabilitiesBox.TabIndex = 18;
            this.capabilitiesBox.TabStop = false;
            this.capabilitiesBox.Text = "Select data to download:";
            // 
            // capabilitiesLayout
            // 
            this.capabilitiesLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.capabilitiesLayout.Location = new System.Drawing.Point(3, 16);
            this.capabilitiesLayout.Name = "capabilitiesLayout";
            this.capabilitiesLayout.Size = new System.Drawing.Size(1137, 59);
            this.capabilitiesLayout.TabIndex = 0;
            // 
            // resultsViewer
            // 
            this.resultsViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resultsViewer.Location = new System.Drawing.Point(8, 239);
            this.resultsViewer.Name = "resultsViewer";
            this.resultsViewer.Size = new System.Drawing.Size(1143, 353);
            this.resultsViewer.TabIndex = 23;
            // 
            // directorySelector
            // 
            this.directorySelector.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.directorySelector.Location = new System.Drawing.Point(8, 209);
            this.directorySelector.Name = "directorySelector";
            this.directorySelector.Size = new System.Drawing.Size(1143, 27);
            this.directorySelector.TabIndex = 24;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1158, 724);
            this.Controls.Add(this.directorySelector);
            this.Controls.Add(this.resultsViewer);
            this.Controls.Add(this.capabilitiesBox);
            this.Controls.Add(this.selectAuthenticationMethodPanel);
            this.Controls.Add(this.selectCloudPanel);
            this.Controls.Add(this.authenticationInfoPanel);
            this.Controls.Add(this.currentOperationProgressBar);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.overallProgressBar);
            this.Controls.Add(this.downloadOrCancelButton);
            this.MinimumSize = new System.Drawing.Size(800, 450);
            this.Name = "MainForm";
            this.Text = "BelkaCloudDownloader";
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.selectCloudPanel.ResumeLayout(false);
            this.selectAuthenticationMethodPanel.ResumeLayout(false);
            this.capabilitiesBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button downloadOrCancelButton;
        private ProgressBar overallProgressBar;
        private ToolTip toolTip;
        private StatusStrip statusBar;
        private ToolStripStatusLabel statusLabel;
        private ProgressBar currentOperationProgressBar;
        private GroupBox authenticationInfoPanel;
        private GroupBox selectCloudPanel;
        private GroupBox selectAuthenticationMethodPanel;
        private GroupBox capabilitiesBox;
        private FlowLayoutPanel selectCloudLayout;
        private FlowLayoutPanel authenticationMethodLayout;
        private FlowLayoutPanel capabilitiesLayout;
        private ResultsViewer resultsViewer;
        private DirectorySelector directorySelector;
    }
}