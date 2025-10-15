using CBReader.Model;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
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
        public ObservableCollection<ComicBook> ComicBooks { get; } = new ObservableCollection<ComicBook>();
        private string _comicBooksPath = "";
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            FillComicBooks();

            if (ComicBooks.Count <= 0) 
            {
                lblSetComicBookFolder.Visibility = Visibility.Visible;
            }
            else
            {
                lblSetComicBookFolder.Visibility = Visibility.Collapsed;
            }

        }

        
        void FillComicBooks()
        {
            ComicBooks.Add(new ComicBook(0, "The Walking Dead", 20));
            ComicBooks.Add(new ComicBook(1, "Bathmanaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", 25));
            ComicBooks.Add(new ComicBook(2, "Spoderman", 177));
        }

        // Sets a path to a folder of comic books. Supports a double click in the ListBox, where all the comic books covers' are shown.
        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!string.IsNullOrEmpty(_comicBooksPath))     // doesn't do anything if the path is already set.
                return;

            var dlg = new OpenFolderDialog();
            if (dlg.ShowDialog() != true) return;       // If nothing is selected

            _comicBooksPath = dlg.FolderName;

            MessageBox.Show($"Path successfuly set to: {_comicBooksPath}!");
        }
    }
}