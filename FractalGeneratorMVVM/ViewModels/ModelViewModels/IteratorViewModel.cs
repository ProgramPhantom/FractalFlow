using Caliburn.Micro;
using FractalCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalGeneratorMVVM.ViewModels
{
    public delegate void IteratorSelected(IteratorViewModel sender);

    public class IteratorViewModel : Screen
    {
        #region Fields
        private IIterator _iteratorModel;

        private int _number;

        private bool _isSelected;
        #endregion

        #region Properties
        public IIterator IteratorModel
        {
            get { return _iteratorModel; }
            set { _iteratorModel = value; }
        }

        public string FormulaString
        {
            get
            {
                return _iteratorModel.FormulaString;
            }
        }

        public string Name
        {
            get { return _iteratorModel.Name; }
            set { _iteratorModel.Name = value; }
        }

        public int Number
        {
            get { return _number; }
            set { _number = value; }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set 
            { 
                _isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);
            }
        }

        public event IteratorSelected IteratorSelecetedEvent;
        #endregion

        public IteratorViewModel(IIterator iteratorModel, int num)
        {
            _iteratorModel = iteratorModel;
            _number = num;
            _isSelected = false;
            
        }

        public void SelectButton()
        {
            IsSelected = true;

            SelectedEvent();
        }

        protected virtual void SelectedEvent()
        {

            IteratorSelecetedEvent?.Invoke(this);
        }


    }
}
