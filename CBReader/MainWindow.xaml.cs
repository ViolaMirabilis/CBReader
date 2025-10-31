using CBReader.Model;
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
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();

        }

        // integrated with MainWindowViewModel
        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var listBox = sender as ListBox;        // casts the sender to ListBox
            var selectedItem = listBox.SelectedItem as ComicBook;   // casts the selected item as ComicBook
            if (DataContext is MainWindowViewModel vm && selectedItem != null)
            {
                vm.ShowComicBookCommand.Execute(selectedItem);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)        // MenuItem as the context menu defined in MainWindow.xaml
            {
                if (menuItem.DataContext is ComicBook comicBook)
                {
                    if (comicBook.IsFavourite == false)
                    {
                        menuItem.Header = "Add to favourite";
                        comicBook.IsFavourite = true;
                        MessageBox.Show("Added to favourites!");
                    }
                    else
                    {
                        menuItem.Header = "Delete from favourites";
                        comicBook.IsFavourite = false;
                        MessageBox.Show("Added to favourites!");
                    }
                        
                }
            }
        }
    }
}