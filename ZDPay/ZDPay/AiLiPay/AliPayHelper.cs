
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using LitJson;
using ZDPay.Log;
using Newtonsoft.Json;
using Alipay.AopSdk.Core;
using Alipay.AopSdk.Core.Response;
using Alipay.AopSdk.Core.Request;
using Alipay.AopSdk.Core.Util;
using Aop.Api.Util;

namespace ZDPay.AiLiPay
{
    public class AliPayHelper
    {

        #region  定义常量参数
        /// <summary>
        /// 支付宝的APP_ID
        /// </summary>
        public static readonly string APP_ID = AliPayConst.APP_ID;
        /// <summary>
        /// 支付宝沙盒环境的测试地址
        /// </summary>
        private static readonly string Host = AliPayConst.Host;
        /// <summary>
        /// 支付宝正式环境地址
        /// </summary>
        private static readonly string AliPayServiceUrl = AliPayConst.AliPayServiceUrl;
        /// <summary>
        /// 支付宝正式环境地址
        /// </summary>
        private static readonly string NotifyUrl = AliPayConst.NotifyUrl;
        /// <summary>
        /// APP产品销售码，支付宝分配的固定值
        /// </summary>
        private static readonly string Product_Code = AliPayConst.Product_Code;
        /// <summary>
        /// Wap产品销售码，支付宝分配的固定值
        /// </summary>
        private static readonly string WapProduct_Code = AliPayConst.WapProduct_Code;
        //编码类型
        private static readonly string Charset = AliPayConst.Charset;
        //加密方式
        private static readonly string Sign_Type = AliPayConst.Sign_Type;
        //接口版本
        private static readonly string Version = AliPayConst.Version;
        //异步通知页面
        private static readonly string Notify_Url = AliPayConst.Notify_Url;
        //参数格式
        private static readonly string Format = AliPayConst.Format;
        //合作者的身份ID
        //public static readonly string Sell_ID = ConfigurationManager.AppSettings["Sell_ID"].ToString();  //从配置文件读取

        private static Dictionary<string, string> dict;
        private static string sign = string.Empty;   //获取客户签名的值
                                                     //private static string ss = string.Empty;
        #endregion

        /// <summary>
        /// 日志名称
        /// </summary>
        private readonly static LogHelp log = new LogHelp(LogType.ALiPay);

        #region 转json
        /// <summary>
        /// 转json
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        private static string ToJson(object Value)
        {
            return JsonMapper.ToJson(Value);
        }
        #endregion

        #region 获取APP请求的加密参数    【外部调用】
        /// <summary>
        /// 获取APP请求的加密参数
        /// </summary>
        /// <param name="timeout_express">该笔订单允许的最晚付款时间，逾期将关闭交易。取值范围：1m～15d。m-分钟，h-小时，d-天，1c-当天（1c-当天的情况下，无论交易何时创建，都在0点关闭）。 该参数数值不接受小数点， 如 1.5h，可转换为 90m。</param>
        /// <param name="seller_id">收款支付宝用户ID。 如果该值为空，则默认为商户签约账号对应的支付宝用户ID</param>
        /// <param name="total_amount">订单总金额，单位为元，精确到小数点后两位，取值范围[0.01,100000000]  【必选项】</param>
        /// <param name="subject">商品的标题/交易标题/订单标题/订单关键字  【必须项】</param>
        /// <param name="body">对一笔交易的具体描述信息。</param>
        /// <param name="out_trade_no">商户网站唯一订单号 【必须项】</param>
        /// <param name="product_code">销售产品码，商家和支付宝签约的产品码，为固定值QUICK_MSECURITY_PAY   【必选项】</param>
        /// <returns></returns>
        public static string GetApplyContent(string timeout_express, string seller_id, string total_amount, string subject, string body, string out_trade_no)
        {
            //获取代签名的公共参数
            dict = new Dictionary<string, string>();
            dict = GetPublicParameNoSign(timeout_express, seller_id, total_amount, subject, body, out_trade_no);
            StringBuilder sb = new StringBuilder();
            foreach (var item in dict)
            {
                sb.Append(BuildKeyValue(item.Key, item.Value, false));
                sb.Append("&");
            }
            //ss = sb.ToString().Substring(0, sb.ToString().Length - 1);   //去掉最后一个&符号

            GetSingValue();  //获取sign值
            return GetEncodeContent(); //获取最后的字符串
        }
        #endregion

