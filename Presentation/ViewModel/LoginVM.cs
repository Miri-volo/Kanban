using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Presentation.Model;
using log4net;

namespace IntroSE.Kanban.Presentation.ViewModel
{
    ///<summary>Represents a Login View Model</summary>
    class LoginVM : NotifiableObject
    {
        private readonly ILog log = LogManager.GetLogger("piza");
        ///<summary>Set and get of BackendController.</summary>
        ///<param name="value">The new BackendController.</param>
        ///<returns>return BackendController.</returns>
        public BackendController Controller { get; private set; }

        private string password;
        ///<summary>Set and get of Password.</summary>
        ///<param name="value">The new password.</param>
        ///<returns>return password.</returns>
        public string Password
        {
            get => password;
            set
            {
                this.password = value;
                RaisePropertyChanged("Password");
            }
        }

        private string email;
        ///<summary>Set and get of Email.</summary>
        ///<param name="value">The new email.</param>
        ///<returns>return email.</returns>
        public string Email
        {
            get => email;
            set
            {
                this.email = value.ToLower();
                RaisePropertyChanged("Email");
            }
        }

        ///<summary>Set and get of Error Message.</summary>
        ///<param name="value">The new Message.</param>
        ///<returns>return Message.</returns>
        private string message;
        public string Message
        {
            get => message;
            set
            {
                this.message = value;
                RaisePropertyChanged("Message");
            }
        }

        ///<summary>Model's User.</summary>
        public UserModel userModel;

        ///<summary>Constructor of LoginVM.</summary>
        public LoginVM()
        {
            log.Debug("Created Login view model.");
            this.Controller = new BackendController();
        }

        ///<summary>Login user. If there is error, update Message.</summary>
        public void Login()
        {
            Message = null;
            try
            {
                log.Debug("Try to login.");
                userModel = Controller.Login(Email, Password);
            }
            catch (Exception e)
            {
                Message = e.Message;
            }
        }
    }
}
