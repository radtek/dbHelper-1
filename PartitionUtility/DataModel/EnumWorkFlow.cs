using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PartitionUtility
{
    /// <summary>
    ///目的：提供工作流程的枚举
    ///创建人：高涛
    ///创建日期：2017-02-18
    ///修改描述：
    ///修改人：
    ///修改日期：
    ///备注：
    /// </summary>
    public enum EnumWorkFlow
    {
        [Description("普通表分区")]
        TablePartition,
        [Description("Dbtune配置")]
        DbtuneConfigure,
        [Description("Dbtune管理")]
        DbtuneManage,
        [Description("非普通表分区")]
        NotTablePartition,
    }
}
