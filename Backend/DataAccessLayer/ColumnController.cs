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
    internal class ColumnController : DOS
    {
        private readonly ILog log = LogManager.GetLogger("piza");

        private const string table = "Columns";

        /// <summary>Create a new controller.</summary>
        public ColumnController() : base(table) { }

        /// <summary>Create a new Column.</summary>
        /// <param name="board">Board id the Column belongs to.</param>
        /// <param name="title">Column title.</param>
        /// <param name="ordinal">Column ordinal.</param>
        /// <returns>DTO of the created Column.</returns>
        public Column Create(int board, int ordinal, string title)
        {
            // Default limit is unlimited, -1.
            using var connection = new SQLiteConnection(connectionString);
            using var command = new SQLiteCommand(connection);
            connection.Open();
            command.CommandText = $"INSERT INTO {table} ({Column.BoardIdColumn}, {Column.OrdinalColumn}, {Column.TitleColumn}, {Column.LimitColumn})"
                + "VALUES (@board, @ordinal, @title, @limit)";
            command.Parameters.AddWithValue(@"board", board);
            command.Parameters.AddWithValue(@"ordinal", ordinal);
            command.Parameters.AddWithValue(@"title", title);
            command.Parameters.AddWithValue(@"limit", -1);
            command.Prepare();
            command.ExecuteNonQuery();
            log.Debug("Created new Column in DB.");
            return new Column(this, (int)connection.LastInsertRowId, board, ordinal, title, -1);
        }

        /// <summary>Select three Columns of a Board.</summary>
        /// <param name="id">Board id to select.</param>
        /// <returns>Columns of the board with their names as keys.</returns>
        public List<Column> SelectBoard(int id)
        {
            using var connection = new SQLiteConnection(connectionString);
            using var command = new SQLiteCommand(connection);
            connection.Open();
            command.CommandText = $"SELECT * FROM {table} WHERE {Column.BoardIdColumn}=@id ORDER BY {Column.OrdinalColumn}";
            command.Parameters.AddWithValue(@"id", id);
            using SQLiteDataReader reader = command.ExecuteReader();
            List<Column> result = new();
            while (reader.Read())
            {
                result.Add((Column)ConvertReaderToObject(reader));
            }
            log.Debug($"Selected Columns of Board '{id}'.");
            return result;
        }

        protected override void CreateTable()
        {
            using var connection = new SQLiteConnection(connectionString);
            using var command = new SQLiteCommand(connection);
            connection.Open();
            command.CommandText = $"CREATE TABLE IF NOT EXISTS {table} (Id INTEGER PRIMARY KEY NOT NULL, {Column.BoardIdColumn} INTEGER NOT NULL, {Column.OrdinalColumn} INTEGER NOT NULL, {Column.TitleColumn} TEXT NOT NULL, {Column.LimitColumn} INTEGER NOT NULL)";
            command.Prepare();
            command.ExecuteNonQuery();
            log.Debug($"Created table '{table}' if it does not exist.");
        }

        protected override DTO ConvertReaderToObject(SQLiteDataReader reader)
        {
            return new Column(this, reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetString(3), reader.GetInt32(4));
        }

        public Column SelectColumnByTask(int taskId)
        {
            log.Debug("Try to open connection and get column holding a certain task.");
            using var connection = new SQLiteConnection(connectionString);
            using var command = new SQLiteCommand(connection);
            connection.Open();

            command.CommandText = $"SELECT * FROM {table} WHERE Id=(SELECT Column FROM Tasks WHERE Id=@task);";
            command.Parameters.AddWithValue(@"task", taskId);

            using SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                return (Column) ConvertReaderToObject(reader);
            }
            return null;
        }
    }
}
