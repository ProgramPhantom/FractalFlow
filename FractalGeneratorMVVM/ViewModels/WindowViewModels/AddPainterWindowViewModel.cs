using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using FractalCore;
using System.Text.RegularExpressions;


namespace FractalGeneratorMVVM.ViewModels
{
    public class AddPainterWindowViewModel : Window
    {
        private BindableCollection<IPainter> _painters;



        public byte Red { get; set; } = 0;

        public byte Green { get; set; } = 0;

        public byte Blue { get; set; } = 0;

        public string PainterName { get; set; } = "Untitled";


        public AddPainterWindowViewModel(BindableCollection<IPainter> painters)
        {
            _painters = painters;
        }


        public void AddBasic()
        {
            _painters.Add(new BasicPainter(PainterName, Red, Green, Blue));
            
        }
    }
    


}
