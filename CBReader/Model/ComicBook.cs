using Microsoft.Win32;

namespace CBReader.Model;

public class ComicBook
{
    public int Id { get; set; }
    public string Title { get; set; } = "Name not found";
    public int Pages { get; set; }

    public ComicBook(int id, string title, int pages)
    {
        Id = id;
        Title = title;
        Pages = pages;
    }
}
