namespace Disaster_Response_Coordination.Interfaces
{
    public interface IAllocatable
    {
        string ID { get; }
        bool IsAvailable { get; set; }
        string CurrentAssignment { get; set; }
        void Allocate(string incidentID);
        void Release();
    }
}