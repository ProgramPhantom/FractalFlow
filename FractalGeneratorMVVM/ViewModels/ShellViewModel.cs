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
        private Fractal ?_currentFractal;
        private FractalImage? _currentImage;
        private IPainter? _currentPainter;
        private IWindowManager _windowManager;
        private string _formulaBox;
        private IIterator _currentIterator;
        private AddPainterWindowViewModel _addPainterWindow;

        private PainterRowViewModel _painterRow;
        private IteratorStackViewModel _formulaStack;


        public Fractal ?CurrentFractal
        {
            get
            {
                return _currentFractal;
            }
            set
            {
                _currentFractal = value;
                NotifyOfPropertyChange(() => CurrentFractal);
            }
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

        public IPainter ?CurrentPainter
        {
            get { return PainterRow.SelectedPainter; }
            set 
            { 
                _currentPainter = value;
                NotifyOfPropertyChange(() => CurrentPainter);
            }
        }

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

        public IWindowManager WindowManager
        {
            get { return _windowManager; }
            set { _windowManager = value; }
        }


        public AddPainterWindowViewModel AddPainterWindow
        {
            get { return _addPainterWindow; }
            set { _addPainterWindow = value; }
        }

        public BindableCollection<IPainter> PainterCollection { get { return _painterRow.PainterCollection; } }


        public string FormulaBox
        {
            get { return _formulaBox; }
            set { 
                _formulaBox = value;
                NotifyOfPropertyChange(() => CurrentFractal);
            }
        }


        public IIterator CurrentIterator
        {
            get { return _currentIterator; }
            set { 
                _currentIterator = value;
                NotifyOfPropertyChange(() => CurrentIterator);
            }
        }


        public ShellViewModel()
        {
            _windowManager = new WindowManager();
            _formulaStack = new IteratorStackViewModel();
            _painterRow = new PainterRowViewModel();
            _addPainterWindow = new AddPainterWindowViewModel(PainterCollection);


            CurrentPainter = PainterRow.PainterCollection[0];
            FormulaBox = "z^2+c";
        }

        public void Render(Fractal currentFractal, IPainter currentPainter)
        {
            // Create a new formula based on 

            BasicIterator newIterator = new BasicIterator("Untitled", FormulaBox);
            IteratorStack.IteratorCollection.Add(newIterator);

            Fractal fractal = new Fractal(1000, 1000, newIterator);

            CurrentImage = new FractalImage(ref fractal, CurrentPainter);

        }

        public void NewPainter()
        {
            //PainterRow.PainterCollection.Add(new BasicPainter("Test", 255, 255, 255));

            //AddPainterWindowViewModel newPaint = new AddPainterWindowViewModel();
            //newPaint.Show();

            _windowManager.ShowWindowAsync(_addPainterWindow, null, null);
        }

        // public bool CanRender(Fractal currentFractal, IPainter currentPainter) => (CurrentPainter != null);
    }
}
