using CBReader.Interfaces;
using CBReader.Model;
using CBReader.Services;
using CBReader.View;
using CBReader.ViewModel;
using Microsoft.Win32;
using SharpCompress.Common;
using SharpCompress.Readers;
using SharpCompress.Writers;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CBReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly FileDialogService _fileDialogService;
        private readonly ComicArchiveReaderService _comicArchiveReaderService;
        private readonly FilePersistanceService _filePersistanceService;

        // I dont understand this one. It's a placeholder for now, because MainWindow will hold a reference to a new view
        public MainWindow() : this(new FileDialogService(), new ComicArchiveReaderService(), new FilePersistanceService())
        {

        }
        
        public MainWindow(FileDialogService fileDialogService, ComicArchiveReaderService comicArchiveReaderService, FilePersistanceService filePersistanceService)
        {
            InitializeComponent();
            _fileDialogService = fileDialogService;
            _comicArchiveReaderService = comicArchiveReaderService;
            _filePersistanceService = filePersistanceService;
            DataContext = new MainWindowViewModel(_fileDialogService, _comicArchiveReaderService, _filePersistanceService);

            FilePersistanceService asd = new FilePersistanceService();
            asd.InitialiseConfigFile();

        }

        // integrated with MainWindowViewModel
        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var listBox = sender as ListBox;        // casts the sender to ListBox
            var selectedItem = listBox?.SelectedItem as ComicBook;   // casts the selected item as ComicBook
            if (DataContext is MainWindowViewModel vm)      // executes if the selected item is ComicBook and Not null. Otherwise, it opens up the dialog.
            {
                if (selectedItem != null)
                {
                    vm.ShowComicBookCommand.Execute(selectedItem);
                    return;
                }
                
                if (vm.ComicBooks.Count == 0)
                {
                    vm.SelectFolderCommand.Execute(null);
                }
                
            }
        }

        // Topbar
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)        // MenuItem as the context menu defined in MainWindow.xaml. Cast is needed to access .DataContext properties
            {
                // to do later on
            }
        }
    }
}