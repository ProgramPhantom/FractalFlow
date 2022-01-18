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

        public Fractal CurrentFractal
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

        private List<Fractal> ?_fractalList;

        public List<Fractal> FractalList
        {
            get { return _fractalList; }
            set { _fractalList = value; }
        }

        private FractalImage ?_currentImage;

        public FractalImage CurrentImage
        {
            get { return _currentImage; }
            set 
            {
                _currentImage = value;
                NotifyOfPropertyChange(() => CurrentImage);
            }
        }

        private BasicPainter ?_testPainter;

        public BasicPainter TestPainter
        {
            get { return _testPainter; }
            set { _testPainter = value; }
        }



        public ShellViewModel()
        {
            _testPainter = new BasicPainter(255, 0, 255);
        }

        public void Render()
        {
            CurrentFractal = new Fractal(2000, 2000);

            // System.Diagnostics.Trace.WriteLine(CurrentFractal.IterationsArray[0, 0]);

            // CurrentFractal.WriteToFile("C:\\Users\\henry\\OneDrive - Xaverian College\\Computer Science\\NEA\\projects\\FractalGeneratorMVVM");

            CurrentImage = new FractalImage(ref _currentFractal, _testPainter);
        
            // CurrentImage.SaveImage("C:\\Users\\henry\\OneDrive - Xaverian College\\Computer Science\\NEA\\projects\\FractalGeneratorMVVM\\FractalGeneratorMVVM\\img\\output.png");
            // NotifyOfPropertyChange(() => CurrentImage);
        }
    }
}
