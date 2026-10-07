using System;
using System.Collections.Generic;
using System.Linq;

namespace Disaster_Response_Coordination.Models
{
    public class ResourceManager
    {
        private List<Resource> _resources;
        private List<Vehicle> _vehicles;
        private Random _random;

        public ResourceManager()
        {
            _resources = new List<Resource>();
            _vehicles = new List<Vehicle>();
            _random = new Random();
        }

        // Resource Management
        public void AddResource(Resource resource)
        {
            _resources.Add(resource);
            Console.WriteLine($"[SUCCESS] Resource {resource.ID} added successfully.");
        }

        public Resource GetResource(string resourceID)
        {
            return _resources.FirstOrDefault(r => r.ID == resourceID);
        }

        public List<Resource> GetAllResources()
        {
            return new List<Resource>(_resources);
        }

        public List<Resource> GetAvailableResources()
        {
            return _resources.Where(r => r.IsAvailable).ToList();
        }

        public void RemoveResource(string resourceID)
        {
            var resource = GetResource(resourceID);
            if (resource != null)
            {
                _resources.Remove(resource);
                Console.WriteLine($"[SUCCESS] Resource {resourceID} removed successfully.");
            }
            else
            {
                Console.WriteLine($"[ERROR] Resource {resourceID} not found.");
            }
        }

        public void ViewAllResources()
        {
            if (!_resources.Any())
            {
                Console.WriteLine("No resources to display.");
                return;
            }

            Console.WriteLine("\nAll Resources:");
            foreach (var resource in _resources)
            {
                Console.WriteLine(resource.GetStatusReport());
            }
        }

        // Vehicle Management
        public void AddVehicle(Vehicle vehicle)
        {
            _vehicles.Add(vehicle);
            Console.WriteLine($"[SUCCESS] Vehicle {vehicle.ID} added successfully.");
        }

        public Vehicle GetVehicle(string vehicleID)
        {
            return _vehicles.FirstOrDefault(v => v.ID == vehicleID);
        }

        public List<Vehicle> GetAllVehicles()
        {
            return new List<Vehicle>(_vehicles);
        }

        public List<Vehicle> GetAvailableVehicles()
        {
            return _vehicles.Where(v => v.IsAvailable).ToList();
        }

        public void RemoveVehicle(string vehicleID)
        {
            var vehicle = GetVehicle(vehicleID);
            if (vehicle != null)
            {
                _vehicles.Remove(vehicle);
                Console.WriteLine($"[SUCCESS] Vehicle {vehicleID} removed successfully.");
            }
            else
            {
                Console.WriteLine($"[ERROR] Vehicle {vehicleID} not found.");
            }
        }

        public void ViewAllVehicles()
        {
            if (!_vehicles.Any())
            {
                Console.WriteLine("No vehicles to display.");
                return;
            }

            Console.WriteLine("\nAll Vehicles:");
            foreach (var vehicle in _vehicles)
            {
                Console.WriteLine(vehicle.GetStatusReport());
            }
        }

        public void GenerateDefaultResources()
        {
            var resources = new List<Resource>
            {
                new Resource("First Aid Kits", "Medical", 50),
                new Resource("Oxygen Tanks", "Medical", 30),
                new Resource("Stretchers", "Medical", 20),
                new Resource("Fire Extinguishers", "Fire", 100),
                new Resource("Rescue Ropes", "Rescue", 40),
                new Resource("Life Jackets", "Rescue", 60)
            };

            foreach (var resource in resources)
            {
                AddResource(resource);
            }

            var vehicles = new List<Vehicle>
            {
                new Vehicle("Ambulance 1", "Medical", 4),
                new Vehicle("Ambulance 2", "Medical", 4),
                new Vehicle("Fire Truck 1", "Fire", 10),
                new Vehicle("Fire Truck 2", "Fire", 10),
                new Vehicle("Rescue Vehicle 1", "Rescue", 8),
                new Vehicle("Rescue Vehicle 2", "Rescue", 8)
            };

            foreach (var vehicle in vehicles)
            {
                AddVehicle(vehicle);
            }

            Console.WriteLine($"Generated {resources.Count} default resources and {vehicles.Count} default vehicles.");
        }

        public int GetResourceCount()
        {
            return _resources.Count + _vehicles.Count;
        }
    }
}