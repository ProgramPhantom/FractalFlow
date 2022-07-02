using Caliburn.Micro;
using FractalCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalGeneratorMVVM.ViewModels.Models
{
    public delegate void IteratorSelected(IteratorViewModel sender);

    public class IteratorViewModel : Screen
    {
        public event IteratorSelected IteratorSelecetedEvent;

        #region Fields
        private BasicIterator _iteratorModel;
        private int _number;
        private bool _isSelected;
        
        #endregion

        #region Properties
        public BasicIterator IteratorModel
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


        public string LaTEX
        {   
            get { return IteratorModel.FormulaObject.LaTEX; }
        }

        #endregion

        public IteratorViewModel()
        {
            _iteratorModel = new BasicIterator();
            _number = 0;
            _isSelected = false;
            
        }

        public IteratorViewModel(BasicIterator iteratorModel, int num)
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
