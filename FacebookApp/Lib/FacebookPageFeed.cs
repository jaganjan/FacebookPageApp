using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Configuration;

namespace FacebookApp.Lib
{
    /// <summary>
    /// Class FacebookPageFeed.
    /// </summary>
    public class FacebookPageFeed
    {
        /// <summary>
        /// The application identifier
        /// </summary>
        public static string AppId = ConfigurationManager.AppSettings["AppId"];
        /// <summary>
        /// The application secret
        /// </summary>
        public static string AppSecret = ConfigurationManager.AppSettings["AppSecret"];
        /// <summary>
        /// The facebook API URL
        /// </summary>
        public static string FacebookApiUrl = ConfigurationManager.AppSettings["FacebookAPIUrl"];
        // public static string _UserAuthToken = "CAAGJLYa5elsBADIim4j2m2ZCOc9GewKiia5DoiytbCgfQncIt238LModEMRHalxTVyht5GFKJnNBjoA5yRN68SHH9rrNansoukF4qJp9ZBcSDOKo9kEw6WWFI9DRZAaFKDgUmCuO1viZAti7wGgY2wAfaMFpE3VRGKVZCT2Ke41koPAukV3YZCZBnCLd0E2etAisy7PQ4nFWfK7eTgyovLU";

        /// <summary>
        /// Gets the access token.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <returns>System.String.</returns>
        public static string GetAccessToken(string clientId, string clientSecret)
        {
            var apiUrl = FacebookApiUrl+ "/oauth/access_token?type=client_cred&client_id=" + clientId + "&client_secret=" + clientSecret;
            var resultSet = TextServiceConsumer.Get(apiUrl);
            var arr = resultSet.Split('=');
            var acessToken = arr[1];
            return acessToken;
        }
        /// <summary>
        /// Updates the user authentication token.
        /// </summary>
        /// <param name="currentAuthToken">The current authentication token.</param>
        /// <returns>System.String.</returns>
        public static string UpdateUserAuthToken(string currentAuthToken) 
        {
            string apiUrl =$"{FacebookApiUrl}/oauth/access_token?client_id={AppId}&client_secret={AppSecret}&grant_type=fb_exchange_token&fb_exchange_token={currentAuthToken}";
            var resultSet = TextServiceConsumer.Get(apiUrl);
            var arr = resultSet.Split('=');
            var acessToken = arr[1];
            return acessToken;
        }
        /// <summary>
        /// Gets the facebook page feeds.
        /// </summary>
        /// <param name="pageId">The page identifier.</param>
        /// <param name="timeStamp">The time stamp.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>System.Object.</returns>
        public static object GetFacebookPageFeeds(string pageId, int timeStamp, int limit)
        {
            var accessToken = GetAccessToken(AppId, AppSecret);
            //string accessToken = UpdateUserAuthToken(_UserAuthToken);
            var apiUrl =$"{FacebookApiUrl}/{pageId}/feed?since={timeStamp}&limit={limit}&access_token={accessToken}";
            var resultSet = JsonServiceConsumer.Get(apiUrl);
            return resultSet;
        }

