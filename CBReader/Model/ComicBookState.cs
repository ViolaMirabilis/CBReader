using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CBReader.Model;

public class ComicBookState : INotifyPropertyChanged
{
    public required Guid ComicId { get; set; }
    private int _lastReadPage;

    // custom Set, the value has to be changed in the "Library" view immediately.
    public int LastReadPage
    {
        get { return _lastReadPage; }
        set
        {
            _lastReadPage = value;
            OnPropertyChanged();
        }
    }
    // custom Set, the value needs to change in the "Library" view immediately.
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

    public bool HasFinishedReading { get; set; }
    public DateTime? LastRead { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
