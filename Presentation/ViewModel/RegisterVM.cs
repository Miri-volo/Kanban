using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Presentation;
using IntroSE.Kanban.Presentation.Model;
using log4net;

namespace IntroSE.Kanban.Presentation.ViewModel
{
    ///<summary>Represents a Register View Model</summary
    class RegisterVM : NotifiableObject
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

        private string validatePassword;
        ///<summary>Set and get of ValidatePassword.</summary>
        ///<param name="value">The new ValidatePassword.</param>
        ///<returns>return ValidatePassword.</returns>
        public string ValidatePassword
        {
            get => validatePassword;
            set
            {
                this.validatePassword = value;
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

        private string message;
        ///<summary>Set and get of Error Message.</summary>
        ///<param name="value">The new Message.</param>
        ///<returns>return Message.</returns>
        public string Message
        {
            get => message;
            set
            {
                this.message = value;
                RaisePropertyChanged("Message");
            }
        }

        ///<summary>Constructor of RegisterVM.</summary>
        public RegisterVM()
        {
            log.Debug("Created Register view model.");
            this.Controller = new BackendController();
        }

        ///<summary>Register user. if there is error update Message.</summary>
        public void Register()
        {
            Message = null;
            try
            {
                log.Debug("Try to register.");
                Controller.Register(Email, Password, ValidatePassword);
            }
            catch (Exception e)
            {
                Message = e.Message;
            }
        }

    }
}
