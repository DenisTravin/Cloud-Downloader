using System.ComponentModel;
using System.Windows.Forms;

namespace BelkaCloudDownloader.Gui
{
    sealed partial class ResultsViewer
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
            this.resultsSplitContainer = new System.Windows.Forms.SplitContainer();
            this.resultsView = new System.Windows.Forms.TreeView();
            this.detailedInfoView = new System.Windows.Forms.ListView();
            this.field = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.value = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            ((System.ComponentModel.ISupportInitialize)(this.resultsSplitContainer)).BeginInit();
            this.resultsSplitContainer.Panel1.SuspendLayout();
            this.resultsSplitContainer.Panel2.SuspendLayout();
            this.resultsSplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // resultsSplitContainer
            // 
            this.resultsSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultsSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.resultsSplitContainer.Name = "resultsSplitContainer";
            // 
            // resultsSplitContainer.Panel1
            // 
            this.resultsSplitContainer.Panel1.Controls.Add(this.resultsView);
            // 
            // resultsSplitContainer.Panel2
            // 
            this.resultsSplitContainer.Panel2.Controls.Add(this.detailedInfoView);
            this.resultsSplitContainer.Size = new System.Drawing.Size(933, 371);
            this.resultsSplitContainer.SplitterDistance = 309;
            this.resultsSplitContainer.TabIndex = 5;
            // 
            // resultsView
            // 
            this.resultsView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultsView.Location = new System.Drawing.Point(0, 0);
            this.resultsView.Name = "resultsView";
            this.resultsView.Size = new System.Drawing.Size(309, 371);
            this.resultsView.TabIndex = 0;
            this.resultsView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.OnResultsViewAfterSelect);
            // 
            // detailedInfoView
            // 
            this.detailedInfoView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.field,
            this.value});
            this.detailedInfoView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailedInfoView.FullRowSelect = true;
            this.detailedInfoView.GridLines = true;
            this.detailedInfoView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.detailedInfoView.LabelEdit = true;
            this.detailedInfoView.Location = new System.Drawing.Point(0, 0);
            this.detailedInfoView.Name = "detailedInfoView";
            this.detailedInfoView.Size = new System.Drawing.Size(620, 371);
            this.detailedInfoView.TabIndex = 0;
            this.detailedInfoView.UseCompatibleStateImageBehavior = false;
            this.detailedInfoView.View = System.Windows.Forms.View.Details;
            // 
            // field
            // 
            this.field.Text = "Field";
            this.field.Width = 200;
            // 
            // value
            // 
            this.value.Text = "Value";
            this.value.Width = 800;
            // 
            // ResultsViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.resultsSplitContainer);
            this.Name = "ResultsViewer";
            this.Size = new System.Drawing.Size(933, 371);
            this.resultsSplitContainer.Panel1.ResumeLayout(false);
            this.resultsSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.resultsSplitContainer)).EndInit();
            this.resultsSplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private SplitContainer resultsSplitContainer;
        private TreeView resultsView;
        private ListView detailedInfoView;
        private ColumnHeader field;
        private ColumnHeader value;
    }
}
