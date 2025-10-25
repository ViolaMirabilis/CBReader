using SharpCompress.Readers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CBReader.View
{
    /// <summary>
    /// Interaction logic for ComicBookView.xaml
    /// </summary>
    public partial class ComicBookView : Window, INotifyPropertyChanged
    {
        // Reading and navigation through a comic book
        private List<BitmapImage> _comicBookPages = new List<BitmapImage>();        // Holds images in the memory, extracted from the comic book archive.
        private int _currentPage = 0;

        // Window & Zoom properties
        private bool _isFullScreen = false;
        private double _baseHeight;
        private double _baseWidth;
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
        private const double _zoomScale = 0.10;

        private readonly DispatcherTimer _mouseHoverDelay;
        public ComicBookView()
        {
            InitializeComponent();

            DataContext = this; // so the CurrentZoom is loaded initially

            LoadComicBookResolution();

            _mouseHoverDelay = new DispatcherTimer();                       // creates a new timer on initialisation
            _mouseHoverDelay.Interval = TimeSpan.FromMilliseconds(1000);     // sets the interval to 1000ms (1 sec)
            _mouseHoverDelay.Tick += MouseHoverDelay_Tick;                  // ticks 
        }

        /// <summary>
        /// If the mouse moves, the timer starts and the UI shows right away. After the 1000ms, it ticks (stops the timer) and hids the UI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        // This event fires when the user moves the mouse on the screen, making the UI visible. It Starts the timer (interval), and after the interval, it stops and hides the UI.

        #region Button Logic
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            OverlapGUIGrid.Visibility = Visibility.Visible;

            _mouseHoverDelay.Stop();
            _mouseHoverDelay.Start();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F11)
            {
                ToggleFullScreen();
            }
        }
        #endregion

        #region Timer Tick
        private void MouseHoverDelay_Tick(object? sender, EventArgs e)
        {
            _mouseHoverDelay.Stop();
            OverlapGUIGrid.Visibility = Visibility.Hidden;
        }
        #endregion

        #region General Methods
        private void LoadComicBookResolution()
        {
            _baseWidth = this.imgSinglePageView.ActualWidth;
            _baseHeight = this.imgSinglePageView.ActualHeight;
        }
        private void ToggleFullScreen()
        {
            if (!_isFullScreen)
            {
                this.WindowStyle = WindowStyle.None;    // Removes top bar
                this.WindowState = WindowState.Maximized;
                this.Topmost = true;
                _isFullScreen = true;
            }
            else
            {
                this.WindowStyle = WindowStyle.SingleBorderWindow;   // reverts back to normal
                this.WindowState = WindowState.Normal;
                this.Topmost = false;
                _isFullScreen = false;
            }
        }

        private void ZoomIN_Click(object sender, RoutedEventArgs e)
        {
            _currentZoom += _zoomScale;

            if (_currentZoom >= 3.0)
                _currentZoom = 3.0; // so it doesnt go below that.*/

            ScaleTransform transform = ContentGrid.LayoutTransform as ScaleTransform;
            if (transform == null)
            {
                transform = new ScaleTransform(1, 1);
                ContentGrid.LayoutTransform = transform;
            }

            transform.ScaleX = _currentZoom;
            transform.ScaleY = _currentZoom;
        }

        private void ZoomOUT_Click(object sender, RoutedEventArgs e)
        {
            _currentZoom -= _zoomScale;

            if (_currentZoom <= 0.10)
                _currentZoom = 0.10; // so it doesnt go below that.

            ScaleTransform transform = ContentGrid.LayoutTransform as ScaleTransform;
            if (transform == null)
            {
                transform = new ScaleTransform(1, 1);
                ContentGrid.LayoutTransform = transform;
            }

            transform.ScaleX = _currentZoom;
            transform.ScaleY = _currentZoom;
            }
        #endregion


        #region Reading comic book from the archive
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
                imgSinglePageView.Source = _comicBookPages[_currentPage];
            }

        }

        #endregion

        // This should be separated to views and view models. Then, a view model should inherit from ViewModelBase
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)     // CallerMemberName so the method can be called without property's name
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));      // if property isn't null
        }

        
    }
}
