using BuildingShopCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace BuildingShopCore.Controllers
{
    public class ProductCategoryController : Controller
    {
        private readonly BuildingShopContext _context;

        public ProductCategoryController(BuildingShopContext context)=>
            _context = context;

        public IActionResult Index() =>
            View();

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return BadRequest();
            ProductCategory productCategory = await _context.ProductCategories.FindAsync(id);
            if (productCategory == null) return NotFound();
            return View(productCategory);
        }

        public IActionResult Create() =>
            View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(include:"Id,Name")]ProductCategory productCategory)
        {
            if (ModelState.IsValid)
            {
                await _context.ProductCategories.AddAsync(productCategory);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("Notification",
                    $"Категория товаров №{productCategory.Id.ToString()} успешно создана");
                return RedirectToAction("Index");
            }
            return View(productCategory);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if(id==null) return BadRequest();
            ProductCategory productCategory = await _context.ProductCategories.FindAsync(id);
            if (productCategory == null) return NotFound();
            return View(productCategory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind(include: "Id,Name")] ProductCategory productCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(productCategory).State=EntityState.Modified;
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("Notification", $"Категория товаров №{productCategory.Id} успешно изменена");
                return RedirectToAction("Index");
            }
            return View(productCategory);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id==null) return BadRequest();
            ProductCategory productCategory = await _context.ProductCategories.FindAsync(id);
            if (productCategory == null) return NotFound();
            return View(productCategory);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ProductCategory productCategory = await _context.ProductCategories.FindAsync(id);
            productCategory.IsDeleted = true;
            _context.Entry(productCategory).State= EntityState.Modified;
            await _context.SaveChangesAsync();
            HttpContext.Session.SetString("Notification", $"Категория товаров №{id.ToString()} успешно удалена");
            return RedirectToAction("Index");   
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _context.Dispose();
            base.Dispose(disposing);
        }

        public async Task<PartialViewResult> CategoryPartialView(string sortOrder,
            string currentFilter, string searchString, int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = sortOrder == "Name" ? "Name_desc" : "Name";
            ViewData["IdSortParm"] = String.IsNullOrEmpty(sortOrder) ? "Id_desc" : "";

            if (searchString != null) page = 1;
            else searchString = currentFilter;

            ViewData["CurrentFilter"] = searchString;

            var categories =await _context.ProductCategories.ToListAsync();
            if (!String.IsNullOrEmpty(searchString))
            {
                categories = categories
                    .Where(c => c.Name.Contains(searchString)
                    || c.Id.ToString().Contains(searchString)
                    && c.IsDeleted == false).ToList();
            }

            switch (sortOrder)
            {
                case "Name":
                    categories = categories.OrderBy(c => c.Name).ToList();
                    break;
                case "Name_desc":
                    categories = categories.OrderByDescending(c => c.Name).ToList();
                    break;
                case "Id_desc":
                    categories = categories.OrderByDescending(c => c.Id).ToList();
                    break;
                default:
                    categories = categories.OrderBy(c => c.Id).ToList();
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return PartialView(("_CategoryPartialView"), categories.ToPagedList(pageNumber, pageSize));
        }

        //public async Task<IActionResult> CategoryPartialView()
        //{
        //    var categories=await _context.ProductCategories.ToArrayAsync();
        //    return PartialView(categories);
        //}
    }
}
