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
using System.Xml.Serialization;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using FractalGeneratorMVVM.ViewModels.Models;
using FractalGeneratorMVVM.ViewModels.Models.Painters;
using System.Windows.Media;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace FractalGeneratorMVVM
{
    /// <summary>
    /// Shell
    /// </summary>
    public class Kernel : PropertyChangedBase
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        Progress<RenderProgressModel> progress = new Progress<RenderProgressModel>();

        #region Feilds

        private readonly IWindowManager _windowManager;

        private DefaultWindowViewModel _mainWindow;
        private DefaultWindowViewModel _consoleWindow;
        private DefaultPageViewModel _defaultPage;
        private ConsolePageViewModel _consolePage;
        private RenderEngine _renderEngine;
        private int _renders;

        private Fractal? _activeFractal;

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
        public FractalFrame? SelectedFractalFrame
        {
            get
            {
                return DefaultPage.SelectedFractalFrame;
            }
        }
        public FractalFrame? FakeFractalFrame = null;
        public FractalFrame? ActiveFractalFrame
        {
            get
            {
                return SelectedFractalFrame == null ? null : FakeFractalFrame ?? SelectedFractalFrame;  // Beautiful
                // Ternary conditional operator and null-coalescing operator in one line!!
            }
        }
        public IPainter? SelectedPainter
        {
            get
            {
                return DefaultPage.SelectedPainter;
            }
        }
        public BasicIterator? SelectedIterator
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

        /// <summary>
        /// Active Fractal is to be null until a render or save
        /// </summary>
        public Fractal? ActiveFractal;
        public bool NoObjectNull
        {
            get
            {
                return !(ActiveFractalFrame == null || SelectedPainter == null || SelectedIterator == null);
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


            WireEvents();


            _consolePage.NewLog(new Status("Done", NotificationType.OperationComplete));

            _renderEngine = new RenderEngine();

            
        }

        public Kernel(BindableCollection<FractalFrameViewModel> fractalFrameViewModels,
            BindableCollection<IPainterViewModel> painterViewModels, BindableCollection<IteratorViewModel> iteratorViewModels)
        {
            _windowManager = new WindowManager();

            _consolePage = new ConsolePageViewModel();
            _defaultPage = new DefaultPageViewModel(fractalFrameViewModels, painterViewModels, iteratorViewModels);

            // Create the main window
            _mainWindow = new DefaultWindowViewModel(_defaultPage, "Untitled", ResizeMode.CanResizeWithGrip);
            _consoleWindow = new DefaultWindowViewModel(_consolePage, "Console", ResizeMode.CanResizeWithGrip);


            WireEvents();


            _consolePage.NewLog(new Status("Done", NotificationType.OperationComplete));

            _renderEngine = new RenderEngine();


        }

        public void WireEvents()
        {
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

            #region Random Painter
            _defaultPage.ToolRibbonVM.RandomPainterEvent += _defaultPage.PainterStackVM.RandomPainter;
            #endregion

            #region File Operations
            _defaultPage.ToolRibbonVM.OpenFileEvent += OpenFrac;
            _defaultPage.ToolRibbonVM.SaveFractalEvent += SaveFrac;

            MainWindow.CTRL_S += SaveFrac;
            #endregion
        }
        #endregion

        public void CanvasHovered(Point hoverLocation, double canvasWidth, double canvasHeight) 
        {
            Complex mousePos;
        
            mousePos = ActiveFractal.FractalFrame.PxToComplex(hoverLocation, canvasWidth, canvasHeight);
          
            

            _defaultPage.StatusBarVM.UpdateHoverMessage(mousePos);
        }

        public void PreRender(bool clearZoom=false)
        {
            

            if (!NoObjectNull)  // IF one of the key objects is missing, complain.
            {
                MessageBox.Show($"Please select: {(ActiveFractalFrame == null ? "Fractal Frame, " : "")} " +
                    $"{(SelectedIterator == null ? "Iterator, " : "")}" +
                    $"{(SelectedPainter == null ? "Painter, " : "")}", "Render Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (clearZoom == true) { FakeFractalFrame = null; }  // Remove the fake fractal
            // Make sure this happens before the Fractal is created

            // Create the space in memory 
            ActiveFractal = new Fractal(RenderWidth, RenderHeight, ActiveFractalFrame!, SelectedIterator!);

            


            cts = new CancellationTokenSource();  // Set up the cancel thing
            progress = new Progress<RenderProgressModel>();  // Set up a progress monitor using the render progress model in Fractal Core
            progress.ProgressChanged += ReportProgress;  // Call this method when there is a progress update


            if (DefaultPage.ToolRibbonVM.GPURender == true)
            {
                CLRenderAsync();
            } else
            {
                RenderAsync();
            }
        }

        
        public async void RenderAsync()
        {
            #region Timer start
            Stopwatch timer = new Stopwatch();
            timer.Start();
            #endregion
            
            FractalImage fractalImage = new FractalImage(ref ActiveFractal!);

            ComputeIterationsJob computeJob = new ComputeIterationsJob(ActiveFractal, JobCount);
            JobCount++;
            RenderBitmapJob job = new RenderBitmapJob(computeJob, SelectedPainter!, fractalImage, JobCount);
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

            FractalImage fractalImage = new FractalImage(ref ActiveFractal!);

            ComputeIterationsJob computeJob = new ComputeIterationsJob(ActiveFractal, JobCount);
            JobCount++;
            RenderBitmapJob job = new RenderBitmapJob(computeJob, SelectedPainter!, fractalImage, JobCount);
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
            ConsolePage.NewLog(new Status($"Overall render duration: {timer.Elapsed.Milliseconds}ms", NotificationType.RenderDuration));
            #endregion
        }

        public void HardZoom(Point clickLocation, double width, double height)
        {
            if (DefaultPage.CanvasVM.Image == null)
            {
                return;
            }

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
            if (DefaultPage.CanvasVM.Image == null)
            {
                return;
            }

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
            if (DefaultPage.CanvasVM.Image == null)
            {
                return;
            }

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

        public void ToggleConsoleWindowShow()
        {
            ConsoleOpen = !ConsoleOpen;

            if (ConsoleOpen == true)
            {
                _windowManager.ShowWindowAsync(ConsoleWindow);
            }
            else
            {
                ConsoleWindow.TryCloseAsync();
            }
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

        public void ShowMainWindow()
        {
            _windowManager.ShowWindowAsync(_mainWindow);
        }

        public void OpenFrac()
        {

            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Fractal Definition File (*.frac)|*.frac";

            string text;
            if (openFile.ShowDialog() == true)
            {
                text = File.ReadAllText(openFile.FileName);
            } else
            {
                return;
            }

            MainWindow.WindowTitle = openFile.SafeFileName;

            text = text.Trim();
            List<string> lines = text.Split("\n").ToList();


            // Remove whitespace
            for (int l = 0; l < lines.Count; l++)
            {
                lines[l] = Regex.Replace(lines[l], @"\s+", "");
            }

            lines.ForEach(o => Console.WriteLine(o));

            #region Fractal Frame Parse
            int ffDefLines = 6;
            int ffIndex = lines.IndexOf("##FRACTALFRAME");
            List<string> ffDef = new List<string>();
            for (int index = ffIndex + 1; index < ffIndex + ffDefLines + 1; index++)
            {
                ffDef.Add(lines[index]);
            }

            Dictionary<string, string> ffDic = new Dictionary<string, string>();
            foreach (string defLine in ffDef)
            {
                string propName = defLine.Split(":")[0];
                string val = defLine.Split(":")[1];
                ffDic.Add(propName, val);
            }

            float left = float.Parse(ffDic["Left"]);
            float right = float.Parse(ffDic["Right"]);
            float top = float.Parse(ffDic["Top"]);
            float bottom = float.Parse(ffDic["Bottom"]);
            uint iterations = uint.Parse(ffDic["Iterations"]);
            int bail = int.Parse(ffDic["Bail"]);

            FractalFrame fractalFrame = new FractalFrame(left, right, top, bottom, "Untitled", iterations, bail);
            #endregion

            #region Iterator Parse
            int iterDefLines = 1;
            int iterIndex = lines.IndexOf("##ITERATOR");
            List<string> iterDef = new List<string>();
            for (int index = iterIndex + 1; index < iterIndex + iterDefLines + 1; index++)
            {
                iterDef.Add(lines[index]);
            }

            Dictionary<string, string> iterDic = new Dictionary<string, string>();
            foreach (string defLine in iterDef)
            {
                string propName = defLine.Split(":")[0];
                string val = defLine.Split(":")[1];
                iterDic.Add(propName, val);
            }

            string formulaString = iterDic["FormulaString"];

            BasicIterator iterator = new BasicIterator(formulaString);
            #endregion

            #region Painter
            IPainter painter;

            if (lines.Contains("##BASICPAINTER"))
            {
                // This fractal is using a basic painter
                int bpDefLines = 3;
                int bpIndex = lines.IndexOf("##BASICPAINTER");
                List<string> bpDef = new List<string>();
                for (int index = bpIndex + 1; index < bpIndex + bpDefLines + 1; index++)
                {
                    bpDef.Add(lines[index]);
                }

                Dictionary<string, string> bpDic = new Dictionary<string, string>();
                foreach (string defLine in bpDef)
                {
                    string propName = defLine.Split(":")[0];
                    string val = defLine.Split(":")[1];
                    bpDic.Add(propName, val);
                }

                string inSetColourString = bpDic["InSetColour"];
                Color inSetColour = (Color)ColorConverter.ConvertFromString(inSetColourString);
                string mainColourString = bpDic["MainColour"];
                Color mainColour = (Color)ColorConverter.ConvertFromString(mainColourString);

                bool type = (bpDic["Type"] == "1");

                painter = type == true ? new BasicPainterLight("Untitled", mainColour, inSetColour) : new BasicPainterDark("Untitled", mainColour, inSetColour);
            } else
            {
                throw new Exception("Cannot find painter definition");
            }

            #endregion

            DefaultPage.FractalFrameStackVM.AddFractalFrame(fractalFrame);
            DefaultPage.IteratorStackVM.AddIterator(iterator);
            DefaultPage.PainterStackVM.AddPainter(painter);

        }

        public void SaveFrac()
        {
            if (!NoObjectNull)  // IF one of the key objects is missing, complain.
            {
                MessageBox.Show($"Please select: {(ActiveFractalFrame == null ? "Fractal Frame, " : "")} " +
                    $"{(SelectedIterator == null ? "Iterator, " : "")}" +
                    $"{(SelectedPainter == null ? "Painter, " : "")}", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string fileString = "";

            #region Fractal Frame Stuff
            fileString += "## FRACTALFRAME\n";
            fileString += $"Left: {ActiveFractalFrame!.Left}\n";
            fileString += $"Right: {ActiveFractalFrame!.Right}\n";
            fileString += $"Bottom: {ActiveFractalFrame!.Bottom}\n";
            fileString += $"Top: {ActiveFractalFrame!.Top}\n";
            fileString += $"Iterations: {ActiveFractalFrame!.Iterations}\n";
            fileString += $"Bail: {ActiveFractalFrame!.Bail}\n";
            #endregion

            #region Iterator
            fileString += "## ITERATOR\n";
            fileString += $"FormulaString: {SelectedIterator!.FormulaString}\n";
            #endregion

            #region Painter
            if (SelectedPainter is BasicPainterBase)
            {
                fileString += "## BASICPAINTER\n";
                fileString += $"InSetColour: {((BasicPainterBase)SelectedPainter).InSetColour}\n";
                fileString += $"MainColour: {((BasicPainterBase)SelectedPainter).MainColour}\n";
                fileString += $"Type: {(SelectedPainter.GetType() == typeof(BasicPainterLight) ? 1 : 0)}\n";
            }
            #endregion

            SaveFileDialog saveFile = new SaveFileDialog();

            saveFile.Filter = "Fractal Definition File (*.frac)|*.frac";

            if (saveFile.ShowDialog() == true)
            {
                File.WriteAllText(saveFile.FileName, fileString);
            }
        }
    }
}
