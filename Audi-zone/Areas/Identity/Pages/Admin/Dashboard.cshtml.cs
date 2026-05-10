using Audi_zone.Data;
using Audi_zone.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Audi_zone.Areas.Identity.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DashboardModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public int TotalProducts { get; set; }
        public int TotalModels { get; set; }
        public int TotalProductTypes { get; set; }
        public int TotalCarts { get; set; }
        public int TotalClients { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalCartValue { get; set; }
        public decimal TotalOrdersValue { get; set; }

        public List<Product> RecentProducts { get; set; } = new();
        public List<Order> RecentOrders { get; set; } = new();
        public List<Model> AllModels { get; set; } = new();
        public List<ProductType> AllProductTypes { get; set; } = new();

        public void OnGet()
        {
            TotalProducts = _context.Products.Count();
            TotalModels = _context.Models.Count();
            TotalProductTypes = _context.ProductTypes.Count();
            TotalCarts = _context.Carts.Count();
            TotalClients = _context.Users.Count();
            TotalOrders = _context.Orders.Count();
            TotalCartValue = _context.Carts
                .Join(_context.Products,
                    c => c.ProductId,
                    p => p.Id,
                    (c, p) => c.Quantity * p.Price)
                .Sum();
            TotalOrdersValue = _context.Orders
                .Sum(o => (decimal?)o.TotalPrice) ?? 0;

            RecentProducts = _context.Products
                .Include(p => p.Models)
                .Include(p => p.ProductTypes)
                .OrderByDescending(p => p.DateRegOn)
                .Take(5)
                .ToList();

            RecentOrders = _context.Orders
                .Include(o => o.Client)
                .OrderByDescending(o => o.OrderedOn)
                .ToList();

            AllModels = _context.Models.ToList();
            AllProductTypes = _context.ProductTypes.ToList();
        }
    }
}
