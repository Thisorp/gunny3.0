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
using Bussiness;
using System.Text;
using System.Collections.Generic;

namespace Tank.SNSAssistant
{
    public partial class PayReturn : System.Web.UI.Page
    {
        public string App_id
        {
            get
            {
                return ConfigurationSettings.AppSettings["App_id"];
            }
        }

        public string Api_key
        {
            get
            {
                return ConfigurationSettings.AppSettings["Api_key"];
            }
        }

        public string CheckMethod
        {
            get
            {
                return ConfigurationSettings.AppSettings["CheckMethod"];
            }
        }

        public string Secret
        {
            get
            {
                return ConfigurationSettings.AppSettings["Secret"];
            }
        }

        public string CheckUrl
        {
            get
            {
                return ConfigurationSettings.AppSettings["CheckUrl"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            string order = Request["order"] == null ? "" : Request["order"];
            string amount = Request["amount"] == null ? "" : Request["amount"];
            string sig = Request["sig"] == null ? "" : Request["sig"];
            string token = string.Empty;
            int result = 1;
            string res = string.Empty;
            StatMgr.SaveInfo("request start time: " + DateTime.Now.ToString("yyMMddHHmmss"));
            try
            {
                string str = string.Format("order={0}&amount={1}&secret_key={2}", order, amount, Secret);
                str = BaseInterface.md5(str);

                using (OrderBussiness db = new OrderBussiness())
                {
                    //加入区信息
                    string serverId = "";
                    string userName = db.GetOrderToName(order, ref serverId);

                    if (sig == str)
                    {
                        int money = int.Parse(amount) * 100;
                        string payWay = "default";

                        if (!string.IsNullOrEmpty(userName))
                        {
                            string ChargeKey = ConfigurationSettings.AppSettings["ChargeKey_" + serverId];
                            string ChargeUrl = ConfigurationSettings.AppSettings["ChargeUrl_" + serverId];
                            string v = BaseInterface.md5(order + userName + money + payWay + amount + ChargeKey);
                            string Url = ChargeUrl + "?content=" + order + "|" + userName + "|" + money + "|" + payWay + "|" + amount + "|" + v;
                            res = BaseInterface.RequestContent(Url);
                            if (res == "0")
                            {
                                //order=100018994345&amount=1&uid=a98218641391f3f26eb1c344cb298d43&secret_key=%s
                                token = string.Format("order={0}&amount={1}&uid={2}&secret_key={3}", order, amount, userName, Secret);
                                token = BaseInterface.md5(token);
                                result = 0;
                            }
                            else
                            {
                                result = 110;
                            }
                        }
                        else
                        {
                            result = 10801;
                        }
                    }
                    else
                    {
                        result = 104;
                    }
                }
            }
            catch (Exception ex)
            {
                StatMgr.SaveError("order id : " + order + ", amount : " + amount + ", " + res + ", result:" + result + ", ex:" + ex);
            }

            //{“result”:”0”,"token":"90190b92e4a134:4dbc37d27b8db51c17”}
            string returnStr = "{\"result\":\"" + result + "\",\"token\":\"" + token + "\"}";
            //将该返回值记录到文本文件中
            string strInfo = "order id : " + order + ", amount : " + amount + ", " + returnStr;
            StatMgr.SaveInfo(strInfo + "request end time: " + DateTime.Now.ToString("yyMMddHHmmss"));
            Response.Write(returnStr);
        }

    }
}
