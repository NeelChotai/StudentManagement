using System;
using System.Data;
using System.Data.OleDb;
using System.Windows;
using System.Collections.Generic;

namespace StudentManagement
{
    class Database
    {
        private const string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=database.accdb;"; //information about the database connection
        private OleDbConnection databaseConnection = new OleDbConnection(connectionString); //private database connection for use within the database file

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
                databaseConnection.Open(); //attempts to open a connection
            }
            catch (OleDbException)
            {
                MessageBox.Show("Error: Accessing database", 
                    "Error", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error); //catches any errors with database 
            }
        }
        public void CloseConnection()
        {
            try
            {
                databaseConnection.Close(); //attempts to close any open database connections
            }
            catch (OleDbException)
            {
                MessageBox.Show("Error: Closing database connection", 
                    "Error", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error); //catches any errors with the database
                Environment.Exit(0); //gracefully closes the program with a return code 0
            }
        }
        public bool Status() //gets the current connection status
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
            using (OleDbCommand getLogin = new OleDbCommand("SELECT * FROM Users WHERE Username = @username AND Password = @password", databaseConnection)) //queries the database
            {
                getLogin.Parameters.AddWithValue("@username", username); //paramaters added to the query
                getLogin.Parameters.AddWithValue("@password", password);
                using (OleDbDataReader reader = getLogin.ExecuteReader(CommandBehavior.CloseConnection)) //executes a new database reader and closes connection when action has been completed
                {
                    return reader.HasRows; //returns whether or not the username and password combo was correct
                }
            }

        }
        public void CreateNewUser(string username, string password, byte accessRights)
        {
            OpenConnection();
            if (UserExists(username)) //checks if username exists already
            {
                MessageBox.Show("Username already exists. Please choose another one.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error); //returns error if they do
            }
            else
            {
                using (OleDbCommand createUser = new OleDbCommand("INSERT INTO Users (Username, [Password], AccessRights) VALUES (@username, @password, @accessRights)", databaseConnection)) //creates new database entry with the details of the user entered
                {
                    createUser.Parameters.AddWithValue("@username", username); //adds the parameters
                    createUser.Parameters.AddWithValue("@password", password);
                    createUser.Parameters.AddWithValue("@accessRights", accessRights);
                    createUser.ExecuteNonQuery(); //executes the query
                }
            }
            CloseConnection();
        }
        public byte GetAccessRights(string username)
        {
            OpenConnection();
            OleDbCommand accessRights = new OleDbCommand("SELECT AccessRights FROM Users WHERE Username = @username", databaseConnection); //query to get access rights from a user
            accessRights.Parameters.AddWithValue("@username", username); //adds parameters
            object value = accessRights.ExecuteScalar(); //executes the query and saves the result to an object
            if (value != null) //checks if something was returned, which always will be the case, 
            {
                CloseConnection();
                return Convert.ToByte(value); //returns the object converted to a byte
            }
            else
            {
                throw new InvalidOperationException("This will never happen."); //this will never happen due to the fact that the idea is verified and never changed by the user
            }
        }
        public void ResetPassword(string username, string password)
        {
            OpenConnection();
            if (UserExists(username)) //checks if user exists
            {
                using (OleDbCommand reset = new OleDbCommand("UPDATE Users SET Password = @password WHERE Username = @username", databaseConnection)) //password reset query
                {
                    reset.Parameters.AddWithValue("@username", username); //adds the parameters
                    reset.Parameters.AddWithValue("@password", password);
                    reset.ExecuteNonQuery(); //executes the query
                }
            }
            else
            {
                MessageBox.Show("User does not exist.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error); //should the user not exist, displays an error and allows the user to retry
            }
            CloseConnection();
        }
        public void UpdateAccess(string username, byte accessRights) //updates a user's access rights
        {
            OpenConnection();
            if (UserExists(username)) //checks if user exists
            {
                using (OleDbCommand update = new OleDbCommand("UPDATE Users SET AccessRights = @accessRights WHERE Username = @username", databaseConnection)) //database update query
                {
                    update.Parameters.AddWithValue("@username", username); //adds the parameters
                    update.Parameters.AddWithValue("@accessRights", accessRights);
                    update.ExecuteNonQuery(); //executes the query
                }
            }
            else
            {
                MessageBox.Show("User does not exist.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            }
            CloseConnection(); //should the user not exist, displays an error and allows the user to retry
        }
        public void RemoveUser(string username)
        {
            OpenConnection();
            if (UserExists(username)) //checks if the user exists
            {
                using (OleDbCommand remove = new OleDbCommand("DELETE FROM Users WHERE Username = @username", databaseConnection)) //database deletion query
                {
                    remove.Parameters.AddWithValue("@username", username); //adds the parameters
                    remove.ExecuteNonQuery(); //executes the query
                }
            }
            else
            {
                MessageBox.Show("User does not exist.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error); //should the user not exist, displays an error and allows the user to retry
            }
            CloseConnection();
        }
        public void UpdatePassword(string username, string password)
        {
            OpenConnection();
            if (UserExists(username)) //checks if the user exists
            {
                try
                {
                    using (OleDbCommand update = new OleDbCommand("UPDATE Users SET Password = @password WHERE Username = @username", databaseConnection)) //database update query
                    {
                        update.Parameters.AddWithValue("@username", username); //adds parameters
                        update.Parameters.AddWithValue("@password", password);
                        update.ExecuteNonQuery(); //executes the query
                    }
                }
                finally
                {
                    MessageBox.Show("Password has been changed.",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information); //informs the user the password has been changed
                }
            }
            else
            {
                MessageBox.Show("User does not exist.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error); //should the user not exist, displays an error and allows the user to retry
            }
            CloseConnection();
        }
        public bool UserExists(string username)
        {
            OleDbCommand exists = new OleDbCommand("SELECT * FROM Users WHERE Username = @username", databaseConnection); //query to check if a user exists
            exists.Parameters.AddWithValue("@username", username);

            using (OleDbDataReader reader = exists.ExecuteReader(CommandBehavior.CloseConnection)) //reads the database and closes the connection
            {
                return reader.HasRows; //returns if a user has been found or not
            }
        }

        #endregion

        #region Students
        public void RemoveStudent(int id)
        {
            OpenConnection();
            using (OleDbCommand remove = new OleDbCommand("DELETE FROM Students WHERE ID = @id", databaseConnection)) //student deletion query
            {
                remove.Parameters.AddWithValue("@id", id); //adds a unique parameter
                remove.ExecuteNonQuery(); //executes query
            }
            CloseConnection();
        }
        public List<Student> GetStudent(string name)
        {
            List<Student> students = new List<Student>(); //initializes a new list of students
            OpenConnection();
            using (OleDbCommand getStudents = new OleDbCommand("SELECT ID, Name, Form, DateOfBirth FROM Students WHERE Name = @name ORDER BY ID ASC", databaseConnection)) //fetches all the entries in the database with provided name, ordered by ID
            {
                getStudents.Parameters.AddWithValue("@name", name); //adds the parameter
                using (OleDbDataReader reader = getStudents.ExecuteReader(CommandBehavior.CloseConnection)) //opens a reader and closes connection once complete
                {
                    while (reader.Read()) //while there are still entries remaining
                    {
                        var oStudent = new Student //creates a temporaray student object
                        {
                            ID = (int)reader[0], //casts integer to the value ID
                            Name = reader[1].ToString(),
                            Form = reader[2].ToString(),
                            DateOfBirth = reader[3].ToString()
                        };
                        students.Add(oStudent); //adds the student to the list
                    }
                    return students; //returns the completed list
                }
            }
        }
        public Student GetStudentByID(int id)
        {
            OpenConnection();
            using (OleDbCommand getStudent = new OleDbCommand("SELECT * FROM Students WHERE [ID] = @id", databaseConnection)) //query to get a specific studentManagement
            {
                getStudent.Parameters.AddWithValue("@id", id); //adds the parameter
                using (OleDbDataReader reader = getStudent.ExecuteReader(CommandBehavior.CloseConnection)) //executes the reader and closes connection once completed
                {
                    if (reader.Read()) //checks if entries are found
                    {
                        return new Student
                        {
                            ID = (int)reader[0], //casts integer to the value ID
                            Name = reader[1].ToString(),
                            Form = reader[2].ToString(),
                            DateOfBirth = reader[3].ToString(),
                            EmergencyName = reader[4].ToString(),
                            EmergencyNumber = reader[5].ToString()
                        };
                    }
                }

            }
            throw new InvalidOperationException("Student was not found."); //throws new exception in the case a user was not found, this should never happen
        }
        #endregion
    }
    internal class Student //internal student object
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Form { get; set; }
        public string DateOfBirth { get; set; }
        public string EmergencyName { get; set; }
        public string EmergencyNumber { get; set; }
    }
}

