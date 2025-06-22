using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BusinessLayer;

public class Comment
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public User Author { get; set; }
    public DateTime Date { get; set; }
    public string Content { get; set; }
    [JsonIgnore]
    public Forum Forum { get; set; }
    public Comment(){}
    public Comment(User author, string content, Forum forum)
    {
        Author = author;
        Content = content;
        Date = DateTime.Now;
        Forum = forum;
    }
}
