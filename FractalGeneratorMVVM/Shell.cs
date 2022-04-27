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

namespace FractalGeneratorMVVM
{
    /// <summary>
    /// View model for the custom flat window
    /// </summary>
    public class Shell : PropertyChangedBase
    {
        private readonly IWindowManager _windowManager;

        private DefaultWindowViewModel _mainWindow;

        public Shell()
        {
            _windowManager = new WindowManager();


            _mainWindow = new DefaultWindowViewModel(new DefaultPageViewModel(), ResizeMode.CanResizeWithGrip);
            _windowManager.ShowWindowAsync(_mainWindow);


        }



    }
}
