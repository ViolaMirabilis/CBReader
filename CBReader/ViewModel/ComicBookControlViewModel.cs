using CBReader.Model;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CBReader.ViewModel;

// The class links the ComicBook and its State models into one, so the control can connect them easily and use one data context.
public class ComicBookControlViewModel : INotifyPropertyChanged
{
    public ComicBook Comic { get; }
    public ComicBookState State { get; }

    public ComicBookControlViewModel(ComicBook comic, ComicBookState state)
    {
        Comic = comic;
        State = state;

        Comic.PropertyChanged += (s, e) => OnPropertyChanged(e.PropertyName);
        State.PropertyChanged += (s, e) => OnPropertyChanged(e.PropertyName);
    }

    public string Title => Comic.Title;
    public string CoverPath => Comic.CoverPath;

    public bool IsFavourite
    {
        get
        {
            return State.IsFavourite;
        }
        set
        {
            State.IsFavourite = value;
        }
    }

    public int LastReadPage
    {
        get
        {
            return State.LastReadPage; 
        }
        set
        {
            State.LastReadPage = value; 
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
