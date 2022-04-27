using Caliburn.Micro;
using FractalCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FractalGeneratorMVVM.ViewModels.Models.Painters
{
    public class PainterWhiteViewModel : Screen, IPainterViewModel
    {
        #region Fields
        private IPainter _painterModel;
        private string _name;
        private int _number;

        private Color _buttonColour;

        private LinearGradientBrush _buttonGradient;
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
            get { return _name; }
            set { _name = value; }
        }

        public LinearGradientBrush ButtonGradient
        {
            get { return _buttonGradient; }
            set { _buttonGradient = value; }
        }


        public Color ButtonColour
        {
            get { return _buttonColour; }
            set { _buttonColour = value; }
        }

        #endregion

        public PainterWhiteViewModel(PainterWhite painter, int num, string name = "Untitled")
        {
            _painterModel = painter;
            _number = num;
            _name = name;



            _buttonColour = Color.FromRgb(painter.Red, painter.Green, painter.Blue);

            // It is bugged so I cant make this in the xaml as the colour will not bind ): 
            _buttonGradient = new LinearGradientBrush();
            _buttonGradient.StartPoint = new System.Windows.Point(0, 0);
            _buttonGradient.EndPoint = new System.Windows.Point(1, 1);


            _buttonGradient.GradientStops.Add(new GradientStop(ButtonColour, 0.5));
            _buttonGradient.GradientStops.Add(new GradientStop(System.Windows.Media.Colors.White, 0.5));

        }
    }
}
