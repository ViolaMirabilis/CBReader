using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CBReader.Model;

namespace CBReader.Controls
{
    /// <summary>
    /// Interaction logic for ComicBookControl.xaml
    /// </summary>
    public partial class ComicBookControl : UserControl
    {
        
        private bool _toggleTextDelay = false;
        public bool IsEditing = false;
        public ComicBookControl()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            _toggleTextDelay = true;
        }

        // when the context menu CLOSES, do this.
        private void ContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            // MenuItem makes _toggle TRUE. Then, when the context menu is closed, it checks if it's true. If it is, it changes the text.
            if (_toggleTextDelay && DataContext is ComicBook comicBook)
            {
                comicBook.State.IsFavourite = !comicBook.State.IsFavourite;

                FavouriteButton.Header = comicBook.State.IsFavourite ? "Remove from favourites" : "Add to favourites";
            }

            _toggleTextDelay = false;
        }

        private void Rename_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ComicBook comicBook)
            {
                IsEditing = true;
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
