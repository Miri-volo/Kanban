using System;
using System.Collections.Generic;
using System.Linq;

using log4net;

using IntroSE.Kanban.Backend.DataAccessLayer;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    /// <summary>
    /// Represents a list of Tasks.
    /// </summary>
    class Column
    {
        private readonly ILog log = LogManager.GetLogger("piza");

        private const int unlimitedMagicValue = -1;

        private DataAccessLayer.DTOs.IColumn dto;

        private string name;
        /// <summary>Gets the name of the Column.</summary>
        /// <value>Column name.</value>
        public string Name
        {
            get => name;
            set
            {
                dto.Title = value;
                name = value;
                log.Debug("Updated Column name.");
            }
        }
        private Dictionary<int, ITask> tasks = new Dictionary<int, ITask>();
        private bool isLimited = false;
        private int limit;

        /// <summary>Id of the Column.</summary>
        /// <value>Id.</value>
        public readonly int Id;

        private int ordinal;
        public int Ordinal
        {
            get
            {
                return ordinal;
            }
            set
            {
                dto.Ordinal = value;
                log.Debug("Updated Column ordinal.");
                ordinal = value;
            }
        }

        public int Count
        {
            get => tasks.Count;
        }

        ///<summary>Create a Column.</summary>
        ///<param name="board">Board the column belongs to.</param>
        ///<param name="name">Name of the column.</param>
        ///<returns>New Column.</returns>
        public Column(int board, int ordinal, string name)
        {
            var controller = new ColumnController();
            dto = controller.Create(board, ordinal, name);
            Id = dto.Id;
            Name = name;
            limit = dto.Limit;
            this.ordinal = dto.Ordinal;
            log.Debug($"Column '{name}' created.");
        }

        ///<summary>Load a Column.</summary>
        ///<param name="columnDTO">DTO to load Column from.</param>
        ///<returns>Column.</returns>
        public Column(DataAccessLayer.DTOs.IColumn columnDTO)
        {
            dto = columnDTO;
            Id = dto.Id;
            Name = dto.Title;
            limit = dto.Limit;
            isLimited = dto.Limit != unlimitedMagicValue;
            ordinal = dto.Ordinal;

            var taskController = new TaskController();
            foreach (Task task in taskController.SelectColumn(Id).Select(dto => new Task(dto)))
            {
                tasks[task.Id] = task;
            }
            log.Debug($"Column '{Name}' loaded from DTO.");
        }

        public Column(DataAccessLayer.ColumnController controller, DataAccessLayer.DTOs.IColumn columnDTO)
        {
            dto = columnDTO;
            Id = dto.Id;
            Name = dto.Title;
            limit = dto.Limit;
            isLimited = dto.Limit != unlimitedMagicValue;
            ordinal = dto.Ordinal;
            log.Debug($"Column '{Name}' loaded from DTO.");
        }

        ///<summary>Delete data of all columns.</summary>
        public void DeleteAllColumnsData()
        {
            var columnController = new ColumnController();
            columnController.Delete();
            var taskController = new TaskController();
            taskController.Delete();
            log.Debug($"Deleted data of all columns.");
        }

        ///<summary>Delete data of the current column.</summary>
        public void Remove()
        {
            var controller = new ColumnController();
            controller.Delete((DataAccessLayer.DTOs.DTO) dto);
            var tasksList = tasks.AsQueryable()
                .Select((KeyValuePair<int, ITask> pair) => pair.Value)
                .ToList();
            foreach (Task task in tasksList)
            {
                task.Remove();
            }
            log.Debug($"Deleted Column '{Name}'.");
        }

        ///<summary>Get all tasks assigned to a user.</summary>
        ///<param name="assigneeEmail">Email of the assignee.</param>
        ///<returns>All tasks assigned to the user.</returns>
        public List<ITask> GetAssigneeTasks(string assigneeEmail)
        {
            return tasks.AsQueryable()
                .Select((KeyValuePair<int, ITask> pair) => pair.Value)
                .Where((ITask task) => task.Assignee == assigneeEmail)
                .ToList();
        }

        ///<summary>Get or set task limit.</summary>
        public int Limit
        {
            get {
                if (isLimited)
                {
                    return limit;
                }
                // Only log to debug because this is not an error in the service and
                // should not show up on the error stream.
                log.Debug($"Column '{Name}' GetTaskLimit called when no limit is set.");
                throw new InvalidOperationException("Column size is not limited.");
            }
            set
            {
                if (value <= 0)
                {
                    log.Error($"Setting task limit to invalid value '{value}' on Column '{Name}'.");
                    throw new ArgumentException("Limit must be bigger than 0.");
                }
                if (value < tasks.Count)
                {
                    log.Error($"Setting task limit to invalid value '{value}' on Column '{Name}'.");
                    throw new ArgumentException("Limit must be bigger than current task count.");
                }
                dto.Limit = value;
                isLimited = true;
                limit = value;
                log.Debug($"Task limit set to '{value}' on Column '{Name}'.");
            }
        }

        ///<summary>Removes the task limit.</summary>
        public void RemoveTaskLimit()
        {
            dto.Limit = unlimitedMagicValue;
            isLimited = false;
            log.Debug($"Task limi removed on Column '{Name}'.");
        }

        ///<summary>Get Tasks in the Column.</summary>
        ///<returns>List of Tasks in the Column.</returns>
        public List<ITask> GetTasks() {
            return tasks.Values.ToList();
        }

        ///<summary>Get Task from the Column.</summary>
        ///<param name="id">Id of the Task.</param>
        ///<exception cref="ArgumentException">When Id does not exist in the Column.</exception>
        ///<returns>Requested Task.</returns>
        public ITask GetTask(int id)
        {
            if (!tasks.ContainsKey(id))
            {
                log.Error($"GetTask called with id '{id}' that does not exist on Column '{Name}'.");
                throw new ArgumentException("Task must exist in the column.");
            }
            return tasks[id];
        }

        ///<summary>Add Task to the Column.</summary>
        ///<param name="task">Task to add.</param>
        ///<exception cref="ArgumentException">When task is null.</exception>
        ///<exception cref="ArgumentException">When Id already exists in the Column.</exception>
        ///<exception cref="InvalidOperationException">When task limit has been reached.</exception>
        ///<returns>Task that was added.</returns>
        public ITask AddTask(ITask task) {
            if (task == null)
            {
                log.Error($"Failed to add null task to Column '{Name}'.");
                throw new ArgumentException("Task must not be null.");
            }
            if (isLimited && limit <= tasks.Count)
            {
                log.Error($"Failed to add Task '{task.Id}' to Column '{Name}' because task limit has been reached.");
                throw new InvalidOperationException("Task limit has been reached.");
            }
            if (tasks.ContainsKey(task.Id))
            {
                log.Error($"Failed to add Task '{task.Id}' to Column '{Name}' because it already contains a task with the same id.");
                throw new ArgumentException("Task ids must be unique.");
            }
            tasks[task.Id] = task;
            task.MoveColumn(Id);
            log.Debug($"Added task '{task.Id}' to Column '{Name}'.");
            return task;
        }

        ///<summary>Remove Task from the Column.</summary>
        ///<param name="id">Id of the Task.</param>
        ///<exception cref="ArgumentException">When Id does not exist in the Column.</exception>
        ///<returns>Removed Task.</returns>
        public ITask RemoveTask(int id)
        {
            if (!tasks.ContainsKey(id))
            {
                log.Error($"Failed to remove Task '{id}' from Column '{Name}' because it does not exist.");
                throw new ArgumentException("Id must belong to a task in the column.");
            }
            ITask removed = tasks[id];
            tasks.Remove(id);
            log.Debug($"Removed task '{removed.Id}' from Column '{Name}'.");
            return removed;
        }

        /// <summary>
        /// Consume Tasks of another Column.
        /// </summary>
        /// <param name="other">Column to consume.</param>
        /// <exception cref="ArgumentException">When Task limit is reached.</exception>
        public void ConsumeColumn(Column other)
        {
            if (isLimited && limit < Count + other.Count)
            {
                throw new ArgumentException("Task limit reached.");
            }
            foreach (Task task in other.tasks.Values)
            {
                AddTask(task);
            }
            other.tasks.Clear();
            log.Debug("Consumed Column.");
        }
    }
}
