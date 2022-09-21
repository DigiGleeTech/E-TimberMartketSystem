using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rocky.Data;
using Rocky.Models;
using Rocky.Models.ViewModels;
using Rocky.Utility;

namespace Rocky.Controllers
{
    [Authorize(WC.SuperAdminRole, Policy = "Order")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> userManager;

        public OrdersController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            this.userManager = userManager;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var userId = userManager.GetUserId(User);
            //ViewBag.userNamw = userManager.GetUserName(User).ToUpper();
            ViewBag.User = _context.ApplicationUser.Where(x => x.Id == userId).Select(u=>u.FullName).FirstOrDefault();
            var shipOrder = _context.shipOrders.Include(s => s.Order).Where(x=>x.UserId == x.UserId);
            var AdminShipOrder = _context.shipOrders.Include(s => s.Order);
            if (User.IsInRole(WC.AdminRole) || User.IsInRole(WC.SuperAdminRole))
            {
                return View(await AdminShipOrder.ToListAsync());
            }else if (User.IsInRole(WC.CustomerRole))
            {
                return View(await shipOrder.ToListAsync());
            }
            return RedirectToAction(nameof(statusCode));
        }

        public StatusCodeResult statusCode()
        {  
           
            return StatusCode(StatusCodes.Status404NotFound);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shipOrder = await _context.shipOrders
                .Include(s => s.Order)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shipOrder == null)
            {
                return NotFound();
            }
            var userId = userManager.GetUserId(User);

            ViewBag.User = _context.ApplicationUser.Where(x => x.Id == userId).Select(u => u.FullName).FirstOrDefault();
            return View(shipOrder);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["OrderId"] = new SelectList(_context.Order, "Id", "Owner");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,PostCode,HouseNoOrBuilding,Street,Country,State,Lga,ShippingPrice,TrackingId,OrderId")] ShipOrder shipOrder)
        {
            if (ModelState.IsValid)
            {
                _context.Add(shipOrder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrderId"] = new SelectList(_context.Order, "Id", "Owner", shipOrder.OrderId);
            return View(shipOrder);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shipOrder = await _context.shipOrders.FindAsync(id);
            if (shipOrder == null)
            {
                return NotFound();
            }
            ViewData["OrderId"] = new SelectList(_context.Order, "Id", "Owner", shipOrder.OrderId);
            return View(shipOrder);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,PostCode,HouseNoOrBuilding,Street,Country,State,Lga,ShippingPrice,TrackingId,OrderId")] ShipOrder shipOrder)
        {
            if (id != shipOrder.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shipOrder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShipOrderExists(shipOrder.Id))
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
            ViewData["OrderId"] = new SelectList(_context.Order, "Id", "Owner", shipOrder.OrderId);
            return View(shipOrder);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shipOrder = await _context.shipOrders
                .Include(s => s.Order)
                .FirstOrDefaultAsync(m => m.Id == id);
            var Order = await _context.Order
              .FirstOrDefaultAsync(m => m.Id == id);
            if (Order == null)
            {
                return NotFound();
            }

            return View(shipOrder);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shipOrder = await _context.shipOrders.FindAsync(id);
            _context.shipOrders.Remove(shipOrder);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShipOrderExists(int id)
        {
            return _context.shipOrders.Any(e => e.Id == id);
        }
    }
}
