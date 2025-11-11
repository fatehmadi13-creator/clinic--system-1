using System;
using System.Windows;
using ClinicProApp.Services;
using System.Data.SQLite;

namespace ClinicProApp.Views
{
    public partial class ActivationWindow : Window
    {
        private readonly LicenseService _lic = new LicenseService();
        private readonly DatabaseService _db = new DatabaseService();

        public ActivationWindow()
        {
            InitializeComponent();
        }

        private void BtnActivate_Click(object sender, RoutedEventArgs e)
        {
            var client = txtClient.Text.Trim();
            var key = txtLicense.Text.Trim();
            if (string.IsNullOrEmpty(client) || string.IsNullOrEmpty(key))
            {
                MessageBox.Show("املأ الحقلين");
                return;
            }

            if (_lic.ValidateLicense(key, client, out string msg))
            {
                _db.ExecuteNonQuery("INSERT OR REPLACE INTO LicenseInfo (Id, LicenseKey, ActivatedOn, ClientName) VALUES (1, @k, @d, @c)",
                    new SQLiteParameter("@k", key),
                    new SQLiteParameter("@d", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                    new SQLiteParameter("@c", client));
                MessageBox.Show("تم التفعيل بنجاح");
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("فشل التفعيل: " + msg);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}
