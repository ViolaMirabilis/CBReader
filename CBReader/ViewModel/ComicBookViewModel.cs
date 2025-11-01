using CBReader.Commands;
using SharpCompress.Readers;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CBReader.Model;
using CBReader.Services;

namespace CBReader.ViewModel;

public class ComicBookViewModel : INotifyPropertyChanged
{

    // TO SET IMAGES = USE THE "SOURCE" IN SINGLE/DOUBLE PAGE VIEW IN THE VIEW@@@@
    private readonly ComicBookService _comicBookService = new ComicBookService();
    private List<BitmapImage> _comicBookPages = new List<BitmapImage>();        // Holds images in the memory, extracted from the comic book archive.
    public IReadOnlyList<BitmapImage> ComicBookPages
    {
        get { return _comicBookPages;}
    }
    private readonly DispatcherTimer _mouseHoverDelay;
    private const double _zoomMultiplier = 0.10;


    #region Binding Properties
    private const double _maxZoomOut = 0.01;
    public double MaxZoomOut        // read only
    {
        get { return _maxZoomOut; }
    }
    private const double _maxZoomIn = 3.0;
   
    public double MaxZoomIn // read only
    {
        get { return _maxZoomIn; }
    }

    private bool _isOverlapUIVisible = false;
    public bool IsOverlapUIVisible
    {
        get { return _isOverlapUIVisible; }
        set
        {
            _isOverlapUIVisible = value;
            OnPropertyChanged();
        }
    }

    private int _totalPages = 0;
    public int TotalPages
    {
        get { return _totalPages; }
        set {
            _totalPages = value;
            OnPropertyChanged();
        }
    }

    private int _currentPage = 0;
    public int CurrentPage
    {
        get { return _currentPage; }
        set
        {
            _currentPage = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CurrentPage));
            OnPropertyChanged(nameof(CurrentPageView));
            OnPropertyChanged(nameof(NextPage));
            OnPropertyChanged(nameof(NextPageView));
        }
    }
    private int _nextPage;
    public int NextPage
    {
        get { return _currentPage; }
        set
        {
            _currentPage = value;
            OnPropertyChanged();
        }
    }

    private bool _isTwoPageView = false;
    public bool IsDoublePage
    {
        get { return _isTwoPageView; }
        set
        {
            _isTwoPageView = value;
            OnPropertyChanged();
        }
    }

    private bool _isFullscreen = false;
    public bool IsFullscreen
    {
        get { return _isFullscreen; }
        set
        {
            _isFullscreen = value;
            OnPropertyChanged();
        }
    }

    private double _currentZoom = 0.50;
    public double CurrentZoom
    {
        get { return _currentZoom; }
        set
        {
            _currentZoom = value;
            OnPropertyChanged();
        }
    }

    public BitmapImage CurrentPageView => _comicBookPages[CurrentPage];    // Holds a reference to the current page                           
    public BitmapImage? NextPageView => CurrentPage >= 0 ? _comicBookPages[CurrentPage + 1] : null;  // Holds a reference to previous page

    #endregion


    #region Commands
    public ICommand ZoomInCommand { get; set; }
    public ICommand ZoomOutCommand { get; set; }
    public ICommand GoNextPageCommand { get; set; }
    public ICommand GoPreviousPageCommand { get; set; }
    public ICommand OnePageViewCommand { get; set; }
    public ICommand TwoPageViewCommand { get; set; }
    #endregion




    public ComicBookViewModel(ComicBookService service)
    {
        _comicBookService = service; // repsonsible for reading the comic book from the archive

        _mouseHoverDelay = new DispatcherTimer();                       // creates a new timer on initialisation
        _mouseHoverDelay.Interval = TimeSpan.FromMilliseconds(1000);     // sets the interval to 1000ms (1 sec)
        _mouseHoverDelay.Tick += MouseHoverDelay_Tick;

        ZoomInCommand = new RelayCommand(ZoomIn);
        ZoomOutCommand = new RelayCommand(ZoomOut);
        GoNextPageCommand = new RelayCommand(GoNextPage, CanGoNextPage);
        GoPreviousPageCommand = new RelayCommand(GoPreviousPage, CanGoPreviousPage);
        OnePageViewCommand = new RelayCommand(SetOnePageView);
        TwoPageViewCommand = new RelayCommand(SetTwoPageView);
    }


    #region Timer Tick
    private void MouseHoverDelay_Tick(object? sender, EventArgs e)
    {
        _mouseHoverDelay.Stop();        // Stops the timer
        IsOverlapUIVisible = false;     // Hides the UI (when the mouse stops moving)
    }
    #endregion

    private void SetOnePageView(object obj) => IsDoublePage = false;

    private void SetTwoPageView(object obj) => IsDoublePage = true;

    private bool CanGoPreviousPage(object obj)
    {
        return CurrentPage > 0;
    }

    private void GoPreviousPage(object obj)
    {
        if (IsDoublePage)
        {
            if (CurrentPage - 2 >= 0)       // so we don't go below 0 (page one)
            {
                CurrentPage -= 2;
            }
            else
            {
                CurrentPage = 0;
            }
        }
        else
        {
            if (CurrentPage -1 >= 0)
            {
                CurrentPage -= 1;
            }
        }
    }

    private bool CanGoNextPage(object obj)
    {
        if (IsDoublePage)
            return CurrentPage < _totalPages - 2;   // so the last two pages are visible at the end.
        else
            return CurrentPage < _totalPages - 1;
    }

    private void GoNextPage(object obj)
    {
        if (IsDoublePage)       // double page view
        {
            if (CurrentPage + 2 < TotalPages)       // if possible, skips two pages
            {
                CurrentPage += 2;
            }
            else if (CurrentPage + 1 < TotalPages)      // if not, skips one
            {
                CurrentPage += 1;
            }        
        }
        else // single page view
        {
            if (CurrentPage + 1 < TotalPages)
            {
                CurrentPage += 1;
            }
        }
       
    }

    private void ZoomOut(object obj)
    {
        CurrentZoom = Math.Max(CurrentZoom - _zoomMultiplier, _maxZoomOut);    // The value can't go below 0.10. Math.Max needed
    }

    private void ZoomIn(object obj)
    {
        CurrentZoom = Math.Min(CurrentZoom + _zoomMultiplier, _maxZoomIn);    // The value can't go above 3.0. Math.Min needed
    }

    #region Reading comic book from the archive --> in memory
    // Should be async with a "loading" animation
    public void LoadComicBookFromArchiveToMemory(ComicBook comic)
    {
        _comicBookPages = _comicBookService.LoadComicBookToMemory(comic);       // utilises the service
        OnPropertyChanged(nameof(ComicBookPages));
        TotalPages = _comicBookPages.Count;

    }

    #endregion

    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)     // CallerMemberName so the method can be called without property's name
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));      // if property isn't null
    }

    #endregion
}
