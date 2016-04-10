using System;
using System.Data;
using System.Data.OleDb;
using System.Windows;
using System.Collections.Generic;

namespace StudentManagement
{
    class Database
    {
        private static string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=database.accdb;";
        private OleDbConnection databaseConnection = new OleDbConnection(connectionString);

        public enum Permissions : byte //enumerators for readability
        {
            Admin,
            User,
            AlphaAdmin,
        }

        #region Connection

        public void OpenConnection()
        {
            try
            {
                databaseConnection.Open();
            }
            catch (OleDbException)
            {
                MessageBox.Show("Error: Accessing database"); //debugging only
            }
        }
        public void CloseConnection()
        {
            try
            {
                databaseConnection.Close();
            }
            catch (OleDbException)
            {
                MessageBox.Show("Error: Closing database connection"); //debugging only
                Environment.Exit(0);
            }
        }
        public bool Status()
        {
            if (databaseConnection != null && databaseConnection.State == ConnectionState.Open)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Users
        public bool GetLoginData(string username, string password)
        {
            OpenConnection();
            using (OleDbCommand getLogin = new OleDbCommand("SELECT * FROM Users WHERE Username = @username AND Password = @password", databaseConnection))
            {
                getLogin.Parameters.AddWithValue("@username", username);
                getLogin.Parameters.AddWithValue("@password", password);
                using (OleDbDataReader reader = getLogin.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    return reader.HasRows;
                }
            }

        }
        public void CreateNewUser(string username, string password, byte accessRights)
        {
            OpenConnection();
            if (UserExists(username))
            {
                MessageBox.Show("Username already exists. Please choose another one.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            else
            {
                using (OleDbCommand createUser = new OleDbCommand("INSERT INTO Users (Username, [Password], AccessRights) VALUES (@username, @password, @accessRights)", databaseConnection))
                {
                    createUser.Parameters.AddWithValue("@username", username);
                    createUser.Parameters.AddWithValue("@password", password);
                    createUser.Parameters.AddWithValue("@accessRights", accessRights);
                    createUser.ExecuteNonQuery();
                }
            }
            CloseConnection();
        }
        public byte GetAccessRights(string username)
        {
            OpenConnection();
            OleDbCommand accessRights = new OleDbCommand("SELECT AccessRights FROM Users WHERE Username = @username", databaseConnection);
            accessRights.Parameters.AddWithValue("@username", username);
            object value = accessRights.ExecuteScalar();
            if (value != null)
            {
                CloseConnection();
                return Convert.ToByte(value);
            }
            else
            {
                throw new InvalidOperationException("This will never happen.");
            }
        }
        public void ResetPassword(string username, string password)
        {
            OpenConnection();
            if (UserExists(username))
            {
                using (OleDbCommand reset = new OleDbCommand("UPDATE Users SET Password = @password WHERE Username = @username", databaseConnection))
                {
                    reset.Parameters.AddWithValue("@username", username);
                    reset.Parameters.AddWithValue("@password", password);
                    reset.ExecuteNonQuery();
                }
            }
            else
            {
                MessageBox.Show("User does not exist.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            CloseConnection();
        }
        public void UpdateAccess(string username, byte accessRights)
        {
            OpenConnection();
            if (UserExists(username))
            {
                using (OleDbCommand update = new OleDbCommand("UPDATE Users SET AccessRights = @accessRights WHERE Username = @username", databaseConnection))
                {
                    update.Parameters.AddWithValue("@username", username);
                    update.Parameters.AddWithValue("@accessRights", accessRights);
                    update.ExecuteNonQuery();
                }
            }
            else
            {
                MessageBox.Show("User does not exist.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            }
            CloseConnection();
        }
        public void RemoveUser(string username)
        {
            OpenConnection();
            if (UserExists(username))
            {
                using (OleDbCommand remove = new OleDbCommand("DELETE FROM Users WHERE Username = @username", databaseConnection))
                {
                    remove.Parameters.AddWithValue("@username", username);
                    remove.ExecuteNonQuery();
                }
            }
            else
            {
                MessageBox.Show("User does not exist.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            CloseConnection();
        }
        public void UpdatePassword(string username, string password)
        {
            OpenConnection();
            if (UserExists(username))
            {
                try
                {
                    using (OleDbCommand update = new OleDbCommand("UPDATE Users SET Password = @password WHERE Username = @username", databaseConnection))
                    {
                        update.Parameters.AddWithValue("@username", username);
                        update.Parameters.AddWithValue("@password", password);
                        update.ExecuteNonQuery();
                    }
                }
                finally
                {
                    MessageBox.Show("Password has been changed.",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("User does not exist.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
            }
            CloseConnection();
        }
        public bool UserExists(string username)
        {
            OleDbCommand exists = new OleDbCommand("SELECT * FROM Users WHERE Username = @username", databaseConnection);
            exists.Parameters.AddWithValue("@username", username);

            using (OleDbDataReader reader = exists.ExecuteReader())
            {
                return reader.HasRows;
            }
        }

        #endregion

        #region Students
        public void RemoveStudent(int id)
        {
            OpenConnection();
            using (OleDbCommand remove = new OleDbCommand("DELETE FROM Students WHERE ID = @id", databaseConnection))
            {
                remove.Parameters.AddWithValue("@id", id);
                remove.ExecuteNonQuery();
            }
            CloseConnection();
        }
        public List<Student> GetStudent(string name)
        {
            List<Student> students = new List<Student>();
            OpenConnection();
            using (OleDbCommand getStudents = new OleDbCommand("SELECT ID, Name, Form, DateOfBirth FROM Students WHERE Name = @name ORDER BY ID ASC", databaseConnection))
            {
                getStudents.Parameters.AddWithValue("@name", name);
                using (OleDbDataReader reader = getStudents.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (reader.Read())
                    {
                        var oStudent = new Student
                        {
                            ID = Convert.ToInt32(reader[0]),
                            Name = reader[1].ToString(),
                            Form = reader[2].ToString(),
                            DateOfBirth = reader[3].ToString()
                        };
                        students.Add(oStudent);
                    }
                    return students;
                }
            }
        }
        public Student GetStudentByID(int id)
        {
            OpenConnection();
            using (OleDbCommand getStudent = new OleDbCommand("SELECT * FROM Students WHERE [ID] = @id", databaseConnection))
            {
                getStudent.Parameters.AddWithValue("@id", id);
                using (OleDbDataReader reader = getStudent.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (reader.Read())
                    {
                        return new Student
                        {
                            ID = Convert.ToInt32(reader[0].ToString()),
                            Name = reader[1].ToString(),
                            Form = reader[2].ToString(),
                            DateOfBirth = reader[3].ToString(),
                            emergencyName = reader[4].ToString(),
                            emergencyNumber = reader[5].ToString()
                        };
                    }
                    else
                    {
                        throw new InvalidOperationException("The selected user was not found. Please make sure you have all other instances of this program closed.");
                    }
                }

            }
        }
        #endregion
    }
    internal class Student
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Form { get; set; }
        public string DateOfBirth { get; set; }
        public string emergencyName { get; set; }
        public string emergencyNumber { get; set; }
    }
}

