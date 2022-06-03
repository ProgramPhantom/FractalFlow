using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FractalCore;

namespace FractalGeneratorMVVM.ViewModels.Pages
{
    public class ConsolePageViewModel : Screen
    {

        private List<Status> _logs;

        public List<Status> Logs
        {
            get { return _logs; }
            set 
            { 
                _logs = value;
                NotifyOfPropertyChange(() => LogString);  // Clever!
            }
        }

        public void NewLog(Status s)
        {
            _logs.Add(s);
            NotifyOfPropertyChange(() => LogString);
        }

        public ConsolePageViewModel()
        {
            _logs = new List<Status> { new Status("Initializing...", NotificationType.Initialization) };
        }

        public string LogString
        {
            get
            {
                return string.Join("\n", Logs.Select(l => l.StatusDescription));
            }
        }

    }
}
