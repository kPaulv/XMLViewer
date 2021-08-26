using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestTask.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Xml;
using System.Text.RegularExpressions;
using System.Globalization;

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
            //var data = Enumerable.Range(1, 10)
            //    .Select(index => new Product
            //    {
            //        ProductID = index,
            //        ProductName = "Product #" + index,
            //        UnitPrice = index * 10,
            //        Discontinued = false
            //    });

            var data = db.Cards;

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        //public ActionResult Select([DataSourceRequest] DataSourceRequest request)
        //{
        //    var data = Enumerable.Range(1, 10)
        //        .Select(index => new Product
        //        {
        //            ProductID = index,
        //            ProductName = "Product #" + index,
        //            UnitPrice = index * 10,
        //            Discontinued = false
        //        });

        //    return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        //}

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
                    string tempBills = "";
                    double tempAmount = 0.0;
                    //string tempAmountString = "";

                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(reader);
                    XmlElement xRoot = xDoc.DocumentElement;

                    string billsPattern = "[A-Z0-9]+";
                    string amountPattern = "[0-9]+";
                    string[] tokens;

                    List<Card> cardList = new List<Card>();

                    foreach (XmlNode xNode in xRoot)
                    {
                        // обходим все дочерние узлы элемента card
                        foreach (XmlNode childNode in xNode.ChildNodes)
                        {
                            // если узел: bills
                            if (childNode.Name == "bills")
                            {
                                
                                if(childNode.InnerText.Length == 16 && Regex.IsMatch(childNode.InnerText, 
                                    billsPattern, RegexOptions.IgnoreCase))
                                {
                                    tempBills = childNode.InnerText;
                                }                
                            }
                            // если узел: amount
                            if (childNode.Name == "amout")
                            {
                                if(Regex.IsMatch(childNode.InnerText, amountPattern))
                                {
                                    tokens = childNode.InnerText.Split('.');
                                    int fracPartLength = tokens.Length > 1 ? tokens[1].Length : 0;
                                    if (fracPartLength <= 2)
                                    {
                                        //tempAmountString = childNode.InnerText;
                                        bool success = Double.TryParse(childNode.InnerText, NumberStyles.Number, CultureInfo.InvariantCulture, out tempAmount);//Convert.ToDouble(childNode.InnerText);
                                    }
                                }
                               
                            }
                        }

                        cardList.Add(new Card
                        {
                            Bills = tempBills,
                            Amount = tempAmount
                        });

                        //Console.WriteLine();
                    }

                    foreach(Card card in cardList)
                    {
                        db.Cards.Add(card);
                        db.SaveChanges();
                    }

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