using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using DalBoard = IntroSE.Kanban.Backend.DataAccessLayer.DTOs.Board;
using IntroSE.Kanban.Backend.DataAccessLayer;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    /// <summary>
    /// Represents a Board which contains 3 <c>Column</c>s and stored inside a <c>BoardController</c>.
    /// </summary>
    class Board
    {
        private readonly ILog log = LogManager.GetLogger("piza");

        /// <summary>Persistance object.</summary>
        private DalBoard dto;

        private const int backlogColumnOrdinal = 0;

        /// <summary>Gets the boardId of the Board.</summary>
        /// <value>Identifier unique Board object int key.</value>
        public readonly int boardId;
        /// <summary>Gets the board creator's email.</summary>
        /// <value>Identifier string for access key.</value>
        public readonly string creatorEmail;
        /// <summary>Gets the name of the Board.</summary>
        /// /// <value>string.</value>
        public readonly string boardName;
        private Columns columns;
        /// <summary>The board members emails.</summary>
        /// <value>Identifier list of strings for access keys.</value>
        private List<string> members;

        /// <summary>
        /// Get the done column's ordinal.
        /// </summary>
        public int DoneOrdinal
        {
            get => columns.DoneOrdinal;
        }

        public List<Column> Columns
        {
            get => columns.List;
        }

        /// <summary>
        /// Create a new board object from data.
        /// </summary>
        /// <param name="dto">The board object to use from data.</param>
        /// <returns>A Board object</returns>
        public Board(DalBoard dto)
        {
            this.boardId = dto.Id;
            this.creatorEmail = dto.creatorEmail;
            this.boardName = dto.boardName;
            this.members = new List<string>();

            this.columns = new Columns(boardId, true);
            log.Debug("Loaded board from data.");
        }

        public Board(string creator, string name)
        {
            var boardController = new DataAccessLayer.BoardController();
            var dalBoard = boardController.Create(creator, name);
            this.boardId = dalBoard.Id;
            this.creatorEmail = dalBoard.creatorEmail;
            this.boardName = dalBoard.boardName;
            this.members = new List<string>();
            this.columns = new Columns(boardId);
            log.Debug("Ceated board object.");
        }

        ///<summary>Removes all persistent data of this board.</summary>
        public void Remove()
        {
            log.Debug("Try to remove board data.");
            columns.RemoveAll();
        }

        /// <summary>
        /// Returns a column given it's columnOrdinal
        /// </summary>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>A Column object.</returns>
        /// <exception cref="InvalidOperationException" > In case of non-existing column ID.</exception>
        public Column GetColumn(int columnOrdinal)
        {
            log.Debug($"Try to get column {columnOrdinal}.");
            return columns.GetColumn(columnOrdinal);
        }

        /// <summary>
        /// Add a new task.
        /// </summary>
        /// <param name="currentUser">Email of the current user.</param>
        /// <param name="title">Title of the new task</param>
        /// <param name="description">Description of the new task</param>
        /// <param name="dueDate">The due date if the new task</param>
        /// <returns>A Task object</returns>
        public Task AddTask(string currentUser, string title, string description, DateTime dueDate)
        {
            log.Debug($"Try to add task.");
            //Create a task.
            Task taskToAdd = new (columns.Backlog.Id, title, description, currentUser, dueDate);
            //Add task to 'backlog' column of this board.
            columns.Backlog.AddTask(taskToAdd);
            log.Debug("Add created new task to board. Task counter update. Return task successfully.");
            return taskToAdd;
        }

        /// <summary>
        /// Verify if task is changable according to column (ordinal input).
        /// </summary>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <exception cref="InvalidOperationException"> In case of non-existing column ID or in case of column is not allowed to be updated.</exception>
        private void VerifyTaskIsChangableAccordingToColumn(int columnOrdinal)
        {
            log.Debug("Try to varify if task is changeable according to column.");
            //Verify column is not 'done'.
            if (columnOrdinal == columns.DoneOrdinal)
            {
                log.Error("'done' column's tasks are not allowed to be changed.");
                throw new InvalidOperationException("Can not change a Task if it is 'done'.");
            }
            //Verify column's ordinal exist.
            if (columnOrdinal < backlogColumnOrdinal | columnOrdinal >= columns.DoneOrdinal)
            {
                log.Error($"Column ordinal entered is not between 0 to {columns.DoneOrdinal}.");
                throw new InvalidOperationException($"A column ordinal is a number between 0 to {columns.DoneOrdinal}.");
            }
            log.Debug("Task is changeable according to column.");
        }

        /// <summary>
        /// Verify the email is legal to change a task according to its assignee.
        /// </summary>
        /// <param name="task">The Task object to check.</param>
        /// <param name="currentUser">Email of the current user. Must be assignee of the task.</param>
        /// <exception cref="UnauthorizedAccessException"> In case of current user is not the assignee of the task.</exception>
        private void VerifyTaskAccess(ITask task, string currentUser)
        {
            log.Debug("Try to varify if task is changeable according to current user.");
            if (!(task.Assignee.Equals(currentUser)))
            {
                log.Error($"User is not task's assignee therfore not allowed to change task.");
                throw new UnauthorizedAccessException($"User mast be task's assignee in order to change the task.");
            }
            log.Debug("Task is changeable according to current user.");
        }

        /// <summary>
        /// Update the due date of a task
        /// </summary>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskID">The task to be updated identified task ID</param>
        /// <param name="newDueDate">The new due date of the column</param>
        /// <param name="currentUser">Email of the current user. Must be assignee of the task.</param>
        /// <returns>A Task object.</returns>
        /// <exception cref="Exception">In case of failing varifying column</exception>
        public ITask UpdateTaskDueDate(int taskID, int columnOrdinal, DateTime newDueDate, string currentUser)
        {
            log.Debug("Try to update task due date.");
            VerifyTaskIsChangableAccordingToColumn(columnOrdinal); //Throws exception if task not changable according to ordinal.
            //Reach task.
            ITask taskToChange = columns.GetColumn(columnOrdinal).GetTask(taskID);
            VerifyTaskAccess(taskToChange, currentUser); //Throws exception if task not changable according to assignee.
            //Change task's due date.
            taskToChange.Due = newDueDate;
            log.Debug("Change task's due date and return updated task successfully.");
            return taskToChange;
        }

        /// <summary>
        /// Update task title
        /// </summary>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskID">The task to be updated identified task ID</param>
        /// <param name="newTitle">New title for the task</param>
        /// <param name="currentUser">Email of the current user. Must be assignee of the task.</param>
        /// <returns>A Task object.</returns>
        /// <exception cref="Exception">In case of failing varifying column</exception>
        public ITask UpdateTaskTitle(int taskID, int columnOrdinal, string newTitle, string currentUser)
        {
            log.Debug("Try to update task title.");
            VerifyTaskIsChangableAccordingToColumn(columnOrdinal); //Throws exception if task not changable according to ordinal.
            //Reach task.
            ITask taskToChange = columns.GetColumn(columnOrdinal).GetTask(taskID);
            VerifyTaskAccess(taskToChange, currentUser); //Throws exception if task not changable according to assignee.
            //Change task's title.
            taskToChange.Title = newTitle;
            log.Debug("Change task's title and return updated task successfully.");
            return taskToChange;
        }

        /// <summary>
        /// Update the description of a task
        /// </summary>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskID">The task to be updated identified task ID</param>
        /// <param name="newDescription">New description for the task</param>
        /// <param name="currentUser">Email of the current user. Must be assignee of the task.</param>
        /// <returns>A Task object.</returns>
        /// <exception cref="Exception">In case of failing varifying column</exception>
        public ITask UpdateTaskDescription(int taskID, int columnOrdinal, string newDescription, string currentUser)
        {
            log.Debug("Try to update task description.");
            VerifyTaskIsChangableAccordingToColumn(columnOrdinal); //Throws exception if task not changable according to ordinal.
            //Reach task.
            ITask taskToChange = columns.GetColumn(columnOrdinal).GetTask(taskID);
            VerifyTaskAccess(taskToChange, currentUser); //Throws exception if task not changable according to assignee.
            //Update task's description.
            taskToChange.Description = newDescription;
            log.Debug("Change task's description and return updated task successfully.");
            return taskToChange;
        }

        /// <summary>
        /// Advance a task to the next column
        /// </summary>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskID">The task to be updated identified task ID</param>
        /// <param name="currentUser">Email of the current user. Must be assignee of the task.</param>
        /// <returns>A Task object.</returns>
        /// <exception cref="Exception">In case of failing varifying column</exception>
        public ITask AdvanceTask(int taskID, int columnOrdinal, string currentUser)
        {
            log.Debug("Try to advance a task.");
            VerifyTaskIsChangableAccordingToColumn(columnOrdinal); //Throws exception if task not changable according to ordinal.
            //Remove Task from old column.
            ITask taskToAdvance = columns.GetColumn(columnOrdinal).RemoveTask(taskID);
            VerifyTaskAccess(taskToAdvance, currentUser); //Throws exception if task not changable according to assignee.
            //Add task to next column.
            columns.GetColumn(columnOrdinal + 1).AddTask(taskToAdvance);
            log.Debug("Remove task from previous column, add to next column. Return updated task successfully.");
            return taskToAdvance;
        }

        /// <summary>
        /// Set new assignee of a task.
        /// </summary>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskID">The task to be updated identified task ID</param>
        /// <param name="newAssigneeEmail">Email of the new assignee of the task.</param>
        /// <exception cref="Exception">In case of failing varifying column</exception>
        /// <exception cref="UnauthorizedAccessException">In case of newAssigneeEmail is not in members.</exception>
        public void AssignTask(int taskID, int columnOrdinal, string newAssigneeEmail)
        {
            log.Debug("Try to set an assignee of a task.");
            VerifyTaskIsChangableAccordingToColumn(columnOrdinal); //Throws exception if task not changable according to ordinal.
            //Verify email is in memebers.
            if (!IsMember(newAssigneeEmail))
            {
                log.Error($"The user can not be assignee to a board's task if he is not a board member.");
                throw new AccessViolationException($"Only board's memebers can be assignee to board's tasks.");
            }
            //Reach task
            ITask taskToAssign = columns.GetColumn(columnOrdinal).GetTask(taskID);
            //Change assignee
            taskToAssign.Assignee = newAssigneeEmail;
            log.Debug("Change assignee of the task successfully.");
        }

        /// <summary>
        /// Returns all the In progress tasks of in the board of the assignee.
        /// </summary>
        /// <param name="assigneeEmail">Email of the assignee.</param>
        /// <returns>A list of tasks object. </returns>
        public List<ITask> GetInProgressTasksList(string assigneeEmail)
        {
            log.Debug("Try to get a list of in-progress tasks of this board from this user.");
            //Verify email is in memebers.
            if (!IsMember(assigneeEmail))
            {
                log.Error($"There are no members of this board with this email.");
                throw new AccessViolationException($"Only board's memebers can have tasks in the board.");
            }
            log.Debug("Return created list of tasks successfully.");
            return columns.GetAssigneeTasks(assigneeEmail);
        }

        /// <summary>
        /// Return if members contains email.
        /// </summary>
        /// <param name="email">Email of the user.</param>
        /// <returns>bool value. </returns>
        public bool IsMember(string email)
        {
            return members.Contains(email);
        }

        /// <summary>
        /// Join a memeber to board.
        /// </summary>
        /// <param name="email">Email of the user to join. Assume not a member yet.</param>
        public void Join(string email)
        {
            members.Add(email);
            log.Debug("Added Board member.");
        }

        /// <summary>
        /// Add Column to Board.
        /// </summary>
        /// <param name="ordinal">Ordinal.</param>
        /// <param name="name">Name.</param>
        public void AddColumn(int ordinal, string name)
        {
            columns.AddColumn(ordinal, name);
            log.Debug("Added Column.");
        }

        /// <summary>
        /// Rename Column.
        /// </summary>
        /// <param name="ordinal">Column ordinal to rename.</param>
        /// <param name="newName">New name.</param>
        public void RenameColumn(int ordinal, string newName)
        {
            columns.RenameColumn(ordinal, newName);
            log.Debug("Renamed Column.");
        }

        /// <summary>
        /// Move Column.
        /// </summary>
        /// <param name="ordinal">Column ordinal to move.</param>
        /// <param name="shift">Amount to shift by.</param>
        public void MoveColumn(int ordinal, int shift)
        {
            columns.MoveColumn(ordinal, shift);
            log.Debug("Moved Column.");
        }

        /// <summary>
        /// Remove Column.
        /// </summary>
        /// <param name="ordinal">Column ordinal to remove.</param>
        public void RemoveColumn(int ordinal)
        {
            columns.RemoveColumn(ordinal);
            log.Debug("Removed Column.");
        }
    }
}
