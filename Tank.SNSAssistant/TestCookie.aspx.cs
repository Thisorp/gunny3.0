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

namespace Tank.SNSAssistant
{
    public partial class TestCookie : System.Web.UI.Page
    {
        public string HomePageUrl
        {
            get
            {
                return ConfigurationSettings.AppSettings["HomePageUrl"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string url = string.Format(HomePageUrl, "username");

                string name = Request.Form["order_msg"];
                Response.Write(name);

                //System.Xml.Linq.XElement sendxml = new System.Xml.Linq.XElement("user");
                //string url = string.Format("http://himg.baidu.com/sys/portraitn/item/{0}.jpg ","123");
                //sendxml.Add(new XElement("tinyHeadUrl", url));
                ////Response.ClearContent();
                //Response.ContentType = "text/plain";
                //Response.Write(sendxml.ToString());

                //HttpCookie aCookie = Request.Cookies["userInfo"];

                //if (aCookie != null)
                //{
                //    string bd_sig_user = aCookie.Values["bd_sig_user"];
                //    string bd_sig_portrait = aCookie.Values["bd_sig_portrait"];
                //    string bd_sig_session_key = aCookie.Values["bd_sig_session_key"];

                //    Response.Write("bd_sig_user:" + bd_sig_user +",bd_sig_portrait:" + bd_sig_portrait + ":bd_sig_session_key:" + bd_sig_session_key);
                //}
                //else
                //{
                //    Response.Write("cookie is null!");
                //}
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
            }
        }
    }
}
