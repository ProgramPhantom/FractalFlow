using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FractalCore.Painting;
using OpenCL;

namespace FractalCore
{
    /// <summary>
    /// This class is dedicated to simplifying the interface in which the front end can request the rendering
    /// of a fractal.
    /// </summary>
    public class RenderEngine
    {
        

        #region Fields
        private MultiCL _paintCLEngine;
        private MultiCL _iterateCLEngine;
        private string? _currentIterScript = null;
        private string? _currentPaintScript = null;
        #endregion


        #region Properties
        public MultiCL PaintCLEngine
        {
            get { return _paintCLEngine; }
            set { _paintCLEngine = value; }
        }
        public MultiCL IterateCLEngine
        {
            get { return _iterateCLEngine; }
            set { _iterateCLEngine = value; }
        }
        public string? CurrentPaintScript
        {
            get { return _currentPaintScript; }
            set { _currentPaintScript = value; }
        }
        public string? CurrentIterScript
        {
            get
            {
                return _currentIterScript;
            }
            set
            {
                _currentIterScript = value;
            }
        }
        #endregion

        #region Constructor
        public RenderEngine()
        {
            _paintCLEngine = new MultiCL();
            _iterateCLEngine = new MultiCL();
            

            
        }
        #endregion

        #region Methods
        
        /// <summary>
        /// Compute a full fractal image, (Iterations Array and Fractal Bitmap)
        /// </summary>
        /// <param name="job"></param>
        /// <param name="progress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task BitmapComputeAsync(RenderBitmapJob job, IProgress<RenderProgressModel> progress, CancellationToken cancellationToken)
        {
            await IterationsComputeAsync(job.ComputeIterationsJob, progress, cancellationToken);

            #region Timer start
            job.SetStatus($"{job.JobNum}: Starting bitmap render", NotificationType.Initialization);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            #endregion

            job.FractalImage.Render(job.Painter);
            
            #region Timer end
            timer.Stop();

            
            TimeSpan ts = timer.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

            job.SetStatus($"{job.JobNum}: Finished bitmap render in {elapsedTime}", NotificationType.OperationComplete);
            #endregion
        }
        public async Task IterationsComputeAsync(ComputeIterationsJob job, IProgress<RenderProgressModel> progress, CancellationToken cancellationToken)
        {

            #region Variables
            Fractal fractal = job.Fractal;
            double realStep = job.Fractal.RealStep;
            double imagStep = job.Fractal.ImagStep;
            int height = job.Fractal.Height;
            int width = job.Fractal.Width;

            RenderProgressModel report = new RenderProgressModel();

            Complex point;

            double realCoord;
            double imaginaryCoord;
            uint iterations;
            #endregion

            #region Timer start
            job.SetStatus($"{job.JobNum}: Beginning iterations compute", NotificationType.Initialization);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            #endregion

            // Iterate through every pixel on the complex plane
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    cancellationToken.ThrowIfCancellationRequested();  // CANCEL HERE IF TOKEN ACTIVATED

                    realCoord = (realStep * x) + fractal.Left;
                    imaginaryCoord = fractal.Top - (imagStep * y) ;

                    point = new Complex(realCoord, imaginaryCoord);

                    iterations = await Task.Run(() => fractal.Iterator.Iterate(point, fractal.Iterations, fractal.Bail));

                    fractal.IterationsArray[y, x] = iterations;

                    
                }

                // Add one as y starts at 0
                report.PercentageComplete = ((y + 1) * 100) / height;
                progress.Report(report);

            }

            #region Timer end
            timer.Stop();

            TimeSpan ts = timer.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

