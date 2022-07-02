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
using System.Windows.Media;
using FractalCore.Painting;
using FractalGeneratorMVVM.ViewModels.Windows;

namespace FractalGeneratorMVVM.ViewModels.WinPages
{
    public class AddPainterWindowViewModel : Screen
    {
        #region Fields
        #region Window Stuff
        private PainterStackViewModel _painterStack;

        private WindowManager _windowManager;

        private NoMaxWindowViewModel _window;

        private int _width = 1000;

        private int _height = 650;

        #endregion

        private int _tabIndex = 0;

        #region Basic Painter
        private Color _basicPainterMainColour = Color.FromRgb(255, 0, 0);
        private Color _basicPainterInSetColour = Color.FromRgb(0, 0, 0);
        #endregion

        #endregion

        #region Properties
        #region Window Stuff
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

        

        public int Width
        {
            get { return _width; }
            set
            {
                _width = value;
                NotifyOfPropertyChange(() => Width);
            }
        }

        public int Height
        {
            get { return _height; }
            set
            {
                _height = value;
                NotifyOfPropertyChange(() => Height);
            }
        }
        #endregion

        public string PainterName { get; set; } = "Untitled Painter";



        public int TabIndex
        {
            get { return _tabIndex; }
            set 
            { 
                _tabIndex = value;
                NotifyOfPropertyChange(() => TabIndex);
            }
        }

        // This is cringe, if I had unlimited time I would make controls for each tab so I did not have to have every
        // property in this single view model
        #region Basic Painter
        public Color BasicPainterMainColour
        {
            get { return _basicPainterMainColour; }
            set { _basicPainterMainColour = value; }
        }

        public Color BasicPainterInSetColour
        {
            get { return _basicPainterInSetColour; }
            set { _basicPainterInSetColour = value; }
        }

        
        #endregion

        #endregion


        public AddPainterWindowViewModel(PainterStackViewModel painterStack)
        {
            _painterStack = painterStack;

            _window = new NoMaxWindowViewModel(this, "Add Painter", ResizeMode.NoResize, Width, Height);
            _windowManager = new WindowManager();

        }

        public void ShowWindow()
        {
            _windowManager.ShowWindowAsync(_window);
        }

        public void CloseWindow()
        {
            _window.TryCloseAsync();
        }

        public void AddPainter()
        {
            switch(TabIndex)
            {
                case 0:
                    AddBasicPainterLight();
                    break;
                case 1:
                    AddBasicPainterDark();
                    break;
            }
        }


        public void AddBasicPainterLight()
        {
            
            _painterStack.NewBasicPainterLight(new BasicPainterLight(PainterName, BasicPainterMainColour, BasicPainterInSetColour));
            _window.TryCloseAsync();
        }

        public void AddBasicPainterDark()
        {

            _painterStack.NewBasicPainterDark(new BasicPainterDark(PainterName, BasicPainterMainColour, BasicPainterInSetColour));
            _window.TryCloseAsync();
        }
    }
    


}
