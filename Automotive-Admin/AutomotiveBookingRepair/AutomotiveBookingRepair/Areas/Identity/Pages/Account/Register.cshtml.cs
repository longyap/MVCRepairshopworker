using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using AutomotiveBookingRepair.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutomotiveBookingRepair.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<AutomotiveBookingRepairUser> _signInManager;
        private readonly UserManager<AutomotiveBookingRepairUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager; //IdentutyRole is one of the services from Identity framework

        public SelectList RoleSelectList = new SelectList(
            new List<SelectListItem>
            {
                new SelectListItem { Selected = true, Text = "Select Role", Value = ""},
                new SelectListItem { Selected = true, Text = "Admin", Value = "Admin"},
                new SelectListItem { Selected = true, Text = "Customer", Value = "Customer"},
                new SelectListItem { Selected = true, Text = "Worker", Value = "Worker"},
            }, "Value", "Text", 1);

        public RegisterModel(
            UserManager<AutomotiveBookingRepairUser> userManager,
            SignInManager<AutomotiveBookingRepairUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender, RoleManager<IdentityRole> roleManager) // roleManager is for bring the data from the online to current role manager.
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel // model use for form design
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "Required")]
            [Display(Name = "Full Name")]
            public string CustomerFullName { get; set; }

            [Required]
            [Display(Name = "Age")]
            public int CustomerAge { get; set; }

            [Display(Name = "User Role")]
            public string userrole { set; get; }

        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid) // if form is validated, then just proceed
            {
                var user = new AutomotiveBookingRepairUser // register data through here
                {
                    // table column refer which input column
                    UserName = Input.Email,
                    Email = Input.Email,
                    CustomerFullName = Input.CustomerFullName,
                    CustomerAge = Input.CustomerAge,
                    EmailConfirmed = true,
                    userrole = Input.userrole
                    //PasswordHash = Input.Password // another way for to password in the database
                };
                var result = await _userManager.CreateAsync(user, Input.Password); // for not to show password in database
                if (result.Succeeded)
                {
                    //_logger.LogInformation("User created a new account with password.");

                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    bool roleresult = await _roleManager.RoleExistsAsync("Admin");

                    //below two if, is for aspnetrole
                    if (!roleresult)
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Admin")); // register role in ASPNETROLE table
                    }
                    roleresult = await _roleManager.RoleExistsAsync("Customer");

                    if (!roleresult)
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Customer"));
                    }

                    roleresult = await _roleManager.RoleExistsAsync("Worker");

                    if (!roleresult)
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Worker"));
                    }

                    await _userManager.AddToRoleAsync(user, Input.userrole); // store userid and roleid in the ASPNETUSERROLES table

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        // return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                        return RedirectToPage("login"); // after registration, redirect to login page
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
