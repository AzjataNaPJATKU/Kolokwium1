namespace kolokwium1.Model;

public class newBookWithAuthor
{
    public int id { get; set; }
    public string title { get; set; }
    
    public IEnumerable<Author> Authors { get; set; } = new List<Author>();
    
    public int IdAuthor { get; set; }
}

public class Author
{
    public int authorid { get; set; }
}