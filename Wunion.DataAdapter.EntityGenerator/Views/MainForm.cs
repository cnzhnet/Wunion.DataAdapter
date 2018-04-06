using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.Kernel.DbInterop;
using Wunion.DataAdapter.Kernel.CommandParser;
using Wunion.DataAdapter.Kernel.SQLServer;
using Wunion.DataAdapter.Kernel.SQLServer.CommandParser;
using Wunion.DataAdapter.EntityUtils.CodeProvider;
using Wunion.DataAdapter.EntityGenerator.Services;

namespace Wunion.DataAdapter.EntityGenerator.Views
{
    public partial class MainForm : BaseForm
    {
        private GeneratorService codeService;

        public MainForm()
        {
            InitializeComponent();
            cmb_databaseType.SelectedIndex = 0;
            //txt_connection.Text = "Server=192.168.1.13;Database=Wunion.Azure.Budget.Resource;User ID=sa;Password=wunion.net;";
            txt_OutputDir.Text = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Output");
        }

        public MainForm(LanguageService language)
        {
            Language = language;
            InitializeComponent();
            cmb_databaseType.SelectedIndex = 0;
            //txt_connection.Text = "Server=192.168.1.13;Database=Wunion.Azure.Budget.Resource;User ID=sa;Password=wunion.net;";
            txt_OutputDir.Text = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Output");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DesignMode)
                return;
        }

