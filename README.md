# FractalFlow
WIP

Fractal Flow is a next generation application for rendering fractal images over the complex plane, with a focus on generating custom user-defined fractals.

<p align="center">
  <img src="https://github.com/ProgramPhantom/FractalFlow/assets/49105496/bea77f82-704e-496d-a2e5-73ba33bd892d" />
</p>

### Mission: 

Fractal Flow aims to streamline and accelerate the process of exploring new fractal objects - beyond the Mandelbrot set. Fractal Flow combines a blazing fast rendering kernel with a UI whioh empowers rapid iteration and generation of ideas.

### Architecture:
- [Windows Presentation Foundation](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/overview/?view=netdesktop-7.0) (C#)
- [Model View View Model (MVVM)](https://learn.microsoft.com/en-us/dotnet/architecture/maui/mvvm) design principal
- MVVM implemented using [Caliburn Micro](https://caliburnmicro.com/)

### Implementation detail:
- Fractal Flow uses a meta-programming approach to achieve superior performance. The application generates an OpenCL Kernel specifically for the formula specified, which is ran on the GPU.
- The application currently also includes a renderer for the CPU, however it is highly unoptimised and unfeasibly slow, and thus will no longer be maintained.

![Examples](https://github.com/ProgramPhantom/FractalFlow/assets/49105496/3213b476-4842-474b-a459-ba36a016894e)

# User Interface

![Tan](https://github.com/ProgramPhantom/FractalFlow/assets/49105496/fb5841b4-b460-44b3-abfa-d46057e9bf8b)




Started 12th Jan 2022
