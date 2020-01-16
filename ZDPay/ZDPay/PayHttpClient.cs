using ZDPay.YinShengBao;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace ZDPay
{
    public class PayHttpClient
    {
        /// <summary>
        /// 请求内容
        /// </summary>
        private Dictionary<String, String> reqContent;

        /// <summary>
        /// 应答内容
        /// </summary>
        private string resContent;

        /// <summary>
        /// 请求方法
        /// </summary>
        private string method;

        /// <summary>
        /// 错误信息
        /// </summary>
        private string errInfo;

        /// <summary>
        /// 超时时间,以秒为单位 
        /// </summary>
        private int timeOut;

        /// <summary>
        /// http应答编码 
        /// </summary>
        private int responseCode;
        /// <summary>
        /// 数据格式
        /// </summary>
        private string contentType;
        /// <summary>
        /// 数据格式
        /// </summary>
        public static string conFrom = "application/x-www-form-urlencoded;charset=utf-8";
        /// <summary>
        /// 银生宝表单数据
        /// </summary>
        private UnspaySignUtil unspaySignUtil;
        private string data;
        public PayHttpClient()
        {
            this.reqContent = new Dictionary<String, String>();
            this.reqContent["url"] = "";
            this.reqContent["data"] = "";

            this.resContent = "";
            this.method = "POST";
            this.errInfo = "";
            this.timeOut = 1 * 60;//5分钟
            this.contentType = "application/json";
            this.responseCode = 0;
            this.data = "ysb";
        }

        /// <summary>
        /// 设置请求数据格式
        /// </summary>
        /// <param name="ContentType"></param>
        public void setYSBFrom(UnspaySignUtil signUtil)
        {
            this.unspaySignUtil = signUtil;
        }
        /// <summary>
        /// 设置请求数据格式
        /// </summary>
        /// <param name="ContentType"></param>
        public void setContentType(string ContentType)
        {
            this.contentType = ContentType;
        }

        /// <summary>
        /// 设置请求内容
        /// </summary>
        /// <param name="reqContent">内容</param>
        public void setReqContent(Dictionary<String, String> reqContent)
        {
            this.reqContent = reqContent;
        }

        /// <summary>
        /// 获取结果内容
        /// </summary>
        /// <returns></returns>
        public string getResContent()
        {
            return this.resContent;
        }

        /// <summary>
        /// 设置请求方法
        /// </summary>
        /// <param name="method">请求方法，可选:POST或GET</param>
        public void setMethod(string method)
        {
            this.method = method;
        }

        /// <summary>
        /// 获取错误信息
        /// </summary>
        /// <returns></returns>
        public string getErrInfo()
        {
            return this.errInfo;
        }

        /// <summary>
        /// 设置超时时间
        /// </summary>
        /// <param name="timeOut">超时时间，单位：秒</param>
        public void setTimeOut(int timeOut)
        {
            this.timeOut = timeOut;
        }

        /// <summary>
        /// 获取http状态码
        /// </summary>
        /// <returns></returns>
        public int getResponseCode()
        {
            return this.responseCode;
        }

        //执行http调用
        public bool call()
        {
            StreamReader sr = null;
            HttpWebResponse wr = null;

            HttpWebRequest hp = null;
            //PayMament pay;
            try
            {
                hp = (HttpWebRequest)WebRequest.Create(this.reqContent["url"]);
                string postData;
                if (this.data == "ysb")
                {
                    postData = this.reqContent["data"];
                }
                else
                {
                    postData = this.data;
                }

                hp.Timeout = this.timeOut * 1000;
                //hp.Timeout = 10 * 1000;
                if (postData != null)
                {
                    //JavaScriptSerializer serializer = new JavaScriptSerializer();
                    // pay = serializer.Deserialize<PayMament>(postData);

                    byte[] data = Encoding.UTF8.GetBytes(postData.ToString());
                    hp.Method = "POST";
                    //hp.Accept = this.contentType;
                    hp.ContentType = this.contentType;
                    hp.ContentLength = data.Length;

                    Stream ws = hp.GetRequestStream();

                    // 发送数据
                    ws.Write(data, 0, data.Length);
                    ws.Close();


                }
                wr = (HttpWebResponse)hp.GetResponse();
                sr = new StreamReader(wr.GetResponseStream(), Encoding.UTF8);

                this.resContent = sr.ReadToEnd();
                sr.Close();
                wr.Close();
            }
            catch (Exception exp)
            {
                this.errInfo += exp.Message;
                if (wr != null)
                {
                    this.responseCode = Convert.ToInt32(wr.StatusCode);
                }

                return false;
            }

            // this.responseCode = Convert.ToInt32(wr.StatusCode);

            return true;
        }
        /// <summary>
        /// post表单数据
        /// </summary>
        /// <returns></returns>
        public bool callForm()
        {
            StreamReader sr = null;
            HttpWebResponse wr = null;

            HttpWebRequest hp = null;
            //PayMament pay;
            try
            {
                hp = (HttpWebRequest)WebRequest.Create(this.reqContent["url"]);
                string postData = YSBForm();//unspay.ToUrl();
                hp.Timeout = this.timeOut * 1000;
                //hp.Timeout = 10 * 1000;
                if (postData != null)
                {
                    byte[] data = Encoding.UTF8.GetBytes(postData.ToString());
                    hp.Method = "POST";
                    //hp.Accept = this.contentType;
                    hp.ContentType = this.contentType;
                    hp.ContentLength = data.Length;

                    Stream ws = hp.GetRequestStream();

                    // 发送数据
                    ws.Write(data, 0, data.Length);
                    ws.Close();


                }
                wr = (HttpWebResponse)hp.GetResponse();
                sr = new StreamReader(wr.GetResponseStream(), Encoding.UTF8);

                this.resContent = sr.ReadToEnd();
                sr.Close();
                wr.Close();
            }
            catch (Exception exp)
            {
                this.errInfo += exp.Message;
                if (wr != null)
                {
                    this.responseCode = Convert.ToInt32(wr.StatusCode);
                }

                return false;
            }

            //this.responseCode = Convert.ToInt32(wr.StatusCode);

            return true;
        }
        /// <summary>
        /// Get
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string Get(string url)
        {
            WebRequest wRequest = WebRequest.Create(url);
            wRequest.Method = "GET";
            wRequest.ContentType = "text/plain;charset=UTF-8";
            WebResponse wResponse = wRequest.GetResponse();
            Stream stream = wResponse.GetResponseStream();
            StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("UTF-8"));
            string str = reader.ReadToEnd();
            reader.Close();
            wResponse.Close();

            return str;
        }
        /// <summary>
        /// 生成表单数据
        /// </summary>
        /// <returns></returns>
        public string YSBForm()
        {
            var parm = this.unspaySignUtil.GetValues();
            bool first_param = true;
            StringBuilder postString = new StringBuilder();
            if (parm != null)
            {
                foreach (var p in parm)
                {
                    if (first_param)
                    {
                        first_param = false;
                    }
                    else
                    {
                        if (p.Key != "mac" && p.Value.ToString() != "")
                        {
                            postString.Append("&");
                        }

                    }
                    if (p.Key != "mac" && p.Value.ToString() != "")
                    {
                        if (p.Value is string)
                        {
                            postString.AppendFormat("{0}={1}", p.Key, p.Value);
                        }
                    }
                }
            }
            //获取mac
            postString.AppendFormat("&{0}={1}", "mac", this.unspaySignUtil.MakeSign());
            return postString.ToString();
        }
        /// <summary>
        ///设置表单的值
        /// </summary>
        /// <param name="value"></param>
        public void setFrom(string value)
        {
            this.data = value;
        }
    }
}
