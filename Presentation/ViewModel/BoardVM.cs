using IntroSE.Kanban.Presentation.Model;
using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace IntroSE.Kanban.Presentation.ViewModel
{
    /// <summary>
    /// A class representing the data context for <c>BoardView</c> window.
    /// </summary>
    class BoardVM : NotifiableObject
    {
        private readonly ILog log = LogManager.GetLogger("piza");
        
        private const int BoardTab = 0;
        private const int TasksTab = 1;
        private const int notExist = -1;
        private const int sortByDueDateIndex = 1;
        private const int sortDefaultIndex = 0;

        private BackendController controller;

        /// <summary>
        /// Get and set the board list shown in left drawer. All boards the current user is membered in.
        /// </summary>
        public ObservableCollection<BoardModel> Boards { get; set; }
        /// <summary>
        /// Get email of current user.
        /// </summary>
        public string UserEmail { get; }

        private ObservableCollection<TaskVM> tasks;
        /// <summary>
        /// Get the tasks list of all in-progress tasks of current user.
        /// </summary>
        public ObservableCollection<TaskVM> Tasks
        {
            get => tasks;
            private set
            {
                tasks = value;
                this.FilteredTasks = CollectionViewSource.GetDefaultView(Tasks);
                RaisePropertyChanged("Tasks");
            }
        }

        private ICollectionView filteredTasks;
        /// <summary>
        /// Get the list of tasks shown in Tasks Tab, representing <seealso cref="Tasks"/>.
        /// </summary>
        public ICollectionView FilteredTasks
        {
            get => filteredTasks;
            private set
            {
                filteredTasks = value;
                RaisePropertyChanged("FilteredTasks");
            }
        }

        /// <summary>
        /// Get the input in the filter-tasks's text box. 
        /// </summary>
        public String FilterBar { get; set; }

        private int sortTasks;
        /// <summary>
        /// Get and set the index of sorting tasks way.
        /// <para><seealso cref="sortDefaultIndex"/> = 0</para>
        /// <para><seealso cref="sortByDueDateIndex"/> = 1</para>
        /// </summary>
        public int SortTasks
        {
            get => sortTasks;
            set
            {
                sortTasks = value;
                if (sortTasks == sortByDueDateIndex)
                {
                    if(BoardTab == TabIndex)
                    {
                        TasksShow();
                    }
                    FilteredTasks.SortDescriptions.Add(new SortDescription("Task.DueDate", ListSortDirection.Ascending));
                }
                else
                {
                    FilteredTasks.SortDescriptions.Clear();
                }
                RaisePropertyChanged("SortTasks");
            }
        }

        private ObservableCollection<ColumnModel> columns;
        /// <summary>
        /// Get the list of columns belongs to <see cref="ShowBoard"/> - shown in the Board Tab.
        /// </summary>
        public ObservableCollection<ColumnModel> Columns
        {
            get => columns;
            private set
            {
                columns = value;
                RaisePropertyChanged("Columns");
            }
        }

        private String message;
        /// <summary>
        /// Get and set the message projected in snack bar. Used for feedback to user.
        /// <para><see cref="MaterialDesignThemes.Wpf.SnackbarMessage"/></para>
        /// <para><see cref="MaterialDesignThemes.Wpf.Snackbar"/></para>
        /// </summary>
        public String Message
        {
            get => message;
            set
            {
                message = value;
                RaisePropertyChanged("Message");
            }
        }

        private bool showMessage;
        /// <summary>
        /// Get and set bool value to open or close snack bar. Used for feedback to user.
        /// True -> open.
        /// False -> close.
        /// <para><see cref="Message"/></para>
        /// <para><see cref="MaterialDesignThemes.Wpf.Snackbar"/></para>
        /// </summary>
        public bool ShowMessage
        {
            get => showMessage;
            set
            {
                showMessage = value;
                RaisePropertyChanged("ShowMessage");
            }
        }

        private int tabIndex;
        /// <summary>
        /// Get and set the current Tab by index.
        /// <para><see cref="BoardTab"/> = 0</para>
        /// <para><see cref="TasksTab"/> = 1</para>
        /// </summary>
        public int TabIndex
        {
            get => tabIndex;
            set
            {
                tabIndex = value;
                RaisePropertyChanged("TabIndex");
            }
        }

        private BoardModel shownBoard;
        /// <summary>
        /// Get the currently projected board data. Defind by last not null <see cref="SelectedBoard"/>.
        /// </summary>
        public BoardModel ShownBoard
        {
            get => shownBoard;
            private set
            {
                shownBoard = value;
                if (shownBoard == null)
                {
                    BoardNameEdit = "No board selected";
                    Columns.Clear();
                }
                else
                {
                    BoardNameEdit = shownBoard.BoardName;
                    Columns = new ObservableCollection<ColumnModel>(shownBoard.Columns);
                }
                RaisePropertyChanged("ShownBoard");
            }
        }

        private BoardModel selectedBoard;
        /// <summary>
        /// Get currently selected board from <see cref="Boards"/>.
        /// </summary>
        public BoardModel SelectedBoard
        {
            get => selectedBoard;
            set
            {
                selectedBoard = value;
                if (selectedBoard != null)
                {
                    ShownTask = null;
                    ShownColumn = null;
                    SortTasks = sortDefaultIndex;
                    TabIndex = BoardTab;
                    ShownBoard = selectedBoard;
                }
            }
        }

        private TaskVM shownTask;
        /// <summary>
        /// Get and set currently task that can be manipulated by UI features for user requirments.
        /// Defined mostly by last not null <see cref="SelectedTask"/>.
        /// </summary>
        public TaskVM ShownTask
        {
            get => shownTask;
            set
            {
                shownTask = value;
                if (shownTask == null)
                {
                    TaskNameEdit = "No task selected";
                }
                else
                {
                    TaskNameEdit = shownTask.Task.Title;
                }
                
                RaisePropertyChanged("ShownTask");
            }
        }

        private TaskVM selectedTask;
        /// <summary>
        /// Get currently selected task from <see cref="Tasks"/> or <see cref="ColumnModel.Tasks"/>.
        /// </summary>
        public TaskVM SelectedTask
        {
            get => selectedTask;
            set
            {
                
                selectedTask = value;
                if (selectedTask != null)
                {
                    if (TabIndex == TasksTab)
                    {
                        ShownBoard = selectedTask.Task.Board;
                    }
                    ShownColumn = ShownBoard.Columns[selectedTask.Task.Ordinal];
                    ShownTask = selectedTask;
                }
                selectedTask = null;
                RaisePropertyChanged("SelectedTask");
            }
        }

        private ColumnModel shownColumn;
        /// <summary>
        /// Get and set currently column that can be manipulated by UI features for user requirments.
        /// Defined mostly by last not null <see cref="SelectedColumn"/>.  
        /// </summary>
        public ColumnModel ShownColumn
        {
            get => shownColumn;
            set
            {
                shownColumn = value;
                if (shownColumn == null)
                {
                    ColumnNameEdit = "No column selected";
                }
                else
                {
                    ColumnNameEdit = shownColumn.Name;
                }
                RaisePropertyChanged("ShownColumn");
            }
        }

        private ColumnModel selectedColumn;
        /// <summary>
        /// Get currently selected column from <see cref="Columns"/>.
        /// </summary>
        public ColumnModel SelectedColumn
        {
            get => selectedColumn;
            set
            {
                selectedColumn = value;
                if (selectedColumn != null)
                {
                    ShownColumn = selectedColumn;
                    ShownTask = null;
                }
            }
        }

        private bool addColumnDialog;
        /// <summary>
        /// Get true or false if the dialog for 'add column' is open.
        /// </summary>
        public bool AddColumnDialog
        {
            get => addColumnDialog;
            private set
            {
                addColumnDialog = value;
                RaisePropertyChanged("AddColumnDialog");
            }
        }

        private bool shiftColumnDialog;
        /// <summary>
        /// Get true or false if the dialog for 'shift column' is open.
        /// </summary>
        public bool ShiftColumnDialog
        {
            get => shiftColumnDialog;
            private set
            {
                shiftColumnDialog = value;
                RaisePropertyChanged("ShiftColumnDialog");
            }
        }

        private bool renameColumnDialog;
        /// <summary>
        /// Get true or false if the dialog for 'rename column' is open.
        /// </summary>
        public bool RenameColumnDialog
        {
            get => renameColumnDialog;
            private set
            {
                renameColumnDialog = value;
                RaisePropertyChanged("RenameColumnDialog");
            }
        }

        private bool limitColumnDialog;
        /// <summary>
        /// Get true or false if the dialog for 'limit column' is open.
        /// </summary>
        public bool LimitColumnDialog
        {
            get => limitColumnDialog;
            private set
            {
                limitColumnDialog = value;
                RaisePropertyChanged("LimitColumnDialog");
            }
        }

        private bool addTaskDialog;
        /// <summary>
        /// Get true or false if the dialog for 'add task' is open.
        /// </summary>
        public bool AddTaskDialog
        {
            get => addTaskDialog;
            private set
            {
                addTaskDialog = value;
                RaisePropertyChanged("AddTaskDialog");
            }
        }

        private bool editTaskTitleDialog;
        /// <summary>
        /// Get true or false if the dialog for 'edit task title' is open.
        /// </summary>
        public bool EditTaskTitleDialog
        {
            get => editTaskTitleDialog;
            private set
            {
                editTaskTitleDialog = value;
                RaisePropertyChanged("EditTaskTitleDialog");
            }
        }

        private bool editTaskDescriptionDialog;
        /// <summary>
        /// Get true or false if the dialog for 'edit task description' is open.
        /// </summary>
        public bool EditTaskDescriptionDialog
        {
            get => editTaskDescriptionDialog;
            private set
            {
                editTaskDescriptionDialog = value;
                RaisePropertyChanged("EditTaskDescriptionDialog");
            }
        }

        private bool updateTaskDueDateDialog;
        /// <summary>
        /// Get true or false if the dialog for 'update task due date' is open.
        /// </summary>
        public bool UpdateTaskDueDateDialog
        {
            get => updateTaskDueDateDialog;
            private set
            {
                updateTaskDueDateDialog = value;
                RaisePropertyChanged("UpdateTaskDueDateDialog");
            }
        }

        private bool assignTaskDialog;
        /// <summary>
        /// Get true or false if the dialog for 'assign task' is open.
        /// </summary>
        public bool AssignTaskDialog
        {
            get => assignTaskDialog;
            private set
            {
                assignTaskDialog = value;
                RaisePropertyChanged("AssignTaskDialog");
            }
        }

        private string boardNameInput;
        /// <summary>
        /// Get the input of the name of the new board to add in the 'add board' text box section.
        /// </summary>
        public string BoardNameInput
        {
            get => boardNameInput;
            set
            {
                boardNameInput = value;
                RaisePropertyChanged("BoardNameInput");
            }
        }

        private string joinBoardNameInput;
        /// <summary>
        /// Get the input of the name of the board to join in the 'join board' text box section.
        /// </summary>
        public string JoinBoardNameInput
        {
            get => joinBoardNameInput;
            set
            {
                joinBoardNameInput = value;
                RaisePropertyChanged("JoinBoardNameInput");
            }
        }

        private string joinBoardEmailInput;
        /// <summary>
        /// Get the input of the creator email of the board to join in the 'join board' text box section.
        /// </summary>
        public string JoinBoardEmailInput
        {
            get => joinBoardEmailInput;
            set
            {
                joinBoardEmailInput = value;
                RaisePropertyChanged("JoinBoardEmailInput");
            }
        }

        private string boardNameEdit;
        /// <summary>
        /// Get the name of the board that can be manipulated currently. Shown in menu in the bottom of the board tab.
        /// </summary>
        public string BoardNameEdit
        {
            get => boardNameEdit;
            set
            {
                boardNameEdit = value;
                RaisePropertyChanged("BoardNameEdit");
            }
        }

        private string columnOrdinalInputAdd;
        /// <summary>
        /// Get the input of the ordinal of the new column to add in the 'add column' dialog.
        /// </summary>
        public string ColumnOrdinalInputAdd
        {
            get => columnOrdinalInputAdd;
            set
            {
                columnOrdinalInputAdd = value;
                RaisePropertyChanged("ColumnOrdinalInputAdd");
            }
        }

        private string columnNameInputAdd;
        /// <summary>
        /// Get the input of the name of the new column to add in the 'add column' dialog.
        /// </summary>
        public string ColumnNameInputAdd
        {
            get => columnNameInputAdd;
            set
            {
                columnNameInputAdd = value;
                RaisePropertyChanged("ColumnNameInputAdd");
            }
        }

        private string columnNameEdit;
        /// <summary>
        /// Get the name of the column that can be manipulated currently. Shown in menu in the bottom of the board tab.
        /// </summary>
        public string ColumnNameEdit
        {
            get => columnNameEdit;
            set
            {
                columnNameEdit = value;
                RaisePropertyChanged("ColumnNameEdit");
            }
        }

        private string shiftColumnInput;
        /// <summary>
        /// Get the input of how many times to shift ordinal of the shown column in the 'shift column' dialog.
        /// </summary>
        public string ShiftColumnInput
        {
            get => shiftColumnInput;
            set
            {
                shiftColumnInput = value;
                RaisePropertyChanged("ShiftColumnInput");
            }
        }

        private string columnNameInputEdit;
        /// <summary>
        /// Get the input of the new name of the shown column in the 'rename column' dialog.
        /// </summary>
        public string ColumnNameInputEdit
        {
            get => columnNameInputEdit;
            set
            {
                columnNameInputEdit = value;
                RaisePropertyChanged("ColumnNameInputEdit");
            }
        }

        private string columnLimitInput;
        /// <summary>
        /// Get the input of the new limit of the shown column in the 'limit column' dialog.
        /// </summary>
        public string ColumnLimitInput
        {
            get => columnLimitInput;
            set
            {
                columnLimitInput = value;
            }
        }

        private string taskNameEdit;
        /// <summary>
        /// Get the name of the task that can be manipulated currently. Shown in menu in the bottom of both tabs.
        /// </summary>
        public string TaskNameEdit
        {
            get => taskNameEdit;
            set
            {
                taskNameEdit = value;
                RaisePropertyChanged("TaskNameEdit");
            }
        }

        private string taskTitleInputAdd;
        /// <summary>
        /// Get the input of the title of the new task to add in the 'add task' dialog.
        /// </summary>
        public string TaskTitleInputAdd
        {
            get => taskTitleInputAdd;
            set
            {
                taskTitleInputAdd = value;
                RaisePropertyChanged("TaskTitleInputAdd");
            }
        }

        private string editTaskTitleInput;
        /// <summary>
        /// Get the input of the new title of the shown task in the 'edit task title' dialog.
        /// Set what shown in the text box.
        /// </summary>
        public string EditTaskTitleInput
        {
            get => editTaskTitleInput;
            set
            {
                editTaskTitleInput = value;
                RaisePropertyChanged("EditTaskTitleInput");
            }
        }

        private string taskDescriptionInputAdd;
        /// <summary>
        /// Get the input of the description of the new task to add in the 'add task' dialog.
        /// </summary>
        public string TaskDescriptionInputAdd
        {
            get => taskDescriptionInputAdd;
            set
            {
                taskDescriptionInputAdd = value;
                RaisePropertyChanged("TaskDescriptionInputAdd");
            }
        }

        private string editTaskDescriptionInput;
        /// <summary>
        /// Get the input of the new description of the shown task in the 'edit task description' dialog.
        /// Set what shown in the text box.
        /// </summary>
        public string EditTaskDescriptionInput
        {
            get => editTaskDescriptionInput;
            set
            {
                editTaskDescriptionInput = value;
                RaisePropertyChanged("EditTaskDescriptionInput");
            }
        }
        
        private string selectedDueDateAdd;
        /// <summary>
        /// Get the input of the due date of the new task to add in the 'add task' dialog.
        /// </summary>
        public string SelectedDueDateAdd
        {
            get => selectedDueDateAdd;
            set
            {
                selectedDueDateAdd = value;
                RaisePropertyChanged("SelectedDueDateAdd");
            }
        }

        private string selectedDueDateUpdate;
        /// <summary>
        /// Get the input of the new due date of the shown task in the 'update task due date' dialog.
        /// </summary>
        public string SelectedDueDateUpdate
        {
            get => selectedDueDateUpdate;
            set
            {
                selectedDueDateUpdate = value;
                RaisePropertyChanged("SelectedDueDateUpdate");
            }
        }

        private string assignTaskInput;
        /// <summary>
        /// Get the input of the email of the new assignee to assign to the shown task in the 'assign task' dialog.
        /// </summary>
        public string AssignTaskInput
        {
            get => assignTaskInput;
            set
            {
                assignTaskInput = value;
                RaisePropertyChanged("AssignTaskInput");
            }
        }

        /// <summary>
        /// Create a new data context class for board.
        /// </summary>
        /// <param name="userEmail">The email of current logged-in user.</param>
        public BoardVM(string userEmail)
        {
            this.controller = new BackendController();
            this.Columns = new ObservableCollection<ColumnModel>();
            this.UserEmail = userEmail;
            this.Boards = new ObservableCollection<BoardModel>(controller.GetBoards(userEmail));
            ResetIO();
            log.Debug("Created board data context.");
        }

        /// <summary>
        /// Resets some input values and states.
        /// </summary>
        internal void ResetIO()
        {
            Message = "";
            TasksShow();
            BoardNameInput = "";
            BoardNameEdit = "";
            ColumnNameInputAdd = "";
            log.Debug("Reset some input values and states.");
        }

        /// <summary>
        /// Filter the Tasks list according to FilterBar.
        /// </summary>
        internal void FilterTasks()
        {
            if (TabIndex == BoardTab)
            {
                TasksShow();
            }
            FilteredTasks.Filter = task => (String.IsNullOrWhiteSpace(FilterBar)) ? true : (((TaskVM)task).Task.Title.Contains(FilterBar) || ((TaskVM)task).Task.Description.Contains(FilterBar));
            log.Debug("Filter the Tasks list according to FilterBar.");
        }
        /// <summary>
        /// Go to Board Tab, Showing current ShownBoard.
        /// </summary>
        /// <returns>If the method run was successful or not</returns>
        internal bool ShowBoard()
        {
            try
            {
                ShownBoard = (ShownTask == null) ? ShownBoard : ShownTask.Task.Board;
                ShownColumn = null;
                ShownTask = null;
                SortTasks = sortDefaultIndex;
                TabIndex = BoardTab;
                log.Debug("Show current ShownBoard.");
                return true;
            }
            catch (Exception e)
            {
                Message = "Cannot show board. " + e.Message;
                ShowMessage = true;
                log.Debug("Show negative feedback to user.");
                return false;
            }
        }
        /// <summary>
        /// Reset and show Tasks tab.
        /// </summary>
        /// <returns>If the method run was successful or not</returns>
        internal bool TasksShow()
        {
            try
            {
                Tasks = new ObservableCollection<TaskVM>(controller.InProgressTasks(UserEmail).Select((task) => new TaskVM() { Task = task }));
                
                if (FilteredTasks != null)
                {
                    FilteredTasks.Filter = null;
                }
                TabIndex = TasksTab;
                log.Debug("Reset and show Tasks tab.");
                return true;
            }
            catch (Exception e)
            {
                Message = "Cannot show tasks. " + e.Message;
                ShowMessage = true;
                log.Debug("Show negative feedback to user.");
                return false;
            }
        }
        /// <summary>
        /// Add board to system through input from 'add board' section in left drawer.
        /// </summary>
        internal void AddBoard()
        {
            try
            {
                BoardModel board = controller.AddBoard(UserEmail, BoardNameInput);
                Boards.Add(board);
                BoardNameInput = "";
                ShownBoard = board;
                Message = "Added board! You can see it on the list now.";
                log.Debug("Add board to system.");

            }
            catch (Exception e)
            {
                Message = "Cannot add board. " + e.Message;
                log.Debug("Try to show negative feedback to user.");
            }
            finally
            {
                ShowMessage = true;
                log.Debug("Show feedback to user.");

            }
        }
        /// <summary>
        /// Add membership to system through input from 'join board' section in left drawer.
        /// </summary>
        internal void JoinBoard()
        {
            try
            {
                BoardModel board = controller.JoinBoard(UserEmail, JoinBoardEmailInput, JoinBoardNameInput);
                Boards.Add(board);
                JoinBoardNameInput = "";
                ShownBoard = board;
                Message = "Joined board! You can see it on the list now.";
                log.Debug("Add membership to system.");

            }
            catch (Exception e)
            {
                Message = "Cannot Join board. " + e.Message;
                log.Debug("Try to show negative feedback to user.");
            }
            finally
            {
                ShowMessage = true;
                log.Debug("Show feedback to user.");

            }
        }
        /// <summary>
        /// Remove shown board from system.
        /// </summary>
        internal void RemoveBoard()
        {
            try
            {
                if (ShownBoard == null)
                {
                    log.Error("Shown board is null.");

                    throw new Exception("No board selected!");
                }

                controller.RemoveBoard(UserEmail, ShownBoard.CreatorEmail, ShownBoard.BoardName);
                Boards.Remove(ShownBoard);
                Message = "Removed board! You can see it on the list now.";
                ShownBoard = null;
                log.Debug("Remove board from system.");

            }
            catch (Exception e)
            {
                Message = "Cannot Remove board. " + e.Message;
                log.Debug("Try to show negative feedback to user.");
            }
            finally
            {
                ShowMessage = true;
                log.Debug("Show feedback to user.");
            }
        }
        /// <summary>
        /// Open 'add column' dialog.
        /// </summary>
        internal void AddColumnMenu()
        {
            try
            {
                if (ShownBoard == null)
                {
                    log.Error("Shown board is null.");

                    throw new Exception("No board selected!");
                }

                AddColumnDialog = true;
                log.Debug("Open dialog.");
            }
            catch (Exception e)
            {
                Message = "Cannot add column. " + e.Message;
                ShowMessage = true;
                log.Debug("Show negative feedback to user.");
            }
        }
        /// <summary>
        /// Add column to shown board and system through input from 'add column' dialog.
        /// </summary>
        internal void AddColumn()
        {
            try
            {
                ShownBoard.AddColumn(IntInputValidation(ColumnOrdinalInputAdd), ColumnNameInputAdd);
                this.Columns = new ObservableCollection<ColumnModel>(ShownBoard.Columns);
                Message = "Added Column! You can see it on the board now.";
                ShownColumn = null;
                AddColumnDialog = false;
                log.Debug("Add column to system.");

            }
            catch (Exception e)
            {
                Message = "Cannot add column. " + e.Message;
                log.Debug("Try to show negative feedback to user.");
            }
            finally
            {
                ShowMessage = true;
                log.Debug("Show feedback to user.");

            }
        }
        /// <summary>
        /// Remove shown column from shown board and system.
        /// </summary>
        internal void RemoveColumn()
        {
            try
            {
                if (ShownColumn == null)
                {
                    log.Error("Shown column is null.");

                    throw new Exception("No column selected!");
                }

                ShownBoard.RemoveColumn(ShownColumn.Ordinal);
                Message = "Removed column! You can see it on the board now.";
                Columns = new ObservableCollection<ColumnModel>(ShownBoard.Columns);
                ShownColumn = null;
                log.Debug("Remove column from system.");

            }
            catch (Exception e)
            {
                Message = "Cannot Remove column. " + e.Message;
                log.Debug("Try to show negative feedback to user.");
            }
            finally
            {
                ShowMessage = true;
                log.Debug("Show feedback to user.");

            }
        }
        /// <summary>
        /// shift shown column in shown board through input from 'shift column' dialog.
        /// </summary>
        internal void ShiftColumn()
        {
            try
            {
                if (ShownColumn == null)
                {
                    log.Error("Shown column is null.");

                    throw new Exception("No column selected!");
                }

                int ordinal = ShownColumn.Ordinal;
                ShownBoard.ShiftColumn(ordinal, IntInputValidation(ShiftColumnInput));
                this.Columns = new ObservableCollection<ColumnModel>(ShownBoard.Columns);
                ShownColumn = Columns[ordinal + IntInputValidation(ShiftColumnInput)];
                Message = "Shifted Column! You can see it on the board now.";
                ShiftColumnDialog = false;
                log.Debug("shift column in board.");

            }
            catch (Exception e)
            {
                Message = "Cannot shift column. " + e.Message;
                log.Debug("Try to show negative feedback to user.");
            }
            finally
            {
                ShowMessage = true;
                log.Debug("Show feedback to user.");

            }
        }
        /// <summary>
        /// Open 'shift column' dialog.
        /// </summary>
        internal void ShiftColumnMenu()
        {
            try
            {
                if (ShownColumn == null)
                {
                    log.Error("Shown column is null.");

                    throw new Exception("No column selected!");
                }

                ShiftColumnDialog = true;
                log.Debug("Open dialog.");

            }
            catch (Exception e)
            {
                Message = "Cannot shift column. " + e.Message;
                ShowMessage = true;
                log.Debug("Show negative feedback to user.");
            }
        }
        /// <summary>
        /// Rename shown column through input from 'rename column' dialog.
        /// </summary>
        internal void RenameColumn()
        {
            try
            {
                ShownBoard.RenameColumn(ShownColumn, ColumnNameInputEdit);
                ColumnNameEdit = ShownColumn.Name;
                Message = "Renamed Column! You can see it on the board now.";
                RenameColumnDialog = false;
                log.Debug("Rename column.");

            }
            catch (Exception e)
            {
                Message = "Cannot rename column. " + e.Message;
                log.Debug("Try to show negative feedback to user.");
            }
            finally
            {
                ShowMessage = true;
                log.Debug("Show feedback to user.");

            }
        }
        /// <summary>
        /// Open 'rename column' dialog.
        /// </summary>
        internal void RenameColumnMenu()
        {
            try
            {
                if (ShownColumn == null)
                {
                    log.Error("Shown column is null.");

                    throw new Exception("No column selected!");
                }

                ColumnNameInputEdit = ShownColumn.Name;
                RenameColumnDialog = true;
                log.Debug("Open dialog.");

            }
            catch (Exception e)
            {
                Message = "Cannot rename column. " + e.Message;
                ShowMessage = true;
                log.Debug("Show negative feedback to user.");
            }
        }
        /// <summary>
        /// Change the limit of tasks in shown column through input from 'limit column' dialog.
        /// </summary
        internal void LimitColumn()
        {
            try
            {
                ShownBoard.LimitColumn(ShownColumn.Ordinal, IntInputValidation(ColumnLimitInput));
                Message = "Limited column! You can get it via Options Bar where the column's name is.";
                LimitColumnDialog = false;
                log.Debug("Limit column.");
            }
            catch (Exception e)
            {
                Message = "Cannot limit column. " + e.Message;
                log.Debug("Try to show negative feedback to user.");
            }
            finally
            {
                ShowMessage = true;
                log.Debug("Show feedback to user.");

            }
        }
        /// <summary>
        /// Open 'limit column' dialog.
        /// </summary>
        internal void LimitColumnMenu()
        {
            try
            {
                if (ShownColumn == null)
                {
                    log.Error("Shown column is null.");

                    throw new Exception("No column selected!");
                }

                LimitColumnDialog = true;
                log.Debug("Open dialog.");

            }
            catch (Exception e)
            {
                Message = "Cannot limit column. " + e.Message;
                ShowMessage = true;
                log.Debug("Show negative feedback to user.");
            }
        }
        /// <summary>
        /// Show the limit of the shown column in the snack bar with a relevant message.
        /// </summary>
        internal void GetColumnLimit()
        {
             try
            {
                if (ShownColumn == null)
                {
                    log.Error("Shown column is null.");

                    throw new Exception("No column selected!");
                }

                int limit = controller.GetColumnLimit(UserEmail, ShownBoard.CreatorEmail, ShownBoard.BoardName, ShownColumn.Ordinal);
                Message = (limit == notExist)
                    ? $"There is not tasks limit to column '{ShownColumn.Name}'." 
                    : $"Column '{ShownColumn.Name}' tasks limit is {limit}.";
                log.Debug("Get the limit of the column.");

            }
            catch (Exception e)
            {
                Message = "Cannot get column tasks limit. " + e.Message;
                log.Debug("Try to show negative feedback to user.");
            }
            finally
            {
                ShowMessage = true;
                log.Debug("Show negative feedback to user.");

            }
        }
        /// <summary>
        /// Add task to shown column and system through input from 'add task' dialog.
        /// </summary
        internal void AddTask()
        {
            try
            {
                if (SelectedDueDateAdd == null)
                {
                    log.Error("SelectedDueDateAdd is null.");

                    throw new Exception("Input is not in date format!");
                }

                ShownBoard.AddTask(TaskTitleInputAdd, TaskDescriptionInputAdd, DateTime.Parse(SelectedDueDateAdd));
                Message = "Added task! You can see it on the board now.";
                AddTaskDialog = false;
                log.Debug("Add task to system.");

            }
            catch (Exception e)
            {
                Message = "Cannot add task. " + e.Message;
                log.Debug("Try to show negative feedback to user.");
            }
            finally
            {
                ShowMessage = true;
                log.Debug("Show negative feedback to user.");

            }
        }
        /// <summary>
        /// Open 'add task' dialog.
        /// </summary>
        internal void AddTaskMenu()
        {
            try
            {
                if (ShownBoard == null)
                {
                    log.Error("Shown board is null.");

                    throw new Exception("No board selected!");
                }

                AddTaskDialog = true;
                log.Debug("Open dialog.");

            }
            catch (Exception e)
            {
                Message = "Cannot add task. " + e.Message;
                ShowMessage = true;
                log.Debug("Show negative feedback to user.");
            }
        }
        /// <summary>
        /// Logout current user with a relevant reaction.
        /// </summary>
        /// <returns>If the method run was successful or not.</returns>
        internal bool Logout()
        {
            Message = null;
            try
            {
                controller.Logout(UserEmail);
                log.Debug("Logout user from system.");

                return true;
            }
            catch (Exception e)
            {
                Message = "Cannot logout. " + e.Message;
                ShowMessage = true;
                log.Debug("Show negative feedback to user.");
                return false;
            }
        }
        /// <summary>
        /// Move shown task to the next column.
        /// </summary>
        internal void AdvanceTask()
        {
            try
            {
                if (ShownTask == null)
                {
                    log.Error("Shown task is null.");

                    throw new Exception("No task selected!");
                }

                ShownBoard.AdvanceTask(ShownTask.Task, ShownColumn, Columns[ShownColumn.Ordinal + 1]);
                if (TabIndex == TasksTab && ShownBoard.LastColumnOrdinal == ShownColumn.Ordinal + 1)
                {
                    Tasks.Remove(ShownTask);
                }
                ShownTask.UpdateTaskStateColor();
                ShownColumn = null;
                ShownTask = null;
                Message = "Advanced task! You can see changes on the board now.";
                log.Debug("Advance task to the next column.");

            }
            catch (Exception e)
            {
                Message = "Cannot advance task. " + e.Message;
                log.Debug("Try to show negative feedback to user.");
            }
            finally
            {
                ShowMessage = true;
                log.Debug("Show negative feedback to user.");

            }
        }

        /// <summary>
        /// Change the title of shown task through input from 'edit task title' dialog.
        /// </summary>
        internal void EditTaskTitle()
        {
            try
            {
                controller.UpdateTaskTitle(UserEmail, ShownBoard.CreatorEmail, ShownBoard.BoardName, ShownColumn.Ordinal, ShownTask.Task.Id, EditTaskTitleInput);
                ShownTask.Task.Title = EditTaskTitleInput;
                TaskNameEdit = ShownTask.Task.Title;
                Message = "Task's title changed! You can see it on it's current column now.";
                EditTaskTitleDialog = false;
                log.Debug("Change the title of the task.");

            }
            catch (Exception e)
            {
                Message = "Cannot edit task. " + e.Message;
                log.Debug("Try to show negative feedback to user.");
            }
            finally
            {
                ShowMessage = true;
                log.Debug("Show negative feedback to user.");

            }
        }
        /// <summary>
        /// Open 'edit task title' dialog.
        /// </summary>
        internal void EditTaskTitleMenu()
        {
            try
            {
                if (ShownTask == null)
                {
                    log.Error("Shown task is null.");

                    throw new Exception("No task selected!");
                }
                EditTaskTitleInput = ShownTask.Task.Title;
                EditTaskTitleDialog = true;
                log.Debug("Open dialog.");

            }
            catch (Exception e)
            {
                Message = "Cannot edit task. " + e.Message;
                ShowMessage = true;
                log.Debug("Show negative feedback to user.");
            }
        }
        /// <summary>
        /// Change the description of shown task through input from 'edit task description' dialog.
        /// </summary>
        internal void EditTaskDecription() 
        {
            try
            {
                controller.UpdateTaskDescription(UserEmail, ShownBoard.CreatorEmail, ShownBoard.BoardName, ShownColumn.Ordinal, ShownTask.Task.Id, EditTaskDescriptionInput);
                ShownTask.Task.Description = EditTaskDescriptionInput;
                Message = "Task's description changed! You can see it on it's current column now.";
                EditTaskDescriptionDialog = false;
                log.Debug("Change the description of the task.");

            }
            catch (Exception e)
            {
                Message = "Cannot edit task. " + e.Message;
                log.Debug("Try to show negative feedback to user.");
            }
            finally
            {
                ShowMessage = true;
                log.Debug("Show negative feedback to user.");

            }
        }

        /// <summary>
        /// Open 'edit task description' dialog.
        /// </summary>
        internal void EditTaskDecriptionMenu()
        {
            try
            {
                if (ShownTask == null)
                {
                    log.Error("Shown task is null.");

                    throw new Exception("No task selected!");
                }
                EditTaskDescriptionInput = ShownTask.Task.Description;
                EditTaskDescriptionDialog = true;
                log.Debug("Open dialog.");

            }
            catch (Exception e)
            {
                Message = "Cannot edit task. " + e.Message;
                ShowMessage = true;
                log.Debug("Show negative feedback to user.");
            }
        }
        /// <summary>
        /// Change the due date of shown task through input from 'update task due date' dialog.
        /// </summary>
        internal void UpdateTaskDueDate()
        {
            try
            {
                controller.UpdateTaskDueDate(UserEmail, ShownBoard.CreatorEmail, ShownBoard.BoardName, ShownColumn.Ordinal, ShownTask.Task.Id, DateTime.Parse(SelectedDueDateUpdate));
                ShownTask.Task.DueDate = DateTime.Parse(SelectedDueDateUpdate);
                ShownTask.UpdateTaskStateColor();
                Message = "Task's due date changed! You can see it on it's current column now.";
                UpdateTaskDueDateDialog = false;
                log.Debug("Change the due date of the task.");

            }
            catch (Exception e)
            {
                Message = "Cannot edit task. " + e.Message;
                log.Debug("Try to show negative feedback to user.");
            }
            finally
            {
                ShowMessage = true;
                log.Debug("Show negative feedback to user.");

            }
        }

        /// <summary>
        /// Open 'update task due date' dialog.
        /// </summary>
        internal void UpdateTaskDueDateMenu()
        {
            try
            {
                if (ShownTask == null)
                {
                    log.Error("Shown task is null.");

                    throw new Exception("No task selected!");
                }
                UpdateTaskDueDateDialog = true;
                log.Debug("Open dialog.");

            }
            catch (Exception e)
            {
                Message = "Cannot edit task. " + e.Message;
                ShowMessage = true;
                log.Debug("Show negative feedback to user.");
            }
        }
        /// <summary>
        /// Change the assignee of shown task through input from 'assign task' dialog.
        /// </summary>
        internal void AssignTask()
        {
            try
            {
                controller.AssignTask(UserEmail, ShownBoard.CreatorEmail, ShownBoard.BoardName, ShownColumn.Ordinal, ShownTask.Task.Id, AssignTaskInput);
                ShownTask.Task.EmailAssignee = AssignTaskInput;
                ShownTask.UpdateAsigneeBorder(ShownTask.Task.EmailAssignee == UserEmail);
                Message = "Task's assignee changed! You can see it on it's current column now on the board.";
                AssignTaskDialog = false;
                log.Debug("Change the assignee of the task.");

            }
            catch (Exception e)
            {
                Message = "Cannot edit task. " + e.Message;
                log.Debug("Try to show negative feedback to user.");
            }
            finally
            {
                ShowMessage = true;
                log.Debug("Show negative feedback to user.");

            }
        }

        /// <summary>
        /// Open 'assign task' dialog.
        /// </summary>
        internal void AssignTaskMenu()
        {
            try
            {
                if (ShownTask == null)
                {
                    log.Error("Shown task is null.");

                    throw new Exception("No task selected!");
                }
                AssignTaskDialog = true;
                log.Debug("Open dialog.");

            }
            catch (Exception e)
            {
                Message = "Cannot edit task. " + e.Message;
                ShowMessage = true;
                log.Debug("Show negative feedback to user.");
            }
        }

        private int IntInputValidation(string s)
        {
            try
            {
                log.Debug("Try to parse the input from string to integer.");

                return Int32.Parse(s);
            }
            catch
            {
                log.Error("Input entered is not a number.");
                throw new Exception("Input is not a number!");
            }
        }

    }
}
