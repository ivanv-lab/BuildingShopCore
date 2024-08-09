using BuildingShopCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace BuildingShopCore.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly BuildingShopContext _context;
        public EmployeeController(BuildingShopContext context)=>
            _context = context;
        
        public IActionResult Index()=>
             View();

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest("Данного id не существует");
            Employee employee = await _context.Employees.FindAsync(id);
            if(employee == null)
                return NotFound("Данного сотрудника не существует");
            return View(employee);
        }

        public IActionResult Create()=>
            View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(include:"Id,FIO,Address,Phone")]Employee employee)
        {
            if (ModelState.IsValid)
            {
                await _context.Employees.AddAsync(employee);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("Notification", $"Сотрудник №{employee.Id} успешно создан");
                return RedirectToAction("Index");
            }
            return View(employee);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return BadRequest("Данного id не существует");
            Employee employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound("Данного сотрудника не существует");
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind(include: "Id,FIO,Address,Phone")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(employee).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("Notification", $"Сотрудник №{employee.Id.ToString()} успешно изменен");
                return RedirectToAction("Index");
            }
            return View(employee);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest("Данного id не существует");
            Employee employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound("Данного сотрудника не существует");
            return View(employee);
        }

        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Employee employee = await _context.Employees.FindAsync(id);
            employee.IsDeleted = true;
            await _context.SaveChangesAsync();
            HttpContext.Session.SetString("Notification", $"Сотрудник №{id.ToString()} успешно удален");
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
                _context.Dispose();
            base.Dispose(disposing);
        }

        public async Task<PartialViewResult> EmployeePartialView(string sortOrder, string currentFilter,
            string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.IdSortParm = String.IsNullOrEmpty(sortOrder) ? "Id_desc" : "";
            ViewBag.AddressSortParm = sortOrder == "Address" ? "Address_desc" : "Address";
            ViewBag.NameSortParm = sortOrder == "FIO" ? "FIO_desc" : "FIO";

            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentFilter = searchString;

            var employees = await _context.Employees.ToListAsync();
            if (!String.IsNullOrEmpty(searchString))
            {
                employees=employees.Where(e=>e.FIO.Contains(searchString)
                || e.Address.Contains(searchString)
                || e.Phone.Contains(searchString)
                || e.Id.ToString().Contains(searchString)
                && e.IsDeleted==false).ToList();
            }

            switch (sortOrder)
            {
                case "Id_desc":
                    employees = employees.OrderByDescending(c => c.Id).ToList();
                    break;
                case "Address":
                    employees = employees.OrderBy(c => c.Address).ToList();
                    break;
                case "Address_desc":
                    employees = employees.OrderByDescending(c => c.Address).ToList();
                    break;
                case "FIO":
                    employees = employees.OrderBy(c => c.FIO).ToList();
                    break;
                case "FIO_desc":
                    employees = employees.OrderByDescending(c => c.FIO).ToList();
                    break;
                default:
                    employees = employees.OrderBy(c => c.Id).ToList();
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return PartialView(("_EmployeePartiallayout"), employees.ToPagedList(pageNumber, pageSize));
        }
    }
}
