using System.ComponentModel;
using System.Windows.Forms;

namespace BelkaCloudDownloader.Gui
{
    sealed partial class ConsentScreenControl
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
            this.openConsentScreenButton = new System.Windows.Forms.Button();
            this.refreshTokenLabel = new System.Windows.Forms.Label();
            this.refreshTokenBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // openConsentScreenButton
            // 
            this.openConsentScreenButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.openConsentScreenButton.Location = new System.Drawing.Point(0, 0);
            this.openConsentScreenButton.Name = "openConsentScreenButton";
            this.openConsentScreenButton.Size = new System.Drawing.Size(535, 41);
            this.openConsentScreenButton.TabIndex = 0;
            this.openConsentScreenButton.Text = "Authenticate using browser consent screen";
            this.openConsentScreenButton.UseVisualStyleBackColor = true;
            this.openConsentScreenButton.Click += new System.EventHandler(this.OnOpenConsentScreenButtonClick);
            // 
            // refreshTokenLabel
            // 
            this.refreshTokenLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.refreshTokenLabel.AutoSize = true;
            this.refreshTokenLabel.Location = new System.Drawing.Point(2, 50);
            this.refreshTokenLabel.Name = "refreshTokenLabel";
            this.refreshTokenLabel.Size = new System.Drawing.Size(209, 13);
            this.refreshTokenLabel.TabIndex = 1;
            this.refreshTokenLabel.Text = "Refresh token (can be saved for later use):";
            // 
            // refreshTokenBox
            // 
            this.refreshTokenBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshTokenBox.Location = new System.Drawing.Point(217, 46);
            this.refreshTokenBox.Name = "refreshTokenBox";
            this.refreshTokenBox.ReadOnly = true;
            this.refreshTokenBox.Size = new System.Drawing.Size(318, 20);
            this.refreshTokenBox.TabIndex = 2;
            // 
            // ConsentScreenControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.refreshTokenBox);
            this.Controls.Add(this.refreshTokenLabel);
            this.Controls.Add(this.openConsentScreenButton);
            this.Name = "ConsentScreenControl";
            this.Size = new System.Drawing.Size(538, 71);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button openConsentScreenButton;
        private Label refreshTokenLabel;
        private TextBox refreshTokenBox;
    }
}
