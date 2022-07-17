using FractalCore;
using FractalCore.Painting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalGeneratorMVVM.ViewModels.Models.Painters
{
    public interface IPainterViewModel
    {
        public IPainter PainterModel { get; set; }
        public string Name { get; set; }
        public Guid ID { get; }
    }
}
