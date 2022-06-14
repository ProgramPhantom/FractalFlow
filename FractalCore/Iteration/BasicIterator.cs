﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FormulaParser;

namespace FractalCore
{
    /// <summary>
    /// This class holds the method that is used to iterate a point on the compelx plane to see if it diverges or converges
    /// </summary>
    public class BasicIterator : IIterator
    {
        Dictionary<string, string> constantTranslation = new Dictionary<string, string>()
        {
            ["pi"] = "M_PI",
            ["e"] = "M_E"
        };

        Dictionary<string, string> variableTranslation = new Dictionary<string, string>()
        {
            ["z"] = "z1",
            ["c"] = "c"
        };

        Dictionary<string, string> functionTranslation = new Dictionary<string, string>()
        {
            
            ["log"] = "clog",
            ["log10"] = "clog10",
            ["exp"] = "cexp",

            ["pos"] = "cpos",
            ["arg"] = "carg",
            ["arg"] = "carg",
            ["conj"] = "cconj",
            ["abs"] = "cabs",

            ["sqrt"] = "csqrt",

            ["sin"] = "csin",
            ["cos"] = "ccos",
            ["tan"] = "ctan",

            ["sinh"] = "csinh",
            ["cosh"] = "ccosh",
            ["tanh"] = "ctanh",

            ["asin"] = "casin",
            ["acos"] = "cacos",
            ["atan"] = "catan",

            ["asinh"] = "casinh",
            ["acosh"] = "cacosh",
            ["atanh"] = "catanh",


        };

        Dictionary<string, string> operatorTranslation = new Dictionary<string, string>()
        {
            ["+"] = "cadd",
            ["-"] = "csub",
            ["*"] = "cmul",
            ["/"] = "cdiv",
            ["^"] = "cpow"

        };

