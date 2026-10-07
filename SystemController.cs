using System;
using System.Collections.Generic;
using System.Linq;
using Disaster_Response_Coordination.Interfaces;

namespace Disaster_Response_Coordination.Models
{
    public class SystemController
    {
        public IncidentManager IncidentManager { get; private set; }
        public TeamManager TeamManager { get; private set; }
        public ResourceManager ResourceManager { get; private set; }
        public DispatchManagement DispatchManager { get; private set; }
        public SystemLogger Logger { get; private set; }

        public SystemController()
        {
            IncidentManager = new IncidentManager();
            TeamManager = new TeamManager();
            ResourceManager = new ResourceManager();
            DispatchManager = new DispatchManagement();
            Logger = new SystemLogger();

            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            DispatchManager.DisasterAlertTrigger += OnDisasterAlert;
            DispatchManager.ResponseCompleted += OnResponseCompleted;
            DispatchManager.TeamWarning += OnTeamWarning;
        }

        private void OnDisasterAlert(object sender, Events.DisasterEventArgs e)
        {
            Logger.Log($"[ALERT] Disaster #{e.DisasterID}: {e.Message}");
        }

        private void OnResponseCompleted(object sender, Events.DisasterEventArgs e)
        {
            Logger.Log($"[COMPLETION] Dispatch #{e.DisasterID}: {e.Message}");
        }

        private void OnTeamWarning(object sender, string message)
        {
            Logger.Log($"[WARNING] {message}");
        }

        public void DisplaySystemStatus()
        {
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("DISASTER RESPONSE COORDINATION SYSTEM - STATUS REPORT");
            Console.WriteLine(new string('=', 60));

            Console.WriteLine($"\n[INCIDENTS] Total: {IncidentManager.GetIncidentCount()}");
            Console.WriteLine($"[TEAMS] Total: {TeamManager.GetTeamCount()}");
            Console.WriteLine($"[RESOURCES] Total: {ResourceManager.GetResourceCount()}");

            Console.WriteLine(new string('-', 60));
        }
    }
}