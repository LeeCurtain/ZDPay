using System;
using System.Collections.Generic;
using System.Text;

namespace ZDPay.UnionPay.Model
{
   public class UnionPayModel
    {
        /// <summary>
        /// 交易金额,单位分
        /// </summary>
        public int TotalAmount { get; set; }

        /// <summary>
        /// 商户订单号，不应含“-”或“_” 商户订单号最小长度为8位,最大长度为40位
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 卡号
        /// </summary>
        public string accNo { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string phoneNo { get; set; }
        /// <summary>
        /// 短信验证码
        /// </summary>
        public string smsCode { get; set; }
        /// <summary>
        /// 订单描述
        /// </summary>
        public string orderDesc { get; set; }
    }

    public class UnionQueryModel {

        /// <summary>
        /// 商户订单号，不应含“-”或“_” 商户订单号最小长度为8位,最大长度为40位
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 订单发送时间
        /// </summary>
        public string txnTime { get; set; }
    }
}
