using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FractalGeneratorMVVM.Views;
using FractalGeneratorMVVM.ViewModels;
using FractalGeneratorMVVM.ViewModels.Windows;
using System.Diagnostics;
using FractalCore;
using System.Threading;

namespace FractalGeneratorMVVM
{
    /// <summary>
    /// Shell
    /// </summary>
    public class Shell : PropertyChangedBase
    {
        #region Feilds

        private readonly IWindowManager _windowManager;

        private DefaultWindowViewModel _mainWindow;

        private DefaultPageViewModel _defaultPage;

        #endregion

        #region Properties

        /// <summary>
        /// The page which is given to the main window to display
        /// </summary>
        public DefaultPageViewModel DefaultPage
        {
            get { return _defaultPage; }
            set { _defaultPage = value; }
        }

        CancellationTokenSource cts = new CancellationTokenSource();
        #endregion

        #region Constructor
        public Shell()
        {
            _windowManager = new WindowManager();

           
            _defaultPage = new DefaultPageViewModel();

            // Create the main window
            _mainWindow = new DefaultWindowViewModel(_defaultPage, "Untitled", ResizeMode.CanResizeWithGrip);

            // Wire up the Render Button in the ToolRibbon to the render here in the shell 😀
            _defaultPage.ToolRibbonVM.FireRenderEvent += RenderAsync;

            // Wire up the cancel render button
            _defaultPage.StatusBarVM.CancelRenderEvent += CancelRender;

            #region Mouse hover event linking
            // Send the mouse hover data over to the selected fractal frame so that it can be processed into a complex number
            _defaultPage.CanvasVM.MouseOverCanvasEvent += _defaultPage.SelectedFractalFrame.PxToComplex;
            // The fractal frame then fires the ComplexHoveredEvent event so everyone can have access to the complex number the mouse is hovering over
            _defaultPage.SelectedFractalFrame.ComplexHoveredEvent += _defaultPage.StatusBarVM.UpdateHoverMessage;
            #endregion


            // Show the window
            _windowManager.ShowWindowAsync(_mainWindow);


        }

        #endregion

        public async void RenderAsync(object? sender, EventArgs e)
        {
            Trace.WriteLine("Message recieved over in the shell");

            cts = new CancellationTokenSource();  // Set up the cancel thing


            Fractal fractal = new Fractal(500, 500, DefaultPage.SelectedFractalFrame, DefaultPage.SelectedIterator);

            // Progress is a metadata thing
            Progress<RenderProgressModel> progress = new Progress<RenderProgressModel>();  // Set up a progress monitor using the render progress model in Fractal Core
            progress.ProgressChanged += ReportProgress;  // Call this method when there is a progress update

            try
            {
                await fractal.GenerateProgressAsync(progress, cts.Token);
            }
            catch (OperationCanceledException)
            {

            }

            // Set the image of the new canvas to the newley rendered fractal painted with the selected painter.
            DefaultPage.CanvasVM.Image = new FractalImage(ref fractal, DefaultPage.SelectedPainter);

        }

        public void CancelRender()
        {
            cts.Cancel();

            DefaultPage.StatusBarVM.ProgressBar = 0;

        }

        private void ReportProgress(object? sender, RenderProgressModel e)
        {
            DefaultPage.StatusBarVM.ProgressBar = e.PercentageComplete;

        }

    }
}
