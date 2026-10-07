using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace Disaster_Response_Coordination
{
    // Enum for severity levels
    public enum SeverityLevel
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4
    }

    // Enum for incident status
    public enum IncidentStatus
    {
        Reported,
        InProgress,
        Resolved
    }

    // ABSTRACTION: Interface defines contract for allocatable objects
    public interface IAllocatable
    {
        string ID { get; }
        bool IsAvailable { get; set; }
        string CurrentAssignment { get; set; }
        void Allocate(string incidentID);
        void Release();
    }                   

    // ABSTRACTION: Interface defines contract for trackable objects
    public interface ITrackable
    {
        DateTime LastUpdated { get; }
        string Status { get; set; }
        void UpdateStatus(string newStatus);
        string GetStatusReport();
    }

    public class ResourceUnavailableException : Exception
    {
        public ResourceUnavailableException() : base("The requested resource is currently unavailable.") { }
        public ResourceUnavailableException(string message) : base(message) { }
        public ResourceUnavailableException(string message, Exception innerException) : base(message, innerException) { }
    }

    // DISASTER MANAGEMENT SECTION
    // ABSTRACTION & ENCAPSULATION: Base class template for all disaster types
    // Responsibility: Define common properties and behavior for different disaster incidents
    public abstract class Disaster
    {
        // ENCAPSULATION: Private setters restrict field modification
        public string IncidentID { get; private set; }
        public string Location { get; set; }
        public SeverityLevel Severity { get; set; }
        public IncidentStatus Status { get; set; }
        public DateTime TimeReported { get; private set; }

        protected Disaster(string location, SeverityLevel severity)
        {
            IncidentID = GenerateID();
            Location = location;
            Severity = severity;
            Status = IncidentStatus.Reported;
            TimeReported = DateTime.Now;
        }

        // ENCAPSULATION: Private method only accessible within this class
        private string GenerateID()
        {
            return "INC-" + Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
        }

        // ABSTRACTION: Subclasses must implement this method
        public abstract int CalculatePriority();

        // POLYMORPHISM: Virtual method can be overridden by derived classes
        public virtual void DisplayDetails()
        {
            Console.WriteLine($"[{IncidentID}] {this.GetType().Name} at {Location} | Severity: {Severity} | Status: {Status} | Priority Score: {CalculatePriority()}");
        }
    }

    // INHERITANCE & POLYMORPHISM: FireDisaster inherits from Disaster
    // Responsibility: Represent fire-type disasters with fire-specific priority calculation
    public class FireDisaster : Disaster
    {
        public bool IsNearResidential { get; set; }

        public FireDisaster(string location, SeverityLevel severity, bool isNearResidential)
            : base(location, severity)
        {
            IsNearResidential = isNearResidential;
        }

        // POLYMORPHISM: Override abstract method with fire-specific logic
        public override int CalculatePriority()
        {
            int baseScore = (int)Severity * 10;
            if (IsNearResidential)
            {
                baseScore += 50;
            }
            return baseScore;
        }

        // POLYMORPHISM: Override virtual method with fire-specific display
        public override void DisplayDetails()
        {
            base.DisplayDetails();
            Console.WriteLine($"   -> Near Residential Area: {IsNearResidential}");
        }
    }

    // INHERITANCE & POLYMORPHISM: FloodDisaster inherits from Disaster
    // Responsibility: Represent flood-type disasters with flood-specific priority calculation
    public class FloodDisaster : Disaster
    {
        public int WaterDepthMeters { get; set; }

        public FloodDisaster(string location, SeverityLevel severity, int waterDepthMeters)
            : base(location, severity)
        {
            WaterDepthMeters = waterDepthMeters;
        }

        // POLYMORPHISM: Override abstract method with flood-specific logic
        public override int CalculatePriority()
        {
            return ((int)Severity * 10) + (WaterDepthMeters * 5);
        }

        // POLYMORPHISM: Override virtual method with flood-specific display
        public override void DisplayDetails()
        {
            base.DisplayDetails();
            Console.WriteLine($"   -> Water Depth: {WaterDepthMeters} meters");
        }
    }

    // ENCAPSULATION: Manager class controls incident collection
    // Responsibility: Create, retrieve, update, and display all disaster incidents
    public class IncidentManager
    {
        private List<Disaster> _incidents;
        private Random _random;

        public IncidentManager()
        {
            _incidents = new List<Disaster>();
            _random = new Random();
        }

        public List<Disaster> GetAllIncidents()
        {
            return new List<Disaster>(_incidents);
        }

        public int GetIncidentCount()
        {
            return _incidents.Count;
        }

        public Disaster GetIncident(string incidentID)
        {
            return _incidents.FirstOrDefault(i => i.IncidentID == incidentID);
        }

        public void AddIncident(Disaster disaster)
        {
            _incidents.Add(disaster);
            Console.WriteLine($"\n[SUCCESS] Incident {disaster.IncidentID} added successfully.");
        }

        public void ViewAllIncidents()
        {
            if (!_incidents.Any())
            {
                Console.WriteLine("No incidents to display.");
                return;
            }
            Console.WriteLine("Active Incidents(Sorted by Priority):");
            // POLYMORPHISM: Each disaster type uses its own CalculatePriority implementation
            var sortedIncidents = _incidents.OrderByDescending(i => i.CalculatePriority()).ToList();

            foreach (var incident in sortedIncidents)
            {
                // POLYMORPHISM: Each disaster type uses its own DisplayDetails implementation
                incident.DisplayDetails();
                Console.WriteLine(new string('-', 30));
            }
        }

        public void UpdateIncidentStatus(string incidentID, IncidentStatus newStatus)
        {
            var incident = _incidents.FirstOrDefault(i => i.IncidentID == incidentID);
            if (incident != null)
            {
                incident.Status = newStatus;
                Console.WriteLine($"Incident {incidentID} status updated to {newStatus}");
            }
            else
            {
                Console.WriteLine($"Incident {incidentID} not found.");
            }
        }

        public void RemoveIncident(string incidentID)
        {
            var incident = _incidents.FirstOrDefault(i => i.IncidentID == incidentID);
            if (incident != null)
            {
                _incidents.Remove(incident);
                Console.WriteLine($"Incident {incidentID} removed successfully.");
            }
            else
            {
                Console.WriteLine($"Incident {incidentID} not found.");
            }
        }

        public void GenerateRandomIncidents(int count)
        {
            string[] locations = { "Downtown", "North Suburbs", "Industrial Park", "Riverside", "East End" };
            for (int i = 0; i < count; i++)
            {
                SeverityLevel randomSeverity = (SeverityLevel)_random.Next(1, 5);
                string randomLocation = locations[_random.Next(locations.Length)];
                if (_random.Next(0, 2) == 0)
                {
                    bool nearRes = _random.Next(0, 2) == 1;
                    // POLYMORPHISM: Creating FireDisaster with specific properties
                    _incidents.Add(new FireDisaster(randomLocation, randomSeverity, nearRes));
                }
                else
                {
                    int depth = _random.Next(1, 10);
                    // POLYMORPHISM: Creating FloodDisaster with specific properties
                    _incidents.Add(new FloodDisaster(randomLocation, randomSeverity, depth));
                }
            }
            Console.WriteLine($"Generated {count} random incidents.");
        }
    }

    // RESOURCE MANAGEMENT SECTION
    // ENCAPSULATION & ABSTRACTION: Resource implements interfaces to control access
    // Responsibility: Track supplies/equipment availability and assignment to incidents
    public class Resource : IAllocatable, ITrackable
    {
        private string _status = "Available";

        // ENCAPSULATION: Properties control field access
        public string ID { get; private set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Quantity { get; set; }
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

        public Resource(string name, string type, int quantity)
        {
            ID = "RES-" + Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
            Name = name;
            Type = type;
            Quantity = quantity;
            IsAvailable = true;
            CurrentAssignment = null;
            _status = "Available";
            LastUpdated = DateTime.Now;
        }

        // ABSTRACTION: Implements interface contract
        public void Allocate(string incidentID)
        {
            if (!IsAvailable)
                throw new ResourceUnavailableException($"Resource {ID} is not available for allocation.");

            IsAvailable = false;
            CurrentAssignment = incidentID;
            Status = $"Allocated to {incidentID}";
        }

        // ABSTRACTION: Implements interface contract
        public void Release()
        {
            IsAvailable = true;
            CurrentAssignment = null;
            Status = "Available";
        }

        // ABSTRACTION: Implements interface contract
        public void UpdateStatus(string newStatus)
        {
            Status = newStatus;
        }

        // ABSTRACTION: Implements interface contract
        public string GetStatusReport()
        {
            return $"[{ID}] {Name} ({Type}) - Quantity: {Quantity} - Status: {Status}";
        }

        public void UpdateQuantity(int newQuantity)
        {
            Quantity = newQuantity;
            LastUpdated = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{GetStatusReport()}";
        }
    }

    // ENCAPSULATION & ABSTRACTION: Vehicle implements interfaces to control access
    // Responsibility: Track emergency vehicles and their availability for dispatch
    public class Vehicle : IAllocatable, ITrackable
    {
        private string _status = "Available";

        // ENCAPSULATION: Properties control field access
        public string ID { get; private set; }
        public string Model { get; set; }
        public string Type { get; set; }
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

        // ABSTRACTION: Implements interface contract
        public void Allocate(string incidentID)
        {
            if (!IsAvailable)
                throw new ResourceUnavailableException($"Vehicle {ID} is not available for allocation.");

            IsAvailable = false;
            CurrentAssignment = incidentID;
            Status = $"Deployed to {incidentID}";
        }

        // ABSTRACTION: Implements interface contract
        public void Release()
        {
            IsAvailable = true;
            CurrentAssignment = null;
            Status = "Available";
        }

        // ABSTRACTION: Implements interface contract
        public void UpdateStatus(string newStatus)
        {
            Status = newStatus;
        }

        // ABSTRACTION: Implements interface contract
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

    // RESPONSE TEAM MANAGEMENT SECTION
    // ABSTRACTION & ENCAPSULATION: Base class template for response teams
    // Responsibility: Define common properties and behavior for different team types
    public abstract class ResponseTeam : ITrackable
    {
        private string _status = "Available";

        // ENCAPSULATION: Properties control field access
        public string TeamID { get; private set; }
        public string TeamName { get; set; }
        public int MemberCount { get; set; }
        // ENCAPSULATION: Protected field accessible only to derived classes
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

        // ENCAPSULATION: Protected constructor only accessible to derived classes
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

        // ABSTRACTION: Subclasses must implement this method
        public abstract string GetTeamType();
        // ABSTRACTION: Subclasses must implement this method
        public abstract int CalculateResponsePriority(Disaster disaster);

        public void AssignToIncident(string incidentID)
        {
            if (!IsAvailable)
                throw new ResourceUnavailableException($"Team {TeamID} is not available.");

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

        // ABSTRACTION: Implements interface contract
        public void UpdateStatus(string newStatus)
        {
            Status = newStatus;
        }

        // ABSTRACTION: Implements interface contract
        public string GetStatusReport()
        {
            return $"[{TeamID}] {TeamName} - Type: {GetTeamType()} - Members: {MemberCount} - Status: {Status} - Skills: {string.Join(", ", Skills)}";
        }

        // POLYMORPHISM: Virtual method can be overridden by derived classes
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

    // INHERITANCE & POLYMORPHISM: MedicalTeam inherits from ResponseTeam
    // Responsibility: Handle medical emergencies with doctors, nurses, and ambulances
    public class MedicalTeam : ResponseTeam
    {
        public int NumDoctors { get; set; }
        public int NumNurses { get; set; }
        public bool HasAmbulance { get; set; }

        public MedicalTeam(string teamName, int memberCount, int numDoctors, int numNurses, bool hasAmbulance)
            : base(teamName, memberCount)
        {
            NumDoctors = numDoctors;
            NumNurses = numNurses;
            HasAmbulance = hasAmbulance;
            Skills.AddRange(new string[] { "First Aid", "Triage", "Emergency Care", "Patient Transport" });
        }

        // POLYMORPHISM: Override abstract method with medical-specific implementation
        public override string GetTeamType()
        {
            return "Medical";
        }

        // POLYMORPHISM: Override abstract method with medical-specific priority logic
        public override int CalculateResponsePriority(Disaster disaster)
        {
            int priority = (int)disaster.Severity * 10;

            if (disaster is FireDisaster fire && fire.IsNearResidential)
                priority += 20;

            if (HasAmbulance)
                priority += 10;

            return priority;
        }

        // POLYMORPHISM: Override virtual method with medical-specific details
        public override void DisplayDetails()
        {
            base.DisplayDetails();
            Console.WriteLine($"   -> Doctors: {NumDoctors}, Nurses: {NumNurses}, Ambulance: {HasAmbulance}");
        }
    }

    // INHERITANCE & POLYMORPHISM: FireTeam inherits from ResponseTeam
    // Responsibility: Handle fire emergencies with firefighters and fire suppression equipment
    public class FireTeam : ResponseTeam
    {
        public int NumFirefighters { get; set; }
        public int NumTrucks { get; set; }
        public bool HasAerialSupport { get; set; }

        public FireTeam(string teamName, int memberCount, int numFirefighters, int numTrucks, bool hasAerialSupport)
            : base(teamName, memberCount)
        {
            NumFirefighters = numFirefighters;
            NumTrucks = numTrucks;
            HasAerialSupport = hasAerialSupport;
            Skills.AddRange(new string[] { "Fire Suppression", "Rescue", "Hazardous Materials" });
        }

        // POLYMORPHISM: Override abstract method with fire-specific implementation
        public override string GetTeamType()
        {
            return "Fire";
        }

        // POLYMORPHISM: Override abstract method with fire-specific priority logic
        public override int CalculateResponsePriority(Disaster disaster)
        {
            int priority = (int)disaster.Severity * 10;

            if (disaster is FireDisaster fire)
            {
                priority += 30;

                if (fire.IsNearResidential)
                    priority += 20;
            }
            else if (disaster is FloodDisaster flood && flood.WaterDepthMeters > 3)
            {
                priority += 15;
            }

            return priority;
        }

        // POLYMORPHISM: Override virtual method with fire-specific details
        public override void DisplayDetails()
        {
            base.DisplayDetails();
            Console.WriteLine($"   -> Firefighters: {NumFirefighters}, Trucks: {NumTrucks}, Aerial Support: {HasAerialSupport}");
        }
    }

    // INHERITANCE & POLYMORPHISM: RescueTeam inherits from ResponseTeam
    // Responsibility: Handle water and technical rescues with specialized rescue equipment
    public class RescueTeam : ResponseTeam
    {
        public int NumRescuers { get; set; }
        public bool HasWaterRescue { get; set; }
        public bool HasTechnicalRescue { get; set; }

        public RescueTeam(string teamName, int memberCount, int numRescuers, bool hasWaterRescue, bool hasTechnicalRescue)
            : base(teamName, memberCount)
        {
            NumRescuers = numRescuers;
            HasWaterRescue = hasWaterRescue;
            HasTechnicalRescue = hasTechnicalRescue;
            Skills.AddRange(new string[] { "Search and Rescue", "Extraction", "Emergency Response" });

            if (HasWaterRescue)
                Skills.Add("Water Rescue");
            if (HasTechnicalRescue)
                Skills.Add("Technical Rescue");
        }

        // POLYMORPHISM: Override abstract method with rescue-specific implementation
        public override string GetTeamType()
        {
            return "Rescue";
        }

        // POLYMORPHISM: Override abstract method with rescue-specific priority logic
        public override int CalculateResponsePriority(Disaster disaster)
        {
            int priority = (int)disaster.Severity * 10;

            if (disaster is FloodDisaster flood)
            {
                priority += 30;

                if (flood.WaterDepthMeters > 2)
                    priority += 20;

                if (HasWaterRescue)
                    priority += 15;
            }
            else if (disaster is FireDisaster fire && fire.IsNearResidential)
            {
                priority += 15;
            }

            return priority;
        }

        // POLYMORPHISM: Override virtual method with rescue-specific details
        public override void DisplayDetails()
        {
            base.DisplayDetails();
            Console.WriteLine($"   -> Rescuers: {NumRescuers}, Water Rescue: {HasWaterRescue}, Technical Rescue: {HasTechnicalRescue}");
        }
    }

    // SYSTEM MANAGEMENT SECTION
    // ENCAPSULATION: Logger manages log collection privately
    // Responsibility: Record all system events and operations with timestamps
    public class Logger
    {
        // ENCAPSULATION: Private list not accessible from outside
        private List<string> _logs = new List<string>();
        private readonly object _lock = new object();

        public void Log(string message)
        {
            var entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            lock (_lock) { _logs.Add(entry); }
            Console.WriteLine(entry);
        }

        public void DisplayLogs()
        {
            Console.WriteLine("\n=== SYSTEM LOGS ===");
            lock (_lock)
            {
                foreach (var l in _logs) Console.WriteLine(l);
            }
            Console.WriteLine("===================\n");
        }

        public void SaveLogs(string filePath)
        {
            lock (_lock)
            {
                File.WriteAllLines(filePath, _logs);
            }
        }
    }

    // ENCAPSULATION: TeamManager controls team collection privately
    // Responsibility: Manage all response teams and match them to incidents by priority
    public class TeamManager
    {
        // ENCAPSULATION: Private list not accessible from outside
        private List<ResponseTeam> _teams = new List<ResponseTeam>();
        private Logger _logger;
        private Random _rand = new Random();

        public TeamManager(Logger logger)
        {
            _logger = logger;
        }

        public int GetTeamCount() => _teams.Count;

        public void GenerateRandomTeams()
        {
            // POLYMORPHISM: Creating different team types, storing in single list
            _teams.Add(new MedicalTeam("Central Med", 8, 2, 4, true));
            _teams.Add(new FireTeam("Station 1", 10, 8, 2, false));
            _teams.Add(new RescueTeam("Swift Rescue", 6, 4, true, true));
            _logger.Log("[TEAM] Generated default teams.");
        }

        public List<ResponseTeam> GetAvailableTeams()
        {
            return _teams.Where(t => t.IsAvailable).ToList();
        }

        public void ViewAllTeams()
        {
            if (!_teams.Any()) { Console.WriteLine("No teams available."); return; }
            // POLYMORPHISM: Each team uses its own DisplayDetails implementation
            foreach (var t in _teams) t.DisplayDetails();
        }

        public void AddTeam(ResponseTeam team)
        {
            _teams.Add(team);
            _logger.Log($"[TEAM] Added {team.TeamID}");
        }

        public void UpdateTeamStatus(string teamID, string newStatus)
        {
            var t = _teams.FirstOrDefault(x => x.TeamID == teamID);
            if (t != null) t.UpdateStatus(newStatus);
            else Console.WriteLine("Team not found.");
        }

        public void RemoveTeam(string teamID)
        {
            var t = _teams.FirstOrDefault(x => x.TeamID == teamID);
            if (t != null) { _teams.Remove(t); _logger.Log($"[TEAM] Removed {teamID}"); }
            else Console.WriteLine("Team not found.");
        }

        // POLYMORPHISM: Each team calculates priority differently for given disaster
        public List<ResponseTeam> GetBestTeamsForDisaster(Disaster disaster)
        {
            return _teams.OrderByDescending(t => t.CalculateResponsePriority(disaster)).ToList();
        }

        public ResponseTeam GetTeam(string teamID)
        {
            return _teams.FirstOrDefault(t => t.TeamID == teamID);
        }
    }

    // ENCAPSULATION: ResourceManager controls collections privately
    // Responsibility: Manage supplies and vehicles, track availability and allocation
    public class ResourceManager
    {
        // ENCAPSULATION: Private lists not accessible from outside
        private List<Resource> _resources = new List<Resource>();
        private List<Vehicle> _vehicles = new List<Vehicle>();
        private Logger _logger;

        public ResourceManager(Logger logger)
        {
            _logger = logger;
        }

        public int GetResourceCount() => _resources.Count + _vehicles.Count;

        public void GenerateDefaultResources()
        {
            _resources.Add(new Resource("Water Bags", "Supplies", 200));
            _resources.Add(new Resource("Medical Kits", "Medical", 50));
            _vehicles.Add(new Vehicle("Ambulance A1", "Ambulance", 4));
            _vehicles.Add(new Vehicle("Fire Truck F1", "FireTruck", 6));
            _logger.Log("[RESOURCE] Generated default resources and vehicles.");
        }

        public void ViewAllResources()
        {
            if (!_resources.Any()) { Console.WriteLine("No resources."); }
            foreach (var r in _resources) Console.WriteLine(r.GetStatusReport());
        }

        public void ViewAllVehicles()
        {
            if (!_vehicles.Any()) { Console.WriteLine("No vehicles."); }
            foreach (var v in _vehicles) Console.WriteLine(v.GetStatusReport());
        }

        public List<Resource> GetAvailableResources() => _resources.Where(r => r.IsAvailable).ToList();
        public List<Vehicle> GetAvailableVehicles() => _vehicles.Where(v => v.IsAvailable).ToList();

        public void AddResource(Resource res) { _resources.Add(res); _logger.Log($"[RESOURCE] Added {res.ID}"); }
        public void AddVehicle(Vehicle veh) { _vehicles.Add(veh); _logger.Log($"[VEHICLE] Added {veh.ID}"); }

        public void RemoveResource(string id)
        {
            var r = _resources.FirstOrDefault(x => x.ID == id);
            if (r != null) { _resources.Remove(r); _logger.Log($"[RESOURCE] Removed {id}"); }
            else Console.WriteLine("Resource not found.");
        }

        public void RemoveVehicle(string id)
        {
            var v = _vehicles.FirstOrDefault(x => x.ID == id);
            if (v != null) { _vehicles.Remove(v); _logger.Log($"[VEHICLE] Removed {id}"); }
            else Console.WriteLine("Vehicle not found.");
        }

        public Resource GetResource(string id) => _resources.FirstOrDefault(r => r.ID == id);
        public Vehicle GetVehicle(string id) => _vehicles.FirstOrDefault(v => v.ID == id);
    }

    // ENCAPSULATION: DispatchManager controls team assignments
    // Responsibility: Assign response teams to incidents and track deployment
    public class DispatchManager
    {
        private SystemController _controller;
        private Logger _logger;

        public DispatchManager(SystemController controller, Logger logger)
        {
            _controller = controller;
            _logger = logger;
        }

        public void AssignTeamToDisaster(string incidentID, string teamID)
        {
            var incident = _controller.IncidentManager.GetIncident(incidentID);
            if (incident == null) throw new ArgumentException("Incident not found.");

            var team = _controller.TeamManager.GetTeam(teamID);
            if (team == null) throw new ArgumentException("Team not found.");

            if (!team.IsAvailable) throw new InvalidOperationException("Team not available.");

            team.AssignToIncident(incidentID);
            incident.Status = IncidentStatus.InProgress;
            _logger.Log($"[DISPATCH] Team {teamID} assigned to {incidentID}");
        }
    }

    // ENCAPSULATION & COMPOSITION: Central controller manages all managers
    // Responsibility: Coordinate and provide access to all system managers
    public class SystemController
    {
        public IncidentManager IncidentManager { get; private set; }
        public TeamManager TeamManager { get; private set; }
        public ResourceManager ResourceManager { get; private set; }
        public DispatchManager DispatchManager { get; private set; }
        public Logger Logger { get; private set; }

        public SystemController()
        {
            Logger = new Logger();
            IncidentManager = new IncidentManager();
            TeamManager = new TeamManager(Logger);
            ResourceManager = new ResourceManager(Logger);
            DispatchManager = new DispatchManager(this, Logger);
        }

        public void DisplaySystemStatus()
        {
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║              SYSTEM STATUS                                 ║");
            Console.WriteLine("╠════════════════════════════════════════════════════════════╣");
            Console.WriteLine($"║  Incidents: {IncidentManager.GetIncidentCount(),-49} ║");
            Console.WriteLine($"║  Teams: {TeamManager.GetTeamCount(),-53} ║");
            Console.WriteLine($"║  Resources & Vehicles: {ResourceManager.GetResourceCount(),-40} ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝\n");
        }
    }

    // ENCAPSULATION: DataManager handles file operations privately
    // Responsibility: Save and load system state to/from persistent storage
    public class DataManager
    {
        private const string DefaultSave = "system_state.txt";

        public void SaveSystemState(SystemController controller, string path = DefaultSave)
        {
            try
            {
                using (var sw = new StreamWriter(path, false))
                {
                    sw.WriteLine($"Incidents:{controller.IncidentManager.GetIncidentCount()}");
                    sw.WriteLine($"Teams:{controller.TeamManager.GetTeamCount()}");
                    sw.WriteLine($"Resources:{controller.ResourceManager.GetResourceCount()}");
                }
                controller.Logger.Log($"[DATA] System state saved to {path}");
            }
            catch (Exception ex)
            {
                controller.Logger.Log($"[DATA ERROR] Failed to save state: {ex.Message}");
            }
        }

        public void LoadSystemState(SystemController controller, string path = DefaultSave)
        {
            if (!File.Exists(path)) { controller.Logger.Log("[DATA] No saved state found."); return; }
            controller.Logger.Log($"[DATA] Loaded saved state from {path}");
        }
    }

    // ENCAPSULATION: SimulationEngine manages simulation privately
    // Responsibility: Generate random incidents at intervals to simulate disaster events
    public class SimulationEngine
    {
        // ENCAPSULATION: Private fields not accessible from outside
        private SystemController _controller;
        private CancellationTokenSource _cts;
        private int _intervalMs;
        public bool IsRunning { get; private set; }

        public SimulationEngine(SystemController controller, int simulationSpeed = 3000)
        {
            _controller = controller;
            _intervalMs = simulationSpeed;
        }

        public void StartSimulation()
        {
            if (IsRunning) return;
            _cts = new CancellationTokenSource();
            IsRunning = true;
            Task.Run(() => Run(_cts.Token));
            _controller.Logger.Log("[SIM] Simulation started.");
        }

        public void StopSimulation()
        {
            if (!IsRunning) return;
            _cts.Cancel();
            IsRunning = false;
            _controller.Logger.Log("[SIM] Simulation stopped.");
        }

        // ENCAPSULATION: Private method used only internally
        private async Task Run(CancellationToken token)
        {
            var rand = new Random();
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(_intervalMs, token).ContinueWith(_ => { });
                if (token.IsCancellationRequested) break;

                int pick = rand.Next(0, 3);
                if (pick == 0)
                {
                    var loc = $"SimArea-{rand.Next(1, 10)}";
                    var sev = (SeverityLevel)rand.Next(1, 5);
                    if (rand.Next(0, 2) == 0)
                    {
                        // POLYMORPHISM: Creating FireDisaster polymorphically
                        var fd = new FireDisaster(loc, sev, rand.Next(0, 2) == 1);
                        _controller.IncidentManager.AddIncident(fd);
                        _controller.Logger.Log($"[SIM] New simulated incident {fd.IncidentID}");
                    }
                    else
                    {
                        // POLYMORPHISM: Creating FloodDisaster polymorphically
                        var fl = new FloodDisaster(loc, sev, rand.Next(1, 6));
                        _controller.IncidentManager.AddIncident(fl);
                        _controller.Logger.Log($"[SIM] New simulated incident {fl.IncidentID}");
                    }
                }
            }
        }
    }

    // MAIN APPLICATION SECTION
    // Responsibility: Initialize system, display user interface, and coordinate menu operations
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║   DISASTER RESPONSE COORDINATION SYSTEM v1.0               ║");
                Console.WriteLine("║   Integrated Emergency Management Platform                ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════════╝\n");

                SystemController controller = new SystemController();
                DataManager dataManager = new DataManager();
                SimulationEngine simulator = new SimulationEngine(controller, simulationSpeed: 3000);

                controller.Logger.Log("[STARTUP] Initializing Disaster Response Coordination System...");
                controller.TeamManager.GenerateRandomTeams();
                controller.ResourceManager.GenerateDefaultResources();
                controller.IncidentManager.GenerateRandomIncidents(3);

                controller.Logger.Log("[STARTUP] System initialized successfully.");

                bool systemRunning = true;
                while (systemRunning)
                {
                    DisplayMainMenu();
                    string choice = Console.ReadLine()?.ToUpper() ?? "";

                    try
                    {
                        switch (choice)
                        {
                            case "1":
                                ManageIncidents(controller);
                                break;
                            case "2":
                                ManageTeams(controller);
                                break;
                            case "3":
                                ManageResources(controller);
                                break;
                            case "4":
                                ManageDispatch(controller);
                                break;
                            case "5":
                                ManageSimulation(controller, simulator);
                                break;
                            case "6":
                                ViewSystemStatus(controller);
                                break;
                            case "7":
                                dataManager.SaveSystemState(controller);
                                break;
                            case "8":
                                controller.Logger.DisplayLogs();
                                break;
                            case "9":
                                systemRunning = ExitSystem(controller, simulator, dataManager);
                                break;
                            default:
                                Console.WriteLine("[ERROR] Invalid choice. Please try again.");
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        controller.Logger.Log($"[ERROR] {ex.Message}");
                    }

                    if (systemRunning)
                    {
                        Console.WriteLine("\nPress any key to continue...");
                        Console.ReadKey();
                        Console.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CRITICAL ERROR] {ex.Message}");
                Console.ReadKey();
            }
        }

        static void DisplayMainMenu()
        {
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    MAIN MENU                                ║");
            Console.WriteLine("╠════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║  1. Manage Incidents                                       ║");
            Console.WriteLine("║  2. Manage Response Teams                                  ║");
            Console.WriteLine("║  3. Manage Resources & Vehicles                            ║");
            Console.WriteLine("║  4. Manage Dispatch Operations                             ║");
            Console.WriteLine("║  5. Run System Simulation                                  ║");
            Console.WriteLine("║  6. View System Status                                     ║");
            Console.WriteLine("║  7. Save System State                                      ║");
            Console.WriteLine("║  8. View Logs                                              ║");
            Console.WriteLine("║  9. Exit System                                            ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.Write("\nEnter your choice (1-9): ");
        }

        // INCIDENT MANAGEMENT: Create, update, and view disaster incidents
        static void ManageIncidents(SystemController controller)
        {
            bool managing = true;
            while (managing)
            {
                Console.WriteLine("\n╔════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║               INCIDENT MANAGEMENT                         ║");
                Console.WriteLine("╠════════════════════════════════════════════════════════════╣");
                Console.WriteLine("║  1. View All Incidents                                   ║");
                Console.WriteLine("║  2. Add Fire Incident                                    ║");
                Console.WriteLine("║  3. Add Flood Incident                                   ║");
                Console.WriteLine("║  4. Update Incident Status                               ║");
                Console.WriteLine("║  5. Remove Incident                                      ║");
                Console.WriteLine("║  6. Generate Random Incidents                            ║");
                Console.WriteLine("║  7. Back to Main Menu                                    ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
                Console.Write("Enter your choice (1-7): ");

                string choice = Console.ReadLine()?.ToUpper() ?? "";
                switch (choice)
                {
                    case "1":
                        controller.IncidentManager.ViewAllIncidents();
                        break;

                    case "2":
                        Console.Write("Enter location: ");
                        string fireLocation = Console.ReadLine() ?? "";
                        Console.Write("Enter severity (Low=1, Medium=2, High=3, Critical=4): ");
                        if (int.TryParse(Console.ReadLine(), out int severity))
                        {
                            Console.Write("Is near residential area? (y/n): ");
                            bool nearRes = Console.ReadLine()?.ToLower() == "y";
                            // POLYMORPHISM: Creating FireDisaster with specific properties
                            var fireDis = new FireDisaster(fireLocation, (SeverityLevel)severity, nearRes);
                            controller.IncidentManager.AddIncident(fireDis);
                            controller.Logger.Log($"Fire incident created: {fireDis.IncidentID}");
                        }
                        break;

                    case "3":
                        Console.Write("Enter location: ");
                        string floodLocation = Console.ReadLine() ?? "";
                        Console.Write("Enter severity (Low=1, Medium=2, High=3, Critical=4): ");
                        if (int.TryParse(Console.ReadLine(), out int floodSeverity))
                        {
                            Console.Write("Enter water depth (meters): ");
                            if (int.TryParse(Console.ReadLine(), out int depth))
                            {
                                // POLYMORPHISM: Creating FloodDisaster with specific properties
                                var floodDis = new FloodDisaster(floodLocation, (SeverityLevel)floodSeverity, depth);
                                controller.IncidentManager.AddIncident(floodDis);
                                controller.Logger.Log($"Flood incident created: {floodDis.IncidentID}");
                            }
                        }
                        break;

                    case "4":
                        Console.Write("Enter incident ID to update: ");
                        string incidentID = Console.ReadLine() ?? "";
                        Console.Write("Enter new status (Reported=0, InProgress=1, Resolved=2): ");
                        if (int.TryParse(Console.ReadLine(), out int statusChoice))
                        {
                            IncidentStatus newStatus = (IncidentStatus)statusChoice;
                            controller.IncidentManager.UpdateIncidentStatus(incidentID, newStatus);
                            controller.Logger.Log($"Incident {incidentID} status updated to {newStatus}");
                        }
                        break;

                    case "5":
                        Console.Write("Enter incident ID to remove: ");
                        string removeID = Console.ReadLine() ?? "";
                        controller.IncidentManager.RemoveIncident(removeID);
                        controller.Logger.Log($"Attempted to remove incident {removeID}");
                        break;

                    case "6":
                        Console.Write("Enter number of random incidents to generate: ");
                        if (int.TryParse(Console.ReadLine(), out int count))
                        {
                            controller.IncidentManager.GenerateRandomIncidents(count);
                            controller.Logger.Log($"Generated {count} random incidents");
                        }
                        break;

                    case "7":
                        managing = false;
                        break;

                    default:
                        Console.WriteLine("[ERROR] Invalid choice.");
                        break;
                }
            }
        }

        // TEAM MANAGEMENT: View and query response teams
        static void ManageTeams(SystemController controller)
        {
            bool managing = true;
            while (managing)
            {
                Console.WriteLine("\n╔════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                 TEAM MANAGEMENT                           ║");
                Console.WriteLine("╠════════════════════════════════════════════════════════════╣");
                Console.WriteLine("║  1. View All Teams                                       ║");
                Console.WriteLine("║  2. View Available Teams                                 ║");
                Console.WriteLine("║  3. View Teams Best Suited for Incident                  ║");
                Console.WriteLine("║  4. Back to Main Menu                                    ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
                Console.Write("Enter your choice (1-4): ");

                string choice = Console.ReadLine()?.ToUpper() ?? "";
                switch (choice)
                {
                    case "1":
                        controller.TeamManager.ViewAllTeams();
                        break;

                    case "2":
                        var available = controller.TeamManager.GetAvailableTeams();
                        if (available.Count == 0)
                            Console.WriteLine("No teams available.");
                        else
                            foreach (var team in available)
                                // POLYMORPHISM: Each team type uses its own DisplayDetails
                                team.DisplayDetails();
                        break;

                    case "3":
                        Console.Write("Enter incident ID: ");
                        string incID = Console.ReadLine() ?? "";
                        var incident = controller.IncidentManager.GetIncident(incID);
                        if (incident != null)
                        {
                            // POLYMORPHISM: Each team calculates priority differently
                            var bestTeams = controller.TeamManager.GetBestTeamsForDisaster(incident);
                            Console.WriteLine($"Best teams for {incident.IncidentID}:");
                            foreach (var team in bestTeams)
                                team.DisplayDetails();
                        }
                        else
                            Console.WriteLine("Incident not found.");
                        break;

                    case "4":
                        managing = false;
                        break;

                    default:
                        Console.WriteLine("[ERROR] Invalid choice.");
                        break;
                }
            }
        }

        // RESOURCE MANAGEMENT: View available resources and vehicles
        static void ManageResources(SystemController controller)
        {
            bool managing = true;
            while (managing)
            {
                Console.WriteLine("\n╔════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║            RESOURCES & VEHICLES MANAGEMENT                ║");
                Console.WriteLine("╠════════════════════════════════════════════════════════════╣");
                Console.WriteLine("║  1. View All Resources                                   ║");
                Console.WriteLine("║  2. View All Vehicles                                    ║");
                Console.WriteLine("║  3. View Available Resources                             ║");
                Console.WriteLine("║  4. View Available Vehicles                              ║");
                Console.WriteLine("║  5. Back to Main Menu                                    ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
                Console.Write("Enter your choice (1-5): ");

                string choice = Console.ReadLine()?.ToUpper() ?? "";
                switch (choice)
                {
                    case "1":
                        controller.ResourceManager.ViewAllResources();
                        break;

                    case "2":
                        controller.ResourceManager.ViewAllVehicles();
                        break;

                    case "3":
                        var availRes = controller.ResourceManager.GetAvailableResources();
                        if (availRes.Count == 0)
                            Console.WriteLine("No available resources.");
                        else
                            foreach (var res in availRes)
                                Console.WriteLine(res.GetStatusReport());
                        break;

                    case "4":
                        var availVeh = controller.ResourceManager.GetAvailableVehicles();
                        if (availVeh.Count == 0)
                            Console.WriteLine("No available vehicles.");
                        else
                            foreach (var veh in availVeh)
                                Console.WriteLine(veh.GetStatusReport());
                        break;

                    case "5":
                        managing = false;
                        break;

                    default:
                        Console.WriteLine("[ERROR] Invalid choice.");
                        break;
                }
            }
        }

        // DISPATCH MANAGEMENT: Assign teams to specific incidents
        static void ManageDispatch(SystemController controller)
        {
            bool managing = true;
            while (managing)
            {
                Console.WriteLine("\n╔════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║            DISPATCH OPERATIONS MANAGEMENT                 ║");
                Console.WriteLine("╠════════════════════════════════════════════════════════════╣");
                Console.WriteLine("║  1. Assign Team to Incident                              ║");
                Console.WriteLine("║  2. Back to Main Menu                                    ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
                Console.Write("Enter your choice (1-2): ");

                string choice = Console.ReadLine()?.ToUpper() ?? "";
                switch (choice)
                {
                    case "1":
                        Console.Write("Enter incident ID: ");
                        string incID = Console.ReadLine() ?? "";
                        Console.Write("Enter team ID: ");
                        string teamID = Console.ReadLine() ?? "";

                        try
                        {
                            controller.DispatchManager.AssignTeamToDisaster(incID, teamID);
                            Console.WriteLine("[SUCCESS] Team assigned to incident.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[ERROR] {ex.Message}");
                        }
                        break;

                    case "2":
                        managing = false;
                        break;

                    default:
                        Console.WriteLine("[ERROR] Invalid choice.");
                        break;
                }
            }
        }

        // SIMULATION MANAGEMENT: Start and stop the disaster simulation engine
        static void ManageSimulation(SystemController controller, SimulationEngine simulator)
        {
            bool managing = true;
            while (managing)
            {
                Console.WriteLine("\n╔════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║              SIMULATION MANAGEMENT                        ║");
                Console.WriteLine("╠════════════════════════════════════════════════════════════╣");
                Console.WriteLine($"║  Simulation Status: {(simulator.IsRunning ? "RUNNING" : "STOPPED"),-41} ║");
                Console.WriteLine("║  1. Start Simulation                                     ║");
                Console.WriteLine("║  2. Stop Simulation                                      ║");
                Console.WriteLine("║  3. Back to Main Menu                                    ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
                Console.Write("Enter your choice (1-3): ");

                string choice = Console.ReadLine()?.ToUpper() ?? "";
                switch (choice)
                {
                    case "1":
                        simulator.StartSimulation();
                        break;

                    case "2":
                        simulator.StopSimulation();
                        break;

                    case "3":
                        managing = false;
                        break;

                    default:
                        Console.WriteLine("[ERROR] Invalid choice.");
                        break;
                }
            }
        }

        // SYSTEM STATUS: Display overall system status
        static void ViewSystemStatus(SystemController controller)
        {
            controller.DisplaySystemStatus();
        }

        // SYSTEM EXIT: Gracefully shut down and save system state
        static bool ExitSystem(SystemController controller, SimulationEngine simulator, DataManager dataManager)
        {
            simulator.StopSimulation();
            dataManager.SaveSystemState(controller);
            controller.Logger.Log("[SHUTDOWN] System shutting down gracefully...");
            Console.WriteLine("\n[SYSTEM] Goodbye!");
            Thread.Sleep(1000);
            return false;
        }
    }
}