using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessLayer;

public class Comment
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public User Author { get; set; }
    public DateTime Date { get; set; }
    public string Content { get; set; }
    public Forum Forum { get; set; }
}