        /// <summary>
        /// Fetches the lead details.
        /// </summary>
        /// <param name="profileId">The profile identifier.</param>
        /// <returns>List&lt;KeyValuePair&lt;System.String, System.String&gt;&gt;.</returns>
        public static List<KeyValuePair<string, string>> FetchLeadDetails(string profileId)
        {
           
            var leadData = new List<KeyValuePair<string, string>>();
            try
            {
                var apiUrl = FacebookApiUrl+ "/" + profileId;
                var resultSet = JsonServiceConsumer.Get(apiUrl);
                var result = (JToken)JsonConvert.DeserializeObject(resultSet.ToString());
                if (result != null)
                {
                    if (result["username"] != null)
                    {
                        leadData.Add(new KeyValuePair<string, string>("UserName", result["username"].ToString()));
                    }
                    if (result["gender"] != null)
                    {
                        leadData.Add(new KeyValuePair<string, string>("Gender", result["gender"].ToString()));
                       // LeadData.Add(Result["gender"].ToString());
                    }
                    if (result["link"] != null)
                    {
                        leadData.Add(new KeyValuePair<string, string>("ProfileLink", result["link"].ToString()));
                       // LeadData.Add(Result["link"].ToString());
                    }
                    if (result["locale"] != null)
                    {
                        leadData.Add(new KeyValuePair<string, string>("CountryCode", result["locale"].ToString()));
                        //LeadData.Add(Result["locale"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
            return leadData;
        }
        /// <summary>
        /// Gets the leads.
        /// </summary>
        /// <param name="pageId">The page identifier.</param>
        /// <param name="timeStamp">The time stamp.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="leadSource">The lead source.</param>
        /// <returns>List&lt;List&lt;KeyValuePair&lt;System.String, System.String&gt;&gt;&gt;.</returns>
        public static List<List<KeyValuePair<string, string>>> GetLeads(string pageId, int timeStamp, int limit, string leadSource)
        {

            var leads = new List<List<KeyValuePair<string, string>>>();
            var duplicateCheck = new List<string>();

            var facebookFeed = FacebookPageFeed.GetFacebookPageFeeds(pageId, timeStamp, limit);

            if (facebookFeed == null) return leads;
            var result = (JToken)JsonConvert.DeserializeObject(facebookFeed.ToString());
            if (result?["data"] != null)
            {
                foreach (var dataChild in result["data"].Children())
                {

                    if (leadSource == "All" || leadSource == "Timeline Post")
                    {
                        if (dataChild["from"] != null)
                        {
                            if (dataChild["from"]["id"].ToString() != pageId)
                            {
                                var leadData =
                                    new List<KeyValuePair<string, string>>
                                    {
                                        new KeyValuePair<string, string>("ProfileId",
                                            dataChild["from"]["id"].ToString()),
                                        new KeyValuePair<string, string>("Name",
                                            dataChild["from"]["name"].ToString()),
                                        new KeyValuePair<string, string>("Source", "Timeline Post"),
                                        new KeyValuePair<string, string>("Message", dataChild["message"].ToString())
                                    };
                                   
                                var leadDetails = FetchLeadDetails(dataChild["from"]["id"].ToString());
                                var res = leadData.Concat(leadDetails);
                                leadData = res.ToList();
                                if (duplicateCheck.Contains(dataChild["from"]["id"].ToString()) == false)
                                {
                                    leads.Add(leadData);
                                    duplicateCheck.Add(dataChild["from"]["id"].ToString());
                                }
                            }
                        }
                    }
                    if (leadSource == "All" || leadSource == "Likes")
                    {
                        if (dataChild["likes"]?["data"] != null)
                        {
                            foreach (var lead in dataChild["likes"]["data"].Children())
                            {
                                if (lead["id"].ToString() == pageId) continue;
                                var leadData =
                                    new List<KeyValuePair<string, string>>
                                    {
                                        new KeyValuePair<string, string>("ProfileId", lead["id"].ToString()),
                                        new KeyValuePair<string, string>("Name", lead["name"].ToString()),
                                        new KeyValuePair<string, string>("Source", "Like")
                                    };

                                        
                                var leadDetails = FetchLeadDetails(lead["id"].ToString());
                                var res = leadData.Concat(leadDetails);
                                leadData = res.ToList();
                                if (duplicateCheck.Contains(lead["id"].ToString())) continue;
                                leads.Add(leadData);
                                duplicateCheck.Add(lead["id"].ToString());
                            }
                        }
                    }
                    if (leadSource != "All" && leadSource != "Comments") continue;
                    {
                        if (dataChild["comments"]?["data"] == null) continue;
                        foreach (var lead in dataChild["comments"]["data"].Children())
                        {
                            if (lead["from"]["id"].ToString() == pageId) continue;
                            var leadData =
                                new List<KeyValuePair<string, string>>
                                {
                                    new KeyValuePair<string, string>("ProfileId",
                                        lead["from"]["id"].ToString()),
                                    new KeyValuePair<string, string>("Name", lead["from"]["name"].ToString()),
                                    new KeyValuePair<string, string>("Source", "Comment"),
                                    new KeyValuePair<string, string>("Message", lead["message"].ToString())
                                };
                                       
                            var leadDetails = FetchLeadDetails(lead["from"]["id"].ToString());
                            var res = leadData.Concat(leadDetails);
                            leadData = res.ToList();
                            if (duplicateCheck.Contains(lead["from"]["id"].ToString())) continue;
                            leads.Add(leadData);
                            duplicateCheck.Add(lead["from"]["id"].ToString());
                        }
                    }
                }
            }

            return leads;
        }
       
    }
}