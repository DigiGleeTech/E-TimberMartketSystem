using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rocky.Data;
using Rocky.Models;
using Rocky.Models.ViewModels;

namespace Rocky.Controllers
{
    public class SettingsController : Controller
    {
        private readonly ApplicationDbContext db;
        public UserManager<IdentityUser> userManager { get; }

        public SettingsController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        // GET: ShippingDetails
        public async Task<IActionResult> Index()
        {
            var userId = userManager.GetUserId(User);
            //var shipOrders = db.shipOrders.Include(s => s.Order).Where(x=>x.UserId == userId);
            //return View(await shipOrders.ToListAsync());
            ProductUserVM ProductUserVM = new ProductUserVM();
            var product = db.shipOrders.Where(x => x.UserId == userId).FirstOrDefault();
            ViewBag.product = product;
            IEnumerable<ApplicationUser> applications = db.ApplicationUser.Include(o => o.Order).Include(s => s.ShipOrder).Where(x=>x.Id == userId).ToList();
            return View(applications);
        }

        // GET: ShippingDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shipOrder = await db.shipOrders
                .Include(s => s.Order)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shipOrder == null)
            {
                return NotFound();
            }

            return View(shipOrder);
        }

        // GET: ShippingDetails/Create
        public IActionResult Create()
        {
            ViewData["OrderId"] = new SelectList(db.Order, "Id", "Owner");
            //get CountryList
            ViewBag.Countries = db.Countries.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.CountryName,
            });
            //get current user Id
            ViewBag.UserId = userManager.GetUserId(HttpContext.User);
            ViewBag.TrackingId = null;
            return View();
        }

        // POST: ShippingDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,PostCode,HouseNoOrBuilding,Street,Country,State,Lga,ShippingPrice,TrackingId,OrderId")] ShipOrder shipOrder)
        {
            if (ModelState.IsValid)
            {
                
                db.Add(shipOrder);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrderId"] = new SelectList(db.Order, "Id", "Owner", shipOrder.OrderId);
            return View(shipOrder);
        }

        // GET: ShippingDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shipOrder = await db.shipOrders.FindAsync(id);
            if (shipOrder == null)
            {
                return NotFound();
            }
            //get CountryList
            ViewBag.Countries = db.Countries.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.CountryName,
            });
            ViewData["OrderId"] = new SelectList(db.Order, "Id", "Owner", shipOrder.OrderId);
            return View(shipOrder);
        }

        // POST: ShippingDetails/Edit/5
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
                    var result =  db.Update(shipOrder);
                    await db.SaveChangesAsync();
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
               
                ViewBag.ErrorMessage = "Failed to update Details";
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrderId"] = new SelectList(db.Order, "Id", "Owner", shipOrder.OrderId);
            return View(shipOrder);
        }

        // GET: ShippingDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shipOrder = await db.shipOrders
                .Include(s => s.Order)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shipOrder == null)
            {
                return NotFound();
            }

            return View(shipOrder);
        }

        // POST: ShippingDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shipOrder = await db.shipOrders.FindAsync(id);
            db.shipOrders.Remove(shipOrder);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShipOrderExists(int id)
        {
            return db.shipOrders.Any(e => e.Id == id);
        }
    }
}
