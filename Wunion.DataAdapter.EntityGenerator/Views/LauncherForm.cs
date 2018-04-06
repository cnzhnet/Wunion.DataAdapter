using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wunion.DataAdapter.EntityGenerator.Services;

namespace Wunion.DataAdapter.EntityGenerator.Views
{
    /// <summary>
    /// 启动器对话框.
    /// </summary>
    public partial class LauncherForm : Form
    {
        private LanguageService _Language;
        private List<LanguageService> Services;

        /// <summary>
        /// 创建一个启动器对话框。
        /// </summary>
        public LauncherForm(List<LanguageService> languages)
        {
            Services = languages;
            InitializeComponent();
            listBox1.SelectedIndex = -1;
            listBox1.SelectedIndexChanged += ListBox_SelectedIndexChanged;
        }

        /// <summary>
        /// 获取启动器中选择的语言.
        /// </summary>
        public LanguageService Language => _Language;

        private void ListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LanguageService service = listBox1.SelectedItem as LanguageService;
            if (service == null)
                return;

            _Language = service;
            this.Close();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DesignMode)
                return;

            listBox1.Items.Clear();
            for (int i = 0; i < Services.Count; ++i)
                listBox1.Items.Add(Services[i]);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            e.Cancel = Language == null;
        }
    }
}
