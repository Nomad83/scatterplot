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

namespace SimpleScatterPlotting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Point lastMousePosition;

        public MainWindow()
        {
            InitializeComponent();

            this.mainCanvas.Background = Brushes.Transparent;
        }

        private void mainCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Point pos = e.GetPosition(sender as Canvas);
            Point pixelPos = this.visualhost.canvasToPixel(pos);

            this.coordinateLbl.Content = this.visualhost.pixelToPoint(pixelPos);

            if (this.panTgBtn.IsChecked == true && e.LeftButton == MouseButtonState.Pressed)
            {
                Vector offset = pos - lastMousePosition;
                this.visualhost.calculatePan(offset);
                this.visualhost.refresh();
            }

            lastMousePosition = pos;
        }

        private void mainCanvas_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (this.zoomTgBtn.IsChecked == true)
            {
                this.visualhost.calculateZoom(e.Delta);
                this.visualhost.refresh();
            }
        }

        private void loadBtn_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (MyFileDialog.ShowDialog() == true)
            {
                this.visualhost.readDataset(MyFileDialog.filename);
                this.visualhost.refresh();
            }
        }

        private void backgroundCbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.backgroundCbx.SelectedIndex < this.backgroundCbx.Items.Count)
            {
                if (this.backgroundCbx.SelectedIndex == 0)
                {
                    this.mainCanvas.Background = Brushes.LightGray;
                }
                else
                {
                    this.mainCanvas.Background = Brushes.White;
                }
            }
        }

        private void foregroundCbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.foregroundCbx.SelectedIndex < this.foregroundCbx.Items.Count)
            {
                if (this.foregroundCbx.SelectedIndex == 0)
                {
                    this.visualhost.setScatterPlotColor(MyVisualHost.ScatterPlotColor.Red);
                }
                else if (this.foregroundCbx.SelectedIndex == 1)
                {
                    this.visualhost.setScatterPlotColor(MyVisualHost.ScatterPlotColor.Green);
                }
                else
                {
                    this.visualhost.setScatterPlotColor(MyVisualHost.ScatterPlotColor.Blue);
                }

                this.visualhost.refresh();
            }
        }
    }
}
