using System.Collections.Generic;
using System.Collections;
namespace Tank.SNSAssistant
{
    public class RequestUtil
    {
        static System.Random random = new System.Random();

        public static string PasticheParams(Dictionary<string, string> dic, string encoding)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            System.Text.Encoding encode = System.Text.Encoding.GetEncoding(encoding);
            foreach (KeyValuePair<string, string> kvp in dic)
            {
                sb.Append(string.Format("{0}={1}&", kvp.Key, System.Web.HttpUtility.UrlEncode(kvp.Value, encode)));
            }
            string strTemp = sb.ToString();
            return strTemp.Substring(0, strTemp.Length - 1);
        }

        public static string Md5String(string secret, Dictionary<string, string> dic)
        {
            ArrayList arr = new ArrayList();
            foreach(string str in dic.Keys)
            {
                arr.Add(str);
            }
            arr.Sort();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (string s in arr)
            {
                sb.Append(s + "=" + dic[s]);
            }
            sb.Append(secret);
            return Bussiness.Interface.BaseInterface.md5(sb.ToString());
        }

        public static string GetNextRandom()
        {
            return random.Next(10, 99).ToString();
        }
    }
}