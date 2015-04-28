using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Configuration;

namespace FacebookApp.Lib
{
    public class FacebookPageFeed
    {
         public static string _AppId = ConfigurationManager.AppSettings["AppId"].ToString();
         public static string _AppSecret = ConfigurationManager.AppSettings["AppSecret"].ToString();
         public static string _FacebookAPIUrl = ConfigurationManager.AppSettings["FacebookAPIUrl"].ToString();
        // public static string _UserAuthToken = "CAAGJLYa5elsBADIim4j2m2ZCOc9GewKiia5DoiytbCgfQncIt238LModEMRHalxTVyht5GFKJnNBjoA5yRN68SHH9rrNansoukF4qJp9ZBcSDOKo9kEw6WWFI9DRZAaFKDgUmCuO1viZAti7wGgY2wAfaMFpE3VRGKVZCT2Ke41koPAukV3YZCZBnCLd0E2etAisy7PQ4nFWfK7eTgyovLU";

        public static string GetAccessToken(string clientId, string clientSecret)
        {
            string ResultSet; string AcessToken = string.Empty;
            try
            {
                string ApiUrl = _FacebookAPIUrl+ "/oauth/access_token?type=client_cred&client_id=" + clientId + "&client_secret=" + clientSecret;
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
        public static string UpdateUserAuthToken(string currentAuthToken) 
        {
            string ResultSet; string AcessToken = string.Empty;
            try
            {
                string ApiUrl = _FacebookAPIUrl+ "/oauth/access_token?client_id="+_AppId+"&client_secret="+_AppSecret+"&grant_type=fb_exchange_token&fb_exchange_token="+currentAuthToken;
                ResultSet = TextServiceConsumer.Get(ApiUrl).ToString();
                String[] arr = ResultSet.Split('=');
                AcessToken = arr[1];
            }
            catch (Exception)
            {
                
                throw;
            }
            return AcessToken;
        }
        public static object GetFacebookPageFeeds(string pageId, int timeStamp, int limit)
        {
            object ResultSet;
            string accessToken = FacebookPageFeed.GetAccessToken(_AppId, _AppSecret);
            //string accessToken = UpdateUserAuthToken(_UserAuthToken);
            try
            {
                string ApiUrl = _FacebookAPIUrl+ "/" + pageId + "/feed?since=" + timeStamp + "&limit=" + limit + "&access_token=" + accessToken;
                ResultSet = JsonServiceConsumer.Get(ApiUrl);
            }
            catch (Exception)
            {

                throw;
            }
            return ResultSet;
        }

        public static List<KeyValuePair<string, string>> FetchLeadDetails(string profileId)
        {
           
            List<KeyValuePair<string, string>> LeadData = new List<KeyValuePair<string, string>>();
            try
            {
                string ApiUrl = _FacebookAPIUrl+ "/" + profileId;
                object ResultSet = JsonServiceConsumer.Get(ApiUrl);
                var Result = (JToken)JsonConvert.DeserializeObject(ResultSet.ToString());
                if (Result != null)
                {
                    if (Result["username"] != null)
                    {
                        LeadData.Add(new KeyValuePair<string, string>("UserName", Result["username"].ToString()));
                    }
                    if (Result["gender"] != null)
                    {
                        LeadData.Add(new KeyValuePair<string, string>("Gender", Result["gender"].ToString()));
                       // LeadData.Add(Result["gender"].ToString());
                    }
                    if (Result["link"] != null)
                    {
                        LeadData.Add(new KeyValuePair<string, string>("ProfileLink", Result["link"].ToString()));
                       // LeadData.Add(Result["link"].ToString());
                    }
                    if (Result["locale"] != null)
                    {
                        LeadData.Add(new KeyValuePair<string, string>("CountryCode", Result["locale"].ToString()));
                        //LeadData.Add(Result["locale"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
            return LeadData;
        }
        public static List<List<KeyValuePair<string, string>>> GetLeads(string pageId, int timeStamp, int limit, string leadSource)
        {

            List<List<KeyValuePair<string, string>>> Leads = new List<List<KeyValuePair<string, string>>>();
            List<string> DuplicateCheck = new List<string>();

            object FacebookFeed = FacebookPageFeed.GetFacebookPageFeeds(pageId, timeStamp, limit);

            if (FacebookFeed != null)
            {
                var Result = (JToken)JsonConvert.DeserializeObject(FacebookFeed.ToString());
                if (Result != null && Result["data"] != null)
                {
                    foreach (var dataChild in Result["data"].Children())
                    {

                        if (leadSource == "All" || leadSource == "Timeline Post")
                        {
                            if (dataChild["from"] != null)
                            {
                                if (dataChild["from"]["id"].ToString() != pageId)
                                {
                                    List<KeyValuePair<string, string>> LeadData = new List<KeyValuePair<string, string>>();
                                    LeadData.Add(new KeyValuePair<string, string>("ProfileId",dataChild["from"]["id"].ToString()));
                                    LeadData.Add(new KeyValuePair<string, string>("Name", dataChild["from"]["name"].ToString()));
                                    LeadData.Add(new KeyValuePair<string, string>("Source", "Timeline Post"));
                                    LeadData.Add(new KeyValuePair<string, string>("Message", dataChild["message"].ToString()));
                                    //LeadData.Add(dataChild["from"]["name"].ToString());
                                   // LeadData.Add("Timeline Post");
                                   // LeadData.Add(dataChild["message"].ToString());
                                    List<KeyValuePair<string, string>> LeadDetails = FetchLeadDetails(dataChild["from"]["id"].ToString());
                                    var Res = LeadData.Concat(LeadDetails);
                                    LeadData = Res.ToList();
                                    // string duplicate_check = string.Join(",", LeadData.ToArray());
                                    if (DuplicateCheck.Contains(dataChild["from"]["id"].ToString()) == false)
                                    {
                                        Leads.Add(LeadData);
                                        DuplicateCheck.Add(dataChild["from"]["id"].ToString());
                                    }
                                }
                            }
                        }
                        if (leadSource == "All" || leadSource == "Likes")
                        {
                            if (dataChild["likes"] != null && dataChild["likes"]["data"] != null)
                            {
                                foreach (var lead in dataChild["likes"]["data"].Children())
                                {
                                    if (lead["id"].ToString() != pageId)
                                    {
                                        List<KeyValuePair<string, string>> LeadData = new List<KeyValuePair<string, string>>();
                                        LeadData.Add(new KeyValuePair<string, string>("ProfileId", lead["id"].ToString()));
                                        LeadData.Add(new KeyValuePair<string, string>("Name", lead["name"].ToString()));
                                        LeadData.Add(new KeyValuePair<string, string>("Source", "Like"));

                                        //List<String> LeadData = new List<String>();
                                        // LeadData.Add(lead["id"].ToString());
                                        // LeadData.Add(lead["name"].ToString());
                                        //  LeadData.Add("Like");
                                        List<KeyValuePair<string, string>> LeadDetails = FetchLeadDetails(lead["id"].ToString());
                                        var Res = LeadData.Concat(LeadDetails);
                                        LeadData = Res.ToList();
                                        //string LeadData = lead["id"].ToString() + ',' + lead["name"].ToString();
                                        if (DuplicateCheck.Contains(lead["id"].ToString()) == false)
                                        {
                                            Leads.Add(LeadData);
                                            DuplicateCheck.Add(lead["id"].ToString());
                                        }
                                    }
                                }
                            }
                        }
                        if (leadSource == "All" || leadSource == "Comments")
                        {
                            if (dataChild["comments"] != null && dataChild["comments"]["data"] != null)
                            {
                                foreach (var lead in dataChild["comments"]["data"].Children())
                                {
                                    if (lead["from"]["id"].ToString() != pageId)
                                    {
                                        List<KeyValuePair<string, string>> LeadData = new List<KeyValuePair<string, string>>();
                                        LeadData.Add(new KeyValuePair<string, string>("ProfileId", lead["from"]["id"].ToString()));
                                        LeadData.Add(new KeyValuePair<string, string>("Name", lead["from"]["name"].ToString()));
                                        LeadData.Add(new KeyValuePair<string, string>("Source", "Comment"));
                                        LeadData.Add(new KeyValuePair<string, string>("Message", lead["message"].ToString()));
                                        /* List<String> LeadData = new List<String>();
                                         LeadData.Add(lead["from"]["id"].ToString());
                                         LeadData.Add(lead["from"]["name"].ToString());
                                         LeadData.Add("Comment");
                                         LeadData.Add(lead["message"].ToString()); */
                                        List<KeyValuePair<string, string>> LeadDetails = FetchLeadDetails(lead["from"]["id"].ToString());
                                        var Res = LeadData.Concat(LeadDetails);
                                        LeadData = Res.ToList();
                                        //string LeadData = lead["from"]["id"].ToString() + ',' + lead["from"]["name"].ToString();
                                        if (DuplicateCheck.Contains(lead["from"]["id"].ToString()) == false)
                                        {
                                            Leads.Add(LeadData);
                                            DuplicateCheck.Add(lead["from"]["id"].ToString());
                                        }
                                    }
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