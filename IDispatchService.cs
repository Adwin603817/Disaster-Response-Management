using System;
using Disaster_Response_Coordination.Events;

namespace Disaster_Response_Coordination.Interfaces
{
    public interface IDispatchService
    {
        event EventHandler<DisasterEventArgs> DisasterAlertTrigger;
        event EventHandler<DisasterEventArgs> ResponseCompleted;
        event EventHandler<string> TeamWarning;

        void AssignTeamToDisaster(string incidentID, string teamID);
        void DispatchTeam(string dispatchID);
        void CancelDispatch(string dispatchID);
        void CompleteResponse(string dispatchID);
    }
}