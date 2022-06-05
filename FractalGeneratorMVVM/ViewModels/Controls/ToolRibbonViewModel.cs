using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalGeneratorMVVM.ViewModels.Controls
{
    public delegate void Render(bool ClearZoom);   // The method

    public class ToolRibbonViewModel : Screen
    {
        public event Render? FireRenderEvent;  // Event


        #region Fields
        private bool _gpuRender;
        private UInt16 _width = 500;
        private UInt16 _height = 500;

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

        public UInt16 Width
        {
            get { return _width; }
            set 
            { 
                _width = value;
                NotifyOfPropertyChange(() => Width);
            }
        }



        public UInt16 Height
        {
            get { return _height; }
            set 
            { 
                _height = value;
                NotifyOfPropertyChange(() => Height);
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
            FireRenderEvent?.Invoke(true);
        }

        #endregion
    }
}
