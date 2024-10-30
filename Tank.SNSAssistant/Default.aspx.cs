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
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string str = Request.QueryString.ToString();
            //string order = "api_key=96478b8657ef00d8b0a1d4d79e759601&method=users.getLoggedInUser&bd_sig=4c4c9ebed30dff05ba4ecb50ccc3b68f&v=1.0&call_id=781&session_key=huzSzuwyds1lq7KvaPwvMQo1VfXj4 UI6UlUJgwjluqt3weUhOL6NktxrDPiZIVa2BVh fcRAejCV iCrJPUhxO7GZ5ck2Ubi3PaW4fTMA2zLyc  EU5krO/stkYphEfaatQLzGgMKQZarmoFrhS6/SSTJ2dkkwQSdktu1ehB9ElQqgkpSIir5hSTuKz6X0U/Uh0nrjFop6iLooUwPvxH6UhnAMfL1loajleRekLFO/PXdYuo15AXytzvjRDArmX&user=553664810&portrait=0f21777765656e6e6262f504&format=xml";
            //bool res = false;
            //if(str==order)
            //    res = true;

            //if(HttpUtility.UrlDecode(str)==order)
            //    res = true;

            Response.Write(str);
        }
    }
}
