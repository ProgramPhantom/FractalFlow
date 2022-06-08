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
using FractalCore.Painting;
using FractalGeneratorMVVM.ViewModels.Pages;
using System.Numerics;
using System.Windows.Controls;

namespace FractalGeneratorMVVM
{
    /// <summary>
    /// Shell
    /// </summary>
    public class Kernel : PropertyChangedBase
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
        private float _zoomDivisor = 2;
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

        public FractalFrame? FakeFractalFrame = null;
        public FractalFrame ActiveFractalFrame
        {
            get
            {
                return FakeFractalFrame ?? SelectedFractalFrame;
            }
        }

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

        public float ZoomFactor
        {
            get { return _zoomDivisor; }
            set { _zoomDivisor = value; }
        }

        #endregion

        #region Constructor
        public Kernel()
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
            _defaultPage.CanvasVM.MouseOverCanvasEvent += CanvasHovered;
            #endregion

            #region Left Mouse Clicked
            _defaultPage.CanvasVM.LeftClickedCanvas += HardZoom;
            #endregion

            #region Toggle Console Window
            _defaultPage.StatusBarVM.ToggleConsoleEvent += ToggleConsoleWindowShow;
            #endregion

            #region Zooming Buttons
            _defaultPage.ToolRibbonVM.ResetZoomEvent += ResetZoom;
            _defaultPage.ToolRibbonVM.ZoomInEvent += DumbZoomIn;
            _defaultPage.ToolRibbonVM.ZoomOutEvent += DumbZoomOut;
            #endregion

            _defaultPage.ToolRibbonVM.RandomPainterEvent += _defaultPage.PainterStackVM.RandomPainter;

            // Show the window
            _windowManager.ShowWindowAsync(_mainWindow);


            _consolePage.NewLog(new Status("Done", NotificationType.OperationComplete));

            _renderEngine = new RenderEngine();
        }

        #endregion

        public void CanvasHovered(Point hoverLocation, double canvasWidth, double canvasHeight) 
        {
            Complex mousePos = ActiveFractalFrame.PxToComplex(hoverLocation, canvasWidth, canvasHeight);

            _defaultPage.StatusBarVM.UpdateHoverMessage(mousePos);
        }

        public void PreRender(bool clearZoom)
        {
            if (clearZoom == true) { FakeFractalFrame = null; }

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


            Fractal fractal = new Fractal(RenderWidth, RenderHeight, ActiveFractalFrame, SelectedIterator);
            FractalImage fractalImage = new FractalImage(ref fractal);

            ComputeIterationsJob computeJob = new ComputeIterationsJob(fractal, JobCount);
            JobCount++;
            RenderBitmapJob job = new RenderBitmapJob(computeJob, SelectedPainter, fractalImage, JobCount);
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

            Fractal fractal = new Fractal(RenderWidth, RenderHeight, ActiveFractalFrame, SelectedIterator);
            FractalImage fractalImage = new FractalImage(ref fractal);

            ComputeIterationsJob computeJob = new ComputeIterationsJob(fractal, JobCount);
            JobCount++;
            RenderBitmapJob job = new RenderBitmapJob(computeJob, SelectedPainter, fractalImage, JobCount);
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

        public void HardZoom(Point clickLocation, double width, double height)
        {
            Complex centre = ActiveFractalFrame.PxToComplex(clickLocation, width, height);

            ConsolePage.NewLog(new Status($"Zooming into point {centre.Real} + {centre.Imaginary}i", NotificationType.Zoom));

            double newWidth = ActiveFractalFrame.RealWidth / ZoomFactor;
            double newHeight = ActiveFractalFrame.ImaginaryHeight / ZoomFactor;

            FractalFrame newFrame = FractalFrame.FractalFrameCentre((float)newWidth, (float)newHeight, (float)centre.Real, (float)centre.Imaginary, 
                "Temporary Frame", SelectedFractalFrame.Iterations, SelectedFractalFrame.Bail);

            FakeFractalFrame = newFrame;

            PreRender(false);
        }
        public void HardZoom(bool zoomOut=false)
        {
            int width = DefaultPage.CanvasVM.Image.Width;
            int height = DefaultPage.CanvasVM.Image.Height;
            Point clickLocation = new Point(width/2, height/2);
            

            Complex centre = ActiveFractalFrame.PxToComplex(clickLocation, width, height);

            ConsolePage.NewLog(new Status($"Zooming into point {centre.Real} + {centre.Imaginary}i", NotificationType.Zoom));

            double newWidth = zoomOut ? ActiveFractalFrame.RealWidth * ZoomFactor : ActiveFractalFrame.RealWidth / ZoomFactor;
            double newHeight = zoomOut ? ActiveFractalFrame.ImaginaryHeight * ZoomFactor : ActiveFractalFrame.ImaginaryHeight / ZoomFactor;

            FractalFrame newFrame = FractalFrame.FractalFrameCentre((float)newWidth, (float)newHeight, (float)centre.Real, (float)centre.Imaginary,
                "Temporary Frame", SelectedFractalFrame.Iterations, SelectedFractalFrame.Bail);

            FakeFractalFrame = newFrame;

            PreRender(false);
        }

        public void ResetZoom()
        {
            PreRender(true);
        }
        public void DumbZoomIn()
        {
            HardZoom();
        }
        public void DumbZoomOut()
        {
            HardZoom(true);
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
