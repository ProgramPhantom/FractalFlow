using Caliburn.Micro;
using FractalGeneratorMVVM.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalGeneratorMVVM.ViewModels
{
    public class DefaultPageViewModel : Screen
    {
        #region Fields

        private FractalFrameStackViewModel _fractalFrameStackVM;

        private IteratorStackViewModel _iteratorStackVM;

        private PainterStackViewModel _painterStackVM;

        private ToolRibbonViewModel _toolRibbonVM;


        private StatusBarViewModel _statusBarVM;
        #endregion

        #region Properties
        /// <summary>
        /// The vertical list holding the Fractal Frames
        /// </summary>
        public FractalFrameStackViewModel FractalFrameStackVM
        {
            get { return _fractalFrameStackVM; }
            set { _fractalFrameStackVM = value; }
        }

        /// <summary>
        /// The vertical list containing the Iterators
        /// </summary>
        public IteratorStackViewModel IteratorStackVM
        {
            get { return _iteratorStackVM; }
            set { _iteratorStackVM = value; }
        }

        /// <summary>
        /// The vertical list containing the Painters
        /// </summary>
        public PainterStackViewModel PainterStackVM
        {
            get { return _painterStackVM; }
            set { _painterStackVM = value; }
        }
        
        /// <summary>
        /// The options at the top of the scree
        /// </summary>
        public ToolRibbonViewModel ToolRibbonVM
        {
            get { return _toolRibbonVM; }
            set { _toolRibbonVM = value; }
        }


        /// <summary>
        /// The bar at the bottom of the window which tells you essential info
        /// </summary>
        public StatusBarViewModel StatusBarVM
        {
            get { return _statusBarVM; }
            set { _statusBarVM = value; }
        }

        #endregion

        #region Constructor
        public DefaultPageViewModel()
        {
            _fractalFrameStackVM = new FractalFrameStackViewModel();
            _iteratorStackVM = new IteratorStackViewModel();
            _painterStackVM = new PainterStackViewModel();
            _toolRibbonVM = new ToolRibbonViewModel();
            _statusBarVM = new StatusBarViewModel();
        }

        #endregion



    }
}
