using Caliburn.Micro;
using FractalCore;
using FractalGeneratorMVVM.ViewModels.Models;
using FractalGeneratorMVVM.ViewModels.Windows;
using FractalGeneratorMVVM.ViewModels.WinPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FractalGeneratorMVVM.ViewModels.Controls
{
    public class FractalFrameStackViewModel : Screen
    {
        static Color DEFAULTCOLOUR = Colors.AliceBlue;

        #region Fields

        private BindableCollection<FractalFrameViewModel> _fractalFrameViewModels;


        private FractalFrameViewModel _selectedFractalFrameVM;


        private AddFractalFrameWindowViewModel _addFractalFrameWindow;

        #endregion

        #region Properties

        /// <summary>
        /// The list of fractal frame view models which is displayed on the screen
        /// </summary>
        public BindableCollection<FractalFrameViewModel> FractalFrameViewModels
        {
            get { return _fractalFrameViewModels; }
            set 
            { 
                _fractalFrameViewModels = value;
                NotifyOfPropertyChange(() => FractalFrameViewModels);
            }
        }

        /// <summary>
        /// The view model of the selected fractal frame
        /// </summary>
        public FractalFrameViewModel SelectedFractalFrameVM
        {
            get { return _selectedFractalFrameVM; }
            set
            {
                _selectedFractalFrameVM = value;

                System.Diagnostics.Trace.WriteLine("NEW FRACTAL FRAME");

                System.Diagnostics.Trace.WriteLine($"{_selectedFractalFrameVM.FractalFrameModel.Iterations}");

            }
        }

        /// <summary>
        /// The model of the selected fractal frame
        /// </summary>
        public FractalFrame SelectedFractalFrame
        {
            get
            {
                return _selectedFractalFrameVM.FractalFrameModel;
            }

        }

        /// <summary>
        /// The window to add a new fractal frame
        /// </summary>
        public AddFractalFrameWindowViewModel AddFractalFrameWindow
        {
            get { return _addFractalFrameWindow; }
            set { _addFractalFrameWindow = value; }
        }
        #endregion


        #region Constructor
        // Constructor
        public FractalFrameStackViewModel()
        {
            _fractalFrameViewModels = new BindableCollection<FractalFrameViewModel>();

            // Create the window for adding a new Fractal Frame, passing in this object so
            // the window knows where to put the new fractal frame
            _addFractalFrameWindow = new AddFractalFrameWindowViewModel(this);
            
            // Initial fractal frames on launch
            AddFractalFrame(new FractalFrame());
            
            _selectedFractalFrameVM = FractalFrameViewModels[0];
            FractalFrameViewModels[0].IsSelected = true;  // Select it 
        }
        #endregion

        #region Methods
        /// <summary>
        /// Add a new fractal frame view model with connected fractal frame object
        /// </summary>
        /// <param name="newFF">The Fractal Frame to add</param>
        public void AddFractalFrame(FractalFrame newFF, byte red=200, byte green=100, byte blue=100)
        { 
            // Add the new fractal frame view model and attatch the fractal frame model
            _fractalFrameViewModels.Add(new FractalFrameViewModel(_fractalFrameViewModels.Count() + 1, newFF, red, green, blue));
        }


        /// <summary>
        /// Opens the new fractal frame WINDOW
        /// </summary>
        public void NewFractalFrameWindow(object sender, EventArgs e)
        {
            _addFractalFrameWindow.ShowWindow();
        }
        #endregion
    }
}
