using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TheAgooProjectModel;

namespace SKNSchoolsPortal.Pages.account
{
    public class loginModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<loginModel> logger;

        public loginModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<loginModel> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }

        public class Inputs
        {
            [Required]
            [StringLength(maximumLength: 20, ErrorMessage = "Username should not exceed 20 characters", MinimumLength = 5)]
            public string Username { get; set; }
            [Required]
            [StringLength(maximumLength: 30)]
            [DataType(DataType.Password)]
            public string Password { get; set; }
            public bool RememberMe { get; set; } = false;
        }
        [BindProperty]
        public Inputs Input { get; set; }
        [BindProperty]
        public string? ReturnUrl { get; set; }
        public void OnGet(string? returnUrl)
        {
            ReturnUrl = returnUrl;
        }
        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(Input.Username);
                if (user != null)
                {
                    if (!user.Active)
                    {
                        ModelState.AddModelError(string.Empty, "User account has been disabled from accessing the system. Please contact your school system administrator.");
                        return Page();
                    }
                    var result = await signInManager.PasswordSignInAsync(user, Input.Password, Input.RememberMe, false);
                    if (result.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                        {
                            if (ReturnUrl == "/")
                            {
                                return RedirectToPage("/Index");
                            }
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToPage("/Index");
                        }
                    }
                    ModelState.AddModelError(string.Empty, "Wrong Username or Password");
                    return Page();
                }
                ModelState.AddModelError(string.Empty, "Wrong Username or Password");
                return Page();
            }
            ModelState.AddModelError(string.Empty, "Provide Username and Password");
            return Page();
        }
    }
}
