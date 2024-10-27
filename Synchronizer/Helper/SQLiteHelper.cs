using Microsoft.Data.Sqlite;
using Synchronizer.Model;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Synchronizer.Helper
{
    public class SQLiteHelper
    {
        private readonly string _sqliteConnectionString;

        public SQLiteHelper(string sqliteConnectionString)
        {
            _sqliteConnectionString = sqliteConnectionString;
        }

        public void InsertOrUpdateCustomers(IEnumerable<Customer> customers)
        {
            const int maxRetryAttempts = 1; // retry attempts
            const int delayBetweenRetries = 2000; // Delay in milliseconds between retries
            int attempt = 0;

            while (attempt <= maxRetryAttempts)
            {
                using (var connection = new SqliteConnection(_sqliteConnectionString))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            foreach (var customer in customers)
                            {
                                InsertOrUpdateCustomer(connection, customer);
                            }
                            transaction.Commit();
                            break;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            LogSync($"Attempt {attempt + 1} - Transaction rolled back due to error: {ex.Message} at {DateTime.Now:g}");

                            if (attempt == maxRetryAttempts)
                            {
                                MessageBox.Show("Sync has failed.");
                                throw;
                            }

                            // Wait before retrying
                            System.Threading.Thread.Sleep(delayBetweenRetries);
                        }
                    }
                }

                attempt++;
            }
        }

        private void InsertOrUpdateCustomer(SqliteConnection connection, Customer customer)
        {
            var query = "SELECT COUNT(*) FROM Customer WHERE CustomerID = @CustomerID";
            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CustomerID", customer.CustomerId);
                int count = Convert.ToInt32(command.ExecuteScalar());

                if (count > 0)
                {
                    var updateQuery = "UPDATE Customer SET Name = @Name, Email = @Email, Phone = @Phone WHERE CustomerID = @CustomerID";
                    using (var updateCommand = new SqliteCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@CustomerID", customer.CustomerId);
                        updateCommand.Parameters.AddWithValue("@Name", customer.Name);
                        updateCommand.Parameters.AddWithValue("@Email", customer.Email);
                        updateCommand.Parameters.AddWithValue("@Phone", customer.Phone);
                        updateCommand.ExecuteNonQuery();
                    }
                }
                else
                {
                    var insertQuery = "INSERT INTO Customer (CustomerID, Name, Email, Phone) VALUES (@CustomerID, @Name, @Email, @Phone)";
                    using (var insertCommand = new SqliteCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@CustomerID", customer.CustomerId);
                        insertCommand.Parameters.AddWithValue("@Name", customer.Name);
                        insertCommand.Parameters.AddWithValue("@Email", customer.Email);
                        insertCommand.Parameters.AddWithValue("@Phone", customer.Phone);
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }

            foreach (var location in customer.Locations)
            {
                InsertOrUpdateLocation(connection, customer.CustomerId, location);
            }
        }

        private void InsertOrUpdateLocation(SqliteConnection connection, int customerId, Location location)
        {
            var query = "SELECT COUNT(*) FROM Location WHERE CustomerID = @CustomerID AND Address = @Address";
            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CustomerID", customerId);
                command.Parameters.AddWithValue("@Address", location.Address);
                int count = Convert.ToInt32(command.ExecuteScalar());

                if (count > 0)
                {
                    var updateQuery = "UPDATE Location SET Address = @Address WHERE CustomerID = @CustomerID";
                    using (var updateCommand = new SqliteCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@CustomerID", customerId);
                        updateCommand.Parameters.AddWithValue("@Address", location.Address);
                        updateCommand.ExecuteNonQuery();
                    }
                }
                else
                {
                    var insertQuery = "INSERT INTO Location (CustomerID, Address) VALUES (@CustomerID, @Address)";
                    using (var insertCommand = new SqliteCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@CustomerID", customerId);
                        insertCommand.Parameters.AddWithValue("@Address", location.Address);
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }

        public void LogSync(string description)
        {
            using (var connection = new SqliteConnection(_sqliteConnectionString))
            {
                connection.Close();
                connection.Open();

                var query = "INSERT INTO SyncLog (Description) VALUES (@Description)";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Description", description);
                    command.ExecuteNonQuery ();
                }
            }
        }
    }
}
