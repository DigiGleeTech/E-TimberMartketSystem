using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rocky.Data;
using Rocky.Models;
using System.Linq;
using static Rocky.Models.Location;

namespace Rocky.Controllers
{
    public class LocationController : Controller
    {
        private readonly ILogger<LocationController> logger;
        private readonly ApplicationDbContext db;

        public LocationController(ILogger<LocationController> logger, ApplicationDbContext db)
        {
            this.logger = logger;
            this.db = db;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Countries = new SelectList(db.Countries, "Id", "CountryName");
            var countries = db.Countries.ToList();
            return View(countries);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Country country)
        {
           
            if (ModelState.IsValid)
            {
                db.Countries.Add(country);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(country);
        }

        [HttpGet]
        public IActionResult StateIndex()
        {
            var state = db.States.Include(x => x.Country).ToList();
            return View(state);
        }

        [HttpGet]
        public IActionResult CreateState()
        {
            ViewBag.Countries = new SelectList(db.Countries, "Id", "CountryName");
            return View();
        }

        [HttpPost]
        public IActionResult CreateState(State state)
        {
            if (ModelState.IsValid)
            {
                db.States.Add(state);
                db.SaveChanges();
                return RedirectToAction(nameof(StateIndex));
            }
            return View(state);
        }
        public IActionResult LgaIndext()
        {
            var cities = db.Lgas.Include(c=>c.State).ToList();
            return View(cities);
        }

        [HttpGet]
        public IActionResult CreateLga()
        {
            ViewBag.State = new SelectList(db.States, "Id", "StateName");
            return View();
        }

        [HttpPost]
        public IActionResult CreateLga(Lga lga)
        {
            if (ModelState.IsValid)
            {
                db.Lgas.Add(lga);
                db.SaveChanges();
                return RedirectToAction(nameof(LgaIndext));
            }
            return View(lga);
        }

        public IActionResult CascadList()
        {
            ViewBag.Countries = db.Countries.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.CountryName
            });
            return View();
        }

        public JsonResult LoadState(int Id)
        {
            //var state = db.States.Where(x => x.CountryId == Id).ToList();
            var state = db.States.Where(x => x.CountryId == Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.StateName
                });
            return Json(state);
        }

        public JsonResult LoadLga(int Id)
        {
            //var city = db.Lgas.Where(x => x.State.Id == Id).ToList();
            //return Json(new SelectList(city, "Id", "LgaName"));
            var city = db.Lgas.Where(x => x.StateId == Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.LgaName,
                    Selected = true
                });
            return Json(city);
        }
    }
}
