using CBReader.Interfaces;
using CBReader.Model;
using SharpCompress.Readers;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

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

    public List<ComicBookContent> GetComicBookArchiveContent(ComicBook comic)
    {
        var contentList = new List<ComicBookContent>();
        using (Stream stream = File.OpenRead(comic.ArchivePath))
        using (var reader = ReaderFactory.Open(stream))    //Sharp Compress
        {
            int tmpIndex = 0;

            while (reader.MoveToNextEntry())
            {
                if(!reader.Entry.IsDirectory)
                {
                    contentList.Add(new ComicBookContent
                    {
                        FileName = reader.Entry.Key,        // reader.Entry.Key reads the name of the file
                        Index = tmpIndex                    // self explanatory
                    });
                }
                tmpIndex++;
            }
        }

        return contentList;

    }
    public BitmapImage LoadPage(ComicBook comic, int pageIndex)     // Loads one page only
    {
        // @See https://github.com/adamhathcock/sharpcompress/blob/master/USAGE.md
        using (Stream stream = File.OpenRead(comic.ArchivePath))
        using (var reader = ReaderFactory.Open(stream))     //SharpCompress library
        {
            int tmpIndex = 0;

            while (reader.MoveToNextEntry())        // Goes into the all the files
            {

                if (reader.Entry.IsDirectory)       // Making sure that only images get loaded in (so the index of the page is correct and the folder won't be included!)
                {
                    continue;
                }

                if (tmpIndex == pageIndex)
                {
                    using (var entryStream = reader.OpenEntryStream())
                    using (var ms = new MemoryStream())
                    {
                        // Cannot use StreamReader, as it reads raw bytes (not suitable for images)
                        // @See https://stackoverflow.com/questions/5346727/convert-memory-stream-to-bitmapimage
                        byte[] data;        // The image data needs to be in an array first
                        entryStream.CopyTo(ms);
                        //ms.Position = 0;  // commented for now, idk if it's necessary.


                        /*using (var ms = new MemoryStream())
                        {
                            entryStream.CopyTo(ms);     // Reads from one stream, writes to another (ms)
                            data = ms.ToArray();        // then the data array gets the memory stream
                        }*/

                        // converting from bytes to bitmap
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;        // Important
                        bitmap.StreamSource = ms;
                        bitmap.EndInit();
                        bitmap.Freeze();
                        return bitmap;

                        //comicBookPages.Add(bitmap);     // Adds the converted bytes to the list of Bitmaps
                    }
                }
            }
        }
        return null;
    }

}
