using System;
using System.Data.SQLite;
using System.IO;

namespace ClinicProApp.Services
{
    public class DatabaseService
    {
        private readonly string _dbPath;

        public DatabaseService(string dbPath = null)
        {
            _dbPath = dbPath ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ClinicPro", "clinicpro.db");
            Directory.CreateDirectory(Path.GetDirectoryName(_dbPath));
            if (!File.Exists(_dbPath))
            {
                SQLiteConnection.CreateFile(_dbPath);
                InitializeSchema();
            }
        }

        private SQLiteConnection GetConnection()
        {
            return new SQLiteConnection($"Data Source={_dbPath};Version=3;");
        }

        private void InitializeSchema()
        {
            using var conn = GetConnection();
            conn.Open();
            var schemaPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "schema.sql");
            string schema = File.Exists(schemaPath) ? File.ReadAllText(schemaPath) : GetEmbeddedSchema();
            using var cmd = new SQLiteCommand(schema, conn);
            cmd.ExecuteNonQuery();

            using var adminCmd = new SQLiteCommand("INSERT OR IGNORE INTO Users (Id,Username,PasswordHash,Role) VALUES (1,'admin',@pwd,'Admin')", conn);
            adminCmd.Parameters.AddWithValue("@pwd", BCrypt.Net.BCrypt.HashPassword("admin123"));
            adminCmd.ExecuteNonQuery();
        }

        private string GetEmbeddedSchema()
        {
            return "";
        }

        public SQLiteDataReader ExecuteReader(string sql, params SQLiteParameter[] parameters)
        {
            var conn = GetConnection();
            conn.Open();
            var cmd = new SQLiteCommand(sql, conn);
            if (parameters != null) cmd.Parameters.AddRange(parameters);
            return cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        }

        public int ExecuteNonQuery(string sql, params SQLiteParameter[] parameters)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand(sql, conn);
            if (parameters != null) cmd.Parameters.AddRange(parameters);
            return cmd.ExecuteNonQuery();
        }

        public object ExecuteScalar(string sql, params SQLiteParameter[] parameters)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand(sql, conn);
            if (parameters != null) cmd.Parameters.AddRange(parameters);
            return cmd.ExecuteScalar();
        }
    }
}
