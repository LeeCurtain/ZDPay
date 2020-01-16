using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using LitJson;
using QRCoder;
using ThoughtWorks.QRCode.Codec;
using ZDPay.Model;
using ZDPay.Log;

namespace ZDPay.WeiXinPay
{
    public class WeiXinApiPay
    {
        public static readonly string WX_APPID = WeiXinConst.WX_APPID;
        public static readonly string WX_MCHID = WeiXinConst.WX_MCHID;
        public static readonly string WX_KEY = WeiXinConst.WX_KEY;
        public static readonly string WX_APPSECRET = WeiXinConst.WX_APPSECRET;
        public static readonly string WX_NOTIFY_URL = WeiXinConst.WX_NOTIFY_URL;
        //public static readonly string WX_NOTIFY_URL_TEST = WeiXinConst.WX_NOTIFY_URL_TEST;

        public static readonly string APPWX_APPID = WeiXinConst.APPWX_APPID;
        public static readonly string APPWX_MCHID = WeiXinConst.APPWX_MCHID;
        public static readonly string APPWX_KEY = WeiXinConst.APPWX_KEY;

        private readonly static LogHelp log = new LogHelp(LogType.WeXinPay);

        /// <summary>
        /// openid用于调用统一下单接口
        /// </summary>
        public string openid { get; set; }

        /// <summary>
        /// access_token用于获取收货地址js函数入口参数
        /// </summary>
        public string access_token { get; set; }

        /// <summary>
        /// 商品金额，用于统一下单
        /// </summary>
        public int total_fee { get; set; }

        /// <summary>
        /// 统一下单接口返回结果
        /// </summary>
        public WxPayData unifiedOrderResult { get; set; }

        //public JsApiPay(Page page)
        //{
        //    this.page = page;
        //}

        /**
         * 调用统一下单，获得下单结果
         * @return 统一下单结果
         * @失败时抛异常WxPayException
         */
        public WxPayData GetUnifiedOrderResult(string body, string out_trade_no, string total_fee, string trade_type, string openid, string ip, string countdown, string type)
        {
            double time = double.Parse(countdown);//交易时间
            //统一下单
            WxPayData data = new WxPayData();
            data.SetValue("body", body);//商品描述    是 
            //data.SetValue("attach", "test");//附加数据   否 
            data.SetValue("out_trade_no", out_trade_no);//商户订单号    是 
            data.SetValue("total_fee", total_fee);//标价金额    是 
            data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));//交易起始时间  否 
            data.SetValue("time_expire", DateTime.Now.AddMinutes(time).ToString("yyyyMMddHHmmss"));//交易结束时间  否 
            //data.SetValue("goods_tag", "test");//商品标记   否 
            if (trade_type == "JSAPI")
            {
                data.SetValue("trade_type", "JSAPI");//交易类型   是 
                data.SetValue("openid", openid);//用户标识  否 (交易类型是JSAPI时必须传)
            }
            else if (trade_type == "NATIVE")
            {
                data.SetValue("trade_type", "NATIVE");//交易类型   是 
                data.SetValue("product_id", out_trade_no);
                //data.SetValue("product_id", "123456");
            }
            else if (trade_type == "APP")
            {
                data.SetValue("trade_type", "APP");//交易类型   是 
            }

            WxPayData result = UnifiedOrder(data, ip, type);
            if (!result.IsSet("appid") || !result.IsSet("prepay_id") || result.GetValue("prepay_id").ToString() == "")
            {
                log.error(this.GetType().ToString() + "UnifiedOrder response error!"+ result.ToJson());
                throw new WxPayException(result.ToJson());
            }

