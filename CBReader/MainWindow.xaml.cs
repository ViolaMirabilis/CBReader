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
            var selectedItem = listBox?.SelectedItem as ComicBook;   // casts the selected item as ComicBook
            if (DataContext is MainWindowViewModel vm && selectedItem != null)
            {
                vm.ShowComicBookCommand.Execute(selectedItem);
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