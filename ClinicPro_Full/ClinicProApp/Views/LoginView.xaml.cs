using System.Windows;
using ClinicProApp.Services;
using System.Data.SQLite;

namespace ClinicProApp.Views
{
    public partial class LoginView : Window
    {
        private readonly DatabaseService _db = new DatabaseService();

        public LoginView()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var user = txtUsername.Text.Trim();
            var pass = txtPassword.Password;
            using var rdr = _db.ExecuteReader("SELECT PasswordHash, Role FROM Users WHERE Username=@u", new SQLiteParameter("@u", user));
            if (rdr.Read())
            {
                var hash = rdr.GetString(0);
                if (BCrypt.Net.BCrypt.Verify(pass, hash))
                {
                    var mw = new MainWindow();
                    mw.Show();
                    this.Close();
                    return;
                }
            }
            MessageBox.Show("اسم المستخدم أو كلمة المرور خاطئة");
        }

        private void BtnActivate_Click(object sender, RoutedEventArgs e)
        {
            var win = new ActivationWindow();
            win.Owner = this;
            win.ShowDialog();
        }
    }
}
