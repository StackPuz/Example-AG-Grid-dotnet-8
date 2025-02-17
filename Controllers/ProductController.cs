using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using App.Models;

namespace App.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataContext _context;

        public ProductController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("api/products")]
        public async Task<IActionResult> Index()
        {
            int page = Request.Query["page"].Any() ? Convert.ToInt32(Request.Query["page"]) : 1;
            int size = Request.Query["size"].Any() ? Convert.ToInt32(Request.Query["size"]) : 10;
            int offset = (page - 1) * size;
            string order = Request.Query["order"].Any() ? Request.Query["order"].First() : "Id";
            string direction = Request.Query["direction"].Any() ? Request.Query["direction"].First() : "asc";
            var query = _context.Product.Select(e => new {
                Id = e.Id,
                Name = e.Name,
                Price = e.Price
            });
            if (!String.IsNullOrEmpty(Request.Query["search"])) {
                query = query.Where(e => e.Name.Contains(Request.Query["search"]));
            }
            query = query.OrderBy(order, direction);
            int count = await query.CountAsync();
            var data = await query.Skip(offset).Take(size).ToListAsync();
            return Ok(new { data, count });
        }
    }
}