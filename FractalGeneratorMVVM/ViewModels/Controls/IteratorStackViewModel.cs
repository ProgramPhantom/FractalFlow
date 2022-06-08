using Caliburn.Micro;
using FractalCore;
using FractalGeneratorMVVM.ViewModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FractalGeneratorMVVM.ViewModels.Controls
{
    public class IteratorStackViewModel : Screen
    {
        #region Fields
        private BindableCollection<IteratorViewModel> _iteratorViewModels;

        private IteratorViewModel _selectedIteratorVM;

        private string _formulaBox = "z^2 + c";
        #endregion

        #region Properties
        /// <summary>
        /// Holds the value of the formula box
        /// </summary>
        public string FormulaBox
        {
            get { return _formulaBox; }
            set
            {
                _formulaBox = value;
                NotifyOfPropertyChange(() => FormulaBox);
            }
        }

        /// <summary>
        /// Collection of ViewModels for iterators
        /// </summary>
        public BindableCollection<IteratorViewModel> IteratorViewModels
        {
            get
            {
                return _iteratorViewModels;
            }
            set
            {
                _iteratorViewModels = value;
                NotifyOfPropertyChange(() => IteratorViewModels);
            }
        }

        /// <summary>
        /// The selected Iterator View Model
        /// </summary>
        public IteratorViewModel SelectedIteratorVM
        {
            get { return _selectedIteratorVM; }
            set
            {
                _selectedIteratorVM = value;
                
                NotifyOfPropertyChange(() => SelectedIteratorVM);
            }
        }

        /// <summary>
        /// Presents the IteratorModel under the SelectedIterator ViewModel
        /// </summary>
        public IIterator SelectedIterator
        {
            get
            {
                return SelectedIteratorVM.IteratorModel;
            }
        }
        #endregion


        #region Constructor
        public IteratorStackViewModel()
        {
            _iteratorViewModels = new BindableCollection<IteratorViewModel>();

            AddIterator();

            _formulaBox = "";
            _selectedIteratorVM = _iteratorViewModels[0];
        }
        #endregion

        #region Methods
        public void OnIteratorSelected(IteratorViewModel sender)
        {
            System.Diagnostics.Trace.WriteLine("Clicked Iterator");

            int index = _iteratorViewModels.IndexOf(sender);

            SelectedIteratorVM = sender;

            foreach (IteratorViewModel vm in _iteratorViewModels)
            {
                // Deselect all of the other iterators
                vm.IsSelected = false;
            }

            _iteratorViewModels[index].IsSelected = true;

        }

        public void AddIterator()
        {
            if (string.IsNullOrEmpty(FormulaBox)) { return; }

            BasicIterator iterator = new BasicIterator(FormulaBox);


            _iteratorViewModels.Add(new IteratorViewModel(iterator, _iteratorViewModels.Count() + 1));

            _iteratorViewModels.Last().IteratorSelecetedEvent += OnIteratorSelected;

            
        }

        public void Enter(KeyEventArgs k)
        {
            if (k.Key == Key.Return)
            {
                AddIterator();
            }
        }
        #endregion
    }
}
