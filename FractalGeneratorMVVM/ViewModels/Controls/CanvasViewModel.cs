using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FractalCore;
using System.Threading.Tasks;
using Caliburn.Micro;
using System.Windows.Input;
using System.Windows;

namespace FractalGeneratorMVVM.ViewModels.Controls
{
    public delegate void MouseOverCanvas(Point Pos, double width, double height);

    public class CanvasViewModel : Screen
    {
        #region Events
        public event MouseOverCanvas? MouseOverCanvasEvent;  // Event
        #endregion

        #region Fields
        private Point _pos;
        private FractalImage? _image;
        #endregion

        #region Properties
        public FractalImage? Image
        {
            get { return _image; }
            set 
            { 
                _image = value;
                NotifyOfPropertyChange(() => Image);
            }
        }

        public Point Pos
        {
            get
            {
                return _pos;
            }
            set
            {
                _pos = value;
                NotifyOfPropertyChange(() => Pos);
            }
        }
        #endregion


        public CanvasViewModel()
        {
            
        }

        public void MouseOver( System.Windows.Controls.Image sender, MouseEventArgs relative)
        {
            // pos = Mouse.GetPosition(relative.View);
            Pos = Mouse.GetPosition(sender);

            MouseOverCanvasEvent?.Invoke(Pos, sender.ActualWidth, sender.ActualHeight);
            
            

            //System.Diagnostics.Trace.WriteLine($"X: {Pos.X}, Y: {Pos.Y}");

            //System.Diagnostics.Trace.WriteLine($"Width: {sender.ActualWidth}, Height: {sender.ActualHeight}");
        }
    }
}
