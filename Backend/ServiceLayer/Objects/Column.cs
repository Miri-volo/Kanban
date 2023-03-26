using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public struct Column
    {
        /// <summary>Column name.</summary>
        public readonly string Name;
        /// <summary>Column ordinal.</summary>
        public readonly int Ordinal;

        /// <summary>Service Column data transfer object.</summary>
        /// <param name="name">Column name.</param>
        /// <param name="limit">Task limit.</param>
        /// <param name="tasks">Tasks inside the Column.</param>
        internal Column(string name, int ordinal)
        {
            Name = name;
            Ordinal = ordinal;
        }
    }
}
