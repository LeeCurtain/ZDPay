using System;
using System.Collections.Generic;
using System.Text;

namespace ZDPay.UnionPay
{
    public class UnionUtil
    {
        public UnionUtil()
        { }
        //采用Dictionary的好处是方便对数据包进行签名，不用再签名之前再做一次排序
        private Dictionary<string, string> m_values = new Dictionary<string, string>();

        /**
        * 设置某个字段的值
        * @param key 字段名
         * @param value 字段值
        */
        public void SetValue(string key, string value)
        {
            m_values[key] = value;
        }

        /**
        * 根据字段名获取某个字段的值
        * @param key 字段名
         * @return key对应的字段值
        */
        public object GetValue(string key)
        {
            string o = null;
            m_values.TryGetValue(key, out o);
            return o;
        }

        /**
         * 判断某个字段是否已设置
         * @param key 字段名
         * @return 若字段key已被设置，则返回true，否则返回false
         */
        public bool IsSet(string key)
        {
            string o = null;
            m_values.TryGetValue(key, out o);
            if (null != o)
                return true;
            else
                return false;
        }

        public Dictionary<string, string> GetValues()
        {
            return m_values;
        }
    }
}
