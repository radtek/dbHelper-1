using ESRI.ArcGIS.Geodatabase;
using Geoway.ADF.MIS.DB.Public.Interface;
using PartitionUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbtuneConfigHelper
{
    /// <summary>
    ///目的：描述dbtune配置及管理流程中涉及参数
    ///创建人：高涛
    ///创建日期：2017-02-17
    ///修改描述：
    ///修改人：
    ///修改日期：
    ///备注：
    /// </summary>
    public class DbtuneConfigParam
    {
        /// <summary>
        /// 数据库连接帮助类
        /// </summary>
        public IDBHelper DBHelper { get; set; }

        /// <summary>
        /// 分区要素
        /// </summary>
        public IFeatureClass FeatureClass { get; set; }

        /// <summary>
        /// 分区列名
        /// </summary>
        public string BasicField { get; set; }

        /// <summary>
        /// 分区方式
        /// </summary>
        public EnumPartitionWay PartitionWay { get; set; }

        /// <summary>
        /// 范围分区的分区个数
        /// </summary>
        public int PartitionCount { get; set; }

        /// <summary>
        /// 各分区对应表空间
        /// </summary>
        public List<string> TablespaceSet { get; set; }

        /// <summary>
        /// 配置关键字
        /// </summary>
        public string DbtuneKeyWord { get; set; }

        public void Clear()
        {
            this.DBHelper = null;
            this.FeatureClass = null;
            this.BasicField = null;
            this.PartitionWay = EnumPartitionWay.Unknow;
            this.PartitionCount = -1;
            this.TablespaceSet = null;
            this.DbtuneKeyWord = null;
        }
    }
}
