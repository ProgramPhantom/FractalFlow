using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FractalCore;
using System.Threading.Tasks;
using Caliburn.Micro;

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
    }
}