        private void InitializeDataEngine()
        {
            if (string.IsNullOrEmpty(txt_connection.Text))
            {
                MessageBox.Show(Language.GetString("NullDbConnection"), 
                    Language.GetString("MsgBoxErrorTitle"), 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                DbAccess DBA;
                ParserAdapter parserAdapter;
                switch (cmb_databaseType.SelectedItem as string)
                {
                    case "Microsoft SQL Server":
                        DBA = new SqlServerDbAccess();
                        DBA.ConnectionString = txt_connection.Text.Trim();
                        parserAdapter = new SqlServerParserAdapter();
                        codeService = new GeneratorService(DBA, parserAdapter);
                        codeService.DbContext = new SqlServerDbContext(codeService.DbEngine);
                        break;
                    case "SQLite3":
                        DBA = new Wunion.DataAdapter.Kernel.SQLite3.SqliteDbAccess();
                        DBA.ConnectionString = txt_connection.Text.Trim();
                        parserAdapter = new Wunion.DataAdapter.Kernel.SQLite3.CommandParser.SqliteParserAdapter();
                        codeService = new GeneratorService(DBA, parserAdapter);
                        codeService.DbContext = new SQLite3DbContext(codeService.DbEngine);
                        break;
                    case "PostgreSQL":
                        DBA = new Wunion.DataAdapter.Kernel.PostgreSQL.NpgsqlDbAccess();
                        DBA.ConnectionString = txt_connection.Text.Trim();
                        parserAdapter = new Wunion.DataAdapter.Kernel.PostgreSQL.CommandParser.NpgsqlParserAdapter();
                        codeService = new GeneratorService(DBA, parserAdapter);
                        codeService.DbContext = new PostgreSQLDbContext(codeService.DbEngine);
                        break;
                    case "MySQL":
                        DBA = new Wunion.DataAdapter.Kernel.MySQL.MySqlDBAccess();
                        DBA.ConnectionString = txt_connection.Text.Trim();
                        parserAdapter = new Wunion.DataAdapter.Kernel.MySQL.CommandParser.MySqlParserAdapter();
                        codeService = new GeneratorService(DBA, parserAdapter);
                        codeService.DbContext = new MySQLDbContext(codeService.DbEngine);
                        break;
                }
                codeService.Language = Language;
                dataGridView1.DataSource = new List<TableInfoModel>();
                dataGridView1.DataSource = codeService.AllTables.Distinct(p => p.tableName).ToList();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, Language.GetString("MsgBoxErrorTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 全选
        /// </summary>
        private void SelectAll()
        {
            List<TableInfoModel> tables = (List<TableInfoModel>)dataGridView1.DataSource;
            if (tables == null)
                return;

            foreach (TableInfoModel info in tables)
                info.Checked = true;
            dataGridView1.Invalidate();
        }

        /// <summary>
        /// 反选
        /// </summary>
        private void InvertSelection()
        {
            List<TableInfoModel> tables = (List<TableInfoModel>)dataGridView1.DataSource;
            if (tables == null)
                return;

            foreach (TableInfoModel info in tables)
                info.Checked = !(info.Checked);
            dataGridView1.Invalidate();
        }

        /// <summary>
        /// 浏览输出目录 
        /// </summary>
        private void BrowseOutput()
        {
            using (FolderBrowserDialog browserDialog = new FolderBrowserDialog())
            {
                browserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
                DialogResult result = browserDialog.ShowDialog(this);
                switch (result)
                {
                    case DialogResult.Yes:
                    case DialogResult.OK:
                        txt_OutputDir.Text = browserDialog.SelectedPath;
                        break;
                }
            }
        }

        private void BuildCode(GeneratorService.BuildPattern pattern)
        {
            if (codeService == null)
            {
                MessageBox.Show(Language.GetString("NullCodeService"), Language.GetString("MsgBoxErrorTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrEmpty(txt_OutputDir.Text))
            {
                MessageBox.Show(Language.GetString("NullOutputDir"), Language.GetString("MsgBoxErrorTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrEmpty(txt_namespace.Text))
            {
                MessageBox.Show(Language.GetString("NullCodeNamespace"), Language.GetString("MsgBoxErrorTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            List<TableInfoModel> tables = (List<TableInfoModel>)dataGridView1.DataSource;
            if (tables == null || tables.Count < 1)
            {
                MessageBox.Show(Language.GetString("NoTableAvailable"), Language.GetString("MsgBoxInfoTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            List<TableInfoModel> selection = tables.Where(item => item.Checked == true).ToList();
            if (selection == null || selection.Count < 1)
            {
                MessageBox.Show(Language.GetString("NoTableSelected"), Language.GetString("MsgBoxInfoTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            codeService.OutputDir = txt_OutputDir.Text.Trim();
            codeService.Pattern = pattern;
            using (BuildingDialogForm buildingDialog = new BuildingDialogForm(codeService))
            {
                buildingDialog.Language = Language;
                buildingDialog.codeNamespace = txt_namespace.Text.Trim();
                buildingDialog.BuildTables = selection;
                buildingDialog.StartPosition = FormStartPosition.CenterParent;
                buildingDialog.ShowDialog(this);
            }
        }

        /// <summary>
        /// 保存注释信息.
        /// </summary>
        private void SaveComments()
        {
            if (codeService == null)
                return;
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Title = Language.GetString("CommentsSaveTitle");
            saveFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            saveFile.Filter = Language.GetString("CommentsFileFilter");
            saveFile.DefaultExt = ".xml";
            saveFile.AddExtension = true;
            try
            {
                DialogResult Okay = saveFile.ShowDialog(this);
                switch (Okay)
                {
                    case DialogResult.OK:
                    case DialogResult.Yes:
                        codeService.SaveComments(saveFile.FileName);
                        break;
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, Language.GetString("MsgBoxErrorTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            saveFile.Dispose();
        }

        /// <summary>
        /// 加载注释信息.
        /// </summary>
        private void LoadComments()
        {
            if (codeService == null)
                return;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = Language.GetString("CommentsLoadTitle");
            fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            fileDialog.Filter = Language.GetString("CommentsFileFilter");
            fileDialog.DefaultExt = ".xml";
            fileDialog.Multiselect = false;
            try
            {
                switch (fileDialog.ShowDialog(this))
                {
                    case DialogResult.OK:
                    case DialogResult.Yes:
                        codeService.LoadComments(fileDialog.FileName);
                        dataGridView1.Refresh();
                        break;
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, Language.GetString("MsgBoxErrorTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            fileDialog.Dispose();
        }

        private void Button_OnClick(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null)
                return;
            switch (btn.Name)
            {
                case "btn_connect":
                    InitializeDataEngine();
                    break;
                case "btn_outPut":
                    BrowseOutput();
                    break;
                case "btn_selectAll":
                    SelectAll();
                    break;
                case "btn_InvertSelection":
                    InvertSelection();
                    break;
                case "btn_buildAll":
                    BuildCode(GeneratorService.BuildPattern.BuildAll);
                    break;
                case "btn_buildEntity":
                    BuildCode(GeneratorService.BuildPattern.BuildEntity);
                    break;
                case "btn_buildAgent":
                    BuildCode(GeneratorService.BuildPattern.BuildAgent);
                    break;
                case "btn_saveComments":
                    SaveComments();
                    break;
                case "btn_loadComments":
                    LoadComments();
                    break;
            }
        }

        private void smi_viewTable_Click(object sender, EventArgs e)
        {
            if (codeService == null)
                return;
            if (dataGridView1.SelectedRows.Count < 1)
                return;
            int currentIndex = dataGridView1.SelectedRows[0].Index;
            List<TableInfoModel> tables = (List<TableInfoModel>)dataGridView1.DataSource;
            TableInfoModel currentTable = tables[currentIndex];
            using (TableColumnsForm columnsView = new TableColumnsForm(codeService.AllTables, currentTable))
            {
                columnsView.Language = Language;
                columnsView.StartPosition = FormStartPosition.CenterParent;
                columnsView.ShowDialog(this);
            }
        }

        private void Button_MouseHover(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
                mainToolTip.Show(btn.Text, btn);
        }

        private void Button_MouseLeave(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
                mainToolTip.Hide(btn);
        }
    }
}
