using Caliburn.Micro;
using FractalCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalGeneratorMVVM.ViewModels
{
    public class IteratorStackViewModel : Screen
    {
        private BindableCollection<BasicIterator> _iteratorCollection;

        public BindableCollection<BasicIterator> IteratorCollection
        {
            get
            {
                return _iteratorCollection;
            }
            set
            {
                _iteratorCollection = value;
                NotifyOfPropertyChange(() => IteratorCollection);
            }
        }

        private IIterator _selectedIterator;

        public IIterator SelectedIterator
        {
            get { return _selectedIterator; }
            set { _selectedIterator = value; }
        }


        public IteratorStackViewModel()
        {
            _iteratorCollection = new BindableCollection<BasicIterator>();

            _iteratorCollection.Add(new BasicIterator("Mandelbrot Set", "z^2 + c"));
            _selectedIterator = _iteratorCollection[0];
        }
    }
}
