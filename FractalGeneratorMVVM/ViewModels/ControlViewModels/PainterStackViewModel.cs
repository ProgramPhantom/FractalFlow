using Caliburn.Micro;
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
        public EventHandler NewPainter;

        private BindableCollection<IPainter> _painterCollection;

        public BindableCollection<IPainter> PainterCollection
        {
            get { return _painterCollection; }
            set
            {
                _painterCollection = value;
                NotifyOfPropertyChange(() => PainterCollection);
            }
        }

        private IPainter _selectedPainter;

        public IPainter SelectedPainter
        {
            get { return _selectedPainter; }
            set
            {
                _selectedPainter = value;
                NotifyOfPropertyChange(() => SelectedPainter);
            }
        }



        public PainterStackViewModel()
        {
            _painterCollection = new BindableCollection<IPainter>();

            PainterCollection.Add(new BasicPainter("Red", 255, 0, 0));
            PainterCollection.Add(new BasicPainter("Green", 0, 255, 0));
            PainterCollection.Add(new BasicPainter("Blue", 0, 0, 255));


            _selectedPainter = _painterCollection[0];


        }

        public void AddPainter()
        {
            NewPainter?.Invoke(this, new EventArgs());
        }
    }
}
