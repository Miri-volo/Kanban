using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using log4net;

[assembly: InternalsVisibleTo("Test")]
namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    /// <summary>
    /// Represents a User table.
    /// </summary>
    internal class UserController : DOS
    {
        private readonly ILog log = LogManager.GetLogger("piza");
        private const string table = "User";

        ///<summary>Constructor of a new UserController.</summary>
        ///<param name="table"> Table name.</param>
        public UserController() : base(table)
        {
            log.Debug("DAL UserController was created");
        }

        ///<summary>Create Table of User if does not exist.</summary>
        protected override void CreateTable()
        {
            log.Debug("Try to create User table");
            using var connection = new SQLiteConnection(connectionString);
            using var command = new SQLiteCommand(connection);
            connection.Open();
            log.Debug("Open connection");
            command.CommandText = $"CREATE TABLE IF NOT EXISTS {table} ({User.IDColumnName} INTEGER PRIMARY KEY NOT NULL, {User.EmailColumn} TEXT NOT NULL, {User.PasswordColumn} TEXT NOT NULL)";
            command.Prepare();
            command.ExecuteNonQuery();
            log.Debug("User table was created");
        }

        ///<summary>Add row in User table.</summary>
        ///<param name="email"> Email of the user.</param>
        ///<param name="password"> Password of the user.</param>
        ///<exception cref = "Exception" > Fail to update.</exception>
        ///<returns>return list.</returns>
        public User Create(String email, String password)
        {
            log.Debug("Try to add user to User's table");
            using var connection = new SQLiteConnection(connectionString);
            using var command = new SQLiteCommand(connection);
            connection.Open();
            log.Debug("Open connection");
            command.CommandText = $"INSERT INTO {table} ({User.EmailColumn},{User.PasswordColumn}) " +
                $"VALUES (@email, @password);";
            SQLiteParameter emailParam = new SQLiteParameter(@"email", email);
            command.Parameters.Add(emailParam);
            SQLiteParameter passwordParam = new SQLiteParameter(@"password", password);
            command.Parameters.Add(passwordParam);
            command.Prepare();
            int rowsChanged = command.ExecuteNonQuery();
            if (rowsChanged == 0)
            {
                log.Error($"Fail to add user {email}");
                throw new Exception($"Fail to add user {email}");
            }
            log.Debug("User added to User's table");
            return new User(this, (int)connection.LastInsertRowId, email, password);
        }

        ///<summary>List of User in User's table.</summary>
        ///<returns>Return list of User.</returns>
        public List<User> SelectAllUsers()
        {
            log.Debug("Try to get list of Users");
            List<User> result = Select().Cast<User>().ToList();
            log.Debug("Return list of Users");
            return result;
        }

        ///<summary>Import user from data with give id.</summary>
        ///<param name="id"> Id of the user.</param>
        ///<exception cref = "Exception" > Fail to import.</exception>
        ///<returns>Return User.</returns>
        public User ImportUser(int id)
        {
            log.Debug($"Try to import user {id}");
            using var connection = new SQLiteConnection(connectionString);
            using var command = new SQLiteCommand(connection);
            connection.Open();
            log.Debug("Open connection");
            command.CommandText = $"SELECT * FROM {table} WHERE {User.IDColumnName} = @id";
            command.Parameters.AddWithValue(@"id", id);
            using SQLiteDataReader reader = command.ExecuteReader();
            reader.Read();
            if (!reader.HasRows)
            {
                log.Error($"{id} is wrong or not exist in data.");
                throw new Exception($"Can not find '{id}' in data'.");
            }
            log.Debug("Return user from data");
            return ConvertReaderToObject(reader);
        }

        ///<summary>Import user from data with give email.</summary>
        ///<param name="id"> Id of the user.</param>
        ///<exception cref = "Exception" > Fail to import.</exception>
        ///<returns>Return User.</returns>
        public User ImportUser(string email)
        {
            log.Debug($"Try to import user {email}");
            List<User> results = new List<User>(); ;
            using var connection = new SQLiteConnection(connectionString);
            using var command = new SQLiteCommand(connection);
            connection.Open();
            log.Debug("Open connection");
            command.CommandText = $"SELECT * FROM {table} WHERE {User.EmailColumn} = @email";
            command.Parameters.AddWithValue(@"email", email);
            using SQLiteDataReader reader = command.ExecuteReader();
            reader.Read();
            if (!reader.HasRows)
            {
                log.Error($"{email} is wrong or not exist in data.");
                throw new Exception($"Can not find '{email}' in data'.");
            }
            log.Debug("Return user from data");
            return ConvertReaderToObject(reader);
        }

        ///<summary>Convert reader to User.</summary>
        ///<param name="reader"> Reader from data.</param>
        ///<returns>Return User.</returns>
        protected override User ConvertReaderToObject(SQLiteDataReader reader)
        {
            User result = new User(this, reader.GetInt32(0), reader.GetString(1), reader.GetString(2));
            return result;
        }

    }
}
