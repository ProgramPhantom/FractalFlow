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
    public delegate void MouseOverCanvas(Point Pos, double width, double height);

    public class CanvasViewModel : Screen
    {
        #region Events
        public event MouseOverCanvas? MouseOverCanvasEvent;  // Event

        #endregion

        #region Fields

        private FractalImage? _image;

        private float _zoom = 1;
        private float _minScale;

        private int _zoomFactor = 16;

        private Point? _oldDragPoint;
        
        #endregion

        #region Properties
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
        public float Zoom
        {
            get { return _zoom; }
            set 
            { 
                _zoom = value;
                NotifyOfPropertyChange(() => Zoom);
            }
        }
        public float MinZoom
        {
            get { return _minScale; }
            set { _minScale = value; }
        }
        public int CanvasHeight { get; set; }
        public double ImageActualHeight
        {
            get
            {
                return Image.Height * Zoom;
            }
        }
        public double ImageActualWidth
        {
            get
            {
                return Image.Width * Zoom;
            }
        }
        public int ZoomFactor
        {
            get { return _zoomFactor; }
            set { _zoomFactor = value; }
        }
        public float ZoomQuantity
        {
            get { return (float)Decimal.Divide(1, _zoomFactor); }
        }

        public Point? OldDragPoint
        {
            get { return _oldDragPoint; }
            set { _oldDragPoint = value; }
        }


        #endregion

        public CanvasViewModel()
        {
            
        }

        #region Mouse Position
        public void MouseOver(Image sender, MouseEventArgs relative)
        {
            Point Pos = Mouse.GetPosition(sender);
            
            MouseOverCanvasEvent?.Invoke(Pos, sender.ActualWidth, sender.ActualHeight);
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




    }
}
