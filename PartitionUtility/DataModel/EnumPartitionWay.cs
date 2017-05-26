using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PartitionUtility
{
    /// <summary>
    ///目的：提供分区方式的枚举
    ///创建人：高涛
    ///创建日期：2017-02-17
    ///修改描述：
    ///修改人：
    ///修改日期：
    ///备注：
    /// </summary>
    public enum EnumPartitionWay
    {
        Unknow = -1,
        [Description("列表分区")]
        List = 0,
        [Description("范围分区")]
        Range = 1
    }
}
