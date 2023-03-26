using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.ServiceLayer;
using log4net;

namespace IntroSE.Kanban.Presentation.Model
{
    ///<summary>Represents a Backend Controller</summary>
    public class BackendController
    {
        private readonly ILog log = LogManager.GetLogger("piza");
        private Service serviceSingleton { get; }

        ///<summary>Constructor of BackendController.</summary>
        public BackendController()
        {
            log.Debug("Created Backend Controller.");
            serviceSingleton = ServiceSingleton.GetInstance;
        }

        /// <summary>        
        /// Register a new user only if passwords are match. 
        /// </summary>
        /// <param name="username">The email of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <param name="validatePassword">Valid the password of the user.</param>
        /// <Exception>Throws in case of an error from responses.</returns>
        internal void Register(string username, string password, string validatePassword)
        {
            Response response1 = serviceSingleton.ValidatePassword(password, validatePassword);
            if (response1.ErrorOccured)
            {
                throw new Exception(response1.ErrorMessage);
            }
            else
            {
                Response response2 = serviceSingleton.Register(username, password);
                if (response2.ErrorOccured)
                {
                    throw new Exception(response2.ErrorMessage);
                }
            }
        }

        /// <summary>        
        /// Login registered user. 
        /// </summary>
        /// <param name="username">The email of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <Exception>Throws in case of an error from responses.</returns>
        internal UserModel Login(string email, string password)
        {
            Response<User> response = serviceSingleton.Login(email, password);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
            return new UserModel(this, email);
        }

        /// <summary>        
        /// Log out an logged in user. 
        /// </summary>
        /// <param name="userEmail">The email of the user to log out.</param>
        /// <Exception>Throws in case of an error from response.</returns>
        internal void Logout(string userEmail)
        {
            Response response = serviceSingleton.Logout(userEmail);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
        }

        /// <summary>
        /// Limit the number of tasks in a specific column
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="limit">The new limit value. A value of -1 indicates no limit.</param>
        /// <exception cref="Exception"></exception>
        internal void LimitColumn(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int limit)
        {
            Response response = serviceSingleton.LimitColumn(userEmail, creatorEmail, boardName, columnOrdinal, limit);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
        }

        /// <summary>
        /// Get the limit of a specific column
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>The int limit of the column.</returns>
        /// <exception cref="Exception"></exception>
        internal int GetColumnLimit(string userEmail, string creatorEmail, string boardName, int columnOrdinal)
        {
            Response<int> response = serviceSingleton.GetColumnLimit(userEmail, creatorEmail, boardName, columnOrdinal);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
            return response.Value;
        }

        /// <summary>
        /// Get the name of a specific column
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>The string name of the column.</returns>
        /// <exception cref="Exception"></exception>
        internal string GetColumnName(string userEmail, string creatorEmail, string boardName, int columnOrdinal)
        {
            Response<string> response = serviceSingleton.GetColumnName(userEmail, creatorEmail, boardName, columnOrdinal);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
            return response.Value;
        }

        /// <summary>
        /// Add a new task.
        /// </summary>
		/// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="title">Title of the new task</param>
        /// <param name="description">Description of the new task</param>
        /// <param name="dueDate">The due date of the new task</param>
        /// <returns>A TaskModel object that just added to data.</returns>
        /// <exception cref="Exception"></exception>
        internal TaskModel AddTask(string userEmail, string creatorEmail, string boardName, string title, string description, DateTime dueDate)
        {
            var response = serviceSingleton.AddTask(userEmail, creatorEmail, boardName, title, description, dueDate);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
            var task = response.Value;
            return new TaskModel(this, userEmail, task.emailAssignee, task.Title, task.Description, task.DueDate, task.CreationTime, task.Id);
        }

        /// <summary>
        /// Update the due date of a task
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="dueDate">The new due date of the column</param>
        /// <exception cref="Exception"></exception>
        internal void UpdateTaskDueDate(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int taskId, DateTime dueDate)
        {
            var response = serviceSingleton.UpdateTaskDueDate(userEmail, creatorEmail, boardName, columnOrdinal, taskId, dueDate);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
        }

        /// <summary>
        /// Update task title
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="title">New title for the task</param>
        /// <exception cref="Exception"></exception>
        internal void UpdateTaskTitle(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int taskId, string title)
        {
            var response = serviceSingleton.UpdateTaskTitle(userEmail, creatorEmail, boardName, columnOrdinal, taskId, title);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
        }

