using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using 面試1.Models;

namespace 面試1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var db = new NorthwindEntities();
            return View(db.Orders.OrderByDescending(b => b.OrderDate).ToList());
        }

        public ActionResult Create()
        {
            CustomerID();
            EmployeeID();
            ShipVia();
            return View();
        }

        public ActionResult CreateSave(Orders orders)
        {
            var db = new NorthwindEntities();
                try
                {
                    int flag = db.Database.ExecuteSqlCommand("INSERT INTO Orders " +
                        "(CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry) " +
                        "Values (@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12)",
                        orders.CustomerID,
                        orders.EmployeeID,
                        DateTime.Now,
                        DateTime.Now,
                        DateTime.Now,
                        orders.ShipVia,
                        orders.Freight,
                        orders.ShipName,
                        orders.ShipAddress,
                        orders.ShipCity,
                        orders.ShipRegion,
                        orders.ShipPostalCode,
                        orders.ShipCountry
                        );
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }

            return RedirectToAction("Index");
        }

        public ActionResult DeleteConfirm(int id)
        {
            var db = new NorthwindEntities();
            return View(db.Orders.Where(c => c.OrderID == id).FirstOrDefault());
        }

        public ActionResult DeleteBegin(string id)
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
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            ShipVia(id);
            CustomerID(id);
            EmployeeID(id);
            var db = new NorthwindEntities();
            return View(db.Orders.Where(c => c.OrderID == id).FirstOrDefault());
        }

        public ActionResult EditDetail(int id)
        {
            var db = new NorthwindEntities();
            return View(db.Order_Details.Where(c => c.OrderID == id).FirstOrDefault());
        }

        public ActionResult SaveOrders(Orders orders)
        {
            var db = new NorthwindEntities();
            try
            {
                int flag = db.Database.ExecuteSqlCommand("UPDATE Orders SET " +
                    "CustomerID=@p0, EmployeeID=@p1, OrderDate=@p2, RequiredDate=@p3, ShippedDate=@p4, ShipVia=@p5," +
                    "Freight=@p6, ShipName=@p7, ShipAddress=@p8, ShipCity=@p9, ShipRegion=@p10, ShipPostalCode=@p11, ShipCountry=@p12 " +
                    "WHERE OrderID=@p13",
                        orders.CustomerID,
                        orders.EmployeeID,
                        DateTime.Now,
                        DateTime.Now,
                        DateTime.Now,
                        orders.ShipVia,
                        orders.Freight,
                        orders.ShipName,
                        orders.ShipAddress,
                        orders.ShipCity,
                        orders.ShipRegion,
                        orders.ShipPostalCode,
                        orders.ShipCountry,
                        orders.OrderID
                        );
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            return RedirectToAction("Index");
        }

        public ActionResult SaveDetail(Order_Details orderDetails)
        {
            var db = new NorthwindEntities();
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    Order_Details dbOrderDetails = db.Order_Details.Single(p => p.OrderID == orderDetails.OrderID);
                    dbOrderDetails = ViewModelToModel(orderDetails, dbOrderDetails);
                    db.Entry(orderDetails).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }

            return RedirectToAction("Edit",  new { id = orderDetails.OrderID });
        }

        public Orders ViewModelToModel(Orders orders, Orders dbOrders)
        {
            dbOrders.CustomerID = orders.CustomerID;
            dbOrders.EmployeeID = orders.EmployeeID;
            dbOrders.OrderDate = orders.OrderDate;
            dbOrders.RequiredDate = orders.RequiredDate;
            dbOrders.ShippedDate = orders.ShippedDate;
            dbOrders.ShipVia = orders.ShipVia;
            dbOrders.Freight = orders.Freight;
            dbOrders.ShipName = orders.ShipName;
            dbOrders.ShipAddress = orders.ShipAddress;
            dbOrders.ShipCity = orders.ShipCity;
            dbOrders.ShipRegion = orders.ShipRegion;
            dbOrders.ShipPostalCode = orders.ShipPostalCode;
            dbOrders.ShipCountry = orders.ShipCountry;

            return dbOrders;
        }

        public Order_Details ViewModelToModel(Order_Details orderDetails, Order_Details dbOrderDetails)
        {
            dbOrderDetails.UnitPrice = orderDetails.UnitPrice;
            dbOrderDetails.Quantity = orderDetails.Quantity;
            dbOrderDetails.Discount = orderDetails.Discount;
            return dbOrderDetails;
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

        public void CustomerID()
        {
            NorthwindEntities db = new NorthwindEntities();

            var CustomerIDList = db.Customers
                    .GroupBy(o => o.CustomerID)
                    .Select(g => g.Key)
                    .ToList();

            List< SelectListItem > items = new List<SelectListItem> ();

            foreach (var item in CustomerIDList)
            {
                items.Add(new SelectListItem() { Text = item.ToString(), Value = item.ToString() });
            }

            ViewBag.CustomerID = items;
        }

        public void CustomerID(int id) {
            NorthwindEntities db = new NorthwindEntities();
            var CustomerID = db.Orders.Where(o => o.OrderID == id).FirstOrDefault().CustomerID;

            var CustomerIDList = db.Customers
                    .GroupBy(o => o.CustomerID)
                    .Select(g => g.Key)
                    .ToList();

            List< SelectListItem > items = new List<SelectListItem> ();

            foreach (var item in CustomerIDList)
            {
                if (CustomerID == item)
                {
                    items.Add(new SelectListItem() { Text = item.ToString(), Value = item.ToString(), Selected = true });

                }
                else
                {
                    items.Add(new SelectListItem() { Text = item.ToString(), Value = item.ToString() });
                }
            }

            ViewBag.CustomerID = items.AsEnumerable();
        }

        public void EmployeeID()
        {
            NorthwindEntities db = new NorthwindEntities();

            var EmployeeIDList = db.Employees
                .GroupBy(o => o.EmployeeID)
                .Select(g => g.Key)
                .OrderBy(employeeID => employeeID)
                .ToList();

            List< SelectListItem > items = new List<SelectListItem> ();

            foreach (var item in EmployeeIDList)
            {
                items.Add(new SelectListItem() { Text = item.ToString(), Value = item.ToString() });
            }

            ViewBag.EmployeeID = items;
        }

        public void EmployeeID(int id)
        {
            NorthwindEntities db = new NorthwindEntities();
            var EmployeeID = db.Orders.Where(o => o.OrderID == id).FirstOrDefault().EmployeeID;

            var EmployeeIDList = db.Employees
                .GroupBy(o => o.EmployeeID)
                .Select(g => g.Key)
                .OrderBy(employeeID => employeeID)
                .ToList();

            List< SelectListItem > items = new List<SelectListItem> ();

            foreach (var item in EmployeeIDList)
            {
                if (EmployeeID == item)
                {
                    items.Add(new SelectListItem() { Text = item.ToString(), Value = item.ToString(), Selected = true });

                }
                else
                {
                    items.Add(new SelectListItem() { Text = item.ToString(), Value = item.ToString() });
                }
            }

            ViewBag.EmployeeID = items;
        }

        public void ShipVia()
        {
            NorthwindEntities db = new NorthwindEntities();

            var ShipViaList = db.Orders
                    .GroupBy(o => o.ShipVia)
                    .Select(g => g.Key)
                    .ToList();

            List< SelectListItem > items = new List<SelectListItem> ();

            foreach (var item in ShipViaList)
            {
                items.Add(new SelectListItem() { Text = item.ToString(), Value = item.ToString() });
            }

            ViewBag.ShipVia = items;
        }

        public void ShipVia(int id)
        {
            NorthwindEntities db = new NorthwindEntities();
            var ShipVia = db.Orders.Where(o => o.OrderID == id).FirstOrDefault().ShipVia;

            var ShipViaList = db.Orders
                    .GroupBy(o => o.ShipVia)
                    .Select(g => g.Key)
                    .ToList();

            List< SelectListItem > items = new List<SelectListItem> ();

            foreach (var item in ShipViaList)
            {
                if (ShipVia == item)
                {
                    items.Add(new SelectListItem() { Text = item.ToString(), Value = item.ToString(), Selected = true });

                }
                else
                {
                    items.Add(new SelectListItem() { Text = item.ToString(), Value = item.ToString()});
                }
            }

            ViewBag.ShipVia = items;
        }
    }
}