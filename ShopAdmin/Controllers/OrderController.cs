﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAdmin.Data;

namespace ShopAdmin.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ProductDbContext _dbContext;

        public OrderController(ProductDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IActionResult> Index()
        {
            var orders = await _dbContext.Orders.Include(o => o.Carts).ThenInclude(c => c.Product).ToListAsync();
            return View(orders);
        }

        public async Task<IActionResult> Details(int? id)
        {
            //var order = await _dbContext.Orders.Include(p => p.Carts).ThenInclude(c => c.Product).ThenInclude(f => f.Category).FirstOrDefaultAsync(o => o.Id == id);
            var order = await _dbContext.Orders
                .Include("Carts.Product.Category")
                .Include("Carts.Product.Brand")
                .FirstOrDefaultAsync(o => o.Id == id);
            return View(order);
        }
        public async Task<IActionResult> CancelOrder(int? id)
        {
            var order = await _dbContext.Orders.FirstOrDefaultAsync(u => u.Id == id);
            if (order != null)
            {
                order.OrderStatus = -1;
            }
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> SuccessOrder(int? id)
        {
            var order = await _dbContext.Orders.FirstOrDefaultAsync(u => u.Id == id);
            if (order != null)
            {
                order.OrderStatus = 1;
            }
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
