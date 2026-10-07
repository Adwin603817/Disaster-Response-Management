using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

namespace Disaster_Response_Coordination.Models
{
    public class DataManager
    {
        private readonly string _dataDirectory = "Data";
        private readonly string _incidentsFile;
        private readonly string _teamsFile;
        private readonly string _resourcesFile;

        public DataManager()
        {
            // Create Data directory if it doesn't exist
            if (!Directory.Exists(_dataDirectory))
            {
                Directory.CreateDirectory(_dataDirectory);
            }

            _incidentsFile = Path.Combine(_dataDirectory, "incidents.json");
            _teamsFile = Path.Combine(_dataDirectory, "teams.json");
            _resourcesFile = Path.Combine(_dataDirectory, "resources.json");
        }

        public void SaveIncidents(List<Disaster> incidents)
        {
            try
            {
                var incidentData = incidents.Select(i => new
                {
                    i.IncidentID,
                    i.Location,
                    Severity = i.Severity.ToString(),
                    Status = i.Status.ToString(),
                    i.TimeReported,
                    Type = i.GetType().Name,
                    Details = i is FireDisaster fire ? new { fire.IsNearResidential } :
                             i is FloodDisaster flood ? (object)new { flood.WaterDepthMeters } : null
                }).ToList();

                string json = JsonSerializer.Serialize(incidentData, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_incidentsFile, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to save incidents: {ex.Message}");
            }
        }

        public void SaveResources(List<Resource> resources, List<Vehicle> vehicles)
        {
            try
            {
                var resourceData = new
                {
                    Resources = resources.Select(r => new
                    {
                        r.ID,
                        r.Name,
                        r.Type,
                        r.Quantity,
                        r.IsAvailable,
                        r.CurrentAssignment
                    }).ToList(),
                    Vehicles = vehicles.Select(v => new
                    {
                        v.ID,
                        v.Model,
                        v.Type,
                        v.Capacity,
                        v.IsAvailable,
                        v.CurrentAssignment
                    }).ToList()
                };

                string json = JsonSerializer.Serialize(resourceData, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_resourcesFile, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to save resources: {ex.Message}");
            }
        }

        public void SaveSystemState(SystemController controller)
        {
            try
            {
                SaveIncidents(controller.IncidentManager.GetAllIncidents());
                SaveResources(controller.ResourceManager.GetAllResources(), controller.ResourceManager.GetAllVehicles());
                Console.WriteLine("[SUCCESS] System state saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to save system state: {ex.Message}");
            }
        }

        public bool HasSavedData()
        {
            return File.Exists(_incidentsFile) || File.Exists(_resourcesFile);
        }
    }
}