using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using 面試1.Models;

namespace 面試1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult List()
        {
            var db = new NorthwindEntities();
            return View(db.Orders.OrderByDescending(b => b.OrderDate).ToList());
        }

        public ActionResult DeleteConfirm(int id)
        {
            var db = new NorthwindEntities();
            return View(db.Orders.Where(c => c.OrderID == id).FirstOrDefault());
        }

        public ActionResult DeleteBegin (string id)
        {
            int sid = int.Parse(id);
            var db = new NorthwindEntities();
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var orderDetailsToRemove = db.Order_Details.Where(od => od.OrderID == sid);
                    db.Order_Details.RemoveRange(orderDetailsToRemove);

                    var removeObj  = db.Orders.Where(c => c.OrderID == sid).FirstOrDefault();
                    db.Orders.Remove(removeObj);

                    db.SaveChanges();
                    transaction.Commit();
                } catch (Exception ex) {
                    transaction.Rollback();
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }

            return RedirectToAction("List");
        }

        public ActionResult Details (int id)
        {
            var db = new NorthwindEntities();
            return View(db.Order_Details.Where(c => c.OrderID == id).FirstOrDefault());
        }

        public ActionResult Edit(int id)
        {
            return View();
        }

        public ActionResult EditDetail(int id)
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
    }
}