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

namespace FractalGeneratorMVVM.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        private Fractal ?_currentFractal;
        private FractalImage? _currentImage;
        private IPainter? _currentPainter;

        
        private PainterRowViewModel _painterRow;
        private FormulaStackViewModel _formulaStack;


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
            get { return _currentPainter; }
            set 
            { 
                _currentPainter = value;
                NotifyOfPropertyChange(() => CurrentPainter);
            }
        }


        public FormulaStackViewModel FormulaStack
        {
            get { return _formulaStack; }
            set { _formulaStack = value; }
        }

        public PainterRowViewModel PainterRow
        {
            get { return _painterRow; }
            set { _painterRow = value; }
        }


        public ShellViewModel()
        {
            FormulaStack = new FormulaStackViewModel();
            PainterRow = new PainterRowViewModel();


            CurrentPainter = PainterRow.PainterCollection[0];
        }

        public void Render(Fractal currentFractal, IPainter currentPainter)
        {
            Fractal newFractal = new Fractal(200, 200);

            FormulaStack.FractalCollection.Add(newFractal);

            CurrentImage = new FractalImage(ref newFractal, PainterRow.SelectedPainter);
        }

        public void NewPainter()
        {
            PainterRow.PainterCollection.Add(new BasicPainter("Test", 255, 255, 255));
        }

        public bool CanRender(Fractal currentFractal, IPainter currentPainter) => (CurrentPainter != null);
    }
}
