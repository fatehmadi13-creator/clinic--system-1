using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using ClinicProApp.Services;

namespace ClinicProApp.Views
{
    public partial class AppointmentsView : UserControl
    {
        private readonly DatabaseService _db = new DatabaseService();

        public AppointmentsView()
        {
            InitializeComponent();
            LoadAppointments();
        }

        private void LoadAppointments(string q = null)
        {
            var list = new List<dynamic>();
            string sql = @"SELECT a.Id, a.DateTime, a.Doctor, a.Status, p.FirstName, p.LastName
                           FROM Appointments a
                           LEFT JOIN Patients p ON a.PatientId = p.Id
                           ORDER BY a.DateTime DESC";
            if (!string.IsNullOrEmpty(q))
                sql = @"SELECT a.Id, a.DateTime, a.Doctor, a.Status, p.FirstName, p.LastName
                        FROM Appointments a
                        LEFT JOIN Patients p ON a.PatientId = p.Id
                        WHERE p.FirstName LIKE @q OR p.LastName LIKE @q OR a.Doctor LIKE @q
                        ORDER BY a.DateTime DESC";

            using var rdr = _db.ExecuteReader(sql, new SQLiteParameter("@q", $"%{q}%"));
            while (rdr.Read())
            {
                var dt = rdr.IsDBNull(1) ? "" : rdr.GetString(1);
                var doc = rdr.IsDBNull(2) ? "" : rdr.GetString(2);
                var status = rdr.IsDBNull(3) ? "" : rdr.GetString(3);
                var fname = rdr.IsDBNull(4) ? "" : rdr.GetString(4);
                var lname = rdr.IsDBNull(5) ? "" : rdr.GetString(5);
                list.Add(new { Id = rdr.GetInt32(0), DateTime = dt, PatientName = (fname + " " + lname).Trim(), Doctor = doc, Status = status });
            }
            dgAppointments.ItemsSource = list;
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            var win = new NewAppointmentWindow();
            win.Owner = Application.Current.MainWindow;
            if (win.ShowDialog() == true) LoadAppointments();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e) => LoadAppointments(txtSearch.Text.Trim());
        private void BtnRefresh_Click(object sender, RoutedEventArgs e) => LoadAppointments();
    }
}
