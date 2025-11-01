using CBReader.Commands;
using CBReader.Interfaces;
using CBReader.Model;
using CBReader.Services;
using CBReader.View;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CBReader.ViewModel;

public class MainWindowViewModel : INotifyPropertyChanged, IFileDragDropTarget
{
    private readonly ComicBookService _comicBookService = new ComicBookService();
    public ObservableCollection<ComicBook> ComicBooks { get; } = new ObservableCollection<ComicBook>();     // list of ComicBook Controls
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
    public ICommand HandleDragAndDrop { get; set; }
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
        HandleDragAndDrop = new RelayCommand(DragAndDrop, CanDragAndDrop);
        
    }



    #region Commands logic
    private bool CanDragAndDrop(object obj)
    {
        // if file extension is:
        // .cbr
        // .cba
        // .rar
        // .zip
        // .7z
        // .pdf in the foreseeable future
        throw new NotImplementedException();
    }

    private void DragAndDrop(object obj)
    {
        throw new NotImplementedException();
    }
    private bool CanOpenComicBook(object obj)
    {
        return obj is ComicBook;
    }

    // REDO THIS INTO FACTORY/SERVICE LATER!
    private void OpenComicBook(object obj)
    {
        if (obj is ComicBook comic)
        {
            var vm = new ComicBookViewModel(_comicBookService);
            vm.LoadComicBookFromArchiveToMemory(comic);

            ComicBookView comicBookView = new ComicBookView
            {
                DataContext = vm        // set here, not needed in the ComicBookView Data Context
            };

            comicBookView.Show();
        }
    }
    #endregion


    #region General Methods
    void FillComicBooks()
    {
        ComicBooks.Add(new ComicBook("The Walking Dead", "asd"));
        ComicBooks.Add(new ComicBook("Batman", "Asd"));
        ComicBooks.Add(new ComicBook("Spiderman", "asdasdasd"));

    }
    #endregion

    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)     // CallerMemberName so the method can be called without property's name
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));      // if property isn't null
    }
    #endregion

    #region IFileDragDrop
    public void OnFileDrop(string[] filepaths)
    {
        foreach (var file in filepaths)
        {
            string filename = Path.GetFileNameWithoutExtension(file);
            string extension = Path.GetExtension(file).ToLower();
            // move/copy from archivePath to ComicBook Library Path set by the user.
            string archivePath = file;      // file is a path already

            switch(extension)
            {
                // all the basic extensions
                case ".cbr":
                case ".cbz":
                case ".rar":
                case ".zip":
                case ".7z":
                    // Get file name, path 
                    var newComicBook = _comicBookService.GetComicBookData(filename, archivePath, ComicBooks);       // gets name and the path
                    _comicBookService.GetComicBookCover(newComicBook);  // creates and saves the cover + path
                    ComicBooks.Add(newComicBook);       // adds a full comic to the list
                    break;
                default:
                    MessageBox.Show("Unsupported file!");
                    break;
            }
        }
        
    }
    #endregion

}
