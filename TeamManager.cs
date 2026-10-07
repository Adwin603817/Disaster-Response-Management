using System;
using System.Collections.Generic;
using System.Linq;

namespace Disaster_Response_Coordination.Models
{
    public class TeamManager
    {
        private List<ResponseTeam> _teams;
        private Random _random;

        public TeamManager()
        {
            _teams = new List<ResponseTeam>();
            _random = new Random();
        }

        public void AddTeam(ResponseTeam team)
        {
            _teams.Add(team);
            Console.WriteLine($"[SUCCESS] Team {team.TeamID} added successfully.");
        }

        public ResponseTeam GetTeam(string teamID)
        {
            return _teams.FirstOrDefault(t => t.TeamID == teamID);
        }

        public List<ResponseTeam> GetAllTeams()
        {
            return new List<ResponseTeam>(_teams);
        }

        public List<ResponseTeam> GetAvailableTeams()
        {
            return _teams.Where(t => t.IsAvailable).ToList();
        }

        public void RemoveTeam(string teamID)
        {
            var team = GetTeam(teamID);
            if (team != null)
            {
                _teams.Remove(team);
                Console.WriteLine($"[SUCCESS] Team {teamID} removed successfully.");
            }
            else
            {
                Console.WriteLine($"[ERROR] Team {teamID} not found.");
            }
        }

        public void UpdateTeamStatus(string teamID, string newStatus)
        {
            var team = GetTeam(teamID);
            if (team != null)
            {
                team.UpdateStatus(newStatus);
                Console.WriteLine($"[SUCCESS] Team {teamID} status updated to {newStatus}.");
            }
            else
            {
                Console.WriteLine($"[ERROR] Team {teamID} not found.");
            }
        }

        public void ViewAllTeams()
        {
            if (!_teams.Any())
            {
                Console.WriteLine("No teams to display.");
                return;
            }

            Console.WriteLine("\nAll Response Teams:");
            foreach (var team in _teams)
            {
                team.DisplayDetails();
                Console.WriteLine(new string('-', 50));
            }
        }

        public void GenerateRandomTeams()
        {
            var teams = new List<ResponseTeam>
            {
                new MedicalTeam("Emergency Medical Team Alpha", 15, 2, 8, true),
                new MedicalTeam("Emergency Medical Team Beta", 12, 1, 6, true),
                new FireTeam("Fire Station 1", 25, 20, 4, true),
                new FireTeam("Fire Station 2", 20, 15, 3, false),
                new RescueTeam("Urban Rescue Unit", 18, 16, true, true),
                new RescueTeam("Water Rescue Team", 12, 10, true, false)
            };

            foreach (var team in teams)
            {
                AddTeam(team);
            }

            Console.WriteLine($"Generated {teams.Count} default response teams.");
        }

        public int GetTeamCount()
        {
            return _teams.Count;
        }

        public List<ResponseTeam> GetBestTeamsForDisaster(Disaster disaster)
        {
            return _teams.Where(t => t.IsAvailable)
                        .OrderByDescending(t => t.CalculateResponsePriority(disaster))
                        .ToList();
        }
    }
}