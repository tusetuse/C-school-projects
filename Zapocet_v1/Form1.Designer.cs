namespace Zapocet_v1
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.lblEndpointUrl = new System.Windows.Forms.Label();
            this.txtEndpointUrl = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.lblNodeId = new System.Windows.Forms.Label();
            this.txtNodeId = new System.Windows.Forms.TextBox();
            this.lblReadValue = new System.Windows.Forms.Label();
            this.txtReadValue = new System.Windows.Forms.TextBox();
            this.btnReadValue = new System.Windows.Forms.Button();
            this.lblWriteValue = new System.Windows.Forms.Label();
            this.txtWriteValue = new System.Windows.Forms.TextBox();
            this.btnWriteValue = new System.Windows.Forms.Button();
            this.lblDataType = new System.Windows.Forms.Label();
            this.cmbDataType = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // lblEndpointUrl
            // 
            this.lblEndpointUrl.AutoSize = true;
            this.lblEndpointUrl.Location = new System.Drawing.Point(10, 13);
            this.lblEndpointUrl.Name = "lblEndpointUrl";
            this.lblEndpointUrl.Size = new System.Drawing.Size(120, 13);
            this.lblEndpointUrl.TabIndex = 0;
            this.lblEndpointUrl.Text = "OPC UA Endpoint URL:";
            // 
            // txtEndpointUrl
            // 
            this.txtEndpointUrl.Location = new System.Drawing.Point(10, 30);
            this.txtEndpointUrl.Name = "txtEndpointUrl";
            this.txtEndpointUrl.Size = new System.Drawing.Size(301, 20);
            this.txtEndpointUrl.TabIndex = 1;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(317, 30);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(64, 20);
            this.btnConnect.TabIndex = 2;
            this.btnConnect.Text = "Connect";
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Location = new System.Drawing.Point(386, 30);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(64, 20);
            this.btnDisconnect.TabIndex = 3;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // lblNodeId
            // 
            this.lblNodeId.AutoSize = true;
            this.lblNodeId.Location = new System.Drawing.Point(10, 61);
            this.lblNodeId.Name = "lblNodeId";
            this.lblNodeId.Size = new System.Drawing.Size(50, 13);
            this.lblNodeId.TabIndex = 4;
            this.lblNodeId.Text = "Node ID:";
            // 
            // txtNodeId
            // 
            this.txtNodeId.Location = new System.Drawing.Point(10, 78);
            this.txtNodeId.Name = "txtNodeId";
            this.txtNodeId.Size = new System.Drawing.Size(301, 20);
            this.txtNodeId.TabIndex = 5;
            // 
            // lblReadValue
            // 
            this.lblReadValue.AutoSize = true;
            this.lblReadValue.Location = new System.Drawing.Point(10, 104);
            this.lblReadValue.Name = "lblReadValue";
            this.lblReadValue.Size = new System.Drawing.Size(66, 13);
            this.lblReadValue.TabIndex = 9;
            this.lblReadValue.Text = "Read Value:";
            // 
            // txtReadValue
            // 
            this.txtReadValue.Location = new System.Drawing.Point(103, 104);
            this.txtReadValue.Name = "txtReadValue";
            this.txtReadValue.ReadOnly = true;
            this.txtReadValue.Size = new System.Drawing.Size(301, 20);
            this.txtReadValue.TabIndex = 10;
            // 
            // btnReadValue
            // 
            this.btnReadValue.Enabled = false;
            this.btnReadValue.Location = new System.Drawing.Point(10, 130);
            this.btnReadValue.Name = "btnReadValue";
            this.btnReadValue.Size = new System.Drawing.Size(64, 20);
            this.btnReadValue.TabIndex = 8;
            this.btnReadValue.Text = "Read Value";
            this.btnReadValue.Click += new System.EventHandler(this.btnReadValue_Click);
            // 
            // lblWriteValue
            // 
            this.lblWriteValue.AutoSize = true;
            this.lblWriteValue.Location = new System.Drawing.Point(10, 156);
            this.lblWriteValue.Name = "lblWriteValue";
            this.lblWriteValue.Size = new System.Drawing.Size(65, 13);
            this.lblWriteValue.TabIndex = 11;
            this.lblWriteValue.Text = "Write Value:";
            // 
            // txtWriteValue
            // 
            this.txtWriteValue.Location = new System.Drawing.Point(103, 156);
            this.txtWriteValue.Name = "txtWriteValue";
            this.txtWriteValue.Size = new System.Drawing.Size(301, 20);
            this.txtWriteValue.TabIndex = 12;
            // 
            // btnWriteValue
            // 
            this.btnWriteValue.Enabled = false;
            this.btnWriteValue.Location = new System.Drawing.Point(10, 182);
            this.btnWriteValue.Name = "btnWriteValue";
            this.btnWriteValue.Size = new System.Drawing.Size(64, 20);
            this.btnWriteValue.TabIndex = 13;
            this.btnWriteValue.Text = "Write Value";
            this.btnWriteValue.Click += new System.EventHandler(this.btnWriteValue_Click);
            // 
            // lblDataType
            // 
            this.lblDataType.AutoSize = true;
            this.lblDataType.Location = new System.Drawing.Point(317, 61);
            this.lblDataType.Name = "lblDataType";
            this.lblDataType.Size = new System.Drawing.Size(60, 13);
            this.lblDataType.TabIndex = 6;
            this.lblDataType.Text = "Data Type:";
            // 
            // cmbDataType
            // 
            this.cmbDataType.Items.AddRange(new object[] {
            "System.Int32",
            "System.Double",
            "System.Boolean",
            "System.String"});
            this.cmbDataType.Location = new System.Drawing.Point(317, 78);
            this.cmbDataType.Name = "cmbDataType";
            this.cmbDataType.Size = new System.Drawing.Size(129, 21);
            this.cmbDataType.TabIndex = 7;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(476, 232);
            this.Controls.Add(this.lblEndpointUrl);
            this.Controls.Add(this.txtEndpointUrl);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.btnDisconnect);
            this.Controls.Add(this.lblNodeId);
            this.Controls.Add(this.txtNodeId);
            this.Controls.Add(this.lblDataType);
            this.Controls.Add(this.cmbDataType);
            this.Controls.Add(this.btnReadValue);
            this.Controls.Add(this.lblReadValue);
            this.Controls.Add(this.txtReadValue);
            this.Controls.Add(this.lblWriteValue);
            this.Controls.Add(this.txtWriteValue);
            this.Controls.Add(this.btnWriteValue);
            this.Name = "Form1";
            this.Text = "PLC OPC UA Communication";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        // Form controls
        private System.Windows.Forms.Label lblEndpointUrl;
        private System.Windows.Forms.TextBox txtEndpointUrl;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnDisconnect;

        private System.Windows.Forms.Label lblNodeId;
        private System.Windows.Forms.TextBox txtNodeId;

        private System.Windows.Forms.Label lblDataType;
        private System.Windows.Forms.ComboBox cmbDataType;

        private System.Windows.Forms.Button btnReadValue;
        private System.Windows.Forms.Label lblReadValue;
        private System.Windows.Forms.TextBox txtReadValue;

        private System.Windows.Forms.Label lblWriteValue;
        private System.Windows.Forms.TextBox txtWriteValue;
        private System.Windows.Forms.Button btnWriteValue;
    }
}

