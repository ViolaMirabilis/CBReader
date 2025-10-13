using CBReader.Model;
using System.Collections.ObjectModel;
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
            ComicBooks.Add(new ComicBook(1, "Bathman", 25));
            ComicBooks.Add(new ComicBook(2, "Spoderman", 177));
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // opens a window where the user sets a folder with comic books
        }
    }
}