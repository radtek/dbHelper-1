using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PartitionUtility
{
    /// <summary>
    ///目的：提供界面输入所需的正则验证
    ///创建人：高涛
    ///创建日期：2017-02-17
    ///修改描述：
    ///修改人：
    ///修改日期：
    ///备注：
    /// </summary>
    public static class RegexCheck
    {
        /// <summary>
        /// 判断输入的字符串只包含数字
        /// 可以匹配整数和浮点数
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNumber(string input)
        {
            string pattern = "^-?\\d+$|^(-?\\d+)(\\.\\d+)?$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(input);
        }

        /// <summary>
        /// 匹配非负整数,包括零
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNotNagtive(string input)
        {
            Regex regex = new Regex(@"^\d+$");
            return regex.IsMatch(input);
        }

        /// <summary>
        /// 匹配正整数
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsUint(string input)
        {
            Regex regex = new Regex("^[0-9]*[1-9][0-9]*$");
            return regex.IsMatch(input);
        }

        /// <summary>
        /// 匹配非空
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsEmpty(string input)
        {
            if (input == null)
            {
                return true;
            }
            else
            {
                if (input == string.Empty)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 检查列表是否有空值
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsFull(List<string> input)
        {
            if (input == null)
            {
                return false;
            }
            else
            {
                bool isFull = true;
                foreach (var item in input)
                {
                    if (IsEmpty(item))
                    {
                        isFull = false;
                        break;
                    }
                }
                return isFull;
            }
        }

        /// <summary>
        /// 检查列表是否存在某值
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsExist(List<string> list, string input)
        {
            bool isExist = false;

            foreach (var item in list)
            {
                if (item == input)
                {
                    isExist = true;
                    break;
                }
            }
            return isExist;
        }

        /// <summary>
        /// 检查字符串中是否存在某值
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsExist(string oringin, string input)
        {
            bool isExist = false;

            if (oringin.IndexOf(input) != -1)
            {
                isExist = true;
            }
            return isExist;
        }

        /// <summary>
        /// 判断输入的字符串字包含英文字母
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsEnglisCh(string input)
        {
            Regex regex = new Regex("^[A-Za-z]+$");
            return regex.IsMatch(input);
        }

        /// <summary>
        /// 判断输入的字符串是否是表示一个IP地址
        /// </summary>
        /// <param name="input">被比较的字符串</param>
        /// <returns>是IP地址则为True</returns>
        public static bool IsIPv4(string input)
        {
            string[] ip = input.Split('.');

            if (ip.Length != 4)
            {
                return false;
            }

            Regex regex = new Regex(@"^\d+$");
            for (int i = 0; i < ip.Length; i++)
            {
                if (!regex.IsMatch(ip[i]))
                {
                    return false;
                }
                if (Convert.ToUInt16(ip[i]) > 255)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 使用指定字符串分割输入字符串
        /// </summary>
        /// <param name="spliter">模式字符串</param>
        /// <param name="input">输入字符串</param>
        /// <returns></returns>
        public static string[] Split(string spliter, string input)
        {
            Regex regex = new Regex(spliter);
            return regex.Split(input);
        }
    }
}
