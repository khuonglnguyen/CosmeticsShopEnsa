using CosmeticsShop.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;

namespace CosmeticsShop.Controllers
{
    public class StatisticController : Controller
    {
        ShoppingEntities db = new ShoppingEntities();
        public bool CheckRole(string type)
        {
            Models.User user = Session["User"] as Models.User;
            if (user != null && user.UserType.Name == type)
            {
                return true;
            }
            return false;
        }
        // GET: Statistic
        public ActionResult Order(DateTime from, DateTime to)
        {
            if (CheckRole("Admin"))
            {

            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
            List<OrderDetail> orderDetails = db.OrderDetails.Where(x => DbFunctions.TruncateTime(x.Order.DateShip) >= from.Date && DbFunctions.TruncateTime(x.Order.DateShip) <= to.Date).ToList();

            List<int> OrderIDs = new List<int>();
            foreach (var item in orderDetails)
            {
                OrderIDs.Add(item.OrderID.Value);
            }
            List<Order> orders = new List<Order>();
            if (OrderIDs.Count() > 0)
            {
                orders = db.Orders.Where(x => x.Status == "Complete" && OrderIDs.Contains(x.ID)).ToList();
            }
            ViewBag.from = from;
            ViewBag.to = to;
            return View(orders);
        }

        public ActionResult Product()
        {
            if (CheckRole("Admin"))
            {

            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
            List<Product> products = db.Products.Where(x=>x.PurchasedCount > 0).OrderByDescending(x=>x.PurchasedCount).ToList();
            return View(products);
        }

        public ActionResult SalesRevenue(DateTime from, DateTime to)
        {
            if (CheckRole("Admin"))
            {

            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
            List<OrderDetail> orderDetails = db.OrderDetails.Where(x => DbFunctions.TruncateTime(x.Order.DateShip) >= from.Date && DbFunctions.TruncateTime(x.Order.DateShip) <= to.Date).ToList();

            List<int> OrderIDs = new List<int>();
            foreach (var item in orderDetails)
            {
                OrderIDs.Add(item.OrderID.Value);
            }
            List<Order> orders = new List<Order>();
            if (OrderIDs.Count() > 0)
            {
                orders = db.Orders.Where(x => x.Status == "Complete" && OrderIDs.Contains(x.ID)).ToList();
            }
            ViewBag.from = from;
            ViewBag.to = to;
            return View(orders);
        }

        [HttpGet]
        public void DownloadExcelStatisticOrder(DateTime from, DateTime to)
        {
            User user = Session["User"] as User;
            List<OrderDetail> orderDetails = db.OrderDetails.Where(x => DbFunctions.TruncateTime(x.Order.DateShip) >= from.Date && DbFunctions.TruncateTime(x.Order.DateShip) <= to.Date).ToList();

            List<int> OrderIDs = new List<int>();
            foreach (var item in orderDetails)
            {
                OrderIDs.Add(item.OrderID.Value);
            }
            List<Order> orders = new List<Order>();
            if (OrderIDs.Count() > 0)
            {
                orders = db.Orders.Where(x => x.Status == "Complete" && OrderIDs.Contains(x.ID)).ToList();
            }

            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Report");

            ws.Cells["A2"].Value = "Export by";
            ws.Cells["B2"].Value = user.Name;

            ws.Cells["A3"].Value = "Date";
            ws.Cells["B3"].Value = DateTime.Now.ToShortDateString();

            ws.Cells["A6"].Value = "Order ID";
            ws.Cells["B6"].Value = "Customer";
            ws.Cells["C6"].Value = "Order date";
            ws.Cells["D6"].Value = "Delivery date";
            ws.Cells["E6"].Value = "Status";
            ws.Cells["F6"].Value = "Total";

            int rowStart = 7;
            foreach (var item in orders)
            {
                ws.Cells[string.Format("A{0}", rowStart)].Value = item.ID;
                ws.Cells[string.Format("B{0}", rowStart)].Value = item.User.Name;
                ws.Cells[string.Format("C{0}", rowStart)].Value = item.DateOrder.Value.ToString("dd/MM/yyyy");
                ws.Cells[string.Format("D{0}", rowStart)].Value = item.DateShip.Value.ToString("dd/MM/yyyy");
                ws.Cells[string.Format("E{0}", rowStart)].Value = "Complete";
                ws.Cells[string.Format("F{0}", rowStart)].Value = "$" + item.OrderDetails.Sum(x => x.ProductPrice * x.Quantity);
                rowStart++;
            }

            ws.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "Orderlist.xlsx");
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }
        [HttpGet]
        public void DownloadExcelStatisticProduct()
        {
            User user = Session["User"] as User;
            List<Product> products = db.Products.Where(x => x.PurchasedCount > 0).OrderByDescending(x => x.PurchasedCount).ToList();
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Report");

            ws.Cells["A2"].Value = "Export by";
            ws.Cells["B2"].Value = user.Name;

            ws.Cells["A3"].Value = "Date";
            ws.Cells["B3"].Value = DateTime.Now.ToShortDateString();

            ws.Cells["A6"].Value = "Product ID";
            ws.Cells["B6"].Value = "Name";
            ws.Cells["C6"].Value = "Purchased Count";

            int rowStart = 7;
            foreach (var item in products)
            {
                ws.Cells[string.Format("A{0}", rowStart)].Value = item.ID;
                ws.Cells[string.Format("B{0}", rowStart)].Value = item.Name;
                ws.Cells[string.Format("C{0}", rowStart)].Value = item.PurchasedCount;
                rowStart++;
            }

            ws.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "Product.xlsx");
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }
    }
}