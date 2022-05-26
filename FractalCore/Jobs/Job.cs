using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalCore
{
    public delegate void StatusUpdate(string message);

    public class Job
    {
        #region Fields
        private string _status = "NO STATUS";

        private string _jobID = "XH23&2^cS^£";
        #endregion

        #region Properties
        public event StatusUpdate? StatusUpdateEvent;
        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                UpdateShell(_status);
            }
        }

        public string JobID
        {
            get { return _jobID; }
            set { _jobID = value; }
        }
        #endregion



        protected virtual void UpdateShell(string message)
        {
            StatusUpdateEvent?.Invoke(message);
        }
    }
}