        #region 获取Wap请求的加密参数    【外部调用】
        /// <summary>
        /// 获取APP请求的加密参数
        /// </summary>
        /// <param name="timeout_express">该笔订单允许的最晚付款时间，逾期将关闭交易。取值范围：1m～15d。m-分钟，h-小时，d-天，1c-当天（1c-当天的情况下，无论交易何时创建，都在0点关闭）。 该参数数值不接受小数点， 如 1.5h，可转换为 90m。</param>
        /// <param name="seller_id">收款支付宝用户ID。 如果该值为空，则默认为商户签约账号对应的支付宝用户ID</param>
        /// <param name="total_amount">订单总金额，单位为元，精确到小数点后两位，取值范围[0.01,100000000]  【必选项】</param>
        /// <param name="subject">商品的标题/交易标题/订单标题/订单关键字  【必须项】</param>
        /// <param name="body">对一笔交易的具体描述信息。</param>
        /// <param name="out_trade_no">商户网站唯一订单号 【必须项】</param>
        /// <param name="product_code">销售产品码，商家和支付宝签约的产品码，为固定值QUICK_MSECURITY_PAY   【必选项】</param>
        /// <returns></returns>
        public static string GetApplyContentForWap(string timeout_express, string seller_id, string total_amount, string subject, string body, string out_trade_no, string return_url)
        {
            //获取代签名的公共参数
            dict = new Dictionary<string, string>();
            dict = GetPublicParameNoSignForWap(timeout_express, seller_id, total_amount, subject, body, out_trade_no, return_url);
            StringBuilder sb = new StringBuilder();
            foreach (var item in dict)
            {
                sb.Append(BuildKeyValue(item.Key, item.Value, false));
                sb.Append("&");
            }
            //ss = sb.ToString().Substring(0, sb.ToString().Length - 1);   //去掉最后一个&符号

            GetSingValue();  //获取sign值
            return GetEncodeContent(); //获取最后的字符串
        }
        #endregion

