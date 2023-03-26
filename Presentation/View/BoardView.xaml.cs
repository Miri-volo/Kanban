using IntroSE.Kanban.Presentation.ViewModel;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IntroSE.Kanban.Presentation.View
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class BoardView : Window
    {
        private readonly ILog log = LogManager.GetLogger("piza");

        private BoardVM viewModel;

        /// <summary>
        /// Create a new view window of board.
        /// </summary>
        /// <param name="userEmail">The email of current logged-in user.</param>
        public BoardView(string userEmail)
        {
            InitializeComponent();
            this.viewModel = new BoardVM(userEmail);
            this.DataContext = viewModel;
            log.Debug("Created a view window of board.");
        }

        private void CloseMessageBar_Button_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ShowMessage = false;
            viewModel.Message = "";
        }

        private void ShowBoard(object sender, MouseButtonEventArgs e)
        {
            viewModel.ShowBoard();
        }

        private void TasksTab_Button_Click(object sender, RoutedEventArgs e)
        {
            viewModel.TasksShow();
        }

        private void AddBoard_Button_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AddBoard();
        }

        private void JoinBoard_Button_Click(object sender, RoutedEventArgs e)
        {
            viewModel.JoinBoard();
        }

        private void RemoveBoard_Menu_Click(object sender, RoutedEventArgs e)
        {
            viewModel.RemoveBoard();
        }

        private void AddColumn_Menu_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AddColumnMenu();
        }

        private void AddColumn_Button_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.AddColumnDialog)
            {
                viewModel.AddColumn();
            }
        }

        private void RemoveColumn_Menu_Click(object sender, RoutedEventArgs e)
        {
            viewModel.RemoveColumn();
        }

        private void RenameColumn_Menu_Click(object sender, RoutedEventArgs e)
        {
            viewModel.RenameColumnMenu();
        }

        private void EditColumnName_Button_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.RenameColumnDialog)
            {
                viewModel.RenameColumn();
            }
        }

        private void ShiftColumn_Menu_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ShiftColumnMenu();
        }

        private void ShiftColumn_Button_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.ShiftColumnDialog)
            {
                viewModel.ShiftColumn();
            }
        }

        private void LimitColumn_Button_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.LimitColumnDialog)
            {
                viewModel.LimitColumn();
            }
        }

        private void LimitColumn_Menu_Click(object sender, RoutedEventArgs e)
        {
            viewModel.LimitColumnMenu();
        }

        private void GetColumnLimit_Menu_Click(object sender, RoutedEventArgs e)
        {
            viewModel.GetColumnLimit();
        }

        private void AddTask_Menu_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AddTaskMenu();
        }

        private void AddTask_Button_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.AddTaskDialog)
            {
                viewModel.AddTask();
            }
        }

        private void Search_Button_Click(object sender, RoutedEventArgs e)
        {
            viewModel.FilterTasks();
        }

        private void DialogHost_DialogClosing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {

        }

        private void Logout_Button_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.Logout())
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
        }

        private void AdvanceTask_Menu_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AdvanceTask();
        }

        private void EditTaskTitle_Menu_Click(object sender, RoutedEventArgs e)
        {
            viewModel.EditTaskTitleMenu();
        }

        private void EditTaskTitle_Button_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.EditTaskTitleDialog)
            {
                viewModel.EditTaskTitle();
            }
        }

        private void EditTaskDescription_Menu_Click(object sender, RoutedEventArgs e)
        {
            viewModel.EditTaskDecriptionMenu();
        }

        private void EditTaskDescription_Button_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.EditTaskDescriptionDialog)
            {
                viewModel.EditTaskDecription();
            }
        }

        private void UpdateTaskDueDate_Menu_Click(object sender, RoutedEventArgs e)
        {
            viewModel.UpdateTaskDueDateMenu();
        }

        private void UpdateTaskDueDate_Button_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.UpdateTaskDueDateDialog)
            {
                viewModel.UpdateTaskDueDate();
            }
        }

        private void AssignTask_Button_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.AssignTaskDialog)
            {
                viewModel.AssignTask();
            }
        }

        private void AssignTask_Menu_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AssignTaskMenu();
        }
    }
}

