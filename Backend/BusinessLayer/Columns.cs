using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using log4net;

using IntroSE.Kanban.Backend.DataAccessLayer;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    class Columns
    {
        private readonly ILog log = LogManager.GetLogger("piza");

        private const string defaultBacklogName = "backlog";
        private const int defaultBacklogOrdinal = 0;
        private const string defaultInProgressName = "in progress";
        private const int defaultInProgressOrdinal = 1;
        private const string defaultDoneName = "done";
        private const int defaultDoneOrdinal = 2;

        private int boardId;

        private List<Column> columns;
        /// <summary>
        /// List all columns.
        /// Not named Columns because apparently C# does not like that.
        /// </summary>
        public List<Column> List
        {
            get => columns;
        }

        /// <summary>
        /// Get the backlog column.
        /// </summary>
        public Column Backlog
        {
            get => columns[defaultBacklogOrdinal];
        }

        /// <summary>
        /// Get the done column's ordinal.
        /// </summary>
        public int DoneOrdinal
        {
            get => columns.Count - 1;
        }

        /// <summary>
        /// Create Columns instance.
        /// </summary>
        /// <param name="board">Id of the Board.</param>
        /// <param name="load">Enables loading from persistence.</param>
        public Columns(int board, bool load=false)
        {
            boardId = board;
            if (load)
            {
                log.Debug($"Loading Columns of board '{board}'.");
                var columnController = new ColumnController();
                columns = columnController.SelectBoard(board)
                    .AsQueryable()
                    .Select(dto => new Column(dto))
                    .ToList();
            }
            else
            {
                log.Debug($"Creating Columns for board '{board}'.");
                columns = new List<Column>
                {
                    new Column(board, defaultBacklogOrdinal, defaultBacklogName),
                    new Column(board, defaultInProgressOrdinal, defaultInProgressName),
                    new Column(board, defaultDoneOrdinal, defaultDoneName)
                };
            }
        }

        /// <summary>
        /// Get a Column.
        /// </summary>
        /// <param name="ordinal">Ordinal of the Column to get.</param>
        /// <exception cref="ArgumentException">When ordinal is out of bounds.</exception>
        /// <returns>Column.</returns>
        public Column GetColumn(int ordinal)
        {
            if (ordinal < 0 || columns.Count <= ordinal)
            {
                log.Error($"Tried to get Column '{ordinal}' which does not exist.");
                throw new ArgumentException("Ordinal must exist.");
            }
            return columns[ordinal];
        }

        /// <summary>
        /// Create a new Column.
        /// </summary>
        /// <param name="ordinal">Ordinal of the new Column.</param>
        /// <param name="name">Name of the new Column.</param>
        /// <exception cref="ArgumentException">When ordinal is out of bounds.</exception>
        /// <exception cref="ArgumentException">When name is null or empty.</exception>
        public void AddColumn(int ordinal, string name)
        {
            if (ordinal < 0 || columns.Count < ordinal)
            {
                log.Error($"Tried to add Column '{ordinal}' which cannot be added.");
                throw new ArgumentException("New Column ordinal must be valid.");
            }
            if (name == null || name.Length == 0)
            {
                log.Error($"Tried to add Column with empty name.");
                throw new ArgumentException("New Column name must not be empty.");
            }
            columns.Insert(ordinal, new Column(boardId, ordinal, name));
            ChangeOrdinals();
            log.Debug("Inserted Column.");
        }

        /// <summary>
        /// Remame Column.
        /// </summary>
        /// <param name="ordinal">Ordinal of Column to rename.</param>
        /// <param name="newName">New name.</param>
        /// <exception cref="ArgumentException">When ordinal is out of bounds.</exception>
        /// <exception cref="ArgumentException">When Column name is null or empty.</exception>
        public void RenameColumn(int ordinal, string newName)
        {
            if (newName == null || newName.Length == 0)
            {
                log.Error("Tried to rename Column to empty or null name.");
                throw new ArgumentException("New Column name must not be empty.");
            }
            GetColumn(ordinal).Name = newName;
            log.Debug("Renamed Column.");
        }

        /// <summary>
        /// Move Column.
        /// </summary>
        /// <param name="ordinal">Ordinal of the Column to move.</param>
        /// <param name="shift">Amount to shift the Column by.</param>
        /// <exception cref="ArgumentException">When shift amount is 0.</exception>
        /// <exception cref="ArgumentException">When ordinal is out of bounds.</exception>
        /// <exception cref="ArgumentException">When shifting out of bounds.</exception>
        public void MoveColumn(int ordinal, int shift)
        {
            if (shift == 0)
            {
                log.Error("Tried to move Column by 0.");
                throw new ArgumentException("Shift amount must not be 0.");
            }
            int newOrdinal = ordinal + shift;
            if (newOrdinal < 0 || columns.Count <= newOrdinal)
            {
                log.Error("Tried to move Column out of bounds.");
                throw new ArgumentException("Shift must keep ordinal within bounds.");
            }
            Column current = GetColumn(ordinal);
            if (current.Count != 0)
            {
                log.Error("Tried to move Column with tasks inside.");
                throw new ArgumentException("Moved Column must be empty.");
            }
            columns.RemoveAt(ordinal);
            columns.Insert(newOrdinal, current);
            ChangeOrdinals();
            log.Debug("Moved Column.");
        }

        /// <summary>
        /// Remove Column.
        /// </summary>
        /// <param name="ordinal">Ordinal of the Column to remove.</param>
        /// <exception cref="ArgumentException">When only 2 Columns exist.</exception>
        /// <exception cref="ArgumentException">When ordinal is out of bounds.</exception>
        public void RemoveColumn(int ordinal)
        {
            if (columns.Count <= 2)
            {
                log.Error("Tried to remove Column when only 2 columns are in the Board.");
                throw new ArgumentException("Cannot remove when there are 2 Columns.");
            }
            Column toRemove = GetColumn(ordinal);
            if (ordinal == defaultBacklogOrdinal)
            {
                log.Debug("Moving tasks forward.");
                GetColumn(ordinal + 1).ConsumeColumn(toRemove);
            }
            else
            {
                log.Debug("Moving tasks backward.");
                GetColumn(ordinal - 1).ConsumeColumn(toRemove);
            }
            columns.RemoveAt(ordinal);
            toRemove.Remove();
            ChangeOrdinals();
            log.Debug("Removed Column.");
        }

        /// <summary>
        /// Delete all columns.
        /// </summary>
        public void RemoveAll()
        {
            foreach (Column column in columns)
            {
                column.Remove();
            }
            log.Debug("Removed all Columns.");
        }

        /// <summary>
        /// Get in progress tasks of an assignee.
        /// </summary>
        /// <param name="email">Assignee.</param>
        /// <returns></returns>
        public List<ITask> GetAssigneeTasks(string email)
        {
            log.Debug("Getting assignee tasks from Columns.");
            return columns.AsQueryable()
                .Skip(1)
                .SkipLast(1)
                .SelectMany((column) => column.GetAssigneeTasks(email))
                .ToList();
        }

        /// <summary>
        /// Recalculate Column ordinals.
        /// </summary>
        private void ChangeOrdinals()
        {
            // The efficiency train has left the station a long time ago.
            for (int ordinal = 0; ordinal < columns.Count; ordinal++)
            {
                columns[ordinal].Ordinal = ordinal;
            }
        }
    }
}
