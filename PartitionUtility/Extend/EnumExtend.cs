using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace PartitionUtility
{
    public static class EnumExtend
    {
        /// <summary>
        /// 对枚举类进行扩展
        /// 获取枚举值的描述信息
        /// </summary>
        /// <param name="source">枚举类型</param>
        /// <returns></returns>
        public static string GetDescription(this Enum source)
        {
            var fieldInfo = source.GetType().GetField(source.ToString());
            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : null;
        }
    }
}
