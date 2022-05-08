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
    public class CanvasViewModel : Screen
    {
        
        private FractalImage? _image;

        public FractalImage? Image
        {
            get { return _image; }
            set 
            { 
                _image = value;
                NotifyOfPropertyChange(() => Image);
            }
        }



        public CanvasViewModel()
        {
            
        }

        public void MouseOver( System.Windows.Controls.Image sender, MouseEventArgs relative)
        {
            // pos = Mouse.GetPosition(relative.View);
            Point pos = Mouse.GetPosition(sender);
            

            System.Diagnostics.Trace.WriteLine($"X: {pos.X}, Y: {pos.Y}");
        }
    }
}
