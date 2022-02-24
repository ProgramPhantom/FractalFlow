﻿using Caliburn.Micro;
using FractalCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalGeneratorMVVM.ViewModels
{
    public  class PainterStackViewModel : Screen
    {
        #region Fields
        /// <summary>
        /// Holds the Fractal Frame models
        /// </summary>
        private List<IPainter> _paintersList;

        private BindableCollection<PainterViewModel> _painterViewModels;

        private AddPainterWindowViewModel _addPainterWindow;

        #endregion

        #region Properties
        public List<IPainter> PaintersList
        {
            get
            {
                return _paintersList;
            }
            set
            {
                _paintersList = value;
                NotifyOfPropertyChange(() => PaintersList);
            }
        }


        public BindableCollection<PainterViewModel> PainterViewModels
        {
            get { return _painterViewModels; }
            set
            {
                _painterViewModels = value;
                NotifyOfPropertyChange(() => PainterViewModels);
            }
        }

        /// <summary>
        /// The painter model that is selected
        /// </summary>
        public IPainter SelectedPainter { get; set; }

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
            _paintersList = new List<IPainter>();
            _painterViewModels = new BindableCollection<PainterViewModel>();

            _addPainterWindow = new AddPainterWindowViewModel(this);



            NewPainter(new BasicPainter("Basic", 255, 0, 0));


            SelectedPainter = PaintersList[0];
            PainterViewModels[0].IsSelected = true;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Add a new fractal frame to the collection and set up a corresponding view model
        /// </summary>
        /// <param name="newFF">The Fractal Frame to add</param>
        public void NewPainter(IPainter newPainter)
        {
            _paintersList.Add(newPainter);

            // ADD THE ACTUAL VISUAL REPRESENTATION OF THE FRACTAL FRAME
            _painterViewModels.Add(new PainterViewModel((BasicPainter)newPainter, 1));

            _painterViewModels.Last().PainterSelectedEvent += OnPainterSelected;
        }

        /// <summary>
        /// Event handler for when one of the fractal frames is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OnPainterSelected(PainterViewModel sender)
        {
            System.Diagnostics.Trace.WriteLine("Gotten the message!");

            int index = _painterViewModels.IndexOf(sender);

            SelectedPainter = PaintersList[index];

            foreach (PainterViewModel vm in _painterViewModels)
            {
                vm.IsSelected = false;
            }

            _painterViewModels[index].IsSelected = true;
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
