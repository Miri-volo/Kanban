using IntroSE.Kanban.Presentation.Model;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IntroSE.Kanban.Presentation.ViewModel
{
    /// <summary>
    /// A class representing the data context of <c>TaskModel</c> inside column in <c>BoardVM</c>.
    /// </summary>
    class TaskVM : NotifiableObject, IEquatable<TaskVM>
    {
        private readonly ILog log = LogManager.GetLogger("piza");

        internal const int opacityTrue = 100;
        internal const int opacityFalse = 0;
        internal const string defaultColor = "White";
        internal const string overdueColor = "Red";
        internal const string vergeColor = "Orange";
        internal const double vergePercent = 0.75;
        /// <summary>
        /// Get the id of the task representing
        /// </summary>
        public int Id { get; private set; }
        /// <summary>
        /// Get the 'select task from column' command.
        /// </summary>
        public ICommand Select { get; private set; }
        
        private TaskModel task;
        /// <summary>
        /// Get and set the task representing.
        /// </summary>
        public TaskModel Task
        {
            get => task;
            set
            {
                task = value;
                this.Id = task.Id;
                UpdateAsigneeBorder(task.UserEmail == task.EmailAssignee);
                UpdateTaskStateColor();
                RaisePropertyChanged("Task");
            }
        }
        /// <summary>
        /// Create an empty taskVM object.
        /// </summary>
        public TaskVM()
        {
            Select = new RelayCommand(OnSelectClick);
            log.Debug("Created task view model.");
        }

        private void OnSelectClick(object obj)
        {
            BoardVM board = (BoardVM)obj;
            board.SelectedTask = this;           
        }

        private int borderTaskUserAssign;
        /// <summary>
        /// Get and set the opacity of the border of the task.
        /// </summary>
        public int BorderTaskUserAssign
        {
            get => borderTaskUserAssign;
            set
            {
                borderTaskUserAssign = value;
                RaisePropertyChanged("BorderTaskUserAssign");
            }
        }

        private string backgroundTaskState;
        /// <summary>
        /// Get and set the background color of the task.
        /// </summary>
        public string BackgroundTaskState
        {
            get => backgroundTaskState;
            set
            {
                backgroundTaskState = value;
                RaisePropertyChanged("BackgroundTaskState");
            }
        }
        /// <summary>
        /// Determines if the border of the task should have 100%(true) or 0%(false) opacity.
        /// </summary>
        /// <param name="isVisible">100%(true) or 0%(false) visible</param>
        public void UpdateAsigneeBorder(bool isVisible)
        {
            BorderTaskUserAssign = isVisible ? opacityTrue : opacityFalse;
            log.Debug("Border of task updated.");

        }
        /// <summary>
        /// Updates the background color of the task accordig to it's current data.
        /// </summary>
        public void UpdateTaskStateColor()
        {
            var precent = (DateTime.Now - Task.CreationTime) / (Task.DueDate - Task.CreationTime);
            if (precent >= vergePercent)
            {
                BackgroundTaskState = (Task.Ordinal == Task.Board.LastColumnOrdinal) ? defaultColor : vergeColor;
            }
            else if (precent < vergePercent)
            {
                BackgroundTaskState = defaultColor;
            }
            else
            {
                BackgroundTaskState = (Task.Ordinal == Task.Board.LastColumnOrdinal) ? defaultColor : overdueColor;
            }
            log.Debug("Background color of the task updated.");

        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="other"><inheritdoc/>.</param>
        /// <returns><inheritdoc/></returns>
        public bool Equals(TaskVM other)
        {
            log.Debug("Try to check equality between two task VMs.");

            return this.Id == other.Id;
        }

        /// <summary>
        /// Helpful nested command class for this class.
        /// </summary>
        public class RelayCommand : ICommand
        {
            private Action<object> execute;
            private Func<object, bool> canExecute;

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
            {
                this.execute = execute;
                this.canExecute = canExecute;
            }

            public bool CanExecute(object parameter)
            {
                return this.canExecute == null || this.canExecute(parameter);
            }

            public void Execute(object parameter)
            {
                this.execute(parameter);
            }
        }
    }
}
