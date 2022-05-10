using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FractalGeneratorMVVM.Views.Windows;
using System.Windows.Input;

namespace FractalGeneratorMVVM.ViewModels.Windows
{
    public class NoMaxWindowViewModel : DefaultWindowViewModel
    {
        

        #region Constructor
        public NoMaxWindowViewModel(Screen page, string windowTitle, ResizeMode resize) : base(page, windowTitle, resize)
        {

            LoadPage();
        }
        #endregion

        
    }
}
