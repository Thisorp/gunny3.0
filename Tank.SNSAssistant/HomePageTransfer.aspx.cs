using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Bussiness.Interface;
using System.Text;
using System.Collections.Generic;

namespace Tank.SNSAssistant
{
    public partial class HomePageTransfer : System.Web.UI.Page
    {
        public string ValidateUrl
        {
            get
            {
                return ConfigurationSettings.AppSettings["ValidateUrl"];
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

        public string HomePageUrl
        {
            get
            {
                return ConfigurationSettings.AppSettings["HomePageUrl"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string result = string.Empty;

            try
            {
                HttpCookie aCookie = Request.Cookies["userInfo"];

                if (aCookie != null)
                {
                    string bd_sig_user = aCookie.Values["bd_sig_user"];
                    string bd_sig_portrait = aCookie.Values["bd_sig_portrait"];
                    string bd_sig_session_key = aCookie.Values["bd_sig_session_key"];


                    string uid = Request["uid"] == null ? "" : Request["uid"].ToString();
                    string call_id = DateTime.Now.Millisecond.ToString();
                    ArrayList arr = new ArrayList();
                    Dictionary<string, string> dic = new Dictionary<string, string>();

                    dic.Add("api_key", Api_key);
                    arr.Add("api_key");
                    dic.Add("method", "baidu.users.getInfo");
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
                    dic.Add("uids", uid);
                    arr.Add("uids");

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
                            string param = "api_key=" + HttpUtility.UrlEncode(Api_key, encode) + "&method=" + HttpUtility.UrlEncode("baidu.users.getInfo", encode) + "&bd_sig=" + HttpUtility.UrlEncode(str, encode) + "&v=" + HttpUtility.UrlEncode("1.0", encode) + "&call_id=" + HttpUtility.UrlEncode(call_id, encode)
                                + "&session_key=" + HttpUtility.UrlEncode(bd_sig_session_key, encode) + "&user=" + HttpUtility.UrlEncode(bd_sig_user, encode) + "&portrait=" + HttpUtility.UrlEncode(bd_sig_portrait, encode) + "&format=xml" + "&uids=" + HttpUtility.UrlEncode(uid, encode);


                            result = BaseInterface.RequestContent(ValidateUrl, param, "gbk");
                            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                            doc.LoadXml(result);
                            if (!string.IsNullOrEmpty(result) && result.IndexOf("error_code") == -1)
                            {
                                result = "0";
                            }

                            if (result == "0")
                            {
                                string username = doc["users_getInfo_response"]["user"]["username"].InnerText;
                                string tip = doc["users_getInfo_response"]["user"]["portrait"].InnerText;
                                string url = string.Format(HomePageUrl, HttpUtility.UrlEncode(username));
                                Response.Redirect(url, false);
                            }

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                StatMgr.SaveError(result + ex.ToString());
                Response.Write(ex.ToString());
            }

            if (result != "0")
            {
                StatMgr.SaveError(result);
            }

            //Response.Redirect(HomePageUrl, false);
        }
    }
}