        #region 验证sign是否正确  【外部调用】
        /// <summary>
        /// 验证sign是否正确  【外部调用】
        /// </summary>
        /// <param name="parameters">参数包含所有返回来的数据（含sign及sign_type）</param>
        /// <param name="keyFromFile">是否读取公钥文件 true读取公钥文件  false直接读取公钥串[公钥写死在程序中了]</param>
        /// <returns></returns>
        public static bool CheckSign(IDictionary<string, string> parameters, bool keyFromFile)
        {
            string charset = parameters["charset"];  //获取charset;
            string signType = Sign_Type;
            string sign = parameters["sign"];
            parameters.Remove("sign");
            parameters.Remove("sign_type");
            string signContent = GetSignContent(parameters);
            try
            {
                if (string.IsNullOrEmpty(charset))
                {
                    charset = "GBK";
                }
                string sPublicKeyPEM;
                if (keyFromFile)
                {
                    string publicKeyPem = GetCurrentPath() + "rsa_public_key.pem";  //获取公钥
                    sPublicKeyPEM = File.ReadAllText(publicKeyPem);
                }
                else
                {
                    string publicKeyPem = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA4JZ190EyA/ZLUwjE8jHZ81JVyXvZLr0gPouQTLxOjh6Tm7DtdTXMWKBTJ6B1ilg9U10ROo8vUbZhSc2ouNJ3o+ZwGbxlYq1yJ2bD421WknYTUPZIOlBDNpDseI+tpx3pJ/uSavdl9nw8PI9bBMDmox3iXSKSIrQiZGTYOdviCkDc8vdtoHIrNbXQavtYQXM9mGhv18BI8/FsC6V40SNGf7M+EIraXmn99n9X3HjiBttt1TUIYC16/+IEQ0QwvEBGKTmxSRzJQdnRXT4k2tiE1T62TD7/dy5KHocq7u7AzObNLpySde/jAJd4ME6GXPPMrEF6ndZkBzxbQ+CS+Jb4LwIDAQAB";  //获取公钥
                    sPublicKeyPEM = "-----BEGIN PUBLIC KEY-----";
                    sPublicKeyPEM += publicKeyPem;
                    sPublicKeyPEM += "-----END PUBLIC KEY-----";
                }
                if ("RSA2".Equals(signType))
                {
                    RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                    rsa.PersistKeyInCsp = false;
                    RSACryptoServiceProviderExtension.LoadPublicKeyPEM(rsa, sPublicKeyPEM);

                    bool bVerifyResultOriginal = rsa.VerifyData(Encoding.GetEncoding(charset).GetBytes(signContent), "SHA256", Convert.FromBase64String(sign));
                    return bVerifyResultOriginal;
                }
                else
                {
                    RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                    rsa.PersistKeyInCsp = false;
                    RSACryptoServiceProviderExtension.LoadPublicKeyPEM(rsa, sPublicKeyPEM);

                    SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
                    bool bVerifyResultOriginal = rsa.VerifyData(Encoding.GetEncoding(charset).GetBytes(signContent), sha1, Convert.FromBase64String(sign));
                    return bVerifyResultOriginal;
                }

            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// 验证sign是否正确
        /// </summary>
        /// <param name="signContent"></param>
        /// <param name="sign">签名</param>
        /// <param name="publicKeyPem">公私</param>
        /// <param name="charset">编码格式</param>
        /// <param name="signType">签名方式</param>
        /// <returns></returns>
        public static bool RSACheckContent(string signContent, string sign, string publicKeyPem, string charset, string signType, bool keyFromFile)
        {
             /** 默认编码字符集 */
              string DEFAULT_CHARSET = "GBK";

            try
            {
                if (string.IsNullOrEmpty(charset))
                {
                    charset = DEFAULT_CHARSET;
                }

                string sPublicKeyPem = publicKeyPem;
                string sPublicKeyPEM = publicKeyPem;
                if (keyFromFile)
                {
                    sPublicKeyPem = File.ReadAllText(publicKeyPem);
                }
                var rsa = ALiPayRSA.CreateRsaProviderFromPublicKey(sPublicKeyPem, signType);

                if ("RSA2".Equals(signType))
                {


                    var bVerifyResultOriginal = rsa.VerifyData(Encoding.GetEncoding(charset).GetBytes(signContent),
                        Convert.FromBase64String(sign), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                    return bVerifyResultOriginal;
                    //RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                    //rsa.PersistKeyInCsp = false;
                    //RSACryptoServiceProviderExtension.LoadPublicKeyPEM(rsa, sPublicKeyPEM);

                    //bool bVerifyResultOriginal = rsa.VerifyData(Encoding.GetEncoding(charset).GetBytes(signContent), "SHA256", Convert.FromBase64String(sign));
                    //return bVerifyResultOriginal;
                }
                else
                {
                    //RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                    //rsa.PersistKeyInCsp = false;
                    //RSACryptoServiceProviderExtension.LoadPublicKeyPEM(rsa, sPublicKeyPEM);

                    //SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
                    //bool bVerifyResultOriginal = rsa.VerifyData(Encoding.GetEncoding(charset).GetBytes(signContent), sha1, Convert.FromBase64String(sign));
                    //return bVerifyResultOriginal;
                    var bVerifyResultOriginal = rsa.VerifyData(Encoding.GetEncoding(charset).GetBytes(signContent),
                        Convert.FromBase64String(sign), HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
                    return bVerifyResultOriginal;
                }
            }
            catch
            {
                return false;
            }

        }


        #endregion

        #region 验证sign是否正确  【外部调用】
        /// <summary>
        /// 验证sign是否正确  【外部调用】
        /// </summary>
        /// <param name="parameters">参数包含所有返回来的数据（含sign及sign_type）</param>
        /// <returns></returns>
        public static bool CheckSignBySDK(IDictionary<string, string> parameters)
        {
            string charset = parameters["charset"];  //获取charset;
            string publicKeyPem = GetCurrentPath() + "rsa_public_key.pem";  //获取公钥路径
            string signContent = GetSignContent(parameters);
            return Aop.Api.Util.AlipaySignature.RSACheckV1(parameters, publicKeyPem, charset);  //调用封装方法
        }
        #endregion

        #region 获取APPbizContent业务参数  [私有方法]
        /// <summary>
        /// 获取APPbizContent业务参数 2016-12-07 [私有方法]
        /// </summary>
        /// <param name="timeout_express">该笔订单允许的最晚付款时间，逾期将关闭交易。取值范围：1m～15d。m-分钟，h-小时，d-天，1c-当天（1c-当天的情况下，无论交易何时创建，都在0点关闭）。 该参数数值不接受小数点， 如 1.5h，可转换为 90m。</param>
        /// <param name="seller_id">收款支付宝用户ID。 如果该值为空，则默认为商户签约账号对应的支付宝用户ID</param>
        /// <param name="total_amount">订单总金额，单位为元，精确到小数点后两位，取值范围[0.01,100000000]  【必选项】</param>
        /// <param name="subject">商品的标题/交易标题/订单标题/订单关键字  【必须项】</param>
        /// <param name="body">对一笔交易的具体描述信息。</param>
        /// <param name="out_trade_no">商户网站唯一订单号 【必须项】</param>
        /// <param name="product_code">销售产品码，商家和支付宝签约的产品码，为固定值QUICK_MSECURITY_PAY   【必选项】</param>
        /// <returns>JSON字符串</returns>
        public static string GetPrivateParameter(string timeout_express, string seller_id, string total_amount, string subject, string body, string out_trade_no)
        {
            dict = new Dictionary<string, string>();
            dict.Add("timeout_express", timeout_express);
            //dict.Add("seller_id",seller_id);
            dict.Add("product_code", Product_Code);
            dict.Add("total_amount", total_amount);
            dict.Add("subject", subject);
            dict.Add("body", body);
            dict.Add("out_trade_no", out_trade_no);
            return ToJson(dict);   //将内容转为json
        }
        #endregion

        #region 获取Wap bizContent业务参数  [私有方法]
        /// <summary>
        /// 获取Wap bizContent业务参数 2016-12-28 [私有方法]
        /// </summary>
        /// <param name="timeout_express">该笔订单允许的最晚付款时间，逾期将关闭交易。取值范围：1m～15d。m-分钟，h-小时，d-天，1c-当天（1c-当天的情况下，无论交易何时创建，都在0点关闭）。 该参数数值不接受小数点， 如 1.5h，可转换为 90m。</param>
        /// <param name="seller_id">收款支付宝用户ID。 如果该值为空，则默认为商户签约账号对应的支付宝用户ID</param>
        /// <param name="total_amount">订单总金额，单位为元，精确到小数点后两位，取值范围[0.01,100000000]  【必选项】</param>
        /// <param name="subject">商品的标题/交易标题/订单标题/订单关键字  【必须项】</param>
        /// <param name="body">对一笔交易的具体描述信息。</param>
        /// <param name="out_trade_no">商户网站唯一订单号 【必须项】</param>
        /// <param name="product_code">销售产品码，商家和支付宝签约的产品码，为固定值QUICK_MSECURITY_PAY   【必选项】</param>
        /// <returns>JSON字符串</returns>
        private static string GetPrivateParameterForWap(string timeout_express, string seller_id, string total_amount, string subject, string body, string out_trade_no)
        {
            dict = new Dictionary<string, string>();
            dict.Add("timeout_express", timeout_express);
            //dict.Add("seller_id",seller_id);
            dict.Add("product_code", WapProduct_Code);
            dict.Add("total_amount", total_amount);
            dict.Add("subject", subject);
            dict.Add("body", body);
            dict.Add("out_trade_no", out_trade_no);
            return ToJson(dict);   //将内容转为json
        }
        #endregion

        #region 获取待签名前的排序好的公共参数 [私有方法]
        /// <summary>
        /// 获取待签名前的排序好的公共参数 [私有方法]
        /// </summary>
        /// <param name="timeout_express">该笔订单允许的最晚付款时间，逾期将关闭交易。取值范围：1m～15d。m-分钟，h-小时，d-天，1c-当天（1c-当天的情况下，无论交易何时创建，都在0点关闭）。 该参数数值不接受小数点， 如 1.5h，可转换为 90m。</param>
        /// <param name="seller_id">收款支付宝用户ID。 如果该值为空，则默认为商户签约账号对应的支付宝用户ID</param>
        /// <param name="total_amount">订单总金额，单位为元，精确到小数点后两位，取值范围[0.01,100000000]  【必选项】</param>
        /// <param name="subject">商品的标题/交易标题/订单标题/订单关键字  【必须项】</param>
        /// <param name="body">对一笔交易的具体描述信息。</param>
        /// <param name="out_trade_no">商户网站唯一订单号 【必须项】</param>
        /// <returns></returns>
        private static Dictionary<string, string> GetPublicParameNoSign(string timeout_express, string seller_id, string total_amount, string subject, string body, string out_trade_no)
        {
            log.info("阿里支付返回加密参数开始，订单id为："+out_trade_no);
            //获取bizContent的具体内容
            string bizContent = GetPrivateParameter(timeout_express, seller_id, total_amount, subject, body, out_trade_no);
            log.info("bizContent的内容为："+ bizContent);
            dict = new Dictionary<string, string>();
            dict.Add("app_id", APP_ID);
            dict.Add("biz_content", bizContent);
            dict.Add("charset", Charset);
            dict.Add("format", Format);
            dict.Add("method", "alipay.trade.app.pay");
            dict.Add("notify_url", Notify_Url);
            dict.Add("sign_type", Sign_Type);
            dict.Add("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            //dict.Add("timestamp", "2016-12-19 18:04:32");
            dict.Add("version", Version);
            return DictionaryOrder(dict);
        }
        #endregion
        /// <summary>
        /// 订单状态查询
        /// </summary>
        /// <returns></returns>
        public  static string GetOrderStatues(string out_trade_no, string trade_no)
        {
            const string URL = "https://openapi.alipay.com/gateway.do";  //沙箱环境与正式环境不同 这里要用沙箱的 支付宝地址https://openapi.alipay.com/gateway.do
                                                                         // https://openapi.alipaydev.com/gateway.do
                                                                         //	APPID即创建应用后生成
            string APPID = APP_ID;
            //开发者应用私钥，由开发者自己生成 
            string APP_PRIVATE_KEY = AliPayConst.APP_PRIVATE_KEY;
            //参数返回格式，只支持json
            string FORMAT = Format;
            //请求和签名使用的字符编码格式，支持GBK和UTF-8
            string CHARSET = Charset;
            //支付宝公钥，由支付宝生成
            string ALIPAY_PUBLIC_KEY = AliPayConst.ALIPAY_PUBLIC_KEY;


            DefaultAopClient client = new DefaultAopClient(URL, APPID, APP_PRIVATE_KEY, FORMAT, "1.0", "RSA2", ALIPAY_PUBLIC_KEY, CHARSET, false);
            AlipayTradeQueryRequest request = new AlipayTradeQueryRequest();

            StringBuilder content = new StringBuilder();
            //支付宝订单号和 商户订单号可以 任填其一 也可以两个都填, 填两个的情况优先用 trade_no 
            content.Append("{");
            content.AppendFormat("\"out_trade_no\":\"{0}\",", out_trade_no.Trim());
            content.AppendFormat("\"trade_no\":\"{0}\"", trade_no.Trim());
            content.Append("}");
            request.BizContent = content.ToString();
            AlipayTradeQueryResponse response = client.Execute(request);
            log.info("订单支付状态查询参数:" + content.ToString());
            string msg =response.Body;
            log.info("订单支付状态查询返回参数：" + msg);
            return msg;
            // string CHARSET = Charset;
            // //获取代签名的公共参数
            // dict = new Dictionary<string, string>();
            // dict = GetOrderCount(out_trade_no, trade_no);
            // GetSingValue();  //获取sign值
            // dict.Add("sign", sign);
            // string dic=GetSignContent(dict);
            //// dict = DictionaryOrder(dict); //获取最后的字符串
            // log.info("订单支付状态查询参数:"+dic);
            // WebUtils webUtils = new WebUtils();
            // string body = webUtils.DoPost(URL + "? charset=" + CHARSET, dict, CHARSET);
            // //string msg = JsonConvert.SerializeObject(body);
            // log.info("订单支付状态查询返回参数："+ body);
            // return body;
        }

        /// <summary>
        /// 订单状态
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetOrderCount(string out_trade_no, string trade_no)
        {
            log.info("订单状态查询");
            string bizContent = GetOrderParameter(out_trade_no,trade_no);
            log.info("bizContent的内容为：" + bizContent);
            dict = new Dictionary<string, string>();
            dict.Add("app_id", APP_ID);
            dict.Add("biz_content", bizContent);
            dict.Add("charset", Charset);
            dict.Add("format", Format);
            dict.Add("method", "alipay.trade.query");
            dict.Add("sign_type", Sign_Type);
            dict.Add("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            dict.Add("version", Version);
            GetSingValue();
            dict.Add("sign", sign);
            return DictionaryOrder(dict);
        }
        /// <summary>
        /// 订单状态请求参数
        /// </summary>
        /// <param name="out_trade_no"></param>
        /// <param name="trade_no"></param>
        /// <returns></returns>
        private static string GetOrderParameter(string out_trade_no, string trade_no)
        {
            dict = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(trade_no))
            {
                dict.Add("trade_no", trade_no);
            }
            else
            {
                dict.Add("out_trade_no", out_trade_no);
            }
            return ToJson(dict);   //将内容转为json
        }


        #region 获取Wap调用待签名前的排序好的公共参数  [私有方法]
        /// <summary>
        /// 获取Wap调用待签名前的排序好的公共参数 [私有方法]
        /// </summary>
        /// <param name="timeout_express">该笔订单允许的最晚付款时间，逾期将关闭交易。取值范围：1m～15d。m-分钟，h-小时，d-天，1c-当天（1c-当天的情况下，无论交易何时创建，都在0点关闭）。 该参数数值不接受小数点， 如 1.5h，可转换为 90m。</param>
        /// <param name="seller_id">收款支付宝用户ID。 如果该值为空，则默认为商户签约账号对应的支付宝用户ID</param>
        /// <param name="total_amount">订单总金额，单位为元，精确到小数点后两位，取值范围[0.01,100000000]  【必选项】</param>
        /// <param name="subject">商品的标题/交易标题/订单标题/订单关键字  【必须项】</param>
        /// <param name="body">对一笔交易的具体描述信息。</param>
        /// <param name="out_trade_no">商户网站唯一订单号 【必须项】</param>
        /// <param name="return_url">wap网站要回跳的页面</param>
        /// <returns></returns>
        private static Dictionary<string, string> GetPublicParameNoSignForWap(string timeout_express, string seller_id, string total_amount, string subject, string body, string out_trade_no, string return_url)
        {
            //获取bizContent的具体内容
            string bizContent = GetPrivateParameter(timeout_express, seller_id, total_amount, subject, body, out_trade_no);
            dict = new Dictionary<string, string>();
            dict.Add("app_id", APP_ID);
            dict.Add("biz_content", bizContent);
            dict.Add("charset", Charset);
            dict.Add("format", Format);
            dict.Add("method", "alipay.trade.wap.pay");
            dict.Add("notify_url", Notify_Url);
            dict.Add("return_url", return_url);    //2017-1-3新增
            dict.Add("sign_type", Sign_Type);
            dict.Add("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            //dict.Add("timestamp", "2016-12-19 18:04:32");
            dict.Add("version", Version);
            return DictionaryOrder(dict);
        }
        #endregion

        #region 获取sign值 [私有方法]
        private static void GetSingValue()
        {
            log.info("获取签名");
            //string RSA_PRIVATE_KEY = GetCurrentPath() + "应用私钥2048.txt";
            //string APP_PRIVATE_KEY = GetCurrentPath() + "app_private_key.pem";
            //把字典变成字符串 key = value & 形式的
            StringBuilder sb = new StringBuilder();
            foreach (var item in dict)
            {
                sb.Append(BuildKeyValue(item.Key, item.Value, false));
                sb.Append("&");
            }
            string data = string.Empty;
            //去掉最后一个&符号
            data = sb.ToString().Substring(0, sb.ToString().Length - 1);
            //data = GetSignContent(dict);
            log.info("获取签名的参数："+data);
            try{
                //log.info("商户的私钥：" +AliPayConst.APP_PRIVATE_KEY);
                log.info("商户的签名方式：" + Sign_Type);
                //获取sign值
                sign = Alipay.AopSdk.Core.Util.AlipaySignature.RSASignCharSet(data, AliPayConst.APP_PRIVATE_KEY, Charset, false, Sign_Type); //ALiPayRSA.sign(data, ALiPayRSA.LoadCertificateFile(APP_PRIVATE_KEY, Sign_Type).ToString(), Charset, Sign_Type);
                log.info("生成的签名为：" + sign);
            }
            catch (Exception e)
            {
                log.error(e.Message);
                throw new Exception(e.Message);
            }

        }
        #endregion

        #region 最终生成返回给app的字符串  [私有方法]
        private static string GetEncodeContent()
        {
            dict.Add("sign", sign);
            dict = DictionaryOrder(dict);   //对字典进行重新排序
            StringBuilder sb = new StringBuilder();
            foreach (var item in dict)
            {
                sb.Append(BuildKeyValue(item.Key, item.Value, true));
                sb.Append("&");
            }
            //去掉最后一个&符号
            string data = sb.ToString().Substring(0, sb.ToString().Length - 1);
            log.info("返回APP的参数：" + data);
            return data;
        }
        #endregion

        #region 获取当前应用的基础路径，方便找到私钥和公钥 [私有方法]
        private static string GetCurrentPath()
        {
            string basePath = System.AppDomain.CurrentDomain.BaseDirectory;
            //int n = basePath.IndexOf("Yunchee.Volkswagen.WebService");
            //basePath = basePath.Substring(0, n)+"YunChee.Volkswagen.Utility\\ALiPayKey\\";
            basePath = basePath + "ALiPayKey\\";
            return basePath;
        }
        #endregion

        #region UrlEncode方法 [私有方法]
        private static string UrlEncode(string str)
        {
            str = System.Web.HttpUtility.UrlEncode(str, System.Text.Encoding.UTF8);
            return str;
        }
        #endregion

        #region 把字典按照字母从小到大的顺序排列 [私有方法]
        /// <summary>
        /// 把字典按照字母从小到大的顺序排列
        /// </summary>
        /// <param name="dict">字典</param>
        /// <returns>排序后的字典</returns>
        private static Dictionary<string, string> DictionaryOrder(Dictionary<string, string> dict)
        {
            return dict.OrderBy(a => a.Key).ToDictionary(a => a.Key, a => a.Value);
        }
        #endregion

        #region 根据key和Value组装为key=value的方式 [私有方法]
        /// <summary>
        /// 根据key和Value组装为key=value的方式 [私有方法]
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">对应的值</param>
        /// <param name="isEncode">是否要做URLEncode加密  true表示加密</param>
        /// <returns>string</returns>
        private static string BuildKeyValue(string key, string value, bool isEncode)
        {
            var str = new StringBuilder();

            str.Append(key);
            str.Append("=");
            if (isEncode)   //等于true就加密
            {
                str.Append(UrlEncode(value));
            }
            else          //否则不加密
            {
                str.Append(value);
            }
            return str.ToString();
        }
        #endregion

        #region PC官网快捷支付所需的方法组新增

        #region  PC官网组装支付参数的方法  【对外调用】
        /// <summary>
        /// PC官网组装支付参数的方法
        /// </summary>
        /// <param name="out_trade_no">商户网站唯一订单号 【必须项】</param>
        /// <param name="subject">商品的标题/交易标题/订单标题/订单关键字  【必须项】</param>
        /// <param name="total_fee">订单总金额，单位为元，精确到小数点后两位，取值范围[0.01,100000000]  【必选项】</param>
        /// <param name="body">对一笔交易的具体描述信息。</param>
        /// <param name="return_url">支付成功后需要跳转的页面</param>
        /// <returns>组装后的串</returns>
        public static string BuildRequest(string out_trade_no, string subject, string total_fee, string body, string return_url)
        {
            string Sell_ID = AliPayConst.Sell_ID;
            #region 组装官网支付参数字典
            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
            sParaTemp.Add("service", "create_direct_pay_by_user");  //无需修改
            sParaTemp.Add("partner", Sell_ID);  //partner的值和sell_id是一样的
            sParaTemp.Add("seller_id", Sell_ID);
            sParaTemp.Add("_input_charset", Charset);
            sParaTemp.Add("payment_type", "1");  //无需修改
            sParaTemp.Add("notify_url", Notify_Url);
            sParaTemp.Add("return_url", return_url);
            sParaTemp.Add("anti_phishing_key", ""); //防钓鱼时间戳  若要使用请调用类文件submit中的Query_timestamp函数
            sParaTemp.Add("exter_invoke_ip", "");  //客户端的IP地址 非局域网的外网IP地址，如：221.0.0.1
            sParaTemp.Add("out_trade_no", out_trade_no);
            sParaTemp.Add("subject", subject);
            sParaTemp.Add("total_fee", total_fee);
            sParaTemp.Add("body", body);  //此值也可以为空
            #endregion

            string strMethod = "get";
            string strButtonValue = "确认";
            //从配置文件读取   支付网关地址
            string GATEWAY_NEW = AliPayConst.GATEWAY_NEW;
            //待请求参数数组
            Dictionary<string, string> dicPara = new Dictionary<string, string>();
            dicPara = BuildRequestPara(sParaTemp);

            StringBuilder sbHtml = new StringBuilder();

            sbHtml.Append("<form id='alipaysubmit' name='alipaysubmit' action='" + GATEWAY_NEW + "_input_charset=" + Charset + "' method='" + strMethod.ToLower().Trim() + "'>");

            foreach (KeyValuePair<string, string> temp in dicPara)
            {
                sbHtml.Append("<input type='hidden' name='" + temp.Key + "' value='" + temp.Value + "'/>");
            }

            //submit按钮控件请不要含有name属性
            sbHtml.Append("<input type='submit' value='" + strButtonValue + "' style='display:none;'></form>");

            sbHtml.Append("<script>document.forms['alipaysubmit'].submit();</script>");

            return sbHtml.ToString();
        }
        #endregion

        #region 生成要请求给支付宝的参数数组
        /// <summary>
        /// 生成要请求给支付宝的参数数组
        /// </summary>
        /// <param name="sParaTemp">初步组装后的参数数组</param>
        /// <returns></returns>
        private static Dictionary<string, string> BuildRequestPara(SortedDictionary<string, string> sParaTemp)
        {
            //待签名请求参数数组
            Dictionary<string, string> sPara = new Dictionary<string, string>();
            //签名结果
            string mysign = "";

            //过滤空值、sign与sign_type参数
            foreach (KeyValuePair<string, string> temp in sParaTemp)
            {
                if (temp.Key.ToLower() != "sign" && temp.Key.ToLower() != "sign_type" && temp.Value != "" && temp.Value != null)
                {
                    sPara.Add(temp.Key, temp.Value);
                }
            }

            //获得签名结果
            mysign = BuildRequestMysign(sPara);

            //签名结果与签名方式加入请求提交参数组中
            sPara.Add("sign", mysign);
            sPara.Add("sign_type", Sign_Type);

            return sPara;
        }
        #endregion

        #region 生成请求时的签名Sign
        /// <summary>
        /// 生成请求时的签名
        /// </summary>
        /// <param name="sPara">请求给支付宝的参数数组</param>
        /// <returns>签名结果</returns>
        private static string BuildRequestMysign(Dictionary<string, string> sPara)
        {
            //把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串
            string prestr = CreateLinkString(sPara);

            //把最终的字符串签名，获得签名结果
            string mysign = "";
            //定义私钥值
            string private_key = AliPayConst.APP_PRIVATE_KEY;
            switch (Sign_Type)
            {
                case "RSA":
                    mysign = ALiPayRSA.sign(prestr, private_key, Charset);
                    break;
                case "RSA2":
                    mysign = ALiPayRSA.sign(prestr, private_key, Charset,Sign_Type);break;
                default:
                    mysign = "";
                    break;
            }

            return mysign;
        }
        #endregion

        #region PC官网快捷支付验签方法    【对外调用】
        /// <summary>
        /// PC官网快捷支付所需的方法
        /// </summary>
        /// <param name="parameters">获取后的字典</param>
        /// <returns></returns>
        public static bool Verify(IDictionary<string, string> parameters)
        {
            string notify_id = parameters["notify_id"];
            string sign = parameters["sign"];
            //-----------支付宝陈工给的公钥--------------------//   不同于之前app的那个公钥
            string alipay_public_key = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCnxj/9qwVfgoUh/y2W89L6BkRAFljhNhgPdyPuBV64bfQNN1PjbCzkIM6qRdKBoLPXmKKMiFYnkd6rAoprih3/PrQEB/VsW8OoM8fxn67UDYuyBTqA23MML9q1+ilIZwBC2AQ2UBVOrFXfFl75p6/B5KsiNG9zpgmLCUYuLkxpLQIDAQAB";
            //string alipay_public_key = File.ReadAllText(sPublicKeyPEM);
            //获取返回时的签名验证结果
            bool isSign = GetSignVeryfy(parameters, sign, alipay_public_key);

            //获取是否是支付宝服务器发来的请求的验证结果
            string responseTxt = "false";
            if (notify_id != null && notify_id != "")
            {
                responseTxt = GetResponseTxt(notify_id);
            }

            if (responseTxt == "true" && isSign)//验证成功
            {
                return true;
            }
            else//验证失败
            {
                return false;
            }
        }
        #endregion

        #region 验签方法
        /// <summary>
        /// 验签方法
        /// </summary>
        /// <param name="parameters">字典参数</param>
        /// <param name="sign">sign签名</param>
        /// <param name="alipay_public_key">公钥</param>
        /// <returns></returns>
        public static bool GetSignVeryfy(IDictionary<string, string> parameters, string sign, string alipay_public_key)
        {
            //获得签名验证结果
            bool isSgin = false;
            Dictionary<string, string> sPara = new Dictionary<string, string>();

            //过滤空值、sign与sign_type参数
            foreach (KeyValuePair<string, string> temp in parameters)
            {
                if (temp.Key.ToLower() != "sign" && temp.Key.ToLower() != "sign_type" && temp.Value != "" && temp.Value != null)
                {
                    sPara.Add(temp.Key, temp.Value);
                }
            }
            //对字典进行排序
            sPara = DictionaryOrder(sPara);
            //获取待签名字符串
            string preSignStr = CreateLinkString(sPara);

            //获得签名验证结果
            if (sign != null && sign != "")
            {
                isSgin = ALiPayRSA.verify(preSignStr, sign, alipay_public_key, Charset);
            }

            return isSgin;
        }
        #endregion

        #region 把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串
        /// <summary>
        /// 把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串
        /// </summary>
        /// <param name="sArray">需要拼接的数组</param>
        /// <returns>拼接完成以后的字符串</returns>
        public static string CreateLinkString(Dictionary<string, string> dicArray)
        {
            StringBuilder prestr = new StringBuilder();
            foreach (KeyValuePair<string, string> temp in dicArray)
            {
                prestr.Append(temp.Key + "=" + temp.Value + "&");
            }

            //去掉最後一個&字符
            int nLen = prestr.Length;
            prestr.Remove(nLen - 1, 1);

            return prestr.ToString();
        }
        #endregion

        #region  获取是否是支付宝服务器发来的请求的验证结果
        /// <summary>
        /// 获取是否是支付宝服务器发来的请求的验证结果
        /// </summary>
        /// <param name="notify_id">通知验证ID</param>
        /// <returns>验证结果</returns>
        private static string GetResponseTxt(string notify_id)
        {
            string Sell_ID = AliPayConst.Sell_ID;
            //支付宝消息验证地址
            string Https_veryfy_url = "https://mapi.alipay.com/gateway.do?service=notify_verify&";
            string veryfy_url = Https_veryfy_url + "partner=" + Sell_ID + "&notify_id=" + notify_id;

            //获取远程服务器ATN结果，验证是否是支付宝服务器发来的请求
            string responseTxt = Get_Http(veryfy_url, 120000);

            return responseTxt;
        }
        #endregion

        #region 获取远程服务器ATN结果
        /// <summary>
        /// 获取远程服务器ATN结果
        /// </summary>
        /// <param name="strUrl">指定URL路径地址</param>
        /// <param name="timeout">超时时间设置</param>
        /// <returns>服务器ATN结果</returns>
        private static string Get_Http(string strUrl, int timeout)
        {
            string strResult;
            try
            {
                HttpWebRequest myReq = (HttpWebRequest)HttpWebRequest.Create(strUrl);
                myReq.Timeout = timeout;
                HttpWebResponse HttpWResp = (HttpWebResponse)myReq.GetResponse();
                Stream myStream = HttpWResp.GetResponseStream();
                StreamReader sr = new StreamReader(myStream, Encoding.Default);
                StringBuilder strBuilder = new StringBuilder();
                while (-1 != sr.Peek())
                {
                    strBuilder.Append(sr.ReadLine());
                }

                strResult = strBuilder.ToString();
            }
            catch (Exception exp)
            {
                strResult = "错误：" + exp.Message;
            }

            return strResult;
        }
        #endregion

        #endregion

        public static string GetSignContent(IDictionary<string, string> parameters)
        {
            // 第一步：把字典按Key的字母顺序排序
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parameters);
            IEnumerator<KeyValuePair<string, string>> dem = sortedParams.GetEnumerator();

            // 第二步：把所有参数名和参数值串在一起
            StringBuilder query = new StringBuilder("");
            while (dem.MoveNext())
            {
                string key = dem.Current.Key;
                string value = dem.Current.Value;
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                {
                    query.Append(key).Append("=").Append(value).Append("&");
                }
            }
            string content = query.ToString().Substring(0, query.Length - 1);

            return content;
        }


    }
}
