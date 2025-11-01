using CBReader.Model;
using SharpCompress.Readers;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CBReader.Services;

public class ComicBookService
{
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

    public string GetComicBookCover(ComicBook comic)       // saves the first page as a .png
    {
        using (Stream stream = File.OpenRead(comic.ArchivePath))
        using (var reader = ReaderFactory.Open(stream))
        {
            while (reader.MoveToNextEntry())        // Goes into the all the files
            {
                if (!reader.Entry.IsDirectory)      // If the file isn't a folder, it runs the code below.
                {
                    using (var entryStream = reader.OpenEntryStream())
                    {
                        byte[] data;
                        using (var ms = new MemoryStream())
                        {
                            entryStream.CopyTo(ms);
                            data = ms.ToArray();
                        }

                        var bitmap = new BitmapImage();
                        using (var ms2 = new MemoryStream(data))
                        {
                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;        // Important
                            bitmap.StreamSource = ms2;
                            bitmap.EndInit();
                            bitmap.Freeze();
                        }

                        string safeTitle = string.Join("_", comic.Title.Split(Path.GetInvalidFileNameChars()));
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
    public ComicBook GetComicBookData(string name, string archivePath)
    {
        return new ComicBook(name, archivePath);
    }
}
