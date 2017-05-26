using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PartitionUtility
{
    ///目的：描述数据库连接及操作涉及的参数
    ///创建人：高涛
    ///创建日期：2017-02-19
    ///修改描述：
    ///修改人：
    ///修改日期：
    ///备注：
    /// </summary>
    public class DbConnectParam
    {
        /// <summary>
        /// IP
        /// </summary>
        public string DBServer { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public string DbPort { get; set; }

        /// <summary>
        /// 数据库实例名
        /// </summary>
        public string DbSid { get; set; }

        /// <summary>
        /// 登陆用户
        /// </summary>
        public string DbUser { get; set; }

        /// <summary>
        /// 用户密码
        /// </summary>
        public string DbPwd{ get; set; }

        public void Clear()
        {
            this.DBServer = null;
            this.DbPort = null;
            this.DbSid = null;
            this.DbUser = null;
            this.DbPwd = null;
        }
    }
}
