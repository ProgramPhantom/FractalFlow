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
using FractalGeneratorMVVM.ViewModels.Windows;

namespace FractalGeneratorMVVM.ViewModels.WinPages
{
    public class AddFractalFrameWindowViewModel : Screen
    {

        #region Fields
        private FractalFrameStackViewModel _fractalFrameStack;

        private WindowManager _windowManager;

        private NoMaxWindowViewModel _window;

        private int _width = 1100;
        private int _height = 650;

        private Color _colour = Colors.AliceBlue;

        private int _tabIndex = 0;

        #endregion


        #region Properties
        #region Window Stuff
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

        #region Universal
        public string Name { get; set; } = BaseScaffold.NameDefault;
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
        #endregion

        #region Sides
        public float Top { get; set; } = BaseScaffold.TopDefault;
        public float Bottom { get; set; } = BaseScaffold.BottomDefault;
        public float Left { get; set; } = BaseScaffold.LeftDefault;
        public float Right { get; set; } = BaseScaffold.RightDefault;
        #endregion

        #region Centre
        public float FFHeight { get; set; } = BaseScaffold.HeightDefault;
        public float FFWidth { get; set; } = BaseScaffold.WidthDefault;
        public decimal RealCentre { get; set; } = (decimal)BaseScaffold.CentreRealDefault;
        public float ImagCentre { get; set; } = BaseScaffold.CentreImagDefault;
        #endregion


        public int TabIndex
        {
            get { return _tabIndex; }
            set { _tabIndex = value; }
        }
        public FractalFrameStackViewModel FractalFrameStack
        {
            get
            {
                return _fractalFrameStack;
            }
            set
            {
                _fractalFrameStack = value;
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

        public void AddFractalFrame()
        {
            switch (TabIndex)
            {
                case 0:
                    AddFractalFrameCentre();
                    break;
                case 1:
                    AddFractalFrameSides();
                    break;
            }
        }

        public void AddFractalFrameSides()
        {
            FractalFrameStack.AddFractalFrame(new FractalFrame(Left, Right, Top, Bottom, Name, Iterations, Bail), Colour);
            _window.TryCloseAsync();
        }

        public void AddFractalFrameCentre()
        {
            FractalFrameStack.AddFractalFrame(FractalFrame.FractalFrameCentre(FFWidth, FFHeight, (float)RealCentre, ImagCentre, Name, Iterations, Bail), Colour);
            _window.TryCloseAsync();
        }
    }
}
