using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessLayer;

public class Forum
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [MaxLength(100)]
    [Required]
    public string Title { get; set; }
    [Required]
    public string Content { get; set; }
    [Required]
    public User Author { get; set; }
    [Required]
    public DateTime Date { get; set; }
    public int Views { get; set; }
    public List<Filters> Filters { get; set; }
    public Forum(){}
    public Forum(string title, string content, User author)
    {
        Title = title;
        Content = content;
        Author = author;
        Date = DateTime.Now;
        Views = 0;
    }
}
