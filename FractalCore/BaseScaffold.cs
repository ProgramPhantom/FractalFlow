using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FractalCore
{

    /// <summary>
    /// Holds the default value for the Fractal Scaffold class
    /// </summary>
    public class BaseScaffold
    {
        private static float _leftDefault = -3.1f;
        private static float _rightDefault = 1.1f;
        private static float _bottomDefault = -2.1f;
        private static float _topDefault = 2.1f;
        private static string _nameDefault = "Untitled";
        private static uint _iterationsDefault = 100;
        private static int _bailDefault = 2;

        #region Universal
        public static string NameDefault
        {
            get { return _nameDefault; }
            set { _nameDefault = value; }
        }

        public static uint IterationsDefault
        {
            get { return _iterationsDefault; }
            set { _iterationsDefault = value; }
        }

        public static int BailDefault
        {
            get { return _bailDefault; }
            set { _bailDefault = value; }
        }
        #endregion

        #region Sides
        public static float LeftDefault
        {
            get { return _leftDefault; }
            set { _leftDefault = value; }
        }

        public static float RightDefault
        {
            get { return _rightDefault; }
            set { _rightDefault = value; }
        }

        public static float BottomDefault
        {
            get { return _bottomDefault; }
            set { _bottomDefault = value; }
        }

        public static float TopDefault
        {
            get { return _topDefault; }
            set { _topDefault = value; }
        }
        #endregion

        #region Centre
        public static float WidthDefault
        {
            get
            {
                return Math.Abs(LeftDefault) + Math.Abs(RightDefault);
            }
        }

        public static float HeightDefault
        {
            get
            {
                return Math.Abs(TopDefault) + Math.Abs(BottomDefault);
            }
        }

        public static float CentreRealDefault
        {
            get
            {
                return (float)Decimal.Divide((decimal)WidthDefault, 2) + LeftDefault;
            }
        }

        public static float CentreImagDefault
        {
            get
            {
                return (float)Decimal.Divide((decimal)HeightDefault, 2) + BottomDefault;
            }
        }
        #endregion
    }
}
