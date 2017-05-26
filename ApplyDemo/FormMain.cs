using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using DevExpress.XtraLayout.Utils;
using ApplyDemo.Utility;
using Geoway.ADF.MIS.DB.Public.Interface;
using Geoway.ADF.MIS.DB.Public;
using ESRI.ArcGIS.Geodatabase;
using Geoway.ADF.MIS.Utility.Core;
using PartitionHelper;
using DbtuneConfigHelper;
using PartitionUtility;
using System.IO;
using Geoway.ADF.MIS.Utility.DevExpressEx;
using PartitionUtility.DataModel;

namespace ApplyDemo
{
    public partial class FormMain : DevExpress.XtraEditors.XtraForm
    {
        #region 主要操作类

        /// <summary>
        /// 分区流程操作类
        /// </summary>
        private TablePartitionHelper _tbPtitionHelper = null;

        /// <summary>
        /// dbtune配置流程操作类
        /// </summary>
        private DbtuneConfigHelper.DbtuneConfigHelper _dbtuneConfigHelper = null;

        /// <summary>
        /// dbtune管理流程操作类
        /// </summary>
        private DbtuneManageHelper _dbtuneManageHelper = null;

        /// <summary>
        /// 数据库操作类
        /// </summary>
        private IDBHelper _dbHelper = null;

        #endregion

        #region 主要参数

        /// <summary>
        /// 分区流程参数
        /// </summary>
        private PartitionParam _partitionParam = null;

        /// <summary>
        /// dbtune配置流程参数
        /// </summary>
        private DbtuneConfigParam _dbtuneConfigParam = null;

        /// <summary>
        /// 数据库操作参数
        /// </summary>
        private DbConnectParam _dbConnectParam = null;

        #endregion

        #region 过程参数

        /// <summary>
        /// dbtune配置所需的要素
        /// </summary>
        private IFeatureClass _featureClass = null;

        /// <summary>
        /// 分区数
        /// </summary>
        private int _partitionCount = -1;

        /// <summary>
        /// 字段值范围
        /// </summary>
        private Dictionary<string, double> _boundary = null;

        /// <summary>
        /// 表结构信息
        /// </summary>
        private Dictionary<string, string> _columnInfoSet = null;

        /// <summary>
        /// 分区信息集合
        /// </summary>
        private List<PartitionInfo> _partitionInfoSet = null;

        /// <summary>
        /// 分区描述，表示按照什么标准生成的分区
        /// </summary>
        private List<string> _partitionRefer = null;

        /// <summary>
        /// 当前工作流程
        /// </summary>
        private string _workFlow = string.Empty;

        #endregion

        #region 界面控件

        /// <summary>
        /// 界面控件，tabcontrol
        /// </summary>
        private XtraTabControl _tabControl = null;

        /// <summary>
        /// 界面控件，commobox
        /// </summary>
        private ComboBoxEdit _comboBox = null;

        #endregion

        #region 窗体

        /// <summary>
        /// 指定各分区对应表空间的窗体
        /// </summary>
        private FormAssignTablespace _frmAssignTablespace = null;

        /// <summary>
        /// 指定分区个数的窗体
        /// </summary>
        private FormAssignRange _frmAssignRange = null;

        #endregion

        #region 界面事件

        /// <summary>
        /// 主窗体加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMain_Load(object sender, EventArgs e)
        {
            //程序初始化
            Init();
        }

