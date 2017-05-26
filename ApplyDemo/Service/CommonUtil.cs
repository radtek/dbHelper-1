using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraEditors;
using Geoway.ADF.MIS.DB.Public.Interface;
using ESRI.ArcGIS.Geodatabase;
using System.Windows.Forms;
using ESRI.ArcGIS.DataSourcesFile;
using PartitionHelper;
using DbtuneConfigHelper;
using System.Threading;
using PartitionUtility;
using Geoway.ADF.MIS.Utility.Log;

namespace ApplyDemo.Utility
{
    /// <summary>
    ///目的：提供主界面的公共方法
    ///创建人：高涛
    ///创建日期：2017-03-07
    ///修改描述：
    ///修改人：
    ///修改日期：
    ///备注：
    /// </summary>
    internal static class CommonUtil
    {
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="txbBlog"></param>
        /// <param name="blogContent"></param>
        internal static void WriteBlog(MemoEdit txbBlog, string blogContent)
        {
            //由于其他线程不可访问UI线程，故使用Dispatcher调度

            //this.Dispatcher.Invoke(new Action(() => { this.txbBlog.AppendText(blogContent + Environment.NewLine); }));
            //this.Dispatcher.Invoke(new Action(() => { this.txbBlog.ScrollToEnd(); }));

            txbBlog.Invoke(new Action(() =>
            {
                txbBlog.Text += blogContent;
                txbBlog.Text += Environment.NewLine;
                txbBlog.Text += Environment.NewLine;
            }));
            txbBlog.Invoke(new Action(() =>
            {
                txbBlog.SelectionStart = txbBlog.Text.Length;
                txbBlog.ScrollToCaret();
            }));
        }

