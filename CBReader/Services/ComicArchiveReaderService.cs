using CBReader.Interfaces;
using CBReader.Model;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace CBReader.Services;

public class ComicArchiveReaderService : IComicArchiveReaderService
{
    private readonly IComicBookService _comicBookService;
    
    public ComicArchiveReaderService(IComicBookService comicBookService)
    {
        _comicBookService = comicBookService;
    }
    public void LoadComicsBase(string[] filepaths, ObservableCollection<ComicBook> comicBooks)
    {
        foreach (var file in filepaths)
        {
            string filename = Path.GetFileNameWithoutExtension(file);
            string extension = Path.GetExtension(file).ToLower();
            string archivePath = file;      // file is a path already

            switch (extension)
            {
                case ".cbr":
                case ".cbz":
                case ".rar":
                case ".zip":
                case ".7z":
                    var comicBook = _comicBookService.CreateComicBook(filename, archivePath);
                    _comicBookService.AppendComicNameIfExists(comicBook, comicBooks);
                    _comicBookService.GetComicBookCover(comicBook);  // creates and saves the cover + path
                    comicBooks.Add(comicBook);       // adds a full comic to the list        // adding it to the memory takes some space.
                    break;
                default:
                    MessageBox.Show("Unsupported file!");   // shouldn't be here, just temporary.
                    break;
            }
        }
    }

    public void LoadFromFolder(string folderPath, ObservableCollection<ComicBook> comicBooks)
    {
        string[] comics = Directory.GetFiles(folderPath);
        LoadComicsBase(comics, comicBooks);
    }

    public void LoadFromDragAndDrop(string[] filepaths, ObservableCollection<ComicBook> comicBooks)
    {
        LoadComicsBase(filepaths, comicBooks);
    }
    
}
