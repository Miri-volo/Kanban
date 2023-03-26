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
    class MembershipController : DOS
    {
        private readonly ILog log = LogManager.GetLogger("piza");
        /// <summary>The string name of the membership table.</summary>
        private const string membershipTableName = "Membership";

        public MembershipController() : base(membershipTableName)
        {
        }

        /// <summary>
        /// Converts reader to Membership data object.
        /// </summary>
        /// <param name="reader">The reader</param>
        /// <returns>A Membership object.</returns>
        protected override DTO ConvertReaderToObject(SQLiteDataReader reader)
        {
            log.Debug("Try convert reader to a membership data object.");
            Membership result = new (this, reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2));
            log.Debug("Return converted reader to membership data object successfully.");
            return result;
        }

        /// <summary>
        /// Delete this method.
        /// </summary>
        protected override void CreateTable()
        {
            using var connection = new SQLiteConnection(connectionString);
            using var command = new SQLiteCommand(connection);
            connection.Open();
            
            command.CommandText = $"CREATE TABLE IF NOT EXISTS {membershipTableName} " +
                $"({Membership.IDColumnName} INTEGER PRIMARY KEY NOT NULL, " +
                $"{Membership.BoardIdColumn} INTEGER NOT NULL, " +
                $"{Membership.MemberEmailColumn} TEXT NOT NULL)";
            
            command.Prepare();
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Creates a new Membership.
        /// </summary>
        /// <param name="boardId">The unique key of the board</param>
        /// <param name="memberEmail">The email of the member</param>
        /// <returns>A Membership object.</returns>
        public Membership Create(int boardId, string memberEmail)
        {
            log.Debug("Try to open connection and save new membership to data.");
            using var connection = new SQLiteConnection(connectionString);
            using var command = new SQLiteCommand(connection);
            connection.Open();
            
            command.CommandText = $"INSERT INTO {membershipTableName} " +
                $"({Membership.BoardIdColumn},{Membership.MemberEmailColumn}) " +
                "VALUES (@boardIdVal,@memberEmailVal);";
            command.Parameters.AddWithValue(@"boardIdVal", boardId);
            command.Parameters.AddWithValue(@"memberEmailVal", memberEmail);
            command.Prepare();
            command.ExecuteNonQuery();
            log.Debug("Close connection, save new membership to data successfully.");
            return new Membership(this, (int)connection.LastInsertRowId, boardId, memberEmail);
        }

        /// <summary>
        /// Creates a new Membership.
        /// </summary>
        /// <param name="board">The board</param>
        /// <param name="member">The member</param>
        /// <returns>A Membership object.</returns>
        public Membership Create(Board board, User member) 
        {
            return Create(board.Id, member.Email);
        }

        /// <summary>
        /// Returns a List of all Memberships data objects in database.
        /// </summary>
        /// <returns>A List of Memberships data objects.</returns>
        public List<Membership> SelectAllMemberships()
        {
            log.Debug("Try to get all memberships data.");
            List<Membership> memberships = Select().Cast<Membership>().ToList();
            log.Debug("Return all memberships data successfully.");
            return memberships;
        }

        /// <summary>
        /// Returns if the membership exist.
        /// </summary>
        /// <returns>A boolean expression.</returns>
        public bool IsMembership(int boardId, string memberEmail)
        {
            return SelectMembership(boardId, memberEmail) is not null;
        }

        /// <summary>
        /// Gets a Membership.
        /// </summary>
        /// <param name="boardId">The unique key of the board</param>
        /// <param name="memberEmail">The email of the member</param>
        /// <returns>A Membership object.</returns>
        public Membership SelectMembership(int boardId, string memberEmail)
        {
            log.Debug("Try to open connection and get membership from data.");
            using var connection = new SQLiteConnection(connectionString);
            using var command = new SQLiteCommand(connection);
            connection.Open();
            
            command.CommandText = $"SELECT * FROM {membershipTableName} WHERE {Membership.BoardIdColumn} = @boardId and {Membership.MemberEmailColumn} == @memberEmail";
            command.Parameters.AddWithValue(@"boardId", boardId);
            command.Parameters.AddWithValue(@"memberEmail", memberEmail);

            using SQLiteDataReader reader = command.ExecuteReader();

            reader.Read();
            log.Debug("Close connection, get membership from data successfully.");
            if (!reader.HasRows)
            {
                log.Error("Board's id with with member's email combination is wrong or not exist in data.");
                throw new Exception($"Can not find a combination of board's id '{boardId}' with member's email '{memberEmail}' in data.");
            }
            return (Membership)ConvertReaderToObject(reader);
        }

        /// <summary>
        /// Gets all memberships of a board.
        /// </summary>
        /// <param name="boardId">The boards's unique key</param>
        /// <returns>A List of Membership data objects.</returns>
        public List<Membership> SelectAllBoardMemberships(int boardId)
        {
            log.Debug("Try to open connection and get all memberships of board from data.");
            using var connection = new SQLiteConnection(connectionString);
            using var command = new SQLiteCommand(connection);
            connection.Open();

            command.CommandText = $"SELECT * FROM {membershipTableName} WHERE {Membership.BoardIdColumn} = @boardId;";
            command.Parameters.AddWithValue(@"boardId", boardId);

            using SQLiteDataReader reader = command.ExecuteReader();

            List<Membership> memberships = new ();
            while (reader.Read())
            {
                memberships.Add((Membership)ConvertReaderToObject(reader));
            }
            log.Debug("Close connection, get and return all memberships of board from data successfully.");
            return memberships;
        }

        /// <summary>
        /// Deletes all memberships of a board.
        /// </summary>
        /// <param name="boardId">The boards's unique key</param>
        /// <returns>A boolean expression.</returns>
        public bool DeleteAllBoardMemberships(int boardId)
        {
            log.Debug("Try to open connection and delete all memberships of board from data.");
            using var connection = new SQLiteConnection(connectionString);
            using var command = new SQLiteCommand(connection);
            connection.Open();

            command.CommandText = $"DELETE FROM {membershipTableName} where {Membership.BoardIdColumn} = @boardId;";
            command.Parameters.AddWithValue(@"boardId", boardId);
            command.Prepare();
            log.Debug("Close connection, delete all memberships of board from data successfully.");
            return command.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// Gets all boards the user is a member of.
        /// </summary>
        /// <param name="memberEmail">The member's email</param>
        /// <param name="bc">The BoardController responsible for boards in membership</param>
        /// <returns>List of boards.</returns>
        public List<Board> SelectAllMemberBoards(string memberEmail, BoardController bc)
        {
            log.Debug("Try to open connection and get all boards the user is a member of from data.");
            using var connection = new SQLiteConnection(connectionString);
            using var command = new SQLiteCommand(connection);
            connection.Open();

            command.CommandText = $"SELECT {Membership.BoardIdColumn} FROM {membershipTableName} WHERE {Membership.MemberEmailColumn} = @memberEmail;";
            command.Parameters.AddWithValue(@"memberEmail", memberEmail);

            using SQLiteDataReader reader = command.ExecuteReader();

            List<Board> boardNames = new();
            while (reader.Read())
            {
                boardNames.Add(bc.SelectBoard(reader.GetInt32(0)));
            }
            log.Debug("Close connection, get and return all boards names the user is a member of from data successfully.");
            return boardNames;
        }
    }
}
