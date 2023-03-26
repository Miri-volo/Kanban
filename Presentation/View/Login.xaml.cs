using IntroSE.Kanban.Presentation.View;
using IntroSE.Kanban.Presentation.ViewModel;
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

namespace IntroSE.Kanban.Presentation
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private LoginVM loginVM;

        ///<summary>Constructor of Login window.</summary>
        public Login()
        {
            InitializeComponent();
            this.DataContext = new LoginVM();
            this.loginVM = (LoginVM)DataContext;
        }

        ///<summary>Login user and open BoardView window if there is no error message and close the current one.</summary>
        ///<param name="e">RoutedEventArgs.</param>
        ///<param name="sender">object.</param>
        private void LoginUser(object sender, RoutedEventArgs e)
        {
            loginVM.Login();
            if(loginVM.Message == null)
            {
                BoardView boardView = new BoardView(loginVM.userModel.Email);
                this.Close();
                boardView.Show();
            }
        }

        ///<summary>Update password from passwordBox.</summary>
        ///<param name="e">RoutedEventArgs.</param>
        ///<param name="sender">object.</param>
        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            loginVM.Password = passwordBox.Password;
        }

        ///<summary>Open register window and close the current one.</summary>
        ///<param name="e">RoutedEventArgs.</param>
        ///<param name="sender">object.</param>
        private void OpenRegisterWindow(object sender, RoutedEventArgs e)
        {
            Register register = new Register();
            this.Close();
            register.Show();
        }
    }
}
