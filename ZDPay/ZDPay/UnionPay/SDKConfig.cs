using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
namespace ZDPay.UnionPay
{
    public class SDKConfig
    {
        public const string merId = "777290058110048";
        public const string encoding="UTF-8";
        private static string signCertPath= @"E:\ZDAPI源代码(licao)\ZDPay\ZDPay\UnionPay\assets\测试环境证书\acp_test_sign.pfx"; //功能：读取配置文件获取签名证书路径
        private static string signCertPwd= "000000";//功能：读取配置文件获取签名证书密码
        private static string validateCertDir= @"E:\ZDAPI源代码(licao)\ZDPay\ZDPay\UnionPay\assets\测试环境证书\";//功能：读取配置文件获取验签目录
        private static string encryptCert; //功能：加密公钥证书路径

        private static string cardRequestUrl;  //功能：有卡交易路径;
        private static string appRequestUrl= "https://gateway.test.95516.com/gateway/api/appTransReq.do"; //功能：appj交易路径;
        private static string singleQueryUrl= "	https://gateway.test.95516.com/gateway/api/queryTrans.do"; //功能：读取配置文件获取交易查询地址
        private static string fileTransUrl; //功能：读取配置文件获取文件传输类交易地址
        private static string frontTransUrl;//功能：读取配置文件获取前台交易地址
        private static string backTransUrl;//功能：读取配置文件获取后台交易地址
        private static string batTransUrl;//功能：读取配批量交易地址

        private static string frontUrl;//功能：读取配置文件获取前台通知地址
        private static string backUrl= "http://222.222.222.222:8080/demo/api_05_app/BackRcvResponse.aspx";//功能：读取配置文件获取前台通知地址

        private static string jfAppRequestUrl;//功能：缴费产品app交易路径;
        private static string jfSingleQueryUrl; //功能：读取配置文件获取缴费产品交易查询地址
        private static string jfFrontTransUrl;//功能：读取配置文件获取缴费产品前台交易地址
        private static string jfBackTransUrl;//功能：读取配置文件获取缴费产品后台交易地址

        private static string ifValidateRemoteCert = "false";//功能：是否验证后台https证书
        private static string ifValidateCNName = "true";//功能：是否验证证书cn
        private static string middleCertPath;//功能：中级证书路径
        private static string rootCertPath;//功能：根证书路径
        private static string secureKey;//功能：散列方式签名密钥
        private static string signMethod = "01";//功能：指定signMethod
        private static string version = "5.0.0";//功能：指定version
        /// <summary>
        /// 产品类型
        /// 000201：B2C 网关支付 
        /// 000301：认证支付 2.0 
        /// 000302：评级支付 
        /// 000401：代付 
        /// 000501：代收
        /// 000601：账单支付 
        /// 000801：跨行收单
        /// 000901：绑定支付
        /// 001001：订购
        /// 000202：B2B
        /// </summary>
        public const string BizType= "000201";

        /// <summary>
        /// 订单发送时间
        /// </summary>
        public string TxnTime { get; } = DateTime.Now.ToString("yyyyMMddHHmmss");

        /// <summary>
        /// 交易类型
        ///00：查询交易，
        ///01：消费，
        ///02：预授权，
        ///03：预授权完成，
        ///04：退货，
        ///05：圈存，
        ///11：代收，
        ///12：代付，
        ///13：账单支付，
        ///14：转账（保留），
        ///21：批量交易，
        ///22：批量查询，
        ///31：消费撤销，
        ///32：预授权撤销，
        ///33：预授权完成撤销，
        ///71：余额查询，
        ///72：实名认证-建立绑定关系，
        ///73：账单查询，
        ///74：解除绑定关系，
        ///75：查询绑定关系，
        ///77：发送短信验证码交易，
        ///78：开通查询交易，
        ///79：开通交易，
        ///94：IC卡脚本通知 
        ///95：查询更新加密公钥证书
        /// </summary>
        public const string TxnType  = "01";

