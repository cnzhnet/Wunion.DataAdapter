namespace Wunion.DataAdapter.EntityGenerator.Views
{
    partial class BuildingDialogForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BuildingDialogForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.progressBar1 = new Wunion.DataAdapter.EntityGenerator.Views.AnnulusProgressBar();
            this.BuildingDlgTitle = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rich_Logs = new System.Windows.Forms.RichTextBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(130)))), ((int)(((byte)(90)))));
            this.panel1.Controls.Add(this.progressBar1);
            this.panel1.Controls.Add(this.BuildingDlgTitle);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(425, 113);
            this.panel1.TabIndex = 0;
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.progressBar1.BackColor = System.Drawing.Color.Transparent;
            this.progressBar1.DisplayProgressText = true;
            this.progressBar1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.progressBar1.ForeColor = System.Drawing.Color.White;
            this.progressBar1.Location = new System.Drawing.Point(42, 23);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.ProgressColor = System.Drawing.Color.White;
            this.progressBar1.ProgressValue = 0.2F;
            this.progressBar1.Size = new System.Drawing.Size(67, 67);
            this.progressBar1.TabIndex = 2;
            this.progressBar1.Text = "annulusProgressBar1";
            this.progressBar1.WideColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(107)))), ((int)(((byte)(65)))));
            this.progressBar1.ZoneWide = 3F;
            // 
            // lab_Title
            // 
            this.BuildingDlgTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.BuildingDlgTitle.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BuildingDlgTitle.ForeColor = System.Drawing.Color.White;
            this.BuildingDlgTitle.Location = new System.Drawing.Point(115, 11);
            this.BuildingDlgTitle.Name = "BuildingDlgTitle";
            this.BuildingDlgTitle.Size = new System.Drawing.Size(262, 91);
            this.BuildingDlgTitle.TabIndex = 1;
            this.BuildingDlgTitle.Text = "正在生成实体及代理类，请稍候......";
            this.BuildingDlgTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rich_Logs);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 113);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(8);
            this.panel2.Size = new System.Drawing.Size(425, 287);
            this.panel2.TabIndex = 1;
            // 
            // rich_Logs
            // 
            this.rich_Logs.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rich_Logs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rich_Logs.Location = new System.Drawing.Point(8, 8);
            this.rich_Logs.Name = "rich_Logs";
            this.rich_Logs.ReadOnly = true;
            this.rich_Logs.Size = new System.Drawing.Size(409, 271);
            this.rich_Logs.TabIndex = 0;
            this.rich_Logs.Text = "";
            // 
            // BuildingDialogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(425, 400);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BuildingDialogForm";
            this.ShowIcon = false;
            this.Text = "正在生成";
            this.MaximizeBox = false;
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label BuildingDlgTitle;
        private System.Windows.Forms.RichTextBox rich_Logs;
        private AnnulusProgressBar progressBar1;
    }
}