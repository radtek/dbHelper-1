using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PartitionHelper
{
    ///目的：表分区流程主要方法的接口
    ///创建人：高涛
    ///创建日期：2017-02-19
    ///修改描述：
    ///修改人：
    ///修改日期：
    ///备注：
    /// </summary>
    interface ITablePartitionHelper
    {
        /// <summary>
        /// 通过交换分区将未分区旧表转换为新分区表
        /// </summary>
        bool PartitionByExchange();

        /// <summary>
        /// 通过插入数据将未分区旧表转换为新分区表
        /// </summary>
        bool PartitionByInsert();

        /// <summary>
        /// 通过在线重定义将未分区旧表转换为新分区表
        /// </summary>
        bool PartitionByRedefinition();

        /// <summary>
        /// 合并分区
        /// </summary>
        /// <param name="tableName">目标表名</param>
        /// <param name="mergePartitionName_1">要合并的分区1</param>
        /// <param name="mergePartitionName_2">合并目标的分区2</param>
        /// <returns></returns>
        bool MergePartition(string tableName, string mergePartitionName_1, string mergePartitionName_2);

        /// <summary>
        /// 删除分区
        /// </summary>
        /// <param name="tableName">目标表名</param>
        /// <param name="dropPartitionName">目标分区名</param>
        /// <returns></returns>
        bool DropPartition(string tableName, string dropPartitionName);
    }
}
