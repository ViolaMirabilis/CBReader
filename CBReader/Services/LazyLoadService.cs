using CBReader.Model;
using System.Windows.Media.Imaging;

namespace CBReader.Services;

public class LazyLoadService
{
    private readonly ComicBook _comic;
    private readonly List<ComicBookContent> _pagesInArchive;
    private readonly Dictionary<int, BitmapImage> LoadedPages = new Dictionary<int, BitmapImage>();         // BitmapImage - currently loaded image, int = its index.

    private int _currentlyLoadedPages = 6;      // 6 at the start, can be less if the user is going to the last page 

    private readonly ComicArchiveReaderService _archiveReader = new ComicArchiveReaderService();
}
