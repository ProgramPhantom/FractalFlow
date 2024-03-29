﻿using Caliburn.Micro;
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
using System.Windows.Media;

namespace FractalGeneratorMVVM.ViewModels.Controls
{
    public class PainterStackViewModel : Screen
    {
        #region Fields
        private int _painterTypes;

        private BindableCollection<IPainterViewModel> _painterViewModels;

        private AddPainterWindowViewModel _addPainterWindow;

        private IPainterViewModel? _selectedPainterVM;

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
        public IPainterViewModel? SelectedPainterVM
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

        /// <summary>
        /// Exposes the model housed in the SelectedPainter View Model.
        /// </summary>
        public IPainter? SelectedPainter
        {
            get
            {
                return SelectedPainterVM == null ? null : SelectedPainterVM.PainterModel;
            }
        }
        #endregion


        #region Constructor
        // Constructor
        public PainterStackViewModel()
        {
            _painterTypes = 2;
            _painterViewModels = new BindableCollection<IPainterViewModel>();

            _addPainterWindow = new AddPainterWindowViewModel(this);



            NewBasicPainterLight(new BasicPainterLight());

            _selectedPainterVM = PainterViewModels[0];
        }

        public PainterStackViewModel(BindableCollection<IPainterViewModel> initialPainters)
        {
            _painterTypes = 2;
            _painterViewModels = initialPainters;

            _addPainterWindow = new AddPainterWindowViewModel(this);

            _selectedPainterVM = PainterViewModels[0];
        }
        #endregion

        #region Methods
        /// <summary>
        /// Add a new fractal frame to the collection and set up a corresponding view model
        /// </summary>
        public void NewBasicPainterLight(BasicPainterLight newPainter)
        {
            _painterViewModels.Add(new BasicPainterLightViewModel(newPainter, 1, newPainter.Name));
            SelectedPainterVM = PainterViewModels.Last();
        }

        public void NewBasicPainterDark(BasicPainterDark newPainter)
        {
            PainterViewModels.Add(new BasicPainterDarkViewModel(newPainter, 1, newPainter.Name));
            SelectedPainterVM = PainterViewModels.Last();
        }

        public void AddPainter(IPainter painter)
        {
            if (painter.GetType() == typeof(BasicPainterLight))
            {
                PainterViewModels.Add(new BasicPainterLightViewModel((BasicPainterLight)painter, 1, ((BasicPainterLight)painter).Name));
                SelectedPainterVM = PainterViewModels.Last();
            } else if (painter.GetType() == typeof(BasicPainterDark))
            {
                PainterViewModels.Add(new BasicPainterDarkViewModel((BasicPainterDark)painter, 1, ((BasicPainterDark)painter).Name));
                SelectedPainterVM = PainterViewModels.Last();
            }
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

        public void RandomPainter()
        {
            Random rnd = new Random();

            int type = rnd.Next(_painterTypes);

            switch (type)
            {
                case 0:
                    // Basic Painter Light
                    NewBasicPainterLight(new BasicPainterLight("Random", RandomColour(), RandomColour()));
                    break;
                case 1:

                    NewBasicPainterDark(new BasicPainterDark("Random", RandomColour(), RandomColour()));
                    break;
            }

        }

        public static Color RandomColour()
        {
            Random rnd = new Random();
            return Color.FromRgb((byte)rnd.Next(256), (byte)rnd.Next(256), (byte)rnd.Next(256));
        }

        public void DeletePainter()
        {
            PainterViewModels.Remove(SelectedPainterVM!);
            SelectedPainterVM = PainterViewModels.FirstOrDefault();
        }
        #endregion
    }
}
 