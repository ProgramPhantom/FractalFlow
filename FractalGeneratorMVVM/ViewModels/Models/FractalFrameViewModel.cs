using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FractalCore;
using Caliburn.Micro;
using System.Windows.Media;

namespace FractalGeneratorMVVM.ViewModels.Models
{
    public delegate void FractalFrameSelected(FractalFrameViewModel sender);

    public class FractalFrameViewModel : Screen
    {
        #region Fields
        private FractalFrame _fractalFrameModel;
        private string _name;
        private int _number;
        private bool _isSelected;

        private SolidColorBrush _colour;
        private SolidColorBrush _textColour;
        #endregion


        #region Properties
        public FractalFrame FractalFrameModel
        {
            get { return _fractalFrameModel; }
            set { _fractalFrameModel = value; }
        }

        public int Number
        {
            get { return _number; }
            set { _number = value; }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);
            }
        }

        public SolidColorBrush Colour
        {
            get { return _colour; }
            set { _colour = value; }
        }

       
        public SolidColorBrush TextColour
        {
            get { return _textColour; }
            set { _textColour = value; }
        }

        #endregion

        public FractalFrameViewModel(int num, FractalFrame model, byte red=123, byte green=255, byte blue=0, string name="Untitled")
        {
            _number = num;
            _name = name;

            _fractalFrameModel = model;
            _colour = new SolidColorBrush(Color.FromRgb(red, green, blue));

            // Decide if the font colour should be black or white depending on the colour of the background
            if (red * 0.299f + green * 0.587f + blue * 0.114f > 186)
            {
                _textColour = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            } else
            {
                _textColour = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
        }

        public event FractalFrameSelected FractalFrameSelectedEvent;

        public void SelectButton()
        {
            IsSelected = true;
            System.Diagnostics.Trace.WriteLine("Clicked!");

            SelectedEvent();
        }

        protected virtual void SelectedEvent()
        {

            FractalFrameSelectedEvent?.Invoke(this);
        }

    }
}