            unifiedOrderResult = result;
            log.info("微信预支付参数：" + result.ToXml());
            return result;
        }
        /// <summary>
        /// 微信订单状态查询
        /// </summary>
        /// <param name="transaction_id">微信受理订单号</param>
        /// <param name="out_trade_no">商户订单号</param>
        /// <returns></returns>
        public static WxPayData GetOrderStatues(string transaction_id, string out_trade_no)
        {
            log.info("查询订单支付状态：" + transaction_id);
            string url = "https://api.mch.weixin.qq.com/pay/orderquery";
            string msg = "2";
            int timeOut = 6;
            WxPayData data = new WxPayData();
            data.SetValue("appid", APPWX_APPID);
            data.SetValue("mch_id", APPWX_MCHID);
            if (!string.IsNullOrEmpty(transaction_id))
            {
                data.SetValue("transaction_id", transaction_id);//订单号    是 
            }
            else
            {
                data.SetValue("out_trade_no", out_trade_no);//订单号    是 
            }
            data.SetValue("nonce_str", GenerateNonceStr());//随机字符串
            string sign = data.MakeSign();
            log.info("查询订单支付状态签名：" + sign);
            //签名
            data.SetValue("sign", sign);
            string xml = data.ToXml();
            log.info("查询订单支付状态参数：" + xml);
            //Log.Debug("WxPayApi", "UnfiedOrder request : " + xml);
            string response = HttpService.Post(xml, url, false, timeOut);
            //Log.Debug("WxPayApi", "UnfiedOrder response : " + response);

            WxPayData result = new WxPayData();
            result.FromXml(response);
            log.info("查询订单支付状态返回结果：" + result);
            return result;
        }

        /// <summary>
        /// 退款申请
        /// </summary>
        /// <param name="weiXinRefund"></param>
        /// <returns></returns>
        public static WxPayData AppRefund(WeiXinRefund weiXinRefund)
        {
            log.info("退款申请：" + weiXinRefund.out_trade_no);
            string url = "https://api.mch.weixin.qq.com/secapi/pay/refund";
            int timeOut = 6;
            WxPayData data = new WxPayData();
            data.SetValue("appid", APPWX_APPID);
            data.SetValue("mch_id", APPWX_MCHID);
            data.SetValue("nonce_str", GenerateNonceStr());//随机字符串
            if (!string.IsNullOrEmpty(weiXinRefund.transaction_id))
                data.SetValue("transaction_id", weiXinRefund.transaction_id);
            else
                data.SetValue("out_trade_no", weiXinRefund.out_trade_no);//订单号    是 	
            data.SetValue("out_refund_no", weiXinRefund.out_refund_no);

            data.SetValue("total_fee", weiXinRefund.total_fee);
            data.SetValue("refund_fee", weiXinRefund.refund_fee);
            string sign = data.MakeSign();
            log.info("申请退款签名：" + sign);
            //签名
            data.SetValue("sign", sign);
            string xml = data.ToXml();
            log.info("申请退款参数：" + xml);
            //Log.Debug("WxPayApi", "UnfiedOrder request : " + xml);
            string response = HttpService.Post(xml, url, true, timeOut);
            //Log.Debug("WxPayApi", "UnfiedOrder response : " + response);
            WxPayData result = new WxPayData();
            result.FromXml(response);
            log.info("退款申请返回参数：" + result);
            return result;
        }

        /// <summary>
        /// 接收从微信支付后台发送过来的数据并验证签名
        /// </summary>
        /// <returns>微信支付后台返回的数据</returns>
        public static WxPayData GetNotifyData(string xml)
        {
            //转换数据格式并验证签名
            WxPayData data = new WxPayData();
            try
            {
                data.FromXml(xml.ToString());
            }
            catch (WxPayException ex)
            {
                //若签名错误，则立即返回结果给微信支付后台
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", ex.Message);
                log.info("微信回调参数为：" + res.ToXml());
                return res;
            }

            //Log.Info(this.GetType().ToString(), "Check sign success");
            return data;
        }

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="url">存储内容</param>
        /// <param name="pixel">像素大小</param>
        /// <returns></returns>
        public Bitmap GetQRCode(string url, int pixel)

        {

            QRCodeGenerator generator = new QRCodeGenerator();

            QRCodeData codeData = generator.CreateQrCode(url, QRCodeGenerator.ECCLevel.M, true);

            QRCoder.QRCode qrcode = new QRCoder.QRCode(codeData);
            Bitmap qrImage = qrcode.GetGraphic(pixel, Color.Black, Color.White, true);
            return qrImage;
        }
        /// <summary>
        /// 生成直接支付url
        /// </summary>
        /// <returns></returns>
        public string GetPayUrl()
        {
            WxPayData appApiParam = new WxPayData();
            //appApiParam.SetValue("code_url", unifiedOrderResult.GetValue("code_url"));

            string url = unifiedOrderResult.GetValue("code_url").ToString();//获得统一下单接口返回的二维码链接
            //string parameters = appApiParam.ToJson();

            //初始化二维码生成工具
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            qrCodeEncoder.QRCodeVersion = 6;//设置编码版本
            qrCodeEncoder.QRCodeScale = 10;//设置图片大小

            //将字符串生成二维码图片
            Bitmap image = GetQRCode(url, 200); //qrCodeEncoder.Encode(url, Encoding.Default);

            string filename = DateTime.Now.ToString("yyyymmddhhmmssfff").ToString() + ".jpg";
            string saveUrl = "~/upload/images/";
            string dirPath = Directory.GetCurrentDirectory() + saveUrl;//System.Web.HttpContext.Current.Server.MapPath(saveUrl);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            string ymd = DateTime.Now.ToString("yyyyMMdd", DateTimeFormatInfo.InvariantInfo);
            dirPath += ymd + "/";
            saveUrl += ymd + "/";

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            string filePath = dirPath + filename;
            string fileUrl = saveUrl.Replace("~", "") + filename;

            FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
            image.Save(fs, ImageFormat.Jpeg);

            fs.Close();
            image.Dispose();

            //string fileUrl = saveUrl.Replace("~", "");
            // 网站域名
            string host = WeiXinConst.host;
            host = host.EndsWith("/") ? host.Substring(0, host.LastIndexOf("/")) : host;

            return host + fileUrl;
        }

        /// <summary>
        /// 从统一下单成功返回的数据中获取调起APP付所需的参数，
        /// </summary>
        /// <returns></returns>
        public string GetAppApiParameters()
        {

            //参与签名的字段名为appId，partnerId，prepayId，nonceStr，timeStamp，package。注意：package的值格式为Sign=WXPay 
            WxPayData appApiParam = new WxPayData();
            appApiParam.SetValue("appid", unifiedOrderResult.GetValue("appid"));//应用ID
            appApiParam.SetValue("partnerid", unifiedOrderResult.GetValue("mch_id"));//商户号
            appApiParam.SetValue("prepayid", unifiedOrderResult.GetValue("prepay_id"));//预支付交易会话ID
            appApiParam.SetValue("package", "Sign=WXPay");//扩展字段
            appApiParam.SetValue("noncestr", GenerateNonceStr());//随机字符串
            appApiParam.SetValue("timestamp", GenerateTimeStamp());//时间戳
            //appApiParam.SetValue("signType", "MD5");
            appApiParam.SetValue("paySign", appApiParam.MakeSign());

            string parameters = appApiParam.ToJson();
            return parameters;
        }

        /**
        *  
        * 从统一下单成功返回的数据中获取微信浏览器调起jsapi支付所需的参数，
        * 微信浏览器调起JSAPI时的输入参数格式如下：
        * {
        *   "appId" : "wx2421b1c4370ec43b",     //公众号名称，由商户传入     
        *   "timeStamp":" 1395712654",         //时间戳，自1970年以来的秒数     
        *   "nonceStr" : "e61463f8efa94090b1f366cccfbbb444", //随机串     
        *   "package" : "prepay_id=u802345jgfjsdfgsdg888",     
        *   "signType" : "MD5",         //微信签名方式:    
        *   "paySign" : "70EA570631E4BB79628FBCA90534C63FF7FADD89" //微信签名 
        * }
        * @return string 微信浏览器调起JSAPI时的输入参数，json格式可以直接做参数用
        * 更详细的说明请参考网页端调起支付API：http://pay.weixin.qq.com/wiki/doc/api/jsapi.php?chapter=7_7
        * 
        */
        public string GetJsApiParameters()
        {
            //Log.Debug(this.GetType().ToString(), "JsApiPay::GetJsApiParam is processing...");

            WxPayData jsApiParam = new WxPayData();
            jsApiParam.SetValue("appId", unifiedOrderResult.GetValue("appid"));
            jsApiParam.SetValue("timeStamp", GenerateTimeStamp());
            jsApiParam.SetValue("nonceStr", GenerateNonceStr());
            jsApiParam.SetValue("package", "prepay_id=" + unifiedOrderResult.GetValue("prepay_id"));
            jsApiParam.SetValue("signType", "MD5");
            jsApiParam.SetValue("paySign", jsApiParam.MakeSign());

            string parameters = jsApiParam.ToJson();

            //Log.Debug(this.GetType().ToString(), "Get jsApiParam : " + parameters);
            return parameters;
        }


        /**
	    * 
	    * 获取收货地址js函数入口参数,详情请参考收货地址共享接口：http://pay.weixin.qq.com/wiki/doc/api/jsapi.php?chapter=7_9
	    * @return string 共享收货地址js函数需要的参数，json格式可以直接做参数使用
	    */
        //public string GetEditAddressParameters()
        //{
        //    string parameter = "";
        //    try
        //    {
        //        string host = page.Request.Url.Host;
        //        string path = page.Request.Path;
        //        string queryString = page.Request.Url.Query;
        //        //这个地方要注意，参与签名的是网页授权获取用户信息时微信后台回传的完整url
        //        string url = "http://" + host + path + queryString;

        //        //构造需要用SHA1算法加密的数据
        //        WxPayData signData = new WxPayData();
        //        signData.SetValue("appid", WX_APPID);
        //        signData.SetValue("url", url);
        //        signData.SetValue("timestamp", GenerateTimeStamp());
        //        signData.SetValue("noncestr", GenerateNonceStr());
        //        signData.SetValue("accesstoken", access_token);
        //        string param = signData.ToUrl();

        //        //Log.Debug(this.GetType().ToString(), "SHA1 encrypt param : " + param);
        //        //SHA1加密
        //        string addrSign = FormsAuthentication.HashPasswordForStoringInConfigFile(param, "SHA1");
        //        //Log.Debug(this.GetType().ToString(), "SHA1 encrypt result : " + addrSign);

        //        //获取收货地址js函数入口参数
        //        WxPayData afterData = new WxPayData();
        //        afterData.SetValue("appId", WX_APPID);
        //        afterData.SetValue("scope", "jsapi_address");
        //        afterData.SetValue("signType", "sha1");
        //        afterData.SetValue("addrSign", addrSign);
        //        afterData.SetValue("timeStamp", signData.GetValue("timestamp"));
        //        afterData.SetValue("nonceStr", signData.GetValue("noncestr"));

        //        //转为json格式
        //        parameter = afterData.ToJson();
        //        //Log.Debug(this.GetType().ToString(), "Get EditAddressParam : " + parameter);
        //    }
        //    catch (Exception ex)
        //    {
        //        //Log.Error(this.GetType().ToString(), ex.ToString());
        //        throw new WxPayException(ex.ToString());
        //    }

        //    return parameter;
        //}

        /**
        * 
        * 统一下单
        * @param WxPaydata inputObj 提交给统一下单API的参数
        * @param int timeOut 超时时间
        * @throws WxPayException
        * @return 成功时返回，其他抛异常
        */
        public static WxPayData UnifiedOrder(WxPayData inputObj, string ip, string type, int timeOut = 6)
        {
            string url = "https://api.mch.weixin.qq.com/pay/unifiedorder";
            string msg = "";
            //检测必填参数
            if (!inputObj.IsSet("out_trade_no"))//商户订单号
            {
                msg = "缺少统一支付接口必填参数out_trade_no";
                log.error(msg);
                throw new WxPayException("缺少统一支付接口必填参数out_trade_no！");
            }
            else if (!inputObj.IsSet("body"))//商品描述
            {
                msg = "缺少统一支付接口必填参数body";
                log.error(msg);
                throw new WxPayException("缺少统一支付接口必填参数body！");
            }
            else if (!inputObj.IsSet("total_fee"))//标价金额 
            {
                msg = "缺少统一支付接口必填参数total_fee";
                log.error(msg);
                throw new WxPayException("缺少统一支付接口必填参数total_fee！");
            }
            else if (!inputObj.IsSet("trade_type"))//交易类型
            {
                msg = "缺少统一支付接口必填参数trade_type";
                log.error(msg);
                throw new WxPayException("缺少统一支付接口必填参数trade_type！");
            }

            //关联参数
            if (inputObj.GetValue("trade_type").ToString() == "JSAPI" && !inputObj.IsSet("openid"))
            {
                msg = "统一支付接口中，缺少必填参数openid！trade_type为JSAPI时，openid为必填参数";
                log.error(msg);
                throw new WxPayException("统一支付接口中，缺少必填参数openid！trade_type为JSAPI时，openid为必填参数！");
            }
            if (inputObj.GetValue("trade_type").ToString() == "NATIVE" && !inputObj.IsSet("product_id"))
            {
                msg = "统一支付接口中，缺少必填参数product_id！trade_type为JSAPI时，product_id为必填参数！";
                log.error(msg);
                throw new WxPayException("统一支付接口中，缺少必填参数product_id！trade_type为JSAPI时，product_id为必填参数！");
            }

            //异步通知url未设置，则使用配置文件中的url
            if (!inputObj.IsSet("notify_url"))
            {
                inputObj.SetValue("notify_url", WX_NOTIFY_URL);//异步通知url
            }

            if (type == "1")
            {
                inputObj.SetValue("appid", WX_APPID);//公众账号ID
                inputObj.SetValue("mch_id", WX_MCHID);//商户号
            }
            if (type == "2")//app商户号
            {
                inputObj.SetValue("appid", APPWX_APPID);//公众账号ID
                inputObj.SetValue("mch_id", APPWX_MCHID);//商户号
            }
            inputObj.SetValue("spbill_create_ip", ip);//终端ip	  	    
            inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串
            string sign = inputObj.MakeSign();
            //签名
            inputObj.SetValue("sign", sign);
            log.info("生成签名为：" + sign);
            string xml = inputObj.ToXml();
            log.info("微信预支付请求参数：" + xml);
            var start = DateTime.Now;
            string response = HttpService.Post(xml, url, false, timeOut);
            log.info("返回i数据："+ response);
            var end = DateTime.Now;
            int timeCost = (int)((end - start).TotalMilliseconds);
            log.info("请求耗时：" + timeCost);
            WxPayData result = new WxPayData();
            result.FromXml(response);
            return result;
        }

        /**
       * 根据当前系统时间加随机序列来生成订单号
        * @return 订单号
       */
        public static string GenerateOutTradeNo()
        {
            var ran = new Random();
            return string.Format("{0}{1}{2}", WX_MCHID, DateTime.Now.ToString("yyyyMMddHHmmss"), ran.Next(999));
        }

        /**
        * 生成时间戳，标准北京时间，时区为东八区，自1970年1月1日 0点0分0秒以来的秒数
         * @return 时间戳
        */
        public static string GenerateTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        /**
        * 生成随机串，随机串包含字母或数字
        * @return 随机串
        */
        public static string GenerateNonceStr()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

    }

    public class WxPayData
    {
        public static readonly string WX_KEY = WeiXinConst.WX_KEY;

        public static readonly string APPWX_KEY = WeiXinConst.APPWX_KEY;

        public WxPayData()
        {

        }

        //采用排序的Dictionary的好处是方便对数据包进行签名，不用再签名之前再做一次排序
        private SortedDictionary<string, object> m_values = new SortedDictionary<string, object>();

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
        * @将Dictionary转成xml
        * @return 经转换得到的xml串
        * @throws WxPayException
        **/
        public string ToXml()
        {
            //数据为空时不能转化为xml格式
            if (0 == m_values.Count)
            {
                //Log.Error(this.GetType().ToString(), "WxPayData数据为空!");
                throw new WxPayException("WxPayData数据为空!");
            }

            string xml = "<xml>";
            foreach (KeyValuePair<string, object> pair in m_values)
            {
                //字段值不能为null，会影响后续流程
                if (pair.Value == null)
                {
                    //Log.Error(this.GetType().ToString(), "WxPayData内部含有值为null的字段!");
                    throw new WxPayException("WxPayData内部含有值为null的字段!");
                }

                if (pair.Value.GetType() == typeof(int))
                {
                    xml += "<" + pair.Key + ">" + pair.Value + "</" + pair.Key + ">";
                }
                else if (pair.Value.GetType() == typeof(string))
                {
                    xml += "<" + pair.Key + ">" + "<![CDATA[" + pair.Value + "]]></" + pair.Key + ">";
                }
                else//除了string和int类型不能含有其他数据类型
                {
                    //Log.Error(this.GetType().ToString(), "WxPayData字段数据类型错误!");
                    throw new WxPayException("WxPayData字段数据类型错误!");
                }
            }
            xml += "</xml>";
            return xml;
        }

        /**
        * @将xml转为WxPayData对象并返回对象内部的数据
        * @param string 待转换的xml串
        * @return 经转换得到的Dictionary
        * @throws WxPayException
        */
        public SortedDictionary<string, object> FromXml(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                //Log.Error(this.GetType().ToString(), "将空的xml串转换为WxPayData不合法!");
                throw new WxPayException("将空的xml串转换为WxPayData不合法!");
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlNode xmlNode = xmlDoc.FirstChild;//获取到根节点<xml>
            XmlNodeList nodes = xmlNode.ChildNodes;
            foreach (XmlNode xn in nodes)
            {
                XmlElement xe = (XmlElement)xn;
                m_values[xe.Name] = xe.InnerText;//获取xml的键值对到WxPayData内部的数据中
            }

            try
            {
                //2015-06-29 错误是没有签名
                if (m_values["return_code"] != "SUCCESS")
                {
                    return m_values;
                }
                CheckSign();//验证签名,不通过会抛异常
            }
            catch (WxPayException ex)
            {
                throw new WxPayException(ex.Message);
            }

            return m_values;
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
                    throw new WxPayException("WxPayData内部含有值为null的字段!");
                }

                if (pair.Key != "sign" && pair.Value.ToString() != "")
                {
                    buff += pair.Key + "=" + pair.Value + "&";
                }
            }
            buff = buff.Trim('&');
            return buff;
        }


        /**
        * @Dictionary格式化成Json
         * @return json串数据
        */
        public string ToJson()
        {
            string jsonStr = JsonMapper.ToJson(m_values);
            return jsonStr;
        }

        /**
        * @values格式化成能在Web页面上显示的结果（因为web页面上不能直接输出xml格式的字符串）
        */
        public string ToPrintStr()
        {
            string str = "";
            foreach (KeyValuePair<string, object> pair in m_values)
            {
                if (pair.Value == null)
                {
                    //Log.Error(this.GetType().ToString(), "WxPayData内部含有值为null的字段!");
                    throw new WxPayException("WxPayData内部含有值为null的字段!");
                }
                str += string.Format("{0}={1}<br>", pair.Key, pair.Value.ToString());
            }
            //Log.Debug(this.GetType().ToString(), "Print in Web Page : " + str);
            return str;
        }

        /**
        * @生成签名，详见签名生成算法
        * @return 签名, sign字段不参加签名
        */
        public string MakeSign()
        {
            //转url格式
            string str = ToUrl();
            //在string后加入API KEY
            str += "&key=" + WX_KEY;
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
            if (!IsSet("sign"))
            {
                //Log.Error(this.GetType().ToString(), "WxPayData签名存在但不合法!");
                throw new WxPayException("WxPayData签名存在但不合法!");
            }
            //如果设置了签名但是签名为空，则抛异常
            else if (GetValue("sign") == null || GetValue("sign").ToString() == "")
            {
                //Log.Error(this.GetType().ToString(), "WxPayData签名存在但不合法!");
                throw new WxPayException("WxPayData签名存在但不合法!");
            }

            //获取接收到的签名
            string return_sign = GetValue("sign").ToString();

            //在本地计算新的签名
            string cal_sign = MakeSign();

            if (cal_sign == return_sign)
            {
                return true;
            }

            //Log.Error(this.GetType().ToString(), "WxPayData签名验证错误!");
            throw new WxPayException("WxPayData签名验证错误!");
        }

        /**
        * @获取Dictionary
        */
        public SortedDictionary<string, object> GetValues()
        {
            return m_values;
        }
    }

    public class WxPayException : Exception
    {
        public WxPayException(string msg)
            : base(msg)
        {

        }
    }


    /// <summary>
    /// http连接基础类，负责底层的http通信
    /// </summary>
    public class HttpService
    {

        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            //直接确认，否则打不开    
            return true;
        }

        public static string Post(string xml, string url, bool isUseCert, int timeout)
        {
            System.GC.Collect();//垃圾回收，回收没有正常关闭的http连接

            string result = "";//返回结果

            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream reqStream = null;

            try
            {
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 200;
                //设置https验证方式
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback =
                            new RemoteCertificateValidationCallback(CheckValidationResult);
                }

                /***************************************************************
                * 下面设置HttpWebRequest的相关属性
                * ************************************************************/
                request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "POST";
                request.Timeout = timeout * 1000;

                //设置代理服务器
                //WebProxy proxy = new WebProxy();                          //定义一个网关对象
                //proxy.Address = new Uri(WxPayConfig.PROXY_URL);              //网关服务器端口:端口
                //request.Proxy = proxy;

                //设置POST的数据类型和长度
                request.ContentType = "text/xml";
                byte[] data = System.Text.Encoding.UTF8.GetBytes(xml);
                request.ContentLength = data.Length;

                //是否使用证书
                if (isUseCert)
                {
                    //string path = HttpContext.Current.Request.PhysicalApplicationPath;
                    //X509Certificate2 cert = new X509Certificate2(path + WxPayConfig.SSLCERT_PATH, WxPayConfig.SSLCERT_PASSWORD);
                    //request.ClientCertificates.Add(cert);
                    //Log.Debug("WxPayApi", "PostXml used cert");
                }

                //往服务器写入数据
                reqStream = request.GetRequestStream();
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();

                //获取服务端返回
                response = (HttpWebResponse)request.GetResponse();

                //获取服务端返回数据
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                result = sr.ReadToEnd().Trim();
                sr.Close();
            }
            catch (System.Threading.ThreadAbortException e)
            {
                //Log.Error("HttpService", "Thread - caught ThreadAbortException - resetting.");
                //Log.Error("Exception message: {0}", e.Message);
                System.Threading.Thread.ResetAbort();
            }
            catch (WebException e)
            {
                //Log.Error("HttpService", e.ToString());
                //if (e.Status == WebExceptionStatus.ProtocolError)
                //{
                //    Log.Error("HttpService", "StatusCode : " + ((HttpWebResponse)e.Response).StatusCode);
                //    Log.Error("HttpService", "StatusDescription : " + ((HttpWebResponse)e.Response).StatusDescription);
                //}
                throw new WxPayException(e.ToString());
            }
            catch (Exception e)
            {
                // Log.Error("HttpService", e.ToString());
                throw new WxPayException(e.ToString());
            }
            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return result;
        }

        /// <summary>
        /// 处理http GET请求，返回数据
        /// </summary>
        /// <param name="url">请求的url地址</param>
        /// <returns>http GET成功后返回的数据，失败抛WebException异常</returns>
        public static string Get(string url)
        {
            System.GC.Collect();
            string result = "";

            HttpWebRequest request = null;
            HttpWebResponse response = null;

            //请求url以获取数据
            try
            {
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 200;
                //设置https验证方式
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                }

                /***************************************************************
                * 下面设置HttpWebRequest的相关属性
                * ************************************************************/
                request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "GET";

                //设置代理
                //WebProxy proxy = new WebProxy();
                //proxy.Address = new Uri(WxPayConfig.PROXY_URL);
                //request.Proxy = proxy;

                //获取服务器返回
                response = (HttpWebResponse)request.GetResponse();

                //获取HTTP返回数据
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                result = sr.ReadToEnd().Trim();
                sr.Close();
            }
            catch (System.Threading.ThreadAbortException e)
            {
                //Log.Error("HttpService", "Thread - caught ThreadAbortException - resetting.");
                //Log.Error("Exception message: {0}", e.Message);
                System.Threading.Thread.ResetAbort();
            }
            catch (WebException e)
            {
                //Log.Error("HttpService", e.ToString());
                //if (e.Status == WebExceptionStatus.ProtocolError)
                //{
                //    Log.Error("HttpService", "StatusCode : " + ((HttpWebResponse)e.Response).StatusCode);
                //    Log.Error("HttpService", "StatusDescription : " + ((HttpWebResponse)e.Response).StatusDescription);
                //}
                throw new WxPayException(e.ToString());
            }
            catch (Exception e)
            {
                //Log.Error("HttpService", e.ToString());
                throw new WxPayException(e.ToString());
            }
            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return result;
        }
    }
}