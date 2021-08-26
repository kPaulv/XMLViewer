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

        public ActionResult ValidationError()
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
            var data = db.Cards;

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase upload)
        {
            try
            {
                if (upload != null)
                {
                    string fileName = System.IO.Path.GetFileName(upload.FileName);
                    if (fileName.Substring(fileName.Length - 4).Equals(".xml"))
                    {
                        XmlReader reader = XmlReader.Create(upload.InputStream);
                        reader.MoveToContent();
                        string tempBills = "";
                        double tempAmount = 0.0;

                        XmlDocument xDoc = new XmlDocument();
                        xDoc.Load(reader);
                        XmlElement xRoot = xDoc.DocumentElement;

                        string billsPattern = "[A-Z0-9]+";
                        string amountPattern = "[0-9]+";
                        string[] tokens;

                        List<Card> cardList = new List<Card>();

                        foreach (XmlNode xNode in xRoot)
                        {
                            // iterating through child nodes of a card element
                            foreach (XmlNode childNode in xNode.ChildNodes)
                            {
                                // for bills node
                                if (childNode.Name == "bills")
                                {

                                    if (childNode.InnerText.Length == 16 && Regex.IsMatch(childNode.InnerText,
                                        billsPattern, RegexOptions.IgnoreCase))
                                    {
                                        tempBills = childNode.InnerText;
                                    } else
                                    {
                                        throw new FormatException("Wrong bills format.");
                                    }
                                }
                                // for amount node
                                if (childNode.Name == "amout")
                                {
                                    if (Regex.IsMatch(childNode.InnerText, amountPattern))
                                    {
                                        tokens = childNode.InnerText.Split('.');
                                        int fracPartLength = tokens.Length > 1 ? tokens[1].Length : 0;
                                        if (fracPartLength <= 2)
                                        {
                                            bool success = Double.TryParse(childNode.InnerText, NumberStyles.Number,
                                                                           CultureInfo.InvariantCulture, out tempAmount);
                                        } else
                                        {
                                            throw new FormatException("Wrong amount format.");
                                        }
                                    } else
                                    {
                                        throw new FormatException("Wrong amount format.");
                                    }
                                }
                            }

                            cardList.Add(new Card
                            {
                                Bills = tempBills,
                                Amount = tempAmount
                            });
                        }

                        foreach (Card card in cardList)
                        {
                            db.Cards.Add(card);
                            db.SaveChanges();
                        }
                        //saving file on server
                        upload.SaveAs(Server.MapPath("~/Files/" + fileName));
                    } else
                    {
                        throw new FormatException("Wrong file format.");
                    }
                }
                else
                {
                    throw new FormatException("Upload is null.");
                }

                return RedirectToAction("Index");
            } catch (Exception e)
            {
                string msg = "Validation error: ";
                msg += e.Message;
                return RedirectToAction("Index"/*"ValidationError"*/);
            }
            
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}