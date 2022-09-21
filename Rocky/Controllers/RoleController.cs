using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rocky.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Controllers
{
   
    [Authorize(Roles = "Super Admin, Admin")]
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<IdentityUser> userManager;

        public RoleController(
            ApplicationDbContext db, 
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager)
        {
            this.db = db;
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        //
        // GET: /Role/
        //public IActionResult Index()
        //{
        //    var roleList = db.Roles.ToList();
        //    return View(roleList);
        //}
        public ViewResult Index() => View(roleManager.Roles);

        [HttpGet]
        public IActionResult UpsertRoleAsync(string id)
        {
            var role = new IdentityRole();
            return View(role);
        }

        [HttpPost]
        [Authorize(Policy = "RoleCreation")]
        public IActionResult UpsertRole(IdentityRole identity)
        {
            var role = new IdentityRole();
            if (ModelState.IsValid)
            {
                role.Name = identity.Name;

                db.Roles.Add(role);
                db.SaveChanges();
            }
            
            return RedirectToAction(nameof(Index));
        }

        //Get
        [HttpGet]
        public async Task<IActionResult> Edit(string Id)
        {
            if (Id == null)
            {
                ViewBag.ErrorMessage = $"Role with Id: {Id} cannot be found!";
                return View("Edit", Id);
            }

            var role = await roleManager.FindByIdAsync(Id);
            if (role == null)
            {
                return NotFound();
            }
            ViewBag.roleId = role.Id;

            return View(role);
        }

        [ActionName("Edit")]
        public async Task<IActionResult> EditRole(IdentityRole identityRole, string Id)
        {
            ViewBag.roleId = Id;

            var role = await roleManager.FindByIdAsync(Id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id: {Id} cannot be found!";
                return View("NotFound");
            }

            role.Name = identityRole.Name;
           
            db.Roles.Update(role);

            var result = await roleManager.UpdateAsync(role);

            await roleManager.UpdateAsync(role);
           
            db.SaveChanges();

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Edit));
        }


        // GET: Role/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await db.Roles.FirstOrDefaultAsync(m => m.Id == id.ToString());
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }



        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(string id)
        {
            var role = db.Roles.Find(id);
            db.Remove(role);
            db.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        private bool RoleExists(string id)
        {
            return db.Roles.Any(e => e.Id == id);
        }

    }
}
