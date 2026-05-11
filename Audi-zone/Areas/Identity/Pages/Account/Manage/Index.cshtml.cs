using System.ComponentModel.DataAnnotations;
using Audi_zone.Data;
using Audi_zone.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Audi_zone.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<Client> _signInManager;
        private readonly UserManager<Client> _userManager;

        public IndexModel(
            ApplicationDbContext context,
            SignInManager<Client> signInManager,
            UserManager<Client> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public int CartItemsCount { get; set; }

        public int OrdersCount { get; set; }

        public decimal OrdersTotal { get; set; }

        [TempData]
        public string? StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required(ErrorMessage = "Въведи име.")]
            [Display(Name = "Име")]
            public string FirstName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Въведи фамилия.")]
            [Display(Name = "Фамилия")]
            public string LastName { get; set; } = string.Empty;

            [Phone(ErrorMessage = "Въведи валиден телефонен номер.")]
            [Display(Name = "Телефон")]
            public string? PhoneNumber { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Потребителят не е намерен.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Потребителят не е намерен.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            user.FirstName = Input.FirstName.Trim();
            user.LastName = Input.LastName.Trim();
            user.PhoneNumber = Input.PhoneNumber?.Trim();

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                await LoadAsync(user);
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Профилът е обновен успешно.";
            return RedirectToPage();
        }

        private async Task LoadAsync(Client user)
        {
            var userId = await _userManager.GetUserIdAsync(user);

            Username = await _userManager.GetUserNameAsync(user) ?? string.Empty;
            Email = await _userManager.GetEmailAsync(user) ?? string.Empty;
            Input = new InputModel
            {
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                PhoneNumber = await _userManager.GetPhoneNumberAsync(user)
            };

            CartItemsCount = await _context.Carts
                .Where(c => c.ClientId == userId)
                .SumAsync(c => (int?)c.Quantity) ?? 0;

            OrdersCount = await _context.Orders
                .Where(o => o.ClientId == userId)
                .CountAsync();

            OrdersTotal = await _context.Orders
                .Where(o => o.ClientId == userId)
                .SumAsync(o => (decimal?)o.TotalPrice) ?? 0;
        }
    }
}
