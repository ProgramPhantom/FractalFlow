using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalGeneratorMVVM.ViewModels.Controls
{
    public delegate void Render(bool ClearZoom);   // The method
    public delegate void ZoomOperation();   // The method
    public delegate void PainterOperation();   // The method
    public delegate void FileOperation();

    public class ToolRibbonViewModel : Screen
    {
        public event Render? FireRenderEvent;  // Event

        public event ZoomOperation? ZoomInEvent;
        public event ZoomOperation? ZoomOutEvent;
        public event ZoomOperation? ResetZoomEvent;

        public event PainterOperation? RandomPainterEvent;

        public event FileOperation? OpenFileEvent;
        public event FileOperation? SaveFractalEvent;

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
        public void ZoomInClicked()
        {
            OnZoomInClicked();
        }
        public void ZoomOutClicked()
        {
            OnZoomOutClicked();
        }
        public void ResetZoomClicked()
        {
            OnResetZoomClicked();
        }
        public void RandomPainterClicked()
        {
            OnRandomPainterClicked();
        }
        public void OpenFileClicked()
        {
            OnOpenFileClicekd();
        }
        public void SaveFractalClicked()
        {
            OnSaveFractalClicked();
        }

        protected virtual void OnRenderClicked() 
        {
            // Send the word that a render has been ordered!
            FireRenderEvent?.Invoke(false);
        }
        protected virtual void OnZoomInClicked()
        {
            // Send the word that a render has been ordered!
            ZoomInEvent?.Invoke();
        }
        protected virtual void OnZoomOutClicked()
        {
            ZoomOutEvent?.Invoke();
        }
        protected virtual void OnResetZoomClicked()
        {
            ResetZoomEvent?.Invoke();
        }
        protected virtual void OnRandomPainterClicked()
        {
            RandomPainterEvent?.Invoke();
            FireRenderEvent?.Invoke(false);
        }
        protected virtual void OnOpenFileClicekd()
        {
            OpenFileEvent?.Invoke();
        }
        protected virtual void OnSaveFractalClicked()
        {
            SaveFractalEvent?.Invoke();
        }

        #endregion
    }
}
