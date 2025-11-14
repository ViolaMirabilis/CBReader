using CBReader.Model;
using System.Collections.ObjectModel;
using System.IO;

namespace CBReader.Interfaces;

public interface IComicArchiveReaderService
{
    public void LoadComicsBase(string[] filepaths, ObservableCollection<ComicBook> comicBooks);
    public void LoadFromFolder(string folderPath, ObservableCollection<ComicBook> comicBooks);
    public void LoadFromDragAndDrop(string[] filepaths, ObservableCollection<ComicBook> comicBooks);
}
