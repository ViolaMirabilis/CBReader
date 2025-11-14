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
    private readonly IFileDialogService _fileDialogService;
    private readonly IComicArchiveReaderService _comicArchiveReaderService;
    private readonly ComicBookService _comicBookService = new ComicBookService();       // Created only once
    public ObservableCollection<ComicBook> ComicBooks { get; } = new ObservableCollection<ComicBook>();     // list of ComicBook Controls
    private bool _showEmptyComicBookListLabel = true;  // Initially true. However, if the ComicBook.Count != 0, it switches to false.
    public bool ShowEmptyComicBookListLabel
    {
        get { return _showEmptyComicBookListLabel;}
        set
        {
            if(_showEmptyComicBookListLabel != value)
            {
                _showEmptyComicBookListLabel = value;
                OnPropertyChanged();
            }
        }
    }

    public string? ComicBookFolderPath { get; set; }


    #region Commands declarations
    public ICommand SelectFolderCommand { get; set; }
    public ICommand ShowComicBookCommand { get; set; }
    public ICommand HandleDragAndDrop { get; set; }
    #endregion

    public MainWindowViewModel(IFileDialogService fileDialogService, IComicArchiveReaderService comicArchiveReaderService)
    {
        _fileDialogService = fileDialogService;
        _comicArchiveReaderService = comicArchiveReaderService;
        // An event, which sets the value to true if the ComicBooks.Count is equal to 0. Lambda expression btw, shorter version.
        ComicBooks.CollectionChanged += (s, e) =>
        {
            ShowEmptyComicBookListLabel = ComicBooks.Count == 0;
        };

        ShowComicBookCommand = new RelayCommand(OpenComicBook, CanOpenComicBook);
        HandleDragAndDrop = new RelayCommand(DragAndDrop, CanDragAndDrop);
        SelectFolderCommand = new RelayCommand(OpenFolder);
       
    }


    #region Commands logic
    private void OpenFolder(object obj)
    {
        string? folderPath = _fileDialogService.ChooseComicFolderPath();

        if (folderPath == null)
            return;

        ComicBookFolderPath = folderPath;

        _comicArchiveReaderService.LoadFromFolder(folderPath, ComicBooks);
    }

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
            var vm = new ComicBookViewModel(_comicBookService);     // creates a new comic book service. Might assign the field in the constructor, just like the FileDialogService.
            vm.LoadComicBookFromArchiveToMemory(comic);

            ComicBookView comicBookView = new ComicBookView
            {
                DataContext = vm        // set here, not needed in the ComicBookView Data Context
            };

            comicBookView.Show();
        }
    }
    #endregion

    #region IFileDragDrop
    public void OnFileDrop(string[] filepaths)      // filepaths needed because of the Helper class.
    {
        _comicArchiveReaderService.LoadFromDragAndDrop(filepaths, ComicBooks); 

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
