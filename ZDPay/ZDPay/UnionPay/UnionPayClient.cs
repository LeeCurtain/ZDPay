using System;
using System.Collections.Generic;
using ZDPay.Log;
using ZDPay.UnionPay.Model;
using System.Text;
namespace ZDPay.UnionPay
{
    public class UnionPayClient
    {
        /// <summary>
        /// 日志名称
        /// </summary>
        private readonly static LogHelp log = new LogHelp("UnionPay");
        /// <summary>
        /// AAP支付
        /// </summary>
        /// <param name="payModel"></param>
        /// <returns></returns>
        public string AppPay(UnionPayModel payModel)
        {
            UnionUtil unionUtil = new UnionUtil();
            if (string.IsNullOrEmpty(SDKConfig.Version))
            {
                throw new Exception("版本号不能为空");
            }
            unionUtil.SetValue("version", SDKConfig.Version);
            unionUtil.SetValue("encoding", SDKConfig.encoding);
            unionUtil.SetValue("bizType", SDKConfig.BizType);
            unionUtil.SetValue("txnTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            unionUtil.SetValue("backUrl", SDKConfig.BackUrl);
            unionUtil.SetValue("currencyCode", SDKConfig.currencyCode);
            unionUtil.SetValue("txnAmt", payModel.TotalAmount.ToString());
            unionUtil.SetValue("txnType", SDKConfig.TxnType);
            unionUtil.SetValue("txnSubType", SDKConfig.TxnSubType);
            unionUtil.SetValue("accessType", SDKConfig.accessType);
            unionUtil.SetValue("signMethod", SDKConfig.SignMethod);
            unionUtil.SetValue("channelType", SDKConfig.channelType);
            unionUtil.SetValue("merId", SDKConfig.merId);
            unionUtil.SetValue("orderId", payModel.OrderId);
            unionUtil.SetValue("orderDesc", payModel.orderDesc);
            //unionUtil.SetValue("encryptCertId", AcpService.GetEncryptCertId());
            //unionUtil.SetValue("frontUrl", SDKConfig.FrontUrl);
            unionUtil.SetValue("payTimeout", DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss"));
            //支付卡信息填写
            string accNo = payModel.accNo; //卡号
            Dictionary<string, string> customerInfo = new Dictionary<string, string>();
            customerInfo["phoneNo"] = "18100000000"; //手机号
            customerInfo["smsCode"] = "111111"; //短信验证码,测试环境不会真实收到短信，固定填111111
            AcpService.Sign(unionUtil.GetValues(),Encoding.UTF8);
            Dictionary<String, String> rspData = AcpService.Post(unionUtil.GetValues(), SDKConfig.AppRequestUrl,Encoding.UTF8);
            string tn = "";
            if (rspData.Count != 0)
            {
                if (AcpService.Validate(rspData,Encoding.UTF8))
                {
                    log.info("商户端验证返回报文签名成功。<br>\n");
                    string respcode = rspData["respCode"]; //其他应答参数也可用此方法获取
                    if ("00" == respcode)
                    {
                        //成功
                        //TODO
                        log.info("成功接收tn：" + rspData["tn"] + "<br>\n");
                        tn = rspData["tn"];
                    }
                    else
                    {
                        //其他应答码做以失败处理
                        //TODO
                        log.info("失败：" + rspData["respMsg"] + "。<br>\n");
                        tn = "0";
                    }
                }
                else
                {
                    log.info("商户端验证返回报文签名失败。<br>\n");
                    tn = "1";
                }
            }
            else
            {
                log.info("请求失败<br>\n");
                tn = "3";
            }
            return tn;
        }

        /// <summary>
        /// 订单状态查询
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        public string UnionQuery(UnionQueryModel queryModel)
        {
            //1 表示支付成功 2 继续查询 3是失败  4是请求失败
            string msg = "2";
            UnionUtil unionUtil = new UnionUtil();
            unionUtil.SetValue("version", SDKConfig.Version);
            unionUtil.SetValue("encoding", SDKConfig.encoding);
            unionUtil.SetValue("certId", CertUtil.GetSignCertId());
            unionUtil.SetValue("signMethod", SDKConfig.SignMethod);
            unionUtil.SetValue("txnType", "00");
            unionUtil.SetValue("txnSubType", "00");
            unionUtil.SetValue("accessType", SDKConfig.accessType);
            unionUtil.SetValue("bizType", "000000");
            unionUtil.SetValue("channelType", SDKConfig.channelType);
            unionUtil.SetValue("merId", SDKConfig.merId);
            unionUtil.SetValue("orderId", queryModel.OrderId);
            unionUtil.SetValue("txnTime", queryModel.txnTime);
            //添加签名并读取证书
            AcpService.Sign(unionUtil.GetValues(), Encoding.UTF8);

            Dictionary<String, String> rspData = AcpService.Post(unionUtil.GetValues(), SDKConfig.SingleQueryUrl,Encoding.UTF8);
            if (rspData.Count != 0)
            {

                if (AcpService.Validate(rspData,Encoding.UTF8))
                {
                    log.info("商户端验证返回报文签名成功。<br>\n");
                    string respcode = rspData["respCode"]; //其他应答参数也可用此方法获取
                    if ("00" == respcode)
                    {
                        string origRespCode = rspData["origRespCode"]; //其他应答参数也可用此方法获取
                        //处理被查询交易的应答码逻辑
                        if ("00" == origRespCode)
                        {
                            //交易成功，更新商户订单状态
                            //TODO
                            log.info("交易成功。<br>\n");
                            msg = "1";
                        }
                        else if ("03" == origRespCode ||
                            "04" == origRespCode ||
                            "05" == origRespCode)
                        {
                            //需再次发起交易状态查询交易
                            //TODO
                            log.info("稍后查询。<br>\n");
                            msg = "2";
                        }
                        else
                        {
                            //其他应答码做以失败处理
                            //TODO
                            log.info("交易失败：" + rspData["origRespMsg"] + "。<br>\n");
                            msg = "3";
                        }
                    }
                    else if ("03" == respcode ||
                            "04" == respcode ||
                            "05" == respcode)
                    {
                        //不明原因超时，后续继续发起交易查询。
                        log.info("处理超时，请稍后查询。<br>\n");
                        msg = "2";
                    }
                    else
                    {
                        msg = "3";
                        //其他应答码做以失败处理
                        log.info("查询操作失败：" + rspData["respMsg"] + "。<br>\n");
                    }
                }
            }
            else
            {
                log.info("请求失败\n");
                msg = "4";
            }
            return msg;
        }
    }
}
