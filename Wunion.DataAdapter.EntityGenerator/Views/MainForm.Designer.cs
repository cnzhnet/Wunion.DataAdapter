namespace Wunion.DataAdapter.EntityGenerator.Views
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.top_panel = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.ToolDescription = new Wunion.DataAdapter.EntityGenerator.Views.LingLabel(this.components);
            this.CopyrightText = new System.Windows.Forms.Label();
            this.CaptionTitle = new System.Windows.Forms.Label();
            this.GroupBoxOptions = new System.Windows.Forms.GroupBox();
            this.btn_outPut = new System.Windows.Forms.Button();
            this.txt_OutputDir = new System.Windows.Forms.TextBox();
            this.LabelOutputDir = new System.Windows.Forms.Label();
            this.txt_namespace = new System.Windows.Forms.TextBox();
            this.LabelNamespace = new System.Windows.Forms.Label();
            this.btn_connect = new System.Windows.Forms.Button();
            this.txt_connection = new System.Windows.Forms.TextBox();
            this.LabelDbConnection = new System.Windows.Forms.Label();
            this.cmb_databaseType = new System.Windows.Forms.ComboBox();
            this.LabelDatabase = new System.Windows.Forms.Label();
            this.GroupBoxTables = new System.Windows.Forms.GroupBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.smi_viewTable = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btn_loadComments = new System.Windows.Forms.Button();
            this.btn_saveComments = new System.Windows.Forms.Button();
            this.btn_buildAgent = new System.Windows.Forms.Button();
            this.btn_buildEntity = new System.Windows.Forms.Button();
            this.btn_buildAll = new System.Windows.Forms.Button();
            this.btn_InvertSelection = new System.Windows.Forms.Button();
            this.btn_selectAll = new System.Windows.Forms.Button();
            this.DataGridColumnChoose = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.DataGridColumnTabName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DataGridColumnTabDescr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DataGridColumnFieldName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DataGridColumnPrimaryKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DataGridColumnIdentity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DataGridColumnDataType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DataGridColumnAllowNull = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DataGridColumnDefaultVal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DataGridColumnFieldDescr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mainToolTip = new System.Windows.Forms.ToolTip();
            this.top_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.GroupBoxOptions.SuspendLayout();
            this.GroupBoxTables.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // top_panel
            // 
            this.top_panel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(87)))), ((int)(((byte)(154)))));
            this.top_panel.Controls.Add(this.pictureBox1);
            this.top_panel.Controls.Add(this.ToolDescription);
            this.top_panel.Controls.Add(this.CopyrightText);
            this.top_panel.Controls.Add(this.CaptionTitle);
            this.top_panel.Dock = System.Windows.Forms.DockStyle.Top;
            this.top_panel.Location = new System.Drawing.Point(0, 0);
            this.top_panel.Name = "top_panel";
            this.top_panel.Size = new System.Drawing.Size(744, 82);
            this.top_panel.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = new System.Drawing.Bitmap(new System.IO.MemoryStream(global::Wunion.DataAdapter.EntityGenerator.Properties.Resources.CodeBuilder));
            this.pictureBox1.Location = new System.Drawing.Point(27, 23);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(43, 38);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // ToolDescription
            // 
            this.ToolDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ToolDescription.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(87)))), ((int)(((byte)(154)))));
            this.ToolDescription.DisplayBorder = false;
            this.ToolDescription.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ToolDescription.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ToolDescription.LineDistance = 2;
            this.ToolDescription.Location = new System.Drawing.Point(342, 4);
            this.ToolDescription.Name = "ToolDescription";
            this.ToolDescription.Size = new System.Drawing.Size(399, 72);
            this.ToolDescription.TabIndex = 3;
            this.ToolDescription.Text = "若在设计数据库时没有为表或字段设置备注信息，可以在“注释”一列为生成的代码添加注释信息，同时还可根据需要保存或加载它们.";
            this.ToolDescription.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CopyrightText
            // 
            this.CopyrightText.AutoSize = true;
            this.CopyrightText.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CopyrightText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.CopyrightText.Location = new System.Drawing.Point(75, 45);
            this.CopyrightText.Name = "CopyrightText";
            this.CopyrightText.Size = new System.Drawing.Size(176, 17);
            this.CopyrightText.TabIndex = 1;
            this.CopyrightText.Text = "Copyright (C) 巽翎君保留所有权利.";
            // 
            // CaptionTitle
            // 
            this.CaptionTitle.AutoSize = true;
            this.CaptionTitle.BackColor = System.Drawing.Color.Transparent;
            this.CaptionTitle.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CaptionTitle.ForeColor = System.Drawing.Color.White;
            this.CaptionTitle.Location = new System.Drawing.Point(73, 19);
            this.CaptionTitle.Name = "CaptionTitle";
            this.CaptionTitle.Size = new System.Drawing.Size(107, 25);
            this.CaptionTitle.TabIndex = 0;
            this.CaptionTitle.Text = "代码生成器";
            // 
            // GroupBoxOptions
            // 
            this.GroupBoxOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupBoxOptions.Controls.Add(this.btn_outPut);
            this.GroupBoxOptions.Controls.Add(this.txt_OutputDir);
            this.GroupBoxOptions.Controls.Add(this.LabelOutputDir);
            this.GroupBoxOptions.Controls.Add(this.txt_namespace);
            this.GroupBoxOptions.Controls.Add(this.LabelNamespace);
            this.GroupBoxOptions.Controls.Add(this.btn_connect);
            this.GroupBoxOptions.Controls.Add(this.txt_connection);
            this.GroupBoxOptions.Controls.Add(this.LabelDbConnection);
            this.GroupBoxOptions.Controls.Add(this.cmb_databaseType);
            this.GroupBoxOptions.Controls.Add(this.LabelDatabase);
            this.GroupBoxOptions.Location = new System.Drawing.Point(12, 97);
            this.GroupBoxOptions.Name = "GroupBoxOptions";
            this.GroupBoxOptions.Size = new System.Drawing.Size(720, 149);
            this.GroupBoxOptions.TabIndex = 1;
            this.GroupBoxOptions.TabStop = false;
            this.GroupBoxOptions.Text = "代码生成选项";
            // 
            // btn_outPut
            // 
            this.btn_outPut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_outPut.Location = new System.Drawing.Point(623, 114);
            this.btn_outPut.Name = "btn_outPut";
            this.btn_outPut.Size = new System.Drawing.Size(75, 23);
            this.btn_outPut.TabIndex = 9;
            this.btn_outPut.Text = "浏览(&B)";
            this.btn_outPut.UseVisualStyleBackColor = true;
            this.btn_outPut.Click += new System.EventHandler(this.Button_OnClick);
            this.btn_outPut.MouseHover += new System.EventHandler(this.Button_MouseHover);
            this.btn_outPut.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            // 
            // txt_OutputDir
            // 
            this.txt_OutputDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_OutputDir.Location = new System.Drawing.Point(126, 115);
            this.txt_OutputDir.Name = "txt_OutputDir";
            this.txt_OutputDir.Size = new System.Drawing.Size(491, 21);
            this.txt_OutputDir.TabIndex = 8;
            // 
            // LabelOutputDir
            // 
            this.LabelOutputDir.AutoSize = true;
            this.LabelOutputDir.Location = new System.Drawing.Point(33, 120);
            this.LabelOutputDir.Name = "LabelOutputDir";
            this.LabelOutputDir.Size = new System.Drawing.Size(89, 12);
            this.LabelOutputDir.TabIndex = 7;
            this.LabelOutputDir.Text = "代码生成目录：";
            // 
            // txt_namespace
            // 
            this.txt_namespace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_namespace.Location = new System.Drawing.Point(126, 84);
            this.txt_namespace.Name = "txt_namespace";
            this.txt_namespace.Size = new System.Drawing.Size(491, 21);
            this.txt_namespace.TabIndex = 6;
            // 
            // LabelNamespace
            // 
            this.LabelNamespace.AutoSize = true;
            this.LabelNamespace.Location = new System.Drawing.Point(33, 89);
            this.LabelNamespace.Name = "LabelNamespace";
            this.LabelNamespace.Size = new System.Drawing.Size(89, 12);
            this.LabelNamespace.TabIndex = 5;
            this.LabelNamespace.Text = "代码命名空间：";
            // 
            // btn_connect
            // 
            this.btn_connect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_connect.Location = new System.Drawing.Point(623, 51);
            this.btn_connect.Name = "btn_connect";
            this.btn_connect.Size = new System.Drawing.Size(75, 23);
            this.btn_connect.TabIndex = 4;
            this.btn_connect.Text = "连接(&C)";
            this.btn_connect.UseVisualStyleBackColor = true;
            this.btn_connect.Click += new System.EventHandler(this.Button_OnClick);
            this.btn_connect.MouseHover += new System.EventHandler(this.Button_MouseHover);
            this.btn_connect.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            // 
            // txt_connection
            // 
            this.txt_connection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_connection.Location = new System.Drawing.Point(126, 53);
            this.txt_connection.Name = "txt_connection";
            this.txt_connection.Size = new System.Drawing.Size(491, 21);
            this.txt_connection.TabIndex = 3;
            // 
            // LabelDbConnection
            // 
            this.LabelDbConnection.AutoSize = true;
            this.LabelDbConnection.Location = new System.Drawing.Point(33, 59);
            this.LabelDbConnection.Name = "LabelDbConnection";
            this.LabelDbConnection.Size = new System.Drawing.Size(77, 12);
            this.LabelDbConnection.TabIndex = 2;
            this.LabelDbConnection.Text = "数据库连接：";
            // 
            // cmb_databaseType
            // 
            this.cmb_databaseType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_databaseType.FormattingEnabled = true;
            this.cmb_databaseType.Items.AddRange(new object[] {
            "Microsoft SQL Server", "SQLite3", "PostgreSQL", "MySQL" });
            this.cmb_databaseType.Location = new System.Drawing.Point(126, 22);
            this.cmb_databaseType.Name = "cmb_databaseType";
            this.cmb_databaseType.Size = new System.Drawing.Size(174, 20);
            this.cmb_databaseType.TabIndex = 1;
            // 
            // LabelDatabase
            // 
            this.LabelDatabase.AutoSize = true;
            this.LabelDatabase.Location = new System.Drawing.Point(33, 26);
            this.LabelDatabase.Name = "LabelDatabase";
            this.LabelDatabase.Size = new System.Drawing.Size(77, 12);
            this.LabelDatabase.TabIndex = 0;
            this.LabelDatabase.Text = "数据库类型：";
            // 
            // GroupBoxTables
            // 
            this.GroupBoxTables.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupBoxTables.Controls.Add(this.dataGridView1);
            this.GroupBoxTables.Location = new System.Drawing.Point(12, 252);
            this.GroupBoxTables.Name = "GroupBoxTables";
            this.GroupBoxTables.Size = new System.Drawing.Size(720, 337);
            this.GroupBoxTables.TabIndex = 2;
            this.GroupBoxTables.TabStop = false;
            this.GroupBoxTables.Text = "数据库中的所有表";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DataGridColumnChoose,
            this.DataGridColumnTabName,
            this.DataGridColumnTabDescr,
            this.DataGridColumnFieldName,
            this.DataGridColumnPrimaryKey,
            this.DataGridColumnIdentity,
            this.DataGridColumnDataType,
            this.DataGridColumnAllowNull,
            this.DataGridColumnDefaultVal,
            this.DataGridColumnFieldDescr});
            this.dataGridView1.ContextMenuStrip = this.contextMenuStrip1;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 17);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(714, 317);
            this.dataGridView1.TabIndex = 0;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.smi_viewTable});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(113, 26);
            // 
            // smi_viewTable
            // 
            this.smi_viewTable.Name = "smi_viewTable";
            this.smi_viewTable.Size = new System.Drawing.Size(112, 22);
            this.smi_viewTable.Text = "配置列";
            this.smi_viewTable.Click += new System.EventHandler(this.smi_viewTable_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.btn_loadComments);
            this.panel1.Controls.Add(this.btn_saveComments);
            this.panel1.Controls.Add(this.btn_buildAgent);
            this.panel1.Controls.Add(this.btn_buildEntity);
            this.panel1.Controls.Add(this.btn_buildAll);
            this.panel1.Controls.Add(this.btn_InvertSelection);
            this.panel1.Controls.Add(this.btn_selectAll);
            this.panel1.Location = new System.Drawing.Point(12, 595);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(720, 42);
            this.panel1.TabIndex = 3;
            // 
            // btn_loadComments
            // 
            this.btn_loadComments.Location = new System.Drawing.Point(588, 9);
            this.btn_loadComments.Name = "btn_loadComments";
            this.btn_loadComments.Size = new System.Drawing.Size(74, 23);
            this.btn_loadComments.TabIndex = 6;
            this.btn_loadComments.Text = "加载注释";
            this.btn_loadComments.UseVisualStyleBackColor = true;
            this.btn_loadComments.Click += new System.EventHandler(this.Button_OnClick);
            this.btn_loadComments.MouseHover += new System.EventHandler(this.Button_MouseHover);
            this.btn_loadComments.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            // 
            // btn_saveComments
            // 
            this.btn_saveComments.Location = new System.Drawing.Point(508, 9);
            this.btn_saveComments.Name = "btn_saveComments";
            this.btn_saveComments.Size = new System.Drawing.Size(74, 23);
            this.btn_saveComments.TabIndex = 5;
            this.btn_saveComments.Text = "保存注释";
            this.btn_saveComments.UseVisualStyleBackColor = true;
            this.btn_saveComments.Click += new System.EventHandler(this.Button_OnClick);
            this.btn_saveComments.MouseHover += new System.EventHandler(this.Button_MouseHover);
            this.btn_saveComments.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            // 
            // btn_buildAgent
            // 
            this.btn_buildAgent.Location = new System.Drawing.Point(401, 9);
            this.btn_buildAgent.Name = "btn_buildAgent";
            this.btn_buildAgent.Size = new System.Drawing.Size(101, 23);
            this.btn_buildAgent.TabIndex = 4;
            this.btn_buildAgent.Text = "仅生成代理类";
            this.btn_buildAgent.UseVisualStyleBackColor = true;
            this.btn_buildAgent.Click += new System.EventHandler(this.Button_OnClick);
            this.btn_buildAgent.MouseHover += new System.EventHandler(this.Button_MouseHover);
            this.btn_buildAgent.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            // 
            // btn_buildEntity
            // 
            this.btn_buildEntity.Location = new System.Drawing.Point(294, 9);
            this.btn_buildEntity.Name = "btn_buildEntity";
            this.btn_buildEntity.Size = new System.Drawing.Size(101, 23);
            this.btn_buildEntity.TabIndex = 3;
            this.btn_buildEntity.Text = "仅生成实体类";
            this.btn_buildEntity.UseVisualStyleBackColor = true;
            this.btn_buildEntity.Click += new System.EventHandler(this.Button_OnClick);
            this.btn_buildEntity.MouseHover += new System.EventHandler(this.Button_MouseHover);
            this.btn_buildEntity.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            // 
            // btn_buildAll
            // 
            this.btn_buildAll.Location = new System.Drawing.Point(164, 9);
            this.btn_buildAll.Name = "btn_buildAll";
            this.btn_buildAll.Size = new System.Drawing.Size(124, 23);
            this.btn_buildAll.TabIndex = 2;
            this.btn_buildAll.Text = "生成实体及代理类";
            this.btn_buildAll.UseVisualStyleBackColor = true;
            this.btn_buildAll.Click += new System.EventHandler(this.Button_OnClick);
            this.btn_buildAll.MouseHover += new System.EventHandler(this.Button_MouseHover);
            this.btn_buildAll.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            // 
            // btn_InvertSelection
            // 
            this.btn_InvertSelection.Location = new System.Drawing.Point(83, 9);
            this.btn_InvertSelection.Name = "btn_InvertSelection";
            this.btn_InvertSelection.Size = new System.Drawing.Size(75, 23);
            this.btn_InvertSelection.TabIndex = 1;
            this.btn_InvertSelection.Text = "反选";
            this.btn_InvertSelection.UseVisualStyleBackColor = true;
            this.btn_InvertSelection.Click += new System.EventHandler(this.Button_OnClick);
            this.btn_InvertSelection.MouseHover += new System.EventHandler(this.Button_MouseHover);
            this.btn_InvertSelection.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            // 
            // btn_selectAll
            // 
            this.btn_selectAll.Location = new System.Drawing.Point(1, 9);
            this.btn_selectAll.Name = "btn_selectAll";
            this.btn_selectAll.Size = new System.Drawing.Size(75, 23);
            this.btn_selectAll.TabIndex = 0;
            this.btn_selectAll.Text = "全选";
            this.btn_selectAll.UseVisualStyleBackColor = true;
            this.btn_selectAll.Click += new System.EventHandler(this.Button_OnClick);
            this.btn_selectAll.MouseHover += new System.EventHandler(this.Button_MouseHover);
            this.btn_selectAll.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            // 
            // DataGridColumnChoose
            // 
            this.DataGridColumnChoose.DataPropertyName = "Checked";
            this.DataGridColumnChoose.HeaderText = "选择";
            this.DataGridColumnChoose.Name = "DataGridColumnChoose";
            this.DataGridColumnChoose.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.DataGridColumnChoose.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.DataGridColumnChoose.Width = 40;
            // 
            // DataGridColumnTabName
            // 
            this.DataGridColumnTabName.DataPropertyName = "tableName";
            this.DataGridColumnTabName.HeaderText = "表名称";
            this.DataGridColumnTabName.Name = "DataGridColumnTabName";
            this.DataGridColumnTabName.ReadOnly = true;
            this.DataGridColumnTabName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.DataGridColumnTabName.Width = 200;
            // 
            // DataGridColumnTabDescr
            // 
            this.DataGridColumnTabDescr.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.DataGridColumnTabDescr.DataPropertyName = "tableDescription";
            this.DataGridColumnTabDescr.HeaderText = "注释";
            this.DataGridColumnTabDescr.Name = "DataGridColumnTabDescr";
            // 
            // DataGridColumnFieldName
            // 
            this.DataGridColumnFieldName.DataPropertyName = "paramName";
            this.DataGridColumnFieldName.HeaderText = "列名称";
            this.DataGridColumnFieldName.Name = "DataGridColumnFieldName";
            this.DataGridColumnFieldName.ReadOnly = true;
            this.DataGridColumnFieldName.Visible = false;
            // 
            // DataGridColumnPrimaryKey
            // 
            this.DataGridColumnPrimaryKey.DataPropertyName = "isPrimary";
            this.DataGridColumnPrimaryKey.HeaderText = "主键";
            this.DataGridColumnPrimaryKey.Name = "DataGridColumnPrimaryKey";
            this.DataGridColumnPrimaryKey.ReadOnly = true;
            this.DataGridColumnPrimaryKey.Visible = false;
            // 
            // DataGridColumnIdentity
            // 
            this.DataGridColumnIdentity.DataPropertyName = "isIdentity";
            this.DataGridColumnIdentity.HeaderText = "自动编号";
            this.DataGridColumnIdentity.Name = "DataGridColumnIdentity";
            this.DataGridColumnIdentity.ReadOnly = true;
            this.DataGridColumnIdentity.Visible = false;
            // 
            // DataGridColumnDataType
            // 
            this.DataGridColumnDataType.DataPropertyName = "dbType";
            this.DataGridColumnDataType.HeaderText = "数据类型";
            this.DataGridColumnDataType.Name = "DataGridColumnDataType";
            this.DataGridColumnDataType.ReadOnly = true;
            this.DataGridColumnDataType.Visible = false;
            // 
            // DataGridColumnAllowNull
            // 
            this.DataGridColumnAllowNull.DataPropertyName = "allowNull";
            this.DataGridColumnAllowNull.HeaderText = "允许空值";
            this.DataGridColumnAllowNull.Name = "DataGridColumnAllowNull";
            this.DataGridColumnAllowNull.ReadOnly = true;
            this.DataGridColumnAllowNull.Visible = false;
            // 
            // DataGridColumnDefaultVal
            // 
            this.DataGridColumnDefaultVal.DataPropertyName = "defaultValue";
            this.DataGridColumnDefaultVal.HeaderText = "默认值";
            this.DataGridColumnDefaultVal.Name = "DataGridColumnDefaultVal";
            this.DataGridColumnDefaultVal.ReadOnly = true;
            this.DataGridColumnDefaultVal.Visible = false;
            // 
            // DataGridColumnFieldDescr
            // 
            this.DataGridColumnFieldDescr.DataPropertyName = "paramDescription";
            this.DataGridColumnFieldDescr.HeaderText = "列说明";
            this.DataGridColumnFieldDescr.Name = "DataGridColumnFieldDescr";
            this.DataGridColumnFieldDescr.ReadOnly = true;
            this.DataGridColumnFieldDescr.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(744, 641);
            this.MinimumSize = new System.Drawing.Size(744, 641);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.GroupBoxTables);
            this.Controls.Add(this.GroupBoxOptions);
            this.Controls.Add(this.top_panel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Wunion.DataAdapter.NetCore.EntityUtils 代码生成工具";
            this.top_panel.ResumeLayout(false);
            this.top_panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.GroupBoxOptions.ResumeLayout(false);
            this.GroupBoxOptions.PerformLayout();
            this.GroupBoxTables.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel top_panel;
        private System.Windows.Forms.Label CopyrightText;
        private System.Windows.Forms.Label CaptionTitle;
        private System.Windows.Forms.GroupBox GroupBoxOptions;
        private System.Windows.Forms.Button btn_connect;
        private System.Windows.Forms.TextBox txt_connection;
        private System.Windows.Forms.Label LabelDbConnection;
        private System.Windows.Forms.ComboBox cmb_databaseType;
        private System.Windows.Forms.Label LabelDatabase;
        private System.Windows.Forms.TextBox txt_namespace;
        private System.Windows.Forms.Label LabelNamespace;
        private System.Windows.Forms.TextBox txt_OutputDir;
        private System.Windows.Forms.Label LabelOutputDir;
        private System.Windows.Forms.GroupBox GroupBoxTables;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btn_buildAgent;
        private System.Windows.Forms.Button btn_buildEntity;
        private System.Windows.Forms.Button btn_buildAll;
        private System.Windows.Forms.Button btn_InvertSelection;
        private System.Windows.Forms.Button btn_selectAll;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem smi_viewTable;
        private System.Windows.Forms.Button btn_outPut;
        private System.Windows.Forms.Button btn_saveComments;
        private System.Windows.Forms.Button btn_loadComments;
        private LingLabel ToolDescription;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn DataGridColumnChoose;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataGridColumnTabName;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataGridColumnTabDescr;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataGridColumnFieldName;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataGridColumnPrimaryKey;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataGridColumnIdentity;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataGridColumnDataType;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataGridColumnAllowNull;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataGridColumnDefaultVal;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataGridColumnFieldDescr;
        private System.Windows.Forms.ToolTip mainToolTip;
    }
}

