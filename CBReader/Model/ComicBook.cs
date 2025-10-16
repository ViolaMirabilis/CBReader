using Microsoft.Win32;

namespace CBReader.Model;

public class ComicBook
{
    public int Id { get; set; }
    public string Title { get; set; } = "Name not found";
    public int Pages { get; set; }
    public int LastReadPage { get; set; } = 0;      // JSON?
    public string CoverPath { get; set; } = @"C:\Users\zajac\Desktop\test.jpg";     // Temporarily default CoverPath

    public ComicBook(int id, string title, int pages, string coverPath = "")
    {
        Id = id;
        Title = title;
        Pages = pages;
        CoverPath = string.IsNullOrEmpty(coverPath) ? CoverPath : coverPath;        // if null, returns the empty "" as a cover path.
    }
}
