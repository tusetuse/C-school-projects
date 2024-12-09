namespace Zapocet_2
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.listBoxServers = new System.Windows.Forms.ListBox();
            this.btnDiscoverServers = new System.Windows.Forms.Button();
            this.treeViewNodes = new System.Windows.Forms.TreeView();
            this.rightPanel = new System.Windows.Forms.Panel();
            this.lblNodeId = new System.Windows.Forms.Label();
            this.txtNodeId = new System.Windows.Forms.TextBox();
            this.lblReadValue = new System.Windows.Forms.Label();
            this.txtReadValue = new System.Windows.Forms.TextBox();
            this.lblWriteValue = new System.Windows.Forms.Label();
            this.txtWriteValue = new System.Windows.Forms.TextBox();
            this.lblDataType = new System.Windows.Forms.Label();
            this.cmbDataType = new System.Windows.Forms.ComboBox();
            this.btnWrite = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.rightPanel.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.listBoxServers);
            this.splitContainer.Panel1.Controls.Add(this.btnDiscoverServers);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.treeViewNodes);
            this.splitContainer.Panel2.Controls.Add(this.rightPanel);
            this.splitContainer.Size = new System.Drawing.Size(878, 644);
            this.splitContainer.SplitterDistance = 292;
            this.splitContainer.TabIndex = 0;
            // 
            // listBoxServers
            // 
            this.listBoxServers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxServers.Location = new System.Drawing.Point(0, 20);
            this.listBoxServers.Name = "listBoxServers";
            this.listBoxServers.Size = new System.Drawing.Size(292, 624);
            this.listBoxServers.TabIndex = 0;
            this.listBoxServers.SelectedIndexChanged += new System.EventHandler(this.listBoxServers_SelectedIndexChanged);
            // 
            // btnDiscoverServers
            // 
            this.btnDiscoverServers.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnDiscoverServers.Location = new System.Drawing.Point(0, 0);
            this.btnDiscoverServers.Name = "btnDiscoverServers";
            this.btnDiscoverServers.Size = new System.Drawing.Size(292, 20);
            this.btnDiscoverServers.TabIndex = 1;
            this.btnDiscoverServers.Text = "Discover Servers";
            this.btnDiscoverServers.Click += new System.EventHandler(this.btnDiscoverServers_Click);
            // 
            // treeViewNodes
            // 
            this.treeViewNodes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewNodes.Location = new System.Drawing.Point(0, 0);
            this.treeViewNodes.Name = "treeViewNodes";
            this.treeViewNodes.Size = new System.Drawing.Size(325, 644);
            this.treeViewNodes.TabIndex = 0;
            // 
            // rightPanel
            // 
            this.rightPanel.Controls.Add(this.lblNodeId);
            this.rightPanel.Controls.Add(this.txtNodeId);
            this.rightPanel.Controls.Add(this.lblReadValue);
            this.rightPanel.Controls.Add(this.txtReadValue);
            this.rightPanel.Controls.Add(this.lblWriteValue);
            this.rightPanel.Controls.Add(this.txtWriteValue);
            this.rightPanel.Controls.Add(this.lblDataType);
            this.rightPanel.Controls.Add(this.cmbDataType);
            this.rightPanel.Controls.Add(this.btnWrite);
            this.rightPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.rightPanel.Location = new System.Drawing.Point(325, 0);
            this.rightPanel.Name = "rightPanel";
            this.rightPanel.Size = new System.Drawing.Size(257, 644);
            this.rightPanel.TabIndex = 1;
            // 
            // lblNodeId
            // 
            this.lblNodeId.Location = new System.Drawing.Point(9, 9);
            this.lblNodeId.Name = "lblNodeId";
            this.lblNodeId.Size = new System.Drawing.Size(86, 20);
            this.lblNodeId.TabIndex = 0;
            this.lblNodeId.Text = "Node ID:";
            // 
            // txtNodeId
            // 
            this.txtNodeId.Location = new System.Drawing.Point(9, 26);
            this.txtNodeId.Name = "txtNodeId";
            this.txtNodeId.Size = new System.Drawing.Size(241, 20);
            this.txtNodeId.TabIndex = 1;
            // 
            // lblReadValue
            // 
            this.lblReadValue.Location = new System.Drawing.Point(9, 52);
            this.lblReadValue.Name = "lblReadValue";
            this.lblReadValue.Size = new System.Drawing.Size(86, 20);
            this.lblReadValue.TabIndex = 2;
            this.lblReadValue.Text = "Current Value:";
            // 
            // txtReadValue
            // 
            this.txtReadValue.Location = new System.Drawing.Point(9, 69);
            this.txtReadValue.Name = "txtReadValue";
            this.txtReadValue.ReadOnly = true;
            this.txtReadValue.Size = new System.Drawing.Size(241, 20);
            this.txtReadValue.TabIndex = 3;
            // 
            // lblWriteValue
            // 
            this.lblWriteValue.Location = new System.Drawing.Point(9, 95);
            this.lblWriteValue.Name = "lblWriteValue";
            this.lblWriteValue.Size = new System.Drawing.Size(86, 20);
            this.lblWriteValue.TabIndex = 4;
            this.lblWriteValue.Text = "New Value:";
            // 
            // txtWriteValue
            // 
            this.txtWriteValue.Location = new System.Drawing.Point(9, 113);
            this.txtWriteValue.Name = "txtWriteValue";
            this.txtWriteValue.Size = new System.Drawing.Size(241, 20);
            this.txtWriteValue.TabIndex = 5;
            // 
            // lblDataType
            // 
            this.lblDataType.Location = new System.Drawing.Point(9, 139);
            this.lblDataType.Name = "lblDataType";
            this.lblDataType.Size = new System.Drawing.Size(86, 20);
            this.lblDataType.TabIndex = 6;
            this.lblDataType.Text = "Data Type:";
            // 
            // cmbDataType
            // 
            this.cmbDataType.Location = new System.Drawing.Point(9, 156);
            this.cmbDataType.Name = "cmbDataType";
            this.cmbDataType.Size = new System.Drawing.Size(241, 21);
            this.cmbDataType.TabIndex = 7;
            // 
            // btnWrite
            // 
            this.btnWrite.Location = new System.Drawing.Point(9, 182);
            this.btnWrite.Name = "btnWrite";
            this.btnWrite.Size = new System.Drawing.Size(240, 20);
            this.btnWrite.TabIndex = 8;
            this.btnWrite.Text = "Write Value";
            this.btnWrite.Click += new System.EventHandler(this.btnWrite_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 644);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 12, 0);
            this.statusStrip.Size = new System.Drawing.Size(878, 22);
            this.statusStrip.TabIndex = 1;
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(878, 666);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.statusStrip);
            this.Name = "Form1";
            this.Text = "OPC UA Browser";
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.rightPanel.ResumeLayout(false);
            this.rightPanel.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.ListBox listBoxServers;
        private System.Windows.Forms.TreeView treeViewNodes;
        private System.Windows.Forms.Button btnDiscoverServers;
        private System.Windows.Forms.TextBox txtNodeId;
        private System.Windows.Forms.TextBox txtReadValue;
        private System.Windows.Forms.TextBox txtWriteValue;
        private System.Windows.Forms.Button btnWrite;
        private System.Windows.Forms.ComboBox cmbDataType;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.Panel rightPanel;
        private System.Windows.Forms.Label lblNodeId;
        private System.Windows.Forms.Label lblReadValue;
        private System.Windows.Forms.Label lblWriteValue;
        private System.Windows.Forms.Label lblDataType;
    }
}
