using ESRI.ArcGIS.Geodatabase;
using Geoway.ADF.MIS.DB.Public.Interface;
using Geoway.ADF.MIS.Utility.Log;
using PartitionUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbtuneConfigHelper
{
    /// <summary>
    ///目的：提供dbtune配置的方法
    ///创建人：高涛
    ///创建日期：2017-03-03
    ///修改描述：
    ///修改人：
    ///修改日期：
    ///备注：
    /// </summary>
    public class DbtuneConfigHelper
    {
        #region 属性

        /// <summary>
        /// dbtune配置帮助类
        /// </summary>
        private DbtuneHelper _dbtuneHelper;

        /// <summary>
        /// 数据库连接帮助类
        /// </summary>
        private IDBHelper _dbHelper { get; set; }

        /// <summary>
        /// 分区要素
        /// </summary>
        private IFeatureClass _featureClass { get; set; }

        /// <summary>
        /// 分区列名
        /// </summary>
        private string _basicField { get; set; }

        /// <summary>
        /// 分区方式
        /// </summary>
        private EnumPartitionWay _partitionWay { get; set; }

        /// <summary>
        /// 范围分区的分区个数
        /// </summary>
        private int _partitionCount { get; set; }

        /// <summary>
        /// 各分区对应表空间
        /// </summary>
        private List<string> _tablespaceSet { get; set; }

        /// <summary>
        /// 配置关键字
        /// </summary>
        private string _dbtuneKeyWord { get; set; }

        /// <summary>
        /// 复制dbtune文件的路径
        /// </summary>
        private string _sdeCopyPath { get; set; }

        #endregion

        #region 公开方法

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="param"></param>
        /// <param name="filePath">dbtune导出文件的复制路径</param>
        public DbtuneConfigHelper(DbtuneConfigParam param, string filePath)
        {
            this._dbHelper = param.DBHelper;
            this._featureClass = param.FeatureClass;
            this._basicField = param.BasicField;
            this._partitionWay = param.PartitionWay;
            this._partitionCount = param.PartitionCount;
            this._tablespaceSet = param.TablespaceSet;
            this._dbtuneKeyWord = param.DbtuneKeyWord;
            this._sdeCopyPath = filePath;

            this._dbtuneHelper = new DbtuneHelper(filePath, this._dbHelper.DBServiceName.Split('/')[0], this._dbHelper.DBPort, this._dbHelper.DBUser, this._dbHelper.DBPwd);
        }

        /// <summary>
        /// 新增并更改一条配置
        /// </summary>
        /// <returns></returns>
        public bool AlterParamValueEX()
        {
            try
            {
                //导出默认配置
                if (this._dbtuneHelper.ExportConfig())
                {
                    //增加配置
                    if (this._dbtuneHelper.AddConfig(this._dbtuneKeyWord))
                    {
                        //导入配置
                        if (this._dbtuneHelper.ImportConfig())
                        {
                            string dbtuneParamValue = string.Empty;

                            //创建配置值
                            if (this._partitionWay == EnumPartitionWay.List)
                            {
                                dbtuneParamValue = CreateDbtuneParamValueInList();
                            }
                            else if (this._partitionWay == EnumPartitionWay.Range)
                            {
                                dbtuneParamValue = CreateDbtuneParamValueInRange();
                            }

                            //替换配置值
                            if (this._dbtuneHelper.AlterConfig(this._dbtuneKeyWord, PartitionKeyWordInfo.DBTUNE_PARAM_NAME, dbtuneParamValue))
                            {
                                DbtuneXMLOperate.InsertKeyWord(new DbtuneInfo()
                                {
                                    KeyWord = this._dbtuneKeyWord,
                                    ParamName = PartitionKeyWordInfo.DBTUNE_PARAM_NAME,
                                    ParamValue = dbtuneParamValue
                                });
                                return true;
                            }
                            else return false;
                        }
                        else return false;
                    }
                    else return false;
                }
                else return false;
            }
            catch (Exception ex)
            {
                LogHelper.Error.Append(ex);

                return false;
            }
        }

        #endregion

        #region 内部方法

        /// <summary>
        /// 获取分区基准字段的基本类型
        /// </summary>
        /// <returns></returns>
        private EnumBaseDataType GetFieldBaseType()
        {
            Dictionary<string, string> columnInfoSet = QueryUnityInfo.QueryColumnInfo(this._featureClass);

            if (RegexCheck.IsExist(ScreenTemplate.ScreenNumericType, this._basicField))
            {
                return EnumBaseDataType.NUMERIC;
            }
            else if (RegexCheck.IsExist(ScreenTemplate.ScreenCharacterType, this._basicField))
            {
                return EnumBaseDataType.CHARACTER;
            }
            else
            {
                return EnumBaseDataType.DATATIME;
            }
        }

        /// <summary>
        /// 在列表分区过程中创建分区详细信息
        /// </summary>
        /// <returns></returns>
        private string CreateDbtuneParamValueInList()
        {
            string dbtuneParamValue = string.Empty;

            dbtuneParamValue = PartitionKeyWordInfo.DBTUNE_PARAM_VALUE_PREFIX + SqlTemplate.CreatePartition.PARTITION_01;
            dbtuneParamValue = dbtuneParamValue.Replace("{basic_field}", this._basicField);

            List<string> uniqueValue = QueryUnityInfo.QueryUniqueValue(this._featureClass, this._basicField);

            string paramSection = string.Empty;

            //按照字段唯一值进行分区信息的生成
            for (int i = 0; i < uniqueValue.Count; i++)
            {
                paramSection += GetFieldBaseType() == EnumBaseDataType.NUMERIC ? SqlTemplate.PartitionInfo.PARTITION_INFO_01 : SqlTemplate.PartitionInfo.PARTITION_INFO_02;
                paramSection = paramSection.Replace("{partition_name}", "PARTITION_" + this._basicField + "_" + i);
                paramSection = paramSection.Replace("{values}", uniqueValue[i]);
                paramSection = paramSection.Replace("{tablespace_name}", this._tablespaceSet[i]);
                paramSection += ",";
            }

            //在最后添加默认分区
            paramSection += SqlTemplate.PartitionInfo.PARTITION_INFO_04;
            paramSection = paramSection.Replace("{partition_name}", "PARTITION_" + this._basicField + "_DEFAULT");
            paramSection = paramSection.Replace("{tablespace_name}", this._tablespaceSet[this._tablespaceSet.Count - 1]);

            dbtuneParamValue = dbtuneParamValue.Replace("{partition_info}", paramSection);

            return dbtuneParamValue;
        }

        /// <summary>
        /// 在范围分区过程中创建分区详细信息
        /// </summary>
        /// <returns></returns>
        private string CreateDbtuneParamValueInRange()
        {
            string dbtuneParamValue = string.Empty;

            dbtuneParamValue = PartitionKeyWordInfo.DBTUNE_PARAM_VALUE_PREFIX + SqlTemplate.CreatePartition.PARTITION_02;
            dbtuneParamValue = dbtuneParamValue.Replace("{basic_field}", this._basicField);

            string paramSection = string.Empty;

            Dictionary<string, double> boundary = QueryUnityInfo.QueryBoundary(this._featureClass, this._basicField);

            List<string> range = QueryUnityInfo.QueryRange(boundary["MIN"], boundary["MAX"], this._partitionCount);

            //按照字段唯一值进行分区信息的生成
            for (int i = 0; i < this._partitionCount; i++)
            {
                paramSection += SqlTemplate.PartitionInfo.PARTITION_INFO_05;
                paramSection = paramSection.Replace("{partition_name}", "PARTITION_" + this._basicField + "_" + i);
                paramSection = paramSection.Replace("{values}", range[i]);
                paramSection = paramSection.Replace("{tablespace_name}", this._tablespaceSet[i]);
                paramSection += ",";
            }
            //在最后添加默认分区
            paramSection += SqlTemplate.PartitionInfo.PARTITION_INFO_07;
            paramSection = paramSection.Replace("{partition_name}", "PARTITION_" + this._basicField + "_MAXVALUE");
            paramSection = paramSection.Replace("{tablespace_name}", this._tablespaceSet[this._tablespaceSet.Count - 1]);

            dbtuneParamValue = dbtuneParamValue.Replace("{partition_info}", paramSection);

            return dbtuneParamValue;
        }

        #endregion
    }
}
