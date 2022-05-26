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
        CancellationTokenSource cts = new CancellationTokenSource();

        #region Feilds

        private readonly IWindowManager _windowManager;

        private DefaultWindowViewModel _mainWindow;
        private DefaultWindowViewModel _consoleWindow;
        private DefaultPageViewModel _defaultPage;
        private ConsolePageViewModel _consolePage;
        private RenderEngine _renderEngine;
        private int _renders;

        #endregion

        #region Properties
        public DefaultWindowViewModel MainWindow
        {
            get { return _mainWindow; }
            set { _mainWindow = value; }
        }
        public DefaultWindowViewModel ConsoleWindow
        {
            get { return _consoleWindow; }
            set { _consoleWindow = value; }
        }

        /// <summary>
        /// The page which is given to the main window to display
        /// </summary>
        public DefaultPageViewModel DefaultPage
        {
            get { return _defaultPage; }
            set { _defaultPage = value; }
        }
        public ConsolePageViewModel ConsolePage
        {
            get { return _consolePage; }
            set { _consolePage = value; }
        }


        public RenderEngine RenderEngine
        {
            get { return _renderEngine; }
            set { _renderEngine = value; }
        }

        public FractalFrame SelectedFractalFrame
        {
            get
            {
                return DefaultPage.SelectedFractalFrame;
            }
        }
        public IPainter SelectedPainter
        {
            get
            {
                return DefaultPage.SelectedPainter;
            }
        }
        public IIterator SelectedIterator
        {
            get
            {
                return DefaultPage.SelectedIterator;
            }
        }

        public int JobCount
        {
            get { return _renders; }
            set { _renders = value; }
        }
        #endregion

        #region Constructor
        public Shell()
        {
            _windowManager = new WindowManager();

            _consolePage = new ConsolePageViewModel();
            _defaultPage = new DefaultPageViewModel();

            // Create the main window
            _mainWindow = new DefaultWindowViewModel(_defaultPage, "Untitled", ResizeMode.CanResizeWithGrip);
            _consoleWindow = new DefaultWindowViewModel(_consolePage, "Console", ResizeMode.CanResizeWithGrip);

            #region Render Button Event
            // Wire up the Render Button in the ToolRibbon to the render here in the shell 😀
            _defaultPage.ToolRibbonVM.FireRenderEvent += PreRender;
            #endregion

            #region Cancel Render Event
            // Wire up the cancel render button
            _defaultPage.StatusBarVM.CancelRenderEvent += CancelRender;
            #endregion

            #region Mouse hover event linking
            // Send the mouse hover data over to the selected fractal frame so that it can be processed into a complex number
            _defaultPage.CanvasVM.MouseOverCanvasEvent += _defaultPage.SelectedFractalFrame.PxToComplex;
            // The fractal frame then fires the ComplexHoveredEvent event so everyone can have access to the complex number the mouse is hovering over
            _defaultPage.SelectedFractalFrame.ComplexHoveredEvent += _defaultPage.StatusBarVM.UpdateHoverMessage;
            #endregion


            // Show the window
            _windowManager.ShowWindowAsync(_mainWindow);

            _windowManager.ShowWindowAsync(_consoleWindow);

            _consolePage.NewLog(new Status("Done", NotificationType.OperationComplete));

            _renderEngine = new RenderEngine();
        }

        #endregion

        public void PreRender(object? sender, EventArgs e)
        {
            if (DefaultPage.ToolRibbonVM.GPURender == true)
            {
                CLRenderAsync();
            } else
            {
                RenderAsync();
            }

            JobCount += 2;
        }

        public async void RenderAsync()
        {
            Trace.WriteLine("Message recieved over in the shell");

            cts = new CancellationTokenSource();  // Set up the cancel thing
            // Progress is a metadata thing
            Progress<RenderProgressModel> progress = new Progress<RenderProgressModel>();  // Set up a progress monitor using the render progress model in Fractal Core
            progress.ProgressChanged += ReportProgress;  // Call this method when there is a progress update


            Fractal fractal = new Fractal(100, 100, DefaultPage.SelectedFractalFrame, DefaultPage.SelectedIterator);
            FractalImage fractalImage = new FractalImage(ref fractal);

            ComputeIterationsJob computeJob = new ComputeIterationsJob(fractal, JobCount);
            RenderBitmapJob job = new RenderBitmapJob(computeJob, DefaultPage.SelectedPainter, fractalImage, JobCount);

            computeJob.StatusUpdateEvent += _consolePage.NewLog;
            job.StatusUpdateEvent += _consolePage.NewLog;

            

            try
            {
                await RenderEngine.BitmapComputeAsync(job, progress, cts.Token);
            }
            catch (OperationCanceledException)
            {

            }

            // Set the image of the new canvas to the newley rendered fractal painted with the selected painter.
            DefaultPage.CanvasVM.Image = fractalImage;

        }

        public async void CLRenderAsync()
        {
            Progress<RenderProgressModel> progress = new Progress<RenderProgressModel>();  // Set up a progress monitor using the render progress model in Fractal Core
            progress.ProgressChanged += ReportProgress;  // Call this method when there is a progress update

            Fractal fractal = new Fractal(10000, 10000, DefaultPage.SelectedFractalFrame, DefaultPage.SelectedIterator);
            FractalImage fractalImage = new FractalImage(ref fractal);

            ComputeIterationsJob computeJob = new ComputeIterationsJob(fractal, JobCount);
            RenderBitmapJob job = new RenderBitmapJob(computeJob, DefaultPage.SelectedPainter, fractalImage, JobCount);

            computeJob.StatusUpdateEvent += _consolePage.NewLog;
            job.StatusUpdateEvent += _consolePage.NewLog;


            await RenderEngine.CLBitmapCompute(job, progress);

            DefaultPage.CanvasVM.Image = fractalImage;
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
