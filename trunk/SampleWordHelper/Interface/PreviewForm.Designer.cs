namespace SampleWordHelper.Interface
{
    partial class PreviewForm
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
            this.previewHandlerHostControl1 = new C4F.DevKit.PreviewHandler.PreviewHandlerHost.PreviewHandlerHostControl();
            this.SuspendLayout();
            // 
            // previewHandlerHostControl1
            // 
            this.previewHandlerHostControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.previewHandlerHostControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewHandlerHostControl1.FilePath = null;
            this.previewHandlerHostControl1.Location = new System.Drawing.Point(0, 0);
            this.previewHandlerHostControl1.Name = "previewHandlerHostControl1";
            this.previewHandlerHostControl1.Size = new System.Drawing.Size(757, 598);
            this.previewHandlerHostControl1.TabIndex = 0;
            // 
            // PreviewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(757, 598);
            this.Controls.Add(this.previewHandlerHostControl1);
            this.Name = "PreviewForm";
            this.Text = "PreviewForm";
            this.ResumeLayout(false);

        }

        #endregion

        private C4F.DevKit.PreviewHandler.PreviewHandlerHost.PreviewHandlerHostControl previewHandlerHostControl1;


    }
}