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
using System.Collections.Generic;
using System.Xml.Linq;
using Bussiness;
using Bussiness.Interface;
using System.Text;
using Tank.SNSAssistant;

namespace Tank.SNSAssistant
{
    public partial class UserInfo : System.Web.UI.Page
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

        public string PhotoUrl
        {
            get
            {
                return ConfigurationSettings.AppSettings["PhotoUrl"];
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

            string uid = Request["uid"] == null ? "" : Request["uid"].ToString();
            string kind = Request["kind"] == null ? "homepage" : Request["kind"].ToString();

            if (uid.Equals(""))
            {
                Response.Write("uid can't be null");
                return;
            }

            string userName = "";
            string portrait = "";
            bool isInDb = false;
            
            //试从数据库中读取信息
            using(UserInfoBussiness db = new UserInfoBussiness())
            {
                isInDb = db.GetFromDbByUid(uid, ref userName, ref portrait);
            }

            //读取不成功,则请求baidu,并插入到数据库中
            if (!isInDb)
            {
                GetFromBaidu(uid, ref userName, ref portrait);
            }

            if ("homepage".Equals(kind))
            {
                //跳转到用户的主页
                GoHomePage(userName);
            }
            else
            {
                //发送用户的头像url到客户端
                SendTinyHeadUrl(portrait);
            }
        }

        /// <summary>
        /// 从baidu获取用户数据并插入到数据库中
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="userName"></param>
        /// <param name="portrait"></param>
        private void GetFromBaidu(string uid, ref string userName, ref string portrait)
        {
            string result = string.Empty;
            try
            {
                string bd_sig_user = uid;
                string bd_sig_portrait = string.Empty;
                string bd_sig_session_key = string.Empty;

                using (CookieInfoBussiness db = new CookieInfoBussiness())
                {
                    try
                    {
                        db.GetFromDbByUser(bd_sig_user, ref bd_sig_portrait, ref bd_sig_session_key);
                        //StatMgr.SaveInfo("从数据库中读取了cookieInfo, bd_sig_user : " + bd_sig_user + "; bd_sig_portrait : " + bd_sig_portrait + "; bd_sig_session_key" + bd_sig_session_key);
                    }
                    catch (Exception exception)
                    {
                        StatMgr.SaveError("从数据库中读取bd_sig_user : " + bd_sig_user + "发生异常" + exception.ToString());
                    }
                }

                string call_id = DateTime.Now.Millisecond.ToString();
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("api_key", Api_key);
                dic.Add("method", "baidu.users.getInfo");
                dic.Add("v", "1.0");
                dic.Add("call_id", call_id);
                dic.Add("session_key", bd_sig_session_key);
                dic.Add("user", bd_sig_user);
                dic.Add("portrait", bd_sig_portrait);
                dic.Add("format", "xml");
                dic.Add("uids", uid);
                string str = RequestUtil.Md5String(Secret, dic);

                if ((!string.IsNullOrEmpty(bd_sig_session_key)) && (!string.IsNullOrEmpty(ValidateUrl)))
                {
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    dictionary.Add("api_key", Api_key);
                    dictionary.Add("method", "baidu.users.getInfo");
                    dictionary.Add("bd_sig", str);
                    dictionary.Add("v", "1.0");
                    dictionary.Add("call_id", call_id);
                    dictionary.Add("session_key", bd_sig_session_key);
                    dictionary.Add("user", bd_sig_user);
                    dictionary.Add("portrait", bd_sig_portrait);
                    dictionary.Add("format", "xml");
                    dictionary.Add("uids", uid);

                    string param = RequestUtil.PasticheParams(dictionary, "gbk");
                    //StatMgr.SaveInfo("\r\n\r\n\r\nReturnParams========================================\r\n" + param + "\r\n\r\n\r\n==================\r\n");
                    result = BaseInterface.RequestContent(ValidateUrl, param, "gbk");
                    //StatMgr.SaveInfo("\r\n\r\n\r\nLoadXML========================================\r\n" + result + "\r\n\r\n\r\n==================\r\n");
                    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                    doc.LoadXml(result);
                    if (!string.IsNullOrEmpty(result) && result.IndexOf("error_code") == -1)
                    {
                        userName = doc["users_getInfo_response"]["user"]["username"].InnerText;
                        portrait = doc["users_getInfo_response"]["user"]["portrait"].InnerText;
                        //将uid, userName, portrait信息插入到数据库中
                        using (UserInfoBussiness db = new UserInfoBussiness())
                        {
                            db.AddUserInfo(uid, userName, portrait);
                        }
                        return;
                    }
                    else
                    {
                        //写入错误日志
                        StatMgr.SaveError("uid : " + uid + ", " + result);
                    }
                }
            }
            catch (Exception ex)
            {
                StatMgr.SaveError("uid : " + uid + ", " + result + ex.ToString());
                Response.Write(ex.ToString());
            }
        }

        /// <summary>
        /// 跳转到用户主页
        /// </summary>
        /// <param name="userName"></param>
        private void GoHomePage(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return;
            }
            try
            {
                string url = string.Format(HomePageUrl, HttpUtility.UrlEncode(userName));
                Response.Redirect(url, false);
            }
            catch (Exception e)
            {
                StatMgr.SaveError(e.ToString());
            }
            
        }

        /// <summary>
        /// 发送用户的头像url到客户端
        /// </summary>
        /// <param name="portrait"></param>
        private void SendTinyHeadUrl(string portrait)
        {
            if (string.IsNullOrEmpty(portrait))
            {
                return;
            }
            try
            {
                System.Xml.Linq.XElement sendxml = new System.Xml.Linq.XElement("user");
                string url = string.Format(PhotoUrl, portrait);
                sendxml.Add(new XElement("tinyHeadUrl", url));
                Response.Write(sendxml);
            }
            catch (Exception e)
            {
                StatMgr.SaveError(e.ToString());
            }
        }
    }
}
