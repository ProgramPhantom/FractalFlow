using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FractalCore;
using System.Windows;
using Caliburn.Micro;
using System.Windows.Media;

namespace FractalGeneratorMVVM.ViewModels
{
    public delegate void PainterSelected(PainterViewModel sender);

    public class PainterViewModel : Screen
    {
        #region Fields
        private BasicPainter _painterModel;
        private string _name;
        private int _number;
        private bool _isSelected;

        private SolidColorBrush _buttonColor;
        #endregion

        #region Properties
        public BasicPainter PainterModel
        {
            get { return _painterModel; }
            set { _painterModel = value; }
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

        public SolidColorBrush ButtonColor
        {
            get { return _buttonColor; }
            set { _buttonColor = value; }
        }
        #endregion

        public event PainterSelected PainterSelectedEvent;

        public PainterViewModel(BasicPainter painter, int num, string name = "Untitled")
        {
            _painterModel = painter;
            _number = num;
            _name = name;

            _buttonColor = new SolidColorBrush(Color.FromRgb(_painterModel.Red, _painterModel.Green, _painterModel.Blue));
        }

        /// <summary>
        /// This view model has been clicked!
        /// </summary>
        public void SelectButton()
        {
            IsSelected = true;
            System.Diagnostics.Trace.WriteLine("Clicked!");

            SelectedEvent();
        }

        protected virtual void SelectedEvent()
        {

            PainterSelectedEvent?.Invoke(this);
        }


    }
}
