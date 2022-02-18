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
        #endregion

        #region Properties
        public List<FractalFrame> FractalFrameCollection
        {
            get
            {
                return _fractalFrameList;
            }
            set
            {
                _fractalFrameList = value;
                NotifyOfPropertyChange(() => FractalFrameCollection);
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
        #endregion

        #region Constructor
        // Constructor
        public FractalFrameStackViewModel()
        {
            _fractalFrameList = new List<FractalFrame>();
            _fractalFrameViewModels = new BindableCollection<FractalFrameViewModel>();

            AddFractalFrame(new FractalFrame());
            AddFractalFrame(new FractalFrame());
            AddFractalFrame(new FractalFrame());
            AddFractalFrame(new FractalFrame());

            SelectedFractalFrame = FractalFrameCollection[0];
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
            _fractalFrameViewModels.Add(new FractalFrameViewModel(1)); 
        }
        #endregion
    }
}
