using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using FractalCore;
using FractalGeneratorMVVM.ViewModels.Controls;
using System.Text.RegularExpressions;


namespace FractalGeneratorMVVM.ViewModels.Windows
{
    public class AddPainterWindowViewModel : Screen
    {
        #region Fields
        private PainterStackViewModel _painterStack;

        private WindowManager _windowManager;

        private NoMaxWindowViewModel _window;
        #endregion

        #region Properties
        public WindowManager WindowManager
        {
            get { return _windowManager; }
            set { _windowManager = value; }
        }

        public NoMaxWindowViewModel Window
        {
            get { return _window; }
            set { _window = value; }
        }
        

        public byte Red { get; set; } = 0;

        public byte Green { get; set; } = 0;

        public byte Blue { get; set; } = 0;

        public string PainterName { get; set; } = "Untitled";
        #endregion


        public AddPainterWindowViewModel(PainterStackViewModel painterStack)
        {
            _painterStack = painterStack;

            _window = new NoMaxWindowViewModel(this, "Add Painter", ResizeMode.NoResize);
            _windowManager = new WindowManager();

        }

        public void ShowWindow()
        {
            _windowManager.ShowWindowAsync(_window);
        }


        public void AddBasicPainter()
        {
            System.Diagnostics.Trace.WriteLine($"{PainterName}, {Red}, {Green}, {Blue}");
            _painterStack.NewBasicPainter(new BasicPainter(PainterName, Red, Green, Blue));
            _window.TryCloseAsync();
        }
    }
    


}
