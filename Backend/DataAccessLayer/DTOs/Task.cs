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
    internal class Task : DTO
    {
        public const string TitleColumn = "Title";
        public const string DescriptionColumn = "Description";
        public const string AssigneeColumn = "Assignee";
        public const string CreatedColumn = "Created";
        public const string DueColumn = "Due";
        public const string ColumnIdColumn = "Column";

        private readonly ILog log = LogManager.GetLogger("piza");

        private int column;
        /// <summary>Parent Column.</summary>
        public int Column
        {
            get => column;
            set
            {
                controller.Update(Id, ColumnIdColumn, value.ToString());
                column = value;
                log.Debug("Updated Task Column id.");
            }
        }

        private string title;
        /// <summary>Task title.</summary>
        public string Title
        {
            get => title;
            set
            {
                controller.Update(Id, TitleColumn, value);
                title = value;
                log.Debug("Updated Task title.");
            }
        }

        private string description;
        /// <summary>Task description.</summary>
        public string Description
        {
            get => description;
            set
            {
                controller.Update(Id, DescriptionColumn, value);
                description = value;
                log.Debug("Updated Task description.");
            }
        }

        private string assignee;
        /// <summary>Task assignee.</summary>
        public string Assignee
        {
            get => assignee;
            set
            {
                controller.Update(Id, AssigneeColumn, value);
                assignee = value;
                log.Debug("Updated Task assignee.");
            }
        }

        private DateTime due;
        /// <summary>Task due date.</summary>
        public DateTime Due
        {
            get => due;
            set
            {
                controller.Update(Id, DueColumn, value.ToString());
                due = value;
                log.Debug("Updated Task due date.");
            }
        }

        /// <summary>Task creation date.</summary>
        public readonly DateTime Created;

        /// <summary>Create a new Task DTO.</summary>
        /// <param name="tasks">TaskController to use.</param>
        /// <param name="id">Task id.</param>
        /// <param name="column">Parent Column id.</param>
        /// <param name="title">Task title.</param>
        /// <param name="description">Task description.</param>
        /// <param name="assignee">Task assignee.</param>
        /// <param name="created">Task creation date.</param>
        /// <param name="due">Task due date.</param>
        public Task(TaskController tasks, int id, int column, string title, string description, string assignee, DateTime created, DateTime due)
            : base(tasks)
        {
            Id = id;
            this.column = column;
            this.title = title;
            this.description = description;
            this.assignee = assignee;
            Created = created;
            this.due = due;
            log.Debug("Created Task DTO.");
        }
    }
}
