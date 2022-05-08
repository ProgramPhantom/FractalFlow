using System;
using System.Collections.Generic;
using System.Linq;
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
        private int _progressBar = 0;
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
        #endregion

        #region Methods
        public void CancelRender()
        {
            CancelRenderEvent?.Invoke();
        }
        #endregion
    }
}
