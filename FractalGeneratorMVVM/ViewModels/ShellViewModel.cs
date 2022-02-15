using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FractalGeneratorMVVM.Views;


namespace FractalGeneratorMVVM.ViewModels
{
    /// <summary>
    /// View model for the custom flat window
    /// </summary>
    public class ShellViewModel : PropertyChangedBase
    {
        private readonly IWindowManager _windowManager;

        private MainWindowViewModel _mainWindow;

        public ShellViewModel()
        {
            /*
            _windowManager = new WindowManager();

            MainWindowView mainWindowView = new MainWindowView();
            MainWindowViewModel mainWindowViewModel = new MainWindowViewModel(ref mainWindowView);

            ViewModelBinder.Bind(mainWindowViewModel, mainWindowView, null);
            

            _windowManager.ShowWindowAsync(mainWindowViewModel);
            */

            _windowManager = new WindowManager();
            _mainWindow = new MainWindowViewModel();

            _windowManager.ShowWindowAsync(_mainWindow);
        }



    }
}
