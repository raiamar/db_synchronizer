using Synchronizer.Model;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Synchronizer.Helper
{
    public class MSSQLHelper
    {
        private readonly string _connectionString;
        public MSSQLHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Fetch customer from MSSQL Server
        public List<Customer> FetchCustomers()
        {
            var customers = new List<Customer>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Query to join Customer and Location tables
                string query = @"SELECT c.CustomerID, c.Name, c.Email, c.Phone, l.Address
                             FROM Customer c
                             LEFT JOIN Location l ON c.CustomerID = l.CustomerID";

                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    // Dictionary to handle customer and their locations
                    var customerDict = new Dictionary<int, Customer>();

                    while (reader.Read())
                    {
                        int customerId = reader.GetInt32(0);

                        if (!customerDict.TryGetValue(customerId, out Customer customer))
                        {
                            customer = new Customer
                            {
                                CustomerId = customerId,
                                Name = reader.GetString(1),
                                Email = reader.GetString(2),
                                Phone = reader.GetString(3),
                            };
                            customerDict[customerId] = customer;
                        }

                        if (!reader.IsDBNull(4)) // Check if location exists
                        {
                            var location = new Location
                            {
                                Address = reader.GetString(4)
                            };
                            customer.Locations.Add(location);
                        }
                    }

                    customers = new List<Customer>(customerDict.Values);
                }
            }

            return customers;
        }
    }
}
