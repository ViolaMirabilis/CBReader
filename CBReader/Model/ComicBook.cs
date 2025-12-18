using Microsoft.Win32;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CBReader.Model;

public class ComicBook : INotifyPropertyChanged
{
    public Guid Id { get; init; } = Guid.NewGuid(); // init, because the value should be immutable after construction.
    public string Title { get; set; } = "N/A";
    public string ArchivePath { get; set; } = @"C:\Users\zajac\Desktop\tmpComicBook\TWD1.cbr";     // Temporarily default CoverPath
    public string CoverPath { get; set; } = @"C:\Users\zajac\Desktop\test.jpg";     // Temporarily default CoverPath

    // Move this to ComicBookState Model.
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

    public ComicBook(string title, string archivePath)
    {
        Title = title;
        ArchivePath = archivePath;
    }

    // used more than once - I should make a ViewModelBase and separate view and view models.
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); 
}
