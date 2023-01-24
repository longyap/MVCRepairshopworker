using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Auto_Cust.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Auto_Cust.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<Auto_CustUser> _userManager;
        private readonly SignInManager<Auto_CustUser> _signInManager;

        public IndexModel(
            UserManager<Auto_CustUser> userManager,
            SignInManager<Auto_CustUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel // form design use
        {
            [Phone]
            [Required]
            [StringLength(50, ErrorMessage = "Only 10 - 50 chars allowed!", MinimumLength = 10)]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Required]
            [Display(Name = "Your Full Name")]
            public string CustomerFullName { get; set; }

            [Required]
            [Display(Name = "Your Age")]
            public int CustomerAge { get; set; }

            [Required]
            [Display(Name = "Your Address")]
            public string CustomerAddress { get; set; }

            [Required]
            [DataType(DataType.Date)]
            [Display(Name = "Your Date of Birth")]
            public DateTime CustomerDOB { get; set; }
        }

        private async Task LoadAsync(Auto_CustUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                //bring database value to form
                PhoneNumber = phoneNumber,
                CustomerFullName = user.CustomerFullName,
                CustomerAge = user.CustomerAge, 
                CustomerAddress = user.CustomerAddress,
                CustomerDOB = user.CustomerDOB,
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
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
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            if(Input.CustomerFullName != user.CustomerFullName)  //save value modify 
            {
                user.CustomerFullName = Input.CustomerFullName;
            }

            if (Input.CustomerAddress != user.CustomerAddress)  //save value modify 
            {
                user.CustomerAddress = Input.CustomerAddress;
            }

            if (Input.CustomerAge != user.CustomerAge)  //save value modify 
            {
                user.CustomerAge = Input.CustomerAge;
            }

            if (Input.CustomerDOB != user.CustomerDOB)  //save value modify 
            {
                user.CustomerDOB = Input.CustomerDOB;
            }


            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
