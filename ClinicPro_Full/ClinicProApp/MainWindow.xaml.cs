using System.Windows;
using ClinicProApp.Services;

namespace ClinicProApp
{
    public partial class MainWindow : Window
    {
        public string ClientDisplayName { get; set; } = "ClinicPro";
        private readonly DatabaseService _db = new DatabaseService();

        public MainWindow()
        {
            InitializeComponent();
            var client = _db.ExecuteScalar("SELECT ClientName FROM LicenseInfo LIMIT 1");
            if (client != null) ClientDisplayName = client.ToString();
            DataContext = this;
            MainContent.Content = new Views.DashboardView();
        }

        private void ToggleTheme_Click(object s, RoutedEventArgs e) => MessageBox.Show("تبديل الثيم - أضف ResourceDictionary هنا.");
        private void MenuExit_Click(object s, RoutedEventArgs e) => Application.Current.Shutdown();
        private void ShowDashboard_Click(object s, RoutedEventArgs e) => MainContent.Content = new Views.DashboardView();
        private void ShowPatients_Click(object s, RoutedEventArgs e) => MainContent.Content = new Views.PatientsView();
        private void ShowAppointments_Click(object s, RoutedEventArgs e) => MainContent.Content = new Views.AppointmentsView();
        private void ShowBilling_Click(object s, RoutedEventArgs e) => MainContent.Content = new Views.BillingView();
        private void ShowInventory_Click(object s, RoutedEventArgs e) => MainContent.Content = new Views.InventoryView();
    }
}
