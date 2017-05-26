using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PartitionUtility
{
    /// <summary>
    ///目的：提供SQL常用命令的字符串常量模板常量字符
    ///创建人：高涛
    ///创建日期：2017-02-17
    ///修改描述：
    ///修改人：
    ///修改日期：
    ///备注：
    /// </summary>
    public class SqlTemplate
    {
        public static class Create
        {
            /// <summary>
            /// 创建新表SQL模板
            /// </summary>
            public const string CREATE_01 = "CREATE TABLE {table_name} ({table_fields})";
            /// <summary>
            /// 创建新表SQL模板，附加表空间名
            /// </summary>
            public const string CREATE_02 = "CREATE TABLE {table_name} ({table_fields}) TABLESPACE {tablespace_name}";
        }

        /// <summary>
        /// 创建临时表
        /// </summary>
        public static class CreateTemp
        {
            /// <summary>
            /// 创建临时表，分区列为数值型
            /// </summary>
            public const string CREATE_TEMP_01 = "CREATE TABLE {table_name} TABLESPACE {tablespace_name} AS SELECT {table_fields} FROM {table_name_old} WHERE {basicfield} = {values}";
            /// <summary>
            /// 创建临时表，分区列为字符型
            /// </summary>
            public const string CREATE_TEMP_02 = "CREATE TABLE {table_name} TABLESPACE {tablespace_name} AS SELECT {table_fields} FROM {table_name_old} WHERE {basicfield} = '{values}'";
            /// <summary>
            /// 创建临时表，分区列为日期型
            /// </summary>
            public const string CREATE_TEMP_03 = "CREATE TABLE {table_name} TABLESPACE {tablespace_name} AS SELECT {table_fields} FROM {table_name_old} WHERE {basicfield} = TO_DATE('{values}','YYYY-MM-DD')";
            /// <summary>
            /// 创建临时表，分区列为数值型，小于某范围
            /// </summary>
            public const string CREATE_TEMP_04 = "CREATE TABLE {table_name} TABLESPACE {tablespace_name} AS SELECT {table_fields} FROM {table_name_old} WHERE {basicfield} <= {values}";
            /// <summary>
            /// 创建临时表，分区列为日期型，小于某范围
            /// </summary>
            public const string CREATE_TEMP_05 = "CREATE TABLE {table_name} TABLESPACE {tablespace_name} AS SELECT {table_fields} FROM {table_name_old} WHERE {basicfield} <= TO_DATE('{values}','YYYY-MM-DD')";
            /// <summary>
            /// 创建临时表，分区列为数值型，在某范围之间
            /// </summary>
            public const string CREATE_TEMP_06 = "CREATE TABLE {table_name} TABLESPACE {tablespace_name} AS SELECT {table_fields} FROM {table_name_old} WHERE {basicfield} > {values_1} AND {basicfield} <= {values_2}";
            /// <summary>
            /// 创建临时表，分区列为日期型，在某范围之间
            /// </summary>
            public const string CREATE_TEMP_07 = "CREATE TABLE {table_name} TABLESPACE {tablespace_name} AS SELECT {table_fields} FROM {table_name_old} WHERE {basicfield} > TO_DATE('{values_1}','YYYY-MM-DD') AND {basicfield} <= TO_DATE('{values_2}','YYYY-MM-DD')";
            /// <summary>
            /// 创建临时表，分区列为数值型，大于某范围
            /// </summary>
            public const string CREATE_TEMP_08 = "CREATE TABLE {table_name} TABLESPACE {tablespace_name} AS SELECT {table_fields} FROM {table_name_old} WHERE {basicfield} > {values} OR {basicfield} IS NULL";
            /// <summary>
            /// 创建临时表，分区列为日期型，大于某范围
            /// </summary>
            public const string CREATE_TEMP_09 = "CREATE TABLE {table_name} TABLESPACE {tablespace_name} AS SELECT {table_fields} FROM {table_name_old} WHERE {basicfield} > TO_DATE('{values}','YYYY-MM-DD') OR {basicfield} IS NULL";
            /// <summary>
            /// 创建临时表，空值
            /// </summary>
            public const string CREATE_TEMP_10 = "CREATE TABLE {table_name} TABLESPACE {tablespace_name} AS SELECT {table_fields} FROM {table_name_old} WHERE {basicfield} IS NULL";
        }

        /// <summary>
        /// 删除表
        /// </summary>
        public static class Remove
        {
            /// <summary>
            /// 删除表结构及内容，无回滚段
            /// </summary>
            public const string DROP = "DROP TABLE {table_name} PURGE";
            /// <summary>
            /// 删除表内容，无回滚段并释放空间
            /// </summary>
            public const string TRUNCATE = "TRUNCATE TABLE {table_name}";
            /// <summary>
            /// 删除表内容，附加限制
            /// </summary>
            public const string DELETE = "DELETE FROM {table_name} WHERE {limit_info}";
        }

        /// <summary>
        /// 交换分区
        /// </summary>
        public static class Exchange
        {
            /// <summary>
            /// 交换临时表与对应分区，表结构需一致
            /// </summary>
            public const string EXCHANGE_01 = "ALTER TABLE {table_name} EXCHANGE PARTITION {partition_name} WITH TABLE {table_name_temp}";
            /// <summary>
            /// 交换临时表与对应分区，不检查表结构，慎用
            /// </summary>
            public const string EXCHANGE_02 = "ALTER TABLE {table_name} EXCHANGE PARTITION {partition_name} WITH TABLE {table_name_temp} WITHOUT VALIDATION";
        }

        /// <summary>
        /// 单个字段信息
        /// </summary>
        public static class FieldInfo
        {
            /// <summary>
            /// 数值字段信息，可空
            /// </summary>
            public const string FIELD_INFO_01 = "{field_name} {field_type}({field_precision},{field_scale})";
            /// <summary>
            /// 字符型字段信息，可空
            /// </summary>
            public const string FIELD_INFO_02 = "{field_name} {field_type}({field_length})";
            /// <summary>
            /// 日期型字段信息，可空
            /// </summary>
            public const string FIELD_INFO_03 = "{field_name} {field_type}";
            /// <summary>
            /// 数值字段信息，不可空
            /// </summary>
            public const string FIELD_INFO_04 = "{field_name} {field_type}({field_precision},{field_scale}) NOT NULL";
            /// <summary>
            /// 字符型字段信息，不可空
            /// </summary>
            public const string FIELD_INFO_05 = "{field_name} {field_type}({field_length}) NOT NULL";
            /// <summary>
            /// 日期型字段信息，不可空
            /// </summary>
            public const string FIELD_INFO_06 = "{field_name} {field_type} NOT NULL";
        }

        /// <summary>
        /// 创建表
        /// </summary>
        public static class CreatePartition
        {
            /// <summary>
            /// 按列表分区
            /// </summary>
            public const string PARTITION_01 = " PARTITION BY LIST({basic_field}) ({partition_info})";
            /// <summary>
            /// 按范围分区
            /// </summary>
            public const string PARTITION_02 = " PARTITION BY RANGE({basic_field}) ({partition_info})";
        }

        /// <summary>
        /// 单个分区信息
        /// </summary>
        public static class PartitionInfo
        {
            /// <summary>
            /// 列表分区，分区列为数值型，指定表空间
            /// </summary>
            public const string PARTITION_INFO_01 = "PARTITION {partition_name} VALUES({values}) TABLESPACE {tablespace_name}";
            /// <summary>
            /// 列表分区，分区列为字符型，指定表空间
            /// </summary>
            public const string PARTITION_INFO_02 = "PARTITION {partition_name} VALUES('{values}') TABLESPACE {tablespace_name}";
            /// <summary>
            /// 列表分区，分区列为日期型，指定表空间
            /// </summary>
            public const string PARTITION_INFO_03 = "PARTITION {partition_name} VALUES(TO_DATE('{values}','YYYY-MM-DD')) TABLESPACE {tablespace_name}";
            /// <summary>
            /// 列表分区，超限分区，指定表空间
            /// </summary>
            public const string PARTITION_INFO_04 = "PARTITION {partition_name} VALUES(DEFAULT) TABLESPACE {tablespace_name}";
            /// <summary>
            /// 范围分区，分区列为数值型，指定表空间
            /// </summary>
            public const string PARTITION_INFO_05 = "PARTITION {partition_name} VALUES LESS THAN ({values}) TABLESPACE {tablespace_name}";
            /// <summary>
            /// 范围分区，分区列为日期型，指定表空间
            /// </summary>
            public const string PARTITION_INFO_06 = "PARTITION {partition_name} VALUES LESS THAN (TO_DATE('{values}','YYYY-MM-DD')) TABLESPACE {tablespace_name}";
            /// <summary>
            /// 范围分区，超限分区，指定表空间
            /// </summary>
            public const string PARTITION_INFO_07 = "PARTITION {partition_name} VALUES LESS THAN (MAXVALUE) TABLESPACE {tablespace_name}";

            /// <summary>
            /// 列表分区，分区列为数值型
            /// </summary>
            public const string PARTITION_INFO_08 = "PARTITION {partition_name} VALUES({values})";
            /// <summary>
            /// 列表分区，分区列为字符型
            /// </summary>
            public const string PARTITION_INFO_09 = "PARTITION {partition_name} VALUES('{values}')";
            /// <summary>
            /// 列表分区，超限分区
            /// </summary>
            public const string PARTITION_INFO_10 = "PARTITION {partition_name} VALUES(DEFAULT)";
            /// <summary>
            /// 范围分区
            /// </summary>
            public const string PARTITION_INFO_11 = "PARTITION {partition_name} VALUES LESS THAN ({values})";
            /// 范围分区，超限分区
            /// </summary>
            public const string PARTITION_INFO_12 = "PARTITION {partition_name} VALUES LESS THAN (MAXVALUE)";
        }

        /// <summary>
        /// 查询字段
        /// </summary>
        public static class Select
        {
            /// <summary>
            /// 排重查询字段
            /// </summary>
            public const string SELECT_01 = "SELECT DISTINCT {table_field} FROM {table_name} WHERE {table_field} IS NOT NULL";
            /// <summary>
            /// 查询所有字段
            /// </summary>
            public const string SELECT_02 = "SELECT * FROM {table_name}";
            /// <summary>
            /// 查询部分字段
            /// </summary>
            public const string SELECT_03 = "SELECT {table_fields} FROM {table_name}";
            /// <summary>
            /// 查询所有字段，限制分区名
            /// </summary>
            public const string SELECT_04 = "SELECT * FROM {table_name} PARTITION({partition_name})";
            /// <summary>
            /// 查询部分字段，限制分区名
            /// </summary>
            public const string SELECT_05 = "SELECT {table_fields} FROM {table_name} PARTITION({partition_name})";
            /// <summary>
            /// 查询某数据库用户具有权限的表空间
            /// </summary>
            public const string SELECT_06 = "SELECT TABLESPACE_NAME FROM USER_TABLESPACES@TABLESPACE_NAME";
            /// <summary>
            /// 查询某表空间下用户具有权限的表
            /// </summary>
            public const string SELECT_07 = "SELECT TABLE_NAME FROM USER_TABLES WHERE TABLESPACE_NAME = '{tablespace_name}'@TABLE_NAME";
            /// <summary>
            /// 查询某表空间下的分区表
            /// </summary>
            public const string SELECT_08 = "SELECT DISTINCT SEGMENT_NAME FROM USER_SEGMENTS WHERE SEGMENT_TYPE = 'TABLE PARTITION' AND TABLESPACE_NAME = '{tablespace_name}'@SEGMENT_NAME";
            /// <summary>
            /// 查询某列最小值
            /// </summary>
            public const string SELECT_09 = "SELECT MIN({field_name}) FROM {table_name}@MIN({field_name})";
            /// <summary>
            /// 查询某列最大值
            /// </summary>
            public const string SELECT_10 = "SELECT MAX({field_name}) FROM {table_name}@MAX({field_name})";
        }

        /// <summary>
        /// 单个值信息
        /// </summary>
        public static class ValueInfo
        {
            /// <summary>
            /// 数值型
            /// </summary>
            public const string VALUE_INFO_01 = "{values}";
            /// <summary>
            /// 字符型
            /// </summary>
            public const string VALUE_INFO_02 = "'{values}'";
            /// <summary>
            /// 日期型
            /// </summary>
            public const string VALUE_INFO_03 = "TO_DATE('{values}','YYYY-MM-DD')";
        }
    }
}
