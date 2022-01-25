using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalCore
{

    /// <summary>
    /// Soved a few default values in here
    /// </summary>
    public class BaseScaffold
    {
        private static float _leftDefault = -1.1f;
        private static float _rightDefault = 1.1f;
        private static float _bottomDefault = -1.1f;
        private static float _topDefault = 1.1f;
        private static string _nameDefault = "Untitled";
        private static int _iterationsDefault = 100;
        private static int _bailDefault = 2;

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

        public static string NameDefault
        {
            get { return _nameDefault; }
            set { _nameDefault = value; }
        }

        public static int IterationsDefault
        {
            get { return _iterationsDefault; }
            set { _iterationsDefault = value; }
        }


        public static int BailDefault
        {
            get { return _bailDefault; }
            set { _bailDefault = value; }
        }

    }
}
