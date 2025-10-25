using Microsoft.Win32;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CBReader.Model;

public class ComicBook : INotifyPropertyChanged
{
    public int Id { get; set; }
    public string Title { get; set; } = "Name not found";
    public int Pages { get; set; }
    public int LastReadPage { get; set; } = 0;      // JSON?
    // Making it bindable, so the UI is notified once the "favourite" property changes.
    private bool _isFavourite;
    public bool IsFavourite
    {
        get { return _isFavourite; }
        set
        {
            _isFavourite = value;
            OnPropertyChanged();
        }
    }


    public string ArchivePath { get; set; } = @"C:\Users\zajac\Desktop\tmpComicBook\TWD1.cbr";     // Temporarily default CoverPath
    public string CoverPath { get; set; } = @"C:\Users\zajac\Desktop\test.jpg";     // Temporarily default CoverPath
    

    public ComicBook(int id, string title, int pages, string coverPath = "", bool isFavourite = false)
    {
        Id = id;
        Title = title;
        Pages = pages;
        CoverPath = string.IsNullOrEmpty(coverPath) ? CoverPath : coverPath;        // if null, returns the empty "" as a cover path.
        IsFavourite = isFavourite;
    }


    // used more than once - I should make a ViewModelBase and separate view and view models.
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); 
    }
}
