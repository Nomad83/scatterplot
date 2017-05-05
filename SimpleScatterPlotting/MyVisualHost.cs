using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SimpleScatterPlotting
{

    public class MyVisualHost : FrameworkElement
    {
        public enum ScatterPlotColor
        {
            Red,
            Green,
            Blue
        }

        private VisualCollection _children;
        private Point minVal;
        private Point maxVal;
        private double widthVal;
        private double heightVal;
        private uint dsSize;
        private Point[] dsCollection;
        private Rect viewRect;
        private Rect windowRect;
        private ScatterPlotColor scatterPlotColor;

        public MyVisualHost()
        {
            minVal = new Point(-100, 0);
            maxVal = new Point(900, 600);
            widthVal = maxVal.X - minVal.X;
            heightVal = maxVal.Y - minVal.Y;
            scatterPlotColor = ScatterPlotColor.Blue;
            _children = new VisualCollection(this);
            viewRect = new Rect(new Point(0, 0), new Size(800, 600));
            windowRect = new Rect(new Point(0, 0), new Size(800, 600));
        }
        public ScatterPlotColor getScatterPlotColor()
        {
            return scatterPlotColor;
        }

        public void setScatterPlotColor(ScatterPlotColor color)
        {
            scatterPlotColor = color;
        }

        public void readDataset(string filename)
        {
            string line;

            System.IO.StreamReader sr = new System.IO.StreamReader(filename);
            
            if ((line = sr.ReadLine()) != null)
            {
                string[] values = line.Split(':');
                dsSize = UInt32.Parse(values[1]);
            }

            dsCollection = new Point[dsSize];

            uint i = 0;
            while ((line = sr.ReadLine()) != null && i < dsCollection.Length)
            {
                string[] values = line.Split(',');
                dsCollection[i].X = Double.Parse(values[0]);
                dsCollection[i].Y = Double.Parse(values[1]);
                i++;
            }

            if (i < dsCollection.Length)
            {
                dsSize = i;
            }

            //MessageBox.Show(dsCollection[0].Y.ToString());
            sr.Close();
        }

        public void calculateZoom(int delta)
        {
            double scale = Math.Pow(2, (-delta) / 3.0 / Mouse.MouseWheelDeltaForOneLine);
            var x = ((scale - 1) / 2) * viewRect.Width;
            var y = ((scale - 1) / 2) * viewRect.Height;
            viewRect.Inflate(x, y);
        }

        public void calculatePan(Vector offset)
        {
            double xOffset = viewRect.Width / windowRect.Width * offset.X;
            double yOffset = viewRect.Height / windowRect.Height * offset.Y;
            Vector e = new Vector(xOffset, yOffset);
            viewRect.Offset(e);
        }

        public bool isPointInViewRect(Point p)
        {
            return (p.X > viewRect.Left && p.X < viewRect.Right && p.Y > viewRect.Top && p.X < viewRect.Bottom);
        }

        public Point pointToPixel(Point p)
        {
            double x = (p.X - minVal.X) / widthVal * windowRect.Width;
            double y = (1 - (p.Y - minVal.Y) / heightVal) * windowRect.Height;
            return new Point(x, y);
        }

        public Point pixelToPoint(Point p)
        {
            double x = p.X / windowRect.Width * widthVal + minVal.X;
            double y = (1 - (p.Y / windowRect.Height)) * heightVal + minVal.Y;
            return new Point(x, y);
        }

        public Point canvasToPixel(Point p)
        {
            double x = p.X / windowRect.Width * viewRect.Width + viewRect.Left;
            double y = p.Y / windowRect.Height * viewRect.Height + viewRect.Top;
            return new Point(x, y);
        }

        public void refresh()
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext dc = drawingVisual.RenderOpen();

            // draw x-axis
            dc.DrawLine(new Pen(Brushes.DarkGray, 2), new Point(20.0, 550.0), new Point(780.0, 550.0));
            for (int i = 150; i < 780; i+=100 )
            {
                double xMarkerPixel = i / windowRect.Width * viewRect.Width + viewRect.Left;
                double xMarker = Math.Round(xMarkerPixel / windowRect.Width * widthVal + minVal.X, 1);
                FormattedText formattedText = new FormattedText(
                    xMarker.ToString(),
                    CultureInfo.GetCultureInfo("en-us"),
                    FlowDirection.LeftToRight,
                    new Typeface("Verdana"),
                    11,
                    Brushes.DarkGray);

                dc.DrawLine(new Pen(Brushes.DarkGray, 1), new Point(i, 540.0), new Point(i, 560.0));
                dc.DrawText(formattedText, new Point(i+5, 555.0));
            }

            // draw y-axis
            dc.DrawLine(new Pen(Brushes.DarkGray, 2), new Point(50.0, 20.0), new Point(50.0, 580.0));
            for (int j = 450; j > 20; j -= 100)
            {
                double yMarkerPixel = j / windowRect.Height * viewRect.Height + viewRect.Top;
                double yMarker = Math.Round((1 - (yMarkerPixel / windowRect.Height)) * heightVal + minVal.Y, 1);
                FormattedText formattedText = new FormattedText(
                    yMarker.ToString(),
                    CultureInfo.GetCultureInfo("en-us"),
                    FlowDirection.LeftToRight,
                    new Typeface("Verdana"),
                    11,
                    Brushes.DarkGray);

                dc.DrawLine(new Pen(Brushes.DarkGray, 1), new Point(40, j), new Point(60, j));
                dc.DrawText(formattedText, new Point(20, j + 5));
            }

            // draw scatter plots
            Brush scatterBrush = Brushes.Blue;
            if (scatterPlotColor == ScatterPlotColor.Red)
            {
                scatterBrush = Brushes.Red;
            }
            else if (scatterPlotColor == ScatterPlotColor.Green)
            {
                scatterBrush = Brushes.Green;
            }


            for (int i = 0; i < dsSize; i++)
            {
                if (i >= dsCollection.Length)
                {
                    break;
                }

                Point p = pointToPixel(dsCollection[i]);

                if (isPointInViewRect(p))
                {                 
                    double x = (p.X - viewRect.Left) / viewRect.Width * windowRect.Width;
                    double y = (p.Y - viewRect.Top) / viewRect.Height * windowRect.Height;
                    Point e = new Point(x, y); 
                    dc.DrawEllipse(scatterBrush, null, e, 3, 3);
                }
            }

            // Persist the drawing content.
            dc.Close();

            _children.Clear();
            _children.Add(drawingVisual);
        }

        // Provide a required override for the VisualChildrenCount property.
        protected override int VisualChildrenCount
        {
            get { return _children.Count; }
        }

        // Provide a required override for the GetVisualChild method.
        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return _children[index];
        }
    }

}