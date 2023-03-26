using IntroSE.Kanban.Presentation.ViewModel;
using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace IntroSE.Kanban.Presentation.Model
{
    internal class ColumnModel : NotifiableModelObject
    {
        private readonly ILog log = LogManager.GetLogger("piza");

        /// <summary>
        /// Gets the ordinal int of the column.
        /// </summary>
        public int Ordinal { get; }

        private ObservableCollection<TaskVM> tasks;
        /// <summary>
        /// Gets and sets the observable collection of the columns tasks.
        /// </summary>
        public ObservableCollection<TaskVM> Tasks
        {
            get => tasks;
            set
            {
                tasks = value;
                RaisePropertyChanged("Tasks");
            }
        }

        private string name;
        /// <summary>
        /// Gets and sets the column's name.
        /// </summary>
        public string Name
        {
            get => name;
            set
            {
                name = value;
                RaisePropertyChanged("Name");
            }
        }

        /// <summary>
        /// Create a new column model object.
        /// </summary>
        /// <param name="backendController">The connection service with the Backend layer.</param>
        /// <param name="name">The column's name.</param>
        /// <param name="ordinal">The ordinal int of the column.</param>
        /// <param name="tasks">The IList of the columns tasks.</param>
        public ColumnModel(BackendController backendController, string name, int ordinal, IList<TaskModel> tasks) : base(backendController)
        {
            Name = name;
            Ordinal = ordinal;
            Tasks = new ObservableCollection<TaskVM>();
            foreach( TaskModel task in tasks){
                Tasks.Add(new TaskVM() { Task = task });
            }
            log.Debug("Created column model.");
        }

        /// <summary>
        /// Add already created and added task to data to the observable collection of tasks in column.
        /// </summary>
        /// <param name="task">Already created and added task to data to add to the observable collection of tasks in column.</param>
        public void AddTask(TaskModel task)
        {
            Tasks.Add(new TaskVM() { Task = task });
            log.Debug("Add task to column model.");
        }

        /// <summary>
        /// Remove already removed task from data from data the observable collection of tasks in column.
        /// </summary>
        /// <param name="task">Already removed task from data from data the observable collection of tasks in column.</param>
        public void RemoveTask(TaskModel task)
        {
            Tasks.Remove(new TaskVM() { Task = task });
            log.Debug("Remove task from column model.");
        }
    }
}