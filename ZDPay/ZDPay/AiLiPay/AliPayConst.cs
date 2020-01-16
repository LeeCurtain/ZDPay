using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDPay.AiLiPay
{
    public class AliPayConst
    {
        /// <summary>
        /// 支付宝的APP_ID
        /// </summary>
        public static string APPID { get; set; }
        /// <summary>
        /// 支付宝沙盒环境的测试地址
        /// </summary>
        public static string Host { get; set; }
        /// <summary>
        /// 支付宝正式环境地址
        /// </summary>
        public static string AliPayServiceUrl { get; set; }
        /// <summary>
        /// 支付宝正式环境地址
        /// </summary>
        public static string NotifyUrl { get; set; }
        /// <summary>
        /// 异步通知页面
        /// </summary>
        public static string Notify_Url { get { return Host + NotifyUrl; } }
        /// <summary>
        /// APP产品销售码，支付宝分配的固定值
        /// </summary>
        public static string Product_Code { get; set; }
        /// <summary>
        /// Wap产品销售码，支付宝分配的固定值
        /// </summary>
        public static string WapProduct_Code { get; set; }
        public static string MapiUrl { get; set; }

        /// <summary>
        /// 支付宝的APP_ID
        /// </summary>
        public static string APP_ID
        {
            get { if (APPID == null) return null; else return APPID; }
        }

        #region MyRegion
        /// <summary>
        /// 支付宝的APP_ID
        /// </summary>
        // public const string APP_ID = "2018121462506815";
        /// <summary>
        /// 支付宝沙盒环境的测试地址
        /// </summary>
        public const string Host_Test = "http://zdtest.9wins.cn";
        //public const string Host = "http://zddemo.9wins.cn";
        /// <summary>
        /// 支付宝正式环境地址
        /// </summary>
        // public const string AliPayServiceUrl = "https://openapi.alipay.com/gateway.do";
        /// <summary>
        /// 支付宝正式环境地址
        /// </summary>
        //public const string NotifyUrl = "/api/V1/AliNotifyUrl";
        /// <summary>
        /// APP产品销售码，支付宝分配的固定值
        /// </summary>
        // public const string Product_Code = "QUICK_MSECURITY_PAY";
        /// <summary>
        /// Wap产品销售码，支付宝分配的固定值
        /// </summary>
        //public const string WapProduct_Code = "QUICK_WAP_PAY";
        #endregion

        /// <summary>
        ///编码类型
        /// </summary>
        public const string Charset = "utf-8";
        //加密方式
        public const string Sign_Type = "RSA2";
        //接口版本
        public const string Version = "1.0";
        //异步通知页面
        //  public const string Notify_Url = Host + NotifyUrl; //"http://boram.ngrok.sapronlee.com/app-pay/notify_url.jsp";    
        //参数格式
        public const string Format = "json";

        //合作者的身份ID
        public const string Sell_ID = "";
        // 支付网关地址
        public const string GATEWAY_NEW = "";
        public const string ALIPAY_PUBLIC_KEY = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA4JZ190EyA/ZLUwjE8jHZ81JVyXvZLr0gPouQTLxOjh6Tm7DtdTXMWKBTJ6B1ilg9U10ROo8vUbZhSc2ouNJ3o+ZwGbxlYq1yJ2bD421WknYTUPZIOlBDNpDseI+tpx3pJ/uSavdl9nw8PI9bBMDmox3iXSKSIrQiZGTYOdviCkDc8vdtoHIrNbXQavtYQXM9mGhv18BI8/FsC6V40SNGf7M+EIraXmn99n9X3HjiBttt1TUIYC16/+IEQ0QwvEBGKTmxSRzJQdnRXT4k2tiE1T62TD7/dy5KHocq7u7AzObNLpySde/jAJd4ME6GXPPMrEF6ndZkBzxbQ+CS+Jb4LwIDAQAB";
        public const string AlipayPublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA4JZ190EyA/ZLUwjE8jHZ81JVyXvZLr0gPouQTLxOjh6Tm7DtdTXMWKBTJ6B1ilg9U10ROo8vUbZhSc2ouNJ3o+ZwGbxlYq1yJ2bD421WknYTUPZIOlBDNpDseI+tpx3pJ/uSavdl9nw8PI9bBMDmox3iXSKSIrQiZGTYOdviCkDc8vdtoHIrNbXQavtYQXM9mGhv18BI8/FsC6V40SNGf7M+EIraXmn99n9X3HjiBttt1TUIYC16/+IEQ0QwvEBGKTmxSRzJQdnRXT4k2tiE1T62TD7/dy5KHocq7u7AzObNLpySde/jAJd4ME6GXPPMrEF6ndZkBzxbQ+CS+Jb4LwIDAQAB";
        //public const string MapiUrl = "https://mapi.alipay.com/gateway.do";
        public const string APP_PRIVATE_KEY = "MIIEowIBAAKCAQEAsAtARhhupDDkCl+ekpXmlsbyu0KFsYSvxXI1AJhYI5/Yztq86bXD2dB7qU2/gnjX3yzwNlBaxbl+KgwBa3MMnY9o0XKoRL5vn0wszFSFe1eKPx+UdibsVmZVRbdbv9sPjugpjG5TYnuqlVuDB6ktfMJ2g3pOHfUAU+g5SXXQi5aeDKQQ9IF0aVWqMAEcz1sU2+ebxcBpYZzT6tWh4frv3wIMSjQoShTDk3b+KmajZTdO+H0e9yqhqBDl8eUzmIby/rZ+e8nW2H6ErD+PHPYupcxOttn0IhLeW7zXQTmz1ARFiCtFlmlSsXDnCb06yHbfCzfFmfTVQ1R3k4WIk/QrDwIDAQABAoIBABufOE+HR9z1WuKRzES9xneD8dQWBPoskpPNbqmQPcwhKZiTU67r+TX082JGM7VaFq0K1QoAo62AhF4+kf51B9a1CMJMVkCrDMKW5jSNUoBe5O8wmk/LnauU1O5ibb/1tEQ9Az/ok6bRNpvGxxFtpxh7PrM0fotOxOVle4+eL8Gm9qmkn2eEqnJNxhzy4Ax5ZFsZiT4oboVaObhkwAdq6hkY4cniTx1Ng86ZAzI9kWaUZcPfj9yvTUGmDM2N+3JITccyeaXtO974z2aVpLqOsXCG6OM2BGYueQJVZMAhH7ZzRMK+huQwpUO1lUXgnMr7Ka/Hdw2xf18z0NrZ1mMfoBkCgYEA3X5qVTHEiz+AmbsM93GPlvg3wqBlcXOnd7vL4ElRgvfYPN5J77ay892yc1Zp+VJsA8Q+76vVgMn81fw6nrwBhxKl90s+Uak4+6xCU1zWec4ba7vUo30VUFtWVQKlGUretDsoXcrwhHOyn5ydJ43gFakBEUx37jM4qciDc67LnWMCgYEAy3g1+Rl/MiH8FwlBmIYqEubAcel9aCnjW96dM5aljcPDMaozcEbJuOcqMhHIis0RJE+FKxSu1AQ16K83sF1DBhy0ODYTIX+EgupnWt1j8efW5EWo40x3I62JzvWQsvGiKgJeG0khfGamerluxyIxS8/5V1bPWs8zk96bx76+kWUCgYAb/LJA65XzJmYAhtMfnRjQsbKSBCRtTnjVt0QOphUHoHuRTWE4SfSYveWjj0K6v0BoxW5SaHsE2AD7pPBfSFBFGgUgl0Bbny9VFbIXyH5NS87JMSWwNsjvq2rgOQJVjNFRIrx6BX+YrOngIzD0Y4fyFLXO0sPAtHYvdeeuQWL3EwKBgBrTycIDU5sHkhZzePukTO3bEOq0D33pb/nP+0gvB7sOYbEtsMSQAM7pY83gPGHYsfboqTVJ79cyiu0YFS6izVCD/lse/hy/lHgycdgwMppVqCbG7IKztU219uKbzC1b4LbLQx8z4D9nxXGO+4U0A+3sCQM0BfkCMtm/UyjeBIBlAoGBANlwzdFRO3NK4snV8NwYye7YImDBLK2T0wpQIPhlKecvC3bh6x0C4cb9uP7qO0xNFcWCSL8iXxlXJ/uhaAwsB7g08eDMp+DJiCP8NLt4cUz3br/v7M7dvoCUvLhdTc4nNwBLmQ070LQNuhwsTMV1x0rDMICkiX5Ad6+WuUFXi4aF";
    }
}

