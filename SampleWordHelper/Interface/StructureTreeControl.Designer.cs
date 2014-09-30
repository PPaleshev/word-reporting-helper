namespace SampleWordHelper.Interface
{
    partial class StructureTreeControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ImageList treeImages;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StructureTreeControl));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.treeStructure = new System.Windows.Forms.TreeView();
            treeImages = new System.Windows.Forms.ImageList(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.treeStructure, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 63F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(300, 450);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // treeStructure
            // 
            this.treeStructure.AllowDrop = true;
            this.treeStructure.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeStructure.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeStructure.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.treeStructure.ImageIndex = 0;
            this.treeStructure.ImageList = treeImages;
            this.treeStructure.Location = new System.Drawing.Point(3, 3);
            this.treeStructure.Name = "treeStructure";
            this.treeStructure.SelectedImageIndex = 0;
            this.treeStructure.ShowNodeToolTips = true;
            this.treeStructure.Size = new System.Drawing.Size(294, 381);
            this.treeStructure.TabIndex = 0;
            // 
            // treeImages
            // 
            treeImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("treeImages.ImageStream")));
            treeImages.TransparentColor = System.Drawing.Color.Transparent;
            treeImages.Images.SetKeyName(0, "closed-folder.png");
            treeImages.Images.SetKeyName(1, "opened-folder.png");
            treeImages.Images.SetKeyName(2, "document.png");
            // 
            // StructureTreeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.Name = "StructureTreeControl";
            this.Size = new System.Drawing.Size(300, 450);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        internal System.Windows.Forms.TreeView treeStructure;

    }
}
