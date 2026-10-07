using System;

namespace Disaster_Response_Coordination.Interfaces
{
    public interface ITrackable
    {
        DateTime LastUpdated { get; }
        string Status { get; set; }
        void UpdateStatus(string newStatus);
        string GetStatusReport();
    }
}