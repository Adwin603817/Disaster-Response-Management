using System;

namespace Disaster_Response_Coordination.Events
{
    public class DisasterEventArgs : EventArgs
    {
        public int DisasterID { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }

        public DisasterEventArgs(int disasterID, string message)
        {
            DisasterID = disasterID;
            Message = message;
            Timestamp = DateTime.Now;
        }
    }
}
