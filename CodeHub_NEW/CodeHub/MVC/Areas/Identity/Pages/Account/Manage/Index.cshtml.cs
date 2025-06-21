using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer;
using DataLayer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class IndexModel : PageModel
{
    private readonly UserManager<User> _userManager;
    private readonly CodeHubDbContext _dbContext;

    public IndexModel(UserManager<User> userManager,CodeHubDbContext context)
    {
        _userManager = userManager;
        _dbContext = context;
    }

    public string Username { get; set; }
    public string Email { get; set; }
    public byte[] ProfilePicture { get; set; }
    public int Points { get; set; }
    public int Level { get; set; }
    public List<Course> Courses { get; set; }
    public List<Forum> UserForums { get; set; } 

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        Username = user.UserName;
        Email = user.Email;
        ProfilePicture = user.ProfilePicture;
        Points = user.Points;
        Level = user.Level;
        Courses = user.Courses?.ToList() ?? new List<Course>();
        UserForums = await _dbContext.Forums
       .Where(f => f.Author.Id == user.Id)
       .OrderByDescending(f => f.Date)
       .ToListAsync();

        return Page();
    }
}