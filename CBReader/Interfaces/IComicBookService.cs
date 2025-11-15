using CBReader.Model;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace CBReader.Interfaces;

public interface IComicBookService
{
    public ComicBook CreateComicBook(string name, string archivePath);
    public List<BitmapImage> LoadComicBookToMemory(ComicBook comicbook);
    public void AppendComicNameIfExists(ComicBook comicBook, ObservableCollection<ComicBook> comicBooks);

}
