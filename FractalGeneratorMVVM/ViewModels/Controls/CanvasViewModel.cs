using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FractalCore;
using System.Threading.Tasks;
using Caliburn.Micro;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;

namespace FractalGeneratorMVVM.ViewModels.Controls
{
    /// <summary>
    /// Deletgate for event call MouseOverCanvasEvent
    /// </summary>
    public delegate void MouseOverCanvas(Point Pos, double width, double height);
    /// <summary>
    /// Delegate for event call LeftClickedCanvas
    /// </summary>
    public delegate void ClickedCanvas(Point Pos, double width, double height);

    public class CanvasViewModel : Screen
    {
        #region Events
        /// <summary>
        /// Event for when the mouse cursor hovers over the canvas
        /// </summary>
        public event MouseOverCanvas? MouseOverCanvasEvent;  // Event
        /// <summary>
        /// Event for when mouse button left click down over canvas
        /// </summary>
        public event ClickedCanvas? LeftClickedCanvas;  // Event
        #endregion

        #region Fields

        private FractalImage? _image;

        private float _zoom = 1;
        private float _minScale;

        private int _zoomFactor = 16;

        private Point? _oldDragPoint;
        
        #endregion

        #region Properties

        /// <summary>
        /// Fractal Image object which is displayed on the canvas.
        /// Xaml reaches down to <see cref="FractalImage"/>'s WriteableBitmap.
        /// </summary>
        public FractalImage? Image
        {
            get { return _image; }
            set 
            { 
                _image = value;

                MinZoom = (float)Decimal.Divide((decimal)Math.Floor(Decimal.Divide(CanvasHeight, value.Height) * ZoomFactor), ZoomFactor);
                Zoom = MinZoom; 

                NotifyOfPropertyChange(() => Image);
            }
        }
        /// <summary>
        /// Scale factor on dimensions of canvas image.
        /// </summary>
        public float Zoom
        {
            get { return _zoom; }
            set 
            { 
                _zoom = value;
                NotifyOfPropertyChange(() => Zoom);
            }
        }
        /// <summary>
        /// Minimum scale factor of canvas image. Used so there is a limit to how much you can zoom out.
        /// </summary>
        public float MinZoom
        {
            get { return _minScale; }
            set { _minScale = value; }
        }
        /// <summary>
        /// Height in pixels of the canvas control
        /// </summary>
        public int CanvasHeight { get; set; }
        /// <summary>
        /// Width in pixels of the canvas control.
        /// </summary>
        public int CanvasWidth { get; set; }
        /// <summary>
        /// Image height in pixels after being scaled by <see cref="Zoom"/>
        /// </summary>
        public double ImageActualHeight
        {
            get
            {
                return Image.Height * Zoom;
            }
        }
        /// <summary>
        /// Image width in pixels after being scaled by <see cref="Zoom"/>
        /// </summary>
        public double ImageActualWidth
        {
            get
            {
                return Image.Width * Zoom;
            }
        }
        /// <summary>
        /// The divisor on the dimensions of the image when it is zoomed
        /// </summary>
        public int ZoomFactor
        {
            get { return _zoomFactor; }
            set { _zoomFactor = value; }
        }
        public float ZoomQuantity
        {
            get { return (float)Decimal.Divide(1, ZoomFactor); }
        }

        public Point? OldDragPoint
        {
            get { return _oldDragPoint; }
            set { _oldDragPoint = value; }
        }


        #endregion

        #region Constructor
        /// <summary>
        /// Default contstructor
        /// </summary>
        public CanvasViewModel()
        {
            
        }
        #endregion

        #region Methods
        #region Mouse Position
        public void MouseOver(Image sender)
        {
            Point pos = Mouse.GetPosition(sender);
            
            MouseOverCanvasEvent?.Invoke(pos, sender.ActualWidth, sender.ActualHeight);
        }
        #endregion

        #region Zoom
        public void Scrolled(MouseWheelEventArgs a, ScrollViewer sender)
        {
            a.Handled = true;  // This stops it from scrolling the vertical scroll bar
            Point mousePos = a.GetPosition(sender);

            if (a.Delta < 0)
            {
                // Stops it going negative and going weird
                if (!(Zoom - ZoomQuantity < MinZoom))
                {
                    Zoom -= ZoomQuantity;
                }

            }
            else
            {
                Zoom += ZoomQuantity;
            }


        }

        public void CanvasSizeChanged(UserControl sender, EventArgs a)
        {
            CanvasHeight = Convert.ToInt32(sender.ActualHeight);
            CanvasWidth = Convert.ToInt32(sender.ActualWidth);

            if (Image is null) { return; }

            MinZoom = (float)Decimal.Divide((decimal)Math.Floor(Decimal.Divide(CanvasHeight, Image.Height) * ZoomFactor), ZoomFactor);
            Zoom = MinZoom;

            NotifyOfPropertyChange(() => Zoom);
        }
        #endregion

        #region Pan
        public void RightMouseDown(ScrollViewer scrollViewer, MouseButtonEventArgs a)
        {
            Point mousePos = a.GetPosition(scrollViewer);

            //make sure we still can use the scrollbars
            if (mousePos.X <= scrollViewer.ViewportWidth && mousePos.Y < scrollViewer.ViewportHeight)
            {
                
                scrollViewer.Cursor = Cursors.SizeAll;
                Mouse.Capture(scrollViewer);
                OldDragPoint = mousePos;
            }

                
            
        }

        public void RightMouseUp(ScrollViewer scrollViewer)
        {
            scrollViewer.ReleaseMouseCapture();
            OldDragPoint = null;
        }

        public void MouseDrag(ScrollViewer scrollViewer, MouseEventArgs a)
        {
            Point Pos = a.GetPosition(scrollViewer);

            if (Pos == OldDragPoint)
            {
                return;
            }

            if (OldDragPoint.HasValue)
            {
                double dX = Pos.X - OldDragPoint.Value.X;
                double dY = Pos.Y - OldDragPoint.Value.Y;

                OldDragPoint = Pos;


                scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - dX);
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - dY);              
            }
        }
        #endregion

        #region Click
        public void LeftMouseDown(Image sender)
        {
            Point pos = Mouse.GetPosition(sender);
            LeftClickedCanvas?.Invoke(pos, sender.ActualWidth, sender.ActualHeight);
        }
        #endregion
        #endregion

    }
}
