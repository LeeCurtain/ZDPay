using System;
using System.Collections.Generic;
using System.Text;

namespace ZDPay.YinShengBao
{
    public  class YinShengBaoConst
    {

        //public YinShengBaoConst(string AccountId, string Key, string ResponseUrlD, string ResponseUrlR)
        //{
        //    this.AccountId = AccountId;
        //    this.Key = Key;
        //    this.ResponseUrlD = ResponseUrlD;
        //    this.ResponseUrlR = ResponseUrlR;
        //}
        /// <summary>
        ///  商户在银生宝注册用户的客户编号
        /// </summary>
        public static string AccountId { get; set; }
        /// <summary>
        /// 商户密匙
        /// </summary>
        public static string Key { get; set; }
        /// <summary>
        /// 结果通知地址代付
        /// </summary>
        public static string ResponseUrlD { get; set; }
        /// <summary>
        /// 结果通知地址快捷支付
        /// </summary>
        public static string ResponseUrlR { get; set; }
        /// <summary>
        ///  商户在银生宝注册用户的客户编号
        /// </summary>
        public static string accountId
        {
            get
            {
                if (AccountId == null)
                {
                    return null;
                }
                else
                {
                    return AccountId;
                }

            }
        }
        /// <summary>
        /// 商户密匙
        /// </summary>
        public static string key
        {
            get
            {
                if (Key == null)
                {
                    return null;
                }
                else
                {
                    return Key;
                }
            }
        }

        /// <summary>
        /// 结果通知地址代付
        /// </summary>
        public static string responseUrlD
        {
            get
            {
                if (ResponseUrlD == null)
                {
                    return null;
                }
                else
                {
                    return ResponseUrlD;
                }
            }
        }
        /// <summary>
        /// 结果通知地址快捷支付
        /// </summary>
        public static string responseUrlR
        {
            get
            {
                if (ResponseUrlR == null)
                {
                    return null;
                }
                else
                {
                    return ResponseUrlR;
                }
            }
        }

        /// <summary>
        /// 前台响应地址
        /// </summary>
        public  const string pageResponseUrl = "http://zdtest.9wins.cn/api/V1/UserOrderRefu";
        /// <summary>
        /// 默认版本 1.0.0；针对异步通知优化商户需要
        ///将版本提升至 1.0.1（异步通知优化功能参考
        ///2.3 含版本号结果通知）
        /// </summary>
        public const string version = "1.0.1";
        /// <summary>
        /// 用户姓名 
        /// </summary>
        public const string name = "";
        /// <summary>
        /// 银行卡号
        /// </summary>
        public const string cardNo = "";
        /// <summary>
        /// 数据格式
        /// </summary>
        public const string conFrom = "application/x-www-form-urlencoded;charset=utf-8";



        /// <summary>
        /// 商户在银生宝注册用户的客户编号
        /// </summary>
        // public const string accountId = "";//  
        /// <summary>
        /// 商户密匙
        /// </summary>
        //public const string key = "";// 
        /// <summary>
        /// 结果通知地址代付
        /// </summary>
        public const string responseUrlD_Test = ";
        /// <summary>
        /// 结果通知地址代付
        /// </summary>
       // public const string responseUrlD = "";
        /// <summary>
        /// 结果通知地址快捷支付
        /// </summary>
        public const string responseUrlR_Test = "";
        /// <summary>
        /// 结果通知地址快捷支付
        /// </summary>
        //public const string responseUrlR = "";

    }
}
