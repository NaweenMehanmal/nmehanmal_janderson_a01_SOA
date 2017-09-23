namespace nmehanmal_janderson
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
            this.cServiceNames = new System.Windows.Forms.ComboBox();
            this.layoutMethodNames = new System.Windows.Forms.FlowLayoutPanel();
            this.layoutParameterNames = new System.Windows.Forms.FlowLayoutPanel();
            this.bHttpPostButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblSoapResponse = new System.Windows.Forms.Label();
            this.lblSoapResponseValue = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cServiceNames
            // 
            this.cServiceNames.FormattingEnabled = true;
            this.cServiceNames.Location = new System.Drawing.Point(190, 20);
            this.cServiceNames.Name = "cServiceNames";
            this.cServiceNames.Size = new System.Drawing.Size(121, 21);
            this.cServiceNames.TabIndex = 0;
            this.cServiceNames.SelectedIndexChanged += new System.EventHandler(this.cServiceNames_SelectedIndexChanged);
            // 
            // layoutMethodNames
            // 
            this.layoutMethodNames.Location = new System.Drawing.Point(12, 88);
            this.layoutMethodNames.Name = "layoutMethodNames";
            this.layoutMethodNames.Size = new System.Drawing.Size(581, 34);
            this.layoutMethodNames.TabIndex = 1;
            // 
            // layoutParameterNames
            // 
            this.layoutParameterNames.Location = new System.Drawing.Point(12, 154);
            this.layoutParameterNames.Name = "layoutParameterNames";
            this.layoutParameterNames.Size = new System.Drawing.Size(581, 34);
            this.layoutParameterNames.TabIndex = 2;
            // 
            // bHttpPostButton
            // 
            this.bHttpPostButton.Location = new System.Drawing.Point(13, 199);
            this.bHttpPostButton.Name = "bHttpPostButton";
            this.bHttpPostButton.Size = new System.Drawing.Size(120, 23);
            this.bHttpPostButton.TabIndex = 3;
            this.bHttpPostButton.Text = "POST";
            this.bHttpPostButton.UseVisualStyleBackColor = true;
            this.bHttpPostButton.Click += new System.EventHandler(this.bHttpPostButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(11, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(177, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "Select a Method to Call:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 127);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(136, 20);
            this.label2.TabIndex = 5;
            this.label2.Text = "Input Parameters:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(10, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(174, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "Choose a Web Service:";
            // 
            // lblSoapResponse
            // 
            this.lblSoapResponse.AutoSize = true;
            this.lblSoapResponse.Location = new System.Drawing.Point(42, 315);
            this.lblSoapResponse.Name = "lblSoapResponse";
            this.lblSoapResponse.Size = new System.Drawing.Size(91, 13);
            this.lblSoapResponse.TabIndex = 7;
            this.lblSoapResponse.Text = "The response is...";
            // 
            // lblSoapResponseValue
            // 
            this.lblSoapResponseValue.AutoSize = true;
            this.lblSoapResponseValue.Location = new System.Drawing.Point(139, 315);
            this.lblSoapResponseValue.Name = "lblSoapResponseValue";
            this.lblSoapResponseValue.Size = new System.Drawing.Size(0, 13);
            this.lblSoapResponseValue.TabIndex = 8;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(605, 487);
            this.Controls.Add(this.lblSoapResponseValue);
            this.Controls.Add(this.lblSoapResponse);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bHttpPostButton);
            this.Controls.Add(this.layoutParameterNames);
            this.Controls.Add(this.layoutMethodNames);
            this.Controls.Add(this.cServiceNames);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cServiceNames;
        private System.Windows.Forms.FlowLayoutPanel layoutMethodNames;
        private System.Windows.Forms.FlowLayoutPanel layoutParameterNames;
        private System.Windows.Forms.Button bHttpPostButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblSoapResponse;
        private System.Windows.Forms.Label lblSoapResponseValue;
    }
}