        /// <summary>
        /// 交易子类
        ///  01：自助消费，通过地址的方式区分前台消费和后台消费（含无跳转支付） 03：分期付款
        /// </summary>
        public const string TxnSubType = "01";
        //private static readonly ILog log = LogManager.GetLogger(typeof(AcpService));
        /// <summary>
        /// 交易币种
        /// </summary>
        public const string currencyCode = "156";
        /// <summary>
        /// 0：普通商户直连接入 2：平台类商户接入
        /// </summary>
        public const string accessType = "0";
        /// <summary>
        /// 渠道类型
        /// </summary>
        public const string channelType = "08";
        public static string CardRequestUrl
        {
            get { return SDKConfig.cardRequestUrl; }
            set { SDKConfig.cardRequestUrl = value; }
        }
        public static string AppRequestUrl
        {
            get { return SDKConfig.appRequestUrl; }
            set { SDKConfig.appRequestUrl = value; }
        }

        public static string FrontTransUrl
        {
            get { return SDKConfig.frontTransUrl; }
            set { SDKConfig.frontTransUrl = value; }
        }
        public static string EncryptCert
        {
            get { return SDKConfig.encryptCert; }
            set { SDKConfig.encryptCert = value; }
        }


        public static string BackTransUrl
        {
            get { return SDKConfig.backTransUrl; }
            set { SDKConfig.backTransUrl = value; }
        }

        public static string SingleQueryUrl
        {
            get { return SDKConfig.singleQueryUrl; }
            set { SDKConfig.singleQueryUrl = value; }
        }

        public static string FileTransUrl
        {
            get { return SDKConfig.fileTransUrl; }
            set { SDKConfig.fileTransUrl = value; }
        }

        public static string SignCertPath
        {
            get { return SDKConfig.signCertPath; }
            set { SDKConfig.signCertPath = value; }
        }

        public static string SignCertPwd
        {
            get { return SDKConfig.signCertPwd; }
            set { SDKConfig.signCertPwd = value; }
        }

        public static string ValidateCertDir
        {
            get { return SDKConfig.validateCertDir; }
            set { SDKConfig.validateCertDir = value; }
        }
        public static string BatTransUrl
        {
            get { return SDKConfig.batTransUrl; }
            set { SDKConfig.batTransUrl = value; }
        }
        public static string BackUrl
        {
            get { return SDKConfig.backUrl; }
            set { SDKConfig.backUrl = value; }
        }
        public static string FrontUrl
        {
            get { return SDKConfig.frontUrl; }
            set { SDKConfig.frontUrl = value; }
        }
        public static string JfCardRequestUrl
        {
            get { return SDKConfig.cardRequestUrl; }
            set { SDKConfig.cardRequestUrl = value; }
        }
        public static string JfAppRequestUrl
        {
            get { return SDKConfig.jfAppRequestUrl; }
            set { SDKConfig.jfAppRequestUrl = value; }
        }

        public static string JfFrontTransUrl
        {
            get { return SDKConfig.jfFrontTransUrl; }
            set { SDKConfig.jfFrontTransUrl = value; }
        }
        public static string JfBackTransUrl
        {
            get { return SDKConfig.jfBackTransUrl; }
            set { SDKConfig.jfBackTransUrl = value; }
        }
        public static string JfSingleQueryUrl
        {
            get { return SDKConfig.jfSingleQueryUrl; }
            set { SDKConfig.jfSingleQueryUrl = value; }
        }

        public static string IfValidateRemoteCert
        {
            get { return SDKConfig.ifValidateRemoteCert; }
            set { SDKConfig.ifValidateRemoteCert = value; }
        }

        public static string IfValidateCNName
        {
            get { return SDKConfig.ifValidateCNName; }
            set { SDKConfig.ifValidateCNName = value; }
        }

        public static string MiddleCertPath
        {
            get { return SDKConfig.middleCertPath; }
            set { SDKConfig.middleCertPath = value; }
        }

        public static string RootCertPath
        {
            get { return SDKConfig.rootCertPath; }
            set { SDKConfig.rootCertPath = value; }
        }

        public static string SecureKey
        {
            get { return SDKConfig.secureKey; }
            set { SDKConfig.secureKey = value; }
        }

        public static string SignMethod
        {
            get { return SDKConfig.signMethod; }
            set { SDKConfig.signMethod = value; }
        }

        public static string Version
        {
            get { return SDKConfig.version; }
            set { SDKConfig.version = value; }
        }
    }
}
