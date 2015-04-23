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
    
           List<String> Leads = FacebookPageFeed.GetLeads(pageId, timeStamp, limit);
          
           return View();
         }
    }
}
