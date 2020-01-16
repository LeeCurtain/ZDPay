using System;
using System.Collections.Generic;
using System.Text;

namespace ZDPay.Model
{
   public class AliPayRefound
    {
        /// <summary>
        /// 订单支付时传入的商户订单号
        /// </summary>
        public string out_trade_no { get; set; }
        /// <summary>
        /// 支付宝交易号
        /// </summary>
        public string trade_no { get; set; }
        /// <summary>
        /// 需要退款的金额
        /// </summary>
        public decimal refund_amount { get; set; }
        /// <summary>
        /// 退款的原因说明
        /// </summary>
        public string refund_reason { get; set; }
        /// <summary>
        /// 标识一次退款请求，同一笔交易多次退款需要保证唯一，如需部分退款，则此参数必传。
        /// </summary>
        public string out_request_no { get; set; }
    }
}
