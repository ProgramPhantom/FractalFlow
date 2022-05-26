using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalGeneratorMVVM.ViewModels.Controls
{
    public delegate void Render(object obj, EventArgs e);   // The method

    public class ToolRibbonViewModel : Screen
    {
        public event Render? FireRenderEvent;  // Event


        #region Fields
        private bool _gpuRender;
        #endregion

        #region Properties
        public bool GPURender
        {
            get { return _gpuRender; }
            set 
            { 
                _gpuRender = value;
                NotifyOfPropertyChange(() => GPURender);
            }
        }
        #endregion



        #region Methods


        public void RenderClicked()
        {
            OnRenderClicked();
        }

        protected virtual void OnRenderClicked() 
        {
            // Send the word that a render has been ordered!
            FireRenderEvent?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
