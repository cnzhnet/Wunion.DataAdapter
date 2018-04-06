using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wunion.DataAdapter.EntityUtils.CodeProvider;
using Wunion.DataAdapter.EntityGenerator.Services;

namespace Wunion.DataAdapter.EntityGenerator.Views
{
    /// <summary>
    /// 代码生成进度对话框.
    /// </summary>
    public partial class BuildingDialogForm : BaseForm
    {
        private GeneratorService codeService;
        private List<TableInfoModel> _BuildTables;
        private string _codeNamespace;
        private bool IsBuilding;

        /// <summary>
        /// 创建一个 <see cref="BuildingDialogForm"/> 对话框实例.
        /// </summary>
        /// <param name="service">代码生成服务.</param>
        public BuildingDialogForm(GeneratorService service)
        {
            InitializeComponent();

            codeService = service;
            IsBuilding = false;
        }

        /// <summary>
        /// 设置要生成的表.
        /// </summary>
        public List<TableInfoModel> BuildTables
        {
            set { _BuildTables = value; }
        }

        /// <summary>
        /// 设置生成的代码命名空间.
        /// </summary>
        public string codeNamespace
        {
            set { _codeNamespace = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DesignMode)
                return;

            switch (codeService.Pattern)
            {
                case GeneratorService.BuildPattern.BuildEntity:
                    BuildingDlgTitle.Text = Language.GetString("BuildingEntiryTitle");
                    break;
                case GeneratorService.BuildPattern.BuildAgent:
                    BuildingDlgTitle.Text = Language.GetString("BuildingAgentTitle");
                    break;
                default:
                    BuildingDlgTitle.Text = Language.GetString("BuildingAllTitle");
                    break;
            }
            InvokeBuildCode();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            e.Cancel = IsBuilding;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            if (codeService != null)
                codeService.ProgressChange -= CodeService_ProgressChange;
        }

        /// <summary>
        /// 调用代码生成.
        /// </summary>
        private void InvokeBuildCode()
        {
            if (IsBuilding)
                return;
            IsBuilding = true;
            codeService.ProgressChange -= CodeService_ProgressChange;
            codeService.ProgressChange += CodeService_ProgressChange;
            Task buildTask = Task.Factory.StartNew(() => {
                try
                {
                    codeService.BuildTo(_codeNamespace, _BuildTables);
                    CodeService_ProgressChange(100, Language.GetString("BuildingCompleted"));
                    Invoke(new MethodInvoker(() => { BuildingDlgTitle.Text = Language.GetString("BuildingCompleted"); }));
                }
                catch (Exception Ex)
                {
                    Invoke(new MethodInvoker(() => {
                        BuildingDlgTitle.Text = Language.GetString("BuildingError");
                        rich_Logs.Text += Ex.Message;
                    }));
                }
                IsBuilding = false;
            });
        }

        /// <summary>
        /// 代码生成服务的进度提醒事件处理.
        /// </summary>
        /// <param name="percentage">进度百分比值.</param>
        /// <param name="message">进度相关的消息.</param>
        private void CodeService_ProgressChange(int percentage, string message)
        {
            Invoke(new MethodInvoker(() => {
                progressBar1.ProgressValue = percentage / 100.0f;
                rich_Logs.Text += string.Format("{0}\r\n", message);
            }));
        }
    }
}
