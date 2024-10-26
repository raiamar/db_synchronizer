using Synchronizer.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Synchronizer
{
    public class DatabaseSeeder
    {
        private readonly string connectionString;

        public DatabaseSeeder(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void SeedData()
        {
            // only seed data initially 
            if (IsTableEmpty("Customer"))
            {
                SeedCustomers();
                SeedLocations();
            }
            else
            {
                Console.WriteLine("Tables are already populated. No seeding required.");
            }
            
        }

        private bool IsTableEmpty(string tableName)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand($"SELECT COUNT(*) FROM dbo.{tableName}", connection);
                int count = (int)command.ExecuteScalar();
                return count == 0;
            }
        }

        private void SeedCustomers()
        {
            int rowsAffected = 0;

            var customers = new List<Customer>
            {
                new Customer {  Name = "Suman Shrestha", Email = "suman@fleetpanda.com", Phone = "9801234567" },
                new Customer { Name = "Ramesh Karki", Email = "ramesh@fleetpanda.com", Phone = "9812345678" },
                new Customer { Name = "Aasha Khadka", Email = "aasha@fleetpanda.com", Phone = "9823456789" },
                new Customer { Name = "Kiran Bista", Email = "kiran@fleetpanda.com", Phone = "9834567890" },
                new Customer { Name = "Nisha Rai", Email = "nisha@fleetpanda.com", Phone = "9845678901" },
                new Customer { Name = "Binod Tamang", Email = "binod@fleetpanda.com", Phone = "9856789012" },
                new Customer { Name = "Mina Gurung", Email = "mina@fleetpanda.com", Phone = "9867890123" },
                new Customer { Name = "Deepak Chaudhary", Email = "deepak@fleetpanda.com", Phone = "9878901234" },
                new Customer { Name = "Sita Pokharel", Email = "sita@fleetpanda.com", Phone = "9802345671" },
                new Customer { Name = "Prakash Thapa", Email = "prakash@fleetpanda.com", Phone = "9813456782" },
                new Customer { Name = "Sunita Shrestha", Email = "sunita@fleetpanda.com", Phone = "9824567893" },
                new Customer { Name = "Manoj Adhikari", Email = "manoj@fleetpanda.com", Phone = "9835678904" },
                new Customer { Name = "Rekha Lama", Email = "rekha@fleetpanda.com", Phone = "9846789015" },
                new Customer { Name = "Rajendra KC", Email = "rajendra@fleetpanda.com", Phone = "9857890126" },
                new Customer { Name = "Madhav Sharma", Email = "madhav@fleetpanda.com", Phone = "9868901237" },
                new Customer { Name = "Sarita Mahato", Email = "sarita@fleetpanda.com", Phone = "9879012348" },
                new Customer { Name = "Keshav Dhakal", Email = "keshav@fleetpanda.com", Phone = "9803456789" },
                new Customer { Name = "Puja Bhattarai", Email = "puja@fleetpanda.com", Phone = "9814567890" },
                new Customer { Name = "Ramita Pant", Email = "ramita@fleetpanda.com", Phone = "9825678901" },
                new Customer { Name = "Subash Rana", Email = "subash@fleetpanda.com", Phone = "9836789012" },
                new Customer { Name = "Laxmi Rijal", Email = "laxmi@fleetpanda.com", Phone = "9847890123" }
            };



            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                foreach(var customer in customers)
                {
                    var command = new SqlCommand("INSERT INTO dbo.Customer (Name, Email, Phone) VALUES (@Name, @Email, @Phone)", connection);
                    command.Parameters.AddWithValue("@Name", customer.Name);
                    command.Parameters.AddWithValue("@Email", customer.Email);
                    command.Parameters.AddWithValue("@Phone", customer.Phone);

                    rowsAffected = command.ExecuteNonQuery();
                }
            }
            Console.WriteLine($"{rowsAffected} Customer data seeded successfully.");
        }

        private void SeedLocations()
        {
            int rowsAffected = 0;

            var locations = new List<Location>
            {
                new Location { CustomerID = 1, Address = "Chabahil, Kathmandu" },
                new Location { CustomerID = 1, Address = "New Road, Kathmandu" },
                new Location { CustomerID = 2, Address = "Lakeside, Pokhara" },
                new Location { CustomerID = 3, Address = "Butwal, Rupandehi" },
                new Location { CustomerID = 3, Address = "Palpa, Tansen" },
                new Location { CustomerID = 4, Address = "Dharan, Sunsari" },
                new Location { CustomerID = 5, Address = "Damak, Jhapa" },
                new Location { CustomerID = 5, Address = "Birtamode, Jhapa" },
                new Location { CustomerID = 6, Address = "Hetauda, Makwanpur" },
                new Location { CustomerID = 7, Address = "Chitwan, Bharatpur" },
                new Location { CustomerID = 7, Address = "Narayangadh, Chitwan" },
                new Location { CustomerID = 7, Address = "Ratnanagar, Chitwan" },
                new Location { CustomerID = 8, Address = "Banepa, Kavre" },
                new Location { CustomerID = 9, Address = "Nepalgunj, Banke" },
                new Location { CustomerID = 9, Address = "Kohalpur, Banke" },
                new Location { CustomerID = 11, Address = "Biratnagar, Morang" },
                new Location { CustomerID = 11, Address = "Itahari, Sunsari" },
                new Location { CustomerID = 12, Address = "Bhaktapur, Suryabinayak" },
                new Location { CustomerID = 12, Address = "Bhaktapur, Kamalbinayak" },
                new Location { CustomerID = 14, Address = "Lamjung, Besisahar" },
                new Location { CustomerID = 15, Address = "Birgunj, Parsa" },
                new Location { CustomerID = 15, Address = "Kalaiya, Bara" },
                new Location { CustomerID = 16, Address = "Gongabu, Kathmandu" },
                new Location { CustomerID = 17, Address = "Chitwan, Ratnanagar" },
                new Location { CustomerID = 19, Address = "Dhangadhi, Kailali" },
                new Location { CustomerID = 20, Address = "Pokhara, Mahendrapool" },
                new Location { CustomerID = 20, Address = "Pokhara, Sarangkot" },
                new Location { CustomerID = 20, Address = "Pokhara, Hemja" }
            };

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                foreach (var customer in locations)
                {
                    var command = new SqlCommand("INSERT INTO dbo.Location (CustomerID, Address) VALUES (@CustomerID, @Address)", connection);
                    command.Parameters.AddWithValue("@CustomerID", customer.CustomerID);
                    command.Parameters.AddWithValue("@Address", customer.Address);

                    rowsAffected = command.ExecuteNonQuery();
                }
            }
            Console.WriteLine($"{rowsAffected} Location data seeded successfully.");
        }
    }
}
