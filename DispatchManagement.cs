using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Disaster_Response_Coordination.Exceptions;
using Disaster_Response_Coordination.Interfaces;
using Disaster_Response_Coordination.Events;

namespace Disaster_Response_Coordination.Models
{
    class Dispatch
    {
        public int DispatchID;
        public bool DispatchStatus;
        public bool CompletionStatus;

        public Dispatch(int dispatchID, bool dispatchStatus, bool completionStatus)
        {
            DispatchID = dispatchID;
            DispatchStatus = dispatchStatus;
            CompletionStatus = completionStatus;
        }
    }
    class DispatchManagement : IDispatchService
    {
        //Containment for Teams, Disasters and Dispatch
        new List<Dispatch> Dispatches = new List<Dispatch>();
        new List<int> AvailableTeams = new List<int>();
        new List<int> NonAvailableTeams = new List<int>();
        new List<int> Disasters = new List<int>();

        //The events declared within the dispatch class
        public event EventHandler<DisasterEventArgs> DisasterAlertTrigger;
        public event EventHandler<DisasterEventArgs> ResponseComlpeted;
        public event EventHandler<string> TeamWarning;

        //Methods for the events
        protected virtual void OnDisasterAlertTrigged(DisasterEventArgs e) => DisasterAlertTrigger?.Invoke(this, e);
        protected virtual void OnResponseCompleted(DisasterEventArgs e) => ResponseComlpeted?.Invoke(this, e);
        protected virtual void OnTeamWarning(string warningMesage) => TeamWarning?.Invoke(this, warningMesage);


        //Method to Assign a team to a disaster
        public void AssignTeamToDisaster(int incidentID, int teamID)
        {

            if (!Disasters.Contains(incidentID))
            {
                throw new InvalidDispatchException("Disater ID does not exsist");
            }

            if (!AvailableTeams.Contains(teamID))
            {
                throw new InvalidDispatchException($"Team {teamID} either does not exsist or is currently unavailble");
            }
            int DispatchID = int.Parse($" {incidentID}{teamID}");
            Dispatch NewDispatch = new Dispatch(DispatchID, false, false);
            Dispatches.Add(NewDispatch);
            NonAvailableTeams.Add(teamID);
            AvailableTeams.RemoveAt(teamID);


            OnDisasterAlertTrigged(new DisasterEventArgs(incidentID, "A team has been sent to the Disaster"));

        }

        //Method used to Dispatch teams made for disaters
        public void DispatchTeam(int dispatchID)
        {
            if (dispatchID == null)
            {
                throw new InvalidDispatchException($"Dispatch ID {dispatchID} not found");
            }
            else
            {
                Dispatches[dispatchID].DispatchStatus = true;
            }

        }

        //Method used to cancel a teams dispatch 
        public void CancelDispatch(int dispatchID)
        {
            for (int i = 0; i < Dispatches.Count; i++)
            {
                if (i == dispatchID)
                {
                    Dispatches[i].DispatchStatus = false;
                }
                else
                {
                    continue;
                }
            }
        }


        //Method used to register a dispatch team as completed
        public void CompleteResponse(int dispatchID)
        {
            for (int i = 0; i < Dispatches.Count; i++)
            {
                if (i == dispatchID)
                {
                    Dispatches[i].CompletionStatus = true;

                    OnResponseCompleted(new DisasterEventArgs(dispatchID, "Response operation completed successfully"));
                }
                else
                {
                    continue;
                }
            }
        }

    }
}
