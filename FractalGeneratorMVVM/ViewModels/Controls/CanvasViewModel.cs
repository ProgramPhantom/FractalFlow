using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FractalCore;
using System.Threading.Tasks;

namespace FractalGeneratorMVVM.ViewModels.Controls
{
    public class CanvasViewModel
    {

        private FractalImage? _image;

        public FractalImage? Image
        {
            get { return _image; }
            set { _image = value; }
        }


    }
}
