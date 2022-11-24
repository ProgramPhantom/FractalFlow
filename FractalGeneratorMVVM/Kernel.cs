using Caliburn.Micro;
using FractalCore;
using FractalCore.Painting;
using FractalGeneratorMVVM.ViewModels.Models;
using FractalGeneratorMVVM.ViewModels.Models.Painters;
using FractalGeneratorMVVM.ViewModels.Pages;
using FractalGeneratorMVVM.ViewModels.Windows;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FractalGeneratorMVVM
{
    /// <summary>
    /// This is the brain of <see cref="FractalGeneratorMVVM"/>. It contains the lower most <see cref="WindowManager"/> and 
    /// an instance of <see cref="FractalCore"/>'s <see cref="RenderEngine"/>.
    /// </summary>
    public class Kernel : PropertyChangedBase
    {
        /// <summary>
        /// The token used to cancel renders
        /// </summary>
        CancellationTokenSource cts = new CancellationTokenSource();
        /// <summary>
        /// Holds the current progress of the Kernel in a render.
        /// </summary>
        Progress<RenderProgressModel> progress = new Progress<RenderProgressModel>();

        #region Feilds
        private readonly IWindowManager _windowManager;
        private DefaultWindowViewModel _mainWindow;
        private DefaultWindowViewModel _consoleWindow;
        private DefaultPageViewModel _defaultPage;
        private ConsolePageViewModel _consolePage;
        private RenderEngine _renderEngine;
        private int _renders;
        public Fractal? ActiveFractal;
        public FractalImage? ActiveFractalImage;
        private float _zoomDivisor = 2;
        private FullRenderJob? _fullJob;
        private PaintJob? _paintJob;
        #endregion

        #region Properties
        /// <summary>
        /// The main window for Fractal Flow
        /// </summary>
        public DefaultWindowViewModel MainWindow
        {
            get { return _mainWindow; }
            set { _mainWindow = value; }
        }
        /// <summary>
        /// The console window for Fractal Flow
        /// </summary>
        public DefaultWindowViewModel ConsoleWindow
        {
            get { return _consoleWindow; }
            set { _consoleWindow = value; }
        }
        /// <summary>
        /// The page which is given to <see cref="MainWindow"/> to display
        /// </summary>
        public DefaultPageViewModel DefaultPage
        {
            get { return _defaultPage; }
            set { _defaultPage = value; }
        }
        /// <summary>
        /// The page given to <see cref="ConsoleWindow"/> to display
        /// </summary>
        public ConsolePageViewModel ConsolePage
        {
            get { return _consolePage; }
            set { _consolePage = value; }
        }
        /// <summary>
        /// A render engine from <see cref="FractalCore"/> used for rendering fractals
        /// </summary>
        public RenderEngine RenderEngine
        {
            get { return _renderEngine; }
            set { _renderEngine = value; }
        }

        #region Exposers
        /// <summary>
        /// The <see cref="FractalFrame"/> selected in the FractalFrameStack in <see cref="MainWindow"/>
        /// </summary>
        public FractalFrame? SelectedFractalFrame
        {
            get
            {
                return DefaultPage.SelectedFractalFrame;
            }
        }
        /// <summary>
        /// An invisible <see cref="FractalFrame"/> used when zooming in
        /// </summary>
        public FractalFrame? FakeFractalFrame = null;
        /// <summary>
        /// The <see cref="FractalFrame"/> used for the current fractal rendered.
        /// This is <see cref="FakeFractalFrame"/> if it exists, else <see cref="SelectedFractalFrame"/>.
        /// </summary>
        public FractalFrame? ActiveFractalFrame
        {
            get
            {
                return SelectedFractalFrame == null ? null : FakeFractalFrame ?? SelectedFractalFrame;  // Beautiful
                // Ternary conditional operator and null-coalescing operator in one line!!
            }
        }
        /// <summary>
        /// The selected <see cref="IPainter"/> on the Painter Stack in <see cref="DefaultPage"/>
        /// </summary>
        public IPainter? SelectedPainter
        {
            get
            {
                return DefaultPage.SelectedPainter;
            }
        }
        /// <summary>
        /// The selected <see cref="BasicIterator"/> in <see cref="DefaultPage"/>'s Iteartor Stack.
        /// </summary>
        public BasicIterator? SelectedIterator
        {
            get
            {
                return DefaultPage.SelectedIterator;
            }
        }
        /// <summary>
        /// The value in the height field on <see cref="DefaultPage"/>.
        /// </summary>
        public ushort RenderHeight
        {
            get
            {
                return DefaultPage.ToolRibbonVM.Height;
            }
        }
        /// <summary>
        /// The value in the width field on <see cref="DefaultPage"/>.
        /// </summary>
        public ushort RenderWidth
        {
            get
            {
                return DefaultPage.ToolRibbonVM.Width;
            }
        }
        #endregion

        /// <summary>
        /// Property for the <see cref="FullRenderJob"/> the Kernel is executing
        /// </summary>
        public FullRenderJob? FullJob
        {
            get
            {
                return _fullJob;
            }
            set
            {
                _fullJob = value;
                _fullJob.StatusUpdateEvent += ConsolePage.NewLog;
            }
        }
        /// <summary>
        /// Property for the <see cref="PaintJob"/> the Kernel is executing.
        /// </summary>
        public PaintJob? PaintJob
        {
            get
            {
                return _paintJob;
            }
            set
            {
                _paintJob = value;
                _paintJob.StatusUpdateEvent += ConsolePage.NewLog;
            }
        }
        /// <summary>
        /// A useful property for checking if <see cref="DefaultPage"/> has all three objects selected.
        /// Returns true if no object on the <see cref="DefaultPage"/> is null.
        /// </summary>
        public bool NoObjectNull
        {
            get
            {
                return !(ActiveFractalFrame == null || SelectedPainter == null || SelectedIterator == null);
            }
        }
        /// <summary>
        /// A useful property for checking if only the <see cref="SelectedPainter"/> has changed since the last render.
        /// This is used to check if only a <see cref="PaintJob"/> is needed for a render.
        /// </summary>
        public bool IsJustPainterChanged
        {
            get
            {
                if (ActiveFractal == null || ActiveFractalImage == null) { return false; }

                bool fractalTheSame = ActiveFractal.Iterator == SelectedIterator && ActiveFractal.FractalFrame == ActiveFractalFrame &&
                    ActiveFractal.Width == RenderWidth && ActiveFractal.Height == RenderHeight;

                return fractalTheSame && (SelectedPainter != ActiveFractalImage.CurrentPaint);
            }
        }
        /// <summary>
        /// A count of the number of renders the Kernel has executed in the current session.
        /// </summary>
        public int JobCount
        {
            get
            {
                _renders++;
                return _renders;
            }
            set { _renders = value; }
        }
        /// <summary>
        /// Used when zooming. The dimentions of the <see cref="FractalFrame"/> will be scaled by 1 over this value.
        /// </summary>
        public float ZoomFactor
        {
            get { return _zoomDivisor; }
            set { _zoomDivisor = value; }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor. Used when there is no initial data.
        /// </summary>
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

        /// <summary>
        /// Constructor used when there is an initial state the session needs to spawn with.
        /// </summary>
        /// <param name="fractalFrameViewModels">BindableCollection of FractalFrameViewModels</param>
        /// <param name="painterViewModels">BindableCollection of IPainterViewModels</param>
        /// <param name="iteratorViewModels">BindableCollection of BasicIteratorViewModels</param>
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

        /// <summary>
        /// This method hooks up events from the <see cref="DefaultPage"/> to methods in the Kernel.
        /// </summary>
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
            _defaultPage.ToolRibbonVM.SaveFractalImageEvent += SaveImage;

            MainWindow.CTRL_S += SaveFrac;
            #endregion
        }
        #endregion

        #region Methods
        /// <summary>
        /// Fired when the <see cref="DefaultPage"/>'s Canvas detects the mouse has moved over the
        /// displayed fractal. It updates the text on <see cref="DefaultPage"/>'s StatusBar to show where 
        /// on the complex plane the mouse was detected.
        /// </summary>
        /// <param name="hoverLocation">A point relative to the top left hand corner of the image displayed on the screen</param>
        /// <param name="canvasWidth">Width of the canvas when the mouse was detected</param>
        /// <param name="canvasHeight">Height of the canvas when the mosue was detected</param>
        public void CanvasHovered(Point hoverLocation, double canvasWidth, double canvasHeight)
        {
            Complex mousePos;

            // Ask the FractalFrame where this mouse hover is on the complex plane.
            mousePos = ActiveFractal.FractalFrame.PxToComplex(hoverLocation, canvasWidth, canvasHeight);

            // Tell the StatusBar the info
            _defaultPage.StatusBarVM.UpdateHoverMessage(mousePos);
        }
        /// <summary>
        /// This method is always fired before the main <see cref="RenderAsync"/> and <see cref="CLRenderAsync"/>.
        /// It's job is to set up the render and make sure everything is ready to go, including deciding which 
        /// of the render methods to execute.
        /// </summary>
        /// <param name="clearZoom">Flag if the <see cref="FakeFractalFrame"/> needs to be cleared or not.</param>
        public void PreRender(bool clearZoom = false)
        {

            // If one of the key objects is missing, complain.
            if (!NoObjectNull)  
            {
                MessageBox.Show($"Please select: {(ActiveFractalFrame == null ? "Fractal Frame, " : "")} " +
                    $"{(SelectedIterator == null ? "Iterator, " : "")}" +
                    $"{(SelectedPainter == null ? "Painter, " : "")}", "Render Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return; 
            }

            // Remove the fake fractal (Make sure this happens before the Fractal is created)
            if (clearZoom == true) { FakeFractalFrame = null; }

            // Set up apparatus for cancelling an reporting progress of a render.
            cts = new CancellationTokenSource();  
            progress = new Progress<RenderProgressModel>(); 
            progress.ProgressChanged += ReportProgress;  


            // Decide which render to do based on the CheckBox on DefaultPage
            if (DefaultPage.ToolRibbonVM.GPURender == true)
            {
                CLRenderAsync();
            }
            else
            {
                RenderAsync();
            }
        }
        /// <summary>
        /// Slow CPU bound fractal render. Sets up <see cref="Fratal"/> and <see cref="FractalImage"/> objects
        /// before packaging them up in <see cref="FullJob"/> and <see cref="PaintJob"/> objects and sending them
        /// to the <see cref="RenderEngine"/>.
        /// </summary>
        internal async void RenderAsync()
        {
            #region Timer start
            Stopwatch timer = new Stopwatch();
            timer.Start();
            #endregion

            // Check if just a paint job is needed
            if (IsJustPainterChanged) 
            {
                PaintJob paintJob = new PaintJob(ActiveFractal!, SelectedPainter!, ActiveFractalImage!, JobCount);

                try
                {
                    RenderEngine.Paint(paintJob, progress, cts.Token);
                }
                catch (OperationCanceledException)
                {

                }
            }
            else
            {
                // If a fresh render, create new Fractal and FractalImage objects.
                ActiveFractal = new Fractal(RenderWidth, RenderHeight, ActiveFractalFrame!, SelectedIterator!);
                ActiveFractalImage = new FractalImage(ActiveFractal.Width, ActiveFractal.Height);

                // Create the job objects and pass into them the ActiveFractal and ActiveFractalImage we just made
                PaintJob = new PaintJob(ActiveFractal, SelectedPainter!, ActiveFractalImage, JobCount);
                FullJob = new FullRenderJob(PaintJob, JobCount);

                try
                {
                    await RenderEngine.FullRenderAsync(FullJob, progress, cts.Token);
                }
                catch (OperationCanceledException)
                {

                }
            }

            // Set the image of the new canvas to the newley rendered fractal painted with the selected painter.
            DefaultPage.CanvasVM.Image = ActiveFractalImage;

            #region Timer end
            timer.Stop();
            ConsolePage.NewLog(new Status($"Overall render duration: {timer.Elapsed.Milliseconds}ms", NotificationType.RenderDuration));
            #endregion
        }
        /// <summary>
        /// Very fast OpenCL optimised rendered. Uses the GPU and CPU to deliver blazing fast speeds.
        /// </summary>
        internal async void CLRenderAsync()
        {
            #region Timer start
            Stopwatch timer = new Stopwatch();
            timer.Start();
            #endregion

            // Check if just a paint is needed
            if (IsJustPainterChanged)
            {
                PaintJob = new PaintJob(ActiveFractal!, SelectedPainter!, ActiveFractalImage!, JobCount);

                try
                {
                    await RenderEngine.CLPaintAsync(PaintJob, progress);
                }
                catch (OperationCanceledException)
                {

                }
            }
            else
            {


                ActiveFractal = new Fractal(RenderWidth, RenderHeight, ActiveFractalFrame!, SelectedIterator!);
                ActiveFractalImage = new FractalImage(ActiveFractal.Width, ActiveFractal.Height);

                PaintJob = new PaintJob(ActiveFractal, SelectedPainter!, ActiveFractalImage, JobCount);
                FullJob = new FullRenderJob(PaintJob, JobCount);

                try
                {
                    await RenderEngine.CLFullRenderAsync(FullJob, progress);
                }
                catch (OperationCanceledException)
                {

                }
            }


            DefaultPage.CanvasVM.Image = ActiveFractalImage;

            #region Timer end
            timer.Stop();
            ConsolePage.NewLog(new Status($"Overall render duration: {timer.Elapsed.Milliseconds}ms", NotificationType.RenderDuration));
            #endregion
        }
        /// <summary>
        /// Zoom into the ActiveFractalImage by clicking on the <see cref="DefaultPage"/>'s Canvas.
        /// Zooms by dividing <see cref="ActiveFractalFrame"/>'s dimensions by <see cref="ZoomFactor"/>.
        /// </summary>
        /// <param name="clickLocation">Point on the <see cref="ActiveFractalFrame"/>'s view model relative to the top left of the image.</param>
        /// <param name="width">The width of the canvas when it was clicked</param>
        /// <param name="height">The height of the canvas when it was clicekd</param>
        public void HardZoom(Point clickLocation, double width, double height)
        {
            // Checking if the image actually exists just to make sure no errors occur.
            if (DefaultPage.CanvasVM.Image == null) { return; }

            // Complex number containing the centre of the new FractalFrame which is being made
            Complex centre = ActiveFractalFrame!.PxToComplex(clickLocation, width, height);
            ConsolePage.NewLog(new Status($"Zooming into point {centre.Real} + {centre.Imaginary}i", NotificationType.Zoom));

            double newWidth = ActiveFractalFrame.RealWidth / ZoomFactor;
            double newHeight = ActiveFractalFrame.ImaginaryHeight / ZoomFactor;

            // Make a new FractalFrame based on the old FractalFrame's info but with a redused width and height and a different centre.
            FractalFrame newFrame = FractalFrame.FractalFrameCentre((float)newWidth, (float)newHeight, (float)centre.Real, (float)centre.Imaginary,
                "Temporary Frame", SelectedFractalFrame!.Iterations, SelectedFractalFrame.Bail);

            FakeFractalFrame = newFrame;

            // Render straight away.
            PreRender(false);
        }
        /// <summary>
        /// The same as <see cref="HardZoom(Point, double, double)"/> but for the zoom in and out buttons
        /// on <see cref="DefaultPage"/>'s ToolRibbon
        /// </summary>
        /// <param name="zoomOut">Specify the direction</param>
        public void HardZoom(bool zoomOut = false)
        {
            if (DefaultPage.CanvasVM.Image == null)
            {
                return;
            }

            int width = DefaultPage.CanvasVM.Image.Width;
            int height = DefaultPage.CanvasVM.Image.Height;
            Point clickLocation = new Point(width / 2, height / 2);


            Complex centre = ActiveFractalFrame!.PxToComplex(clickLocation, width, height);

            ConsolePage.NewLog(new Status($"Zooming into point {centre.Real} + {centre.Imaginary}i", NotificationType.Zoom));

            double newWidth = zoomOut ? ActiveFractalFrame.RealWidth * ZoomFactor : ActiveFractalFrame.RealWidth / ZoomFactor;
            double newHeight = zoomOut ? ActiveFractalFrame.ImaginaryHeight * ZoomFactor : ActiveFractalFrame.ImaginaryHeight / ZoomFactor;

            FractalFrame newFrame = FractalFrame.FractalFrameCentre((float)newWidth, (float)newHeight, (float)centre.Real, (float)centre.Imaginary,
                "Temporary Frame", SelectedFractalFrame!.Iterations, SelectedFractalFrame.Bail);

            FakeFractalFrame = newFrame;

            PreRender(false);
        }
        /// <summary>
        /// Resets the zoom by nulling <see cref="FakeFractalFrame"/> and rendering.
        /// </summary>
        public void ResetZoom()
        {
            if (DefaultPage.CanvasVM.Image == null)
            {
                return;
            }

            PreRender(true);
        }
        /// <summary>
        /// Parameterless method so <see cref="DefaultPage"/>'s Zoom In and Zoom Out buttons can 
        /// bind their events easily
        /// </summary>
        public void DumbZoomIn()
        {
            HardZoom();
        }
        /// <summary>
        /// Parameterless method so <see cref="DefaultPage"/>'s Zoom In and Zoom Out buttons can 
        /// bind their events easily
        /// </summary>
        public void DumbZoomOut()
        {
            HardZoom(true);
        }
        /// <summary>
        /// Toggles the console window.
        /// </summary>
        public void ToggleConsoleWindowShow()
        {
            if (ConsoleWindow.IsActive == true)
            {
                ConsoleWindow.TryCloseAsync();
            } else
            {
                _windowManager.ShowWindowAsync(ConsoleWindow);  
            }
        }
        /// <summary>
        /// Cancels the current render
        /// </summary>
        public void CancelRender()
        {
            cts.Cancel();

            DefaultPage.StatusBarVM.ProgressBar = 0;

        }
        /// <summary>
        /// Site for updates of the render progress
        /// </summary>
        /// <param name="sender">Object which called the update</param>
        /// <param name="e">Model containing information on the progress of the render</param>
        private void ReportProgress(object? sender, RenderProgressModel e)
        {
            DefaultPage.StatusBarVM.ProgressBar = e.PercentageComplete;

        }
        /// <summary>
        /// Shows the <see cref="MainWindow"/>
        /// </summary>
        public void ShowMainWindow()
        {
            _windowManager.ShowWindowAsync(_mainWindow);
        }
        /// <summary>
        /// Open a .frac file from the filesystem
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void OpenFrac()
        {

            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Fractal Definition File (*.frac)|*.frac";

            string text;
            if (openFile.ShowDialog() == true)
            {
                text = File.ReadAllText(openFile.FileName);
            }
            else
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
            }
            else
            {
                throw new Exception("Cannot find painter definition");
            }

            #endregion

            DefaultPage.FractalFrameStackVM.AddFractalFrame(fractalFrame);
            DefaultPage.IteratorStackVM.AddIterator(iterator);
            DefaultPage.PainterStackVM.AddPainter(painter);

        }
        /// <summary>
        /// Save a .frac file to the filesystem
        /// </summary>
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
        /// <summary>
        /// Save a .png file to the filesystem. Uses the WriteableBitmap in the <see cref="ActiveFractalImage"/>.
        /// </summary>
        public void SaveImage()
        {
            if (ActiveFractalImage == null) { return; }

            SaveFileDialog saveImage = new SaveFileDialog();
            saveImage.Filter = "PNG (*.png)|*.png";

            if (saveImage.ShowDialog() == true)
            {
                if (saveImage.FileName != String.Empty)
                {
                    using (FileStream stream = new FileStream(saveImage.FileName, FileMode.Create))
                    {
                        PngBitmapEncoder encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(ActiveFractalImage!.FractalBitmap.Clone()));
                        encoder.Save(stream);
                    }
                }
            }
        }
        #endregion
    }
}