        /// <summary>
        /// tabcontrol切换时，更改当前工作流程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabWorkFlow_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
        {
            this._tabControl = (XtraTabControl)sender;

            switch (this._tabControl.SelectedTabPageIndex)
            {
                case (int)EnumWorkFlow.TablePartition: //表分区流程
                    this.lciPartitionConfig.Visibility = LayoutVisibility.Always;
                    CommonUtil.WriteBlog(this.txbBlog, "---  当前运行功能已更改为普通表转分区表");
                    InitPartitionWorkFlow();
                    this.lciTargetName.Text = "分区表名";
                    this.btnExcute.Text = "开始分区";
                    break;
                case (int)EnumWorkFlow.DbtuneConfigure: //debtune配置流程
                    this.lciPartitionConfig.Visibility = LayoutVisibility.Always;
                    CommonUtil.WriteBlog(this.txbBlog, "---  当前运行功能已更改为dbtune参数配置");
                    InitDbtuneConfigWorkFlow();
                    this.lciTargetName.Text = "配置关键字";
                    this.btnExcute.Text = "开始配置";
                    break;
                case (int)EnumWorkFlow.DbtuneManage: //dbtune管理流程
                    this.lciPartitionConfig.Visibility = LayoutVisibility.Never;
                    CommonUtil.WriteBlog(this.txbBlog, "---  当前运行功能已更改为dbtune参数管理");
                    InitDbtuneDbtuneManageWorkFlow();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 选择表空间下拉列表的关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbTablespace_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            this._comboBox = (ComboBoxEdit)sender;

            if (this._comboBox.SelectedIndex != -1)
            {
                CommonUtil.BindcmbOriginTable(this.cmbOriginTable, this._comboBox.SelectedItem.ToString(), this._dbHelper);
            }
        }

        /// <summary>
        /// 选择分区源表下拉列表的关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbOriginTable_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            this._comboBox = (ComboBoxEdit)sender;

            if (this._comboBox.SelectedIndex != -1)
            {
                CommonUtil.BindcmbColumnIdentity(ref this._columnInfoSet, this.cmbColumnIdentity, this._comboBox.SelectedItem.ToString(), this._dbHelper);
            }
        }

        /// <summary>
        /// 选择分区基准列下拉列表的关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbColumnIdentity_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            this._comboBox = (ComboBoxEdit)sender;

            if (this._comboBox.SelectedIndex != -1)
            {
                //判断分区基准列是否是字符型，若是，关闭分区方式下拉列表的权限，只能使用列表分区
                if (RegexCheck.IsExist(ScreenTemplate.ScreenCharacterType, this._columnInfoSet[this._comboBox.SelectedItem.ToString()]))
                {
                    this.cmbPartitionWay.SelectedIndex = (int)EnumPartitionWay.List;
                    this.cmbPartitionWay.Enabled = false;

                    PrepareListPartition("所选字段为字符型，只可使用列表分区，正在分析分区数量");
                }
                else
                {
                    this.cmbPartitionWay.Enabled = true;
                    this.cmbPartitionWay.SelectedIndex = -1;
                    this.txbTips.Text = string.Empty;
                }
                this.btnAssignTablespace.Text = string.Empty;
            }
        }

        /// <summary>
        /// 选择分区方式下拉列表的关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbPartitionWay_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            this._comboBox = (ComboBoxEdit)sender;

