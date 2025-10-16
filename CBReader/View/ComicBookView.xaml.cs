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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CBReader.View
{
    /// <summary>
    /// Interaction logic for ComicBookView.xaml
    /// </summary>
    public partial class ComicBookView : Window
    {
        private readonly DispatcherTimer _mouseHoverDelay;
        public ComicBookView()
        {
            InitializeComponent();

            _mouseHoverDelay = new DispatcherTimer();                       // creates a new timer on initialisation
            _mouseHoverDelay.Interval = TimeSpan.FromMilliseconds(1000);     // sets the interval to 1000ms (1 sec)
            _mouseHoverDelay.Tick += MouseHoverDelay_Tick;                  // ticks 
        }

        /// <summary>
        /// If the mouse moves, the timer starts and the UI shows right away. After the 1000ms, it ticks (stops the timer) and hids the UI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        // This event fires when the user moves the mouse on the screen, making the UI visible. It Starts the timer (interval), and after the interval, it stops and hides the UI.
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            OverlapGUIGrid.Visibility = Visibility.Visible;

            _mouseHoverDelay.Stop();
            _mouseHoverDelay.Start();
        }

        private void MouseHoverDelay_Tick(object? sender, EventArgs e)
        {
            _mouseHoverDelay.Stop();
            OverlapGUIGrid.Visibility = Visibility.Hidden;
        }
    }
}
