using CBReader.Model;
using System.Windows.Media.Imaging;

namespace CBReader.Services;

public class LazyLoadService
{
    private readonly ComicBook _comic;
    private readonly List<ComicBookContent> _pagesInArchive;
    private readonly Dictionary<int, BitmapImage> _loadedPages = new Dictionary<int, BitmapImage>();         // BitmapImage - currently loaded image, int = its index.
    private readonly ComicArchiveReaderService _archiveReader = new ComicArchiveReaderService();
    
    private int _currentlyLoadedPages = 6;      // 6 at the start, can be less if the user is going to the last page 

    public LazyLoadService(ComicBook comic, List<ComicBookContent> pagesInArchive, ComicArchiveReaderService archiveReader)
    {
        _comic = comic;
        _pagesInArchive = pagesInArchive;
        _archiveReader = archiveReader;
    }

    public BitmapImage GetPage(int pageIndex)
    {
        if (pageIndex < 0 || pageIndex >= _pagesInArchive.Count)        // less than 0 or higher than .jpg count in the archive
            return null;

        if (_loadedPages.TryGetValue(pageIndex, out BitmapImage image))     // if valid index, return the image (if between 0 and archive count)
            return image;

        image = _archiveReader.LoadPage(_comic, pageIndex);     // Loads the image into memory
        _loadedPages[pageIndex] = image;                        // FIRST ADDITION TO THE DICTIONARY. page index and the image are in one dictionary now

        UnloadPagesFromMemory(pageIndex);

        return image;
    }

    /// <summary>
    /// removes REFERENCE to the dictionary. If there is no reference, it's picked up by the garbage collector, so no memory leaks.
    /// </summary>
    /// <param name="currentPageIndex"></param>
    public void UnloadPagesFromMemory(int currentPageIndex)
    {
        int half = _currentlyLoadedPages / 2;       // 6/2 = 3

        int min = currentPageIndex - half;      // e.g. 5 - 3 = 2, so two pages to the left
        int max = currentPageIndex + half;      // e.g. 5 + 3 = 8, 8 pages loaded in total
        // min and max create a range = between 2 and 8, so pages: 2,3,4,5,6,7,8 can be loaded in at once.

        foreach (var page in _loadedPages.Keys.ToList())        // Key is the PAGE INDEX
        {
            if (page < min || page > max)
            {
                _loadedPages.Remove(page);      // removes everything below min and above max
            }
        }
    }


}
