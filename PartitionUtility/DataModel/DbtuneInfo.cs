using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PartitionUtility
{
    ///目的：描述dbtune配置文件单个单元的信息
    ///创建人：高涛
    ///创建日期：2017-02-19
    ///修改描述：
    ///修改人：
    ///修改日期：
    ///备注：
    /// </summary>
    public class DbtuneInfo
    {
        /// <summary>
        /// 配置关键字
        /// </summary>
        public string KeyWord { get; set; }

        /// <summary>
        /// 配置关键字下的参数名
        /// </summary>
        public string ParamName { get; set; }

        /// <summary>
        /// 参数的值
        /// </summary>
        public string ParamValue { get; set; }
    }
}
