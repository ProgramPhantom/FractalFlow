using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using FractalCore;

namespace FractalGeneratorMVVM.ViewModels
{
    public class AddFractalFrameWindowViewModel : Screen
    {
        private BindableCollection<FractalFrame> _fractalFrames;

        public string Name { get; set; } = "Untitled";
        public float Top { get; set; } = BaseScaffold.TopDefault;
        public float Bottom { get; set; } = BaseScaffold.BottomDefault;
        public float Left { get; set; } = BaseScaffold.LeftDefault;
        public float Right { get; set; } = BaseScaffold.RightDefault;
        public uint Iterations { get; set; } = BaseScaffold.IterationsDefault;
        public int Bail { get; set; } = BaseScaffold.BailDefault;

        public AddFractalFrameWindowViewModel(BindableCollection<FractalFrame> fractalFrames)
        {
            _fractalFrames = fractalFrames;
        }

        public void AddFractalFrame()
        {
            _fractalFrames.Add(new FractalFrame(Left, Right, Top, Bottom, Name, Iterations, Bail));
        }

    }
}
