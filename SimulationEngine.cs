using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Disaster_Response_Coordination.Models
{
    public class SimulationEngine
    {
        private SystemController _controller;
        private Random _random;
        private bool _isRunning;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _simulationTask;
        private int _simulationSpeed; // Milliseconds between events

        public SimulationEngine(SystemController controller, int simulationSpeed = 5000)
        {
            _controller = controller;
            _random = new Random();
            _isRunning = false;
            _simulationSpeed = simulationSpeed;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void StartSimulation()
        {
            if (_isRunning)
            {
                Console.WriteLine("[WARNING] Simulation is already running.");
                return;
            }

            _isRunning = true;
            _cancellationTokenSource = new CancellationTokenSource();
            _simulationTask = Task.Run(() => RunSimulation(_cancellationTokenSource.Token));
            Console.WriteLine("[INFO] Simulation started.");
        }

        public void StopSimulation()
        {
            if (!_isRunning)
            {
                Console.WriteLine("[WARNING] Simulation is not running.");
                return;
            }

            _isRunning = false;
            _cancellationTokenSource.Cancel();
            _simulationTask?.Wait();
            Console.WriteLine("[INFO] Simulation stopped.");
        }

        private void RunSimulation(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested && _isRunning)
                {
                    // Randomly generate new incidents
                    if (_random.Next(0, 3) == 0)
                    {
                        GenerateRandomIncident();
                    }

                    // Monitor system health
                    MonitorSystemHealth();

                    Thread.Sleep(_simulationSpeed);
                }
            }
            catch (OperationCanceledException)
            {
                _controller.Logger.Log("[INFO] Simulation cancelled.");
            }
        }

        private void GenerateRandomIncident()
        {
            try
            {
                string[] locations = { "Downtown", "North Suburbs", "Industrial Park", "Riverside", "East End", "Central Hospital" };
                SeverityLevel severity = (SeverityLevel)_random.Next(1, 5);
                string location = locations[_random.Next(locations.Length)];

                Disaster newDisaster;
                if (_random.Next(0, 2) == 0)
                {
                    bool nearResidential = _random.Next(0, 2) == 1;
                    newDisaster = new FireDisaster(location, severity, nearResidential);
                }
                else
                {
                    int waterDepth = _random.Next(1, 10);
                    newDisaster = new FloodDisaster(location, severity, waterDepth);
                }

                _controller.IncidentManager.AddIncident(newDisaster);
                _controller.Logger.Log($"[SIM] New incident generated: {newDisaster.IncidentID} - {newDisaster.GetType().Name} at {location}");
            }
            catch (Exception ex)
            {
                _controller.Logger.Log($"[ERROR] Failed to generate incident: {ex.Message}");
            }
        }

        private void MonitorSystemHealth()
        {
            var allTeams = _controller.TeamManager.GetAllTeams();
            var availableTeams = _controller.TeamManager.GetAvailableTeams();

            if (allTeams.Count > 0)
            {
                double availabilityPercentage = (availableTeams.Count / (double)allTeams.Count) * 100;

                if (availabilityPercentage < 30)
                {
                    _controller.Logger.Log($"[WARNING] Critical team availability: {availabilityPercentage:F1}%");
                }
            }

            var availableResources = _controller.ResourceManager.GetAvailableResources();
            var availableVehicles = _controller.ResourceManager.GetAvailableVehicles();

            _controller.Logger.Log($"[MONITOR] System Status - Teams: {availableTeams.Count}/{allTeams.Count} available | Resources: {availableResources.Count} available | Vehicles: {availableVehicles.Count} available");
        }

        public bool IsRunning => _isRunning;
    }
}