        /// <summary>
        /// Update the description of a task
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="description">New description for the task</param>
        /// <exception cref="Exception"></exception>
        internal void UpdateTaskDescription(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int taskId, string description)
        {
            var response = serviceSingleton.UpdateTaskDescription(userEmail, creatorEmail, boardName, columnOrdinal, taskId, description);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
        }

        /// <summary>
        /// Advance a task to the next column
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <exception cref="Exception"></exception>
        internal void AdvanceTask(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int taskId)
        {
            var response = serviceSingleton.AdvanceTask(userEmail, creatorEmail, boardName, columnOrdinal, taskId);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
        }

        /// <summary>
        /// Returns a column given it's name
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>An IList of TaskModel objects belong to column.</returns>
        /// <exception cref="Exception"></exception>
        internal IList<TaskModel> GetColumn(string userEmail, string creatorEmail, string boardName, int columnOrdinal)
        {
            var response = serviceSingleton.GetColumn(userEmail, creatorEmail, boardName, columnOrdinal);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
            var tasks = response.Value.Select((task, result) => new TaskModel(this, userEmail, task.emailAssignee, task.Title, task.Description, task.DueDate, task.CreationTime, task.Id));
            return new List<TaskModel>(tasks);
        }

        /// <summary>
        /// Creates a new board for the logged-in user.
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="boardName">The name of the new board</param>
        /// <returns>A BoardModel object that just added to data.</returns>
        /// <exception cref="Exception"></exception>
        internal BoardModel AddBoard(string userEmail, string boardName)
        {
            Response<Board> response = (Response<Board>)serviceSingleton.AddBoard(userEmail, boardName);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
            Board board = response.Value;
            return new BoardModel(this, userEmail, userEmail, boardName, board.Id, board.DoneOrdinal);
        }

        /// <summary>
        /// Adds a board created by another user to the logged-in user. 
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the new board</param>
        /// <returns>A BoardModel object that just added it's memebership to data.</returns>
        /// <exception cref="Exception"></exception>
        internal BoardModel JoinBoard(string userEmail, string creatorEmail, string boardName)
        {
            Response<Board> response = (Response<Board>)serviceSingleton.JoinBoard(userEmail, creatorEmail, boardName);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
            Board board = response.Value;
            return new BoardModel(this, userEmail, creatorEmail, boardName, board.Id, board.DoneOrdinal);
        }

        /// <summary>
        /// Removes a board.
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <exception cref="Exception"></exception>
        internal void RemoveBoard(string userEmail, string creatorEmail, string boardName)
        {
            Response response = serviceSingleton.RemoveBoard(userEmail, creatorEmail, boardName);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
        }

        /// <summary>
        /// Returns all the in-progress tasks of the logged-in user is assigned to.
        /// </summary>
        /// <param name="userEmail">Email of the logged in user</param>
        /// <returns>An IList of TaskModel objects belong to all in-progress columns for user.</returns>
        /// <exception cref="Exception"></exception>
        internal IList<TaskModel> InProgressTasks(string userEmail)
        {
            var response = serviceSingleton.InProgressTasks(userEmail);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
            var tasks = response.Value.Select((task, result) => new TaskModel(this, userEmail, task.emailAssignee, task.Title, task.Description, task.DueDate, task.CreationTime, task.Id));
            return new List<TaskModel>(tasks);
        }

        /// <summary>
        /// Assigns a task to a user
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>        
        /// <param name="emailAssignee">Email of the user to assign to task to</param>
        /// <exception cref="Exception"></exception>
        internal void AssignTask(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int taskId, string emailAssignee)
        {
            var response = serviceSingleton.AssignTask(userEmail, creatorEmail, boardName, columnOrdinal, taskId, emailAssignee);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
        }

        /// <summary>
        /// Returns the list of board of a user. The user must be logged-in. The function returns all the board names the user created or joined.
        /// </summary>
        /// <param name="userEmail">The email of the user. Must be logged-in.</param>
        /// <returns>An IList of String objects of all board names of user.</returns>
        /// <exception cref="Exception"></exception>
        internal IList<String> GetBoardNames(string userEmail)
        {
            var response = serviceSingleton.GetBoardNames(userEmail);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
            var boardNames = response.Value;
            return new List<String>(boardNames);
        }

