using CBReader.Commands;
using SharpCompress.Readers;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace CBReader.ViewModel;

public class ComicBookViewModel : INotifyPropertyChanged
{

    // TO SET IMAGES = USE THE "SOURCE" IN SINGLE/DOUBLE PAGE VIEW IN THE VIEW@@@@
    private List<BitmapImage> _comicBookPages = new List<BitmapImage>();        // Holds images in the memory, extracted from the comic book archive.
    private const double _zoomMultiplier = 0.10;
    private const double _maxZoomOut = 0.10;
    private const double _maxZoomIn = 2.5;
    private int _totalPages = 0;

    #region Binding Properties
    private int _currentPage = 0;
    public int CurrentPage
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

    private double _currentZoom = 0.5F;
    public double CurrentZoom
    {
        get { return _currentZoom; }
        set
        {
            _currentZoom = value;
            OnPropertyChanged();
        }
    }
    #endregion


    #region Commands
    public ICommand FullScreenCommand { get; set; }
    public ICommand ZoomInCommand { get; set; }
    public ICommand ZoomOutCommand { get; set; }
    public ICommand NextPageCommand { get; set; }
    public ICommand PreviousPageCommand { get; set; }
    public ICommand OnePageViewCommand { get; set; }
    public ICommand TwoPageViewCommand { get; set; }
    #endregion




    public ComicBookViewModel()
    {
        _totalPages = SetTotalPages();
        FullScreenCommand = new RelayCommand(ToggleFullScreen);
        ZoomInCommand = new RelayCommand(ZoomIn);
        ZoomOutCommand = new RelayCommand(ZoomOut);
        NextPageCommand = new RelayCommand(NextPage, CanGoNextPage);
        PreviousPageCommand = new RelayCommand(PreviousPage, CanGoPreviousPage);
        OnePageViewCommand = new RelayCommand(SetOnePageView);
        TwoPageViewCommand = new RelayCommand(SetTwoPageView);
    }

    private void SetOnePageView(object obj) => IsDoublePage = false;

    private void SetTwoPageView(object obj) => IsDoublePage = true;

    private bool CanGoPreviousPage(object obj) => CurrentPage > 0;

    private void PreviousPage(object obj)
    {
        CurrentPage--;
    }

    private bool CanGoNextPage(object obj) => CurrentPage < _totalPages - 1;        // If current page is NOT the last page.

    private void NextPage(object obj)
    {
        CurrentPage += 1;
    }

    private void ZoomOut(object obj)
    {
        CurrentZoom = Math.Min(CurrentZoom - _zoomMultiplier, _maxZoomOut);    // The value can't go below 0.10
    }

    private void ZoomIn(object obj)
    {
        CurrentZoom = Math.Min(CurrentZoom + _zoomMultiplier, _maxZoomIn);    // The value can't go above 3.0
    }

    private void ToggleFullScreen(object obj)
    {
        IsFullscreen = !IsFullscreen;
    }


    #region Helper methods
    private int SetTotalPages()
    {
        return _comicBookPages.Count;
    }
    #endregion

    #region Reading comic book from the archive --> in memory
    // Should be async with a "loading" animation
    public void LoadComicBookFromArchive(string path)
    {
        _comicBookPages.Clear();

        // @See https://github.com/adamhathcock/sharpcompress/blob/master/USAGE.md
        using (Stream stream = File.OpenRead(path))
        using (var reader = ReaderFactory.Open(stream))
        {
            while (reader.MoveToNextEntry())        // Goes into the all the files
            {
                if (!reader.Entry.IsDirectory)      // If the file isn't a folder, it runs the code below.
                {
                    using (var entryStream = reader.OpenEntryStream())
                    {
                        // Cannot use StreamReader, as it reads raw bytes (not suitable for images)
                        // @See https://stackoverflow.com/questions/5346727/convert-memory-stream-to-bitmapimage
                        byte[] data;        // The image data needs to be in an array first
                        using (var ms = new MemoryStream())
                        {
                            entryStream.CopyTo(ms);     // Reads from one stream, writes to another (ms)
                            data = ms.ToArray();        // then the data array gets the memory stream
                        }

                        var bitmap = new BitmapImage();
                        using (var ms2 = new MemoryStream(data))
                        {
                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;        // Important
                            bitmap.StreamSource = ms2;
                            bitmap.EndInit();
                            bitmap.Freeze();
                        }

                        _comicBookPages.Add(bitmap);     // Adds the converted bytes to the list of Bitmaps
                    }
                }
            }
        }

        if (_comicBookPages.Count > 0)
        {
            // To change due to moving from the view to the view model
            //imgSingle.Source = _comicBookPages[_currentPage];
        }

    }

    #endregion

    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)     // CallerMemberName so the method can be called without property's name
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));      // if property isn't null
    }

    #endregion
}
