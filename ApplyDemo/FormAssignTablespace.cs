using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Collections.ObjectModel;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors.Controls;
using PartitionUtility;
using Geoway.ADF.MIS.Utility.DevExpressEx;

namespace ApplyDemo
{
    /// <summary>
    ///目的：为各分区指定表空间
    ///创建人：高涛
    ///创建日期：2017-03-05
    ///修改描述：
    ///修改人：
    ///修改日期：
    ///备注：
    /// </summary>
    public partial class FormAssignTablespace : DevExpress.XtraEditors.XtraForm
    {
        /// <summary>
        /// GridControl的数据源，分区信息数据集
        /// </summary>
        public DataTable PartitionInfoSet { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="partitionInfoSet"></param>
        /// <param name="tablespaceSet"></param>
        public FormAssignTablespace(List<PartitionInfo> partitionInfoSet, List<string> tablespaceSet)
        {
            InitializeComponent();

            InitDataSource(partitionInfoSet, tablespaceSet);
        }

        /// <summary>
        /// 初始化数据源
        /// </summary>
        /// <param name="partitionInfoSet"></param>
        /// <param name="tablespaceSet"></param>
        private void InitDataSource(List<PartitionInfo> partitionInfoSet, List<string> tablespaceSet)
        {
            for (int i = 0; i < tablespaceSet.Count; i++)
            {
                cmbTablespace.Items.Add(tablespaceSet[i]);
            }

            this.PartitionInfoSet = new DataTable();

            this.PartitionInfoSet.Columns.Add("Id", typeof(int));
            this.PartitionInfoSet.Columns.Add("Refer", typeof(string));
            this.PartitionInfoSet.Columns.Add("Tablespace", typeof(string));

            for (int i = 0; i < partitionInfoSet.Count; i++)
            {
                this.PartitionInfoSet.Rows.Add(new object[] { partitionInfoSet[i].Id, partitionInfoSet[i].Refer, partitionInfoSet[i].Tablespace });
            }

            this.dgPartitionInfo.DataSource = this.PartitionInfoSet;

            this.cmbTablespace.SelectedIndexChanged += new EventHandler(cmb_SelectedIndexChanged);
            this.cmbTablespace.ParseEditValue += new ConvertEditValueEventHandler(cmb_ParseEditValue);
        }

        /// <summary>
        /// 检查所有分区是否都已指定表空间
        /// </summary>
        /// <returns></returns>
        private bool CheckInfoComplete()
        {
            bool isInfoComplete = true;

            for (int i = 0; i < PartitionInfoSet.Rows.Count; i++)
            {
                if (RegexCheck.IsEmpty(PartitionInfoSet.Rows[i]["Tablespace"].ToString()))
                {
                    isInfoComplete = false;
                    break;
                }
            }
            return isInfoComplete;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (CheckInfoComplete())
            {
                this.Close();
            }
            else
            {
                DevMessageUtil.ShowMessageDialog("存在没有指定表空间的分区");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmb_ParseEditValue(object sender, ConvertEditValueEventArgs e)
        {
            e.Value = e.Value.ToString();
            e.Handled = true;
        }

        private void cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            //获取当前gridview选中行
            GridView gridView = (dgPartitionInfo.MainView as GridView);
            int rowIndex = gridView.GetDataSourceRowIndex(gridView.FocusedRowHandle);

            //保存选中值到datatable
            this.PartitionInfoSet.Rows[rowIndex]["tablespace"] = (sender as ComboBoxEdit).SelectedItem;
        }
    }
}