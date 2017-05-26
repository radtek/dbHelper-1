using Geoway.ADF.MIS.DB.Public.Interface;
using PartitionUtility;
using PartitionUtility.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace DbtuneConfigHelper
{
    ///目的：操作（描述当前用户自定义dbtune配置的）XML文件
    ///创建人：高涛
    ///创建日期：2017-03-05
    ///修改描述：
    ///修改人：
    ///修改日期：
    ///备注：
    /// </summary>
    public static class DbtuneXMLOperate
    {
        /// <summary>
        /// XML的根结点
        /// </summary>
        public const string ROOT_NAME = "CustomerDbtune";

        /// <summary>
        /// 删除关键字结点
        /// </summary>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        public static bool DeleteKeyWord(string keyWord)
        {
            if (!File.Exists(string.Format("{0}\\{1}.xml", Environment.CurrentDirectory, UtilityConst.XML_NAME)))
            {
                return false;
            }

            XDocument xml = XDocument.Load(string.Format("{0}\\{1}.xml", Environment.CurrentDirectory, UtilityConst.XML_NAME));

            foreach (var keyword in xml.Element(ROOT_NAME).Elements())
            {
                if (keyword.Attribute("Name").Value.ToString() == keyWord)
                {
                    keyword.Remove();
                    break;
                }
            }
            xml.Save(string.Format("{0}\\{1}.xml", Environment.CurrentDirectory, UtilityConst.XML_NAME));

            return true;
        }

        /// <summary>
        /// 插入关键字结点
        /// </summary>
        /// <param name="dbtuneInfo"></param>
        /// <returns></returns>
        public static bool InsertKeyWord(DbtuneInfo dbtuneInfo)
        {
            if (!File.Exists(string.Format("{0}\\{1}.xml", Environment.CurrentDirectory, UtilityConst.XML_NAME)))
            {
                return false;
            }

            XDocument xml = XDocument.Load(string.Format("{0}\\{1}.xml", Environment.CurrentDirectory, UtilityConst.XML_NAME));

            XElement ParamValue = new XElement("ParamValue", new XAttribute("Value", dbtuneInfo.ParamValue));
            XElement ParamName = new XElement("ParamName", new XAttribute("Name", dbtuneInfo.ParamName));
            XElement keyWord = new XElement("KeyWord", new XAttribute("Name", dbtuneInfo.KeyWord));

            ParamName.Add(ParamValue);
            keyWord.Add(ParamName);

            xml.Element(ROOT_NAME).Add(keyWord);

            xml.Save(string.Format("{0}\\{1}.xml", Environment.CurrentDirectory, UtilityConst.XML_NAME));

            return true;
        }

        /// <summary>
        /// 展示关键字结点
        /// </summary>
        /// <returns></returns>
        public static List<DbtuneInfo> ListKeyWord(IDBHelper dbHelper = null, string filePath = null)
        {
            if (!File.Exists(string.Format("{0}\\{1}.xml", Environment.CurrentDirectory, UtilityConst.XML_NAME)))
            {
                CreateXml(dbHelper, filePath);
            }

            List<DbtuneInfo> dbtuneInfoSet = new List<DbtuneInfo>();

            DbtuneInfo dbtuneInfo = new DbtuneInfo();

            XDocument xml = XDocument.Load(string.Format("{0}\\{1}.xml", Environment.CurrentDirectory, UtilityConst.XML_NAME));

            foreach (var keyword in xml.Element(ROOT_NAME).Elements())
            {
                dbtuneInfo = new DbtuneInfo();
                dbtuneInfo.KeyWord = keyword.Attribute("Name").Value.ToString();
                dbtuneInfo.ParamName = keyword.Element("ParamName").Attribute("Name").Value.ToString();
                dbtuneInfo.ParamValue = keyword.Element("ParamName").Element("ParamValue").Attribute("Value").Value.ToString();
                dbtuneInfoSet.Add(dbtuneInfo);
            }
            return dbtuneInfoSet;
        }

        /// <summary>
        /// 查询关键字结点
        /// </summary>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        public static DbtuneInfo QueryKeyWord(string keyWord)
        {
            if (!File.Exists(string.Format("{0}\\{1}.xml", Environment.CurrentDirectory, UtilityConst.XML_NAME)))
            {
                return null;
            }

            DbtuneInfo dbtuneInfo = new DbtuneInfo();

            XDocument xml = XDocument.Load(string.Format("{0}\\{1}.xml", Environment.CurrentDirectory, UtilityConst.XML_NAME));

            foreach (var keyword in xml.Element(ROOT_NAME).Elements())
            {
                if (keyword.Attribute("Name").Value.ToString() == keyWord)
                {
                    dbtuneInfo.KeyWord = keyword.Attribute("Name").Value.ToString();
                    dbtuneInfo.ParamName = keyword.Element("ParamName").Attribute("Name").Value.ToString();
                    dbtuneInfo.ParamValue = keyword.Element("ParamName").Element("ParamValue").Attribute("Value").Value.ToString();
                    break;
                }
            }
            return dbtuneInfo;
        }

        /// <summary>
        /// 检查某节点是否已存在
        /// </summary>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        public static bool CheckKeyWordExist(string keyWord)
        {
            bool isExist = false;

            List<DbtuneInfo> dbtuneConfigSet = DbtuneXMLOperate.ListKeyWord();

            if (dbtuneConfigSet.Count != 0)
            {
                for (int i = 0; i < dbtuneConfigSet.Count; i++)
                {
                    if (keyWord == dbtuneConfigSet[i].KeyWord)
                    {
                        isExist = true;
                        break;
                    }
                }
            }
            return isExist;
        }

        /// <summary>
        /// 创建（描述当前用户自定义dbtune配置的）XML文件
        /// </summary>
        private static void CreateXml(IDBHelper dbHelper, string filePath)
        {
            XDocument xml = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement(ROOT_NAME));

            xml.Save(string.Format("{0}\\{1}.xml", Environment.CurrentDirectory, UtilityConst.XML_NAME));

            DbtuneHelper dbtuneHelper = new DbtuneHelper(filePath, dbHelper.DBServiceName.Split('/')[0], dbHelper.DBPort, dbHelper.DBUser, dbHelper.DBPwd);

            List<string> keywordSet = dbtuneHelper.ListConfig();

            List<string> keywordScreenedSet = new List<string>();

            for (int i = 0; i < keywordSet.Count; i++)
            {
                if (!RegexCheck.IsExist(ScreenTemplate.ScreenDbtuneReservedKeyWord, keywordSet[i]))
                {
                    keywordScreenedSet.Add(keywordSet[i]);
                }
            }

            for (int i = 0; i < keywordScreenedSet.Count; i++)
            {
                InsertKeyWord(new DbtuneInfo()
                {
                    KeyWord = keywordScreenedSet[i],
                    ParamName = PartitionKeyWordInfo.DBTUNE_PARAM_NAME,
                    ParamValue = dbtuneHelper.ListConfig(keywordScreenedSet[i], PartitionKeyWordInfo.DBTUNE_PARAM_NAME)
                });
            }
        }
    }
}
