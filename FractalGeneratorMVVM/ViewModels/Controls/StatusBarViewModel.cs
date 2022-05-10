using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace FractalGeneratorMVVM.ViewModels.Controls
{
    public delegate void CancelRender();   // The method

    public class StatusBarViewModel : Screen
    {
        public event CancelRender? CancelRenderEvent;  // Event

        #region Fields
        private int _progressBar = 100;
        private string _hoverLocationString = "";
        #endregion


        #region Properties
        public int ProgressBar
        {
            get { return _progressBar; }
            set
            {
                _progressBar = value;
                NotifyOfPropertyChange(() => ProgressBar);
            }
        }


        public string HoverLocationString
        {
            get { return _hoverLocationString; }
            set 
            { 
                _hoverLocationString = value;
                NotifyOfPropertyChange(() => HoverLocationString);
            }
        }

        #endregion

        #region Methods
        public void CancelRender()
        {
            CancelRenderEvent?.Invoke();
        }

        public void UpdateHoverMessage(Complex p)
        {
            HoverLocationString = $"{(p.Real > 0 ? " " : string.Empty)} {Math.Round(p.Real, 10).ToString("N10")} {(p.Imaginary > 0 ? '+' : '-')} i{(Math.Round(Math.Abs(p.Imaginary), 10).ToString("N10"))}";

            
        }
        #endregion
    }
}
