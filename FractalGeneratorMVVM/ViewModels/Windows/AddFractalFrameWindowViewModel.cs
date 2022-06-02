using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using FractalGeneratorMVVM.ViewModels.Controls;
using FractalCore;
using System.Windows.Media;

namespace FractalGeneratorMVVM.ViewModels.Windows
{
    public class AddFractalFrameWindowViewModel : Screen
    {

        #region Fields
        private FractalFrameStackViewModel _fractalFrameStack;

        private WindowManager _windowManager;

        private NoMaxWindowViewModel _window;

        private int _width = 1000;
        private int _height = 650;

        private Color _colour = Colors.AliceBlue;

        #endregion


        #region Properties
        public WindowManager WindowManager
        {
            get { return _windowManager; }
            set { _windowManager = value; }
        }


        public NoMaxWindowViewModel Window
        {
            get { return _window; }
            set { _window = value; }
        }


        public string Name { get; set; } = BaseScaffold.NameDefault;
        public float Top { get; set; } = BaseScaffold.TopDefault;
        public float Bottom { get; set; } = BaseScaffold.BottomDefault;
        public float Left { get; set; } = BaseScaffold.LeftDefault;
        public float Right { get; set; } = BaseScaffold.RightDefault;
        public uint Iterations { get; set; } = BaseScaffold.IterationsDefault;
        public int Bail { get; set; } = BaseScaffold.BailDefault;

        public Color Colour
        {
            get
            {
                return _colour;
            }
            set
            {
                _colour = value;
                NotifyOfPropertyChange(() => Colour);
            }
        }


        public int Width
        {
            get { return _width; }
            set 
            { 
                _width = value;
                NotifyOfPropertyChange(() => Width);
            }
        }

        public int Height
        {
            get { return _height; }
            set 
            { 
                _height = value;
                NotifyOfPropertyChange(() => Height);
            }
        }
        #endregion


        #region Constructors
        public AddFractalFrameWindowViewModel(FractalFrameStackViewModel fractalFrameStack)
        {
            _fractalFrameStack = fractalFrameStack;

            _windowManager = new WindowManager();
            _window = new NoMaxWindowViewModel(this, "Add Fractal Frame", ResizeMode.NoResize, Width, Height);
        }

        /// <summary>
        /// Default constructor so cal can bind at design time
        /// Don't use.
        /// </summary>
        public AddFractalFrameWindowViewModel()
        {
            _fractalFrameStack = new FractalFrameStackViewModel();

            _windowManager = new WindowManager();
            _window = new NoMaxWindowViewModel(this, "Add Fractal Frame", ResizeMode.NoResize, Width, Height);
        }
        #endregion 

        public void ShowWindow()
        {
            _windowManager.ShowWindowAsync(_window);
           
        }

        public void CloseWindow()
        {
            _window.TryCloseAsync();
        }
        
        /// <summary>
        /// When the add button is clicked
        /// </summary>
        public void AddFractalFrame()
        {
            System.Diagnostics.Trace.WriteLine("Add fractal frame!");
            _fractalFrameStack.AddFractalFrame(new FractalFrame(Left, Right, Top, Bottom, Name, Iterations, Bail), Colour.R, Colour.G, Colour.B);
            _window.TryCloseAsync();
        }

    }
}
