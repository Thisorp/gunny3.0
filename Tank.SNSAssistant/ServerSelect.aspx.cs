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
using Bussiness;

namespace Tank.SNSAssistant
{
    public partial class ServerSelect : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string bd_sig_user = Request["bd_sig_user"] == null ? "" : Request["bd_sig_user"];
            string bd_sig_portrait = Request["bd_sig_portrait"] == null ? "" : Request["bd_sig_portrait"];
            string bd_sig_session_key = Request["bd_sig_session_key"] == null ? "" : Request["bd_sig_session_key"];

            using (CookieInfoBussiness db = new CookieInfoBussiness())
            {
                try
                {
                    db.AddCookieInfo(bd_sig_user, bd_sig_portrait, bd_sig_session_key);
                    //StatMgr.SaveInfo("将bd_sig_user : " + bd_sig_user + "写到数据库中");
                }
                catch (Exception exception)
                {
                    StatMgr.SaveError("将bd_sig_user : " + bd_sig_user + "写到数据库中发生异常" + exception.ToString());
                }
            }
        }
    }
}
