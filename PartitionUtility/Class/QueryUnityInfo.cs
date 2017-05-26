using Geoway.ADF.MIS.DB.Public.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Geoway.ADF.MIS.DB;
using ESRI.ArcGIS.Geodatabase;
using Geoway.ADF.MIS.DB.Public.Enum;
using System.Collections;
using Geoway.ADF.MIS.Utility.Log;

namespace PartitionUtility
{
    /// <summary>
    /// 目的：查询数据库表空间，表，字段及其他通用信息
    /// 创建人：高涛
    /// 创建日期：2017-03-02
    /// 修改描述：
    /// 修改人：
    /// 修改日期：
    /// 备注：
    /// </summary>
    public static class QueryUnityInfo
    {
        /// <summary>
        /// 查询某用户具有权限的表空间
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <returns></returns>
        public static List<string> QueryTablespace(IDBHelper dbHelper)
        {
            try
            {
                dbHelper.TryConnect();

                List<string> tablespaceSet = new List<string>();

                string[] sqlCommand = SqlTemplate.Select.SELECT_06.Split('@');

                DataTable tablespaceResultSet = dbHelper.DoQueryEx(sqlCommand[0]);

                string tablespace = string.Empty;

                for (int i = 0; i < tablespaceResultSet.Rows.Count; i++)
                {
                    tablespace = tablespaceResultSet.Rows[i][sqlCommand[1]].ToString();

                    if (!RegexCheck.IsExist(ScreenTemplate.ScreenLimitTablespace, tablespace))
                    {
                        tablespaceSet.Add(tablespace);
                    }
                }

                return tablespaceSet;
            }
            catch (Exception ex)
            {
                LogHelper.Error.Append(ex);

                throw ex;
            }
        }

        /// <summary>
        /// 查询某表空间中的所有未分区表和分区表
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="tablespaceName"></param>
        /// <returns></returns>
        public static List<string> QueryTotalTable(IDBHelper dbHelper, string tablespaceName)
        {
            try
            {
                List<string> nomalTableSet = QueryNomalTable(dbHelper, tablespaceName);

                List<string> partitionedTableSet = QueryPartitionedTable(dbHelper, tablespaceName);

                List<string> tableSet = nomalTableSet;

                tableSet.AddRange(partitionedTableSet);

                return tableSet;
            }
            catch (Exception ex)
            {
                LogHelper.Error.Append(ex);

                throw ex;
            }
        }

        /// <summary>
        /// 查询某表空间中的未分区表
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="tablespaceName"></param>
        /// <returns></returns>
        public static List<string> QueryNomalTable(IDBHelper dbHelper, string tablespaceName)
        {
            try
            {
                dbHelper.TryConnect();

                List<string> tableSet = new List<string>();

                string[] sqlCommand = SqlTemplate.Select.SELECT_07.Split('@');

                sqlCommand[0] = sqlCommand[0].Replace("{tablespace_name}", tablespaceName.ToUpper());

                DataTable tableResultSet = dbHelper.DoQueryEx(sqlCommand[0]);

                for (int i = 0; i < tableResultSet.Rows.Count; i++)
                {
                    tableSet.Add(tableResultSet.Rows[i][sqlCommand[1]].ToString());
                }

                return tableSet;
            }
            catch (Exception ex)
            {
                LogHelper.Error.Append(ex);

                throw ex;
            }
        }

        /// <summary>
        /// 查询某表空间中的分区表
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="tablespaceName"></param>
        /// <returns></returns>
        public static List<string> QueryPartitionedTable(IDBHelper dbHelper, string tablespaceName)
        {
            try
            {
                dbHelper.TryConnect();

                List<string> tableSet = new List<string>();

                string[] sqlCommand = SqlTemplate.Select.SELECT_08.Split('@');

                sqlCommand[0] = sqlCommand[0].Replace("{tablespace_name}", tablespaceName.ToUpper());

                DataTable tableResultSet = dbHelper.DoQueryEx(sqlCommand[0]);

                for (int i = 0; i < tableResultSet.Rows.Count; i++)
                {
                    tableSet.Add(tableResultSet.Rows[i][sqlCommand[1]].ToString());
                }

                return tableSet;
            }
            catch (Exception ex)
            {
                LogHelper.Error.Append(ex);

                throw ex;
            }
        }

