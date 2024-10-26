using Synchronizer.Helper;
using Synchronizer.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Synchronizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MSSQLHelper _serverHelper;
        private List<Customer> _customers;
        public MainWindow()
        {
            InitializeComponent();

            string connectionString = "Server=DESKTOP-7IDTFU3\\SQLEXPRESS;Database=FleetPanda;Trusted_Connection=True;";
            DatabaseSeeder seeder = new DatabaseSeeder(connectionString);

            // Seed data into the database
            seeder.SeedData();

            _serverHelper = new MSSQLHelper(connectionString);
        }
        private void FetchDataButton_Click(object sender, RoutedEventArgs e)
        {
            _customers = _serverHelper.FetchCustomers();
            DisplayData(_customers);
        }


        private void DisplayData(List<Customer> customers)
        {
            // Flatten customer data with associated locations for display
            var displayData = customers
                .SelectMany(c => c.Locations.DefaultIfEmpty(), (c, l) => new
                {
                    c.CustomerId,
                    c.Name,
                    c.Email,
                    c.Phone,
                    Address = l?.Address ?? "No Address"
                }).ToList();

            CustomerDataGrid.ItemsSource = displayData;
        }
    }
}
