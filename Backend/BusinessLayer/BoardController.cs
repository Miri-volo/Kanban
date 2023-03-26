using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using DalBoardController = IntroSE.Kanban.Backend.DataAccessLayer.BoardController;
using DalBoard = IntroSE.Kanban.Backend.DataAccessLayer.DTOs.Board;
using DalMembershipController = IntroSE.Kanban.Backend.DataAccessLayer.MembershipController;
using DalMembership = IntroSE.Kanban.Backend.DataAccessLayer.DTOs.Membership;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    /// <summary>
    /// Represents a BoardController which contains List of <c>Board</c>s.
    /// </summary>
    class BoardController
    {
        private readonly ILog log = LogManager.GetLogger("piza");
        /// <summary>Persistance BoardController object.</summary>
        private readonly DalBoardController dalBoardController;
        /// <summary>Persistance MembershipController object.</summary>
        private readonly DalMembershipController dalMembershipController;
        /// <summary>Persistance userController object.</summary>
        private readonly UserController userController;

        /// <summary>Boards created from this Board Controller or loaded to it.</summary>
        private List<Board> boards = new ();

        /// <summary>
        /// Returns a Board given it's boardName
        /// </summary>
        /// <param name="boardName">The name of the board to get</param>
        /// <returns>A Board object.</returns>
        /// <exception cref="KeyNotFoundException"> In case of non-existing boardName.</exception>
        public BoardController(UserController userController)
        {
            this.userController = userController;
            this.dalBoardController = new DalBoardController();
            this.dalMembershipController = new DalMembershipController();
        }

        ///<summary>This method loads the data of the boardController from the persistance.
        ///         Call function when the program starts. </summary>
        public void LoadController()
        {
            boards.Clear();
            log.Debug("Try to load BoardController from data.");
            this.dalBoardController.SelectAllBoards().ForEach(delegate (DalBoard dto)
            {
                Board board = new (dto);
                dalMembershipController.SelectAllBoardMemberships(board.boardId).ForEach(delegate (DalMembership dalMembership)
                {
                    board.Join(dalMembership.memberEmail);
                });
                boards.Add(board);
                log.Debug($"Load a Board {board.boardName} and its members from data successfully.");
            });
            log.Debug("Load BoardController successfully.");
        }

        ///<summary>Removes all persistent data.</summary>
        public void DeleteData()
        {
            log.Debug("Try to Delete all BoardController data.");
            dalBoardController.Delete();
            //Tell a column to delete all columns from data.
            Column column = new (0, 0, "delete");
            column.DeleteAllColumnsData();
            log.Debug($"Delete all BoardController data successfully.");

            log.Debug("Try to Delete all Memberships data.");
            dalMembershipController.Delete();
            log.Debug($"Delete all Memberships data successfully.");
        }
        /// <summary>
        /// Returns a Board given its boardName.
        /// </summary>
        /// <param name="boardName">The name of the board to get</param>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <returns>A Board object.</returns>
        /// <exception cref="UnauthorizedAccessException"> In case of user email is not a logged-in member or creator of board.</exception>
        /// <exception cref="Exception"> In case of non-existing boardName.</exception>
        public Board GetBoard(string userEmail, string creatorEmail, string boardName)
        {
            log.Debug($"Try to get Board {boardName}.");

            Board boardToGet = GetBoard(creatorEmail, boardName);
            //Verify access to board.
            if (creatorEmail.Equals(userEmail) || boardToGet.IsMember(userEmail))
            {
                VerifyBoardAccess(userEmail, "user", "get this board");
            }
            else
            {
                log.Error("User is not creator nor a member therefore can not get this board.");
                throw new UnauthorizedAccessException("Only the board's creator or members can get the board.");
            }
            log.Debug("Return board successfully.");
            return boardToGet;
        }

        /// <summary>
        /// Returns a Board given it's boardName without checking access.
        /// </summary>
        /// <param name="boardName">The name of the board to get</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <returns>A Board object.</returns>
        /// <exception cref="KeyNotFoundException"> In case of non-existing boardName.</exception>
        private Board GetBoard(string creatorEmail, string boardName)
        {
            log.Debug($"Try to get Board {boardName} by {creatorEmail} before checking access.");
            //Find board
            int boardToGetIndex = boards.FindIndex((Board board) => board.creatorEmail.Equals(creatorEmail) & board.boardName.Equals(boardName));
            //Verify this board exist.
            if (boardToGetIndex < 0)
            {
                log.Error("Board's name with creator's id combination is wrong or not exist.");
                throw new KeyNotFoundException($"Can not find a combination of board named '{boardName}' with creator's id '{creatorEmail}'.");
            }
            log.Debug("Return board without checking access successfully.");
            return boards[boardToGetIndex];
        }

        /// <summary>
        /// Adds a new board to the board controller.
        /// </summary>
        /// <param name="boardName">The name of the new board</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <returns>A Board object.</returns>
        /// <exception cref="ArgumentException"> In case of already existing boardName.</exception>
        public Board AddBoard(string creatorEmail, string boardName)
        {
            log.Debug($"Try to add board {boardName} to a user.");
            //verify creator identity.
            VerifyBoardAccess(creatorEmail, "user", "create and add board");
            //Verify this board exist.
            if (boards.Exists((Board board) => board.creatorEmail.Equals(creatorEmail) & board.boardName.Equals(boardName)))
            {
                log.Error("Board's name is already exist for this creator.");
                throw new ArgumentException($"There is already a board named '{boardName}' by this creator's id.");
            }
            //Create board and save to data
            Board boardToAdd = new Board(creatorEmail, boardName);
            JoinBoard(creatorEmail, boardToAdd);
            //Add created board to boards.
            boards.Add(boardToAdd);
            log.Debug("Add created new board. Return board successfully.");
            return boardToAdd;
        }

        /// <summary>
        /// Removes a board from the board controller.
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="boardName">The name of the board to remove</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <exception cref="Exception"> In case of failing varifying or getting board.</exception>
        /// <exception cref="UnauthorizedAccessException"> In case the user is not the board creator.</exception>
        public void RemoveBoard(string userEmail, string creatorEmail, string boardName)
        {
            log.Debug($"Try to Remove board {boardName}.");
            //verify creator identity.
            if (!userEmail.Equals(creatorEmail))
            {
                log.Error("User is not the creator therefore can not remove this board.");
                throw new UnauthorizedAccessException("Only the board's creator can remove the board.");
            }
            VerifyBoardAccess(userEmail, "user", "remove this board");

            //Save the board ment to be removed.
            Board boardToRemove = GetBoard(creatorEmail, boardName);
            //Remove board from boards.
            dalBoardController.Delete(dalBoardController.SelectBoard(boardToRemove.boardId));
            dalMembershipController.DeleteAllBoardMemberships(boardToRemove.boardId);
            boardToRemove.Remove();
            boards.Remove(boardToRemove);
            log.Debug("Find and remove board. Return removed board successfully.");
        }

        /// <summary>
        /// Join a memeber to board.
        /// </summary>
        /// <param name="userEmail">Email of the user to join.</param>
        /// <param name="creatorEmail">Email of the board creator.</param>
        /// <param name="boardName">The name of the board to remove.</param>
        /// <returns>A Board object.</returns>
        /// <exception cref="Exception"> In case of failling Joining user to board.</exception>
        public Board JoinBoard(string userEmail, string creatorEmail, string boardName)
        {
            log.Debug($"Try to Join board {boardName}.");
            //Find board and try to join.
            return JoinBoard(userEmail, GetBoard(creatorEmail, boardName));
        }
        /// <summary>
        /// Join a memeber to board after finding Board.
        /// </summary>
        /// <param name="userEmail">Email of the user to join.</param>
        /// <param name="boardToJoin">Board object to join to.</param>
        /// <returns>A Board object.</returns>
        /// <exception cref="ArgumentException"> In case of membership alrady exist in board.</exception>
        private Board JoinBoard(string userEmail, Board boardToJoin)
        {
            VerifyBoardAccess(userEmail, "user", "join the board.");
            //Verify userEmail is not already a member.
            if (boardToJoin.IsMember(userEmail))
            {
                log.Error("Membership for this board is already exist for user.");
                throw new ArgumentException($"There is already a membership for board named '{boardToJoin.boardName}' by this user.");
            }
            //Save membership in persistance.
            dalMembershipController.Create(boardToJoin.boardId, userEmail);
            //Add membership to board.
            boardToJoin.Join(userEmail);
            log.Debug($"Join board successfully.");
            return boardToJoin;
        }

        /// <summary>
        /// Returns all the In progress tasks for the user in the board controller.
        /// </summary>
        /// <returns>A list of tasks object. </returns>
        /// <exception cref="Exception"> In case of failing VerifyBoardAccess.</exception>
        public List<ITask> GetInProgressTasks(string assigneeEmail)
        {
            log.Debug("Try to get a list of in-progress tasks from this user.");
            //verify user identity.
            VerifyBoardAccess(assigneeEmail, "user", "get his in-progress tasks");
            //Create an empty list of tasks.
            List<ITask> inProgressTasksList = new ();
            //Collect all 'in progress' tasks from all boards and add to the created list.
            foreach (Board board in boards)
            {
                if (board.IsMember(assigneeEmail))
                {
                    inProgressTasksList.AddRange(board.GetInProgressTasksList(assigneeEmail));
                    log.Debug($"In loop. Add in-progress tasks from board {board.boardName} to list.");
                }
            }
            log.Debug("Return created list of tasks successfully.");
            return inProgressTasksList;
        }

        /// <summary>
        /// Returns the list of board of a user. The user must be logged-in. The function returns all the board names the user created or joined.
        /// </summary>
        /// <param name="userEmail">The email of the user. Must be logged-in.</param>
        /// <returns>A list of Strings of all board names by user.</returns>
        /// <exception cref="Exception"> In case of failling VerifyBoardAccess.</exception>
        public List<String> GetBoardNames(string userEmail)
        {
            log.Debug("Try to get a list of all board names by user.");
            return GetBoards(userEmail)
                .AsQueryable()
                .Select(board => board.boardName)
                .ToList();
        }

        /// <summary>
        /// Returns the list of board of a user. The user must be logged-in. The function returns all the boards the user created or joined.
        /// </summary>
        /// <param name="userEmail">The email of the user. Must be logged-in.</param>
        /// <returns>A list of all boards related to user.</returns>
        /// <exception cref="Exception"> In case of failling VerifyBoardAccess.</exception>
        public List<Board> GetBoards(string userEmail)
        {
            log.Debug("Try to get a list of all boards by user.");
            VerifyBoardAccess(userEmail, "user", "get his boards");
            return dalMembershipController.SelectAllMemberBoards(userEmail, dalBoardController)
                .AsQueryable()
                .Select(dalBoard => new Board(dalBoard))
                .ToList();
        }

        /// <summary>
        /// Verify user email is logged-in for Board access.
        /// </summary>
        /// <param name="userEmail">Email of the user to verify.</param>
        /// <param name="name">The name of the role of the user.</param>
        /// <param name="action">description of the action that can fail in string.</param>
        /// <exception cref="AccessViolationException"> In case of userEmail is not logged-in.</exception>
        private void VerifyBoardAccess(string userEmail, string name, string action)
        {
            log.Debug($"Try to varify if {userEmail} has access to {action}.");
            if (!userController.IsUserLogged(userEmail))
            {
                log.Error($"The {name} is not logged in therefore can not {action}.");
                throw new AccessViolationException($"Only logged in {name} can {action}.");
            }
            log.Debug($"{userEmail} has access.");
        }

        /// <summary>
        /// Get Column containing a Task.
        /// </summary>
        /// <param name="userEmail">The email of the user. Must be logged-in.</param>
        /// <param name="taskId">Task to look up.</param>
        /// <returns>Column containing the Task.</returns>
        public Column GetTaskColumn(string userEmail, int taskId)
        {
            VerifyBoardAccess(userEmail, "user", "lookup columns");
            return new Column(dalBoardController.SelectColumnByTask(taskId));
        }

        /// <summary>
        /// Get Board containing a Task.
        /// </summary>
        /// <param name="userEmail">The email of the user. Must be logged-in.</param>
        /// <param name="taskId">Task to look up.</param>
        /// <returns>Board containing the Task.</returns>
        public Board GetTaskBoard(string userEmail, int taskId)
        {
            VerifyBoardAccess(userEmail, "user", "lookup columns");
            return new Board(dalBoardController.SelectBoardByTask(taskId));
        }
    }
}
