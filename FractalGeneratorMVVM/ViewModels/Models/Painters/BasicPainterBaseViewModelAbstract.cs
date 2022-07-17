using FractalCore.Painting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FractalGeneratorMVVM.ViewModels.Models.Painters
{
    /// <summary>
    /// This is a viewless model used to hold shared properties between BasicPainterDarkViewModel
    /// and BasicPainterLightViewModel. It mirrors the backend, where there is corresponding 
    /// intermediary class
    /// </summary>
    public class BasicPainterBaseViewModelAbstract : IPainterViewModel
    {
        #region Fields
        private IPainter _painterModel;
        private string _name;
        private int _number;
        private bool _type;

        private SolidColorBrush _mainColourBrush;
        private SolidColorBrush _inSetColourBrush;
        #endregion

        #region Properties

        /// <summary>
        /// ALWAYS A BASIC PAINTER. But this is needed to fufill the Interface 
        /// </summary>
        public IPainter PainterModel
        {
            get { return _painterModel; }
            set { _painterModel = value; }
        }

        public int Number
        {
            get { return _number; }
            set { _number = value; }
        }

        public SolidColorBrush MainColourBrush
        {
            get { return _mainColourBrush; }
            set { _mainColourBrush = value; }
        }

        public SolidColorBrush InSetColourBrush
        {
            get { return _inSetColourBrush; }
            set { _inSetColourBrush = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }  
        }
        
        public bool Type
        {
            get { return _type; }
            set { _type = value; }
        }


        public Guid ID { get; } = Guid.NewGuid();
        #endregion

        public BasicPainterBaseViewModelAbstract(BasicPainterBase painter, int num, bool type, string name = "Untitled")
        {
            _type = type;

            _painterModel = painter;
            _number = num;
            _name = name;

            _mainColourBrush = new SolidColorBrush(painter.MainColour);
            _inSetColourBrush = new SolidColorBrush(painter.InSetColour);
        }
    }
}