        public static string PreCode
        {
            get
            {
                return @"
#ifndef OPENCL_COMPLEX_MATH
#define OPENCL_COMPLEX_MATH

#define CONCAT(x, y) x##y
//                        LETTER C + function name + f if float version
#define FNAME(name, sufix) c##name##sufix

// float2
#define clrealf(complex) complex.x;
#define climagf(complex) complex.y;

// double2
#define clreal(complex) complex.x;
#define climag(complex) complex.y;

#define OPENCL_COMPLEX_MATH_FUNCS(complex_type, real_type, func_sufix, math_consts_sufix) \
    complex_type CONCAT(complex, func_sufix)(real_type r, real_type i) \
    { \
        return (complex_type)(r, i); \
    } \
    \
    complex_type FNAME(add, func_sufix)(complex_type x, complex_type y) \
    { \
        return x + y; \
    } \
    \
    complex_type FNAME(sub, func_sufix)(complex_type x, complex_type y) \
    { \
        return x - y; \
    } \
    \
    complex_type FNAME(add_real, func_sufix)(complex_type z, real_type r) \
    { \
        return (complex_type)(z.x + r, z.y); \
    } \
    \
    complex_type FNAME(sub_real, func_sufix)(complex_type z, real_type r) \
    { \
        return (complex_type)(z.x - r, z.y); \
    } \
    \
    real_type FNAME(abs, func_sufix)(complex_type z) \
    { \
        return sqrt(pow(z.x, 2) + pow(z.y, 2)); \
    } \
    \
    complex_type FNAME(pos, func_sufix)(complex_type z) \
    { \
        real_type r = z.x > 0 ? z.x : z.x * -1; \
        real_type i = z.y > 0  ? z.y : z.y * -1; \
        return (complex_type)(r, i); \
    } \
    \
    real_type FNAME(arg, func_sufix)(complex_type z) \
    { \
        return atan2(z.y, z.x); \
    } \
    \
    complex_type FNAME(mul, func_sufix)(complex_type z1, complex_type z2) \
    { \
        real_type x1 = z1.x; \
        real_type y1 = z1.y; \
        real_type x2 = z2.x; \
        real_type y2 = z2.y; \
        return (complex_type)(x1 * x2 - y1 * y2, x1 * y2 + x2 * y1); \
    } \
    \
    complex_type FNAME(div, func_sufix)(complex_type z1, complex_type z2) \
    { \
        real_type x1 = z1.x; \
        real_type y1 = z1.y; \
        real_type x2 = z2.x; \
        real_type y2 = z2.y; \
        real_type iabs_z2 = CONCAT(1.0, func_sufix) / FNAME(abs, func_sufix)(z2); \
        return (complex_type)( \
            ((x1 * x2 * iabs_z2) + (y1 * y2 * iabs_z2)) * iabs_z2, \
            ((y1 * x2 * iabs_z2) - (x1 * y2 * iabs_z2)) * iabs_z2  \
        ); \
    } \
    \
    complex_type FNAME(mul_real, func_sufix)(complex_type z, real_type r) \
    { \
        return z * r; \
    } \
    \
    complex_type FNAME(div_real, func_sufix)(complex_type z, real_type r) \
    { \
        return z / r; \
    } \
    \
    complex_type FNAME(conj, func_sufix)(complex_type z) \
    { \
        return (complex_type)(z.x, -z.y); \
    } \
    \
    complex_type FNAME(proj, func_sufix)(complex_type z) \
    { \
        if(isinf(z.x) || isinf(z.y)) \
        { \
            return (complex_type)(INFINITY, (copysign(CONCAT(0.0, func_sufix), z.y))); \
        } \
        return z; \
    } \
    \
    real_type FNAME(norm, func_sufix)(complex_type z) \
    { \
        /* Returns the squared magnitude of the complex number z. */ \
        /* The norm calculated by this function is also known as */ \
        /* field norm or absolute square. */ \
        real_type x = z.x; \
        real_type y = z.y; \
        return x * x + y * y; \
    } \
    \
    complex_type FNAME(polar, func_sufix)(real_type r, real_type theta) \
    { \
        /* Returns a complex number with magnitude r and phase angle theta. */ \
        return (complex_type)(r * cos(theta), r * sin(theta)); \
    } \
    \
    complex_type FNAME(exp, func_sufix)(complex_type z) \
    { \
        /* The complex exponential function e^z for z = x+i*y */ \
        /* equals to e^x * cis(y), */ \
        /* or, e^x * (cos(y) + i*sin(y)) */ \
        real_type expx = exp(z.x); \
        return (complex_type)(expx * cos(z.y), expx * sin(z.y)); \
    } \
    \
    complex_type FNAME(log, func_sufix)(complex_type z) \
    { \
        /* log(z) = log(abs(z)) + i * arg(z)  */ \
        return (complex_type)(log(FNAME(abs, func_sufix)(z)),FNAME(arg, func_sufix)(z)); \
    } \
    \
    complex_type FNAME(log10, func_sufix)(complex_type z) \
    { \
        return FNAME(log, func_sufix)(z) / log(CONCAT(10.0, func_sufix)); \
    } \
    \
    complex_type FNAME(pow, func_sufix)(complex_type z1, complex_type z2) \
    { \
        /* (z1)^(z2) = exp(z2 * log(z1)) = cexp(mul(z2, clog(z1))) */ \
        return \
            FNAME(exp, func_sufix)( \
                FNAME(mul, func_sufix)( \
                    z2, \
                    FNAME(log, func_sufix)(z1) \
                ) \
            ); \
    } \
    \
    complex_type FNAME(sqrt, func_sufix)(complex_type z) \
    { \
        /*  */ \
        real_type x = z.x; \
        real_type y = z.y; \
        if(x == CONCAT(0.0, func_sufix)) \
        { \
            real_type t = sqrt(fabs(y) / 2); \
            return (complex_type)(t, y < CONCAT(0.0, func_sufix) ? -t : t); \
        } \
        else \
        { \
            real_type t = sqrt(2 * FNAME(abs, func_sufix)(z) + fabs(x)); \
            real_type u = t / 2; \
            return x > CONCAT(0.0, func_sufix) \
                ? (complex_type)(u, y / t) \
                : (complex_type)(fabs(y) / t, y < CONCAT(0.0, func_sufix) ? -u : u); \
        } \
    } \
    \
    complex_type FNAME(sin, func_sufix)(complex_type z) \
    { \
        const real_type x = z.x; \
        const real_type y = z.y; \
        return (complex_type)(sin(x) * cosh(y), cos(x) * sinh(y)); \
    } \
    \
    complex_type FNAME(sinh, func_sufix)(complex_type z) \
    { \
        const real_type x = z.x; \
        const real_type y = z.y; \
        return (complex_type)(sinh(x) * cos(y), cosh(x) * sin(y)); \
    } \
    \
    complex_type FNAME(cos, func_sufix)(complex_type z) \
    { \
        const real_type x = z.x; \
        const real_type y = z.y; \
        return (complex_type)(cos(x) * cosh(y), -sin(x) * sinh(y)); \
    } \
    \
    complex_type FNAME(cosh, func_sufix)(complex_type z) \
    { \
        const real_type x = z.x; \
        const real_type y = z.y; \
        return (complex_type)(cosh(x) * cos(y), sinh(x) * sin(y)); \
    } \
    \
    complex_type FNAME(tan, func_sufix)(complex_type z) \
    { \
        return FNAME(div, func_sufix)( \
            FNAME(sin, func_sufix)(z), \
            FNAME(cos, func_sufix)(z) \
        ); \
    } \
    \
    complex_type FNAME(tanh, func_sufix)(complex_type z) \
    { \
        return FNAME(div, func_sufix)( \
            FNAME(sinh, func_sufix)(z), \
            FNAME(cosh, func_sufix)(z) \
        ); \
    } \
    \
    complex_type FNAME(asinh, func_sufix)(complex_type z) \
    { \
        complex_type t = (complex_type)( \
            (z.x - z.y) * (z.x + z.y) + CONCAT(1.0, func_sufix), \
            CONCAT(2.0, func_sufix) * z.x * z.y \
        ); \
        t = FNAME(sqrt, func_sufix)(t) + z; \
        return FNAME(log, func_sufix)(t); \
    } \
    \
    complex_type FNAME(asin, func_sufix)(complex_type z) \
    { \
        complex_type t = (complex_type)(-z.y, z.x); \
        t = FNAME(asinh, func_sufix)(t); \
        return (complex_type)(t.y, -t.x); \
    } \
    \
    complex_type FNAME(acosh, func_sufix)(complex_type z) \
    { \
        return \
            CONCAT(2.0, func_sufix) * FNAME(log, func_sufix)( \
                FNAME(sqrt, func_sufix)( \
                    CONCAT(0.5, func_sufix) * (z + CONCAT(1.0, func_sufix)) \
                ) \
                + FNAME(sqrt, func_sufix)( \
                    CONCAT(0.5, func_sufix) * (z - CONCAT(1.0, func_sufix)) \
                ) \
            ); \
    } \
    \
    complex_type FNAME(acos, func_sufix)(complex_type z) \
    { \
        complex_type t = FNAME(asin, func_sufix)(z);\
        return (complex_type)( \
            CONCAT(M_PI_2, math_consts_sufix) - t.x, -t.y \
        ); \
    } \
    \
    complex_type FNAME(atanh, func_sufix)(complex_type z) \
    { \
        const real_type zy2 = z.y * z.y; \
        real_type n = CONCAT(1.0, func_sufix) + z.x; \
        real_type d = CONCAT(1.0, func_sufix) - z.x; \
        n = zy2 + n * n; \
        d = zy2 + d * d; \
        return (complex_type)( \
            CONCAT(0.25, func_sufix) * (log(n) - log(d)), \
            CONCAT(0.5, func_sufix) * atan2( \
                CONCAT(2.0, func_sufix) * z.y, \
                CONCAT(1.0, func_sufix) - zy2 - (z.x * z.x) \
            ) \
        ); \
    } \
    \
    complex_type FNAME(atan, func_sufix)(complex_type z) \
    { \
        const real_type zx2 = z.x * z.x; \
        real_type n = z.y + CONCAT(1.0, func_sufix); \
        real_type d = z.y - CONCAT(1.0, func_sufix); \
        n = zx2 + n * n; \
        d = zx2 + d * d; \
        return (complex_type)( \
            CONCAT(0.5, func_sufix) * atan2( \
                CONCAT(2.0, func_sufix) * z.x, \
                CONCAT(1.0, func_sufix) - zx2 - (z.y * z.y) \
            ), \
            CONCAT(0.25, func_sufix) * (log(n / d)) \
        ); \
    }

// float complex
typedef float2 cfloat;
OPENCL_COMPLEX_MATH_FUNCS(float2, float, f, _F)

// double complex

#pragma OPENCL EXTENSION cl_khr_fp64 : enable
typedef double2 cdouble;
OPENCL_COMPLEX_MATH_FUNCS(double2, double, , )


#undef FNAME
#undef CONCAT
#endif // OPENCL_COMPLEX_MATH

                        
                        kernel void Mandelbrot(global int *message, int width, int height, float left, float top, double realStep, double imagStep, int iterations, int bail)
                        {
                            int pixNum = get_global_id(0);

                            //printf("" % d"",pixNum);

                            int x = pixNum % width;
                            int y = pixNum / width;
            
                            double real = left + x * realStep;
                            double imag = top - y * imagStep;

                            
                            
                           //printf("" % d % d"",x,y);

            
                            //cfloat f;
                            //printf(""Value of r = % lf\n"",real);
                            // printf(""Value of i = % lf\n"", top);
                            
                            
                            // -------------- Iterate Point -----------------
                            // c
                            cdouble c = complex(real, imag);

                            // this z
                            cdouble z1 = c;
                            
                            
                            int completedIterations = 0;

                            // printf(""Value of r = % lf Value of i = % lf\n"",real, imag);
                            //printf(""Value of i = % lf\n"", top);

                            
                            
                            for (int iter = 0; iter < iterations; iter++) {
                                
                                

                                ";
            }
        }

        public static string PostCode
        {
            get
            {
                return @"       if (cabs(z1) >= bail) {
                                    // Not in set
                                    
                                    completedIterations = iter;
                                    break;
                                }

                                completedIterations ++;    
                                
                                

                            }

                            

                            // printf("" % d"",completedIterations);

                            message[pixNum] = completedIterations;

                        }";
            }
        }

