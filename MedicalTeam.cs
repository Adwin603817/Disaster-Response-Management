using System;

namespace Disaster_Response_Coordination.Models
{
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

        public override string GetTeamType()
        {
            return "Medical";
        }

        public override int CalculateResponsePriority(Disaster disaster)
        {
            // Medical teams prioritize based on severity and if it's a fire (which may cause injuries)
            int priority = (int)disaster.Severity * 10;

            if (disaster is FireDisaster fire && fire.IsNearResidential)
                priority += 20; // More priority if people might be injured

            if (HasAmbulance)
                priority += 10; // Bonus for having transport

            return priority;
        }

        public override void DisplayDetails()
        {
            base.DisplayDetails();
            Console.WriteLine($"   -> Doctors: {NumDoctors}, Nurses: {NumNurses}, Ambulance: {HasAmbulance}");
        }
    }
}