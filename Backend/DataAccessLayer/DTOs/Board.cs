using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using log4net;

[assembly: InternalsVisibleTo("Test")]
namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOs
{
    internal class Board : DTO
    {
        private readonly ILog log = LogManager.GetLogger("piza");

        /// <summary>The string name of creators emails column.</summary>
        public const string CreatorEmailColumn = "Creator";
        /// <summary>The string name of boards names column.</summary>
        public const string BoardNameColumn = "Name";
        /// <summary>The string name of this board.</summary>
        public readonly string boardName;
        /// <summary>The string creator's email of this board.</summary>
        public readonly string creatorEmail;

        /// <summary>
        /// Creates a new Board.
        /// </summary>
        /// <param name="boards">The responsible boardController Object</param>
        /// <param name="id">The unique key of the board</param>
        /// <param name="creatorEmail">The email of the creator of the board</param>
        /// <param name="boardName">The name of the board to create</param>
        /// <returns>A Board object.</returns>
        public Board(BoardController boards, int id, string creatorEmail, string boardName)
            : base(boards)
        {
            log.Debug("Try to create new data board object.");
            Id = id;
            this.boardName = boardName;
            this.creatorEmail = creatorEmail;
            log.Debug("Create new data board object successfully.");
        }
    }
}
