using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCL;

namespace FractalCore
{
    /// <summary>
    /// This class is dedicated to simplifying the interface in which the front end can request the rendering
    /// of a fractal.
    /// </summary>
    public class RenderEngine
    {
        static string Mandelbrot
        {
            get
            {
                return @"
                        
                        kernel void Mandelbrot(global int *message, int width, int height, float left, float top, double realStep, double imagStep, int iterations, int bail)
                        {
                            int pixNum = get_global_id(0);

                            //printf("" % d"",pixNum);

                            int x = pixNum % width;
                            int y = pixNum / width;

                            double real = left + x * realStep;
                            double imag = top - y * imagStep;


                           //printf("" % d % d"",x,y);
            

                            // printf(""Value of r = % lf\n"",real);
                            // printf(""Value of i = % lf\n"", top);
                            
                            
                            // -------------- Iterate Point -----------------
                            // c
                            double cr = real;
                            double ci = imag;


                            // next z
                            double z1r = 0;
                            double z1i = 0;

                            // this z
                            double znr = 0;
                            double zni = 0;

                            
                            int completedIterations = 0;

                            // printf(""Value of r = % lf Value of i = % lf\n"",real, imag);
                            //printf(""Value of i = % lf\n"", top);

                            
                            
                            for (int iter = 0; iter < iterations; iter++) {
                                
                                // z1r
                                z1r = (znr * znr - zni * zni) + cr;

                                // z1i
                                z1i = (2 * znr * zni) + ci;

                                if (sqrt(z1r*z1r + z1i*z1i) > bail) {
                                    // Not in set
                                    
                                    completedIterations = iter;
                                    break;
                                }

                                completedIterations ++;    
                                
                                // Shift them back
                                znr = z1r;
                                zni = z1i;
                            }

                            

                            // printf("" % d"",completedIterations);

                            message[pixNum] = completedIterations;

                        }";
            }
        }

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
        public void RenderMandelbrot(ref uint[] iterationsArr, int width, int height, float left, float top, double realStep, double imagStep, int iterations, int bail)
        {
            CLEngine.SetParameter(iterationsArr, width, height, left, top, realStep, imagStep, iterations, bail);

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            CLEngine.Invoke(0, iterationsArr.Length, 1);
            stopWatch.Stop();

            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Trace.WriteLine(elapsedTime);
        }
    
    
        public void GenerateIterations(ref Fractal fractalObj, FractalFrame pos)
        {

        }

        public void GenerateWritableBitmap(ref Fractal fractalObj, FractalFrame pos)
        {

        }

        public void CLBitmapCompute(RenderBitmapJob job)
        {
            // First, compute the iterations array in Fractal

            // ------------ This is where a check is needed to see if this fractal already exists ----------------
            CLIterationsCompute(job.ComputeIterationsJob);

            // Colour 
            job.FractalImage.Render(job.Painter);
        }

        public void CLIterationsCompute(ComputeIterationsJob job)
        {
            // Fill the array at Fractal.IterationsArr with numbers and set it to rendered

            // ------- This will be where a new c script will have to be generated --------
            CLEngine.SetKernel(Mandelbrot, "Mandelbrot");
            // ----------------------------------------------------------------------------

            Fractal fractal = job.Fractal;

            int height = fractal.Height;
            int width = fractal.Width;

            // Create new array as OpenCL kernel only accepts 1D array
            uint[] flatArray = new uint[width * height];

            CLEngine.SetParameter(flatArray, width, height, fractal.Left, fractal.Top, fractal.RealStep, fractal.ImagStep, fractal.Iterations, fractal.Bail);
            CLEngine.Invoke(0, flatArray.Length, 1);

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
        }
        #endregion
    }
}
