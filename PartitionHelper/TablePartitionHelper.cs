using Geoway.ADF.MIS.DB;
using Geoway.ADF.MIS.DB.Public.Enum;
using Geoway.ADF.MIS.DB.Public.Interface;
using Geoway.ADF.MIS.Utility.Log;
using PartitionUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace PartitionHelper
{
    ///目的：对数据库中的某张表进行分区操作
    ///生成一张新的分区表
    ///创建人：高涛
    ///创建日期：2017-02-19
    ///修改描述：
    ///修改人：
    ///修改日期：
    ///备注：
    /// </summary>
    public class TablePartitionHelper : ITablePartitionHelper
    {
        #region 属性

        /// <summary>
        /// 数据库连接帮助类
        /// </summary>
        private IDBHelper _dbHelper { get; set; }

        /// <summary>
        /// 表空间名
        /// </summary>
        private string _tablespaceName { get; set; }


        /// <summary>
        /// 初始表名
        /// </summary>
        private string _originTableName { get; set; }


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
        /// 分区表表名
        /// </summary>
        private string _partitionedTableName { get; set; }

        /// <summary>
        /// 分区索引
        /// </summary>
        private Dictionary<string, string> _partitionIndex { get; set; }

        /// <summary>
        /// 分区与临时表映射关系
        /// </summary>
        private Dictionary<string, string> _exchangeIndex { get; set; }

        #endregion

        #region 公开方法

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="param"></param>
        public TablePartitionHelper(PartitionParam param)
        {
            this._dbHelper = param.DBHelper;
            this._tablespaceName = param.TablespaceName;
            this._originTableName = param.OriginTableName;
            this._basicField = param.BasicField;
            this._partitionWay = param.PartitionWay;
            this._partitionCount = param.PartitionCount;
            this._tablespaceSet = param.TablespaceSet;
            this._partitionedTableName = param.PartitionedTableName;
        }

        /// <summary>
        /// 删除分区
        /// </summary>
        /// <param name="tableName">目标表名</param>
        /// <param name="dropPartitionName">目标分区名</param>
        /// <returns></returns>
        public bool DropPartition(string tableName, string dropPartitionName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 合并分区
        /// </summary>
        /// <param name="tableName">目标表名</param>
        /// <param name="mergePartitionName_1">要合并的分区1</param>
        /// <param name="mergePartitionName_2">合并目标的分区2</param>
        /// <returns></returns>
        public bool MergePartition(string tableName, string mergePartitionName_1, string mergePartitionName_2)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 通过交换分区将未分区旧表转换为新分区表
        /// </summary>
        public bool PartitionByExchange()
        {
            if (this._dbHelper.TryConnect())
            {
                try
                {
                    this._dbHelper.BeginTransaction();

                    //列表分区
                    if (this._partitionWay == EnumPartitionWay.List)
                    {
                        //获取创建空的分区表的SQL命令
                        string sqlCreatePartitionTable = InitSqlforListPartitionStruct();

                        //获取根据分区创建映射临时表的SQL命令集合
                        List<string> sqlCreateTempTableSet = InitSqlforListTempTable();

                        //获取将分区与临时表交换的SQL命令集合
                        List<string> sqlExchangePartitionSet = InitSqlforExchange();

                        //获取删除临时表的SQL命令集合
                        List<string> sqlDropTempTableSet = InitSqlforDropTemp();

                        //创建新的分区表
                        this._dbHelper.DoSQL(sqlCreatePartitionTable);

                        //依次创建临时表，交换分区并删除临时表
                        for (int i = 0; i < sqlCreateTempTableSet.Count; i++)
                        {
                            this._dbHelper.DoSQL(sqlCreateTempTableSet[i]);
                            this._dbHelper.DoSQL(sqlExchangePartitionSet[i]);
                            this._dbHelper.DoSQL(sqlDropTempTableSet[i]);
                        }

                        this._dbHelper.Commit();
                        return true;
                    }
                    //范围分区
                    else if (this._partitionWay == EnumPartitionWay.Range)
                    {
                        //获取创建空的分区表的SQL命令
                        string sqlCreatePartitionTable = InitSqlforRangePartitionStruct();

                        //获取根据分区创建映射临时表的SQL命令集合
                        List<string> sqlCreateTempTableSet = InitSqlforRangeTempTable();

                        //获取将分区与临时表交换的SQL命令集合
                        List<string> sqlExchangePartitionSet = InitSqlforExchange();

                        //获取删除临时表的SQL命令集合
                        List<string> sqlDropTempTableSet = InitSqlforDropTemp();

                        //创建新的分区表
                        this._dbHelper.DoSQL(sqlCreatePartitionTable);

                        //依次创建临时表，交换分区并删除临时表
                        for (int i = 0; i < sqlCreateTempTableSet.Count; i++)
                        {
                            this._dbHelper.DoSQL(sqlCreateTempTableSet[i]);
                            this._dbHelper.DoSQL(sqlExchangePartitionSet[i]);
                            this._dbHelper.DoSQL(sqlDropTempTableSet[i]);
                        }

                        //提交事务
                        this._dbHelper.Commit();

                        return true;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    //回滚并关闭连接
                    this._dbHelper.Rollback();
                    this._dbHelper.DisConnect();

                    LogHelper.Error.Append(ex);

                    return false;
                }
                finally
                {
                    //关闭连接
                    this._dbHelper.DisConnect();
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 通过插入数据将未分区旧表转换为新分区表
        /// </summary>
        public bool PartitionByInsert()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 通过在线重定义将未分区旧表转换为新分区表
        /// </summary>
        public bool PartitionByRedefinition()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region 内部方法

        /// <summary>
        /// 定义删除交换分区临时表的SQL
        /// </summary>
        /// <returns></returns>
        private List<string> InitSqlforDropTemp()
        {
            List<string> sqlCommands = new List<string>();
            string sqlCommand = string.Empty;

            List<string> keyIndex = new List<string>(this._exchangeIndex.Keys);

            //在分区交换明细字典中检索临时表名，填充至对应SQL模板中
            for (int i = 0; i < keyIndex.Count; i++)
            {
                sqlCommand = SqlTemplate.Remove.DROP;
                sqlCommand = sqlCommand.Replace("{table_name}", keyIndex[i]);
                sqlCommands.Add(sqlCommand);
            }

            return sqlCommands;
        }

        /// <summary>
        /// 定义交换分区SQL
        /// </summary>
        /// <returns></returns>
        private List<string> InitSqlforExchange()
        {
            List<string> sqlCommands = new List<string>();
            string sqlCommand = string.Empty;

            List<string> keyIndex = new List<string>(this._exchangeIndex.Keys);

            //检索分区交换明细表，将分区名与临时表名填充至对应SQL模板中
            for (int i = 0; i < keyIndex.Count; i++)
            {
                sqlCommand = SqlTemplate.Exchange.EXCHANGE_02;

                sqlCommand = sqlCommand.Replace("{table_name_temp}", keyIndex[i]);
                sqlCommand = sqlCommand.Replace("{partition_name}", this._exchangeIndex[keyIndex[i]]);
                sqlCommand = sqlCommand.Replace("{table_name}", this._partitionedTableName);

                sqlCommands.Add(sqlCommand);
            }

            return sqlCommands;
        }

        /// <summary>
        /// 在列表分区过程中定义创建临时表SQL
        /// </summary>
        /// <returns></returns>
        private List<string> InitSqlforListTempTable()
        {
            //实例化分区明细字典，建立各分区与临时表的关系
            this._exchangeIndex = new Dictionary<string, string>();

            List<string> sqlCommands = new List<string>();
            string sqlCommand = string.Empty;

            //获取原表字段集合
            string sqlfieldInfo = GetFieldInfo();

            //获取原表字段信息
            DBColumnInfo dbColumnInfo = this._dbHelper.GetColumn(this._originTableName, this._basicField);
            int columnDataTypeID = dbColumnInfo.DataType.EnumDBFieldType;

            List<string> keyIndex = new List<string>(this._partitionIndex.Keys);

            string tempTableName = string.Empty;

            //根据分区明细表创建各分区对应的临时表
            for (int i = 0; i < keyIndex.Count - 1; i++)
            {
                tempTableName = "TEMP_" + keyIndex[i];

                sqlCommand = GetTempTableInfo(keyIndex[i], columnDataTypeID, sqlfieldInfo, tempTableName, false);

                this._exchangeIndex.Add(tempTableName, keyIndex[i]);

                sqlCommands.Add(sqlCommand);
            }

            //在最后添加默认值临时表
            tempTableName = "TEMP_" + keyIndex[keyIndex.Count - 1];
            sqlCommand = GetTempTableInfo(keyIndex[keyIndex.Count - 1], columnDataTypeID, sqlfieldInfo, tempTableName, true);
            this._exchangeIndex.Add(tempTableName, keyIndex[keyIndex.Count - 1]);

            sqlCommands.Add(sqlCommand);

            return sqlCommands;
        }

        /// <summary>
        /// 在范围分区过程中定义创建临时表SQL
        /// </summary>
        /// <returns></returns>
        private List<string> InitSqlforRangeTempTable()
        {
            //实例化分区明细字典，建立各分区与临时表的关系
            this._exchangeIndex = new Dictionary<string, string>();

            List<string> sqlCommands = new List<string>();
            string sqlCommand = string.Empty;

            //获取原表字段集合
            string sqlfieldInfo = GetFieldInfo();

            //获取原表字段信息
            DBColumnInfo dbColumnInfo = this._dbHelper.GetColumn(this._originTableName, this._basicField);
            int columnDataTypeID = dbColumnInfo.DataType.EnumDBFieldType;

            List<string> keyIndex = new List<string>(this._partitionIndex.Keys);

            string tempTableName = string.Empty;

            //根据分区明细表创建各分区对应的临时表
            for (int i = 0; i < keyIndex.Count - 1; i++)
            {
                if (i == 0)
                {
                    //第一位应小于
                    tempTableName = "TEMP_" + keyIndex[i];
                    sqlCommand = GetTempTableInfoLeft(keyIndex[i], columnDataTypeID, sqlfieldInfo, tempTableName);
                    this._exchangeIndex.Add(tempTableName, keyIndex[i]);
                    sqlCommands.Add(sqlCommand);
                }
                else if (i == keyIndex.Count - 2)
                {
                    //最后一位应大于
                    tempTableName = "TEMP_" + keyIndex[i];
                    sqlCommand = GetTempTableInfoRight(keyIndex[i], columnDataTypeID, sqlfieldInfo, tempTableName);
                    this._exchangeIndex.Add(tempTableName, keyIndex[i + 1]);
                    sqlCommands.Add(sqlCommand);

                    break;
                }
                //中间的值应小于且大于
                tempTableName = "TEMP_" + keyIndex[i] + "_M";
                sqlCommand = GetTempTableInfoCenter(keyIndex[i], keyIndex[i + 1], columnDataTypeID, sqlfieldInfo, tempTableName);
                this._exchangeIndex.Add(tempTableName, keyIndex[i + 1]);

                sqlCommands.Add(sqlCommand);
            }

            return sqlCommands;
        }

        /// <summary>
        ///  在列表分区过程中定义创建表结构SQL
        /// </summary>
        /// <returns></returns>
        private string InitSqlforListPartitionStruct()
        {
            string sqlCommand = string.Empty;

            sqlCommand = GetTableStructInfo();

            #region 构建SQL命令用于创建分区

            //选择SQL模板
            sqlCommand += SqlTemplate.CreatePartition.PARTITION_01;
            sqlCommand = sqlCommand.Replace("{basic_field}", this._basicField);

            DBColumnInfo dbColumnInfo = this._dbHelper.GetColumn(this._originTableName, this._basicField);

            string sqlQuery = GetUniqueValue();
            DataTable UniqueValueTable = this._dbHelper.DoQueryEx(sqlQuery);

            //根据不同字段类型创建SQL命令填入模板
            if (dbColumnInfo.DataType.EnumDBFieldType == (int)EnumDBFieldType.FTString)
            {
                sqlCommand = sqlCommand.Replace("{partition_info}", GetListPartitionInfo(UniqueValueTable, EnumDBFieldType.FTString));
            }
            else if (dbColumnInfo.DataType.EnumDBFieldType == (int)EnumDBFieldType.FTNumber)
            {
                sqlCommand = sqlCommand.Replace("{partition_info}", GetListPartitionInfo(UniqueValueTable, EnumDBFieldType.FTNumber));
            }
            else if (dbColumnInfo.DataType.EnumDBFieldType == (int)EnumDBFieldType.FTDatetime)
            {
                sqlCommand = sqlCommand.Replace("{partition_info}", GetListPartitionInfo(UniqueValueTable, EnumDBFieldType.FTDatetime));
            }
            #endregion

            return sqlCommand;
        }

        /// <summary>
        /// 在范围分区过程中定义创建表结构SQL
        /// </summary>
        /// <returns></returns>
        private string InitSqlforRangePartitionStruct()
        {
            string sqlCommand = string.Empty;

            sqlCommand = GetTableStructInfo();

            this._partitionIndex = new Dictionary<string, string>();

            //选择SQL模板
            sqlCommand += SqlTemplate.CreatePartition.PARTITION_02;
            sqlCommand = sqlCommand.Replace("{basic_field}", this._basicField);

            string sqlPartitionInfo = string.Empty;
            string partitionName = string.Empty;

            DBColumnInfo dbColumnInfo = this._dbHelper.GetColumn(this._originTableName, this._basicField);

            Dictionary<string, double> boundary = QueryUnityInfo.QueryBoundary(this._dbHelper, this._basicField, this._originTableName);

            List<string> range = QueryUnityInfo.QueryRange(boundary["MIN"], boundary["MAX"], this._partitionCount);

            //若分区列是日期
            if (dbColumnInfo.DataType.EnumDBFieldType == (int)EnumDBFieldType.FTDatetime)
            {
                for (int i = 0; i < range.Count; i++)
                {
                    #region 定义分区名
                    partitionName = "PARTITION_" + this._basicField + "_" + i;
                    #endregion

                    //选择对应SQL模板
                    sqlPartitionInfo += SqlTemplate.PartitionInfo.PARTITION_INFO_06;
                    //向模板中填入参数
                    sqlPartitionInfo = sqlPartitionInfo.Replace("{partition_name}", partitionName);
                    sqlPartitionInfo = sqlPartitionInfo.Replace("{tablespace_name}", this._tablespaceSet[i]);
                    sqlPartitionInfo = sqlPartitionInfo.Replace("{values}", range[i]);

                    sqlPartitionInfo += ",";

                    this._partitionIndex.Add(partitionName, range[i]);
                }
            }
            //若分区列不是日期
            else
            {
                for (int i = 0; i < range.Count; i++)
                {
                    #region 定义分区名
                    partitionName = "PARTITION_" + this._basicField + "_" + i;
                    #endregion
                    //选择对应SQL模板
                    sqlPartitionInfo += SqlTemplate.PartitionInfo.PARTITION_INFO_05;
                    //向模板中填入参数
                    sqlPartitionInfo = sqlPartitionInfo.Replace("{partition_name}", partitionName);
                    sqlPartitionInfo = sqlPartitionInfo.Replace("{tablespace_name}", this._tablespaceSet[i]);
                    sqlPartitionInfo = sqlPartitionInfo.Replace("{values}", range[i]);

                    sqlPartitionInfo += ",";

                    this._partitionIndex.Add(partitionName, range[i]);
                }
            }

            //在最后添加[MAXVALUE]分区
            #region 定义分区名
            partitionName = "PARTITION_" + this._basicField + "_MAXVALUE";
            sqlPartitionInfo += SqlTemplate.PartitionInfo.PARTITION_INFO_07;
            sqlPartitionInfo = sqlPartitionInfo.Replace("{partition_name}", partitionName);
            sqlPartitionInfo = sqlPartitionInfo.Replace("{tablespace_name}", this._tablespaceSet[this._tablespaceSet.Count - 1]);
            #endregion

            this._partitionIndex.Add(partitionName, "MAXVALUE");

            sqlCommand = sqlCommand.Replace("{partition_info}", sqlPartitionInfo);

            return sqlCommand;
        }

        /// <summary>
        /// 获取表结构信息SQL
        /// </summary>
        /// <returns></returns>
        private string GetTableStructInfo()
        {
            //设置SQL模板
            string sqlSection = SqlTemplate.Create.CREATE_02;

            //替换模板参数
            sqlSection = sqlSection.Replace("{table_fields}", GetColumnInfo());
            sqlSection = sqlSection.Replace("{table_name}", this._partitionedTableName);
            sqlSection = sqlSection.Replace("{tablespace_name}", this._tablespaceName);

            return sqlSection;
        }

        /// <summary>
        /// 获取列信息SQL
        /// </summary>
        /// <returns></returns>
        private string GetColumnInfo()
        {
            string sqlSection = string.Empty;

            List<DBColumnInfo> dbColumnInfoSet = this._dbHelper.GetColumns(this._originTableName);

            //根据字段类型补全SQL模板
            for (int i = 0; i < dbColumnInfoSet.Count; i++)
            {
                switch (dbColumnInfoSet[i].DataType.EnumDBFieldType)
                {
                    //字符串
                    case (int)EnumDBFieldType.FTString:
                        sqlSection += GetColumnInfoByType(dbColumnInfoSet[i], EnumDataTableFieldType.VARCHAR2);
                        break;
                    //数值类型
                    case (int)EnumDBFieldType.FTNumber:
                        sqlSection += GetColumnInfoByType(dbColumnInfoSet[i], EnumDataTableFieldType.NUMBER);
                        break;
                    //日期
                    case (int)EnumDBFieldType.FTDatetime:
                        sqlSection += GetColumnInfoByType(dbColumnInfoSet[i], EnumDataTableFieldType.DATETIME);
                        break;
                    default:
                        break;
                }
                if (i != dbColumnInfoSet.Count - 1)
                {
                    sqlSection += ",";
                }
            }
            return sqlSection;
        }

        /// <summary>
        /// 获取字段信息SQL
        /// </summary>
        /// <returns></returns>
        private string GetFieldInfo()
        {
            string sqlSection = string.Empty;

            List<DBColumnInfo> dbColumnInfoSet = this._dbHelper.GetColumns(this._originTableName);

            for (int i = 0; i < dbColumnInfoSet.Count; i++)
            {
                sqlSection += dbColumnInfoSet[i].Name;

                if (i != dbColumnInfoSet.Count - 1)
                {
                    sqlSection += ",";
                }
            }

            return sqlSection;
        }

        /// <summary>
        /// 获取字段唯一值SQL
        /// </summary>
        /// <returns></returns>
        private string GetUniqueValue()
        {
            //设置SQL模板
            string sqlSection = SqlTemplate.Select.SELECT_01;
            //替换模板参数
            sqlSection = sqlSection.Replace("{table_field}", this._basicField);
            sqlSection = sqlSection.Replace("{table_name}", this._originTableName);

            return sqlSection;
        }

        /// <summary>
        /// 在列表分区过程中获取SQL定义临时表SQL
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="dataTypeID"></param>
        /// <param name="fieldInfo"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private string GetTempTableInfo(string keyValue, int dataTypeID, string fieldInfo, string tableName, bool isDefault)
        {
            string sqlSection = string.Empty;

            if (isDefault)
            {
                sqlSection = SqlTemplate.CreateTemp.CREATE_TEMP_10;
            }
            else
            {
                //根据字段类型选择SQL模板
                switch (dataTypeID)
                {
                    case (int)EnumDBFieldType.FTNumber:
                        sqlSection = SqlTemplate.CreateTemp.CREATE_TEMP_01;
                        break;
                    case (int)EnumDBFieldType.FTString:
                        sqlSection = SqlTemplate.CreateTemp.CREATE_TEMP_02;
                        break;
                    case (int)EnumDBFieldType.FTDatetime:
                        sqlSection = SqlTemplate.CreateTemp.CREATE_TEMP_03;
                        break;
                    default:
                        break;
                }
            }

            //替换模板参数
            sqlSection = sqlSection.Replace("{table_name}", tableName);
            sqlSection = sqlSection.Replace("{table_fields}", fieldInfo);
            sqlSection = sqlSection.Replace("{table_name_old}", this._originTableName);
            sqlSection = sqlSection.Replace("{basicfield}", this._basicField);
            sqlSection = sqlSection.Replace("{values}", this._partitionIndex[keyValue]);
            sqlSection = sqlSection.Replace("{tablespace_name}", this._tablespaceName);

            return sqlSection;
        }

        /// <summary>
        /// 在范围分区过程中获取SQL定义临时表SQL（小于某值）
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="dataTypeID"></param>
        /// <param name="fieldInfo"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private string GetTempTableInfoLeft(string keyValue, int dataTypeID, string fieldInfo, string tableName)
        {
            string sqlSection = string.Empty;

            //根据字段类型选择SQL模板
            switch (dataTypeID)
            {
                case (int)EnumDBFieldType.FTNumber:
                    sqlSection = SqlTemplate.CreateTemp.CREATE_TEMP_04;
                    break;
                case (int)EnumDBFieldType.FTDatetime:
                    sqlSection = SqlTemplate.CreateTemp.CREATE_TEMP_05;
                    break;
                default:
                    break;
            }
            //替换模板参数
            sqlSection = sqlSection.Replace("{table_name}", tableName);
            sqlSection = sqlSection.Replace("{table_fields}", fieldInfo);
            sqlSection = sqlSection.Replace("{table_name_old}", this._originTableName);
            sqlSection = sqlSection.Replace("{basicfield}", this._basicField);
            sqlSection = sqlSection.Replace("{values}", this._partitionIndex[keyValue]);
            sqlSection = sqlSection.Replace("{tablespace_name}", this._tablespaceName);

            return sqlSection;
        }

        /// <summary>
        /// 在范围分区过程中获取SQL定义临时表SQL（在某值之间）
        /// </summary>
        /// <param name="keyValue_1"></param>
        /// <param name="keyValue_2"></param>
        /// <param name="dataTypeID"></param>
        /// <param name="fieldInfo"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private string GetTempTableInfoCenter(string keyValue_1, string keyValue_2, int dataTypeID, string fieldInfo, string tableName)
        {
            string sqlSection = string.Empty;
            //根据字段类型选择SQL模板
            switch (dataTypeID)
            {
                case (int)EnumDBFieldType.FTNumber:
                    sqlSection = SqlTemplate.CreateTemp.CREATE_TEMP_06;
                    break;
                case (int)EnumDBFieldType.FTDatetime:
                    sqlSection = SqlTemplate.CreateTemp.CREATE_TEMP_07;
                    break;
                default:
                    break;
            }
            //替换模板参数
            sqlSection = sqlSection.Replace("{table_name}", tableName);
            sqlSection = sqlSection.Replace("{table_fields}", fieldInfo);
            sqlSection = sqlSection.Replace("{table_name_old}", this._originTableName);
            sqlSection = sqlSection.Replace("{basicfield}", this._basicField);
            sqlSection = sqlSection.Replace("{values_1}", this._partitionIndex[keyValue_1]);
            sqlSection = sqlSection.Replace("{values_2}", this._partitionIndex[keyValue_2]);
            sqlSection = sqlSection.Replace("{tablespace_name}", this._tablespaceName);

            return sqlSection;
        }

        /// <summary>
        /// 在范围分区过程中获取SQL定义临时表SQL（大于于某值）
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="dataTypeID"></param>
        /// <param name="fieldInfo"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private string GetTempTableInfoRight(string keyValue, int dataTypeID, string fieldInfo, string tableName)
        {
            string sqlSection = string.Empty;
            //根据字段类型选择SQL模板
            switch (dataTypeID)
            {
                case (int)EnumDBFieldType.FTNumber:
                    sqlSection = SqlTemplate.CreateTemp.CREATE_TEMP_08;
                    break;
                case (int)EnumDBFieldType.FTDatetime:
                    sqlSection = SqlTemplate.CreateTemp.CREATE_TEMP_09;
                    break;
                default:
                    break;
            }
            //替换模板参数
            sqlSection = sqlSection.Replace("{table_name}", tableName);
            sqlSection = sqlSection.Replace("{table_fields}", fieldInfo);
            sqlSection = sqlSection.Replace("{table_name_old}", this._originTableName);
            sqlSection = sqlSection.Replace("{basicfield}", this._basicField);
            sqlSection = sqlSection.Replace("{values}", this._partitionIndex[keyValue]);
            sqlSection = sqlSection.Replace("{tablespace_name}", this._tablespaceName);

            return sqlSection;
        }

        /// <summary>
        /// 在列表分区过程中获取定义分区信息SQL
        /// </summary>
        /// <param name="UniqueValueTable"></param>
        /// <param name="BasicFieldDataType"></param>
        /// <returns></returns>
        private string GetListPartitionInfo(DataTable UniqueValueTable, EnumDBFieldType BasicFieldDataType)
        {
            string sqlSection = string.Empty;
            List<string> UniqueValueSet = new List<string>();
            this._partitionIndex = new Dictionary<string, string>();

            //获取分区列数据
            foreach (DataRow dataRow in UniqueValueTable.Rows)
            {
                UniqueValueSet.Add(dataRow[this._basicField].ToString());
            }

            string partitionName = string.Empty;

            //在SQL模板中添加分区列
            for (int i = 0; i < UniqueValueSet.Count; i++)
            {
                partitionName = "PARTITION_" + this._basicField + "_" + i;

                sqlSection += GetPartitionInfo(UniqueValueSet[i], BasicFieldDataType, partitionName, this._tablespaceSet[i]);

                sqlSection += ",";
            }

            //在最后添加[DEFAULT]分区
            partitionName = "PARTITION_" + this._basicField + "_DEFAULT";
            sqlSection += SqlTemplate.PartitionInfo.PARTITION_INFO_04;
            sqlSection = sqlSection.Replace("{partition_name}", partitionName);
            sqlSection = sqlSection.Replace("{tablespace_name}", this._tablespaceSet[this._tablespaceSet.Count - 1]);

            //在分区明细字典中添加分区名与分区列
            this._partitionIndex.Add(partitionName, "DEFAULT");

            return sqlSection;
        }

        /// <summary>
        /// 在列表分区过程中获取定义分区信息的SQL（辅助）
        /// </summary>
        /// <param name="value"></param>
        /// <param name="BasicFieldDataType"></param>
        /// <returns></returns>
        private string GetPartitionInfo(string value, EnumDBFieldType BasicFieldDataType, string partitionName, string tablespaceName)
        {
            string sqlSection = string.Empty;

            //根据字段类型选择分区列SQL模板
            switch (BasicFieldDataType)
            {
                case EnumDBFieldType.UnKnown:
                    break;
                case EnumDBFieldType.FTNumber:
                    sqlSection = SqlTemplate.PartitionInfo.PARTITION_INFO_01;
                    break;
                case EnumDBFieldType.FTString:
                    sqlSection = SqlTemplate.PartitionInfo.PARTITION_INFO_02;
                    break;
                case EnumDBFieldType.FTDate:
                    break;
                case EnumDBFieldType.FTDatetime:
                    sqlSection = SqlTemplate.PartitionInfo.PARTITION_INFO_03;
                    break;
                default:
                    break;
            }

            //替换SQL模板参数
            sqlSection = sqlSection.Replace("{partition_name}", partitionName).Replace("{values}", value);
            sqlSection = sqlSection.Replace("{tablespace_name}", tablespaceName);

            this._partitionIndex.Add(partitionName, value);

            return sqlSection;
        }

        /// <summary>
        /// 按照字段类型获取列信息SQL
        /// </summary>
        /// <param name="dbColumnInfo"></param>
        /// <param name="fieldDataType"></param>
        /// <returns></returns>
        private string GetColumnInfoByType(DBColumnInfo dbColumnInfo, EnumDataTableFieldType fieldDataType)
        {
            string sqlSection = string.Empty;

            //根据字段类型选择数据库字段定义SQL模板
            switch (dbColumnInfo.DataType.EnumDBFieldType)
            {
                case (int)EnumDBFieldType.FTNumber:
                    if (dbColumnInfo.Nullable)
                    {
                        sqlSection = SqlTemplate.FieldInfo.FIELD_INFO_01;
                    }
                    else
                    {
                        sqlSection = SqlTemplate.FieldInfo.FIELD_INFO_04;
                    }
                    break;
                case (int)EnumDBFieldType.FTString:
                    //根据是否为空选择不同SQL模板
                    if (dbColumnInfo.Nullable)
                    {
                        sqlSection = SqlTemplate.FieldInfo.FIELD_INFO_02;
                    }
                    else
                    {
                        sqlSection = SqlTemplate.FieldInfo.FIELD_INFO_05;
                    }
                    break;
                case (int)EnumDBFieldType.FTDatetime:
                    if (dbColumnInfo.Nullable)
                    {
                        sqlSection = SqlTemplate.FieldInfo.FIELD_INFO_03;
                    }
                    else
                    {
                        sqlSection = SqlTemplate.FieldInfo.FIELD_INFO_06;
                    }
                    break;
                default:
                    break;
            }

            //替换SQL模板参数
            switch (dbColumnInfo.DataType.EnumDBFieldType)
            {
                case (int)EnumDBFieldType.FTString:
                    sqlSection = sqlSection.Replace("{field_length}", dbColumnInfo.Length.ToString());
                    break;
                case (int)EnumDBFieldType.FTNumber:
                    sqlSection = sqlSection.Replace("{field_precision}", dbColumnInfo.Precision.ToString());
                    sqlSection = sqlSection.Replace("{field_scale}", dbColumnInfo.Scale.ToString());
                    break;
                default:
                    break;
            }

            sqlSection = sqlSection.Replace("{field_name}", dbColumnInfo.Name);
            sqlSection = sqlSection.Replace("{field_type}", fieldDataType.GetDescription());

            return sqlSection;
        }

        #endregion

    }
}
