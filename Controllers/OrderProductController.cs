using BuildingShopCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BuildingShopCore.Controllers
{
    public class OrderProductController : Controller
    {
        private readonly BuildingShopContext _context;

        public OrderProductController(BuildingShopContext context)=>
            _context = context;
        
        public async Task<IActionResult> Index()
        {
            var productList =await _context.OrderProducts
                .Include(p => p.Order).Include(p => p.Product).ToListAsync();
            return View(productList);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest();
            var orderProduct =await _context.OrderProducts.FindAsync(id);
            if(orderProduct==null)
                return NotFound();
            return View(orderProduct);
        }

        public IActionResult Create(int id)
        {
            ViewBag.OrderId = id;
            ViewBag.ProductId = new SelectList(_context.Products, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(include:"Id,ProductId,OrderId,Count,Price")] OrderProduct orderProduct,int id)
        {
            orderProduct.OrderId = id;
            if (ModelState.IsValid)
            {
                await _context.OrderProducts.AddAsync(orderProduct);
                await _context.SaveChangesAsync();
                Order order= await _context.Orders.FindAsync(id);
                var sum = _context.OrderProducts
                    .Where(s => s.OrderId == order.Id)
                    .Sum(s => s.Count * s.Price);
                order.Sum = sum;
                _context.Entry(order).State= EntityState.Modified;
                await _context.SaveChangesAsync();
                Product product = await _context.Products.FindAsync(orderProduct.ProductId);
                int productCount = product.Count - orderProduct.Count;
                product.Count = productCount;
                _context.Entry(product).State= EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToAction("../Order/Details", new { id = id });
            }
            ViewBag.ProductId = new SelectList(_context.Products, "Id", "Name", orderProduct.ProductId);
            return View(orderProduct);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
               return BadRequest();
            var orderProduct = await _context.OrderProducts.FindAsync(id);
            if (orderProduct == null)
               return NotFound();
            ViewBag.OrderId = new SelectList(_context.Orders, "Id", "Id", orderProduct.OrderId);
            ViewBag.ProductId=new SelectList(_context.Products,"Id","Name",orderProduct.ProductId);
            return View(orderProduct);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind(include:"Id,ProductId,OrderId,Count,Price")] OrderProduct orderProduct)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(orderProduct).State= EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.OrderId=new SelectList(_context.Orders,"Id", "Id", orderProduct.OrderId);
            ViewBag.ProductId = new SelectList(_context.Products, "Id", "Name", orderProduct.ProductId);
            return View(orderProduct);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null) return BadRequest();
            var orderProduct = await _context.OrderProducts.FindAsync(id);
            if (orderProduct == null) return NotFound();
            return View(orderProduct);
        }

        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            OrderProduct orderProduct = await _context.OrderProducts.FindAsync(id);
            orderProduct.IsDeleted = true;
            int orderId= orderProduct.OrderId;
            _context.Entry(orderProduct).State = EntityState.Modified;
            Order order = await _context.Orders.FindAsync(orderId);
            var sum = _context.OrderProducts
                .Where(s => s.OrderId == orderProduct.OrderId)
                .Sum(s => s.Count * s.Price);
            order.Sum = sum;
            _context.Entry(order).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            HttpContext.Session.SetString("Notification", "Товар успешно удален из заказа");
            return RedirectToAction("../Order/Details", new {id=orderId});
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _context.Dispose();
            base.Dispose(disposing);
        }

        [HttpPost]
        public async Task<IActionResult> GetProductInfo(int ProductId)
        {
            var product=await _context.Products
                .FirstOrDefaultAsync(p=>p.Id== ProductId);
            if (product == null)
                return NotFound();
            var price=product.Price;
            var image = Url.Content($"~/ProductImages/{ProductId}.jpg");
            var count = product.Count;


            //var price=_context.Products
            //    .Where(p=>p.Id==ProductId)
            //    .First().Price;
            //var image = Url.Content($"~/ProductImages/{ProductId}.jpg");
            //var count = _context.Products
            //    .Where(p => p.Id == ProductId)
            //    .First().Count;
            return Json(new {price, image, count});
        }
    }
}
