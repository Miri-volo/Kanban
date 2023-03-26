using log4net;
namespace IntroSE.Kanban.Presentation.Model
{
    ///<summary>Represents a Service Singleton</summary>
    public class UserModel : NotifiableModelObject
    {
        private readonly ILog log = LogManager.GetLogger("piza");
        private string email;
        ///<summary>Set and get of Email.</summary>
        ///<param name="value">The new email.</param>
        ///<returns>return email.</returns>
        public string Email
        {
            get => email;
            set
            {
                email = value;
                RaisePropertyChanged("Email"); 
            }
        }

        ///<summary>Constructor of Model's User.</summary>
        /// <param name="controller">The BackendController.</param>
        /// <param name="email">The email of the user.</param>
        public UserModel(BackendController controller, string email) : base(controller)
        {
            this.Email = email;
            log.Debug("Created User model.");
        }

    }
}
