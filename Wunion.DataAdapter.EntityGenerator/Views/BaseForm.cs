using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wunion.DataAdapter.EntityGenerator.Services;

namespace Wunion.DataAdapter.EntityGenerator.Views
{
    /// <summary>
    /// 所有生成器窗口的基类
    /// </summary>
    public abstract class BaseForm : Form
    {
        /// <summary>
        /// 创建一个 <see cref="BaseForm"/> 的对象类型.
        /// </summary>
        protected BaseForm()
        { }

        /// <summary>
        /// 获取或设置当前的语言服务.
        /// </summary>
        public LanguageService Language { get; set; }

        /// <summary>
        /// 为窗口应用当前的语言.
        /// </summary>
        public void ApplyLanguage()
        {
            if (Language == null)
                return;
            FieldInfo field;
            Type t;
            object ctrl;
            foreach (LocaleResourceItem item in Language.GetUiResources())
            {
                t = this.GetType();
                field = t.GetField(item.Key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field == null)
                    continue;
                ctrl = field.GetValue(this);
                if (ctrl != null)
                    ApplyLocaleResourceItem(ctrl, item);
            }
        }

        /// <summary>
        /// 为 UI 应用语言环境中的项.
        /// </summary>
        /// <param name="control">为该控件应用语言项.</param>
        /// <param name="item">语言项.</param>
        protected virtual void ApplyLocaleResourceItem(object control, LocaleResourceItem item)
        {
            DataGridViewColumn gridViewColumn = control as DataGridViewColumn;
            if (gridViewColumn != null)
            {
                gridViewColumn.HeaderText = item.Value;
                gridViewColumn.ToolTipText = item.Value;
                return;
            }
            ToolStripItem toolStrip = control as ToolStripItem;
            if (toolStrip != null)
            {
                toolStrip.Text = item.Value;
                toolStrip.ToolTipText = item.Value;
                return;
            }
            Control c = control as Control;
            if (c != null)
                c.Text = item.Value;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DesignMode)
                return;
            string TextBuffer = Language.GetString(this.GetType().Name);
            if (!string.IsNullOrEmpty(TextBuffer))
                this.Text = TextBuffer;
            ApplyLanguage();
        }
    }
}