        /// <summary>
        /// 检查数据库连接参数
        /// </summary>
        /// <returns></returns>
        internal static bool CheckConnectParam(MemoEdit txbBlog, DbConnectParam dbConnectParam)
        {
            if (RegexCheck.IsEmpty(dbConnectParam.DBServer))
            {
                WriteBlog(txbBlog, "---  未指定IP地址");
                return false;
            }
            else if (!RegexCheck.IsIPv4(dbConnectParam.DBServer))
            {
                WriteBlog(txbBlog, "---  指定的IP地址格式不正确");
                return false;
            }
            else if (RegexCheck.IsEmpty(dbConnectParam.DbPort))
            {
                WriteBlog(txbBlog, "---  未指定端口号");
                return false;
            }
            else if (!RegexCheck.IsUint(dbConnectParam.DbPort))
            {
                WriteBlog(txbBlog, "---  指定的端口号格式不正确");
                return false;
            }
            else if (RegexCheck.IsEmpty(dbConnectParam.DbSid))
            {
                WriteBlog(txbBlog, "---  未指定实例名");
                return false;
            }
            else if (RegexCheck.IsEmpty(dbConnectParam.DbUser))
            {
                WriteBlog(txbBlog, "---  未指定登陆名");
                return false;
            }
            else if (RegexCheck.IsEmpty(dbConnectParam.DbPwd))
            {
                WriteBlog(txbBlog, "---  未指定口令");
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 检查Dbtune配置流程涉及参数
        /// </summary>
        /// <returns></returns>
        internal static bool CheckDbtuneConfigParam(MemoEdit txbBlog, DbtuneConfigParam dbtuneConfigParam)
        {
            if (dbtuneConfigParam.DBHelper == null)
            {
                WriteBlog(txbBlog, "---  数据库连接异常, 请查看错误日志");
                return false;
            }
            else if (dbtuneConfigParam.FeatureClass == null)
            {
                WriteBlog(txbBlog, "---  未指定要素类");
                return false;
            }
            else if (RegexCheck.IsEmpty(dbtuneConfigParam.BasicField))
            {
                WriteBlog(txbBlog, "---  未指定分区列");
                return false;
            }
            else if (dbtuneConfigParam.PartitionWay == EnumPartitionWay.Unknow)
            {
                WriteBlog(txbBlog, "---  未指定分区方式");
                return false;
            }
            else if (!RegexCheck.IsFull(dbtuneConfigParam.TablespaceSet))
            {
                WriteBlog(txbBlog, "---  存在没有指定表空间的分区");
                return false;
            }
            else if (DbtuneXMLOperate.CheckKeyWordExist(dbtuneConfigParam.DbtuneKeyWord))
            {
                WriteBlog(txbBlog, "---  关键字为 " + dbtuneConfigParam.DbtuneKeyWord + "的配置项已经存在，请更改关键字");
                return false;
            }
            else
            {
                if (dbtuneConfigParam.PartitionWay == EnumPartitionWay.Range)
                {
                    if (dbtuneConfigParam.PartitionCount == -1)
                    {
                        WriteBlog(txbBlog, "---  未指定范围分区数");
                        return false;
                    }
                    else return true;
                }
                else return true;
            }
        }

        /// <summary>
        /// 检查分区流程涉及参数
        /// </summary>
        /// <returns></returns>
        internal static bool CheckPartitionParam(MemoEdit txbBlog, PartitionParam partitionParam)
        {
            if (partitionParam.DBHelper == null)
            {
                WriteBlog(txbBlog, "---  数据库连接异常, 请查看错误日志");
                return false;
            }
            else if (RegexCheck.IsEmpty(partitionParam.TablespaceName))
            {
                WriteBlog(txbBlog, "---  未指定表空间");
                return false;
            }
            else if (RegexCheck.IsEmpty(partitionParam.OriginTableName))
            {
                WriteBlog(txbBlog, "---  未指定初始表");
                return false;
            }
            else if (RegexCheck.IsEmpty(partitionParam.BasicField))
            {
                WriteBlog(txbBlog, "---  未指定分区列");
                return false;
            }
            else if (partitionParam.PartitionWay == EnumPartitionWay.Unknow)
            {
                WriteBlog(txbBlog, "---  未指定分区方式");
                return false;
            }
            else if (!RegexCheck.IsFull(partitionParam.TablespaceSet))
            {
                WriteBlog(txbBlog, "---  存在没有指定表空间的分区");
                return false;
            }
            else if (RegexCheck.IsExist(QueryUnityInfo.QueryTotalTable(partitionParam.DBHelper, partitionParam.TablespaceName), partitionParam.PartitionedTableName))
            {
                WriteBlog(txbBlog, "---  名字为 " + partitionParam.PartitionedTableName + "的表已经存在，请更改导出名");
                return false;
            }
            else
            {
                if (partitionParam.PartitionWay == EnumPartitionWay.Range && partitionParam.PartitionCount == -1)
                {
                    WriteBlog(txbBlog, "---未指定范围分区数");
                    return false;
                }
                else return true;
            }
        }

        /// <summary>
        /// 获取分区目录（普通表列表分区）
        /// </summary>
        /// <param name="basicField"></param>
        /// <param name="originTable"></param>
        /// <param name="dbHelper"></param>
        /// <returns></returns>
        internal static List<string> GetPartitionRefer(string basicField, string originTable, IDBHelper dbHelper)
        {
            List<string> partitionRefer = new List<string>();

            partitionRefer = QueryUnityInfo.QueryUniqueValue(dbHelper, basicField, originTable);

            return partitionRefer;
        }

        /// <summary>
        /// 获取分区目录（要素列表分区）
        /// </summary>
        /// <param name="basicField"></param>
        /// <param name="featureClass"></param>
        /// <returns></returns>
        internal static List<string> GetPartitionRefer(string basicField, IFeatureClass featureClass)
        {
            List<string> partitionRefer = new List<string>();

            partitionRefer = QueryUnityInfo.QueryUniqueValue(featureClass, basicField);

            return partitionRefer;
        }

        /// <summary>
        /// 获取分区目录（范围分区）
        /// </summary>
        /// <param name="partitionCount"></param>
        /// <param name="boundary"></param>
        /// <returns></returns>
        internal static List<string> GetPartitionRefer(int partitionCount, Dictionary<string, double> boundary)
        {
            List<string> partitionRefer = new List<string>();

            partitionRefer = QueryUnityInfo.QueryRange(boundary["MIN"], boundary["MAX"], partitionCount);

            return partitionRefer;
        }

        /// <summary>
        /// 获取分区详细信息
        /// </summary>
        /// <param name="partitionWayIndex"></param>
        /// <param name="basicField"></param>
        /// <param name="partitionRefer"></param>
        /// <returns></returns>
        internal static List<PartitionInfo> GetPartitionInfoSet(int partitionWayIndex, string basicField, List<string> partitionRefer)
        {
            List<PartitionInfo> partitionInfoSet = new List<PartitionInfo>();

            for (int i = 0; i < partitionRefer.Count; i++)
            {
                partitionInfoSet.Add(new PartitionInfo()
                {
                    Id = i + 1,
                    Refer = basicField + (partitionWayIndex == (int)EnumPartitionWay.List ? " = " : " Less Than ") + partitionRefer[i],
                });
            }

            partitionInfoSet.Add(new PartitionInfo()
            {
                Id = partitionRefer.Count + 1,
                Refer = (partitionWayIndex == (int)EnumPartitionWay.List ? "DEFAULT" : "MAXVALUE")
            });

            return partitionInfoSet;
        }

        /// <summary>
        /// 执行dbtune配置工作
        /// </summary>
        /// <param name="txbBlog"></param>
        /// <param name="dbtuneConfigParam"></param>
        /// <param name="dbtuneConfigHelper"></param>
        internal static void ExcuteDbtuneConfig(MemoEdit txbBlog, DbtuneConfigParam dbtuneConfigParam, DbtuneConfigHelper.DbtuneConfigHelper dbtuneConfigHelper)
        {
            dbtuneConfigHelper = new DbtuneConfigHelper.DbtuneConfigHelper(dbtuneConfigParam, Environment.CurrentDirectory);

            if (dbtuneConfigHelper.AlterParamValueEX())
            {
                WriteBlog(txbBlog, "---  执行成功");
            }
            else
            {
                WriteBlog(txbBlog, "---  执行失败，请检查参数");
            }
        }

        /// <summary>
        /// 执行数据表分区工作
        /// </summary>
        /// <param name="txbBlog"></param>
        /// <param name="partitionParam"></param>
        /// <param name="tbpHelper"></param>
        internal static void ExcutePartition(MemoEdit txbBlog, PartitionParam partitionParam, TablePartitionHelper tbpHelper)
        {
            tbpHelper = new TablePartitionHelper(partitionParam);

            if (tbpHelper.PartitionByExchange())
            {
                WriteBlog(txbBlog, "---  执行成功");
            }
            else
            {
                WriteBlog(txbBlog, "---  执行失败，请检查参数");
            }
        }

        /// <summary>
        /// 初始化数据库连接帮助类
        /// 尝试连接
        /// </summary>
        /// <param name="txbBlog"></param>
        /// <param name="dbHelper"></param>
        /// <param name="dbConnectParam"></param>
        /// <param name="workflow"></param>
        /// <returns></returns>
        internal static bool TryConnect(MemoEdit txbBlog, ref IDBHelper dbHelper, DbConnectParam dbConnectParam, EnumWorkFlow workflow)
        {
            dbHelper.DBServiceName = string.Format("{0}/{1}", dbConnectParam.DBServer, dbConnectParam.DbSid);
            dbHelper.DBPort = dbConnectParam.DbPort;
            dbHelper.DBUser = dbConnectParam.DbUser;
            dbHelper.DBPwd = dbConnectParam.DbPwd;

            try
            {
                if (workflow == EnumWorkFlow.TablePartition)
                {
                    if (dbHelper.TryConnect())
                    {
                        WriteBlog(txbBlog, "---  数据库连接成功");
                        return true;
                    }
                    else
                    {
                        WriteBlog(txbBlog, "---  数据库连接异常, 请查看错误日志");
                        return false;
                    }
                }
                else
                {
                    DbtuneHelper dbtuneHelper = new DbtuneHelper(Environment.CurrentDirectory, dbConnectParam.DBServer, dbConnectParam.DbPort, dbConnectParam.DbUser, dbConnectParam.DbPwd);

                    if (dbtuneHelper.TryConnect())
                    {
                        WriteBlog(txbBlog, "---  SDE服务连接成功");
                        return true;
                    }
                    else
                    {
                        WriteBlog(txbBlog, "---  SDE服务连接异常, 请查看错误日志");
                        return false;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        
        /// <summary>
        /// 为配置关键字下拉菜单提供数据绑定
        /// </summary>
        /// <param name="cmbDbtuneKeyWord"></param>
        /// <param name="dbHelper"></param>
        /// <param name="filePath">dbtune导出文件的复制路径</param>
        internal static void BindcmbDbtuneKeyWord(ComboBoxEdit cmbDbtuneKeyWord, IDBHelper dbHelper = null, string filePath = null)
        {
            List<string> dbtuneKeyWordSet = GetDbtuneKeyWordSet(dbHelper: dbHelper, filePath: filePath);

            cmbDbtuneKeyWord.Invoke(new Action(() => { cmbDbtuneKeyWord.Properties.Items.Clear(); }));

            foreach (var dbtuneKeyWord in dbtuneKeyWordSet)
            {
                cmbDbtuneKeyWord.Invoke(new Action(() => { cmbDbtuneKeyWord.Properties.Items.Add(dbtuneKeyWord); }));
            }
        }

        /// <summary>
        /// 为表空间下拉菜单提供数据绑定
        /// </summary>
        /// <param name="cmbTablespace"></param>
        /// <param name="dbHelper"></param>
        internal static void BindcmbTablespace(ComboBoxEdit cmbTablespace, IDBHelper dbHelper)
        {
            List<string> tablespaceSet = QueryUnityInfo.QueryTablespace(dbHelper);

            cmbTablespace.Invoke(new Action(() => { cmbTablespace.Properties.Items.Clear(); }));

            foreach (var tablespace in tablespaceSet)
            {
                cmbTablespace.Invoke(new Action(() => { cmbTablespace.Properties.Items.Add(tablespace); }));
            }
        }

        /// <summary>
        /// 为源表下拉菜单提供数据绑定
        /// </summary>
        /// <param name="cmbOriginTable"></param>
        /// <param name="tablespace"></param>
        /// <param name="dbHelper"></param>
        internal static void BindcmbOriginTable(ComboBoxEdit cmbOriginTable, string tablespace, IDBHelper dbHelper)
        {
            List<string> originTableSet = QueryUnityInfo.QueryTotalTable(dbHelper, tablespace);

            cmbOriginTable.Invoke(new Action(() => { cmbOriginTable.Properties.Items.Clear(); }));

            cmbOriginTable.Invoke(new Action(() => { cmbOriginTable.Properties.Items.AddRange(originTableSet); }));
        }

        /// <summary>
        /// 为分区基准列下拉菜单提供数据绑定
        /// </summary>
        /// <param name="columnInfoSet"></param>
        /// <param name="cmbColumnIdentity"></param>
        /// <param name="originTable"></param>
        /// <param name="dbHelper"></param>
        internal static void BindcmbColumnIdentity(ref Dictionary<string, string> columnInfoSet, ComboBoxEdit cmbColumnIdentity, string originTable, IDBHelper dbHelper)
        {
            List<string> columnIdentitySet = GetColumnIdentitySet(ref columnInfoSet, originTable, dbHelper);

            cmbColumnIdentity.Invoke(new Action(() => { cmbColumnIdentity.Properties.Items.Clear(); }));

            cmbColumnIdentity.Invoke(new Action(() => { cmbColumnIdentity.Properties.Items.AddRange(columnIdentitySet); }));
        }

        /// <summary>
        /// 为分区基准列下拉菜单提供数据绑定（要素）
        /// </summary>
        /// <param name="columnInfoSet"></param>
        /// <param name="cmbColumnIdentity"></param>
        /// <param name="featureClass"></param>
        internal static void BindcmbColumnIdentity(ref Dictionary<string, string> columnInfoSet, ComboBoxEdit cmbColumnIdentity, IFeatureClass featureClass)
        {
            List<string> columnIdentitySet = GetColumnIdentitySet(ref columnInfoSet, featureClass);

            cmbColumnIdentity.Invoke(new Action(() => { cmbColumnIdentity.Properties.Items.Clear(); }));

            foreach (var columnIdentity in columnIdentitySet)
            {
                cmbColumnIdentity.Invoke(new Action(() => { cmbColumnIdentity.Properties.Items.Add(columnIdentity); }));
            }
        }

        /// <summary>
        /// 导入要素
        /// </summary>
        /// <param name="featureClass"></param>
        /// <param name="btnImportFeature"></param>
        internal static void ImportFeature(ref IFeatureClass featureClass, ButtonEdit btnImportFeature)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "添加Shape数据";
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Shape|*.shp";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.CheckFileExists = true;
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (openFileDialog.FileName != null)
                {
                    IWorkspaceFactory shapeWorkspaceFactory = new ShapefileWorkspaceFactoryClass();
                    IFeatureWorkspace shapeFeatureWorkspace = (IFeatureWorkspace)shapeWorkspaceFactory.OpenFromFile(System.IO.Path.GetDirectoryName(openFileDialog.FileName), 0);
                    featureClass = shapeFeatureWorkspace.OpenFeatureClass(System.IO.Path.GetFileName(openFileDialog.FileName));

                    string aliasName = featureClass.AliasName;

                    btnImportFeature.Invoke(new Action(() => { btnImportFeature.Text = aliasName; }));
                }
                else featureClass = null;
            }
            else featureClass = null;
        }

        /// <summary>
        /// 获取数据表字段集合
        /// </summary>
        /// <param name="columnInfoSet"></param>
        /// <param name="originTable"></param>
        /// <param name="dbHelper"></param>
        /// <returns></returns>
        internal static List<string> GetColumnIdentitySet(ref Dictionary<string, string> columnInfoSet, string originTable, IDBHelper dbHelper)
        {
            columnInfoSet = QueryUnityInfo.QueryColumnInfo(dbHelper, originTable);

            List<string> columnIdentitySet = new List<string>();

            foreach (var columnIdentity in columnInfoSet.Keys)
            {
                columnIdentitySet.Add(columnIdentity);
            }

            return columnIdentitySet;
        }

        /// <summary>
        /// 获取数据表字段集合（要素）
        /// </summary>
        /// <param name="columnInfoSet"></param>
        /// <param name="featureClass"></param>
        /// <returns></returns>
        private static List<string> GetColumnIdentitySet(ref Dictionary<string, string> columnInfoSet, IFeatureClass featureClass)
        {
            columnInfoSet = QueryUnityInfo.QueryColumnInfo(featureClass);

            List<string> columnIdentitySet = new List<string>();

            foreach (var columnIdentity in columnInfoSet.Keys)
            {
                columnIdentitySet.Add(columnIdentity);
            }

            return columnIdentitySet;
        }

        /// <summary>
        /// 获取配置关键字集合
        /// </summary>
        /// <returns></returns>
        internal static List<string> GetDbtuneKeyWordSet(IDBHelper dbHelper = null, string filePath = null)
        {
            List<DbtuneInfo> dbtuneInfoSet = DbtuneXMLOperate.ListKeyWord(dbHelper: dbHelper, filePath: filePath);

            List<string> dbtuneKeyWordSet = new List<string>();

            foreach (var dbtuneInfo in dbtuneInfoSet)
            {
                dbtuneKeyWordSet.Add(dbtuneInfo.KeyWord);
            }

            return dbtuneKeyWordSet;
        }
    }
}
