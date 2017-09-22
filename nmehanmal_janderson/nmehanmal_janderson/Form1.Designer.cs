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
            this.SuspendLayout();
            // 
            // cServiceNames
            // 
            this.cServiceNames.FormattingEnabled = true;
            this.cServiceNames.Location = new System.Drawing.Point(12, 12);
            this.cServiceNames.Name = "cServiceNames";
            this.cServiceNames.Size = new System.Drawing.Size(121, 21);
            this.cServiceNames.TabIndex = 0;
            this.cServiceNames.SelectedIndexChanged += new System.EventHandler(this.cServiceNames_SelectedIndexChanged);
            // 
            // layoutMethodNames
            // 
            this.layoutMethodNames.Location = new System.Drawing.Point(12, 39);
            this.layoutMethodNames.Name = "layoutMethodNames";
            this.layoutMethodNames.Size = new System.Drawing.Size(581, 29);
            this.layoutMethodNames.TabIndex = 1;
            // 
            // layoutParameterNames
            // 
            this.layoutParameterNames.Location = new System.Drawing.Point(12, 75);
            this.layoutParameterNames.Name = "layoutParameterNames";
            this.layoutParameterNames.Size = new System.Drawing.Size(581, 34);
            this.layoutParameterNames.TabIndex = 2;
            // 
            // bHttpPostButton
            // 
            this.bHttpPostButton.Location = new System.Drawing.Point(13, 116);
            this.bHttpPostButton.Name = "bHttpPostButton";
            this.bHttpPostButton.Size = new System.Drawing.Size(120, 23);
            this.bHttpPostButton.TabIndex = 3;
            this.bHttpPostButton.Text = "POST";
            this.bHttpPostButton.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(605, 487);
            this.Controls.Add(this.bHttpPostButton);
            this.Controls.Add(this.layoutParameterNames);
            this.Controls.Add(this.layoutMethodNames);
            this.Controls.Add(this.cServiceNames);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cServiceNames;
        private System.Windows.Forms.FlowLayoutPanel layoutMethodNames;
        private System.Windows.Forms.FlowLayoutPanel layoutParameterNames;
        private System.Windows.Forms.Button bHttpPostButton;
    }
}

