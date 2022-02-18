using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FractalCore;
using Caliburn.Micro;

namespace FractalGeneratorMVVM.ViewModels
{
    public class FractalFrameViewModel : Screen
    {
        #region Fields
        private FractalFrame _fractalFrameModel;
        private string _name;
        private int _number;
        private bool _isSelected;
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
        #endregion

        public FractalFrameViewModel(int num, string name="Untitled")
        {
            _fractalFrameModel = new FractalFrame();

            _number = num;
            _name = name;
        }


    }
}
