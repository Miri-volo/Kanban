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
    class Membership : DTO
    {
        private readonly ILog log = LogManager.GetLogger("piza");

        /// <summary>The string name of boards ids column.</summary>
        public const string BoardIdColumn = "Board";
        /// <summary>The string name of members emails column.</summary>
        public const string MemberEmailColumn = "Member";
        /// <summary>The unique key of the board in this membership.</summary>
        public readonly int boardId;
        /// <summary>The string email of the memeber in this membership.</summary>
        public readonly string memberEmail;

        /// <summary>
        /// Creates a new Membership.
        /// </summary>
        /// <param name="memberships">The responsible membershipController Object</param>
        /// <param name="id">The unique key of this membership</param>
        /// <param name="memberEmail">The email of the member in this membership</param>
        /// <param name="boardId">The unique key of the board in this membership.</param>
        /// <returns>A Membership object.</returns>
        public Membership(MembershipController memberships, int id, int boardId, string memberEmail)
            : base(memberships)
        {
            log.Debug("Try to create new data membership object.");
            Id = id;
            this.boardId = boardId;
            this.memberEmail = memberEmail;
            log.Debug("Create new data membership object successfully.");
        }

        /// <summary>
        /// Creates a new Membership.
        /// </summary>
        /// <param name="memberships">The responsible membershipController Object</param>
        /// <param name="id">The unique key of this membership</param>
        /// <param name="member">The member in this membership</param>
        /// <param name="board">The board in this membership.</param>
        /// <returns>A Membership object.</returns>
        public Membership(MembershipController memberships, int id, Board board, User member)
            : base(memberships)
        {
            log.Debug("Try to create new data membership object.");
            Id = id;
            this.boardId = board.Id;
            this.memberEmail = member.Email;
            log.Debug("Create new data membership object successfully.");
        }
    }
}

