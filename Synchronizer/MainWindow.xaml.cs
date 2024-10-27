using Synchronizer.Helper;
using Synchronizer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Synchronizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MSSQLHelper _serverHelper;
        private List<Customer> _customers;
        private string _sqliteConnectionString;
        private DispatcherTimer _timer;
        private int _currentPage = 1;
        private int _pageSize = 15;
        public MainWindow()
        {
            InitializeComponent();

            // ensures e_sqlite3.dll is loaded properly.
            //SQLitePCL.Batteries.Init();


            string connectionString = "Server=DESKTOP-7IDTFU3\\SQLEXPRESS;Database=FleetPanda;Trusted_Connection=True;";
            DatabaseSeeder seeder = new DatabaseSeeder(connectionString);

             _sqliteConnectionString = "C:\\Users\\Acer\\Desktop\\a\\FleetPanda.db;";

            // Seed data into the database
            seeder.SeedData();

            _serverHelper = new MSSQLHelper(connectionString);

            // set timer
            _timer = new DispatcherTimer();
            _timer.Tick += Timer_Tick;
        }
        private async void FetchDataButton_Click(object sender, RoutedEventArgs e)
        {
            _customers = _serverHelper.FetchCustomers();

            var sqliteHelper = new SQLiteHelper($"Data Source={_sqliteConnectionString}");
            await sqliteHelper.InsertOrUpdateCustomersAsync(_customers);

            await sqliteHelper.LogSyncAsync("Synchronized data from MSSQL to SQLite at " + DateTime.Now.ToString("g"));

            DisplayData(_customers);
        }


        private void DisplayData(List<Customer> customers)
        {
            int snCounter = 1;
            // Flatten customer data with associated locations for display
            var displayData = customers
                .SelectMany(c => c.Locations.DefaultIfEmpty(), (c, l) => new
                {
                    SN = snCounter++,
                    //c.CustomerId,
                    c.Name,
                    c.Email,
                    c.Phone,
                    Address = l?.Address ?? "No Address"
                }).ToList();

            CustomerDataGrid.ItemsSource = displayData;
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_customers == null || !_customers.Any())
            {
                // No data fetched yet, so do nothing to display 
                return;
            }

            string searchText = Search.Text.ToLower();
            if (string.IsNullOrWhiteSpace(searchText))
            {
                // If the search box is empty, display the full list
                DisplayData(_customers);
            }
            else
            {
                var filteredCustomers = _customers.Where(c => c.Name.ToLower().Contains(searchText)).ToList();
                DisplayData(filteredCustomers);
            }
        }

        private void SetTimeButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(TimeIntervalTextBox.Text, out int interval))
            {
                _timer.Interval = TimeSpan.FromSeconds(interval);
                _timer.Start();
                MessageBox.Show("Automatic data fetch started with interval of " + interval + " seconds.");
            }
            else
            {
                MessageBox.Show("Please enter a valid interval in seconds.");
            }
        }
        
        private async void Timer_Tick(object sender, EventArgs e)
        {
            // This will be triggered automatically based on the time interval
            _customers = _serverHelper.FetchCustomers();
            DisplayData(_customers);
        }
    }
}
