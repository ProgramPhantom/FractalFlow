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

namespace FractalGeneratorMVVM.ViewModels.Controls
{
    public  class PainterStackViewModel : Screen
    {
        #region Fields
       

        private BindableCollection<BasicPainterViewModel> _painterViewModels;

        private AddPainterWindowViewModel _addPainterWindow;

        private BasicPainterViewModel _selectedPainterVM;

        #endregion

        #region Properties
        /// <summary>
        /// The view models of the painters
        /// </summary>
        public BindableCollection<BasicPainterViewModel> PainterViewModels
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
        public BasicPainterViewModel SelectedPainterVM
        {
            get
            {
                return _selectedPainterVM;
            }
            set
            {
                _selectedPainterVM = value;
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
        #endregion



        #region Constructor
        // Constructor
        public PainterStackViewModel()
        {
            
            _painterViewModels = new BindableCollection<BasicPainterViewModel>();

            _addPainterWindow = new AddPainterWindowViewModel(this);



            NewPainter(new BasicPainter("Basic", 255, 0, 0));
            NewPainter(new BasicPainter("Basic", 0, 255, 0));
            NewPainter(new BasicPainter("Basic", 0, 0, 255));



            
            PainterViewModels[0].IsSelected = true;
            _selectedPainterVM = PainterViewModels[0];
        }
        #endregion

        #region Methods
        /// <summary>
        /// Add a new fractal frame to the collection and set up a corresponding view model
        /// </summary>
        /// <param name="newFF">The Fractal Frame to add</param>
        public void NewPainter(IPainter newPainter)
        {
            

            // ADD THE ACTUAL VISUAL REPRESENTATION OF THE FRACTAL FRAME
            _painterViewModels.Add(new BasicPainterViewModel((BasicPainter)newPainter, 1));


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
 