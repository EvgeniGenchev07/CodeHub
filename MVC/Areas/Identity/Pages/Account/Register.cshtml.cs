using BusinessLayer;
using DataLayer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

[AllowAnonymous]
public class RegisterModel : PageModel
{
    private readonly IdentityContext _identityContext;
    private readonly SignInManager<User> _signInManager;
    public RegisterModel(IdentityContext identityContext,SignInManager<User> signInManager)
    {
        _identityContext = identityContext;
        _signInManager = signInManager;
    }

    [BindProperty]
    public InputModel Input { get; set; }
    public string ReturnUrl { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Изисква се потребителско име")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Изисква се имейл")]
        [EmailAddress(ErrorMessage = "Невалиден имейл")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Изисква се парола")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Потвърдете паролата")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Паролите не съвпадат")]
        public string ConfirmPassword { get; set; }
    }
    public async Task OnGetAsync(string? returnUrl)
    {
        ReturnUrl = returnUrl;
    }
    public async Task<IActionResult> OnPostAsync(string? returnUrl)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");
        if (ModelState.IsValid)
        {
            var user = new User
            {
                UserName = Input.Username,
                Email = Input.Email
            };

            try
            {
                await _identityContext.CreateUserAsync(user, Input.Password, Role.USER);
                var newUser = await _signInManager.UserManager.FindByEmailAsync(user.Email);
                await _signInManager.SignInAsync(newUser, isPersistent: false);
                return LocalRedirect("/");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
        }
        return Page();
    }
}
