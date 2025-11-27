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

        int minIndex = currentPageIndex - half;      // e.g. index 120 - 3 = 117, so two pages to the left
        int maxIndex = currentPageIndex + half;      // e.g. 120 + 3 = 123, so indexes 117 - 123 are fully loaded in memory

        foreach (var page in _loadedPages.Keys.ToList())        // Key is the PAGE INDEX
        {
            if (page < minIndex || page > maxIndex)
            {
                _loadedPages.Remove(page);      // removes everything below min and above max
            }
        }
    }


}
