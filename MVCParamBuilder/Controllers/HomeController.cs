using System.Web.Mvc;
using MVCParamBuilder.Models;

namespace MVCParamBuilder.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View(new Test { InnerTest = new InnerTest { Id = "1", Name = "Alex", MoreInnerTest =  new MoreInnerTest { Id = "MeineId", Name = "Mein Name"}} });
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your quintessential app description page.";

            return View();
        }

        public ActionResult Test(InnerTest test)
        {
            return new EmptyResult();
        }
    }
}
