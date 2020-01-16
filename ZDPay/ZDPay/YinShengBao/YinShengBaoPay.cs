using LitJson;
using ZDPay.Model;
using System;
using System.Collections.Generic;
using System.Text;
using ZDPay.Log;
namespace ZDPay.YinShengBao
{
    public class YinShengBaoPay
    {
        #region 参数
        public readonly static string accountId = YinShengBaoConst.accountId;
        public readonly static string cardNo = YinShengBaoConst.cardNo;
        public readonly static string name = YinShengBaoConst.name;
        public readonly static string key = YinShengBaoConst.key;
        public readonly static string responseUrlD = YinShengBaoConst.responseUrlD;
        public readonly static string responseUrlR = YinShengBaoConst.responseUrlR;
        public readonly static string version = YinShengBaoConst.version;
        public readonly static string pageResponseUr = YinShengBaoConst.pageResponseUrl;
        #endregion

        private static PayHttpClient pay = new PayHttpClient();
        /// <summary>
        /// 日志名称
        /// </summary>
        private readonly static LogHelp log = new LogHelp(LogType.YSBPay);
        #region 代付
        /// <summary>
        ///代付组装参数
        /// </summary>
        /// <param name="yinShengWidth"></param>
        /// <returns></returns>
        public static string WidthRaw(YinShengWidth yinShengWidth)
        {
            //组装参数
            UnspaySignUtil data = new UnspaySignUtil();
            data.SetValue("accountId", YinShengBaoConst.accountId);
            data.SetValue("name", yinShengWidth.name);
            data.SetValue("cardNo", yinShengWidth.cardNo);
            data.SetValue("orderId", yinShengWidth.orderId);
            data.SetValue("purpose", yinShengWidth.purpose);
            data.SetValue("amount", yinShengWidth.amount);
            data.SetValue("idCardNo", yinShengWidth.idCardNo);
            data.SetValue("summary", yinShengWidth.summary);
            if (!string.IsNullOrEmpty(yinShengWidth.phoneNo))
            {
                data.SetValue("phoneNo", yinShengWidth.phoneNo);
            }
            if (!string.IsNullOrEmpty(responseUrlD))
            {
                data.SetValue("responseUrl", responseUrlD);
            }
            if (!string.IsNullOrEmpty(yinShengWidth.businessType))
            {
                data.SetValue("businessType", yinShengWidth.businessType);
            }
            if (!string.IsNullOrEmpty(version))
            {
                data.SetValue("version", version);
            }
            string resutl = UnifiedOrder(data);
            return resutl;
        }

        /**
      * 
      * 代付请求
      * @param WxPaydata inputObj 提交给统一下单API的参数
      * @param int timeOut 超时时间
      * @throws YbPayException
      * @return 成功时返回，其他抛异常
      */
        public static string UnifiedOrder(UnspaySignUtil inputObj)
        {
            log.info("代付请求开始");
            string message = "";
            string url = "http://180.166.114.155:7181/delegate-pay-front-dp/delegatePay/fourElementsPay";//http://180.166.114.155:7181/delegate-pay-front-dp-dp/delegatePay/fourElementsPay
            //检测必填参数
            if (!inputObj.IsSet("orderId"))//商户订单号
            {
                message = "缺少统一支付接口必填参数orderId！";
                log.error(message);
                throw new YbPayException(message);
            }
            else if (!inputObj.IsSet("purpose"))//商品描述
            {
                message = "缺少统一支付接口必填参数purpose！";
                log.error(message);
                throw new YbPayException(message);
            }
            else if (!inputObj.IsSet("amount"))
            {
                message = "缺少统一支付接口必填参数amount！";
                log.error(message);
                throw new YbPayException(message);
            }
            else if (!inputObj.IsSet("idCardNo"))
            {
                message = "缺少统一支付接口必填参数idCardNo！";
                log.error(message);
                throw new YbPayException(message);
            }
            else if (!inputObj.IsSet("summary"))
            {
                message = "缺少统一支付接口必填参数summary！";
                log.error(message);
                throw new YbPayException(message);
            }
            //签名
            inputObj.SetValue("mac", inputObj.MakeSign());
            //string data = inputObj.ToJson(); //JsonMapper.ToJson(inputObj);
            log.info("请求地址：" + url);
            //log.info("请求参数：" + data);
            string result = ResponOrder(url,inputObj);
            log.info("返回参数："+result);
            return result;
        }

