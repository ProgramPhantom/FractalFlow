using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FractalCore;
using System.Windows;
using Caliburn.Micro;
using System.Windows.Media;

namespace FractalGeneratorMVVM.ViewModels.Models.Painters
{

    public class BasicPainterViewModel : Screen, IPainterViewModel
    {
        #region Fields
        private IPainter _painterModel;
        private string _name;
        private int _number;

        private SolidColorBrush _buttonColor;
        #endregion

        #region Properties
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

        public string Name
        {
            get { return _name ; }
            set { _name = value; }
        }

        public SolidColorBrush ButtonColour
        {
            get { return _buttonColor; }
            set { _buttonColor = value; }
        }
        #endregion

        public BasicPainterViewModel(BasicPainter painter, int num, string name = "Untitled")
        {
            _painterModel = painter;
            _number = num;
            _name = name;

            _buttonColor = new SolidColorBrush(Color.FromRgb(painter.Red, painter.Green, painter.Blue));
        }

    }
}
