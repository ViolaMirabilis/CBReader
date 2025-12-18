using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CBReader.Model;

public enum Status {
    [Description("Not started")]
    NotStarted,

    [Description("In Progress")]
    InProgress,

    Finished}
public class ComicBookState : INotifyPropertyChanged
{
    public Status Status { get; set; } = Status.NotStarted;
    public DateTime? LastRead { get; set; }

    // OnPropertyChanged, because it should notify the UI right away.
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

    private int _lastReadPage;

    // Notifies the UI right away.
    public int LastReadPage
    {
        get { return _lastReadPage; }
        set
        {
            _lastReadPage = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
