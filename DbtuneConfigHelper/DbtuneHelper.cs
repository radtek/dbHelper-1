using Geoway.ADF.MIS.Utility.Log;
using PartitionUtility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace DbtuneConfigHelper
{
    /// <summary>
    ///目的：提供dbtune配置参数的导入导出、删除及修改插入工作
    ///创建人：高涛
    ///创建日期：2017-02-17
    ///修改描述：
    ///修改人：
    ///修改日期：
    ///备注：调用此类中的方法，须运行环境安装有 ArcSDE命令行工具 并重启运行环境
    /// </summary>
    public class DbtuneHelper
    {
        /// <summary>
        /// 指定外部命令行方法
        /// </summary>
        private const string CMD_COMMAND = "sdedbtune";

        /// <summary>
        /// 表示CMD命令执行成功
        /// </summary>
        private const string CMD_SUCCESS_STATE = "Successfully";

        /// <summary>
        /// 表示CMD命令执行失败
        /// </summary>
        private const string CMD_ERROR_STATE = "Error";

        /// <summary>
        /// dbtune配置参数的导出的文件名常量字符串
        /// </summary>
        public const string DBTUNE_FILE_NAME = "\\" + "sdedbtune.txt";

        /// <summary>
        /// dbtune配置参数的导入导出的文件路径
        /// </summary>
        private string _filePath { get; set; }

        /// <summary>
        /// 企业地理数据库所在服务器IP
        /// </summary>
        private string _dbServer { get; set; }

        /// <summary>
        /// 企业地理数据库端口号
        /// </summary>
        private string _dbPort { get; set; }

        /// <summary>
        /// 企业地理数据库用户名
        /// </summary>
        private string _userName { get; set; }

        /// <summary>
        /// 企业地理数据库用户登陆密码
        /// </summary>
        public string _userPwd { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filePath">dbtune配置参数的导入导出的文件路径，包含文件名与扩展名</param>
        /// <param name="dbServer">企业地理数据库所在服务器IP</param>
        /// <param name="userName">企业地理数据库用户名</param>
        /// <param name="userPwd">企业地理数据库用户登陆密码</param>
        public DbtuneHelper(string filePath, string dbServer, string dbPort, string userName, string userPwd)
        {
            this._filePath = filePath;
            this._dbServer = dbServer;
            this._dbPort = dbPort;
            this._userName = userName;
            this._userPwd = userPwd;
        }

        /// <summary>
        /// 测试连接
        /// </summary>
        /// <returns></returns>
        public bool TryConnect()
        {
            try
            {
                //外部命令行方法参数列表
                List<string> cmdParamSet = new List<string>();

                //使用变量替换参数模板
                string exportParam = DbtuneTemplate.DBTUNE_EXPORT_PARAM;
                exportParam = SetBaseParam(exportParam);
                exportParam = exportParam.Replace("{file_path}", this._filePath + DBTUNE_FILE_NAME);

                cmdParamSet.Add(exportParam);

                string cmdInvorkOutput = CmdInvork(cmdParamSet);

                if (RegexCheck.IsExist(cmdInvorkOutput, CMD_ERROR_STATE))
                {
                    LogHelper.Error.Append(cmdInvorkOutput);
                    return false;
                }
                else return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error.Append(ex);

                return false;
            }
        }

        /// <summary>
        /// 展示配置((展示整个关键字)
        /// </summary>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        public string ListConfig(string keyWord)
        {
            try
            {
                List<string> cmdParamSet = new List<string>();

                string listParam = DbtuneTemplate.DBTUNE_LIST_PARAM_1;
                listParam = SetBaseParam(listParam);
                listParam = listParam.Replace("{key_word}", keyWord);

                cmdParamSet.Add(listParam);

                string cmdInvorkOutput = CmdInvork(cmdParamSet);

                return cmdInvorkOutput;
            }
            catch (Exception ex)
            {
                LogHelper.Error.Append(ex);

                return ex.ToString();
            }
        }

        /// <summary>
        /// 展示配置(展示关键字下某条参数)
        /// </summary>
        /// <param name="keyWord"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public string ListConfig(string keyWord, string paramName)
        {
            try
            {
                List<string> cmdParamSet = new List<string>();

                string listParam = DbtuneTemplate.DBTUNE_LIST_PARAM_2;
                listParam = SetBaseParam(listParam);
                listParam = listParam.Replace("{key_word}", keyWord).Replace("{param_name}", paramName);

                cmdParamSet.Add(listParam);

                string cmdInvorkOutput = CmdInvork(cmdParamSet);

                return cmdInvorkOutput;
            }
            catch (Exception ex)
            {
                LogHelper.Error.Append(ex);

                return ex.ToString();
            }
        }

        /// <summary>
        /// 展示配置(展示关键字集合)
        /// </summary>
        /// <param name="keyWord"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public List<string> ListConfig()
        {
            try
            {
                List<string> cmdParamSet = new List<string>();

                string listParam = DbtuneTemplate.DBTUNE_LIST_PARAM_3;
                listParam = SetBaseParam(listParam);

                cmdParamSet.Add(listParam);

                string[] cmdInvorkOutput = RegexCheck.Split(Environment.NewLine, CmdInvork(cmdParamSet).Trim().Replace("\t", string.Empty));

                List<string> keywordSet = new List<string>();

                for (int i = 0; i < cmdInvorkOutput.Length; i++)
                {
                    keywordSet.Add(cmdInvorkOutput[i]);
                }

                return keywordSet;
            }
            catch (Exception ex)
            {
                LogHelper.Error.Append(ex);

                return null;
            }
        }

        /// <summary>
        /// 导入配置
        /// </summary>
        /// <returns></returns>
        public bool ImportConfig()
        {
            try
            {
                string filePath = this._filePath + DBTUNE_FILE_NAME;

                if (!File.Exists(filePath))
                {
                    return false;
                }

                List<string> cmdParamSet = new List<string>();

                string importParam = DbtuneTemplate.DBTUNE_IMPORT_PARAM;
                importParam = SetBaseParam(importParam);
                importParam = importParam.Replace("{file_path}", filePath);

                cmdParamSet.Add(importParam);

                //这里因为dbtune配置参数的导入命令需要用户确认，故在参数列表里传入一个'Y'
                cmdParamSet.Add("Y");

                string cmdInvorkOutput = CmdInvork(cmdParamSet);

                if (RegexCheck.IsExist(cmdInvorkOutput, CMD_ERROR_STATE))
                {
                    LogHelper.Error.Append(cmdInvorkOutput);
                    return false;
                }
                else return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error.Append(ex);

                return false;
            }
        }

        /// <summary>
        /// 导出配置
        /// </summary>
        /// <returns></returns>
        public bool ExportConfig()
        {
            try
            {
                //外部命令行方法参数列表
                List<string> cmdParamSet = new List<string>();

                //使用变量替换参数模板
                string exportParam = DbtuneTemplate.DBTUNE_EXPORT_PARAM;
                exportParam = SetBaseParam(exportParam);
                exportParam = exportParam.Replace("{file_path}", this._filePath + DBTUNE_FILE_NAME);

                cmdParamSet.Add(exportParam);

                string cmdInvorkOutput = CmdInvork(cmdParamSet);

                if (RegexCheck.IsExist(cmdInvorkOutput, CMD_ERROR_STATE))
                {
                    LogHelper.Error.Append(cmdInvorkOutput);
                    return false;
                }
                else return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error.Append(ex);

                return false;
            }
        }

        /// <summary>
        /// 增加配置
        /// </summary>
        /// <param name="keyWord">配置关键字</param>
        /// <returns></returns>
        public bool AddConfig(string keyWord)
        {
            try
            {
                string filePath = this._filePath + DBTUNE_FILE_NAME;

                if (!File.Exists(filePath))
                {
                    return false;
                }

                string keyContent = PartitionKeyWordInfo.Template;

                keyContent = keyContent.Replace("{key_word}", keyWord);

                File.AppendAllText(filePath, Environment.NewLine);
                File.AppendAllText(filePath, keyContent);

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error.Append(ex);

                return false;
            }
        }

        /// <summary>
        /// 删除配置(删除整个关键字)
        /// </summary>
        /// <param name="keyWord">配置关键字</param>
        /// <returns></returns>
        public bool DeleteConfig(string keyWord)
        {
            try
            {
                List<string> cmdParamSet = new List<string>();

                string deleteParam = DbtuneTemplate.DBTUNE_DELETE_PARAM_1;
                deleteParam = SetBaseParam(deleteParam);
                deleteParam = deleteParam.Replace("{key_word}", keyWord);

                cmdParamSet.Add(deleteParam);

                //这里因为dbtune配置参数的导入命令需要用户确认，故在参数列表里传入一个'Y'
                cmdParamSet.Add("Y");

                string cmdInvorkOutput = CmdInvork(cmdParamSet);

                if (RegexCheck.IsExist(cmdInvorkOutput, CMD_ERROR_STATE))
                {
                    LogHelper.Error.Append(cmdInvorkOutput);
                    return false;
                }
                else return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error.Append(ex);

                return false;
            }
        }

        /// <summary>
        /// 删除配置(删除关键字下某条参数)
        /// </summary>
        /// <param name="keyWord">配置关键字</param>
        /// <returns></returns>
        public bool DeleteConfig(string keyWord, string paramName)
        {
            try
            {
                List<string> cmdParamSet = new List<string>();

                string deleteParam = DbtuneTemplate.DBTUNE_DELETE_PARAM_2;
                deleteParam = SetBaseParam(deleteParam);
                deleteParam = deleteParam.Replace("{key_word}", keyWord).Replace("{param_name}", paramName);

                cmdParamSet.Add(deleteParam);

                //这里因为dbtune配置参数的导入命令需要用户确认，故在参数列表里传入一个'Y'
                cmdParamSet.Add("Y");

                string cmdInvorkOutput = CmdInvork(cmdParamSet);

                if (RegexCheck.IsExist(cmdInvorkOutput, CMD_ERROR_STATE))
                {
                    LogHelper.Error.Append(cmdInvorkOutput);
                    return false;
                }
                else return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error.Append(ex);

                return false;
            }
        }

        /// <summary>
        /// 更改配置参数
        /// </summary>
        /// <param name="keyWord">配置关键字</param>
        /// <param name="paramName">参数名</param>
        /// <param name="paramValue">参数内容</param>
        /// <returns></returns>
        public bool AlterConfig(string keyWord, string paramName, string paramValue)
        {
            try
            {
                List<string> cmdParamSet = new List<string>();

                string alterParam = DbtuneTemplate.DBTUNE_ALTER_PARAM;
                alterParam = SetBaseParam(alterParam);
                alterParam = alterParam.Replace("{key_word}", keyWord).Replace("{param_name}", paramName).Replace("{parameter_value}", paramValue);

                cmdParamSet.Add(alterParam);

                //这里因为dbtune配置参数的导入命令需要用户确认，故在参数列表里传入一个'Y'
                cmdParamSet.Add("Y");

                string cmdInvorkOutput = CmdInvork(cmdParamSet);

                if (RegexCheck.IsExist(cmdInvorkOutput, CMD_ERROR_STATE))
                {
                    LogHelper.Error.Append(cmdInvorkOutput);
                    return false;
                }
                else return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error.Append(ex);

                return false;
            }
        }

        /// <summary>
        /// 插入配置参数
        /// </summary>
        /// <param name="keyWord">配置关键字</param>
        /// <param name="paramName">参数名</param>
        /// <param name="paramValue">参数内容</param>
        /// <returns></returns>
        public bool InsertConfig(string keyWord, string paramName, string paramValue)
        {
            try
            {
                List<string> cmdParamSet = new List<string>();

                string insertParam = DbtuneTemplate.DBTUNE_INSERT_PARAM;
                insertParam = SetBaseParam(insertParam);
                insertParam = insertParam.Replace("{key_word}", keyWord).Replace("{param_name}", paramName).Replace("{parameter_value}", paramValue);

                cmdParamSet.Add(insertParam);

                //这里因为dbtune配置参数的导入命令需要用户确认，故在参数列表里传入一个'Y'
                cmdParamSet.Add("Y");

                string cmdInvorkOutput = CmdInvork(cmdParamSet);

                if (RegexCheck.IsExist(cmdInvorkOutput, CMD_ERROR_STATE))
                {
                    LogHelper.Error.Append(cmdInvorkOutput);
                    return false;
                }
                else return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error.Append(ex);

                return false;
            }
        }

        /// <summary>
        /// 调用外部CMD命令行
        /// </summary>
        /// <param name="cmdParamSet">命令行执行方法所需的参数</param>
        /// <returns>返回命令行执行结果</returns>
        private string CmdInvork(List<string> cmdParamSet)
        {
            try
            {
                string cmdOutput = string.Empty;

                //实例化一个进程类
                Process cmd = new Process();
                //指定命令行执行方法
                cmd.StartInfo.FileName = CMD_COMMAND;

                //输入初始参数，相当于args
                cmd.StartInfo.Arguments = cmdParamSet[0];

                //关闭Shell的使用，此处必须为false否则引发异常
                cmd.StartInfo.UseShellExecute = false;
                cmd.StartInfo.RedirectStandardInput = true; //标准输入重定向
                cmd.StartInfo.RedirectStandardOutput = true; //标准输出重定向
                cmd.StartInfo.RedirectStandardError = true; //错误输出重定向

                //不显示命令行窗口界面
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //启动进程
                cmd.Start();

                //向cmd窗口发送输入信息
                if (cmdParamSet.Count > 1)
                {
                    for (int i = 1; i < cmdParamSet.Count; i++)
                    {
                        cmd.StandardInput.WriteLine(cmdParamSet[i] + "&exit");
                    }
                }
                ////获取输出
                cmdOutput = cmd.StandardOutput.ReadToEnd();
                //等待控制台程序执行完成
                cmd.WaitForExit();
                //关闭该进程
                cmd.Close();

                return cmdOutput;
            }
            catch (Exception ex)
            {
                string errorInfo = "如找不到其他错误，则需注意：调用此类中的方法，须运行环境安装有 ArcSDE命令行工具 并重启运行环境" + Environment.NewLine + ex.ToString();

                LogHelper.Error.Append(errorInfo);

                return errorInfo;
            }
            
        }

        /// <summary>
        /// 为命令模板提供公共参数的替换
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        private string SetBaseParam(string template)
        {
            return template.Replace("{db_server}", this._dbServer).Replace("{db_port}", this._dbPort).Replace("{user_name}", this._userName).Replace("{user_pwd}", this._userPwd);
        }
    }
}
