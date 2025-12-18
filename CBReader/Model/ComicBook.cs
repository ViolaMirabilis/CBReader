using Microsoft.Win32;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CBReader.Model;

public class ComicBook : INotifyPropertyChanged
{
    public Guid Id { get; init; } = Guid.NewGuid(); // init, because the value should be immutable after construction.
    public string Title { get; set; }
    public string ArchivePath { get; }
    public string? CoverPath { get; set; }
    public ComicBookState State { get;} = new ComicBookState();        // State is assigned only once, but can be changed later on.

    public ComicBook(string title, string archivePath)
    {

        Title = title;
        ArchivePath = archivePath;

        State.PropertyChanged += (s, e) => OnPropertyChanged(nameof(State));
    }

    //Shorten the title if longer than 75. Visual purpose only.
    public string ShortenedTitle => Title.Length > 70 ? Title.Substring(0,70) : Title;

    // used more than once - I should make a ViewModelBase and separate view and view models.
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); 
}
