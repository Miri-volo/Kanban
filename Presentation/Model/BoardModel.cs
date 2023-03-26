using log4net;
using System;
using System.Collections.Generic;

namespace IntroSE.Kanban.Presentation.Model
{
    /// <summary>
    /// Represents a Board Data.
    /// </summary>
    public class BoardModel : NotifiableModelObject, IEquatable<BoardModel>
    {
        private readonly ILog log = LogManager.GetLogger("piza");

        private const int firstColumnOrdinal = 0;

        /// <summary>Gets the 'done' column ordinal of the Board.</summary>
        public int LastColumnOrdinal { get; private set; }
        /// <summary>Gets the email of creator of the Board.</summary>
        public string CreatorEmail { get; }
        /// <summary>Gets the email of current user.</summary>
        public string UserEmail { get; }
        /// <summary>Gets the name of the Board.</summary>
        public string BoardName { get; }
        /// <summary>Gets the id of the Board.</summary>
        public int Id { get; }

        private IList<ColumnModel> columns;
        /// <summary>Gets the list of columns sorted by their ordinal of the Board.</summary>
        internal IList<ColumnModel> Columns
        {
            get
            {
                if(columns == null)
                {
                    columns = Controller.GetColumns(UserEmail, CreatorEmail, BoardName);
                }

                return columns;
            }
            private set
            {
                columns = value;
            }
        }

        /// <summary>
        /// Create a new board model object.
        /// </summary>
        /// <param name="backendController">The connection service with the Backend layer.</param>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="id">The id key of the board</param>
        /// <param name="lastColumnOrdinal">The the 'done' column ordinal of the Board.</param>
        /// <returns>A Board Model object</returns>
        public BoardModel(BackendController backendController, string userEmail, string creatorEmail, string boardName, int id, int lastColumnOrdinal) : base(backendController)
        {
            this.LastColumnOrdinal = lastColumnOrdinal;
            UserEmail = userEmail;
            CreatorEmail = creatorEmail;
            BoardName = boardName;
            Id = id;
            log.Debug("Created board model.");
        }

        /// <summary>
        /// Adds a new column
        /// </summary>
        /// <param name="columnOrdinal">The location of the new column. Location for old columns with index>=columnOrdinal is increased by 1 (moved right). The first column is identified by 0, the location increases by 1 for each column.</param>
        /// <param name="columnName">The name for the new columns</param>        
        public void AddColumn(int columnOrdinal, string columnName)
        {
            Controller.AddColumn(UserEmail, CreatorEmail, BoardName, columnOrdinal, columnName);
            this.LastColumnOrdinal ++;
            Columns = Controller.GetColumns(UserEmail, CreatorEmail, BoardName);
            log.Debug("Add column to board model.");

        }

        /// <summary>
        /// Removes a specific column
        /// </summary>
        /// <param name="columnOrdinal">The column location. The first column location is identified by 0, the location increases by 1 for each column</param>
        internal void RemoveColumn(int columnOrdinal)
        {
            Controller.RemoveColumn(UserEmail, CreatorEmail, BoardName, columnOrdinal);
            this.LastColumnOrdinal --;
            Columns = Controller.GetColumns(UserEmail, CreatorEmail, BoardName);
            log.Debug("Remove column from board model.");
        }

        /// <summary>
        /// Moves a column shiftSize times to the right. If shiftSize is negative, the column moves to the left
        /// </summary>
        /// <param name="columnOrdinal">The column location. The first column location is identified by 0, the location increases by 1 for each column</param>
        /// <param name="shiftSize">The number of times to move the column, relativly to its current location. Negative values are allowed</param>  
        internal void ShiftColumn(int columnOrdinal, int shiftSize)
        {
            Controller.MoveColumn(UserEmail, CreatorEmail, BoardName, columnOrdinal, shiftSize);
            Columns = Controller.GetColumns(UserEmail, CreatorEmail, BoardName);
            log.Debug("Shift column int board model.");
        }

        /// <summary>
        /// Renames a specific column
        /// </summary>
        /// <param name="column">The column to rename.</param>
        /// <param name="newColumnName">The new column name</param>     
        internal void RenameColumn(ColumnModel column, string newColumnName)
        {
            Controller.RenameColumn(UserEmail, CreatorEmail, BoardName, column.Ordinal, newColumnName);
            Columns[column.Ordinal].Name = newColumnName;
            column.Name = newColumnName;
            log.Debug("Rename column in board model.");
        }

        /// <summary>
        /// Limit the number of tasks in a specific column
        /// </summary>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="limit">The new limit value. A value of -1 indicates no limit.</param>
        internal void LimitColumn(int columnOrdinal, int limit)
        {
            Controller.LimitColumn(UserEmail, CreatorEmail, BoardName, columnOrdinal, limit);
            log.Debug("Limit column in board model.");
        }

        /// <summary>
        /// Add a new task.
        /// </summary>
        /// <param name="title">Title of the new task</param>
        /// <param name="description">Description of the new task</param>
        /// <param name="dueDate">The due date of the new task</param>
        internal void AddTask(string title, string description, DateTime dueDate)
        {
            TaskModel task = Controller.AddTask(UserEmail, CreatorEmail, BoardName, title, description, dueDate);
            Columns[firstColumnOrdinal].AddTask(task);
            log.Debug("Add task to column in board model.");
        }

        /// <summary>
        /// Advance a task to the next column
        /// </summary>
        /// <param name="task">Task to advance.</param>
        /// <param name="oldcolumn">Column task was in.</param>
        /// <param name="newColumn">Column task goes to.</param>
        internal void AdvanceTask(TaskModel task, ColumnModel oldcolumn, ColumnModel newColumn)
        {
            Controller.AdvanceTask(UserEmail, CreatorEmail, BoardName, task.Ordinal, task.Id);
            oldcolumn.RemoveTask(task);
            newColumn.AddTask(task);
            log.Debug("Advance task between columns in board model.");
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="other"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public bool Equals(BoardModel other)
        {
            log.Debug("Try to check equality between two board models.");
            return (this.Id == other.Id) & (this.CreatorEmail == other.CreatorEmail);
        }
    }
}