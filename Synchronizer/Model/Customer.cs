using System.Collections.Generic;

namespace Synchronizer.Model
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public List<Location> Locations { get; set; } = new List<Location>();
    }
}
