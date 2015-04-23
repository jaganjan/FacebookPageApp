using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace FacebookApp.Lib
{
    public class FacebookPageFeed
    {
         public static string _AppId = "1532906690304107";
         public static string _AppSecret = "e723770cb90e14da9961da0ac5be50c6";

        public static string GetAccessToken(string clientId, string clientSecret)
        {
            string ResultSet; string AcessToken = string.Empty;
            try
            {
                string ApiUrl = "https://graph.facebook.com//oauth/access_token?type=client_cred&client_id=" + clientId + "&client_secret=" + clientSecret;
                ResultSet = TextServiceConsumer.Get(ApiUrl).ToString();
                String[] arr = ResultSet.Split('=');
                AcessToken = arr[1];
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return AcessToken;
        }

        public static object GetFacebookPageFeeds(string pageId, int timeStamp, int limit)
        {
            object ResultSet;
            string accessToken = FacebookPageFeed.GetAccessToken(_AppId, _AppSecret);
            try
            {
                string ApiUrl = "https://graph.facebook.com/" + pageId + "/feed?since=" + timeStamp + "&limit=" + limit + "&access_token=" + accessToken;
                ResultSet = JsonServiceConsumer.Get(ApiUrl);
            }
            catch (Exception)
            {

                throw;
            }
            return ResultSet;
        }
        public static List<String> GetLeads(string pageId, int timeStamp, int limit)
        {
            
            List<String> Leads = new List<String>();

            object FacebookFeed = FacebookPageFeed.GetFacebookPageFeeds(pageId, timeStamp, limit);

            if (FacebookFeed != null)
            {
                var Result = (JToken)JsonConvert.DeserializeObject(FacebookFeed.ToString());
                if (Result != null && Result["data"] != null)
                {
                    foreach (var dataChild in Result["data"].Children())
                    {
                        if (dataChild["from"] != null) {
                            if (dataChild["from"]["id"].ToString() != pageId)
                            {
                                string LeadData = dataChild["from"]["id"].ToString() + ',' + dataChild["from"]["name"].ToString();
                                if (Leads.Contains(LeadData) == false)
                                {
                                    Leads.Add(LeadData);
                                }
                            }
                        }
                        
                        if(dataChild["likes"] != null && dataChild["likes"]["data"] != null )
                        {
                            foreach (var lead in dataChild["likes"]["data"].Children())
                            {
                                string LeadData = lead["id"].ToString() + ',' + lead["name"].ToString();
                                if (Leads.Contains(LeadData) == false)
                                {
                                    Leads.Add(LeadData);
                                } 
                               
                             }                           
                        }

                        if (dataChild["comments"] != null && dataChild["comments"]["data"] != null)
                        {
                            foreach (var lead in dataChild["comments"]["data"].Children())
                            {
                                string LeadData = lead["from"]["id"].ToString() + ',' + lead["from"]["name"].ToString();
                                if (Leads.Contains(LeadData) == false)
                                {
                                    Leads.Add(LeadData);
                                }
                               
                            }
                        }
                    }
                }
            }

            return Leads;
        }
       
    }
}