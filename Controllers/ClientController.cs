using BuildingShopCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace BuildingShopCore.Controllers
{
    public class ClientController : Controller
    {
        private readonly BuildingShopContext _context;
        public ClientController(BuildingShopContext context)=>
            _context = context;

        //public IActionResult Index()=>
        //     View();

        public async Task<IActionResult> Index(string sortOrder, 
            string currentFilter, string searchString, int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["IdSortParm"] = String.IsNullOrEmpty(sortOrder) ? "Id_desc" : "";
            ViewData["AddressSortParm"]= sortOrder == "Address" ? "Address_desc" : "Address";
            ViewData["NameSortParm"] = sortOrder == "FIO" ? "FIO_desc" : "FIO";

            if (searchString != null) page = 1;
            else searchString = currentFilter;

            ViewData["CurrentFilter"] = searchString;

            var clients = await _context.Clients
                .Where(c=>c.IsDeleted==false).ToListAsync();
            if (!String.IsNullOrEmpty(searchString))
            {
                clients = clients.Where(e => e.FIO.Contains(searchString)
                || e.Address.Contains(searchString)
                || e.Phone.Contains(searchString)
                || e.Id.ToString().Contains(searchString)
                && e.IsDeleted == false).ToList();
            }

            switch (sortOrder)
            {
                case "Id_desc":
                    clients = clients.OrderByDescending(c => c.Id).ToList();
                    break;
                case "Address":
                    clients = clients.OrderBy(c => c.Address).ToList();
                    break;
                case "Address_desc":
                    clients = clients.OrderByDescending(c => c.Address).ToList();
                    break;
                case "FIO":
                    clients = clients.OrderBy(c => c.FIO).ToList();
                    break;
                case "FIO_desc":
                    clients = clients.OrderByDescending(c => c.FIO).ToList();
                    break;
                default:
                    clients = clients.OrderBy(c => c.Id).ToList();
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(clients.ToPagedList(pageNumber, pageSize));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest("Данного id не существует");
            Client client=await _context.Clients.FindAsync(id);
            if (client == null)
                return NotFound("Данный клиент остутствует");
            return View(client);
        }

        public IActionResult Create()=>
            View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(include:"Id,FIO,Address,Phone")] Client client)
        {
            if (ModelState.IsValid)
            {
                await _context.Clients.AddAsync(client);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("Notification", $"Клиент №{client.Id.ToString()} успешно создан");
                return RedirectToAction("Index");
            }
            return View(client);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return BadRequest("Данного id не существует");
            Client client = await _context.Clients.FindAsync(id);
            if (client == null)
                return NotFound("Данный клиент отсутствует");
            return View(client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind(include: "Id,FIO,Address,Phone")] Client client)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(client).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("Notification", $"Клиент №{client.Id.ToString()} успешно изменен");
                return RedirectToAction("Index");
            }
            return View(client);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest("Данного id не существует");
            Client client = await _context.Clients.FindAsync(id);
            if (client == null)
                return NotFound("Данный клиент отсутствует");
            return View(client);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletedConfirmed(int id)
        {
            Client client = await _context.Clients.FindAsync(id);
            _context.Entry(client).State= EntityState.Modified;
            client.IsDeleted = true;
            await _context.SaveChangesAsync();
            HttpContext.Session.SetString("Notification", $"Клиент №{id} успешно удален");
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
                _context.Dispose();
            base.Dispose(disposing);
        }

        public async Task<IActionResult> ClientPartialView(
            string sortOrder,string currentFilter, string searchString, int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["IdSortParm"] = String.IsNullOrEmpty(sortOrder) ? "Id_desc" : "";
            ViewData["AddressSortParm"] = sortOrder == "Address" ? "Address_desc" : "Address";
            ViewData["NameSortParm"] = sortOrder == "FIO" ? "FIO_desc" : "FIO";

            if (searchString != null) page = 1;
            else searchString = currentFilter;

            ViewData["CurrentFilter"] = searchString;

            var clients = await _context.Clients
                .Where(c => c.IsDeleted == false).ToListAsync();
            if (!String.IsNullOrEmpty(searchString))
            {
                clients = clients.Where(e => e.FIO.Contains(searchString)
                || e.Address.Contains(searchString)
                || e.Phone.Contains(searchString)
                || e.Id.ToString().Contains(searchString)
                && e.IsDeleted == false).ToList();
            }

            switch (sortOrder)
            {
                case "Id_desc":
                    clients = clients.OrderByDescending(c => c.Id).ToList();
                    break;
                case "Address":
                    clients = clients.OrderBy(c => c.Address).ToList();
                    break;
                case "Address_desc":
                    clients = clients.OrderByDescending(c => c.Address).ToList();
                    break;
                case "FIO":
                    clients = clients.OrderBy(c => c.FIO).ToList();
                    break;
                case "FIO_desc":
                    clients = clients.OrderByDescending(c => c.FIO).ToList();
                    break;
                default:
                    clients = clients.OrderBy(c => c.Id).ToList();
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return PartialView(("_ClientPartialView"),clients.ToPagedList(pageNumber, pageSize));
        }
    }
}
