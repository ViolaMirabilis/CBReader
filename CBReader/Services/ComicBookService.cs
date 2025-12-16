using CBReader.Interfaces;
using CBReader.Model;
using SharpCompress.Readers;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace CBReader.Services;

public class ComicBookService : IComicBookService
{
    public ComicBook CreateComicBook(string name, string archivePath)
    {
        return new ComicBook(name, archivePath);
    }
    // loads teh entire comic book to memory. Obsolete now, uses too much memory, so it's actually loaded in the LazyLoadService, loading x pages at once, to reduce memory usage.
    public List<BitmapImage> LoadComicBookToMemory(ComicBook comic)
    {
        List<BitmapImage> comicBookPages = new List<BitmapImage>();
        
        // @See https://github.com/adamhathcock/sharpcompress/blob/master/USAGE.md
        using (Stream stream = File.OpenRead(comic.ArchivePath))
        using (var reader = ReaderFactory.Open(stream))
        {
            while (reader.MoveToNextEntry())        // Goes into the all the files
            {
                if (!reader.Entry.IsDirectory)      // If the file isn't a folder(dir), it saves the file (bitmap) to a stream
                {
                    using (var entryStream = reader.OpenEntryStream())
                    {
                        // Cannot use StreamReader, as it reads raw bytes (not suitable for images)
                        // @See https://stackoverflow.com/questions/5346727/convert-memory-stream-to-bitmapimage
                        byte[] data;        // The image data needs to be in an array of bytes first
                        using (var ms = new MemoryStream())
                        {
                            entryStream.CopyTo(ms);     // Reads from one stream, writes to another (ms)
                            data = ms.ToArray();        // then the data array gets the memory stream
                        }

                        // converting from bytes to bitmap
                        var bitmap = new BitmapImage();
                        using (var ms2 = new MemoryStream(data))
                        {
                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;        // Important
                            bitmap.StreamSource = ms2;
                            bitmap.EndInit();
                            bitmap.Freeze();
                        }

                        comicBookPages.Add(bitmap);     // Adds the converted bytes to the list of Bitmaps
                    }
                }
            }
        }

        return comicBookPages;
    }

    public void AppendComicNameIfExists(ComicBook comicBook, ObservableCollection<ComicBook> comicBooks)
    {
        string name = comicBook.Title;
        if (comicBooks.Any(c => c.Title == name))       // if a name exists, add 1
        {
            int counter = 1;
            string newName;
            do
            {
                // do some tinkering here, so it's always Book (1), Book (2), etc. and not Book (1) (2)
                newName = $"{name} ({counter})";     // e.g. TWD (1);
                name = newName;
                counter++;
            } while (comicBooks.Any(c => c.Title == name));     // infinite loop

            comicBook.Title = newName;
        }
    }
    public void LoadCoverFromFolder(string coverPath)
    {
        // TO DO
    }
}
