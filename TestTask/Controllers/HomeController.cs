using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestTask.Models;
//using TestTask.Models;
//using Kendo.Mvc.Extensions;
//using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace TestTask.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Select([DataSourceRequest] DataSourceRequest request)
        {
            var data = Enumerable.Range(1, 10)
                .Select(index => new Product
                {
                    ProductID = index,
                    ProductName = "Product #" + index,
                    UnitPrice = index * 10,
                    Discontinued = false
                });

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
    }
}