using Caliburn.Micro;
using FractalCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalGeneratorMVVM.ViewModels
{
    public class FractalFrameStackViewModel : Screen
    {
        #region Fields
        /// <summary>
        /// Holds the Fractal Frame models
        /// </summary>
        private List<FractalFrame> _fractalFrameList;

        private BindableCollection<FractalFrameViewModel> _fractalFrameViewModels;

        private AddFractalFrameWindowViewModel _addFractalFrameWindow;

        #endregion

        #region Properties
        public List<FractalFrame> FractalFrameList
        {
            get
            {
                return _fractalFrameList;
            }
            set
            {
                _fractalFrameList = value;
                NotifyOfPropertyChange(() => FractalFrameList);
            }
        }


        public BindableCollection<FractalFrameViewModel> FractalFrameViewModels
        {
            get { return _fractalFrameViewModels; }
            set 
            { 
                _fractalFrameViewModels = value;
                NotifyOfPropertyChange(() => FractalFrameViewModels);
            }
        }


        public FractalFrame SelectedFractalFrame { get; set; }

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
            _fractalFrameList = new List<FractalFrame>();
            _fractalFrameViewModels = new BindableCollection<FractalFrameViewModel>();


            // Create the window for adding a new Fractal Frame, passing in this object so
            // the window knows where to put the new fractal frame
            _addFractalFrameWindow = new AddFractalFrameWindowViewModel(this);

            AddFractalFrame(new FractalFrame());


            SelectedFractalFrame = FractalFrameList[0];
            FractalFrameViewModels[0].IsSelected = true;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Add a new fractal frame to the collection and set up a corresponding view model
        /// </summary>
        /// <param name="newFF">The Fractal Frame to add</param>
        public void AddFractalFrame(FractalFrame newFF)
        {
            _fractalFrameList.Add(newFF);

            // ADD THE ACTUAL VISUAL REPRESENTATION OF THE FRACTAL FRAME
            _fractalFrameViewModels.Add(new FractalFrameViewModel(_fractalFrameViewModels.Count() + 1));

            _fractalFrameViewModels.Last().FractalFrameSelectedEvent += OnFractalFrameSelected;
        }

        /// <summary>
        /// Event handler for when one of the fractal frames is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OnFractalFrameSelected(FractalFrameViewModel sender)
        {
            System.Diagnostics.Trace.WriteLine("Gotten the message!");

            

            int index = _fractalFrameViewModels.IndexOf(sender);

            SelectedFractalFrame = FractalFrameList[index];

            foreach (FractalFrameViewModel vm in _fractalFrameViewModels)
            {
                vm.IsSelected = false;
            }

            _fractalFrameViewModels[index].IsSelected = true;
        }

        /// <summary>
        /// Opens the new fractal frame WINDOW
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void NewFractalFrameWindow(object sender, EventArgs e)
        {
            _addFractalFrameWindow.ShowWindow();
        }
        #endregion
    }
}
