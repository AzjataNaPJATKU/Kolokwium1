namespace kolokwium1.Model;

public class BooksDTO
{
    public int id { get; set; }
    public string title { get; set; }
    public authorDTO author { get; set; }
}

public class authorDTO
{
    public string firstName { get; set; }
    public string lastName { get; set; }
}