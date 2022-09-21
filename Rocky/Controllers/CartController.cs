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
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> userManager;

        [BindProperty]
        public ProductUserVM ProductUserVM { get; set; }
        public CartController(
            ApplicationDbContext db, 
            IWebHostEnvironment webHostEnvironment, 
            IEmailSender emailSender,
            UserManager<IdentityUser> userManager)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
            this.userManager = userManager;
        }
        public IActionResult Index()
        {

            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                //session exsits
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            List<int> prodInCart = shoppingCartList.Select(i => i.ProductId).ToList();
            IEnumerable<Product> prodList = _db.Product.Where(u => prodInCart.Contains(u.Id));

            //Total price of product 
            ViewBag.TotalPrice = prodList.Select(p => p.Price).Sum();

            return View(prodList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {

            return RedirectToAction(nameof(Summary));
        }


        public IActionResult Summary()
        {
            //var orde = _db.Order.Include(s=>s.ShipOrder).Include(a=>a.ApplicationUsers);

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            //var userId = User.FindFirstValue(ClaimTypes.Name);

            //get CountryList
            ViewBag.Countries = _db.Countries.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.CountryName,
            });

            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                //session exsits
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            List<int> prodInCart = shoppingCartList.Select(i => i.ProductId).ToList();
            IEnumerable<Product> prodList = _db.Product.Where(u => prodInCart.Contains(u.Id));


            //user user id
            var userId = userManager.GetUserId(HttpContext.User);

            ProductUserVM = new ProductUserVM()
            {
                ApplicationUser = _db.ApplicationUser.FirstOrDefault(u => u.Id == claim.Value),
                ProductList = prodList.ToList(),
                ShippingList = _db.shipOrders.Where(x => x.UserId == claim.Value).ToList()

            };
            //User Id
            ViewBag.UserId = userId;

            var IsShippingDetailsAvailable = _db.shipOrders.Where(x=>x.UserId == userId).Any();
            //List<ShipOrder> shippingDetails = _db.shipOrders.Where(x => x.UserId == userId).ToList();
            ViewBag.IsShippingDetailsAvailable = IsShippingDetailsAvailable;
            ViewBag.shippingDetails = ProductUserVM.ShippingList;
            //Get currentUser Details
            var user = _db.ApplicationUser.Where(x => x.Id == userId).FirstOrDefault();

            //Get FullName of the current user
            ViewBag.FullName = user.FullName;


                        
            //Get total price of all selected items
            var totalPrice = prodList.Select(p => p.Price).Sum();
            //Total price of product  
            
            //Calculate shipping fees
            decimal shippingFees = 0;

            ViewBag.TotalPrice = totalPrice;

            foreach (var pro in prodList)
            {
                shippingFees = ((decimal)((pro.Price / 100) * 3));
            }

           

             //Calculte total amount + shipping fee
             var totalAmount = shippingFees + (decimal)totalPrice;

            ViewBag.TotalAmount = totalAmount;  

            //Shpping fees
            ViewBag.ShippingFees = shippingFees;

            //Get Shpping Id
            var ShippinID = _db.shipOrders.Where(x => x.UserId == userId).Select(s => s.Id).FirstOrDefault();
            ViewBag.ShippinID = ShippinID;


            return View(ProductUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(ProductUserVM ProductUserVM)
        {
            if (ProductUserVM.CreditCard.CardNumber == null)
            {
                return RedirectToAction(nameof(PaymentRequired));
            }

            var PathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
                + "templates" + Path.DirectorySeparatorChar.ToString() +
                "Inquiry.html";

            var subject = "New Inquiry";
            string HtmlBody = "";
            using (StreamReader sr = System.IO.File.OpenText(PathToTemplate))
            {
                HtmlBody = sr.ReadToEnd();
            }
            //Name: { 0}
            //Email: { 1}
            //Phone: { 2}
            //Products: {3}

            StringBuilder productListSB = new StringBuilder();
            foreach (var prod in ProductUserVM.ProductList)
            {
                productListSB.Append($" - Name: {prod.Name} <span style='font-size:14px;'> (ID: {prod.Id})</span><br />");
            }

            string messageBody = string.Format(HtmlBody,
                ProductUserVM.ApplicationUser.FullName,
                ProductUserVM.ApplicationUser.Email,
                ProductUserVM.ApplicationUser.PhoneNumber,
                productListSB.ToString());

            //get current user username
            var userName = userManager.GetUserName(HttpContext.User);

            //Get Current User Id
            var userId = userManager.GetUserId(HttpContext.User);

            ItemTracker tracker = new ItemTracker();
            
            //Get user Identity
            var claimsIdentity = (ClaimsIdentity)User.Identity;

            //Get Users claim
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            //Items in the shoppingCart
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                //session exsits
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            List<int> prodInCart = shoppingCartList.Select(i => i.ProductId).ToList();
            IEnumerable<Product> prodList = _db.Product.Where(u => prodInCart.Contains(u.Id));

            //Total price of product 
            var TotalPrice = prodList.Select(p => p.Price).Sum();

            ViewBag.TotalPrice = TotalPrice;

            //Append coma to each product
            var productName = "";
            foreach (var prod in prodList)
            {
                 productName += prod.Name + ", ";
            }
                        
            var productId = prodList.Select(p => p.Id);

            //Retrive User Information
            var user = _db.ApplicationUser.Where(x => x.Id == userId).FirstOrDefault();
            
            //Get total price of all selected items
            var totalPrice = prodList.Select(p => p.Price).Sum();
            //Total price of product  

            //Calculate shipping fees
            decimal shippingFees = 0;

            ViewBag.TotalPrice = totalPrice;

            foreach (var pro in prodList)
            {
                shippingFees = ((decimal)((pro.Price / 100) * 3));
            }

            //Calculte total amount + shipping fee
            var totalAmount = shippingFees + (decimal)totalPrice;

            ViewBag.TotalAmount = totalAmount;

            //Shpping fees
            ViewBag.ShippingFees = shippingFees;

            
            //Add order
            Order order = new Order()
            {
                Owner = user.FullName,
                TrackingId = "DGT" + tracker.TrackingNumber,
                OrderDate = DateTime.Now,
                DeliveryDate = DateTime.Now.AddDays(25),
                Product = productName,
                Amount = ProductUserVM.Order.Amount,
                User = user.Id,
            };

            //Add order to Database
            _db.Add(order);

            //Save change made
            _db.SaveChanges();
       

            //order Id
            int orderId = order.Id;

            ShipOrder shipping = new ShipOrder();
            // get country name base on the collected id
            var GetCountryName = _db.Countries.Where(x => x.Id.ToString() == ProductUserVM.ApplicationUser.ShipOrder.Country).FirstOrDefault();
            shipping.Country = GetCountryName.CountryName;
            shipping.State = ProductUserVM.ApplicationUser.ShipOrder.State;
            shipping.Lga = ProductUserVM.ApplicationUser.ShipOrder.Lga;
            var Num = "NO";
            Num.ToUpper();
            if (ProductUserVM.ApplicationUser.ShipOrder.HouseNoOrBuilding.Contains(Num))
            {
                shipping.HouseNoOrBuilding = ProductUserVM.ApplicationUser.ShipOrder.HouseNoOrBuilding;
            }
            else
            {
                shipping.HouseNoOrBuilding = $"{Num} {ProductUserVM.ApplicationUser.ShipOrder.HouseNoOrBuilding}";
            }
            shipping.Street = ProductUserVM.ApplicationUser.ShipOrder.Street;
            shipping.TrackingId = "DGT" + tracker.TrackingNumber;
            shipping.UserId = userId;
            shipping.OrderId = orderId;
            shipping.ShippingPrice = shippingFees;
            var applicationUser = _db.ApplicationUser.Where(x=>x.ShipOrder.Id == ProductUserVM.ApplicationUser.ShipOrder.Id).FirstOrDefault();
            var shipingId = _db.shipOrders.Where(x => x.UserId == userId).Select(x=>x.Id).FirstOrDefault();
            if (ProductUserVM.ApplicationUser.ShipOrder.Id != 0)
            {
                 if (shipingId == ProductUserVM.ApplicationUser.ShipOrder.Id)
                {
                    //Add ShippingDetails
                    _db.Update(shipping);
                }
                else
                {
                    //Add ShippingDetails to Database
                    _db.Add(shipping);
                }

            }
            else
            {
                //Add ShippingDetails to Database
                _db.Add(shipping);
            }
            
            //Save change made
            _db.SaveChanges();

            //Shipping Id
            int shippingId = shipping.Id;

            //Save tracking to user
            user.TrackingId = order.TrackingId;
            var randomPin = new Random();
            //CreditCard
            CreditCard creditCard = new CreditCard();
            creditCard.Ownner = ProductUserVM.ApplicationUser.FullName;
            creditCard.Type = ProductUserVM.CreditCard.Type;
            creditCard.CardNumber = ProductUserVM.CreditCard.CardNumber;
            creditCard.Expiry = ProductUserVM.CreditCard.Expiry;
            creditCard.Cvv = ProductUserVM.CreditCard.Cvv;
            creditCard.UserId = ProductUserVM.CreditCard.UserId;
            creditCard.Pin = randomPin.Next(7777, 9999);

            ProductUserVM = new ProductUserVM()
            {
                ApplicationUser = _db.ApplicationUser.FirstOrDefault(u => u.Id == claim.Value),
                ProductList = prodList.ToList()
            };

            //Check the availability of card of the current user
            if (userId == creditCard.UserId)
            {
                //Update Creadit Card
                _db.Update(creditCard);
            }
            else
            {
                //Add CreditCard 
                _db.Add(creditCard);
            }

            // Update Ids
            user.ShipOrderId = shippingId;
            user.OrderId = orderId;
            
            

            //Update User Infromation
            _db.ApplicationUser.Update(user);

            //Save change made
            _db.SaveChanges();

            

            await _emailSender.SendEmailAsync(WC.EmailAdmin, subject, messageBody);

            return RedirectToAction(nameof(InquiryConfirmation), order);
        }
        public IActionResult InquiryConfirmation()
        {
            HttpContext.Session.Clear();
            IEnumerable<ApplicationUser> applications = _db.ApplicationUser.Include(o=>o.Order).Include(s=>s.ShipOrder).ToList();
            return View(applications);
        }

        public IActionResult Remove(int id)
        {

            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                //session exsits
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            shoppingCartList.Remove(shoppingCartList.FirstOrDefault(u => u.ProductId == id));
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult TrackOder(string trackingNumber)
        {
            var userId = userManager.GetUserId(HttpContext.User);
            var user = _db.ApplicationUser.Where(x => x.Id == userId).FirstOrDefault();

            //Track order
            var IsOrderAvailable = _db.Order.Include(s => s.ShipOrders).Where(x => x.User == userId).Count();
            ViewBag.AvailableOrder = IsOrderAvailable;

            var prodList = _db.Order.Where(x => x.User == userId).ToList();

            prodList.Select(c => c.Product.Replace(',', ' '));

            return View(prodList);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

           
            var Order = await _db.Order
              .FirstOrDefaultAsync(m => m.Id == id);
            if (Order == null)
            {
                return NotFound();
            }

            return View(Order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var Order = await _db.Order.FindAsync(id);
            _db.Order.Remove(Order);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(TrackOder));
        }



        [HttpPost]
        public IActionResult SearchOder(string trackingNumber)
        {
            var userId = userManager.GetUserId(HttpContext.User);

            //List<string> orderList = _db.Order.Select(x => x.Id.ToString()).ToList();
            var prodList = _db.Order.Where(u => u.TrackingId == trackingNumber).ToList();

            return View("TrackOder", prodList);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _db.Order
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }
            var userId = userManager.GetUserId(User);

            ViewBag.User = _db.ApplicationUser.Where(x => x.Id == userId).Select(u => u.FullName).FirstOrDefault();
            return View(order);
        }

        public StatusCodeResult PaymentRequired()
        {
            return StatusCode(StatusCodes.Status402PaymentRequired);
        }


        public IActionResult cardd()
        {
            CreditCard credit = new CreditCard();
            return View();
        }
    }
}
