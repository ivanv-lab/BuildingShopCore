using BuildingShopCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace BuildingShopCore.Controllers
{
    public class OrderController : Controller
    {
        private readonly BuildingShopContext _context;

        public OrderController(BuildingShopContext context)=>
            _context = context;
        
        public IActionResult Index()=>
             View();
        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest("Данного id не существует");
            Order order=await _context.Orders.FindAsync(id);
            if(order == null)
                return NotFound();
            return View(order);
        }

        public IActionResult Create()
        {
            ViewBag.ClientId = new SelectList(_context.Clients, "Id", "FIO");
            ViewBag.EmployeeId = new SelectList(_context.Employees, "Id", "FIO");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(include:"Id,ClientId," +
            "EmployeeId,Ready,Date,Sum")]Order order)
        {
            if (ModelState.IsValid)
            {
                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("Notification", $"Заказ №{order.Id.ToString()} успешно создан");
                return RedirectToAction("Index");
            }
            ViewBag.ClientId = new SelectList(_context.Clients, "Id", "FIO");
            ViewBag.EployeeId = new SelectList(_context.Employees, "Id", "FIO");
            return View(order);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return BadRequest();
            Order order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();
            ViewBag.ClientId = new SelectList(_context.Clients, "Id", "FIO");
            ViewBag.EployeeId = new SelectList(_context.Employees, "Id", "FIO");
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind(include:"Id,ClientId," +
            "EmployeeId,Ready,Date,Sum")]Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(order).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("Notification", $"Заказ №{order.Id.ToString()} успешно изменен");
            }
            ViewBag.ClientId = new SelectList(_context.Clients, "Id", "FIO");
            ViewBag.EployeeId = new SelectList(_context.Employees, "Id", "FIO");
            return View(order);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest();
            Order order = await _context.Orders.FindAsync(id);
            if (order == null) 
                return NotFound();
            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Order order = await _context.Orders.FindAsync(id);
            order.IsDeleted=true;
            await _context.SaveChangesAsync();
            HttpContext.Session.SetString("Notification", $"Заказ №{id.ToString()} успешно удален");
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _context.Dispose();
            base.Dispose(disposing);
        }

        public async Task<PartialViewResult> OrderPartilView(string sortOrder,
            string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.DoneSortParm = String.IsNullOrEmpty(sortOrder) ? "Done_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.SumSortParm = sortOrder == "Sum" ? "sum_desc" : "Sum";
            ViewBag.ClientSortParm = sortOrder == "Client" ? "client_desc" : "Client";
            ViewBag.EmployeeSortParm = sortOrder == "Empl" ? "empl_desc" : "Empl";
            ViewBag.IdSortParm = sortOrder == "Id" ? "id_desc" : "Id";

            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;
            ViewBag.CurrentFilter = searchString;

            var orders = await _context.Orders.ToListAsync();

            if (!String.IsNullOrEmpty(searchString))
            {
                orders = orders.Where(o => o.Date.ToString("dd.MM.yyyy").Contains(searchString)
                || o.Client.FIO.Contains(searchString)
                || o.Employee.FIO.Contains(searchString)
                || o.Sum.ToString().Contains(searchString)
                || o.Id.ToString().Contains(searchString)
                && o.IsDeleted==false).ToList();
            }

            switch (sortOrder)
            {
                case "Done_desc":
                    orders = orders.OrderByDescending(o => o.Ready).ToList();
                    break;
                case "Date":
                    orders = orders.OrderBy(o => o.Date).ToList();
                    break;
                case "date_desc":
                    orders = orders.OrderByDescending(o => o.Date).ToList();
                    break;
                case "Sum":
                    orders = orders.OrderBy(o => o.Sum).ToList();
                    break;
                case "sum_desc":
                    orders = orders.OrderByDescending(o => o.Sum).ToList();
                    break;
                case "Client":
                    orders = orders.OrderBy(o => o.Client.FIO).ToList();
                    break;
                case "client_desc":
                    orders = orders.OrderByDescending(o => o.Client.FIO).ToList();
                    break;
                case "Empl":
                    orders = orders.OrderBy(o => o.Employee.FIO).ToList();
                    break;
                case "empl_desc":
                    orders = orders.OrderByDescending(o => o.Employee.FIO).ToList();
                    break;
                case "Id":
                    orders = orders.OrderBy(o => o.Id).ToList();
                    break;
                case "id_desc":
                    orders = orders.OrderByDescending(o => o.Id).ToList();
                    break;
                default:
                    orders = orders.OrderBy(o => o.Ready).ToList();
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return PartialView(("OrderPartialLayout"), orders.ToPagedList(pageNumber, pageSize));
        }
    }
}
