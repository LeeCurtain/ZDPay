using System;
using System.Collections.Generic;
using System.Text;

namespace ZDPay.Model
{
    public class YsbResultModel
    {
        /// <summary>
        /// 商户号
        /// </summary>
        //public string accountId { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string orderId { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public string amount { get; set; }
        /// <summary>
        /// 响应码
        /// </summary>
        public string result_code { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string result_msg { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        public string mac { get; set; }
    }

    public class YsbRefundModel
    {
        /// <summary>
        /// 用户
        /// </summary>
        public string userId { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string orderNo { get; set; }
        /// <summary>
        /// 银行卡尾号(后四位)
        /// </summary>
        public string tailNo { get; set; }
        /// <summary>
        /// 银行名称
        /// </summary>
        public string bankName { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public string amount { get; set; }
        /// <summary>
        /// 响应码
        /// </summary>
        public string result_code { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string result_msg { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        public string mac { get; set; }
    }
}
