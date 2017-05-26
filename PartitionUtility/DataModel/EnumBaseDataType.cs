using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace PartitionUtility
{
    ///目的：提供基本数据类型归类的枚举
    ///创建人：高涛
    ///创建日期：2017-02-19
    ///修改描述：
    ///修改人：
    ///修改日期：
    ///备注：
    /// </summary>
    public enum EnumBaseDataType
    {
        [Description("NUMERIC")]
        NUMERIC = 1,
        [Description("CHARACTER")]
        CHARACTER = 2,
        [Description("DATATIME")]
        DATATIME = 3,
    }
}
