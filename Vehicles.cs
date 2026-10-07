using System;
using Disaster_Response_Coordination.Interfaces;

namespace Disaster_Response_Coordination.Models
{
    public class Vehicle : IAllocatable, ITrackable
    {
        private string _status = "Available";

        public string ID { get; private set; }
        public string Model { get; set; }
        public string Type { get; set; } // e.g., "Ambulance", "Fire Truck", "Rescue Boat"
        public int Capacity { get; set; }
        public bool IsAvailable { get; set; }
        public string CurrentAssignment { get; set; }
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

        public Vehicle(string model, string type, int capacity)
        {
            ID = "VEH-" + Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
            Model = model;
            Type = type;
            Capacity = capacity;
            IsAvailable = true;
            CurrentAssignment = null;
            _status = "Available";
            LastUpdated = DateTime.Now;
        }

        public void Allocate(string incidentID)
        {
            if (!IsAvailable)
                throw new Exceptions.ResourceUnavailableException($"Vehicle {ID} is not available for allocation.");

            IsAvailable = false;
            CurrentAssignment = incidentID;
            Status = $"Deployed to {incidentID}";
        }

        public void Release()
        {
            IsAvailable = true;
            CurrentAssignment = null;
            Status = "Available";
        }

        public void UpdateStatus(string newStatus)
        {
            Status = newStatus;
        }

        public string GetStatusReport()
        {
            return $"[{ID}] {Model} ({Type}) - Capacity: {Capacity} - Status: {Status}";
        }

        public void PerformMaintenance()
        {
            Status = "In Maintenance";
            IsAvailable = false;
        }

        public override string ToString()
        {
            return $"{GetStatusReport()}";
        }
    }
}