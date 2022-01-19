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

        private List<IPainter> _painters;
        private List<Fractal> _fractalList;

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

        public List<Fractal> FractalList
        {
            get { return _fractalList; }
            set { _fractalList = value; }
        }

        public List<IPainter> Painters
        {
            get { return _painters; }
            set { _painters = value; }
        }

        private FormulaStackViewModel _formulaStack;

        public FormulaStackViewModel FormulaStack
        {
            get { return _formulaStack; }
            set { _formulaStack = value; }
        }

        public ShellViewModel()
        {
            Painters = new List<IPainter>();  // This will be where the fractals must be loaded from the database into the application
            FractalList = new List<Fractal>();


            Painters.Add(new BasicPainter(0, 255, 255));
            CurrentPainter = new BasicPainter(255, 140, 40);

            FormulaStack = new FormulaStackViewModel();
        }

        public void Render(Fractal currentFractal, IPainter currentPainter)
        {
            Fractal newFractal = new Fractal(200, 200);

            FormulaStack.FractalCollection.Add(newFractal);

            CurrentImage = new FractalImage(ref newFractal, Painters[0]);


       
        }

        public bool CanRender(Fractal currentFractal, IPainter currentPainter) => (CurrentPainter != null);
    }
}
