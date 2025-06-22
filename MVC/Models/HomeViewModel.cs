using BusinessLayer;

namespace MVC.Models;

public class HomeViewModel
{
    public List<Battle> Battles { get; set; }
    public List<Forum> Forums { get; set; }
    public List<Course> Courses { get; set; }
}
