using System;
using System.Collections.Generic;
using System.Linq;
using Disaster_Response_Coordination.Interfaces;

namespace Disaster_Response_Coordination.Models
{
    public abstract class ResponseTeam : ITrackable
    {
        private string _status = "Available";

        public string TeamID { get; private set; }
        public string TeamName { get; set; }
        public int MemberCount { get; set; }
        public List<string> Skills { get; protected set; }
        public bool IsAvailable { get; set; }
        public string CurrentIncident { get; set; }
        public DateTime LastUpdated { get; private set; }

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                LastUpdated = DateTime.Now;
            }
        }

        protected ResponseTeam(string teamName, int memberCount)
        {
            TeamID = "TEAM-" + Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
            TeamName = teamName;
            MemberCount = memberCount;
            Skills = new List<string>();
            IsAvailable = true;
            CurrentIncident = null;
            _status = "Available";
            LastUpdated = DateTime.Now;
        }

        public abstract string GetTeamType();
        public abstract int CalculateResponsePriority(Disaster disaster);

        public void AssignToIncident(string incidentID)
        {
            if (!IsAvailable)
                throw new Exceptions.ResourceUnavailableException($"Team {TeamID} is not available.");

            IsAvailable = false;
            CurrentIncident = incidentID;
            Status = $"Deployed to {incidentID}";
        }

        public void ReturnToBase()
        {
            IsAvailable = true;
            CurrentIncident = null;
            Status = "Available";
        }

        public void UpdateStatus(string newStatus)
        {
            Status = newStatus;
        }

        public string GetStatusReport()
        {
            return $"[{TeamID}] {TeamName} - Type: {GetTeamType()} - Members: {MemberCount} - Status: {Status} - Skills: {string.Join(", ", Skills)}";
        }

        public virtual void DisplayDetails()
        {
            Console.WriteLine(GetStatusReport());
            if (!string.IsNullOrEmpty(CurrentIncident))
                Console.WriteLine($"   -> Currently handling: {CurrentIncident}");
        }

        public override string ToString()
        {
            return GetStatusReport();
        }
    }
}