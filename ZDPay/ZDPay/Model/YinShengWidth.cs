using System;
using System.Collections.Generic;
using System.Text;

namespace ZDPay.Model
{
    public class YinShengWidth
    {
        /// <summary>
        /// 银行卡号 
        /// </summary>
        public string cardNo { get; set; }
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string orderId { get; set; }
        /// <summary>
        /// 付款目的
        /// </summary>
        public string purpose { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public string amount { get; set; }
        /// <summary>
        /// 收款人身份证号
        /// </summary>
        public string idCardNo { get; set; }
        /// <summary>
        /// 付款人付款摘要
        /// </summary>
        public string summary { get; set; }
        /// <summary>
        /// 收款人手机号 
        /// </summary>
        public string phoneNo { get; set; }
        /// <summary>
        /// 业务种类
        /// </summary>
        public string businessType { get; set; }
    }
}
