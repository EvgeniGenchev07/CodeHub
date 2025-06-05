using Microsoft.AspNetCore.Identity;

namespace BusinessLayer;

public class User: IdentityUser
{
    public string FullName { get; set; }
    public List<Course> Courses { get; set; }
    public int Points { get; set; }
    public int Level { get; set; }
    public User()
    {
        FullName = string.Empty; 
        Courses = new List<Course>(); 
        Points = 0; 
        Level = 1; 
    }
    public User(string id, string fullName, string email, string userName) : this (fullName, email, userName)
    {
        Id = id;
    }

    public User(string fullName, string email, string userName)
    {
        FullName = fullName;
        NormalizedEmail = email.ToUpper();
        NormalizedUserName = userName.ToUpper();
        UserName = userName;
        Email = email;
        Courses = new List<Course>();
        Points = 0;
        Level = 0;
    }
    public User(string email, string userName)
    {
        FullName = userName;
        NormalizedEmail = email.ToUpper();
        NormalizedUserName = email.ToUpper();
        UserName = email;
        Email = email;
        Courses = new List<Course>();
        Points = 0;
        Level = 0;
    }
}
