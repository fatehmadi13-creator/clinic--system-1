using System;
using System.Data.SQLite;
using System.Windows;
using ClinicProApp.Services;
using System.Collections.Generic;

namespace ClinicProApp.Views
{
    public partial class NewAppointmentWindow : Window
    {
        private readonly DatabaseService _db = new DatabaseService();

        public NewAppointmentWindow()
        {
            InitializeComponent();
            LoadPatients();
            dpDate.SelectedDate = DateTime.Today;
            txtTime.Text = DateTime.Now.ToString("HH:mm");
        }

        private void LoadPatients()
        {
            var list = new List<dynamic>();
            using var rdr = _db.ExecuteReader("SELECT Id, FirstName, LastName FROM Patients ORDER BY FirstName");
            while (rdr.Read())
            {
                var id = rdr.GetInt32(0);
                var full = (rdr.IsDBNull(1) ? "" : rdr.GetString(1)) + " " + (rdr.IsDBNull(2) ? "" : rdr.GetString(2));
                list.Add(new { Id = id, Display = full.Trim() });
            }
            cbPatients.ItemsSource = list;
            if (list.Count>0) cbPatients.SelectedIndex = 0;
        }

        private void BtnBook_Click(object sender, RoutedEventArgs e)
        {
            if (cbPatients.SelectedValue == null) { MessageBox.Show("اختر مريض"); return; }
            if (!dpDate.SelectedDate.HasValue) { MessageBox.Show("اختر تاريخ"); return; }
            var time = txtTime.Text.Trim();
            if (string.IsNullOrEmpty(time)) { MessageBox.Show("ادخل وقت"); return; }

            var dtStr = dpDate.SelectedDate.Value.ToString("yyyy-MM-dd") + " " + time;
            var patientId = (int)cbPatients.SelectedValue;
            var doctor = txtDoctor.Text.Trim();
            var notes = txtNotes.Text.Trim();

            _db.ExecuteNonQuery("INSERT INTO Appointments (PatientId, Doctor, DateTime, Status, Notes) VALUES (@pid,@doc,@dt,'مجدول',@notes)",
                new SQLiteParameter("@pid", patientId),
                new SQLiteParameter("@doc", doctor),
                new SQLiteParameter("@dt", dtStr),
                new SQLiteParameter("@notes", notes));

            MessageBox.Show("تم حجز الموعد");
            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}
