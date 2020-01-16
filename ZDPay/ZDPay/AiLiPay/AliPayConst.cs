using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDPay.AiLiPay
{
    public class AliPayConst
    {
        /// <summary>
        /// 支付宝的APP_ID
        /// </summary>
        public static string APPID { get; set; }
        /// <summary>
        /// 支付宝沙盒环境的测试地址
        /// </summary>
        public static string Host { get; set; }
        /// <summary>
        /// 支付宝正式环境地址
        /// </summary>
        public static string AliPayServiceUrl { get; set; }
        /// <summary>
        /// 支付宝正式环境地址
        /// </summary>
        public static string NotifyUrl { get; set; }
        /// <summary>
        /// 异步通知页面
        /// </summary>
        public static string Notify_Url { get { return Host + NotifyUrl; } }
        /// <summary>
        /// APP产品销售码，支付宝分配的固定值
        /// </summary>
        public static string Product_Code { get; set; }
        /// <summary>
        /// Wap产品销售码，支付宝分配的固定值
        /// </summary>
        public static string WapProduct_Code { get; set; }
        public static string MapiUrl { get; set; }

        /// <summary>
        /// 支付宝的APP_ID
        /// </summary>
        public static string APP_ID
        {
            get { if (APPID == null) return null; else return APPID; }
        }

        #region MyRegion
        /// <summary>
        /// 支付宝的APP_ID
        /// </summary>
        // public const string APP_ID = "";
        /// <summary>
        /// 支付宝沙盒环境的测试地址
        /// </summary>
        public const string Host_Test = "";
        //public const string Host = "";
        /// <summary>
        /// 支付宝正式环境地址
        /// </summary>
        // public const string AliPayServiceUrl = "https://openapi.alipay.com/gateway.do";
        /// <summary>
        /// 支付宝正式环境地址
        /// </summary>
        //public const string NotifyUrl = "/api/V1/AliNotifyUrl";
        /// <summary>
        /// APP产品销售码，支付宝分配的固定值
        /// </summary>
        // public const string Product_Code = "QUICK_MSECURITY_PAY";
        /// <summary>
        /// Wap产品销售码，支付宝分配的固定值
        /// </summary>
        //public const string WapProduct_Code = "QUICK_WAP_PAY";
        #endregion

        /// <summary>
        ///编码类型
        /// </summary>
        public const string Charset = "utf-8";
        //加密方式
        public const string Sign_Type = "RSA2";
        //接口版本
        public const string Version = "1.0";
        //异步通知页面
        //  public const string Notify_Url = Host + NotifyUrl; //"http://boram.ngrok.sapronlee.com/app-pay/notify_url.jsp";    
        //参数格式
        public const string Format = "json";

        //合作者的身份ID
        public const string Sell_ID = "";
        // 支付网关地址
        public const string GATEWAY_NEW = "";
        public const string ALIPAY_PUBLIC_KEY = "";
        public const string AlipayPublicKey = "";
        //public const string MapiUrl = "https://mapi.alipay.com/gateway.do";
        public const string APP_PRIVATE_KEY = "";
    }
}

