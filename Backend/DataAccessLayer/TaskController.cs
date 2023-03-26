using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Task = IntroSE.Kanban.Backend.DataAccessLayer.DTOs.Task;
using log4net;

[assembly: InternalsVisibleTo("Test")]
namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    internal class TaskController : DOS
    {
        private readonly ILog log = LogManager.GetLogger("piza");

        private const string table = "Tasks";

        /// <summary>Create a new controller.</summary>
        public TaskController() : base(table) { }

        /// <summary>Create a new Task.</summary>
        /// <param name="column">Column id the Task belongs to.</param>
        /// <param name="title">Task title.</param>
        /// <param name="description">Task description.</param>
        /// <param name="description">Task description.</param>
        /// <param name="due">Task due date.</param>
        /// <returns>Task DTO of the created Task.</returns>
        public Task Create(int column, string title, string description, string assignee, DateTime due)
        {
            using var connection = new SQLiteConnection(connectionString);
            using var command = new SQLiteCommand(connection);
            connection.Open();
            command.CommandText = $"INSERT INTO {table} ({Task.ColumnIdColumn}, {Task.TitleColumn}, {Task.DescriptionColumn}, {Task.AssigneeColumn}, {Task.CreatedColumn}, {Task.DueColumn})"
                + "VALUES (@column, @title, @description, @assignee, @created, @due)";
            var columnParam = new SQLiteParameter(@"column", column);
            command.Parameters.Add(columnParam);
            var titleParam = new SQLiteParameter(@"title", title);
            command.Parameters.Add(titleParam);
            var descriptionParam = new SQLiteParameter(@"description", description);
            command.Parameters.Add(descriptionParam);
            var assigneeParam = new SQLiteParameter(@"assignee", assignee);
            command.Parameters.Add(assigneeParam);
            var now = DateTime.Now;
            var createdParam = new SQLiteParameter(@"created", now.ToString());
            command.Parameters.Add(createdParam);
            var dueParam = new SQLiteParameter(@"due", due.ToString());
            command.Parameters.Add(dueParam);
            command.Prepare();
            command.ExecuteNonQuery();
            log.Debug("Created new Task in DB.");
            return new Task(this, (int) connection.LastInsertRowId, column, title, description, assignee, now, due);
        }

        /// <summary>Select Tasks belonging to a Column.</summary>
        /// <param name="id">Column id to look up.</param>
        /// <returns>All tasks in the Column.</returns>
        public List<Task> SelectColumn(int id)
        {
            using var connection = new SQLiteConnection(connectionString);
            using var command = new SQLiteCommand(connection);
            connection.Open();
            command.CommandText = $"SELECT * FROM {table} WHERE {Task.ColumnIdColumn}=@id";
            command.Parameters.AddWithValue(@"id", id);
            using SQLiteDataReader reader = command.ExecuteReader();
            var result = new List<DTO>();
            while (reader.Read())
            {
                result.Add(ConvertReaderToObject(reader));
            }
            log.Debug($"Selected Tasks of Column '{id}'.");
            return result.Cast<Task>().ToList();
        }

        protected override void CreateTable()
        {
            using var connection = new SQLiteConnection(connectionString);
            using var command = new SQLiteCommand(connection);
            connection.Open();
            command.CommandText = $"CREATE TABLE IF NOT EXISTS {table} (Id INTEGER PRIMARY KEY NOT NULL, {Task.ColumnIdColumn} INTEGER NOT NULL, {Task.TitleColumn} TEXT NOT NULL, {Task.DescriptionColumn} TEXT, {Task.AssigneeColumn} TEXT NOT NULL, {Task.CreatedColumn} TEXT NOT NULL, {Task.DueColumn} TEXT NOT NULL)";
            command.Prepare();
            command.ExecuteNonQuery();
            log.Debug($"Created table '{table}' if it does not exist.");
        }

        protected override DTO ConvertReaderToObject(SQLiteDataReader reader)
        {
            return new Task(this, reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), DateTime.Parse(reader.GetString(5)), DateTime.Parse(reader.GetString(6)));
        }
    }
}
