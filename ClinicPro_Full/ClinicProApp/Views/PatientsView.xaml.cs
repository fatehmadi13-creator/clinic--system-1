using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using ClinicProApp.Services;

namespace ClinicProApp.Views
{
    public partial class PatientsView : UserControl
    {
        private readonly DatabaseService _db = new DatabaseService();

        public PatientsView()
        {
            InitializeComponent();
            LoadPatients();
        }

        private void LoadPatients(string q = null)
        {
            var list = new List<dynamic>();
            string sql = "SELECT Id, FileNumber, FirstName, LastName, Phone FROM Patients";
            if (!string.IsNullOrEmpty(q))
                sql += " WHERE FirstName LIKE @q OR LastName LIKE @q OR FileNumber LIKE @q";
            using var rdr = _db.ExecuteReader(sql, new SQLiteParameter("@q", $"%{q}%"));
            while (rdr.Read())
            {
                list.Add(new { Id = rdr.GetInt32(0), FileNumber = rdr.IsDBNull(1)?"":rdr.GetString(1), FullName = ((rdr.IsDBNull(2)?"":rdr.GetString(2)) + " " + (rdr.IsDBNull(3)?"":rdr.GetString(3))).Trim(), Phone = rdr.IsDBNull(4)?"":rdr.GetString(4) });
            }
            dgPatients.ItemsSource = list;
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            var win = new EditPatientWindow();
            win.Owner = Application.Current.MainWindow;
            if (win.ShowDialog() == true) LoadPatients();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e) => LoadPatients(txtSearch.Text.Trim());
    }
}
