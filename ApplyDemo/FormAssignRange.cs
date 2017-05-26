using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using PartitionUtility;
using Geoway.ADF.MIS.Utility.DevExpressEx;

namespace ApplyDemo
{
    /// <summary>
    ///目的：指定范围分区的分区数
    ///创建人：高涛
    ///创建日期：2017-03-05
    ///修改描述：
    ///修改人：
    ///修改日期：
    ///备注：
    /// </summary>
    public partial class FormAssignRange : DevExpress.XtraEditors.XtraForm
    {
        /// <summary>
        /// 某字段的最小值
        /// </summary>
        public double MinValue { get; set; }

        /// <summary>
        /// 某字段的最大值
        /// </summary>
        public double MaxValue { get; set; }

        /// <summary>
        /// 分区数量
        /// </summary>
        public int PartitionCount { get; set; }

        /// <summary>
        /// 是否取消范围放弃
        /// </summary>
        public bool IsCancel { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        public FormAssignRange(double minValue, double maxValue)
        {
            InitializeComponent();

            this.MinValue = minValue;
            this.MaxValue = maxValue;

            this.txbMinValue.Text = minValue.ToString("0.00");
            this.txbMaxValue.Text = maxValue.ToString("0.00");
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (RegexCheck.IsNotNagtive(this.txbPartitionCount.Text))
            {
                this.PartitionCount = int.Parse(this.txbPartitionCount.Text);
                this.IsCancel = false;
                this.Close();
            }
            else
            {
                DevMessageUtil.ShowMessageDialog("输入的参数不正确");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.IsCancel = true;
            this.Close();
        }
    }
}