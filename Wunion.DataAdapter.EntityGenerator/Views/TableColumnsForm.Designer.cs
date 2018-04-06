namespace Wunion.DataAdapter.EntityGenerator.Views
{
    partial class TableColumnsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TableColumnsForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.ColumnsDlgDescription = new System.Windows.Forms.Label();
            this.ColumnsDlgTableName = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.DataGridColumnChoose = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DataGridColumnTabName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DataGridColumnTabDescr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DataGridColumnFieldName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DataGridColumnDataType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DataGridColumnPrimaryKey = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.DataGridColumnIdentity = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.DataGridColumnAllowNull = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.DataGridColumnDefaultVal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DataGridColumnFieldDescr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(130)))), ((int)(((byte)(90)))));
            this.panel1.Controls.Add(this.ColumnsDlgDescription);
            this.panel1.Controls.Add(this.ColumnsDlgTableName);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(531, 72);
            this.panel1.TabIndex = 0;
            // 
            // label2
            // 
            this.ColumnsDlgDescription.AutoSize = true;
            this.ColumnsDlgDescription.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ColumnsDlgDescription.Location = new System.Drawing.Point(17, 41);
            this.ColumnsDlgDescription.Name = "ColumnsDlgDescription";
            this.ColumnsDlgDescription.Size = new System.Drawing.Size(317, 12);
            this.ColumnsDlgDescription.TabIndex = 1;
            this.ColumnsDlgDescription.Text = "在此，您可以为该表的字段生成的实体属性编写注释信息。";
            // 
            // lab_currentTable
            // 
            this.ColumnsDlgTableName.AutoSize = true;
            this.ColumnsDlgTableName.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ColumnsDlgTableName.ForeColor = System.Drawing.Color.White;
            this.ColumnsDlgTableName.Location = new System.Drawing.Point(13, 16);
            this.ColumnsDlgTableName.Name = "ColumnsDlgTableName";
            this.ColumnsDlgTableName.Size = new System.Drawing.Size(115, 21);
            this.ColumnsDlgTableName.TabIndex = 0;
            this.ColumnsDlgTableName.Text = "Tablename 表";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DataGridColumnChoose,
            this.DataGridColumnTabName,
            this.DataGridColumnTabDescr,
            this.DataGridColumnFieldName,
            this.DataGridColumnDataType,
            this.DataGridColumnPrimaryKey,
            this.DataGridColumnIdentity,
            this.DataGridColumnAllowNull,
            this.DataGridColumnDefaultVal,
            this.DataGridColumnFieldDescr});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(4, 4);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(523, 366);
            this.dataGridView1.TabIndex = 1;
            // 
            // COL_CHECKED
            // 
            this.DataGridColumnChoose.DataPropertyName = "Checked";
            this.DataGridColumnChoose.HeaderText = "选定";
            this.DataGridColumnChoose.Name = "DataGridColumnChoose";
            this.DataGridColumnChoose.ReadOnly = true;
            this.DataGridColumnChoose.Visible = false;
            // 
            // COL_TABLE_NAME
            // 
            this.DataGridColumnTabName.DataPropertyName = "tableName";
            this.DataGridColumnTabName.HeaderText = "表名";
            this.DataGridColumnTabName.Name = "DataGridColumnTabName";
            this.DataGridColumnTabName.ReadOnly = true;
            this.DataGridColumnTabName.Visible = false;
            // 
            // COL_TABLE_DESCR
            // 
            this.DataGridColumnTabDescr.DataPropertyName = "tableDescription";
            this.DataGridColumnTabDescr.HeaderText = "说明";
            this.DataGridColumnTabDescr.Name = "DataGridColumnTabDescr";
            this.DataGridColumnTabDescr.ReadOnly = true;
            this.DataGridColumnTabDescr.Visible = false;
            // 
            // COL_PARAM_NAME
            // 
            this.DataGridColumnFieldName.DataPropertyName = "paramName";
            this.DataGridColumnFieldName.HeaderText = "列名";
            this.DataGridColumnFieldName.Name = "DataGridColumnFieldName";
            this.DataGridColumnFieldName.ReadOnly = true;
            this.DataGridColumnFieldName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // COL_DBTYPE
            // 
            this.DataGridColumnDataType.DataPropertyName = "dbType";
            this.DataGridColumnDataType.HeaderText = "数据类型";
            this.DataGridColumnDataType.Name = "DataGridColumnDataType";
            this.DataGridColumnDataType.ReadOnly = true;
            this.DataGridColumnDataType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // COL_IS_Primary
            // 
            this.DataGridColumnPrimaryKey.DataPropertyName = "isPrimary";
            this.DataGridColumnPrimaryKey.HeaderText = "主键";
            this.DataGridColumnPrimaryKey.Name = "DataGridColumnPrimaryKey";
            this.DataGridColumnPrimaryKey.ReadOnly = true;
            this.DataGridColumnPrimaryKey.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.DataGridColumnPrimaryKey.Width = 40;
            // 
            // COL_IS_Identity
            // 
            this.DataGridColumnIdentity.DataPropertyName = "isIdentity";
            this.DataGridColumnIdentity.HeaderText = "自增长";
            this.DataGridColumnIdentity.Name = "DataGridColumnIdentity";
            this.DataGridColumnIdentity.ReadOnly = true;
            this.DataGridColumnIdentity.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.DataGridColumnIdentity.Width = 50;
            // 
            // COLL_ALLOW_NULL
            // 
            this.DataGridColumnAllowNull.DataPropertyName = "allowNull";
            this.DataGridColumnAllowNull.HeaderText = "允许空值";
            this.DataGridColumnAllowNull.Name = "DataGridColumnAllowNull";
            this.DataGridColumnAllowNull.ReadOnly = true;
            this.DataGridColumnAllowNull.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.DataGridColumnAllowNull.Width = 60;
            // 
            // COL_DEFAULT_VALUE
            // 
            this.DataGridColumnDefaultVal.DataPropertyName = "defaultValue";
            this.DataGridColumnDefaultVal.HeaderText = "默认值";
            this.DataGridColumnDefaultVal.Name = "DataGridColumnDefaultVal";
            this.DataGridColumnDefaultVal.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.DataGridColumnDefaultVal.Width = 50;
            // 
            // COL_PARAM_DESCR
            // 
            this.DataGridColumnFieldDescr.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.DataGridColumnFieldDescr.DataPropertyName = "paramDescription";
            this.DataGridColumnFieldDescr.HeaderText = "注释";
            this.DataGridColumnFieldDescr.Name = "DataGridColumnFieldDescr";
            this.DataGridColumnFieldDescr.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.dataGridView1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 72);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(4);
            this.panel2.Size = new System.Drawing.Size(531, 374);
            this.panel2.TabIndex = 2;
            // 
            // TableColumnsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(531, 446);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TableColumnsForm";
            this.Text = "数据表字段信息";
            this.MaximizeBox = false;
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label ColumnsDlgTableName;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label ColumnsDlgDescription;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataGridColumnChoose;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataGridColumnTabName;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataGridColumnTabDescr;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataGridColumnFieldName;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataGridColumnDataType;
        private System.Windows.Forms.DataGridViewCheckBoxColumn DataGridColumnPrimaryKey;
        private System.Windows.Forms.DataGridViewCheckBoxColumn DataGridColumnIdentity;
        private System.Windows.Forms.DataGridViewCheckBoxColumn DataGridColumnAllowNull;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataGridColumnDefaultVal;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataGridColumnFieldDescr;
    }
}