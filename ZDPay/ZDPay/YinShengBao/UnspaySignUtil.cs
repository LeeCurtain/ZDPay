using LitJson;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ZDPay.YinShengBao
{
    public class UnspaySignUtil
    {

        public UnspaySignUtil()
        {

        }

        //采用排序的Dictionary的好处是方便对数据包进行签名，不用再签名之前再做一次排序
        private Dictionary<string, object> m_values = new Dictionary<string, object>();

        /**
        * 设置某个字段的值
        * @param key 字段名
         * @param value 字段值
        */
        public void SetValue(string key, object value)
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
            object o = null;
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
            object o = null;
            m_values.TryGetValue(key, out o);
            if (null != o)
                return true;
            else
                return false;
        }

        /**
       * @Dictionary格式转化成url参数格式
       * @ return url格式串, 该串不包含sign字段值
       */
        public string ToUrl()
        {
            string buff = "";
            foreach (KeyValuePair<string, object> pair in m_values)
            {
                if (pair.Value == null)
                {
                    //Log.Error(this.GetType().ToString(), "WxPayData内部含有值为null的字段!");
                    throw new YbPayException("YbPayException内部含有值为null的字段!");
                }

                if (pair.Key != "mac")//&& pair.Value.ToString() != ""
                {
                    buff += pair.Key + "=" + pair.Value + "&";
                }
            }
            buff = buff.Trim('&');
            return buff;
        }

        #region 转json
        /// <summary>
        /// 转json
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
      /**
        * @Dictionary格式化成Json
         * @return json串数据
        */
        public string ToJson()
        {
            string jsonStr = JsonMapper.ToJson(m_values);
            return jsonStr;
        }
        #endregion
        /**
       * @生成签名，详见签名生成算法
       * @return 签名, mac字段不参加签名
       */
        public string MakeSign()
        {
            //转url格式
            string str = ToUrl();
            //在string后加入API KEY
            str += "&key=" + YinShengBaoConst.key;
            //MD5加密
            var md5 = MD5.Create();
            var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            var sb = new StringBuilder();
            foreach (byte b in bs)
            {
                sb.Append(b.ToString("x2"));
            }
            //所有字符转为大写
            return sb.ToString().ToUpper();
        }

        /**
        * 
        * 检测签名是否正确
        * 正确返回true，错误抛异常
        */
        public bool CheckSign()
        {
            //如果没有设置签名，则跳过检测
            if (!IsSet("mac"))
            {
                //Log.Error(this.GetType().ToString(), "WxPayData签名存在但不合法!");
                throw new YbPayException("YbPayException签名存在但不合法!");
            }
            //如果设置了签名但是签名为空，则抛异常
            else if (GetValue("mac") == null || GetValue("mac").ToString() == "")
            {
                //Log.Error(this.GetType().ToString(), "WxPayData签名存在但不合法!");
                throw new YbPayException("YbPayException签名存在但不合法!");
            }

            //获取接收到的签名
            string return_sign = GetValue("mac").ToString();

            //在本地计算新的签名
            string cal_sign = MakeSign();

            if (cal_sign == return_sign)
            {
                return true;
            }

            //Log.Error(this.GetType().ToString(), "WxPayData签名验证错误!");
            throw new YbPayException("YbPayException签名验证错误!");
        }
        /// <summary>
        /// 生成表单数据
        /// </summary>
        /// <returns></returns>
        public string YSBForm()
        {
            var parm = m_values;
            bool first_param = true;
            StringBuilder postString = new StringBuilder();
            if (parm != null)
            {
                foreach (var p in parm)
                {
                    if (first_param)
                    {
                        first_param = false;
                    }
                    else
                    {
                        if (p.Key != "mac" && p.Value.ToString() != "")
                        {
                            postString.Append("&");
                        }

                    }

                    if (p.Key != "mac" && p.Value.ToString() != "")
                    {
                        if (p.Value is string)
                        {
                            postString.AppendFormat("{0}={1}", p.Key, p.Value);
                        }
                    }
                }
            }
            //获取mac
            postString.AppendFormat("&{0}={1}", "mac", MakeSign());
            return postString.ToString();
        }
        /**
        * @获取Dictionary
        */
        public Dictionary<string, object> GetValues()
        {
            return m_values;
        }
    }
    public class YbPayException : Exception
    {
        public YbPayException(string msg)
            : base(msg)
        {

        }
    }
}
