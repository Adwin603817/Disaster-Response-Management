using System;

namespace Disaster_Response_Coordination.Models
{
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

        public override string GetTeamType()
        {
            return "Rescue";
        }

        public override int CalculateResponsePriority(Disaster disaster)
        {
            // Rescue teams prioritize based on severity and type of disaster
            int priority = (int)disaster.Severity * 10;

            if (disaster is FloodDisaster flood)
            {
                priority += 30; // Rescue teams are most relevant for floods

                if (flood.WaterDepthMeters > 2)
                    priority += 20; // Deep water requires more rescue

                if (HasWaterRescue)
                    priority += 15; // Bonus for specialized water rescue
            }
            else if (disaster is FireDisaster fire && fire.IsNearResidential)
            {
                priority += 15; // Rescue teams can assist with evacuation
            }

            return priority;
        }

        public override void DisplayDetails()
        {
            base.DisplayDetails();
            Console.WriteLine($"   -> Rescuers: {NumRescuers}, Water Rescue: {HasWaterRescue}, Technical Rescue: {HasTechnicalRescue}");
        }
    }
}