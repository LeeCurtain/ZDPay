using System;
using System.Collections.Generic;
using System.Text;

namespace ZDPay.Model
{
   public class YinShengWeb
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public string customerId { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string orderNo { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string commodityName { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public string amount { get; set; }
    }
}
