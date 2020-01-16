using Alipay.AopSdk.Core;
using Alipay.AopSdk.Core.Request;
using Alipay.AopSdk.Core.Response;
using System;
using System.Collections.Generic;
using System.Text;
using ZDPay.Log;
using Newtonsoft.Json;
using ZDPay.Model;

namespace ZDPay.AiLiPay
{
   public class AliPayHelperNew
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
        //public static string GetApplyContent(string timeout_express, string seller_id, string total_amount, string subject, string body, string out_trade_no)
        //{
        //    log.info("app支付请求订单号："+ out_trade_no);
        //    const string URL = "https://openapi.alipay.com/gateway.do";
        //    string APPID = APP_ID;
        //    //开发者应用私钥，由开发者自己生成 
        //    string APP_PRIVATE_KEY = AliPayConst.APP_PRIVATE_KEY;
        //    //参数返回格式，只支持json
        //    string FORMAT = Format;
        //    //请求和签名使用的字符编码格式，支持GBK和UTF-8
        //    string CHARSET = Charset;
        //    //支付宝公钥，由支付宝生成
        //    string ALIPAY_PUBLIC_KEY = AliPayConst.ALIPAY_PUBLIC_KEY;
        //    DefaultAopClient client = new DefaultAopClient(URL, APPID, APP_PRIVATE_KEY, FORMAT, Version, Sign_Type, ALIPAY_PUBLIC_KEY, CHARSET, false);
        //    AlipayTradeAppPayRequest request = new AlipayTradeAppPayRequest();
        //    request.BizContent = AliPayHelper.GetPrivateParameter(timeout_express,seller_id,total_amount,subject,body,out_trade_no);
        //    log.info("bizContent的内容为：" + request.BizContent);
        //    AlipayTradeAppPayResponse response = client.pageExecute(request);

        //    return response.Body;
        //}
        #endregion
        /// <summary>
        /// 订单状态查询
        /// </summary>
        /// <returns></returns>
        public static AlipayTradeQuery GetOrderStatues(string out_trade_no, string trade_no)
        {
            const string URL = "https://openapi.alipay.com/gateway.do";  //沙箱环境与正式环境不同 这里要用沙箱的 支付宝地址https://openapi.alipay.com/gateway.do
                                                                         // https://openapi.alipaydev.com/gateway.do 
            log.info("订单状态查询："+out_trade_no);
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

            DefaultAopClient client = new DefaultAopClient(URL, APPID, APP_PRIVATE_KEY, FORMAT, Version, Sign_Type, ALIPAY_PUBLIC_KEY, CHARSET, false);
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
            string msg = response.Body;
            log.info("订单支付状态查询返回参数："+ msg);
           AlipayTradeQuery alipay = JsonConvert.DeserializeObject<AlipayTradeQuery>(msg);
            return alipay;
        }

        /// <summary>
        /// 订单状态查询
        /// </summary>
        /// <returns></returns>
        public static AlipayTradeQuery GetOrderStatuesn(string out_trade_no, string trade_no)
        {
            const string URL = "https://openapi.alipay.com/gateway.do";
            WebUtils webUtils = new WebUtils();
            var body = webUtils.DoPost(URL + "? charset=" + Charset, AliPayHelper.GetOrderCount(out_trade_no,trade_no),Charset);            
            AlipayTradeQuery alipay = JsonConvert.DeserializeObject<AlipayTradeQuery>(body);
            return alipay;
        }

        /// <summary>
        /// 退款
        /// </summary>
        /// <param name="aliPayRefound"></param>
        public static AlipayRefoundQuery AliPayRefund(AliPayRefound aliPayRefound)
        {
            log.info("订单退款发起：" + aliPayRefound.out_trade_no);
            const string URL = "https://openapi.alipay.com/gateway.do";
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
            IAopClient client = new DefaultAopClient(URL, APPID, APP_PRIVATE_KEY, FORMAT, Version, Sign_Type, ALIPAY_PUBLIC_KEY, CHARSET, false);
            AlipayTradeRefundRequest request = new AlipayTradeRefundRequest();
            request.BizContent = JsonConvert.SerializeObject(aliPayRefound);
            log.info("订单退款发起参数:" + request.BizContent);
            AlipayTradeRefundResponse response = client.Execute(request);
            string result = response.Body;
            log.info("订单退款返回参数：" + result);
            AlipayRefoundQuery refoundQuery= JsonConvert.DeserializeObject<AlipayRefoundQuery>(result);
            return refoundQuery;
        }
    }
}
