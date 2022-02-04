using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using FractalCore;

namespace FractalGeneratorMVVM.ViewModels
{
    public class FractalFrameRowViewModel : Screen
    {
        private BindableCollection<FractalFrame> _fractalFrameCollection;

        public BindableCollection<FractalFrame> FractalFrameCollection
        {
            get { return _fractalFrameCollection; }
            set {
                _fractalFrameCollection = value;
                NotifyOfPropertyChange(() => FractalFrameCollection);
            }
        }

        private FractalFrame _selectedFractalFrame;

        public FractalFrame SelectedFractalFrame
        {
            get { return _selectedFractalFrame; }
            set {
                _selectedFractalFrame = value;
                NotifyOfPropertyChange(() => SelectedFractalFrame);
            }
        }


        public FractalFrameRowViewModel()
        {
            _fractalFrameCollection = new BindableCollection<FractalFrame>();

            _fractalFrameCollection.Add(new FractalFrame());

            _selectedFractalFrame = _fractalFrameCollection[0];
        }
    }
}
