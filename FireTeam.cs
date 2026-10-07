using System;

namespace Disaster_Response_Coordination.Models
{
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

        public override string GetTeamType()
        {
            return "Fire";
        }

        public override int CalculateResponsePriority(Disaster disaster)
        {
            // Fire teams prioritize fire disasters
            int priority = (int)disaster.Severity * 10;

            if (disaster is FireDisaster fire)
            {
                priority += 30; // Fire teams are most relevant for fires

                if (fire.IsNearResidential)
                    priority += 20; // Extra priority for residential fires
            }
            else if (disaster is FloodDisaster flood && flood.WaterDepthMeters > 3)
            {
                priority += 15; // Fire teams can assist in deep floods with rescue boats
            }

            return priority;
        }

        public override void DisplayDetails()
        {
            base.DisplayDetails();
            Console.WriteLine($"   -> Firefighters: {NumFirefighters}, Trucks: {NumTrucks}, Aerial Support: {HasAerialSupport}");
        }
    }
}