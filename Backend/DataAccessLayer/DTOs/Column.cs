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
    internal class Column : DTO, IColumn
    {
        /// <summary>Column name of the Board id.</summary>
        public const string BoardIdColumn = "Board";
        /// <summary>Column name of the title.</summary>
        public const string TitleColumn = "Title";
        /// <summary>Column name of the ordinal.</summary>
        public const string OrdinalColumn = "Ordinal";
        // When limit is -1, there is no limit.
        /// <summary>Column name of the limit.</summary>
        public const string LimitColumn = "TaskLimit";

        private readonly ILog log = LogManager.GetLogger("piza");

        private int board;
        /// <summary>Parent Board id.</summary>
        public int Board
        {
            get => board;
            set
            {
                controller.Update(Id, BoardIdColumn, value.ToString());
                board = value;
                log.Debug("Update Column Board id.");
            }
        }

        private string title;
        /// <summary>Title.</summary>
        public string Title
        {
            get => title;
            set
            {
                controller.Update(Id, TitleColumn, value);
                title = value;
                log.Debug("Update Column name.");
            }
        }

        private int ordinal;
        /// <summary>Ordinal.</summary>
        public int Ordinal
        {
            get => ordinal;
            set
            {
                controller.Update(Id, OrdinalColumn, value.ToString());
                ordinal = value;
                log.Debug("Updated Column ordinal.");
            }
        }

        private int limit;
        /// <summary>Task Limit, unlimited when -1.</summary>
        public int Limit
        {
            get => limit;
            set
            {
                controller.Update(Id, LimitColumn, value.ToString());
                board = value;
                log.Debug("Update Column Task limit.");
            }
        }

        /// <summary>Creates a Column DTO.</summary>
        /// <param name="columns">Column controller to use.</param>
        /// <param name="id">Id of the Column.</param>
        /// <param name="board">Parent Board id.</param>
        /// <param name="ordinal">Ordinal of the Column.</param>
        /// <param name="title">Title of the Column.</param>
        /// <param name="limit">Task limit.</param>
        public Column(ColumnController columns, int id, int board, int ordinal, string title, int limit)
            : base(columns)
        {
            Id = id;
            this.board = board;
            this.title = title;
            this.ordinal = ordinal;
            this.limit = limit;
            log.Debug("Created Column DTO.");
        }
    }
}
