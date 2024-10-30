using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.MobileControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Bussiness.Interface;
using Bussiness;

namespace Tank.SNSAssistant
{
    public partial class LoginTransfer : System.Web.UI.Page
    {
        public static string FlashUrl
        {
            get
            {
                return ConfigurationSettings.AppSettings["FlashUrl"];
            }
        }

        public string ValidateUrl
        {
            get
            {
                return ConfigurationSettings.AppSettings["ValidateUrl"];
            }
        }

        public string LoginOnUrl
        {
            get
            {
                return ConfigurationSettings.AppSettings["LoginOnUrl"];
            }
        }

        public string Api_key
        {
            get
            {
                return ConfigurationSettings.AppSettings["Api_key"];
            }
        }

        public string Secret
        {
            get
            {
                return ConfigurationSettings.AppSettings["Secret"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string result = "";
            try
            {
                string bd_sig_time = Request["bd_sig_time"] == null ? "" : Request["bd_sig_time"];
                string bd_sig_added = Request["bd_sig_added"] == null ? "" : Request["bd_sig_added"];
                string bd_sig_user = Request["bd_sig_user"] == null ? "" : Request["bd_sig_user"];
                string bd_sig_portrait = Request["bd_sig_portrait"] == null ? "" : Request["bd_sig_portrait"];
                string bd_sig_session_key = Request["bd_sig_session_key"] == null ? "" : Request["bd_sig_session_key"];
                string bd_sig = Request["bd_sig"] == null ? "" : Request["bd_sig"];
                string call_id = DateTime.Now.Millisecond.ToString();
                
                ArrayList arr = new ArrayList();
                Dictionary<string, string> dic = new Dictionary<string, string>();        

                dic.Add("api_key", Api_key);
                arr.Add("api_key");
                dic.Add("method", "baidu.users.getLoggedInUser");
                arr.Add("method");
                dic.Add("v", "1.0");
                arr.Add("v");
                dic.Add("call_id", call_id);
                arr.Add("call_id");
                dic.Add("session_key", bd_sig_session_key);
                arr.Add("session_key");
                dic.Add("user", bd_sig_user);
                arr.Add("user");
                dic.Add("portrait", bd_sig_portrait);
                arr.Add("portrait");
                dic.Add("format", "xml");
                arr.Add("format");
                
                arr.Sort();

                string str = string.Empty;
                foreach (string s in arr)
                {
                    str += s + "=" + dic[s];
                }

                str += Secret;
                str = BaseInterface.md5(str);

                if (!string.IsNullOrEmpty(bd_sig_session_key))
                {
                    if (!string.IsNullOrEmpty(ValidateUrl))
                    {
                        Encoding encode = Encoding.GetEncoding("gbk");
                        string param = "api_key=" + HttpUtility.UrlEncode(Api_key, encode) + "&method=" + HttpUtility.UrlEncode("baidu.users.getLoggedInUser", encode) + "&bd_sig=" + HttpUtility.UrlEncode(str, encode) + "&v=" + HttpUtility.UrlEncode("1.0", encode) + "&call_id=" + HttpUtility.UrlEncode(call_id, encode)
                            + "&session_key=" + HttpUtility.UrlEncode(bd_sig_session_key, encode) + "&user=" + HttpUtility.UrlEncode(bd_sig_user, encode) + "&portrait=" + HttpUtility.UrlEncode(bd_sig_portrait, encode) + "&format=xml";

                        result = BaseInterface.RequestContent(ValidateUrl, param, "gbk");
                        System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                        doc.LoadXml(result);
                        if (!string.IsNullOrEmpty(result) && result.IndexOf("error_code") == -1 && !string.IsNullOrEmpty(doc["users_getLoggedInUser_response"]["uid"].InnerText))
                        {
                            result = "0";
                        }
                    }
                    else
                    {
                        result = "0";
                    }

                    if (result == "0")
                    {
                        string password = Guid.NewGuid().ToString();
                        int time = BaseInterface.ConvertDateTimeInt(DateTime.Now);
                        string v = BaseInterface.md5(bd_sig_user + password + time.ToString() + BaseInterface.GetLoginKey);
                        string Url = BaseInterface.LoginUrl + "?content=" + HttpUtility.UrlEncode(bd_sig_user + "|" + password + "|" + time.ToString() + "|" + v);
                        result = BaseInterface.RequestContent(Url);
                        if (result == "0")
                        {
                            string flashUrl = FlashUrl + "?user=" + HttpUtility.UrlEncode(bd_sig_user) + "&key=" + HttpUtility.UrlEncode(password);
                            Response.Redirect(flashUrl, false);
                            return;
                        }
                        result = "";
                    }

                }

               // Response.Redirect(LoginOnUrl, false);

            }
            catch
            {
                //Response.Redirect(LoginOnUrl, false);
            }
        }
    }
}
