namespace _DToursEditor
{
    partial class frmRenderMethod
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
            this.label1 = new System.Windows.Forms.Label();
            this.comboRenderers = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select a render method:";
            // 
            // comboRenderers
            // 
            this.comboRenderers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.comboRenderers.FormattingEnabled = true;
            this.comboRenderers.Items.AddRange(new object[] {
            "Irrlicht Software Renderer",
            "Apfelbaum Software Renderer",
            "DirectX 8.1",
            "DirectX 9.0c",
            "OpenGL 1.5"});
            this.comboRenderers.Location = new System.Drawing.Point(12, 30);
            this.comboRenderers.Name = "comboRenderers";
            this.comboRenderers.Size = new System.Drawing.Size(140, 97);
            this.comboRenderers.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 133);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(139, 27);
            this.button1.TabIndex = 2;
            this.button1.Text = "Use selected as default";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // frmRenderMethod
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(164, 177);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.comboRenderers);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmRenderMethod";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Render method";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmRenderMethod_FormClosing);
            this.Load += new System.EventHandler(this.frmRenderMethod_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox comboRenderers;
        private System.Windows.Forms.Button button1;
    }
}