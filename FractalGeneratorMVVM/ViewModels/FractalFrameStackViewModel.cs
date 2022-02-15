using Caliburn.Micro;
using FractalCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalGeneratorMVVM.ViewModels
{
    public class FractalFrameStackViewModel : Screen
    {
        private BindableCollection<FractalFrame> _fractalFrameCollection;


        public BindableCollection<FractalFrame> FractalFrameCollection 
        { 
            get
            {
                return _fractalFrameCollection;
            }
            set
            {
                _fractalFrameCollection = value;
                NotifyOfPropertyChange(() => FractalFrameCollection);
            }
        }
        public FractalFrame SelectedFractalFrame { get; set; }

        public EventHandler NewFractalFrame;

        public FractalFrameStackViewModel()
        {
            FractalFrameCollection = new BindableCollection<FractalFrame>();

            FractalFrameCollection.Add(new FractalFrame());
            SelectedFractalFrame = FractalFrameCollection[0];
        }

        public void AddFractalFrame()
        {
            NewFractalFrame?.Invoke(this, new EventArgs());
            System.Diagnostics.Trace.WriteLine("Button triggered");
        }

    }
}
