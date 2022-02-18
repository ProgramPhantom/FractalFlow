using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FractalCore;
using Caliburn.Micro;
using FractalGeneratorMVVM.ViewModels;
using System.Diagnostics;
using System.IO;
using System.Windows;
using FormulaParser;
using System.Threading;
using System.Text.RegularExpressions;

namespace FractalGeneratorMVVM.ViewModels
{
    public class ShellViewModelOld : Conductor<object>
    {
        #region Fields
        // MISC
        private IWindowManager _windowManager;
        private FractalImage? _currentImage;
        private string _formulaBox;
        private int _progressBar;


        private int _width = 1000;
        private int _height = 1000;

        // WINDOWS 
        private AddPainterWindowViewModel _addPainterWindow;
        private AddFractalFrameWindowViewModel _addFractalFrameWindow;

        // USER CONTROLS

        private PainterStackViewModel _painterStack;

        private IteratorStackViewModel _formulaStack;

        private FractalFrameStackViewModel _fractalFrameStack;
        #endregion

        #region Properties
        // MISC
        public IWindowManager WindowManager
        {
            get { return _windowManager; }
            set { _windowManager = value; }
        }
        public FractalImage? CurrentImage
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

        public int ProgressBar
        {
            get { return _progressBar; }
            set
            {
                _progressBar = value;
                NotifyOfPropertyChange(() => ProgressBar);
            }
        }

        CancellationTokenSource cts = new CancellationTokenSource();


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

        public PainterStackViewModel PainterStack
        {
            get { return _painterStack; }
            set { _painterStack = value; }
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
                NotifyOfPropertyChange(() => FractalFrameStack);
            }
        }

        // EXTRAS
        public BindableCollection<IPainter> PainterCollection { get { return PainterStack.PainterCollection; } }
        public BindableCollection<FractalFrame> FractalFrameCollection { get { return _fractalFrameStack.FractalFrameCollection; } }
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }
        #endregion

        public ShellViewModelOld()
        {
            // USER CONTROLS
            _formulaStack = new IteratorStackViewModel();

            _painterStack = new PainterStackViewModel();
            _painterStack.NewPainter += new EventHandler(NewPainter);

            _fractalFrameStack = new FractalFrameStackViewModel();
            _fractalFrameStack.NewFractalFrame += new EventHandler(NewFractalFrame);

            // WINDOWS
            _windowManager = new WindowManager();
            _addPainterWindow = new AddPainterWindowViewModel(PainterCollection);
            _addFractalFrameWindow = new AddFractalFrameWindowViewModel(FractalFrameCollection);

            // MISC
            _formulaBox = "z^2 + c";

        }

        #region Methods
        /// <summary>
        /// Don't use it.
        /// </summary>
        public void Render()
        {
            // Create a new formula based on the string in the FormulaBox
            BasicIterator newIterator = new BasicIterator("Untitled", FormulaBox);
            // Add it to the Iterator Collection
            IteratorStack.IteratorCollection.Add(newIterator);

            Fractal fractal = new Fractal(_width, _height, FractalFrameStack.SelectedFractalFrame, newIterator);

            CurrentImage = new FractalImage(ref fractal, PainterStack.SelectedPainter);
        }

        public async Task RenderAsync()
        {
            cts = new CancellationTokenSource();  // Set up the cancel thing
            // Create a new formula based on the string in the FormulaBox


            NewIterator(FormulaBox);


            Fractal fractal = new Fractal(_width, _height, FractalFrameStack.SelectedFractalFrame, IteratorStack.IteratorCollection.Last());

            Progress<RenderProgressModel> progress = new Progress<RenderProgressModel>();
            progress.ProgressChanged += ReportProgress;  // Call this method when there is a progress update

            try
            {
                await fractal.GenerateProgressAsync(progress, cts.Token);
            }
            catch (OperationCanceledException)
            {

            }


            CurrentImage = new FractalImage(ref fractal, PainterStack.SelectedPainter);

        }

        public void CancelRender()
        {
            cts.Cancel();

            ProgressBar = 0;

        }

        private void ReportProgress(object? sender, RenderProgressModel e)
        {
            ProgressBar = e.PercentageComplete;

        }

        public void NewIterator(string formula)
        {
            // Remove the whitespace in the formula:
            formula = Regex.Replace(formula, @"\s+", "");

            // Search through the iterators already there
            foreach (IIterator iterator in IteratorStack.IteratorCollection)
            {
                if (iterator.FormulaString == formula)
                {
                    IteratorStack.SelectedIterator = iterator;  // Select that iterator (:
                    return;  // Iterator already used before, so return
                }
            }

            BasicIterator newIterator = new BasicIterator("Untitled", formula);
            // Add it to the Iterator Collection
            IteratorStack.IteratorCollection.Add(newIterator);

            IteratorStack.UpdateSelected();  // Set the selected iterator to the most recently made one
        }

        /// <summary>
        /// Opens the new painter window
        /// </summary>
        public void NewPainter(object sender, EventArgs e)
        {
            _windowManager.ShowWindowAsync(_addPainterWindow, null, null);
        }

        /// <summary>
        /// Opens the new fractal frame window
        /// </summary>
        public void NewFractalFrame(object sender, EventArgs e)
        {
            _windowManager.ShowWindowAsync(_addFractalFrameWindow, null, null);
        }
        #endregion
    }
}
