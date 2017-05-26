using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PartitionUtility
{
    ///目的：描述dbtune中分区关键字的相关信息
    ///创建人：高涛
    ///创建日期：2017-02-22
    ///修改描述：
    ///修改人：
    ///修改日期：
    ///备注：
    /// </summary>
    public static class PartitionKeyWordInfo
    {
        /// <summary>
        /// sdedbtune配置参数默认值
        /// </summary>
        public const string DBTUNE_PARAM_VALUE_PREFIX = "PCTFREE 0 INITRANS 4";

        //sdedbtune配置参数名
        public const string DBTUNE_PARAM_NAME = "B_STORAGE";

        /// <summary>
        /// dbtune分区关键字模板字典
        /// </summary>
        public static Dictionary<string, string> TemplateInfo = new Dictionary<string, string>()
        {
            { "B_INDEX_RASTER", "PCTFREE 0 INITRANS 4 NOLOGGING"},
            { "UI_TEXT", "User Interface text description for Partition"},
            { "ST_INDEX_PARTITION_LOCAL", "true"},
            { "SDO_SRID", "4490000"},
            { "S_STORAGE", "PCTFREE 0 INITRANS 4 NOLOGGING"},
            { "S_INDEX_ALL", "PCTFREE 0 INITRANS 4 NOLOGGING"},
            { "GEOMETRY_STORAGE", "SDO_GEOMETRY"},
            { "B_STORAGE", "PCTFREE 0 INITRANS 4"},
            { "B_INDEX_XML", "PCTFREE 0 INITRANS 4 NOLOGGING"},
            { "B_INDEX_USER", "PCTFREE 0 INITRANS 4 NOLOGGING"},
            { "B_INDEX_TO_DATE", "PCTFREE 0 INITRANS 4 NOLOGGING"},
            { "B_INDEX_ROWID", "PCTFREE 0 INITRANS 4 NOLOGGING"},
        };

        /// <summary>
        /// dbtune分区关键字模板
        /// </summary>
        public static string Template = "##{key_word}" +
                                        Environment.NewLine +
                                        "B_INDEX_RASTER             \"PCTFREE 0 INITRANS 4 NOLOGGING\"" +
                                        Environment.NewLine +
                                        "UI_TEXT                    \"User Interface text description for Partition\"" +
                                        Environment.NewLine +
                                        "ST_INDEX_PARTITION_LOCAL   \"true\"" +
                                        Environment.NewLine +
                                        "SDO_SRID                   4490000" +
                                        Environment.NewLine +
                                        "S_STORAGE                  \"PCTFREE 0 INITRANS 4 NOLOGGING\"" +
                                        Environment.NewLine +
                                        "S_INDEX_ALL                \"PCTFREE 0 INITRANS 4 NOLOGGING\"" +
                                        Environment.NewLine +
                                        "GEOMETRY_STORAGE           \"SDO_GEOMETRY\"" +
                                        Environment.NewLine +
                                        "B_STORAGE                  \"PCTFREE 0 INITRANS 4\"" +
                                        Environment.NewLine +
                                        "B_INDEX_XML                \"PCTFREE 0 INITRANS 4 NOLOGGING\"" +
                                        Environment.NewLine +
                                        "B_INDEX_USER               \"PCTFREE 0 INITRANS 4 NOLOGGING\"" +
                                        Environment.NewLine +
                                        "B_INDEX_TO_DATE            \"PCTFREE 0 INITRANS 4 NOLOGGING\"" +
                                        Environment.NewLine +
                                        "B_INDEX_ROWID              \"PCTFREE 0 INITRANS 4 NOLOGGING\"" +
                                        Environment.NewLine +
                                        "END";
    }
}
