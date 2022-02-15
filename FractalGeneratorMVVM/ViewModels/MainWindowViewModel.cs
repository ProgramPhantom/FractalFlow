using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FractalGeneratorMVVM.Views;
using System.Windows.Input;

namespace FractalGeneratorMVVM.ViewModels
{
    public class MainWindowViewModel : Screen
    {
        #region Fields

        /// <summary>
        /// The margin around the window to allow for a drop shadow
        /// </summary>
        private int _outerMarginSize = 10;

        private int _resizeBorderSize = 6;

        private int _titleHeight = 25;

        /// <summary>
        /// The radius of the edges of the window
        /// </summary>
        private int _windowRadius = 10;

        private bool _maximised = false;

        #endregion

        #region Properties

        /// <summary>
        /// The size of the region around the window where the resize handles appear.
        /// ResizeBorder must be set to 0 when maximised otherwise there will be an area
        /// around the outside of the screen where nothing is interactable as the resize 
        /// area is blocking it
        /// </summary>
        public int ResizeBorder
        {
            get
            {
                return _maximised ? 0 : _resizeBorderSize;
            }
            set
            {
                _resizeBorderSize = value;
                NotifyOfPropertyChange(() => ResizeBorder);
            }
        }

        /// <summary>
        /// Size of the resize border around the window taking into account the outer margin
        /// </summary>
        public Thickness ResizeBorderThickness { get { return new Thickness(ResizeBorder + OuterMarginSize); } }

        /// <summary>
        /// The margin around the window to allow for a drop shadow
        /// </summary>
        public int OuterMarginSize
        {
            get
            {
                // If maximised, remove the margin so the window properly goes to the edges of the monitor, if not 
                // Margin is needed so the drop shadow can be displayed correctly

                return _maximised ? 0 : _outerMarginSize;

            }
            set
            {
                _outerMarginSize = value;
                NotifyOfPropertyChange(() => OuterMarginSize);
            }
        }

        public Thickness OuterMarginSizeThickness { get { return new Thickness(OuterMarginSize); } }


        /// <summary>
        /// The radius of the edges of the window
        /// </summary>
        public int WindowRadius
        {
            get
            {
                // If maximised, no radius
                return _maximised ? 0 : _windowRadius;
            }
            set
            {
                _windowRadius = value;
                NotifyOfPropertyChange(() => WindowRadius);
            }
        }

        public CornerRadius WindowCornerRadius { get { return new CornerRadius(WindowRadius); } }

        public int TitleHeight { get { return _titleHeight; }  }

        public GridLength TitleHeightGridLength { get { return new GridLength(TitleHeight + ResizeBorder); } }

        #endregion

        #region Commands

        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            

            
        }
        #endregion

        #region Methods
        /// <summary>
        /// Connected up with the window's StateChanged event, fires when the window state is changed to update the
        /// window radius and margin
        /// </summary>
        /// <param name="view"></param>
        public void UpdateProperties(MainWindowView view)
        {

            if (view.WindowState == WindowState.Maximized)
            {
                _maximised = true;

            } else
            {
                _maximised = false;
            }

            // Update the properties!!!
            NotifyOfPropertyChange(() => OuterMarginSizeThickness);
            NotifyOfPropertyChange(() => WindowCornerRadius);
            NotifyOfPropertyChange(() => ResizeBorderThickness);
            NotifyOfPropertyChange(() => TitleHeightGridLength);
        }

        #region Commands
        /// <summary>
        /// Close the window
        /// </summary>
        public void CloseWindow(Window window)
        {
            window.Close();
        }

        /// <summary>
        /// For the maximize button
        /// </summary>
        public void MaximizeWindow(Window window)
        {
            if (window.WindowState == WindowState.Maximized)
            {
                window.WindowState = WindowState.Normal;
            } else
            {
                window.WindowState = WindowState.Maximized;
            }
           
        }

        /// <summary>
        /// Minimize the window
        /// </summary>
        public void MinimizeWindow(Window window)
        {
            window.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// To bring up the menu when clicking the SysIco
        /// </summary>
        public void OpenMenu(Window window)
        {
            Point position = Mouse.GetPosition(window);

            System.Diagnostics.Trace.WriteLine($"X: {position.X}, Y: {position.Y}");

            SystemCommands.ShowSystemMenu(window, new Point(position.X + window.Left, position.Y + window.Top));
        }
        #endregion
        #endregion
    }
}
