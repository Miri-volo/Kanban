using log4net;
using System;

namespace IntroSE.Kanban.Presentation.Model
{
    public class TaskModel : NotifiableModelObject, IEquatable<TaskModel>
    {
        private readonly ILog log = LogManager.GetLogger("piza");

        /// <summary>
        /// Get Email of current user.
        /// </summary>
        public string UserEmail { get; }
        /// <summary>
        /// Get the creation time of the task.
        /// </summary>
        public DateTime CreationTime { get; }
        /// <summary>
        /// Get the id key of the task.
        /// </summary>
        public int Id { get; }
        /// <summary>
        /// (Method, not property) Get the current ordinal of the column the task currently stated in.
        /// </summary>
        public int Ordinal
        {
            get
            {
                return Controller.GetTaskOrdinal(UserEmail, Id);
            }
        }
        /// <summary>
        /// (Method, not property) Get the Board the task belongs to.
        /// </summary>
        public BoardModel Board
        {
            get
            {
                return Controller.GetTaskBoard(UserEmail, Id);
            }
        }

        private string emailAssignee;
        /// <summary>
        /// Get the email of the assignee of the task.
        /// Set the assignee of the task by the assignee's email.
        /// </summary>
        public string EmailAssignee
        {
            get => emailAssignee;
            set
            {
                emailAssignee = value;
                RaisePropertyChanged("EmailAssignee");
            }
        }

        private string title;
        /// <summary>
        /// Get and set the task's title.
        /// </summary>
        public string Title
        {
            get => title;
            set
            {
                title = value;
                RaisePropertyChanged("Title");
            }
        }

        private string description;
        /// <summary>
        /// Get and set the task's description.
        /// </summary>
        public string Description
        {
            get => description;
            set
            {
                description = value;
                RaisePropertyChanged("Description");
            }
        }

        private DateTime dueDate;
        /// <summary>
        /// Get and set the task's due date.
        /// </summary>
        public DateTime DueDate
        {
            get => dueDate;
            set
            {
                dueDate = value;
                RaisePropertyChanged("DueDate");
            }
        }

        /// <summary>
        /// Create a new task model object.
        /// </summary>
        /// <param name="backendController">The connection service with the Backend layer.</param>
        /// <param name="userEmail">Email of the current user.</param>
        /// <param name="emailAssignee">Email of the user to assign to task to</param>
        /// <param name="title">Title of the task</param>
        /// <param name="description">Description of the task</param>
        /// <param name="dueDate">The due date of the task</param>
        /// <param name="creationTime">The creation date of the task</param>
        /// <param name="id">The ID of the task.</param>
        public TaskModel(BackendController backendController, string userEmail, string emailAssignee, string title, string description, DateTime dueDate, DateTime creationTime, int id) : base(backendController)
        {
            UserEmail = userEmail;
            EmailAssignee = emailAssignee;
            Title = title;
            Description = description;
            DueDate = dueDate;
            CreationTime = creationTime;
            Id = id;
            log.Debug("Created task model.");
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="other"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public bool Equals(TaskModel other)
        {
            log.Debug("Try to check equality between two task models.");
            return (this.Id == other.Id);
        }
    }
}