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
    internal class BoardController : DOS
    {
        private readonly ILog log = LogManager.GetLogger("piza");
        /// <summary>The string name of the board table.</summary>
        private const string BoardTableName = "Board";

        public BoardController() : base(BoardTableName)
        {
        }

        /// <summary>
        /// Delete this method.
        /// </summary>
        protected override void CreateTable()
        {
            using var connection = new SQLiteConnection(connectionString);
            using var command = new SQLiteCommand(connection);
            connection.Open();
            
            command.CommandText = $"CREATE TABLE IF NOT EXISTS {BoardTableName} " +
                $"({Board.IDColumnName} INTEGER PRIMARY KEY NOT NULL, " +
                $"{Board.CreatorEmailColumn} TEXT NOT NULL, " +
                $"{Board.BoardNameColumn} TEXT NOT NULL)";
            
            command.Prepare();
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Creates a new Board.
        /// </summary>
        /// <param name="creator">The email of the creator of the board</param>
        /// <param name="name">The name of the board to create</param>
        /// <returns>A Board object.</returns>
        public Board Create(string creator, string name)
        {
            log.Debug("Try to open connection and save new board to data.");
            using var connection = new SQLiteConnection(connectionString);
            using var command = new SQLiteCommand(connection);
            connection.Open();
            
            command.CommandText = $"INSERT INTO {BoardTableName} " +
                $"({Board.CreatorEmailColumn},{Board.BoardNameColumn}) " +
                "VALUES (@creatorEmailVal,@boardNameVal);";
            
            SQLiteParameter creatorEmailParam = new (@"creatorEmailVal", creator);
            SQLiteParameter boardNameParam = new (@"boardNameVal", name);

            command.Parameters.Add(creatorEmailParam);
            command.Parameters.Add(boardNameParam);

            command.Prepare();
            command.ExecuteNonQuery();
            log.Debug("Close connection, save new board to data successfully.");
            return new Board(this, (int)connection.LastInsertRowId, creator, name);
        }

        /// <summary>
        /// Returns a List of all Boards data objects in database.
        /// </summary>
        /// <returns>A List of Boards data objects.</returns>
        public List<Board> SelectAllBoards()
        {
            log.Debug("Try to get all boards data.");
            List<Board> result = Select().Cast<Board>().ToList();
            log.Debug("Return all boards data successfully.");
            return result;
        }

        /// <summary>
        /// Get Column holding a certain task.
        /// </summary>
        /// <param name="taskId">Task to look up.</param>
        /// <returns>Column containing the Task.</returns>
        public Column SelectColumnByTask(int taskId)
        {
            log.Debug("Try to open connection and get column holding a certain task.");
            // Little bit of indirection to make things more aesthetically pleasing.
            // You know how much we like aesthetics around here.
            return new ColumnController().SelectColumnByTask(taskId);
        }

        /// <summary>
        /// Get Board containing a certain task.
        /// </summary>
        /// <param name="taskId">Task to look up.</param>
        /// <returns>Board containing the Task.</returns>
        public Board SelectBoardByTask(int taskId)
        {
            log.Debug("Try to open connection and get column holding a certain task.");
            using var connection = new SQLiteConnection(connectionString);
            using var command = new SQLiteCommand(connection);
            connection.Open();

            command.CommandText = $"SELECT * FROM {BoardTableName} WHERE Id=(SELECT Columns.Board FROM Tasks JOIN Columns ON Tasks.Column=Columns.Id AND Tasks.Id=@task);";
            command.Parameters.AddWithValue(@"task", taskId);

            using SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                return ConvertReaderToObject(reader);
            }
            return null;
        }

        /// <summary>
        /// Converts reader to Board data object.
        /// </summary>
        /// <param name="reader">The reader</param>
        /// <returns>A Board object.</returns>
        protected override Board ConvertReaderToObject(SQLiteDataReader reader)
        {
            log.Debug("Try convert reader to a board data object.");
            Board boardDto = new (this, reader.GetInt32(0), reader.GetString(1), reader.GetString(2));
            log.Debug("Return converted reader to board data object successfully.");
            return boardDto;
        }

        /// <summary>
        /// Gets a Board given its unique id.
        /// </summary>
        /// <param name="boardId">The unique key of the board to get</param>
        /// <returns>A Board object.</returns>
        public Board SelectBoard(int boardId)
        {
            log.Debug("Try to open connection and get board from data.");
            using var connection = new SQLiteConnection(connectionString);
            using var command = new SQLiteCommand(connection);
            connection.Open();
            
            command.CommandText = $"SELECT * FROM {BoardTableName} " +
                $"WHERE {Board.IDColumnName} = @boardIdVal;";
            
            SQLiteParameter boardIdParam = new (@"boardIdVal", boardId);
            command.Parameters.Add(boardIdParam);

            using SQLiteDataReader reader = command.ExecuteReader();
            reader.Read();
            if (!reader.HasRows)
            {
                log.Error("Board's id is wrong or not exist in data.");
                throw new Exception($"Can not find board's id '{boardId}' in data'.");
            }
            log.Debug("Close connection, get board from data successfully.");
            return ConvertReaderToObject(reader);
        }

        /// <summary>
        /// Gets all Board names created by creator.
        /// </summary>
        /// <param name="creator">The creator's email</param>
        /// <returns>A List of strings.</returns>
        public List<string> SelectAllCreatorBoardNames(string creator)
        {
            log.Debug("Try to open connection and get all boards names by creator from data.");
            using var connection = new SQLiteConnection(connectionString);
            using var command = new SQLiteCommand(connection);
            connection.Open();

            command.CommandText = $"SELECT {Board.BoardNameColumn} FROM {BoardTableName} WHERE {Board.CreatorEmailColumn} = @creatorEmail;";
            command.Parameters.AddWithValue(@"creatorEmail", creator);

            using SQLiteDataReader reader = command.ExecuteReader();

            List<string> boardsNames = new();
            while (reader.Read())
            {
                boardsNames.Add(reader.GetString(0));
            }
            log.Debug("Close connection, get and return all boards names by creator from data successfully.");
            return boardsNames;
        }
    }
}
