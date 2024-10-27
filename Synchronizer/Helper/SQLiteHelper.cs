using Microsoft.Data.Sqlite;
using Synchronizer.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Synchronizer.Helper
{
    public class SQLiteHelper
    {
        private readonly string _sqliteConnectionString;

        public SQLiteHelper(string sqliteConnectionString)
        {
            _sqliteConnectionString = sqliteConnectionString;
        }

        public async Task InsertOrUpdateCustomersAsync(IEnumerable<Customer> customers)
        {
            SQLitePCL.Batteries_V2.Init();

            using (var connection = new SqliteConnection(_sqliteConnectionString))
            {
                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {
                    foreach (var customer in customers)
                    {
                        await InsertOrUpdateCustomerAsync(connection, customer);
                    }
                    transaction.Commit();
                }
            }
        }

        private async Task InsertOrUpdateCustomerAsync(SqliteConnection connection, Customer customer)
        {
            var query = "SELECT COUNT(*) FROM Customer WHERE CustomerID = @CustomerID";
            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CustomerID", customer.CustomerId);
                int count = Convert.ToInt32(await command.ExecuteScalarAsync());

                if (count > 0)
                {
                    var updateQuery = "UPDATE Customer SET Name = @Name, Email = @Email, Phone = @Phone WHERE CustomerID = @CustomerID";
                    using (var updateCommand = new SqliteCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@CustomerID", customer.CustomerId);
                        updateCommand.Parameters.AddWithValue("@Name", customer.Name);
                        updateCommand.Parameters.AddWithValue("@Email", customer.Email);
                        updateCommand.Parameters.AddWithValue("@Phone", customer.Phone);
                        await updateCommand.ExecuteNonQueryAsync();
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
                        await insertCommand.ExecuteNonQueryAsync();
                    }
                }
            }

            foreach (var location in customer.Locations)
            {
                await InsertOrUpdateLocationAsync(connection, customer.CustomerId, location);
            }
        }

        private async Task InsertOrUpdateLocationAsync(SqliteConnection connection, int customerId, Location location)
        {
            var query = "SELECT COUNT(*) FROM Location WHERE CustomerID = @CustomerID AND Address = @Address";
            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CustomerID", customerId);
                command.Parameters.AddWithValue("@Address", location.Address);
                int count = Convert.ToInt32(await command.ExecuteScalarAsync());

                if (count > 0)
                {
                    var updateQuery = "UPDATE Location SET Address = @Address WHERE CustomerID = @CustomerID";
                    using (var updateCommand = new SqliteCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@CustomerID", customerId);
                        updateCommand.Parameters.AddWithValue("@Address", location.Address);
                        await updateCommand.ExecuteNonQueryAsync();
                    }
                }
                else
                {
                    var insertQuery = "INSERT INTO Location (CustomerID, Address) VALUES (@CustomerID, @Address)";
                    using (var insertCommand = new SqliteCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@CustomerID", customerId);
                        insertCommand.Parameters.AddWithValue("@Address", location.Address);
                        await insertCommand.ExecuteNonQueryAsync();
                    }
                }
            }
        }

        public async Task LogSyncAsync(string description)
        {
            using (var connection = new SqliteConnection(_sqliteConnectionString))
            {
                await connection.OpenAsync();

                var query = "INSERT INTO SyncLog (Description) VALUES (@Description)";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Description", description);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
