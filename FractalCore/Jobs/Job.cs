using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalCore
{
    public enum NotificationType { OperationComplete, OperationCancel, Initialization }

    public struct Status
    {
        public string StatusDescription;
        public DateTime TimeCreated;
        public NotificationType NotificationType;

        public Status(string status, NotificationType type)
        {
            StatusDescription = status;
            TimeCreated = DateTime.Now;
            NotificationType = type;
        }
    }

    public delegate void StatusUpdate(Status statusStruct);

    public class Job
    {
        #region Fields
        private Status _status;
        private int _jobNum;
        private string _jobID = "XH23&2^cS^£";
        #endregion

        #region Properties
        public event StatusUpdate? StatusUpdateEvent;


        public Status JobStatus
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }

        public int JobNum
        {
            get { return _jobNum; }
            set { _jobNum = value; }
        }

        public string JobID
        {
            get { return _jobID; }
            set { _jobID = value; }
        }
        #endregion

        public Job(int num)
        {
            _jobNum = num;
        }

        public void SetStatus(string statusMessage, NotificationType type)
        {
            JobStatus = new Status(statusMessage, type);

            UpdateShell();
        }

        protected virtual void UpdateShell()
        {
            StatusUpdateEvent?.Invoke(JobStatus);
        }
    }
}
