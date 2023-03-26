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

namespace IntroSE.Kanban.Presentation.View
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        private RegisterVM registerVM;
        private Login login;

        ///<summary>Constructor of Register window.</summary>
        public Register()
        {
            InitializeComponent();
            this.DataContext = new RegisterVM();
            this.registerVM = (RegisterVM)DataContext;
            this.login = new Login();

        }

        ///<summary>Register user and open Login window if there is no error message and close the current one.</summary>
        ///<param name="e">RoutedEventArgs.</param>
        ///<param name="sender">object.</param>
        private void RegisterUser(object sender, RoutedEventArgs e)
        {
            registerVM.Register();
            if (registerVM.Message == null)
            {
                this.Close();
                login.Show();
            }
        }

        ///<summary>Update password from passwordBox.</summary>
        ///<param name="e">RoutedEventArgs.</param>
        ///<param name="sender">object.</param>
        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            registerVM.Password = passwordBox.Password;
        }

        ///<summary>Update ValidatePassword from passwordBox.</summary>
        ///<param name="e">RoutedEventArgs.</param>
        ///<param name="sender">object.</param>
        private void ValidatePasswordChanged(object sender, RoutedEventArgs e)
        {
            registerVM.ValidatePassword = validatePasswordBox.Password;
        }

        ///<summary>Open Login window and close the current one.</summary>
        ///<param name="e">RoutedEventArgs.</param>
        ///<param name="sender">object.</param>
        private void OpenLoginWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
            login.Show();
        }
    }
}
