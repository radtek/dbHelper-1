using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartitionUtility
{
    /// <summary>
    ///目的：描述分区的基本信息
    ///创建人：高涛
    ///创建日期：2017-02-17
    ///修改描述：
    ///修改人：
    ///修改日期：
    ///备注：
    /// </summary>
    public class PartitionInfo
    {
        /// <summary>
        /// 分区编号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 分区参考，表示按照什么标准生成的分区
        /// </summary>
        public string Refer { get; set; }

        /// <summary>
        /// 分区所属表空间
        /// </summary>
        public string Tablespace { get; set; }
    }
}
