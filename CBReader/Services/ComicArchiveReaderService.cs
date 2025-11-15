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
    
    public bool IsExtensionSupported(string extension)
    {
        return extension switch
        {
            ".cbr" => true,
            ".cba" => true,
            ".cbz" => true,
            ".rar" => true,
            ".zip" => true,
            ".7z" => true,
            _ => false
        };
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
                    // CAN BE SIMPLIFIED LATER ON, AS IT HAS BEEN DONE WITH LAZY LOADING!!!!!!!!!!!!!!!!
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

    public void LoadFromFolder(string folderPath, ObservableCollection<ComicBook> comicBooks)
    {
        string[] comics = Directory.GetFiles(folderPath);
        //LoadComicsBase(comics, comicBooks);
    }

    public void LoadFromDragAndDrop(string[] filepaths, ObservableCollection<ComicBook> comicBooks)
    {
        //LoadComicsBase(filepaths, comicBooks);
    }

    /// <summary>
    /// Reads the content of the archive and stores names of the files (xyz.jpg, xyz1.png, etc.) and indexes to them.
    /// A simple list, so it doesn't use much resources and can be passed further on to the lazy loading method.
    /// </summary>
    /// <param name="comic"></param>
    /// <returns></returns>
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
                        Index = contentList.Count                    // self explanatory
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
                        //byte[] data;        // The image data needs to be in an array first
                        entryStream.CopyTo(ms);
                        ms.Position = 0;  // otherwise the position would be at the very end


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
                        bitmap.Freeze();        // Important as well
                        return bitmap;

                        //comicBookPages.Add(bitmap);     // Adds the converted bytes to the list of Bitmaps
                    }
                }
                tmpIndex++;
            }
        }
        return null;
    }

}
