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
    public class AddFractalFramePageViewModel : Screen
    {

        #region Fields
        private FractalFrameStackViewModel _fractalFrameStack;

        private Color _colour = Colors.AliceBlue;

        #endregion


        #region Properties

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
        public AddFractalFramePageViewModel(FractalFrameStackViewModel fractalFrameStack)
        {
            _fractalFrameStack = fractalFrameStack;
        }

        /// <summary>
        /// Default constructor so cal can bind at design time
        /// Don't use.
        /// </summary>
        public AddFractalFramePageViewModel()
        {
            _fractalFrameStack = new FractalFrameStackViewModel();
        }
        #endregion 


        public void CloseWindow()
        {
            FractalFrameStack.AddFractalFrameWindow.TryCloseAsync();
        }
        
        /// <summary>
        /// When the add button is clicked
        /// </summary>
        public void AddFractalFrame()
        {
            FractalFrameStack.AddFractalFrame(new FractalFrame(Left, Right, Top, Bottom, Name, Iterations, Bail), Colour.R, Colour.G, Colour.B);
            FractalFrameStack.AddFractalFrameWindow.TryCloseAsync();
        }

    }
}