        /// <summary>
        /// 获取订单状态
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static string GetOrderStatues(string orderId)
        {
            log.info("获取订单状态请求开始");
            string url = "http://180.166.114.155:7181/delegate-pay-front-dp/delegatePay/queryOrderStatus";
            UnspaySignUtil unspaySign = new UnspaySignUtil();
            unspaySign.SetValue("accountId", YinShengBaoConst.accountId);
            unspaySign.SetValue("orderId", orderId);
            unspaySign.SetValue("mac", unspaySign.MakeSign());
            log.info("请求地址：" + url);
            string result = ResponOrder(url, unspaySign);
            log.info("返回参数：" + result);
            return result;
        }
        /// <summary>
        /// 商户账户余额查询接口 
        /// </summary>
        /// <returns></returns>
        public static string GetAllPrice()
        {
            log.info("获取商户账户余额请求开始");
            string url = "http://180.166.114.155:7181/delegate-pay-front-dp/delegatePay/queryBlance";
            UnspaySignUtil unspaySign = new UnspaySignUtil();
            unspaySign.SetValue("accountId", YinShengBaoConst.accountId);
            unspaySign.SetValue("mac", unspaySign.MakeSign());
            log.info("请求地址：" + url);
            string result = ResponOrder(url, unspaySign);
            log.info("返回参数：" + result);
            return result;
        }
        #endregion

        #region 网页快捷支付
        /// <summary>
        /// 网页快捷支付组装参数
        /// </summary>
        /// <param name="yinShengWeb"></param>
        /// <returns></returns>
        public static string WebprePay(YinShengWeb yinShengWeb)
        {
            //组装参数
            UnspaySignUtil data = new UnspaySignUtil();
            data.SetValue("accountId", YinShengBaoConst.accountId);
            if (!string.IsNullOrEmpty(yinShengWeb.customerId))
            {
                data.SetValue("customerId", yinShengWeb.customerId);
            }
            data.SetValue("orderNo", yinShengWeb.orderNo);
            data.SetValue("commodityName", yinShengWeb.commodityName);
            data.SetValue("amount", yinShengWeb.amount);
            if (!string.IsNullOrEmpty(responseUrlR))
            {
                data.SetValue("responseUrl", responseUrlR);
            }
            if (!string.IsNullOrEmpty(pageResponseUr))
            {
                data.SetValue("pageResponseUrl", pageResponseUr);
            }
            if (!string.IsNullOrEmpty(version))
            {
                data.SetValue("version", version);
            }
            string resutl = WebChackPay(data);
            return resutl;
        }
        /// <summary>
        /// 验证参数发送请求
        /// </summary>
        /// <param name="unspaySignUtil"></param>
        /// <returns></returns>
        public static string WebChackPay(UnspaySignUtil inputObj)
        {
            log.info("web支付开始");
            string msg = "";
            string url = "http://180.166.114.155:18083/quickpay-front/quickPayWap/prePay";
            //检测必填参数
            if (!inputObj.IsSet("orderNo"))//商户订单号
            {
                msg = "缺少统一支付接口必填参数orderNo！";
                log.error(msg);
                throw new YbPayException(msg);
            }
            else if (!inputObj.IsSet("commodityName"))//商品描述
            {
                msg = "缺少统一支付接口必填参数commodityName！";
                log.error(msg);
                throw new YbPayException(msg);
            }
            else if (!inputObj.IsSet("amount"))
            {
                msg = "缺少统一支付接口必填参数amount！";
                log.error(msg);
                throw new YbPayException(msg);
            }
            else if (!inputObj.IsSet("responseUrl"))
            {
                msg = "缺少统一支付接口必填参数responseUrl！";
                log.error(msg);
                throw new YbPayException(msg);
            }
            else if (!inputObj.IsSet("pageResponseUrl"))
            {
                msg = "缺少统一支付接口必填参数pageResponseUrl ！";
                log.error(msg);
                throw new YbPayException(msg);
            }
            //签名
            inputObj.SetValue("mac", inputObj.MakeSign());
            //string data = inputObj.ToJson();
            log.info("请求地址：" + url);
            //log.info("请求参数：" + data);
            string result = ResponOrder(url, inputObj);
            log.info("返回参数：" + result);
            return result;
        }

