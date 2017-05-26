using Geoway.ADF.MIS.DB.Public.Interface;
using Geoway.ADF.MIS.Utility.Log;
using PartitionUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbtuneConfigHelper
{
    /// <summary>
    ///目的：提供dbtune的简单管理方法
    ///创建人：高涛
    ///创建日期：2017-03-03
    ///修改描述：
    ///修改人：
    ///修改日期：
    ///备注：
    /// </summary>
    public class DbtuneManageHelper
    {
        /// <summary>
        /// dbtune配置帮助类
        /// </summary>
        private DbtuneHelper _dbtuneHelper;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="filePath">dbtune导出文件的复制路径</param>
        public DbtuneManageHelper(IDBHelper dbHelper, string filePath)
        {
            this._dbtuneHelper = new DbtuneHelper(filePath, dbHelper.DBServiceName.Split('/')[0], dbHelper.DBPort, dbHelper.DBUser, dbHelper.DBPwd);
        }

        /// <summary>
        /// 展示某配置关键字下的配置值
        /// </summary>
        /// <param name="dbtuneKeyWord"></param>
        /// <returns></returns>
        public string ListParamValueEX(string dbtuneKeyWord)
        {
            return this._dbtuneHelper.ListConfig(dbtuneKeyWord, PartitionKeyWordInfo.DBTUNE_PARAM_NAME);
        }

        /// <summary>
        /// 展示配置关键字
        /// </summary>
        /// <param name="dbtuneKeyWord"></param>
        /// <returns></returns>
        public bool DeleteConfigEX(string dbtuneKeyWord)
        {
            try
            {
                if (DbtuneXMLOperate.CheckKeyWordExist(dbtuneKeyWord))
                {
                    this._dbtuneHelper.DeleteConfig(dbtuneKeyWord);

                    DbtuneXMLOperate.DeleteKeyWord(dbtuneKeyWord);

                    return true;
                }
                else return false;
            }
            catch (Exception ex)
            {
                LogHelper.Error.Append(ex);

                throw ex;
            }
        }
    }
}
