using BusinessLayer;
using DataLayer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

public class RegisterModel : PageModel
{
    private readonly IdentityContext _identityContext;

    public RegisterModel(IdentityContext identityContext)
    {
        _identityContext = identityContext;
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

    public void OnGet(string returnUrl = null)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

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
                await _identityContext.LogInUserAsync(Input.Email, Input.Password);
                return LocalRedirect(returnUrl);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
        }

        // Something failed, redisplay form
        return Page();
    }
}