        /// <summary>
        /// Adds a new column
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The location of the new column. Location for old columns with index>=columnOrdinal is increased by 1 (moved right). The first column is identified by 0, the location increases by 1 for each column.</param>
        /// <param name="columnName">The name for the new columns</param>        
        /// <exception cref="Exception"></exception>
        internal void AddColumn(string userEmail, string creatorEmail, string boardName, int columnOrdinal, string columnName)
        {
            var response = serviceSingleton.AddColumn(userEmail, creatorEmail, boardName, columnOrdinal, columnName);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
        }

        /// <summary>
        /// Removes a specific column
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column location. The first column location is identified by 0, the location increases by 1 for each column</param>
        /// <exception cref="Exception"></exception>
        internal void RemoveColumn(string userEmail, string creatorEmail, string boardName, int columnOrdinal)
        {
            var response = serviceSingleton.RemoveColumn(userEmail, creatorEmail, boardName, columnOrdinal);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
        }

        /// <summary>
        /// Renames a specific column
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column location. The first column location is identified by 0, the location increases by 1 for each column</param>
        /// <param name="newColumnName">The new column name</param>     
        /// <exception cref="Exception"></exception>
        internal void RenameColumn(string userEmail, string creatorEmail, string boardName, int columnOrdinal, string newColumnName)
        {
            var response = serviceSingleton.RenameColumn(userEmail, creatorEmail, boardName, columnOrdinal, newColumnName);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
        }

        /// <summary>
        /// Moves a column shiftSize times to the right. If shiftSize is negative, the column moves to the left
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column location. The first column location is identified by 0, the location increases by 1 for each column</param>
        /// <param name="shiftSize">The number of times to move the column, relativly to its current location. Negative values are allowed</param>  
        /// <exception cref="Exception"></exception>
        internal void MoveColumn(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int shiftSize)
        {
            var response = serviceSingleton.MoveColumn(userEmail, creatorEmail, boardName, columnOrdinal, shiftSize);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
        }

        /// <summary>
        /// Get all columns of a board.
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in.</param>
        /// <param name="creatorEmail">Email of the board creator.</param>
        /// <param name="boardName">Name of the board.</param>
        /// <returns>List of a board's columns ordered by ordinals.</returns>
        /// <exception cref="Exception"></exception>
        internal List<ColumnModel> GetColumns(string userEmail, string creatorEmail, string boardName)
        {
            var response = serviceSingleton.GetColumns(userEmail, creatorEmail, boardName);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
            var columns = response.Value.Select((column, result) => new ColumnModel(this, column.Name, column.Ordinal, GetColumn(userEmail, creatorEmail, boardName, column.Ordinal)));
            return new List<ColumnModel>(columns);
        }

        /// <summary>
        /// Get all boards the user created or is a member of.
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in.</param>
        /// <returns>All boards the user created or is a member of.</returns>
        /// <exception cref="Exception"></exception>
        internal List<BoardModel> GetBoards(string userEmail)
        {
            var response = serviceSingleton.GetBoards(userEmail);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
            var boards = response.Value.Select((board, result) => new BoardModel(this, userEmail, board.Creator, board.Name, board.Id, board.DoneOrdinal));
            return new List<BoardModel>(boards);
        }

        /// <summary>
        /// Get Board which Task belongs to.
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in.</param>
        /// <param name="taskId">Id of the Task.</param>
        /// <returns>Board the Task belongs to.</returns>
        /// <exception cref="Exception"></exception>
        public BoardModel GetTaskBoard(string userEmail, int taskId)
        {
            var response = serviceSingleton.GetTaskBoard(userEmail, taskId);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
            var board = response.Value;
            return new BoardModel(this, userEmail, board.Creator, board.Name, board.Id, board.DoneOrdinal);
        }

        /// <summary>
        /// Get Column which Task belongs to.
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in.</param>
        /// <param name="taskId">Id of the Task.</param>
        /// <returns>Column the Task belongs to.</returns>
        /// <exception cref="Exception"></exception>
        public int GetTaskOrdinal(string userEmail, int taskId)
        {
            var response = serviceSingleton.GetTaskColumn(userEmail, taskId);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
            return response.Value.Ordinal;
        }
    }
}