        #endregion

        #region 退款
        /// <summary>
        /// 退款申请
        /// </summary>
        /// <param name="orderNo">退款订单号</param>
        /// <param name="oriOrderNo">原订单号</param>
        /// <param name="amount">金额（元为单位，保留两位小数）</param>
        /// <param name="purpose">目的</param>
        /// <returns></returns>
        public static string RefundPay(string orderNo, string oriOrderNo, string amount, string purpose)
        {
            log.info("退款申请开始");
            string msg = "";
            string url = "http://180.166.114.155:18083/quickpay-front/quickPayWap/refund";
            //判断参数是否为空
            if (string.IsNullOrEmpty(orderNo))
            {
                msg = "orderNo为空";
                log.error(msg);
                throw new YbPayException(msg);
            }
            if (string.IsNullOrEmpty(oriOrderNo))
            {
                msg = "oriOrderNo为空";
                log.error(msg);
                throw new YbPayException(msg);
            }
            if (string.IsNullOrEmpty(amount))
            {
                msg = "amount为空";
                log.error(msg);
                throw new YbPayException(msg);
            }
            if (string.IsNullOrEmpty(purpose))
            {
                msg = "purpose为空";
                log.error(msg);
                throw new YbPayException(msg);
            }
            //组装参数
            UnspaySignUtil data = new UnspaySignUtil();
            data.SetValue("accountId", YinShengBaoConst.accountId);
            data.SetValue("orderNo", orderNo);
            data.SetValue("oriOrderNo", oriOrderNo);
            data.SetValue("amount", amount);
            data.SetValue("amount", purpose);
            //签名
            data.SetValue("mac", data.MakeSign());
            //string res = data.ToJson();
            log.info("请求地址：" + url);
           // log.info("请求参数：" + data);
            string result = ResponOrder(url, data);
            log.info("返回参数：" + result);
            return result;
        }
        #endregion

        #region 获取支付状态 web
        /// <summary>
        /// 获取订单状态
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static string GetOrderStatuesWeb(string orderNo)
        {
            log.info("web获取订单状态开始");
            string url = "http://180.166.114.155:18083/quickpay-front/queryOrderStatus";
            UnspaySignUtil unspaySign = new UnspaySignUtil();
            unspaySign.SetValue("accountId", YinShengBaoConst.accountId);
            unspaySign.SetValue("orderNo", orderNo);
            unspaySign.SetValue("mac", unspaySign.MakeSign());
            log.info("请求地址：" + url);
            string result = ResponOrder(url, unspaySign);
            log.info("返回参数：" + result);
            return result;
        }
        #endregion

        #region 退款订单状态
        /// <summary>
        /// 获取订单状态
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static string GetRefundPayStatues(string orderNo)
        {
            log.info("获取订单状态开始");
            string url = "http://180.166.114.155:18083/quickpay-front/quickPayWap/queryRefundStatus";
            UnspaySignUtil unspaySign = new UnspaySignUtil();
            unspaySign.SetValue("accountId", YinShengBaoConst.accountId);
            unspaySign.SetValue("orderNo", orderNo);
            unspaySign.SetValue("mac", unspaySign.MakeSign());
            log.info("请求地址：" + url);
            string result = ResponOrder(url, unspaySign);
            log.info("返回参数：" + result);
            return result;
        }
        #endregion

        #region post封装调用
        /// <summary>
        /// httt请求 并返回结果 post
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ResponOrder(string url,UnspaySignUtil unspaySign)
        {
            string data="";
            Dictionary<string, string> reqContent = new Dictionary<string, string>();
            reqContent.Add("url", url);
            reqContent.Add("data", data);
            pay.setReqContent(reqContent);
            pay.setContentType(YinShengBaoConst.conFrom);
            pay.setYSBFrom(unspaySign);
            pay.setFrom(unspaySign.YSBForm());
            log.info("表单参数："+ unspaySign.YSBForm());
            //获取应答内容
            string resContent = "";
            try
            {
                if (pay.call())
                {
                    //获取应答内容
                    resContent = pay.getResContent();
                }
                else
                {
                    resContent = pay.getErrInfo();
                }
                return resContent;
            }
            catch (Exception e)
            {
                log.error(e.Message);
                throw new Exception(e.Message);
            }

        }
        #endregion

    }
}