        /// <summary>
        /// 查询某普通表的表结构
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static Dictionary<string, string> QueryColumnInfo(IDBHelper dbHelper, string tableName)
        {
            try
            {
                dbHelper.TryConnect();

                Dictionary<string, string> columnInfoSet = new Dictionary<string, string>();

                List<DBColumnInfo> columnInfoResultSet = dbHelper.GetColumns(tableName);

                string columnIdentity = string.Empty;
                int columnDataTypeId = -1;
                string columnDataType = string.Empty;

                for (int i = 0; i < columnInfoResultSet.Count; i++)
                {
                    columnIdentity = columnInfoResultSet[i].Name.ToUpper();
                    columnDataTypeId = columnInfoResultSet[i].DataType.EnumDBFieldType;

                    switch (columnDataTypeId)
                    {
                        case (int)EnumDBFieldType.FTBlob:
                            columnDataType = EnumDBFieldType.FTBlob.ToString();
                            break;
                        case (int)EnumDBFieldType.FTDate:
                            columnDataType = EnumDBFieldType.FTDate.ToString();
                            break;
                        case (int)EnumDBFieldType.FTDatetime:
                            columnDataType = EnumDBFieldType.FTDatetime.ToString();
                            break;
                        case (int)EnumDBFieldType.FTNumber:
                            columnDataType = EnumDBFieldType.FTNumber.ToString();
                            break;
                        case (int)EnumDBFieldType.FTString:
                            columnDataType = EnumDBFieldType.FTString.ToString();
                            break;
                        case (int)EnumDBFieldType.UnKnown:
                            columnDataType = EnumDBFieldType.UnKnown.ToString();
                            break;
                        default:
                            break;
                    }

                    if (RegexCheck.IsExist(ScreenTemplate.ScreenAllowFieldType, columnDataType))
                    {
                        columnInfoSet.Add(columnIdentity, columnDataType);
                    }
                }

                return columnInfoSet;
            }
            catch (Exception ex)
            {
                LogHelper.Error.Append(ex);

                throw ex;
            }
        }

        /// <summary>
        /// 查询某要素的表结构
        /// </summary>
        /// <param name="featureClass"></param>
        /// <returns></returns>
        public static Dictionary<string, string> QueryColumnInfo(IFeatureClass featureClass)
        {
            try
            {
                Dictionary<string, string> columnInfoSet = new Dictionary<string, string>();

                string columnIdentity = string.Empty;
                string columnDataType = string.Empty;

                for (int i = 0; i < featureClass.Fields.FieldCount; i++)
                {
                    columnIdentity = featureClass.Fields.get_Field(i).Name.ToUpper();
                    columnDataType = featureClass.Fields.get_Field(i).Type.ToString();

                    if (RegexCheck.IsExist(ScreenTemplate.ScreenAllowFieldType, columnDataType))
                    {
                        columnInfoSet.Add(columnIdentity, columnDataType);
                    }
                }

                return columnInfoSet;
            }
            catch (Exception ex)
            {
                LogHelper.Error.Append(ex);

                throw ex;
            }
        }

        /// <summary>
        /// 查询普通表某字段唯一值
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="basicField"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static List<string> QueryUniqueValue(IDBHelper dbHelper, string basicField, string tableName)
        {
            try
            {
                dbHelper.TryConnect();

                List<string> uniqueValueSet = new List<string>();

                //设置SQL模板
                string sqlCommand = SqlTemplate.Select.SELECT_01;
                //替换模板参数
                sqlCommand = sqlCommand.Replace("{table_field}", basicField);
                sqlCommand = sqlCommand.Replace("{table_name}", tableName);

                DataTable uniqueValueResultSet = dbHelper.DoQueryEx(sqlCommand);

                for (int i = 0; i < uniqueValueResultSet.Rows.Count; i++)
                {
                    uniqueValueSet.Add(uniqueValueResultSet.Rows[i][basicField].ToString());
                }

                return uniqueValueSet;
            }
            catch (Exception ex)
            {
                LogHelper.Error.Append(ex);

                throw ex;
            }
        }

