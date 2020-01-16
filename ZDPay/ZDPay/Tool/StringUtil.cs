using System;
using System.Collections.Generic;
using System.Text;

namespace ZDPay.Tool
{
    /// <summary>
    /// 文本操作工具类
    /// </summary>
    public static class StringUtil
    {
        /// <summary>
        /// 判断字符串是否为空
        /// </summary>
        /// <param name="value">要判断的字符串</param>
        /// <returns>true 为空; false 不为空</returns>
        public static bool Empty(string value)
        {
            return value == null || value.Trim().Length == 0;
        }

        /// <summary>
        /// 判断字符串是否不为空
        /// </summary>
        /// <param name="value">要判断的字符串</param>
        /// <returns>true 不为空; false 为空</returns>
        public static bool NotEmpty(string value)
        {
            return !Empty(value);
        }

        /// <summary>
        /// 将null的字符串转换为空字符串
        /// </summary>
        /// <param name="value">要转换的字符串</param>
        /// <returns>转换后的字符串</returns>
        public static string NullToSpace(string value)
        {
            return value == null ? "" : value;
        }

        /// <summary>
        /// 将字符串中的匹配项替换为指定的字符串
        /// </summary>
        /// <param name="value">要替换的字符串</param>
        /// <param name="find">匹配项</param>
        /// <param name="replace">替换字符串</param>
        /// <returns>替换后的字符串</returns>
        public static string Replace(string value, string find, string replace)
        {
            if (value == null)
            {
                return null;
            }
            return value.Replace(find, replace);
        }

        /// <summary>
        /// 以指定的字符串为分隔符分割字符串
        /// </summary>
        /// <param name="value">要分割的字符串</param>
        /// <param name="split">分隔符字符串</param>
        /// <returns>分割后的字符串数组</returns>
        public static string[] Split(string value, string split)
        {
            if (value == null)
            {
                return null;
            }
            return value.Split(split.ToCharArray());
        }
        /// <summary>
        /// 获取json字段的值
        /// </summary>
        /// <param name="jsonProperty">json字段</param>
        /// <param name="type">值类型</param>
        /// <returns>json字段的值</returns>
        public static string getJsonValue(JsonProperty jsonProperty, string type)
        {
            if (jsonProperty == null)
            {
                switch (type)
                {
                    case "string":
                        return "";
                    case "number":
                        return "0";
                    default:
                        return "";
                }
            }
            else
            {
                switch (type)
                {
                    case "date":
                        if (jsonProperty.Value.Equals("0001-01-0100:00:00"))
                        {
                            return "1970-01-01 00:00:00";
                        }
                        return jsonProperty.Value.Insert(10, " ");
                    default:
                        return jsonProperty.Value;
                }
            }
        }
    }
}
