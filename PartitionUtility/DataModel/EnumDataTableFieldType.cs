using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PartitionUtility
{
    ///目的：提供数据库常用字段类型枚举
    ///创建人：高涛
    ///创建日期：2017-02-17
    ///修改描述：
    ///修改人：
    ///修改日期：
    ///备注：
    /// </summary>
    public enum EnumDataTableFieldType
    {
        [Description("VARCHAR2")]
        VARCHAR2 = 1,
        [Description("NUMBER")]
        NUMBER = 2,
        [Description("DATETIME")]
        DATETIME = 3
    }
}
