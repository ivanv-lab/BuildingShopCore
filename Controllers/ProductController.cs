using BuildingShopCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace BuildingShopCore.Controllers
{
    public class ProductController : Controller
    {
        private readonly BuildingShopContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProductController(BuildingShopContext context, IWebHostEnvironment environment){
            _context = context;
            _environment = environment;
        }
            

        public IActionResult Index()=>
             View();

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return BadRequest();
            Product product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        public IActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(_context.ProductCategories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(include: "Id,Name,Description,CategoryId,Price,Count,CountryProd,Prod")] 
        Product product,IFormFile image)
        {
            if (ModelState.IsValid)
            {
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();
                if(image != null && image.Length > 0)
                {
                    var fileName=product.Id.ToString()+Path.GetExtension(image.FileName);
                    var path = Path.Combine(_environment.WebRootPath, "ProductImages", fileName);
                    using(var stream=new FileStream(path,FileMode.Create))
                        await image.CopyToAsync(stream);
                }
                HttpContext.Session.SetString("Notification", $"Товар №{product.Id.ToString()} успешно создан");
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(_context.ProductCategories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        [OutputCache(NoStore =true,Duration =0)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return BadRequest();
            Product product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            ViewBag.CategoryId = new SelectList(_context.ProductCategories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind(include: "Id,Name,Description,CategoryId,Price,Count,CountryProd,Prod")] Product product,
            IFormFile image)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(product).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                if (image != null && image.Length>0) 
                {
                    var filename = product.Id.ToString() + Path.GetExtension(image.FileName);
                    var path = Path.Combine(_environment.WebRootPath, "ProductImages", filename);
                    using (var stream = new FileStream(path, FileMode.Create))
                        await image.CopyToAsync(stream);
                }
                HttpContext.Session.SetString("Notification", $"Товар №{product.Id.ToString()} успешно изменен");
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId=new SelectList(_context.ProductCategories,"Id", "Name", product.CategoryId);
            return View(product);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();
            Product product = await _context.Products.FindAsync(id);
            if(product == null) return NotFound();
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Product product = await _context.Products.FindAsync(id);
            product.IsDeleted = true;
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            HttpContext.Session.SetString("Notification", $"Товар №{id.ToString()} успешно удален");
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _context.Dispose();
            base.Dispose(disposing);
        }

        public async Task<IActionResult> ProductpartialView(string sortOrder, string currentFilter, 
            string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.IdSortParm = String.IsNullOrEmpty(sortOrder) ? "Id_desc" : "";
            ViewBag.NameSortParm = sortOrder == "Name" ? "Name_desc" : "Name";
            ViewBag.DescSortParm = sortOrder == "Desc" ? "Desc_desc" : "Desc";
            ViewBag.CatSortParm = sortOrder == "Cat" ? "Cat_desc" : "Cat";
            ViewBag.PriceSortParm = sortOrder == "Price" ? "Price_desc" : "Price";
            ViewBag.CountrySortParm = sortOrder == "Country" ? "Country_desc" : "Country";
            ViewBag.ProdSortParm = sortOrder == "Prod" ? "Prod_desc" : "Prod";
            ViewBag.CountSortParm = sortOrder == "Count" ? "Count_desc" : "Count";

            if (searchString != null) page = 1;
            else searchString = currentFilter;
            ViewBag.CurrentFilter = searchString;

            var products=await _context.Products.ToListAsync();
            if (!String.IsNullOrEmpty(searchString))
            {
                products=products
                    .Where(p=>p.Id.ToString().Contains(searchString)
                    || p.Name.Contains(searchString)
                    || p.Description.Contains(searchString)
                    || p.CategoryId.ToString().Contains(searchString)
                    || p.Price.ToString().Contains(searchString)
                    || p.CountryProd.Contains(searchString)
                    || p.Prod.Contains(searchString)
                    || p.Count.ToString().Contains(searchString)
                    && p.IsDeleted==false).ToList();
            }

            switch (sortOrder)
            {
                case "Id_desc":
                    products = products.OrderByDescending(p => p.Id).ToList();
                    break;
                case "Desc":
                    products = products.OrderBy(p => p.Description).ToList();
                    break;
                case "Desc_desc":
                    products = products.OrderByDescending(p => p.Description).ToList();
                    break;
                case "Price":
                    products = products.OrderBy(p => p.Price).ToList();
                    break;
                case "Price_desc":
                    products = products.OrderByDescending(p => p.Price).ToList();
                    break;
                case "Country":
                    products = products.OrderBy(p => p.CountryProd).ToList();
                    break;
                case "Country_desc":
                    products = products.OrderByDescending(p => p.CountryProd).ToList();
                    break;
                case "Cat":
                    products = products.OrderBy(p => p.Category.Name).ToList();
                    break;
                case "Cat_desc":
                    products = products.OrderByDescending(p => p.Category.Name).ToList();
                    break;
                case "Prod":
                    products = products.OrderBy(p => p.Prod).ToList();
                    break;
                case "Prod_desc":
                    products = products.OrderByDescending(p => p.Prod).ToList();
                    break;
                case "Count":
                    products = products.OrderBy(p => p.Count).ToList();
                    break;
                case "Count_desc":
                    products = products.OrderByDescending(p => p.Count).ToList();
                    break;
                default:
                    products = products.OrderBy(p => p.Id).ToList();
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return PartialView(("_ProductPartialLayout"), products.ToPagedList(pageNumber, pageSize));
        }
    }
}
