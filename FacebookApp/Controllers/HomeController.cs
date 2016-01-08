using System.Web.Mvc;
using FacebookApp.Lib;

namespace FacebookApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var limit = 25;
            var pageId = "383796804983536";
            var timeStamp = 1429029016;
            var leadSource = "All";
                // Likes = to get lead from likes, Comments = to get leads from Comment, Timeline Post = to get lead from time line posts 
            FacebookPageFeed.GetLeads(pageId, timeStamp, limit, leadSource);

            return View();
        }
    }
}
