using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.BusinessLayer;
using log4net;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    ///<summary>This class represents the behavior of <c>Board</c> in the ServiceLayer.</summary> 
    class BoardService
    {
        private readonly ILog log = LogManager.GetLogger("piza");
        private readonly BoardController boardController;

        public BoardService(BoardController boardController)
        {
            this.boardController = boardController;
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
        /// <returns>A response object with a value set to the Task, instead the response should contain a error message in case of an error</returns>
        public Response<Task> AddTask(string userEmail, string creatorEmail, string boardName, string title, string description, DateTime dueDate)
        {
            try
            {
                ServiceLayer.Task task = boardController.GetBoard(userEmail, creatorEmail, boardName).AddTask(userEmail, title, description, dueDate).ToServiceTask();
                log.Debug("Add and return task successfully.");
                return Response<Task>.FromValue(task);
            }
            catch (Exception e)
            {
                return Response<Task>.FromError(e.Message);
            }
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
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response UpdateTaskDueDate(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int taskId, DateTime dueDate)
        {
            try
            {
                boardController.GetBoard(userEmail, creatorEmail, boardName).UpdateTaskDueDate(taskId, columnOrdinal, dueDate, userEmail);
                log.Debug("Update due date of task successfully.");
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
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
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response UpdateTaskTitle(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int taskId, string title)
        {
            try
            {
                boardController.GetBoard(userEmail, creatorEmail, boardName).UpdateTaskTitle(taskId, columnOrdinal, title, userEmail);
                log.Debug("Update title of task successfully.");
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
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
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response UpdateTaskDescription(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int taskId, string description)
        {
            try
            {
                boardController.GetBoard(userEmail, creatorEmail, boardName).UpdateTaskDescription(taskId, columnOrdinal, description, userEmail);
                log.Debug("Update description of task successfully.");
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
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
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response AdvanceTask(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int taskId)
        {
            try
            {
                boardController.GetBoard(userEmail, creatorEmail, boardName).AdvanceTask(taskId, columnOrdinal, userEmail);
                log.Debug("Advanced task successfully.");
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        /// <summary>
        /// Creates a new board for the logged-in user.
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="boardName">The name of the new board</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response AddBoard(string userEmail, string boardName)
        {
            try
            {
                var board = boardController.AddBoard(userEmail, boardName);
                log.Debug("Add board successfully.");
                return Response<Board>.FromValue(new Board(board.boardId, board.boardName, board.creatorEmail, 0, board.DoneOrdinal));
            }
            catch (Exception e)
            {
                return Response<Board>.FromError(e.Message);
            }
        }

        /// <summary>
        /// Adds a board created by another user to the logged-in user. 
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the new board</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response JoinBoard(string userEmail, string creatorEmail, string boardName)
        {
            try
            {
                var board = boardController.JoinBoard(userEmail, creatorEmail, boardName);
                log.Debug("Join board successfully.");
                return Response<Board>.FromValue(new Board(board.boardId, board.boardName, board.creatorEmail, 0, board.DoneOrdinal));
            }
            catch (Exception e)
            {
                return Response<Board>.FromError(e.Message);
            }
        }

        /// <summary>
        /// Removes a board.
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response RemoveBoard(string userEmail, string creatorEmail, string boardName)
        {
            try
            {
                boardController.RemoveBoard(userEmail, creatorEmail, boardName);
                log.Debug("Remove board successfully.");
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }
        /// <summary>
        /// Returns all the in-progress tasks of the logged-in user is assigned to.
        /// </summary>
        /// <param name="userEmail">Email of the logged in user</param>
        /// <returns>A response object with a value set to the list of tasks, The response should contain a error message in case of an error</returns>
        public Response<IList<Task>> InProgressTasks(string userEmail)
        {
            try
            {
                var tasks = boardController.GetInProgressTasks(userEmail).Select(task => task.ToServiceTask());
                log.Debug("Return list of all in-progress tasks by this user successfully.");
                return Response<IList<Task>>.FromValue(tasks.ToList());
            }
            catch (Exception e)
            {
                return Response<IList<Task>>.FromError(e.Message);
            }
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
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response AssignTask(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int taskId, string emailAssignee)
        {
            try
            {
                boardController.GetBoard(userEmail, creatorEmail, boardName).AssignTask(taskId, columnOrdinal, emailAssignee);
                log.Debug("Assign task successfully.");
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        /// <summary>
        /// Returns the list of board of a user. The user must be logged-in. The function returns all the board names the user created or joined.
        /// </summary>
        /// <param name="userEmail">The email of the user. Must be logged-in.</param>
        /// <returns>A response object with a value set to the board, instead the response should contain a error message in case of an error</returns>
        public Response<IList<String>> GetBoardNames(string userEmail)
        {
            try
            {
                var boardNames = boardController.GetBoardNames(userEmail);
                log.Debug("Return list of all board names successfully.");
                return Response<IList<String>>.FromValue(boardNames);
            }
            catch (Exception e)
            {
                return Response<IList<String>>.FromError(e.Message);
            }
        }

        /// <summary>
        /// Returns a column given it's name
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>A response object with a value set to the Column, The response should contain a error message in case of an error</returns>
        public Response<IList<Task>> GetColumn(string userEmail, string creatorEmail, string boardName, int columnOrdinal)
        {
            log.Info($"GetColumn called by '{userEmail}' on board '{boardName}' for column {columnOrdinal}.");
            try
            {
                var board = boardController.GetBoard(userEmail, creatorEmail, boardName);
                var tasks = board.GetColumn(columnOrdinal).GetTasks().Select(task => task.ToServiceTask());
                return Response<IList<Task>>.FromValue(tasks.ToList());
            }
            catch (Exception e)
            {
                return Response<IList<Task>>.FromError(e.Message);
            }
        }

        /// <summary>
        /// Get the name of a specific column
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>The name of the column.</returns>
        public Response<string> GetColumnName(string userEmail, string creatorEmail, string boardName, int columnOrdinal)
        {
            log.Info($"GetColumnName called by '{userEmail}' on board '{boardName}' for column {columnOrdinal}.");
            try
            {
                string columnName = boardController.GetBoard(userEmail, creatorEmail, boardName).GetColumn(columnOrdinal).Name;
                return Response<string>.FromValue(columnName);
            }
            catch (Exception e)
            {
                return Response<string>.FromError(e.Message);
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
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response LimitColumn(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int limit)
        {
            log.Info($"LimitColumn called by '{userEmail}' on board '{boardName}' to limit column {columnOrdinal} to {limit} tasks.");
            try
            {
                BusinessLayer.Column column = boardController.GetBoard(userEmail, creatorEmail, boardName).GetColumn(columnOrdinal);
                // Handle magic value for removing column limit.
                if (limit == -1)
                {
                    column.RemoveTaskLimit();
                }
                else
                {
                    column.Limit = limit;
                }
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        /// <summary>
        /// Get the limit of a specific column
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>The limit of the column.</returns>
        public Response<int> GetColumnLimit(string userEmail, string creatorEmail, string boardName, int columnOrdinal)
        {
            log.Info($"GetColumnLimit called by '{userEmail}' on board '{boardName}' for column {columnOrdinal}.");
            try
            {
                BusinessLayer.Column column = boardController.GetBoard(userEmail, creatorEmail, boardName).GetColumn(columnOrdinal);
                return Response<int>.FromValue(column.Limit);
            }
            catch (InvalidOperationException)
            {
                // Return the -1 magic value for unlimited columns.
                return Response<int>.FromValue(-1);
            }
            catch (Exception e)
            {
                return Response<int>.FromError(e.Message);
            }
        }

        /// <summary>
        /// Adds a new column
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The location of the new column. Location for old columns with index>=columnOrdinal is increased by 1 (moved right). The first column is identified by 0, the location increases by 1 for each column.</param>
        /// <param name="columnName">The name for the new columns</param>        
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response AddColumn(string userEmail, string creatorEmail, string boardName, int columnOrdinal, string columnName)
        {
            try
            {
                boardController.GetBoard(userEmail, creatorEmail, boardName).AddColumn(columnOrdinal, columnName);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
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
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response RenameColumn(string userEmail, string creatorEmail, string boardName, int columnOrdinal, string newColumnName)
        {
            try
            {
                boardController.GetBoard(userEmail, creatorEmail, boardName).RenameColumn(columnOrdinal, newColumnName);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
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
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response MoveColumn(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int shiftSize)
        {
            try
            {
                boardController.GetBoard(userEmail, creatorEmail, boardName).MoveColumn(columnOrdinal, shiftSize);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        /// <summary>
        /// Removes a specific column
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column location. The first column location is identified by 0, the location increases by 1 for each column</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response RemoveColumn(string userEmail, string creatorEmail, string boardName, int columnOrdinal)
        {
            try
            {
                boardController.GetBoard(userEmail, creatorEmail, boardName).RemoveColumn(columnOrdinal);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        /// <summary>
        /// Get all columns of a board.
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in.</param>
        /// <param name="creatorEmail">Email of the board creator.</param>
        /// <param name="boardName">Name of the board.</param>
        /// <returns>List of a board's columns ordered by ordinals.</returns>
        public Response<List<Column>> GetColumns(string userEmail, string creatorEmail, string boardName)
        {
            log.Info($"GetColumns called.");
            try
            {
                List<Column> columns = boardController.GetBoard(userEmail, creatorEmail, boardName)
                    .Columns
                    .AsQueryable()
                    .Select(column => new Column(column.Name, column.Ordinal))
                    .ToList();
                return Response<List<Column>>.FromValue(columns);
            }
            catch (Exception e)
            {
                return Response<List<Column>>.FromError(e.Message);
            }
        }

        /// <summary>
        /// Get all boards the user created or is a member of.
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in.</param>
        /// <returns>All boards the user created or is a member of.</returns>
        public Response<List<Board>> GetBoards(string userEmail)
        {
            log.Info($"GetBoards called.");
            try
            {
                List<Board> boards = boardController.GetBoards(userEmail)
                    .AsQueryable()
                    .Select(board => new Board(board.boardId, board.boardName, board.creatorEmail, 0, board.DoneOrdinal))
                    .ToList();
                return Response<List<Board>>.FromValue(boards);
            }
            catch (Exception e)
            {
                return Response<List<Board>>.FromError(e.Message);
            }
        }

        /// <summary>
        /// Get Board which Task belongs to.
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in.</param>
        /// <param name="taskId">Id of the Task.</param>
        /// <returns>Board the Task belongs to.</returns>
        public Response<Board> GetTaskBoard(string userEmail, int taskId)
        {
            log.Info($"GetTaskColumn called.");
            try
            {
                var board = boardController.GetTaskBoard(userEmail, taskId);
                return Response<Board>.FromValue(new Board(board.boardId, board.boardName, board.creatorEmail, 0, board.DoneOrdinal));
            }
            catch (Exception e)
            {
                return Response<Board>.FromError(e.Message);
            }
        }

        /// <summary>
        /// Get Column which Task belongs to.
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in.</param>
        /// <param name="taskId">Id of the Task.</param>
        /// <returns>Column the Task belongs to.</returns>
        public Response<Column> GetTaskColumn(string userEmail, int taskId)
        {
            log.Info($"GetTaskColumn called.");
            try
            {
                var column = boardController.GetTaskColumn(userEmail, taskId);
                return Response<Column>.FromValue(new Column(column.Name, column.Ordinal));
            }
            catch (Exception e)
            {
                return Response<Column>.FromError(e.Message);
            }
            throw new NotImplementedException();
        }
    }
}
