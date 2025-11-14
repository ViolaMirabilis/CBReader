using CBReader.Model;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace CBReader.Interfaces;

public interface IComicBookService
{
    public ComicBook CreateComicBook(string name, string archivePath);
    public List<BitmapImage> LoadComicBookToMemory(ComicBook comicbook);
    public string GetComicBookCover(ComicBook comicbook);
    //public ComicBook GetComicBookData(string name, string archivePath, ObservableCollection<ComicBook> comicBooks);
    public void LoadComicsFromFolder(string folderPath, ObservableCollection<ComicBook> ComicBooks);


}
