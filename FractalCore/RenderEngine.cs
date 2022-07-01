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
        private Queue<Job> _jobQueue;
        private MultiCL _cLEngine;

        #endregion


        #region Properties
        public MultiCL CLEngine
        {
            get { return _cLEngine; }
            set { _cLEngine = value; }
        }

        public Queue<Job> JobQueue
        {
            get { return _jobQueue; }
            set { _jobQueue = value; }
        }

        
        #endregion

        #region Constructor
        public RenderEngine()
        {
            CLEngine = new MultiCL();
            
            
            JobQueue = new Queue<Job>();
            
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
            await CLIterationsCompute(job.ComputeIterationsJob, progress);
            // ---------------------------------------------------------------------------------------------------


            // Colour 
            // job.FractalImage.Render(job.Painter);  // TODO: the paint is becoming computationally important at large images


            CLEngine.SetKernel(job.Painter.PaintCLScript, "Paint");
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
            job.Painter.SetKernelParameters(ref _cLEngine, ref pixels, ref flatIterations, job.FractalFrame.Iterations);
            


            #region Timer start
            job.SetStatus($"CL-{job.JobNum}: Starting bitmap render", NotificationType.Initialization);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            #endregion

            await Task.Run(() => CLEngine.Invoke(0, pixels.Length, 100));  // Colour pixels 

            PainterBase.WriteArrToBM(ref pixels, fractal.Width, fractal.Height, fractalImage.FractalBitmap);  // Write pixels to writable bitmap

            #region Timer end
            timer.Stop();

            TimeSpan ts = timer.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

            job.SetStatus($"CL-{job.JobNum}: Finished bitmap render in {elapsedTime}", NotificationType.OperationComplete);
            #endregion
        }

        public async Task CLIterationsCompute(ComputeIterationsJob job, IProgress<RenderProgressModel> progress)
        {
            RenderProgressModel report = new RenderProgressModel();
            void Report(object sender, double e)
            {
                report.PercentageComplete = (int)Math.Round(e * 100);
                progress.Report(report);

            }

            // Fill the array at Fractal.IterationsArr with numbers and set it to rendered

            // ------- This will be where a new c script will have to be generated --------
            CLEngine.SetKernel(job.Fractal.Iterator.FullIterationScript, "Mandelbrot");
            // ----------------------------------------------------------------------------

            Fractal fractal = job.Fractal;

            int height = fractal.Height;
            int width = fractal.Width;

            // Create new array as OpenCL kernel only accepts 1D array
            uint[] flatArray = new uint[width * height];


            CLEngine.SetParameter(flatArray, width, height, fractal.Left, fractal.Top, fractal.RealStep, fractal.ImagStep, fractal.Iterations, fractal.Bail);
            CLEngine.ProgressChangedEvent += Report;


            #region Timer start
            job.SetStatus($"CL-{job.JobNum}: Beginning iterations compute", NotificationType.Initialization);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            #endregion

            // The parts simply splits it into sections to be completed between which a progress report is sent
            await Task.Run(() => CLEngine.Invoke(0, flatArray.Length, 100));
            
            

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
        #endregion
    }
}
