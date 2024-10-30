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
using System.Text;
using Bussiness.Interface;
using System.Collections.Generic;
using Bussiness;

namespace Tank.SNSAssistant
{
    public partial class Pay : System.Web.UI.Page
    {
        public string PayUrl
        {
            get
            {
                return ConfigurationSettings.AppSettings["PayUrl"];
            }
        }

        public string PayMethod
        {
            get
            {
                return ConfigurationSettings.AppSettings["PayMethod"];
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

        public string App_id
        {
            get
            {
                return ConfigurationSettings.AppSettings["App_id"];
            }
        }

        public string ComfirmUrl
        {
            get
            {
                return ConfigurationSettings.AppSettings["ComfirmUrl"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            txtChannel.Text = Request["serverid"] == null ? "" : Request["serverid"].ToString();


        }

        protected void btnSubmit_Click1(object sender, EventArgs e)
        {
            int flag = 0;
            string result = string.Empty;
            string itemName = "dianquan";

            //加入区服务器编号
            string serverId = Request["serverid"] == null ? "s1" : Request["serverid"];
            //string strServer = ConfigurationSettings.AppSettings[serverId];
            //string server = strServer == null ? "" : strServer;
            try
            {
                string bd_sig_user = string.Empty;
                string bd_sig_portrait = string.Empty;
                string bd_sig_session_key = string.Empty;

                bd_sig_user = Request["user"] == null ? "" : Request["user"].ToString();
                
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

                Encoding encode = Encoding.GetEncoding("gbk");
                
                string call_id = DateTime.Now.Millisecond.ToString();
                string order = App_id + DateTime.Now.ToString("yyMMddHHmmss") + RequestUtil.GetNextRandom();
                int money = int.Parse(select.Value);
                int amount = money / 100;
                int time = BaseInterface.ConvertDateTimeInt(DateTime.Now);

                ArrayList arr = new ArrayList();
                Dictionary<string, string> dic = new Dictionary<string, string>();

                dic.Add("api_key", Api_key);
                arr.Add("api_key");
                dic.Add("method", PayMethod);
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

                dic.Add("order_id", order);
                arr.Add("order_id");
                dic.Add("amount", amount.ToString());
                arr.Add("amount");
                dic.Add("desc", HttpUtility.UrlEncode(itemName, encode));//itemName);
                arr.Add("desc");
                dic.Add("time", time.ToString());
                arr.Add("time");

                arr.Sort();

                string bd_sig = string.Empty;
                foreach (string s in arr)
                {
                    bd_sig += s + "=" + dic[s];
                }

                bd_sig += Secret;
                //bd_sig = BaseInterface.md5(HttpUtility.UrlEncode(bd_sig,encode));
                bd_sig = BaseInterface.md5(bd_sig);

                if (!string.IsNullOrEmpty(bd_sig_session_key))
                {
                    using (OrderBussiness db = new OrderBussiness())
                    {
                        //if (db.AddOrder(order, amount, bd_sig_user, "default"))
                        //加入区的信息
                        if (db.AddOrder(order, amount, bd_sig_user, "default", serverId))
                        {
                            string param = "api_key=" + HttpUtility.UrlEncode(Api_key, encode) + "&method=" + HttpUtility.UrlEncode(PayMethod, encode) + "&bd_sig=" + HttpUtility.UrlEncode(bd_sig, encode) + "&v=" + HttpUtility.UrlEncode("1.0", encode) + "&call_id=" + HttpUtility.UrlEncode(call_id, encode)
                                + "&session_key=" + HttpUtility.UrlEncode(bd_sig_session_key, encode) + "&user=" + HttpUtility.UrlEncode(bd_sig_user, encode) + "&portrait=" + HttpUtility.UrlEncode(bd_sig_portrait, encode) + "&format=xml"
                                + "&order_id=" + HttpUtility.UrlEncode(order, encode) + "&amount=" + HttpUtility.UrlEncode(amount.ToString(), encode) + "&desc=" + HttpUtility.UrlEncode(itemName, encode) + "&time=" + HttpUtility.UrlEncode(time.ToString(), encode);

                            result = BaseInterface.RequestContent(PayUrl, param, "gbk");
                            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                            doc.LoadXml(result);
                            if (!string.IsNullOrEmpty(result) && result.IndexOf("error_code") == -1 && doc["pay_regOrder_response"]["result"].InnerText == "0")
                            {
                                flag = 13;
                                result = "0";
                            }

                            if (result == "0")
                            {
                                flag = 14;
                                //order=100019043849&amount=1&app_id=10001&secret_key=%s 进行md5，得到sig传递过来
                                string sig = string.Format("order={0}&amount={1}&app_id={2}&secret_key={3}", order, amount, App_id, Secret);
                                sig = BaseInterface.md5(sig);
                                param = "order=" + HttpUtility.UrlEncode(order, encode) + "&amount=" + HttpUtility.UrlEncode(amount.ToString(), encode) + "&app_id=" + HttpUtility.UrlEncode(App_id, encode)
                                    + "&sig=" + HttpUtility.UrlEncode(sig, encode);

                                Response.Redirect("Confirm.aspx?" + param, false);

                            }
                            else
                            {
                                StatMgr.SaveError("result 5: " + result);
                                flag = 5;
                            }
                        }
                        else
                        {
                            flag = 4;
                        }
                    }
                }
                else
                {
                    flag = 3;
                }
            }
            catch (Exception ex)
            {
                StatMgr.SaveError("result : " + result + ", " + ex.ToString());
                //this.Response.Write(ex.ToString() + flag);
                //return;
            }
            if (flag != 0)
            {
                StatMgr.SaveError(result);
            }

            this.Response.Write("<script language='javascript'>alert('提交失败!!" + flag + "');</script>");
        }
    }
}