        private string _formulaString;
        private string _name;
        private RPN _formulaObject;

        public string FormulaString
        {
            get { return _formulaString; }
            set { _formulaString = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }


        public RPN FormulaObject
        {
            get { return _formulaObject; }
            set { _formulaObject = value; }
        }

        public List<string> IterationsCode { get; set; }

        public string FullIterationScript
        {
            get
            {
                return PreCode + string.Join("\n", IterationsCode) + "\n" + PostCode;
            }
        }


        public BasicIterator(string formulaString, string name="Unitiled")
        {
            _formulaString = Regex.Replace(formulaString, @"\s+", "");
            _name = name;

            _formulaObject = new RPN(_formulaString);

            IterationsCode = new List<string>();
            // IterationsCode = _formulaObject.GenerateOpenCLC("z", "z1", new Dictionary<string, string>() { ["pi"] = "M_PI", ["e"] = "M_E" });
            RPNToCL RPNToCLObj = new RPNToCL(_formulaObject, "z1", constantTranslation, variableTranslation, functionTranslation, operatorTranslation);
            IterationsCode.Add(RPNToCLObj.CCode);
        }

        public uint Iterate(Complex c, uint maxIterations, int bail)
        {
            uint currentIterations = 0;
            Complex z = Complex.Zero;
            Dictionary<string, Complex> variables = new Dictionary<string, Complex>()
            {
                ["z"] = z,
                ["c"] = c
            };

            for (int i = 0; i < maxIterations; i++)
            {
                if (Complex.Abs(z) > bail)
                {
                    break;
                } else
                {
                    currentIterations++; // Increment current iterations!
                }

                variables["z"] = z;

                z = FormulaObject.ComputeComplex(variables);  // Calculate the next z value!!!
            }

            return currentIterations;
        }
    }
}
