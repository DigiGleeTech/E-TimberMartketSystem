using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rocky.Data;
using Rocky.Models;

namespace Rocky.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext db;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this.db = db;
        }

       
        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
            public string FullName { get; set; }
            public string UserName { get; set; }
            public string TrakingId { get; set; }
            [EmailAddress]
            public string Email { get; set; }
            public byte[] ProfilePicture { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);


            Input = new InputModel
            {
                PhoneNumber = phoneNumber
            };
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var fullName = user.FullName;
            var trakingId = user.TrackingId;
            var profilePicture = user.ProfilePicture;
            var email = user.Email;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                UserName = userName,
                FullName = fullName,
                TrakingId = trakingId,
                ProfilePicture = profilePicture,
                Email = email
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            //Get current user
            var userId = _userManager.GetUserId(HttpContext.User);
            //Retrieve user datas
            var currentUser = db.ApplicationUser.Where(x => x.Id == userId).FirstOrDefault();

            //ViewData["Email"] = currentUser.Email;
                       
            //var fullName = currentUser.FullName;
            //var profilePicture = currentUser.ProfilePicture;
            //var email = _userManager.GetEmailAsync(user).ToString();
            //var mail = currentUser.Email;
            //Input = new InputModel
            //{
            //    PhoneNumber = phoneNumber,
            //    UserName = userName,
            //    FullName = currentUser.FullName,
            //    ProfilePicture = profilePicture,
            //    Email = mail
            //};

            await LoadAsync(currentUser);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var usernam = await _userManager.GetUserNameAsync(user);
            if (Input.PhoneNumber != phoneNumber )
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            ApplicationUser user1 = new ApplicationUser();

            if (Input.UserName != user.UserName)
            {
                var setUserNameResult = await _userManager.SetUserNameAsync(user, Input.UserName);
                if (!setUserNameResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set Username";
                    return RedirectToPage();
                }
            }

            // Get Current User Username
            var userName = await _userManager.GetUserNameAsync(user);
            //Get current user
            var userId = _userManager.GetUserId(HttpContext.User);
            //Retrieve user datas
            var currentUser = db.ApplicationUser.Where(x => x.Id == userId).FirstOrDefault();

            if (currentUser != null)
            {
                currentUser.FullName = Input.FullName;
                db.ApplicationUser.Update(currentUser);
                db.SaveChanges();
            }

            if (Request.Form.Files.Count > 0)
            {
                IFormFile file = Request.Form.Files.FirstOrDefault();
                using (var dataStream = new MemoryStream())
                {
                    await file.CopyToAsync(dataStream);
                    currentUser.ProfilePicture = dataStream.ToArray();

                    db.ApplicationUser.Update(currentUser);
                    db.SaveChanges();
                }
                await _userManager.UpdateAsync(user);
            }
            await _userManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
