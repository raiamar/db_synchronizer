using System.Windows;
using System.Windows.Controls;

namespace Synchronizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            string connectionString = "Server=DESKTOP-7IDTFU3\\SQLEXPRESS;Database=FleetPanda;Trusted_Connection=True;";
            DatabaseSeeder seeder = new DatabaseSeeder(connectionString);

            // Seed data into the database
            seeder.SeedData();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
