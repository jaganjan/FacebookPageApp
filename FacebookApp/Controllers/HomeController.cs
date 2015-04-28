using System.Net;
using System.IO;
using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using FacebookApp.Lib;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;

namespace FacebookApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
          
           int limit = 25;
           string pageId = "383796804983536";
           int timeStamp = 1429029016;
           string leadSource = "All"; // Likes = to get lead from likes, Comments = to get leads from Comment, Timeline Post = to get lead from time line posts 
           List<List<KeyValuePair<string, string>>> Leads = FacebookPageFeed.GetLeads(pageId, timeStamp, limit, leadSource);
          
           return View();
         }
    }
}
