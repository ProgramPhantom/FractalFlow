using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FractalGeneratorMVVM.Views;
using FractalGeneratorMVVM.ViewModels;


namespace FractalGeneratorMVVM
{
    /// <summary>
    /// View model for the custom flat window
    /// </summary>
    public class Shell : PropertyChangedBase
    {
        private readonly IWindowManager _windowManager;

        private MainWindowViewModel _mainWindow;

        public Shell()
        {
            _windowManager = new WindowManager();


            _mainWindow = new MainWindowViewModel();
            _windowManager.ShowWindowAsync(_mainWindow);


        }



    }
}