            if (this._comboBox.SelectedIndex == (int)EnumPartitionWay.List)
            {
                //进行列表分区的准备工作
                PrepareListPartition("正在分析分区数量");
            }
            else if (this._comboBox.SelectedIndex == (int)EnumPartitionWay.Range)
            {
                //进行范围分区的准备工作
                PrepareRangePartition();
            }
        }

        /// <summary>
        /// dbtune配置流程中导入要素按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImportFeature_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonUtil.ImportFeature(ref this._featureClass, this.btnImportFeature);

            if (this._featureClass != null)
            {
                CommonUtil.BindcmbColumnIdentity(ref this._columnInfoSet, this.cmbColumnIdentity, this._featureClass);
            }
        }

        /// <summary>
        /// 指定各分区表空间按钮的点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAssignTablespace_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            //如果分区信息集合已经初始化
            if (this._partitionInfoSet.Count != 0)
            {
                if (this._frmAssignTablespace == null || this._frmAssignTablespace.IsDisposed)
                {
                    this._frmAssignTablespace = new FormAssignTablespace(this._partitionInfoSet, QueryUnityInfo.QueryTablespace(this._dbHelper));
                }

                this._frmAssignTablespace.ShowDialog();

                for (int i = 0; i < this._partitionInfoSet.Count; i++)
                {
                    this._partitionInfoSet[i].Tablespace = this._frmAssignTablespace.PartitionInfoSet.Rows[i]["Tablespace"].ToString();

                    if (!RegexCheck.IsEmpty(this._partitionInfoSet[i].Tablespace))
                    {
                        this.btnAssignTablespace.Text += this._partitionInfoSet[i].Tablespace + " | ";
                    }
                }

                this._frmAssignTablespace = null;
            }
        }

        /// <summary>
        /// dbtune管理流程中选择配置关键字下拉列表的关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbDbtuneKeyWord_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            this._comboBox = (ComboBoxEdit)sender;

            if (this._comboBox.SelectedIndex != -1)
            {
                this.txbDbtuneParamValue.Text = this._dbtuneManageHelper.ListParamValueEX(this._comboBox.SelectedItem.ToString());
            }
        }

        /// <summary>
        /// 连接数据库按钮的点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTryConnect_Click(object sender, EventArgs e)
        {
            SetDbConnectParam();

            if (CommonUtil.CheckConnectParam(this.txbBlog, this._dbConnectParam))
            {
                if (this.tabWorkFlow.SelectedTabPageIndex == (int)EnumWorkFlow.TablePartition)
                {
                    if (CommonUtil.TryConnect(this.txbBlog, ref this._dbHelper, this._dbConnectParam, EnumWorkFlow.TablePartition))
                    {
                        CommonUtil.BindcmbTablespace(this.cmbTablespace, this._dbHelper);
                    }
                }
                else
                {
                    if (CommonUtil.TryConnect(this.txbBlog, ref this._dbHelper, this._dbConnectParam, EnumWorkFlow.NotTablePartition))
                    {
                        if (this.tabWorkFlow.SelectedTabPageIndex == (int)EnumWorkFlow.DbtuneConfigure)
                        {
                            this.btnImportFeature.Enabled = true;
                            this.lciConnTips_1.Visibility = LayoutVisibility.Never;
                        }
                        else if (this.tabWorkFlow.SelectedTabPageIndex == (int)EnumWorkFlow.DbtuneManage)
                        {
                            this._dbtuneManageHelper = new DbtuneManageHelper(this._dbHelper, Environment.CurrentDirectory);

                            GwWaitForm.Start("正在收集dbtune配置关键字");
                            this.Cursor = Cursors.WaitCursor;
                            CommonUtil.BindcmbDbtuneKeyWord(this.cmbDbtuneKeyWord, dbHelper: this._dbHelper, filePath: Environment.CurrentDirectory);
                            GwWaitForm.Stop();

                            this.cmbDbtuneKeyWord.Enabled = true;
                            this.lciConnTips_2.Visibility = LayoutVisibility.Never;
                        }
                    }
                }
            }  
        }

        /// <summary>
        /// 执行按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExcute_Click(object sender, EventArgs e)
        {
            //判断当前工作流程
            switch (this.tabWorkFlow.SelectedTabPageIndex)
            {
                case (int)EnumWorkFlow.TablePartition: //表分区工作流程
                    SetPartitionParam();
                    if (CommonUtil.CheckPartitionParam(this.txbBlog, this._partitionParam))
                    {
                        GwWaitForm.Start("正在执行");
                        this.Cursor = Cursors.WaitCursor;
                        CommonUtil.ExcutePartition(this.txbBlog, this._partitionParam, this._tbPtitionHelper);
                        GwWaitForm.Stop();
                        this.Cursor = Cursors.Arrow;
                    }
                    break;
                case (int)EnumWorkFlow.DbtuneConfigure: //dbtune配置工作流程
                    SetDbtuneConfigParam();
                    if (CommonUtil.CheckDbtuneConfigParam(this.txbBlog, this._dbtuneConfigParam))
                    {
                        GwWaitForm.Start("正在执行");
                        this.Cursor = Cursors.WaitCursor;
                        CommonUtil.ExcuteDbtuneConfig(this.txbBlog, this._dbtuneConfigParam, this._dbtuneConfigHelper);
                        GwWaitForm.Stop();
                        this.Cursor = Cursors.Arrow;
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 中断执行按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBreakExcute_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 移除配置关键字按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemoveKeyWord_Click(object sender, EventArgs e)
        {
            if (this._dbtuneManageHelper != null)
            {
                if (this.cmbDbtuneKeyWord.SelectedIndex != -1)
                {
                    if (DevMessageUtil.ShowMsgYesNo("确定要删除关键字" + this.cmbDbtuneKeyWord.SelectedItem.ToString() + "吗？") == DialogResult.Yes)
                    {
                        //如果移除成功，重新绑定配置关键字下拉列表
                        if (this._dbtuneManageHelper.DeleteConfigEX(this.cmbDbtuneKeyWord.SelectedItem.ToString()))
                        {
                            CommonUtil.BindcmbDbtuneKeyWord(this.cmbDbtuneKeyWord);
                            this.cmbDbtuneKeyWord.SelectedIndex = -1;
                            this.txbDbtuneParamValue.Text = string.Empty;
                        }
                        else
                        {
                            CommonUtil.WriteBlog(this.txbBlog, "删除失败，请检查数据库连接参数或有无权限");
                        }
                    }
                }
            }
        }

        #endregion

        #region 公共方法

        public FormMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 程序初始化
        /// </summary>
        private void Init()
        {
            CommonUtil.WriteBlog(this.txbBlog, "---  开始使用分区工具");
            CommonUtil.WriteBlog(this.txbBlog, "---  当前运行主功能为普通表分区");

            //为分区方式下拉列表赋值
            this.cmbPartitionWay.Properties.Items.Add(EnumPartitionWay.List);
            this.cmbPartitionWay.Properties.Items.Add(EnumPartitionWay.Range);

            //初始化分区工作流程
            InitPartitionWorkFlow();
        }

        /// <summary>
        /// 初始化数据库连接
        /// </summary>
        private void InitDbConnect()
        {
            this._dbHelper = DBHelper.Create(DBHelper.enumDBType.DB_Oracle);

            this._dbConnectParam = new DbConnectParam();
            this._dbConnectParam.Clear();

            //this.txbDBServer.Text = string.Empty;
            //this.txbDbPort.Text = string.Empty;
            //this.txbDbSid.Text = string.Empty;
            //this.txbDbUser.Text = string.Empty;
            //this.txbDbPwd.Text = string.Empty;
        }

        /// <summary>
        /// 初始化分区工作流程
        /// </summary>
        private void InitPartitionWorkFlow()
        {
            InitDbConnect();

            this.txbDbPort.Text = "1521";

            CommonUtil.WriteBlog(this.txbBlog, "---  数据库当前未连接");

            this._partitionParam = new PartitionParam();
            this._partitionParam.Clear();
            this._partitionCount = -1;
            this._columnInfoSet = new Dictionary<string, string>();
            this._boundary = new Dictionary<string, double>();
            this._partitionInfoSet = new List<PartitionInfo>();
            this._partitionRefer = new List<string>();

            this.cmbTablespace.SelectedIndex = -1;
            this.cmbOriginTable.SelectedIndex = -1;
            this.cmbTablespace.Properties.Items.Clear();
            this.cmbOriginTable.Properties.Items.Clear();

            this.cmbColumnIdentity.SelectedIndex = -1;
            this.cmbPartitionWay.SelectedIndex = -1;
            this.txbTips.Text = string.Empty;
            this.btnAssignTablespace.Text = string.Empty;
            this.txbTargetName.Text = string.Empty;
            this.cmbColumnIdentity.Properties.Items.Clear();


            this._dbtuneConfigHelper = null;
            this._dbtuneConfigParam = null;
            this._featureClass = null;
            this.btnImportFeature.Text = string.Empty;
            this.lciConnTips_1.Visibility = LayoutVisibility.Never;

            this._dbtuneManageHelper = null;
            this.cmbDbtuneKeyWord.SelectedIndex = -1;
            this.txbDbtuneParamValue.Text = string.Empty;
            this.cmbDbtuneKeyWord.Properties.Items.Clear();
            this.lciConnTips_2.Visibility = LayoutVisibility.Never;

            this._workFlow = EnumWorkFlow.TablePartition.ToString();
        }

        /// <summary>
        /// 初始化dbtune配置工作流程
        /// </summary>
        private void InitDbtuneConfigWorkFlow()
        {
            if (this._workFlow == EnumWorkFlow.TablePartition.ToString())
            {
                InitDbConnect();

                this.txbDbPort.Text = "5151";

                CommonUtil.WriteBlog(this.txbBlog, "---  SDE服务当前未连接");

                this.btnImportFeature.Text = string.Empty;
                this.cmbColumnIdentity.SelectedIndex = -1;
                this.cmbPartitionWay.SelectedIndex = -1;
                this.txbTips.Text = string.Empty;
                this.btnAssignTablespace.Text = string.Empty;
                this.txbTargetName.Text = string.Empty;

                this.cmbColumnIdentity.Properties.Items.Clear();
            }


            this.lciConnTips_1.Visibility = LayoutVisibility.Always;
            this._dbtuneConfigParam = new DbtuneConfigParam();
            this._dbtuneConfigParam.Clear();
            this._featureClass = null;
            this._partitionCount = -1;
            this._columnInfoSet = new Dictionary<string, string>();
            this._boundary = new Dictionary<string, double>();
            this._partitionInfoSet = new List<PartitionInfo>();
            this._partitionRefer = new List<string>();

            this.btnImportFeature.Enabled = false;

            this._tbPtitionHelper = null;
            this._partitionParam = null;

            this.cmbTablespace.SelectedIndex = -1;
            this.cmbOriginTable.SelectedIndex = -1;
            this.cmbTablespace.Properties.Items.Clear();
            this.cmbOriginTable.Properties.Items.Clear();

            this._dbtuneManageHelper = null;
            this.cmbDbtuneKeyWord.SelectedIndex = -1;
            this.txbDbtuneParamValue.Text = string.Empty;
            this.cmbDbtuneKeyWord.Properties.Items.Clear();
            this.lciConnTips_2.Visibility = LayoutVisibility.Never;

            this._workFlow = EnumWorkFlow.DbtuneConfigure.ToString();
        }

        /// <summary>
        /// 初始化dbtune管理工作流程
        /// </summary>
        private void InitDbtuneDbtuneManageWorkFlow()
        {
            if (this._workFlow == EnumWorkFlow.TablePartition.ToString())
            {
                InitDbConnect();

                this.txbDbPort.Text = "5151";

                CommonUtil.WriteBlog(this.txbBlog, "---  SDE服务当前未连接");

                this.cmbDbtuneKeyWord.SelectedIndex = -1;
                this.txbDbtuneParamValue.Text = string.Empty;

                this.cmbDbtuneKeyWord.Properties.Items.Clear();
            }
            this.lciConnTips_2.Visibility = LayoutVisibility.Always;
            this.cmbDbtuneKeyWord.Enabled = false;

            this._dbtuneConfigHelper = null;
            this._dbtuneConfigParam = null;
            this._featureClass = null;
            this.btnImportFeature.Text = string.Empty;
            this.lciConnTips_1.Visibility = LayoutVisibility.Never;

            this._tbPtitionHelper = null;
            this._partitionParam = null;

            this.cmbTablespace.SelectedIndex = -1;
            this.cmbOriginTable.SelectedIndex = -1;
            this.cmbTablespace.Properties.Items.Clear();
            this.cmbOriginTable.Properties.Items.Clear();

            this._workFlow = EnumWorkFlow.DbtuneManage.ToString();
        }

        /// <summary>
        /// 设置数据库连接参数
        /// </summary>
        private void SetDbConnectParam()
        {
            if (!RegexCheck.IsEmpty(this.txbDBServer.Text))
            {
                this._dbConnectParam.DBServer = this.txbDBServer.Text;
            }
            if (!RegexCheck.IsEmpty(this.txbDbPort.Text))
            {
                this._dbConnectParam.DbPort = this.txbDbPort.Text;
            }
            if (!RegexCheck.IsEmpty(this.txbDbSid.Text))
            {
                this._dbConnectParam.DbSid = this.txbDbSid.Text;
            }
            if (!RegexCheck.IsEmpty(this.txbDbUser.Text))
            {
                this._dbConnectParam.DbUser = this.txbDbUser.Text;
            }
            if (!RegexCheck.IsEmpty(this.txbDbPwd.Text))
            {
                this._dbConnectParam.DbPwd = this.txbDbPwd.Text;
            }
        }

        /// <summary>
        /// 设置分区工作流程参数
        /// </summary>
        private void SetPartitionParam()
        {
            if (this._dbHelper != null)
            {
                this._partitionParam.DBHelper = this._dbHelper;
            }
            if (this.cmbTablespace.SelectedIndex != -1)
            {
                this._partitionParam.TablespaceName = this.cmbTablespace.SelectedItem.ToString();
            }
            if (this.cmbOriginTable.SelectedIndex != -1)
            {
                this._partitionParam.OriginTableName = this.cmbOriginTable.SelectedItem.ToString();
            }
            if (this.cmbColumnIdentity.SelectedIndex != -1)
            {
                this._partitionParam.BasicField = this.cmbColumnIdentity.SelectedItem.ToString();
            }
            if (this.cmbPartitionWay.SelectedIndex != -1)
            {
                if (this.cmbPartitionWay.SelectedIndex == (int)EnumPartitionWay.List)
                {
                    this._partitionParam.PartitionWay = EnumPartitionWay.List;
                }
                else if (this.cmbPartitionWay.SelectedIndex == (int)EnumPartitionWay.Range)
                {
                    this._partitionParam.PartitionWay = EnumPartitionWay.Range;
                }
            }
            if (this._partitionCount != -1 && this.cmbPartitionWay.SelectedIndex == (int)EnumPartitionWay.Range)
            {
                this._partitionParam.PartitionCount = this._partitionCount;
            }
            if (this._partitionInfoSet != null)
            {
                List<string> tablespaceSet = new List<string>();
                for (int i = 0; i < this._partitionInfoSet.Count; i++)
                {
                    tablespaceSet.Add(this._partitionInfoSet[i].Tablespace);
                }
                this._partitionParam.TablespaceSet = tablespaceSet;
            }
            if (!RegexCheck.IsEmpty(this.txbTargetName.Text))
            {
                this._partitionParam.PartitionedTableName = this.txbTargetName.Text;
            }
        }

        /// <summary>
        /// 设置dbtune配置工作流程参数
        /// </summary>
        private void SetDbtuneConfigParam()
        {
            if (this._dbHelper != null)
            {
                this._dbtuneConfigParam.DBHelper = this._dbHelper;
            }
            if (this._featureClass != null)
            {
                this._dbtuneConfigParam.FeatureClass = this._featureClass;
            }
            if (this.cmbColumnIdentity.SelectedIndex != -1)
            {
                this._dbtuneConfigParam.BasicField = this.cmbColumnIdentity.SelectedItem.ToString();
            }
            if (this.cmbPartitionWay.SelectedIndex != -1)
            {
                if (this.cmbPartitionWay.SelectedIndex == (int)EnumPartitionWay.List)
                {
                    this._dbtuneConfigParam.PartitionWay = EnumPartitionWay.List;
                }
                else if (this.cmbPartitionWay.SelectedIndex == (int)EnumPartitionWay.Range)
                {
                    this._dbtuneConfigParam.PartitionWay = EnumPartitionWay.Range;
                }
            }
            if (this._partitionCount != -1 && this.cmbPartitionWay.SelectedIndex == (int)EnumPartitionWay.Range)
            {
                this._dbtuneConfigParam.PartitionCount = this._partitionCount;
            }
            if (this._partitionInfoSet != null)
            {
                List<string> tablespaceSet = new List<string>();
                for (int i = 0; i < this._partitionInfoSet.Count; i++)
                {
                    tablespaceSet.Add(this._partitionInfoSet[i].Tablespace);
                }
                this._dbtuneConfigParam.TablespaceSet = tablespaceSet;
            }
            if (!RegexCheck.IsEmpty(this.txbTargetName.Text))
            {
                this._dbtuneConfigParam.DbtuneKeyWord = this.txbTargetName.Text;
            }
        }

        /// <summary>
        /// 列表分区工作准备
        /// </summary>
        /// <param name="tips"></param>
        private void PrepareListPartition(string tips)
        {
            GwWaitForm.Start(tips);
            this.Cursor = Cursors.WaitCursor;

            //根据工作流程，查找分区描述信息
            if (this.tabWorkFlow.SelectedTabPageIndex == (int)EnumWorkFlow.TablePartition)
            {
                this._partitionRefer = CommonUtil.GetPartitionRefer(this.cmbColumnIdentity.SelectedItem.ToString(), this.cmbOriginTable.SelectedItem.ToString(), this._dbHelper);
            }
            else if (this.tabWorkFlow.SelectedTabPageIndex == (int)EnumWorkFlow.DbtuneConfigure)
            {
                this._partitionRefer = CommonUtil.GetPartitionRefer(this.cmbColumnIdentity.SelectedItem.ToString(), this._featureClass);
            }

            this._partitionCount = this._partitionRefer.Count;

            this.Cursor = Cursors.Arrow;
            GwWaitForm.Stop();

            this._partitionInfoSet = CommonUtil.GetPartitionInfoSet(this.cmbPartitionWay.SelectedIndex, this.cmbColumnIdentity.SelectedItem.ToString(), this._partitionRefer);

            this.txbTips.Text = string.Format("分区数：{0}", this._partitionCount.ToString());
        }

        /// <summary>
        /// 范围分区工作准备
        /// </summary>
        private void PrepareRangePartition()
        {
            GwWaitForm.Start("正在查找范围边界");
            this.Cursor = Cursors.WaitCursor;

            //根据工作流程，查找分区基准列的边界
            if (this.tabWorkFlow.SelectedTabPageIndex == (int)EnumWorkFlow.TablePartition)
            {
                this._boundary = QueryUnityInfo.QueryBoundary(this._dbHelper, this.cmbColumnIdentity.SelectedItem.ToString(), this.cmbOriginTable.SelectedItem.ToString());
            }
            else if (this.tabWorkFlow.SelectedTabPageIndex == (int)EnumWorkFlow.DbtuneConfigure)
            {
                this._boundary = QueryUnityInfo.QueryBoundary(this._featureClass, this.cmbColumnIdentity.SelectedItem.ToString());
            }

            this.Cursor = Cursors.Arrow;
            GwWaitForm.Stop();

            //打开指定分区数的窗体
            if (this._frmAssignRange == null || this._frmAssignRange.IsDisposed)
            {
                this._frmAssignRange = new FormAssignRange(this._boundary["MIN"], this._boundary["MAX"]);
            }

            this._frmAssignRange.ShowDialog();

            //如果取消指定分区数，则默认采用列表分区
            if (this._frmAssignRange.IsCancel)
            {
                this.cmbPartitionWay.SelectedIndex = (int)EnumPartitionWay.List;
                //转而进行列表分区工作准备
                PrepareListPartition("已取消范围分区，默认采用列表分区，正在分析分区数量");
            }
            else
            {
                this._partitionCount = this._frmAssignRange.PartitionCount;

                this._partitionRefer = CommonUtil.GetPartitionRefer(this._partitionCount, this._boundary);

                this._partitionInfoSet = CommonUtil.GetPartitionInfoSet(this.cmbPartitionWay.SelectedIndex, this.cmbColumnIdentity.SelectedItem.ToString(), this._partitionRefer);

                this.txbTips.Text = string.Format("{0} ~ {1}   分区数：{2}", this._boundary["MIN"].ToString("0.00"), this._boundary["MAX"].ToString("0.00"), this._partitionCount);
            }

            this._frmAssignRange = null;
        }

        #endregion
    }
}