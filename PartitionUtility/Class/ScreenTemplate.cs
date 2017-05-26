using ESRI.ArcGIS.Geodatabase;
using Geoway.ADF.MIS.DB.Public.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PartitionUtility
{
    /// <summary>
    ///目的：提供数据或字段筛选模板
    ///创建人：高涛
    ///创建日期：2017-02-17
    ///修改描述：
    ///修改人：
    ///修改日期：
    ///备注：
    /// </summary>
    public static class ScreenTemplate
    {
        /// <summary>
        /// 用以筛选限制访问的表空间
        /// 包含一些系统表空间，回滚表空间等
        /// </summary>
        public readonly static List<string> ScreenLimitTablespace = new List<string>()
        {
            "SYSTEM",
            "SYSAUX",
            "UNDOTBS1",
            "TEMP",
            "USERS",
        };

        /// <summary>
        /// 用以筛选允许的字段类型
        /// </summary>
        public readonly static List<string> ScreenAllowFieldType = new List<string>()
        {
            EnumDBFieldType.FTString.ToString(),
            EnumDBFieldType.FTNumber.ToString(),
            //EnumDBFieldType.FTDatetime.ToString(), //暂时屏蔽时间类型的字段
            esriFieldType.esriFieldTypeDouble.ToString(),
            esriFieldType.esriFieldTypeInteger.ToString(),
            esriFieldType.esriFieldTypeSmallInteger.ToString(),
            esriFieldType.esriFieldTypeString.ToString(),
            esriFieldType.esriFieldTypeOID.ToString()
        };

        /// <summary>
        /// 用以筛选数值型字段
        /// </summary>
        public readonly static List<string> ScreenNumericType = new List<string>()
        {
            EnumDBFieldType.FTNumber.ToString(),
            esriFieldType.esriFieldTypeDouble.ToString(),
            esriFieldType.esriFieldTypeInteger.ToString(),
            esriFieldType.esriFieldTypeSmallInteger.ToString(),
            esriFieldType.esriFieldTypeOID.ToString()
        };

        /// <summary>
        /// 用以筛选字符型字段
        /// </summary>
        public readonly static List<string> ScreenCharacterType = new List<string>()
        {
            EnumDBFieldType.FTString.ToString(),
            esriFieldType.esriFieldTypeString.ToString()
        };

        /// <summary>
        /// 用以筛选dbtune中的保留关键字
        /// </summary>
        public readonly static List<string> ScreenDbtuneReservedKeyWord = new List<string>()
        {
            "Keywords found in DBTUNE table are:",
            "TERRAIN_DEFAULTS::EMBEDDED",
            "TERRAIN_DEFAULTS",
            "TOPOLOGY_DEFAULTS::DIRTYAREAS",
            "TOPOLOGY_DEFAULTS",
            "IMS_GAZETTEER",
            "DATA_DICTIONARY",
            "NETWORK_DEFAULTS::NETWORK",
            "NETWORK_DEFAULTS::DESC",
            "NETWORK_DEFAULTS",
            "LOGFILE_DEFAULTS",
            "WKB_GEOMETRY",
            "SDELOB",
            "SDO_GEORASTER",
            "SDO_GEOMETRY",
            "DEFAULTS",
            ""
        };
    }
}
