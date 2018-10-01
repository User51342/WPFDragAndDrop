using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfDragAndDrop
{
    public class DragDropAnimation : IDisposable
    {
        #region Fields
        private DispatcherTimer myTimer;
        private Canvas myCanvas;
        private Grid myRootGrid;
        private Window myWindow;
        private Image myImage;
        #endregion

        #region Construction / Initalization / Deconstruction
        public DragDropAnimation(Grid parentElement)
        {
            myRootGrid = parentElement;
            GetWindow();
            AddMouseHandler();
            myTimer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 5)
            };
            myTimer.Tick += OnTick;
        }
        #endregion

        #region Public implementation
        public void CreateDragAndDropEffect(Rectangle dragDropObject)
        {
            myCanvas = new Canvas
            {
                Background = new SolidColorBrush(Color.FromScRgb(0, 0, 0, 0)),
                Width = myRootGrid.Width,
                Height = myRootGrid.Height,
                IsHitTestVisible = false
            };
            myRootGrid.Children.Add(myCanvas);
            RenderTargetBitmap bmpCopied = CreateImageCopy(dragDropObject);
            myImage = new Image
            {
                Source = bmpCopied,
                Width = dragDropObject.Width,
                Height = dragDropObject.Height
            };
            myCanvas.Children.Add(myImage);
            myTimer.Start();
        }

        public void Dispose()
        {
            if (myCanvas != null)
            {
                myCanvas.Children.Clear();
                myCanvas = null;
            }
            if (myImage != null)
            {
                myImage = null;
            }
        }
        #endregion

        #region Private implementation
        private RenderTargetBitmap CreateImageCopy(Rectangle dragDropObject)
        {
            double width = dragDropObject.ActualWidth;
            double height = dragDropObject.ActualHeight;
            RenderTargetBitmap bmpCopied = new RenderTargetBitmap((int)Math.Round(width), (int)Math.Round(height), 96, 96, PixelFormats.Default);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(dragDropObject);
                dc.DrawRectangle(vb, null, new Rect(new Point(), new Size(width, height)));
            }
            bmpCopied.Render(dv);
            return bmpCopied;
        }

        internal void Clear()
        {
            if (myCanvas != null)
            {
                myCanvas.Children.Clear();
                myCanvas = null;
                myImage = null;
            }
        }

        private void AddMouseHandler()
        {
            myWindow.ContentRendered += OnContentRendered;
        }

        private void MoveImage(double x, double y)
        {
            if (myImage != null)
            {
                Canvas.SetLeft(myImage, x);
                Canvas.SetTop(myImage, y);
            }
        }

        private void GetWindow()
        {
            FrameworkElement element = myRootGrid;
            while (element != null && element.GetType().BaseType != typeof(Window))
            {
                element = element.Parent as FrameworkElement;
            }
            if (element != null)
            {
                myWindow = (Window)element;
            }
        }

        private void OnContentRendered(object sender, EventArgs e)
        {
            foreach (UIElement element in myRootGrid.Children)
            {
                if (element.GetType() == typeof(Rectangle))
                {
                    Rectangle rec = element as Rectangle;
                    if(rec.AllowDrop)
                    {
                        rec.Drop += OnTargetDrop;
                    }
                    rec.MouseDown += OnMouseDown;
                    rec.MouseLeftButtonDown += OnMouseLeftButtonDown;
                }
            }
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var dragObject = sender as Rectangle;
            var dragDropEffects = DragDrop.DoDragDrop(dragObject, dragObject.Fill, DragDropEffects.Move);
            Clear();
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var rec = e.OriginalSource as Rectangle;
            if (rec != null)
            {
                CreateDragAndDropEffect(rec);
            }
        }

        private void OnTick(object sender, EventArgs e)
        {
            var point = MouseUtilities.CorrectGetPosition(myWindow);
            MoveImage(point.X, point.Y);
        }

        private void OnTargetDrop(object sender, DragEventArgs e)
        {
            myTimer.Stop();
            var target = sender as Rectangle;
            target.Fill = (SolidColorBrush)e.Data.GetData(typeof(SolidColorBrush));
        }
        #endregion
    }
}
