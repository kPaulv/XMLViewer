using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestTask.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Xml;

namespace TestTask.Controllers
{
    public class HomeController : Controller
    {
        CardContext db = new CardContext();
        public ActionResult Index()
        {
            return View(/*db.Cards*/);
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

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase upload)
        {
            if (upload != null)
            {
                string fileName = System.IO.Path.GetFileName(upload.FileName);
                if(fileName.Substring(fileName.Length - 4).Equals(".xml"))
                {
                    //saving file on server
                    XmlReader reader = XmlReader.Create(upload.InputStream);
                    reader.MoveToContent();
                    string tempBills;
                    double tempAmount;

                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(reader);
                    XmlElement xRoot = xDoc.DocumentElement;

                    foreach (XmlNode xnode in xRoot)
                    {
                        // обходим все дочерние узлы элемента card
                        foreach (XmlNode childnode in xnode.ChildNodes)
                        {
                            // если узел: bills
                            if (childnode.Name == "bills")
                            {
                                Console.WriteLine($"bills: {childnode.InnerText}");
                            }
                            // если узел: amount
                            if (childnode.Name == "amount")
                            {
                                Console.WriteLine($"amount: {childnode.InnerText}");
                            }
                        }
                        Console.WriteLine();
                    }

                    //while(reader.Read())
                    //{
                    //    if(reader.NodeType == XmlNodeType.Text)
                    //    {
                    //        if (reader.Name.Equals("bills"))
                    //        {
                    //            tempBills = reader.Value;
                    //            Console.WriteLine(reader.Value);
                    //        }
                    //        if(reader.Name.Equals("amount"))
                    //        {
                    //            tempAmount = Convert.ToDouble(reader.Value);
                    //            Console.WriteLine(reader.Value);
                    //        }
                    //    }
                    //    //switch(reader.NodeType)
                    //    //{
                    //    //    case XmlNodeType.Element:
                    //    //        Console.WriteLine("Start Element {0}", reader.Name);
                    //    //        break;
                    //    //    case XmlNodeType.Text:
                    //    //        Console.WriteLine("Text Node: {0}");
                    //    //        break;
                    //    //    case XmlNodeType.EndElement:
                    //    //        Console.WriteLine("End Element {0}", reader.Name);
                    //    //        break;
                    //    //    default:
                    //    //        Console.WriteLine("Other node {0} with value {1}",
                    //    //            reader.NodeType, reader.Value);
                    //    //        break;
                    //    //}
                    //}
                    upload.SaveAs(Server.MapPath("~/Files/" + fileName));
                }
                
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}