using System;
using System.Collections.Generic;
using System.Text;

namespace ZDPay.Model
{
    public class AlipayTradeQuery
    {
        /// <summary>
        /// 实体参数
        /// </summary>
        public object alipay_trade_query_response { get; set; }
        /// <summary>
        /// 返回的签名
        /// </summary>
        public string sign { get; set; }
    }

    public class AlipayRefoundQuery
    {
        /// <summary>
        /// 实体参数
        /// </summary>
        public object alipay_trade_refund_response { get; set; }
        /// <summary>
        /// 返回的签名
        /// </summary>
        public string sign { get; set; }
    }
}
