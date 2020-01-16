using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDPay.WeiXinPay
{
    public class WeiXinConst
    {

        #region MyRegion
        //<!-- 微信公众号信息配置 -->
        /// <summary>
        /// 绑定支付的APPID
        /// </summary>
        public const string WX_APPID = "wx0dae61610000bbb9";
        /// <summary>
        /// 商户号
        /// </summary>
        public const string WX_MCHID = "1520777411";
        /// <summary>
        /// 商户支付密钥
        /// </summary>
        //public const string WX_KEY = "962332b824cb446eb8d3bdb808cebb35";
        /// <summary>
        /// 商户支付密钥
        /// </summary>
        public const string WX_KEY_TEST = "6114cb89b632409b895f5ed2bb9fc125";
        /// <summary>
        /// 公众帐号secert（仅JSAPI支付的时候需要配置
        /// </summary>
        //public const string WX_APPSECRET = "defca892b87bc5f8a730a30ce9c4c0bb";
        /// <summary>
        /// 回调地址测试
        /// </summary>
        public const string WX_NOTIFY_URL_TEST = "http://zdtest.9wins.cn/api/V1/WxNotifyUrl";
        /// <summary>
        /// 回调地址正式
        /// </summary>
        //public const string WX_NOTIFY_URL = "http://zddemo.9wins.cn/api/V1/WxNotifyUrl";
        // <!-- 微信支付App支付 -->
        /// <summary>
        /// APPID：绑定支付的APPID（必须配置）test
        /// </summary>
        public const string APPWX_APPID_TEST = "wx0dae61610000bbb9";
        /// <summary>
        /// MCHID：商户号（必须配置）test
        /// </summary>
        public const string APPWX_MCHID_TEST = "1520777411";
        /// <summary>
        /// KEY：商户支付密钥，参考开户邮件设置（必须配置）test
        /// </summary>
        public const string APPWX_KEY_TEST = "6114cb89b632409b895f5ed2bb9fc125";

        #endregion
        /// <summary>
        /// APPID：绑定支付的APPID（必须配置）
        /// </summary>
        public static string APPWXAPPID { get; set; }
        /// <summary>
        /// MCHID：商户号（必须配置）
        /// </summary>
        public static string APPWXMCHID { get; set; }
        /// <summary>
        /// 商户支付密钥
        /// </summary>
        public static string WXKEY { get; set; }
        /// <summary>
        /// KEY：商户支付密钥，参考开户邮件设置（必须配置）
        /// </summary>
        public static string APPWXKEY { get; set; }
        /// <summary>
        /// 公众帐号secert（仅JSAPI支付的时候需要配置
        /// </summary>
        public static string WXAPPSECRET { get; set; }
        /// <summary>
        /// 回调地址正式
        /// </summary>
        public static string WXNOTIFYURL { get; set; }

        /// <summary>
        /// 支付过期时间
        /// </summary>
        public static string CountDown { get; set; }

        /// <summary>
        /// APPID：绑定支付的APPID（必须配置）
        /// </summary>
        public static string APPWX_APPID
        {
            get
            {
                if (APPWXAPPID == null)
                {
                    return null;
                }
                else
                {
                    return APPWXAPPID;
                }
            }
        }

        /// <summary>
        /// MCHID：商户号（必须配置）
        /// </summary>
        public static string APPWX_MCHID
        {
            get
            {
                if (APPWXMCHID == null)
                {
                    return null;
                }
                else
                {
                    return APPWXMCHID;
                }
            }
        }
        /// <summary>
        /// 商户支付密钥
        /// </summary>
        public static string WX_KEY
        {
            get
            {
                if (WXKEY == null)
                {
                    return null;
                }
                else
                {
                    return WXKEY;
                }
            }
        }
        /// <summary>
        /// KEY：商户支付密钥，参考开户邮件设置（必须配置）
        /// </summary>
        public static string APPWX_KEY
        {
            get
            {
                if (APPWXKEY == null)
                {
                    return null;
                }
                else
                {
                    return APPWXKEY;
                }
            }
        }
        /// <summary>
        /// 公众帐号secert（仅JSAPI支付的时候需要配置
        /// </summary>
        public static string WX_APPSECRET
        {
            get
            {
                if (WXAPPSECRET == null)
                {
                    return null;
                }
                else
                {
                    return WXAPPSECRET;
                }
            }
        }
        /// <summary>
        /// 回调地址正式
        /// </summary>
        public static string WX_NOTIFY_URL
        {
            get
            {
                if (WXNOTIFYURL == null)
                {
                    return null;
                }
                else
                {
                    return WXNOTIFYURL;
                }
            }
        }
        /// <summary>
        /// 支付过期时间
        /// </summary>
        public static string countdown
        {
            get
            {
                if (CountDown == null)
                {
                    return "15";
                }
                else
                {
                    return CountDown;
                }
            }
        }

        /// <summary>
        /// APPID：绑定支付的APPID（必须配置）
        /// </summary>
        //public const string APPWX_APPID = "wxcf51531d059dcdf0";
        /// <summary>
        /// MCHID：商户号（必须配置）
        /// </summary>
        //public const string APPWX_MCHID = "1519946691";
        /// <summary>
        /// KEY：商户支付密钥，参考开户邮件设置（必须配置）
        /// </summary>
        //public const string APPWX_KEY = "962332b824cb446eb8d3bdb808cebb35";
        /// <summary>
        /// 网站域名
        /// </summary>
        public const string host = "";
        /// <summary>
        /// 支付过期时间
        /// </summary>
       // public const string countdown = "15";
    }
}
