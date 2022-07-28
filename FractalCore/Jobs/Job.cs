using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalCore
{
    public enum NotificationType { OperationComplete, OperationCancel, Initialization, RenderDuration, Zoom, Complile, Misc}

    public struct Status
    {
        public static string Prompt = ">>>";

        private string _statusDescription;

        
        public DateTime TimeCreated;
        public NotificationType NotificationType;

        public string LogPrefix
        {
            get
            {
                return $"{TimeCreated.ToString("HH:mm")} {NotificationType.ToString()}";
            }
        }

        public string StatusDescription
        {
            get
            {
                return $"{LogPrefix.PadRight(35)} {Prompt} {_statusDescription}";
            }
            set
            {
                _statusDescription = value;
            }
        }

        public Status(string status, NotificationType type)
        {
            _statusDescription = status;
            TimeCreated = DateTime.Now;
            NotificationType = type;
        }
    }

    public delegate void StatusUpdate(Status statusStruct);

    public class Job
    {
        public event StatusUpdate? StatusUpdateEvent;

        #region Fields
        private Status _status;
        private int _jobNum;
        #endregion

        #region Properties
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
