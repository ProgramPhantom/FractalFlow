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

        private bool _consoleOpen = false;


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

        #region Exposers
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
        public ushort RenderHeight
        {
            get
            {
                return DefaultPage.ToolRibbonVM.Height;
            }
        }
        public ushort RenderWidth
        {
            get
            {
                return DefaultPage.ToolRibbonVM.Width;
            }
        }
        #endregion

        public int JobCount
        {
            get { return _renders; }
            set { _renders = value; }
        }
        public bool ConsoleOpen
        {
            get { return _consoleOpen; }
            set { _consoleOpen = value; }
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

            _defaultPage.StatusBarVM.ToggleConsoleEvent += ToggleConsoleWindowShow;

            // Show the window
            _windowManager.ShowWindowAsync(_mainWindow);


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

            
        }

        public void ToggleConsoleWindowShow()
        {
            ConsoleOpen = !ConsoleOpen;

            if (ConsoleOpen == true)
            {
                _windowManager.ShowWindowAsync(ConsoleWindow);
            } else
            {
                ConsoleWindow.TryCloseAsync();
            }
        }

        public async void RenderAsync()
        {
            #region Timer start
            Stopwatch timer = new Stopwatch();
            timer.Start();
            #endregion


            cts = new CancellationTokenSource();  // Set up the cancel thing
            // Progress is a metadata thing
            Progress<RenderProgressModel> progress = new Progress<RenderProgressModel>();  // Set up a progress monitor using the render progress model in Fractal Core
            progress.ProgressChanged += ReportProgress;  // Call this method when there is a progress update


            Fractal fractal = new Fractal(RenderWidth, RenderHeight, DefaultPage.SelectedFractalFrame, DefaultPage.SelectedIterator);
            FractalImage fractalImage = new FractalImage(ref fractal);

            ComputeIterationsJob computeJob = new ComputeIterationsJob(fractal, JobCount);
            JobCount++;
            RenderBitmapJob job = new RenderBitmapJob(computeJob, DefaultPage.SelectedPainter, fractalImage, JobCount);
            JobCount++;

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

            #region Timer end
            timer.Stop();
            TimeSpan ts = timer.Elapsed;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

            ConsolePage.NewLog(new Status($"Overall render duration: {elapsedTime}", NotificationType.RenderDuration));
            #endregion
        }

        public async void CLRenderAsync()
        {
            #region Timer start
            Stopwatch timer = new Stopwatch();
            timer.Start();
            #endregion

            cts = new CancellationTokenSource();  // Set up the cancel thing

            Progress<RenderProgressModel> progress = new Progress<RenderProgressModel>();  // Set up a progress monitor using the render progress model in Fractal Core
            progress.ProgressChanged += ReportProgress;  // Call this method when there is a progress update

            Fractal fractal = new Fractal(RenderWidth, RenderHeight, DefaultPage.SelectedFractalFrame, DefaultPage.SelectedIterator);
            FractalImage fractalImage = new FractalImage(ref fractal);

            ComputeIterationsJob computeJob = new ComputeIterationsJob(fractal, JobCount);
            JobCount++;
            RenderBitmapJob job = new RenderBitmapJob(computeJob, DefaultPage.SelectedPainter, fractalImage, JobCount);
            JobCount++;

            computeJob.StatusUpdateEvent += _consolePage.NewLog;
            job.StatusUpdateEvent += _consolePage.NewLog;

            try
            {
                await RenderEngine.CLBitmapCompute(job, progress, cts.Token);
            }
            catch (OperationCanceledException)
            {

            }
            

            DefaultPage.CanvasVM.Image = fractalImage;

            #region Timer end
            timer.Stop();
            TimeSpan ts = timer.Elapsed;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

            ConsolePage.NewLog(new Status($"Overall render duration: {elapsedTime}", NotificationType.RenderDuration));
            #endregion
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
