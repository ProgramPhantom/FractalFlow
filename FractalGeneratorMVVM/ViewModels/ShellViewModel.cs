using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FractalCore;
using Caliburn.Micro;
// using FractalGeneratorMVVM.Models;
using FractalGeneratorMVVM.ViewModels;
using System.Diagnostics;
using System.IO;
using System.Windows;
using FormulaParser;

namespace FractalGeneratorMVVM.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        #region Fields
        // MISC
        private IWindowManager _windowManager;
        private FractalImage? _currentImage;
        private string _formulaBox;

        // WINDOWS 
        private AddPainterWindowViewModel _addPainterWindow;
        private AddFractalFrameWindowViewModel _addFractalFrameWindow;

        // USER CONTROLS
        private PainterRowViewModel _painterRow;
        private IteratorStackViewModel _formulaStack;
        private FractalFrameRowViewModel _fractalFrameRow;
        #endregion

        #region Properties
        // MISC
        public IWindowManager WindowManager
        {
            get { return _windowManager; }
            set { _windowManager = value; }
        }
        public FractalImage ?CurrentImage
        {
            get { return _currentImage; }
            set 
            {
                _currentImage = value;
                NotifyOfPropertyChange(() => CurrentImage);
            }
        }
        public string FormulaBox
        {
            get { return _formulaBox; }
            set
            {
                _formulaBox = value;
                NotifyOfPropertyChange(() => FormulaBox);
            }
        }

        // WINDOWS
        public AddPainterWindowViewModel AddPainterWindow
        {
            get { return _addPainterWindow; }
            set { _addPainterWindow = value; }
        }
        public AddFractalFrameWindowViewModel AddFractalFrameWindow
        {
            get { return _addFractalFrameWindow; }
            set { _addFractalFrameWindow = value; }
        }

        // USER CONTROLS
        public IteratorStackViewModel IteratorStack
        {
            get { return _formulaStack; }
            set { _formulaStack = value; }
        }
        public PainterRowViewModel PainterRow
        {
            get { return _painterRow; }
            set { _painterRow = value; }
        }
        public FractalFrameRowViewModel FractalFrameRow
        {
            get { return _fractalFrameRow; }
            set { _fractalFrameRow = value; }
        }

        // EXTRAS
        public BindableCollection<IPainter> PainterCollection { get { return PainterRow.PainterCollection; } }
        public BindableCollection<FractalFrame> FractalFrameCollection { get { return _fractalFrameRow.FractalFrameCollection; } }
        #endregion


        public ShellViewModel()
        {
            // USER CONTROLS
            _formulaStack = new IteratorStackViewModel();
            _painterRow = new PainterRowViewModel();
            _fractalFrameRow = new FractalFrameRowViewModel();

            // WINDOWS
            _windowManager = new WindowManager();
            _addPainterWindow = new AddPainterWindowViewModel(PainterCollection);
            _addFractalFrameWindow = new AddFractalFrameWindowViewModel(FractalFrameCollection);

            // MISC
            _formulaBox = "z^2 + c";

        }

        public void Render()
        {
            // Create a new formula based on the string in the FormulaBox
            BasicIterator newIterator = new BasicIterator("Untitled", FormulaBox);
            // Add it to the Iterator Collection
            IteratorStack.IteratorCollection.Add(newIterator);

            Fractal fractal = new Fractal(1000, 1000, FractalFrameRow.SelectedFractalFrame, newIterator);

            CurrentImage = new FractalImage(ref fractal, PainterRow.SelectedPainter);
        }


        public void NewPainter()
        {
            _windowManager.ShowWindowAsync(_addPainterWindow, null, null);
        }

        public void NewFractalFrame()
        {
            _windowManager.ShowWindowAsync(_addFractalFrameWindow, null, null);
        }
    }
}
