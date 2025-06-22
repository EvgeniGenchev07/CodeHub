using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using BusinessLayer;

namespace CodeHub.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class ProfileEditModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public ProfileEditModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public byte[] ProfilePicture { get; set; }

        private async Task LoadAsync(User user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            Username = userName;
            ProfilePicture = user.ProfilePicture ?? Array.Empty<byte>();
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

        [HttpPost]
        public async Task<JsonResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return new JsonResult(new { success = false, message = "User not found" });
            }

            // Handle profile picture upload
            var profilePicture = Request.Form.Files["profilePicture"];
            if (profilePicture != null && profilePicture.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(profilePicture.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                {
                    return new JsonResult(new { success = false, message = "Invalid file type. Only JPG and PNG are supported." });
                }

                using (var memoryStream = new MemoryStream())
                {
                    await profilePicture.CopyToAsync(memoryStream);
                    user.ProfilePicture = memoryStream.ToArray();
                }
            }

            // Handle username change
            var username = Request.Form["username"];
            if (!string.IsNullOrEmpty(username) && username != user.UserName)
            {
                var setUsernameResult = await _userManager.SetUserNameAsync(user, username);
                if (!setUsernameResult.Succeeded)
                {
                    return new JsonResult(new { success = false, message = "Error changing username." });
                }
            }

            // Handle password change if provided
            var currentPassword = Request.Form["currentPassword"];
            var newPassword = Request.Form["newPassword"];
            if (!string.IsNullOrEmpty(currentPassword) && !string.IsNullOrEmpty(newPassword))
            {
                var changePasswordResult = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
                if (!changePasswordResult.Succeeded)
                {
                    var errors = changePasswordResult.Errors.Select(e => e.Description).ToList();
                    return new JsonResult(new { success = false, message = string.Join(", ", errors) });
                }

                await _signInManager.RefreshSignInAsync(user);
            }

            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);

            // Return the updated profile picture URL if changed
            string profilePictureUrl = null;
            if (user.ProfilePicture != null && user.ProfilePicture.Length > 0)
            {
                profilePictureUrl = $"data:image/jpeg;base64,{Convert.ToBase64String(user.ProfilePicture)}";
            }

            return new JsonResult(new
            {
                success = true,
                message = "Profile updated successfully!",
                profilePictureUrl
            });
        }
    }
}