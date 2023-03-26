using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public struct Board
    {
        /// <summary>Board Id.</summary>
        public readonly int Id;
        /// <summary>Board name.</summary>
        public readonly string Name;
        /// <summary>Board creator's email.</summary>
        public readonly string Creator;
        /// <summary>Backlog column ordinal.</summary>
        public readonly int BacklogOrdinal;
        /// <summary>Done column ordinal.</summary>
        public readonly int DoneOrdinal;

        /// <summary>Service Board data transfer object.</summary>
        /// <param name="name">Board name.</param>
        /// <param name="creator">Board creator.</param>
        /// <param name="columns">Columns inside the Board.</param>
        /// <param name="members">Members of the Board.</param>
        internal Board(int id, string name, string creator, int backlogOrdinal, int doneOrdinal)
        {
            Id = id;
            Name = name;
            Creator = creator;
            BacklogOrdinal = backlogOrdinal;
            DoneOrdinal = doneOrdinal;
        }
    }
}
