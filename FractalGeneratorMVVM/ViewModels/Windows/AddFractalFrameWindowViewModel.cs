using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using FractalGeneratorMVVM.ViewModels.Controls;
using FractalCore;

namespace FractalGeneratorMVVM.ViewModels.Windows
{
    public class AddFractalFrameWindowViewModel : Screen
    {
        private FractalFrameStackViewModel _fractalFrameStack;

        private WindowManager _windowManager;

        public WindowManager WindowManager
        {
            get { return _windowManager; }
            set { _windowManager = value; }
        }


        private DefaultWindowViewModel _window;

        public DefaultWindowViewModel Window
        {
            get { return _window; }
            set { _window = value; }
        }


        public string Name { get; set; } = "Untitled";
        public float Top { get; set; } = BaseScaffold.TopDefault;
        public float Bottom { get; set; } = BaseScaffold.BottomDefault;
        public float Left { get; set; } = BaseScaffold.LeftDefault;
        public float Right { get; set; } = BaseScaffold.RightDefault;
        public uint Iterations { get; set; } = BaseScaffold.IterationsDefault;
        public int Bail { get; set; } = BaseScaffold.BailDefault;

        public AddFractalFrameWindowViewModel(FractalFrameStackViewModel fractalFrameStack)
        {
            _fractalFrameStack = fractalFrameStack;

            _windowManager = new WindowManager();
            _window = new DefaultWindowViewModel(this, "Add Fractal Frame", ResizeMode.NoResize);
        }

        public void ShowWindow()
        {
            _windowManager.ShowWindowAsync(_window);
        }

        /// <summary>
        /// When the add button is clicked
        /// </summary>
        public void AddFractalFrame()
        {
            System.Diagnostics.Trace.WriteLine("Add fractal frame!");
            _fractalFrameStack.AddFractalFrame(new FractalFrame(Left, Right, Top, Bottom, Name, Iterations, Bail));
            _window.TryCloseAsync();
        }

    }
}
