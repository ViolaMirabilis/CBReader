using Microsoft.Win32;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CBReader.Model;

public class ComicBook : INotifyPropertyChanged
{
    public int Id { get; }
    public string Title { get; set; } = "N/A";
    public int LastReadPage { get; set; } = 0;      // JSON? move it to a separate class
    // Making it bindable, so the UI is notified once the "favourite" property changes.
    private bool _isFavourite = false;      // move to separate class too
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
    

    public ComicBook(string title, string archivePath)
    {
        Title = title;
        ArchivePath = archivePath;
    }


    // used more than once - I should make a ViewModelBase and separate view and view models.
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); 
    }
}
