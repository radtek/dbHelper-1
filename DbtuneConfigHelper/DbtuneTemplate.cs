using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbtuneConfigHelper
{
    /// <summary>
    ///目的：提供Dbtune命令行操作参数模板
    ///创建人：高涛
    ///创建日期：2017-02-17
    ///修改描述：
    ///修改人：
    ///修改日期：
    ///备注：
    /// </summary>
    public static class DbtuneTemplate
    {
        /// <summary>
        /// dbtune配置参数导入命令的参数
        /// </summary>
        public const string DBTUNE_IMPORT_PARAM = "-o import -f {file_path} -i port:{db_port} -s {db_server} -u {user_name} -p {user_pwd} -q";

        /// <summary>
        /// dbtune配置参数导出命令的参数
        /// </summary>
        public const string DBTUNE_EXPORT_PARAM = "-o export -f {file_path} -i port:{db_port} -s {db_server} -u {user_name} -p {user_pwd} -q  ";

        /// <summary>
        /// dbtune配置参数更改命令的参数
        /// </summary>
        public const string DBTUNE_ALTER_PARAM = "-o alter -k {key_word} -P {param_name} -v \"{parameter_value}\" -i port:{db_port} -s {db_server} -u {user_name} -p {user_pwd} -q";

        /// <summary>
        /// dbtune配置参数展示命令的参数(展示整个关键字)
        /// </summary>
        public const string DBTUNE_LIST_PARAM_1 = "-o list -k {key_word} -i port:{db_port} -s {db_server} -u {user_name} -p {user_pwd} -q";

        /// <summary>
        /// dbtune配置参数展示命令的参数(展示关键字下某条参数)
        /// </summary>
        public const string DBTUNE_LIST_PARAM_2 = "-o list -k {key_word} -P {param_name} -i port:{db_port} -s {db_server} -u {user_name} -p {user_pwd} -q";

        /// <summary>
        /// dbtune配置参数展示命令的参数(展示关键字集合)
        /// </summary>
        public const string DBTUNE_LIST_PARAM_3 = "-o list -l keywords -i port:{db_port} -s {db_server} -u {user_name} -p {user_pwd} -q";

        /// <summary>
        /// dbtune配置参数删除命令的参数(删除整个关键字)
        /// </summary>
        public const string DBTUNE_DELETE_PARAM_1 = "-o delete_data -k {key_word} -i port:{db_port} -s {db_server} -u {user_name} -p {user_pwd} -q";

        /// <summary>
        /// dbtune配置参数删除命令的参数(删除关键字下某条参数)
        /// </summary>
        public const string DBTUNE_DELETE_PARAM_2 = "-o delete_data -k {key_word} -P {param_name} -i port:{db_port} -s {db_server} -u {user_name} -p {user_pwd} -q";

        /// <summary>
        /// dbtune配置参数插入命令的参数
        /// </summary>
        public const string DBTUNE_INSERT_PARAM = "-o insert -k {key_word} -P {param_name} -v \"{parameter_value}\" -i port:{db_port} -s {db_server} -u {user_name} -p {user_pwd} -q";
    }
}
