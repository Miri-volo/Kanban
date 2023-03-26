using System;

using log4net;

using IntroSE.Kanban.Backend.DataAccessLayer;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    /// <summary>
    /// Represents a Task, which is usually stored inside a column.
    /// </summary>
    class Task : ITask
    {
        private readonly ILog log = LogManager.GetLogger("piza");

        private const int titleLimit = 50;
        private const int descriptionLimit = 300;

        private DataAccessLayer.DTOs.Task dto;

        private readonly int id;
        /// <summary>Gets the Id of the Task.</summary>
        /// <value>Task id.</value>
        public int Id
        {
            get => id;
        }

        private readonly DateTime created;
        /// <summary>Gets the creation DateTime of the Task.</summary>
        /// <value>Task creation DateTime.</value>
        public DateTime Created
        {
            get => created;
        }

        private string assignee;
        /// <summary>Assignee of the Task.</summary>
        /// <value>Task assignee's email.</value>
        public string Assignee
        {
            get => assignee;
            set
            {
                if (dto != null)
                {
                    dto.Assignee = value;
                }
                assignee = value;
            }
        }

        private DateTime due;
        /// <summary>Gets and sets the due DateTime of the Task.</summary>
        /// <value>Task due DateTime.</value>
        /// <exception cref="ArgumentException">When the new due date is before now.</exception>
        public DateTime Due
        {
            get => due;
            set
            {
                if (isDueDateValid(value))
                {
                    if (dto != null)
                    {
                        dto.Due = value;
                    }
                    due = value;
                    log.Debug($"Task {Id} Due set to {value}.");
                }
                else
                {
                    log.Error($"Task {Id} failed to set Due to {value} because it is invalid.");
                    throw new ArgumentException("Due date must be now or later.");
                }
            }
        }

        private string title;
        /// <summary>Gets and sets the title of the Task.</summary>
        /// <value>Task title.</value>
        /// <exception cref="ArgumentException">When the title is null, empty, or longer than 50 characters.</exception>
        public string Title
        {
            get => title;
            set
            {
                if (IsTitleValid(value))
                {
                    if (dto != null)
                    {
                        dto.Title = value;
                    }
                    title = value;
                    log.Debug($"Task {Id} Title set to '{value}'.");
                }
                else
                {
                    log.Error($"Task {Id} failed to set Title to '{value}' because it is invalid.");
                    throw new ArgumentException($"Title should be {titleLimit} characters or shorter, and not empty.");
                }
            }
        }

        private string description;
        /// <summary>Gets and sets the description of the Task.</summary>
        /// <value>Task description.</value>
        /// <exception cref="ArgumentException">When the description is longer than 300 characters.</exception>
        public string Description
        {
            get => description;
            set
            {
                if (IsDescriptionValid(value))
                {
                    if (dto != null)
                    {
                        dto.Description = value;
                    }
                    description = value;
                    log.Debug($"Task {Id} Description set to '{value}'.");
                } else
                {
                    log.Error($"Task {Id} failed to set Description to '{value}' because it is invalid.");
                    throw new ArgumentException($"Description should be {descriptionLimit} characters or shorter.");
                }
            }
        }

        ///<summary>Create a new Task.</summary>
        ///<param name="column">Column the Task is in.</param>
        ///<param name="title">Title of the Task.</param>
        ///<param name="description">Description of the Task.</param>
        ///<param name="assignee">Assignee of the Task.</param>
        ///<param name="due">Due date of the Task.</param>
        /// <exception cref="ArgumentException">When the description is longer than 300 characters.</exception>
        /// <exception cref="ArgumentException">When the title is null, empty, or longer than 50 characters.</exception>
        /// <exception cref="ArgumentException">When the due date is before now.</exception>
        ///<returns>New Task.</returns>
        public Task(int column, string title, string description, string assignee, DateTime due)
        {
            Due = due;
            Title = title;
            Description = description;
            Assignee = assignee;
            var controller = new TaskController();
            dto = controller.Create(column, title, description, assignee, due);
            created = dto.Created;
            id = dto.Id;
            log.Debug($"Task '{Id}' created.");
        }

        ///<summary>Load a Task.</summary>
        ///<param name="taskDTO">DTO to load Task from.</param>
        ///<returns>Loaded Task.</returns>
        public Task(DataAccessLayer.DTOs.Task taskDTO)
        {
            dto = taskDTO;
            // Should not validate due date because tasks with due dates in the past
            // exist in the database.
            // Should not validate other properties as well to prevent data loading
            // errors if field restrictions change in the future.
            due = dto.Due;
            title = dto.Title;
            description = dto.Description;
            assignee = dto.Assignee;
            created = dto.Created;
            id = dto.Id;
            log.Debug($"Task '{Id}' loaded from DTO.");
        }

        ///<summary>Remove Task data.</summary>
        public void Remove()
        {
            var taskController = new TaskController();
            taskController.Delete(dto);
            log.Debug($"Task '{Id}' removed.");
        }

        ///<summary>Set Column the Task belongs to.</summary>
        ///<param name="id">Id of the Column.</param>
        public void MoveColumn(int id)
        {
            dto.Column = id;
            log.Debug($"Task '{Id}' Column was set to '{id}'.");
        }

        ///<summary>Convert to ServiceLayer.Task.</summary>
        ///<returns>ServiceLayer.Task with the same information.</returns>
        public ServiceLayer.Task ToServiceTask()
        {
            log.Debug($"Converting Task {Id} to service layer task.");
            return new ServiceLayer.Task(Id, Created, Title, Description, Due, Assignee);
        }

        private bool IsTitleValid(string title)
        {
            if (title == null)
            {
                return false;
            }
            if (title.Length == 0)
            {
                return false;
            }
            if (titleLimit < title.Length)
            {
                return false;
            }
            return true;
        }

        private bool IsDescriptionValid(string description)
        {
            if (description == null)
            {
                return true;
            }
            if (descriptionLimit < description.Length)
            {
                return false;
            }
            return true;
        }

        private bool isDueDateValid(DateTime due)
        {
            if (due < DateTime.Now)
            {
                return false;
            }
            return true;
        }
    }
}
