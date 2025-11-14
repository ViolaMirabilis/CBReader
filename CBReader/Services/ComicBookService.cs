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
    
    public List<BitmapImage> LoadComicBookToMemory(ComicBook comic)
    {
        List<BitmapImage> comicBookPages = new List<BitmapImage>();
        
        // @See https://github.com/adamhathcock/sharpcompress/blob/master/USAGE.md
        using (Stream stream = File.OpenRead(comic.ArchivePath))
        using (var reader = ReaderFactory.Open(stream))
        {
            while (reader.MoveToNextEntry())        // Goes into the all the files
            {
                if (!reader.Entry.IsDirectory)      // If the file isn't a folder, it runs the code below.
                {
                    using (var entryStream = reader.OpenEntryStream())
                    {
                        // Cannot use StreamReader, as it reads raw bytes (not suitable for images)
                        // @See https://stackoverflow.com/questions/5346727/convert-memory-stream-to-bitmapimage
                        byte[] data;        // The image data needs to be in an array first
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

    public string GetComicBookCover(ComicBook comic)       // Saves the first page as a .png
    {
        using (Stream stream = File.OpenRead(comic.ArchivePath))
        using (var reader = ReaderFactory.Open(stream))
        {
            while (reader.MoveToNextEntry())        // Goes into the all the files in a folder
            {
                if (!reader.Entry.IsDirectory)      // If the file isn't a folder, it runs the code below.
                {
                    using (var entryStream = reader.OpenEntryStream())
                    {
                        byte[] data;        // placeholder to which the data is copied
                        using (var ms = new MemoryStream())
                        {
                            entryStream.CopyTo(ms);
                            data = ms.ToArray();        // here
                        }

                        var bitmap = new BitmapImage();     // new comic book cover
                        using (var ms2 = new MemoryStream(data))
                        {
                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;        // Important
                            bitmap.StreamSource = ms2;
                            bitmap.EndInit();
                            bitmap.Freeze();

                        }

                        string safeTitle = string.Join("_", comic.Title.Split(Path.GetInvalidFileNameChars()));     // if characters are in incorrect format, "_" is used instead.
                        // directory for the covers/thumbnails
                        string comicBookCoversPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Thumbnails");
                        if (!Directory.Exists(comicBookCoversPath))
                        {
                            Directory.CreateDirectory(comicBookCoversPath);
                        }

                        // full path to the folder + cover 
                        string outputPath = Path.Combine(comicBookCoversPath, $"{safeTitle}_COVER.png");


                        try
                        { 
                            // make sure the image doesn't stay in memory
                            using (var fileStream = new FileStream(outputPath, FileMode.Create))
                            {
                                PngBitmapEncoder encoder = new PngBitmapEncoder();
                                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                                encoder.Save(fileStream);
                            }

                            comic.CoverPath = outputPath;       // assigns the path
                            return outputPath;
                        }
                        catch (System.IO.IOException ex)
                        {
                            MessageBox.Show(ex.Message);
                            break;
                        }
                        
                    } 
                }
            }
        }

        return string.Empty;

    }
    public ComicBook GetComicBookData(string name, string archivePath, ObservableCollection<ComicBook> comicBooks)
    {

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

        }
        
        return new ComicBook(name, archivePath);
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
                    var newComicBook = GetComicBookData(filename, archivePath, comicBooks);       // Gets name and the path. Returns a comic book.
                    AppendComicNameIfExists(newComicBook, comicBooks);
                    GetComicBookCover(newComicBook);  // creates and saves the cover + path
                    comicBooks.Add(newComicBook);       // adds a full comic to the list        // adding it to the memory takes some space.
                    break;
                default:
                    MessageBox.Show("Unsupported file!");   // shouldn't be here, just temporary.
                    break;
            }
        }
    }
    
    public void LoadComicsFromFolder(string folderPath, ObservableCollection<ComicBook> comicBooks)
    {
        string[] comics = Directory.GetFiles(folderPath);
        LoadComicsBase(comics, comicBooks);
    }

    public void LoadComicsFromDragAndDrop(string[] filepaths, ObservableCollection<ComicBook> comicBooks)
    {
        LoadComicsBase(filepaths, comicBooks);
    }
    public void LoadCoverFromFolder(string coverPath)
    {
        // TO DO
    }
}
