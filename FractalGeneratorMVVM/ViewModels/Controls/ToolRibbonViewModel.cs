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

        #region Methods

        public void RenderClicked()
        {
            System.Diagnostics.Trace.WriteLine("Render clicked");

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
