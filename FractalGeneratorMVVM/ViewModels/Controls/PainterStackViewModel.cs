using Caliburn.Micro;
using FractalCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Xml.Serialization;
using FractalGeneratorMVVM.ViewModels.Models;
using FractalGeneratorMVVM.ViewModels.Windows;
using FractalGeneratorMVVM.Views.Models;
using FractalGeneratorMVVM.ViewModels.Models.Painters;
using FractalCore.Painting;
using FractalGeneratorMVVM.ViewModels.WinPages;

namespace FractalGeneratorMVVM.ViewModels.Controls
{
    public class PainterStackViewModel : Screen
    {
        #region Fields
       

        private BindableCollection<IPainterViewModel> _painterViewModels;

        private AddPainterWindowViewModel _addPainterWindow;

        private IPainterViewModel _selectedPainterVM;

        #endregion

        #region Properties
        /// <summary>
        /// The view models of the painters
        /// </summary>
        public BindableCollection<IPainterViewModel> PainterViewModels
        {
            get { return _painterViewModels; }
            set
            {
                _painterViewModels = value;
                NotifyOfPropertyChange(() => PainterViewModels);
            }
        }

        /// <summary>
        /// The painter view model which is selected
        /// </summary>
        public IPainterViewModel SelectedPainterVM
        {
            get
            {
                return _selectedPainterVM;
            }
            set
            {
                _selectedPainterVM = value;
                System.Diagnostics.Trace.WriteLine($"New painter selected: {_selectedPainterVM.PainterModel.GetType()}");
                NotifyOfPropertyChange(() => SelectedPainterVM);
            }

        }

        /// <summary>
        /// The window to add a painter
        /// </summary>
        public AddPainterWindowViewModel AddPainterWindow
        {
            get { return _addPainterWindow; }
            set { _addPainterWindow = value; }
        }

        /// <summary>
        /// Exposes the model housed in the SelectedPainter View Model.
        /// </summary>
        public IPainter SelectedPainter
        {
            get
            {
                return SelectedPainterVM.PainterModel;
            }
        }
        #endregion



        #region Constructor
        // Constructor
        public PainterStackViewModel()
        {
            
            _painterViewModels = new BindableCollection<IPainterViewModel>();

            _addPainterWindow = new AddPainterWindowViewModel(this);



            NewBasicPainterLight(new BasicPainterLight());
            NewBasicPainterLight(new BasicPainterLight());
            NewBasicPainterLight(new BasicPainterLight());

            


            _selectedPainterVM = PainterViewModels[0];
        }
        #endregion

        #region Methods
        /// <summary>
        /// Add a new fractal frame to the collection and set up a corresponding view model
        /// </summary>
        public void NewBasicPainterLight(BasicPainterLight newPainter)
        {
           
            // ADD THE ACTUAL VISUAL REPRESENTATION OF THE FRACTAL FRAME
            _painterViewModels.Add(new BasicPainterLightViewModel(newPainter, 1));

        }

        public void NewBasicPainterDark(BasicPainterDark newPainter)
        {
            _painterViewModels.Add(new BasicPainterDarkViewModel(newPainter, 1));
        }




        /// <summary>
        /// Opens the new fractal frame WINDOW
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void NewPainterWindow(object sender, EventArgs e)
        {
            _addPainterWindow.ShowWindow();
        }
        #endregion
    }
}
 