            job.SetStatus($"{job.JobNum}: Finished iterations compute in {elapsedTime}", NotificationType.OperationComplete);
            #endregion
        }



        public async Task CLBitmapCompute(RenderBitmapJob job, IProgress<RenderProgressModel> progress, CancellationToken cancellationToken)
        {
            // First, compute the iterations array in Fractal

            // ------------ This is where a check is needed to see if this fractal already exists ----------------
            await CLIterationsCompute(job.ComputeIterationsJob, progress, cancellationToken);
            // ---------------------------------------------------------------------------------------------------

            #region Initialize PaintCLEngine
            #region Start timer
            Stopwatch timer = new Stopwatch();
            timer.Start();
            #endregion
            if (!(job.Painter.PaintCLScript == CurrentPaintScript))
            {
                PaintCLEngine.SetKernel(job.Painter.PaintCLScript, "Paint");
                CurrentPaintScript = job.Painter.PaintCLScript;
            }
            #region Timer end
            timer.Stop();
            job.SetStatus($"CL-{job.JobNum}: Compiled Paint CL Script in {timer.Elapsed.Milliseconds}ms", NotificationType.Complile);
            #endregion

            Fractal fractal = job.Fractal;
            FractalImage fractalImage = job.FractalImage;

            // Set up variables
            byte[] pixels = new byte[fractal.Height * fractal.Width * 4];

            // Flatten iterations array
            uint[] flatIterations = new uint[fractal.Height * fractal.Width];
            for (int i = 0; i < fractal.IterationsArray.Length; i++)
            {
                int x = i % fractal.Width;
                int y = i / fractal.Height;

                flatIterations[i] = fractal.IterationsArray[y, x];
            }

            // Set the parameters 
            job.Painter.SetKernelParameters(ref _paintCLEngine, ref pixels, ref flatIterations, job.FractalFrame.Iterations);
            #endregion

            #region Timer start
            timer = new Stopwatch();
            timer.Start();
            #endregion

            // Execute
            await Task.Run(() => PaintCLEngine.Invoke(0, pixels.Length, 100, cancellationToken));  // Colour pixels 

            #region Write to Fractal Image
            PainterBase.WriteArrToBM(ref pixels, fractal.Width, fractal.Height, fractalImage.FractalBitmap);  // Write pixels to writable bitmap
            #endregion

            #region Timer end
            timer.Stop();
            job.SetStatus($"CL-{job.JobNum}: Finished bitmap render in {timer.Elapsed.Milliseconds}ms", NotificationType.OperationComplete);
            #endregion
        }
        public async Task CLIterationsCompute(ComputeIterationsJob job, IProgress<RenderProgressModel> progress, CancellationToken cancellationToken)
        {
            RenderProgressModel report = new RenderProgressModel();
            void Report(object? sender, double e)
            {
                

                report.PercentageComplete = (int)Math.Round(e * 100);
                progress.Report(report);

            }

            #region Initialize CLEngine

            #region Timer start
            Stopwatch timer = new Stopwatch();
            timer.Start();
            #endregion
            // ------- This will be where a new c script will have to be generated --------
            if (!(job.Fractal.Iterator.FullIterationScript == CurrentIterScript))
            {
                IterateCLEngine.SetKernel(job.Fractal.Iterator.FullIterationScript, "Mandelbrot");
                IterateCLEngine.ProgressChangedEvent += Report;
                CurrentIterScript = job.Fractal.Iterator.FullIterationScript;
            }
            // ----------------------------------------------------------------------------
            #region Timer end
            timer.Stop();
            job.SetStatus($"CL-{job.JobNum}: Compiled Iterations CL Script in: {timer.Elapsed.Milliseconds}ms", NotificationType.Complile);
            #endregion

            
            Fractal fractal = job.Fractal;

            int height = fractal.Height;
            int width = fractal.Width;

            // Create new array as OpenCL kernel only accepts 1D array
            uint[] flatArray = new uint[width * height];


            IterateCLEngine.SetParameter(flatArray, width, height, fractal.Left, fractal.Top, fractal.RealStep, fractal.ImagStep, fractal.Iterations, fractal.Bail);
            #endregion

            #region Timer start
            timer = new Stopwatch();
            timer.Start();
            #endregion

            // Execute
            await Task.Run(() => IterateCLEngine.Invoke(0, flatArray.Length, 100, cancellationToken));

            #region Write 1d array to 2d
            int pos = 0;
            // Write the flat array to the 2d array in fractal
            for (int h = 0; h < fractal.Height; h++)
            {
                for (int w = 0; w < fractal.Width; w++)
                {
                    fractal.IterationsArray[h, w] = flatArray[pos];
                    pos++;                
                }
            }
            #endregion

            #region Timer end
            timer.Stop();
            job.SetStatus($"CL-{job.JobNum}: Finished iterations compute in {timer.Elapsed.Milliseconds}ms", NotificationType.OperationComplete);
            #endregion
        }
        #endregion
    }
}
