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
using Wunion.DataAdapter.EntityUtils.CodeProvider;

namespace Wunion.DataAdapter.EntityGenerator.Views
{
    public partial class TableColumnsForm : BaseForm
    {
        private List<TableInfoModel> AllTableInfo;
        private TableInfoModel currentTable;

        public TableColumnsForm(List<TableInfoModel> tables, TableInfoModel tableInfo)
        {
            InitializeComponent();
            AllTableInfo = tables;
            currentTable = tableInfo;
            ColumnsDlgTableName.Text = string.Format("{0} 表", tableInfo.tableName);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DesignMode)
                return;

            IEnumerable<TableInfoModel> Result = from t in AllTableInfo
                                             where t.tableName == currentTable.tableName
                                             select t;
            dataGridView1.DataSource = Result.ToList();
        }

        protected override void ApplyLocaleResourceItem(object control, LocaleResourceItem item)
        {
            if (item.Key == "ColumnsDlgTableName")
            {
                Control c = control as Control;
                if (c != null)
                    c.Text = string.Format(item.Value, currentTable.tableName);
            }
            else
            {
                base.ApplyLocaleResourceItem(control, item);
            }
        }
    }
}
