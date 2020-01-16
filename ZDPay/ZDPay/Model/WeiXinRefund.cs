using System;
using System.Collections.Generic;
using System.Text;

namespace ZDPay.Model
{
   public class WeiXinRefund
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string out_trade_no { get; set;}
        /// <summary>
        /// 微信订单号
        /// </summary>
        public string transaction_id { get; set; }
        /// <summary>
        /// 退款订单号
        /// </summary>
        public string out_refund_no { get; set; }
        /// <summary>
        /// 订单金额 单位分
        /// </summary>
        public int total_fee { get; set; }
        /// <summary>
        /// 退款金额 单位分
        /// </summary>
        public int refund_fee { get; set; }
    }
}