        /// <summary>
        /// 查询要素某字段唯一值
        /// </summary>
        /// <param name="basicField"></param>
        /// <param name="featureClass"></param>
        /// <returns></returns>
        public static List<string> QueryUniqueValue(IFeatureClass featureClass, string basicField)
        {
            try
            {
                List<string> uniqueValueSet = new List<string>();

                //得到IFeatureCursor游标
                IFeatureCursor cursor = featureClass.Search(null, false);

                IDataStatistics statistics = new DataStatisticsClass();
                statistics.Field = basicField;
                statistics.Cursor = cursor as ICursor;
                //枚举唯一值
                IEnumerator enumValue = statistics.UniqueValues;

                enumValue.Reset();

                while (enumValue.MoveNext())
                {
                    uniqueValueSet.Add(enumValue.Current.ToString());
                }

                return uniqueValueSet;
            }
            catch (Exception ex)
            {
                LogHelper.Error.Append(ex);

                throw ex;
            }
        }

        /// <summary>
        /// 查询要素某数值类型字段的最大值与最小值
        /// </summary>
        /// <param name="featureClass"></param>
        /// <param name="basicField"></param>
        /// <returns></returns>
        public static Dictionary<string, double> QueryBoundary(IFeatureClass featureClass, string basicField)
        {
            try
            {
                Dictionary<string, double> boundary = new Dictionary<string, double>();

                List<string> uniqueValue = QueryUniqueValue(featureClass, basicField);

                List<double> uniqueValueCV = new List<double>();

                foreach (var value in uniqueValue)
                {
                    uniqueValueCV.Add(double.Parse(value));
                }

                boundary.Add("MIN", uniqueValueCV.ToArray().Min());
                boundary.Add("MAX", uniqueValueCV.ToArray().Max());

                return boundary;
            }
            catch (Exception ex)
            {
                LogHelper.Error.Append(ex);

                throw ex;
            }
        }

        /// <summary>
        /// 查询普通表某数值类型字段的最大值与最小值
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="basicField"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static Dictionary<string, double> QueryBoundary(IDBHelper dbHelper, string basicField, string tableName)
        {
            try
            {
                Dictionary<string, double> boundary = new Dictionary<string, double>();

                boundary.Add("MIN", QueryMinValue(dbHelper, basicField, tableName));
                boundary.Add("MAX", QueryMaxValue(dbHelper, basicField, tableName));

                return boundary;
            }
            catch (Exception ex)
            {
                LogHelper.Error.Append(ex);

                throw ex;
            }
        }

        /// <summary>
        /// 查询普通表某数值类型字段的最小值
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="basicField"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static double QueryMinValue(IDBHelper dbHelper, string basicField, string tableName)
        {
            try
            {
                dbHelper.TryConnect();

                DataTable queryResultSet = new DataTable();

                double minValue;

                string[] sqlCommand = SqlTemplate.Select.SELECT_09.Split('@');

                sqlCommand[0] = sqlCommand[0].Replace("{field_name}", basicField);
                sqlCommand[0] = sqlCommand[0].Replace("{table_name}", tableName);
                sqlCommand[1] = sqlCommand[1].Replace("{field_name}", basicField);

                queryResultSet = dbHelper.DoQueryEx(sqlCommand[0]);
                minValue = Convert.ToDouble(queryResultSet.Rows[0][sqlCommand[1]]);

                return minValue;
            }
            catch (Exception ex)
            {
                LogHelper.Error.Append(ex);

                throw ex;
            }
        }

        /// <summary>
        /// 查询普通表某数值类型字段的最大值
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="basicField"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static double QueryMaxValue(IDBHelper dbHelper, string basicField, string tableName)
        {
            try
            {
                dbHelper.TryConnect();

                DataTable queryResultSet = new DataTable();

                double maxValue;

                string[] sqlCommand = SqlTemplate.Select.SELECT_10.Split('@');

                sqlCommand[0] = sqlCommand[0].Replace("{field_name}", basicField);
                sqlCommand[0] = sqlCommand[0].Replace("{table_name}", tableName);
                sqlCommand[1] = sqlCommand[1].Replace("{field_name}", basicField);

                queryResultSet = dbHelper.DoQueryEx(sqlCommand[0]);
                maxValue = Convert.ToDouble(queryResultSet.Rows[0][sqlCommand[1]]);

                return maxValue;
            }
            catch (Exception ex)
            {
                LogHelper.Error.Append(ex);

                throw ex;
            }
        }

        /// <summary>
        /// 查询范围边界
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<string> QueryRange(double minValue, double maxValue, int count)
        {
            List<string> range = new List<string>();

            for (int i = 0; i < count; i++)
            {
                range.Add((minValue + ((maxValue - minValue) / count) * (i + 1)).ToString());
            }
            return range;
        }
    }
}
