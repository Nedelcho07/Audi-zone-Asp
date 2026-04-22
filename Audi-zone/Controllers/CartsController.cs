using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Audi_zone.Data;
using Audi_zone.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Audi_zone.Controllers
{
    [Authorize]
    public class CartsController : Controller
        
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Client> _userManager;

        public CartsController(ApplicationDbContext context, UserManager<Client> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Carts
        public async Task<IActionResult> Index()
        {
            var currentUserId = _userManager.GetUserId(User);
            var applicationDbContext = _context.Carts
                .Where(c => c.ClientId == currentUserId)
                .Include(c => c.Clients)
                .Include(c => c.Products);

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Carts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);

            var cart = await _context.Carts
                .Include(c => c.Clients)
                .Include(c => c.Products)
                .FirstOrDefaultAsync(m => m.Id == id && m.ClientId == currentUserId);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Carts/Create
        public IActionResult Create()
        {
            //ViewData["ClientId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,Quantity")] Cart cart)
        {
            cart.DataRegOn = DateTime.Now;
            cart.ClientId = _userManager.GetUserId(User);

            ModelState.Remove(nameof(Cart.ClientId));
            ModelState.Remove(nameof(Cart.Clients));
            ModelState.Remove(nameof(Cart.Products));

            if (cart.Quantity < 1)
            {
                cart.Quantity = 1;
            }

            var existingCartItem = await _context.Carts
                .FirstOrDefaultAsync(c => c.ClientId == cart.ClientId && c.ProductId == cart.ProductId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += cart.Quantity;
                existingCartItem.DataRegOn = DateTime.Now;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //ViewData["ClientId"] = new SelectList(_context.Users, "Id", "Id", cart.ClientId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", cart.ProductId);
            return View(cart);
        }

        // GET: Carts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);

            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.Id == id && c.ClientId == currentUserId);
            if (cart == null)
            {
                return NotFound();
            }
            ViewData["ClientId"] = new SelectList(_context.Users, "Id", "Id", cart.ClientId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", cart.ProductId);
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClientId,ProductId,Quantity,DataRegOn")] Cart cart)
        {
            if (id != cart.Id)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);
            if (cart.ClientId != currentUserId)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Users, "Id", "Id", cart.ClientId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", cart.ProductId);
            return View(cart);
        }

        // GET: Carts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);

            var cart = await _context.Carts
                .Include(c => c.Clients)
                .Include(c => c.Products)
                .FirstOrDefaultAsync(m => m.Id == id && m.ClientId == currentUserId);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentUserId = _userManager.GetUserId(User);
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.Id == id && c.ClientId == currentUserId);
            if (cart != null)
            {
                _context.Carts.Remove(cart);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder()
        {
            var currentUserId = _userManager.GetUserId(User);

            var cartItems = await _context.Carts
                .Where(c => c.ClientId == currentUserId)
                .Include(c => c.Products)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["OrderMessage"] = "Количката е празна. Добави продукти преди поръчка.";
                return RedirectToAction(nameof(Index));
            }
            TempData["OrderMessage"] = "Поръчката е приета успешно!";

            _context.Carts.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(int id)
        {
            return _context.Carts.Any(e => e.Id == id);
        }
    }
}
