using System.ComponentModel.DataAnnotations;

namespace BusinessLayer;

public class UserCourse
{
    [Key]
    public int Id { get; set; }
    public User User { get; set; }
    public Course Course { get; set; }
    public double Completion { get; set; }
}