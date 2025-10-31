using CBReader.Commands;
using CBReader.Model;
using CBReader.View;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CBReader.ViewModel;

public class MainWindowViewModel : INotifyPropertyChanged
{
    public ObservableCollection<ComicBook> ComicBooks { get; } = new ObservableCollection<ComicBook>();
    private readonly string _comicBooksPath = "";        // temporary
    private bool _showEmptyComicBookListLabel;  // responsible for showing
    public bool ShowEmptyComicBookListLabel
    {
        get { return _showEmptyComicBookListLabel;}
        set
        {
            _showEmptyComicBookListLabel = value;
            OnPropertyChanged();
        }

    }

    #region Commands declarations
    public ICommand ShowComicBookCommand { get; set; }
    #endregion

    public MainWindowViewModel()
    {
        FillComicBooks();
        // An event, which sets the value to true if the ComicBooks.Count is equal to 0.
        ComicBooks.CollectionChanged += (s, e) =>
        {
            ShowEmptyComicBookListLabel = ComicBooks.Count == 0;
        };
        ShowComicBookCommand = new RelayCommand(OpenComicBook, CanOpenComicBook);
        
    }

    #region Commands logic
    private bool CanOpenComicBook(object obj)
    {
        return obj is ComicBook;
    }

    private void OpenComicBook(object obj)
    {
        if (obj is ComicBook comic)
        {
            ComicBookView comicBookView = new ComicBookView();
            comicBookView.LoadComicBookFromArchive(comic.ArchivePath);
            comicBookView.Show();
        }
    }
    #endregion


    #region General Methods
    void FillComicBooks()
    {
        ComicBooks.Add(new ComicBook(0, "The Walking Dead", 20, @"C:\Users\zajac\Desktop\test.jpg", true));
        ComicBooks.Add(new ComicBook(1, "Batman", 25, @"C:\Users\zajac\Desktop\Batman.png"));
        ComicBooks.Add(new ComicBook(2, "Spiderman", 25, @"C:\Users\zajac\Desktop\Spiderman.png"));

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
