using System;
using System.Data.SQLite;
using System.Windows;
using ClinicProApp.Services;

namespace ClinicProApp.Views
{
    public partial class EditPatientWindow : Window
    {
        private readonly DatabaseService _db = new DatabaseService();
        private int? _patientId = null;

        public EditPatientWindow(int? patientId = null)
        {
            InitializeComponent();
            _patientId = patientId;
            if (_patientId.HasValue) LoadPatient(_patientId.Value);
            else GenerateFileNumber();
        }

        private void GenerateFileNumber()
        {
            var seed = DateTime.Now.ToString("yyyyMMddHHmmss");
            txtFileNumber.Text = "P" + seed;
        }

        private void LoadPatient(int id)
        {
            using var rdr = _db.ExecuteReader("SELECT FileNumber, FirstName, LastName, DOB, Phone, Address, Notes FROM Patients WHERE Id=@id", new SQLiteParameter("@id", id));
            if (rdr.Read())
            {
                txtFileNumber.Text = rdr.IsDBNull(0) ? "" : rdr.GetString(0);
                txtFirstName.Text = rdr.IsDBNull(1) ? "" : rdr.GetString(1);
                txtLastName.Text = rdr.IsDBNull(2) ? "" : rdr.GetString(2);
                if (!rdr.IsDBNull(3) && DateTime.TryParse(rdr.GetString(3), out DateTime dob)) dpDOB.SelectedDate = dob;
                txtPhone.Text = rdr.IsDBNull(4) ? "" : rdr.GetString(4);
                txtAddress.Text = rdr.IsDBNull(5) ? "" : rdr.GetString(5);
                txtNotes.Text = rdr.IsDBNull(6) ? "" : rdr.GetString(6);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var fileNo = txtFileNumber.Text.Trim();
            var fname = txtFirstName.Text.Trim();
            var lname = txtLastName.Text.Trim();
            var dob = dpDOB.SelectedDate?.ToString("yyyy-MM-dd") ?? "";
            var phone = txtPhone.Text.Trim();
            var address = txtAddress.Text.Trim();
            var notes = txtNotes.Text.Trim();

            if (string.IsNullOrEmpty(fname) || string.IsNullOrEmpty(lname))
            {
                MessageBox.Show("الرجاء إدخال الاسم واللقب");
                return;
            }

            if (_patientId.HasValue)
            {
                _db.ExecuteNonQuery(@"UPDATE Patients SET FileNumber=@file, FirstName=@fn, LastName=@ln, DOB=@dob, Phone=@phone, Address=@addr, Notes=@notes WHERE Id=@id",
                    new SQLiteParameter("@file", fileNo),
                    new SQLiteParameter("@fn", fname),
                    new SQLiteParameter("@ln", lname),
                    new SQLiteParameter("@dob", dob),
                    new SQLiteParameter("@phone", phone),
                    new SQLiteParameter("@addr", address),
                    new SQLiteParameter("@notes", notes),
                    new SQLiteParameter("@id", _patientId.Value));
            }
            else
            {
                _db.ExecuteNonQuery(@"INSERT INTO Patients (FileNumber, FirstName, LastName, DOB, Phone, Address, Notes) VALUES (@file,@fn,@ln,@dob,@phone,@addr,@notes)",
                    new SQLiteParameter("@file", fileNo),
                    new SQLiteParameter("@fn", fname),
                    new SQLiteParameter("@ln", lname),
                    new SQLiteParameter("@dob", dob),
                    new SQLiteParameter("@phone", phone),
                    new SQLiteParameter("@addr", address),
                    new SQLiteParameter("@notes", notes));
            }

            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}
