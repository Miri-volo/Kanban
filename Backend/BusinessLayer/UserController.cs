using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using log4net;
using System.Reflection;
using System.Runtime.CompilerServices;
using DALUserCOntroller = IntroSE.Kanban.Backend.DataAccessLayer.UserController;
using DALUser = IntroSE.Kanban.Backend.DataAccessLayer.DTOs.User;

[assembly: InternalsVisibleTo("Tests")]
namespace IntroSE.Kanban.Backend.BusinessLayer
{
    /// <summary>
    /// Represents a list of Users.
    /// </summary>
    class UserController
    {
        private readonly ILog log = LogManager.GetLogger("piza");
        private Dictionary<string, User> users = new Dictionary<string, User>(); //dictionary of users and emails
        private User currentUser; //current user that is logged in
        private DALUserCOntroller userController;

        ///<summary>>Constructor of a new UserController and a new DataAccessLayer userController.</summary>
        public UserController()
        {
            userController = new DALUserCOntroller();
            log.Debug("New UserController was created.");
        }

        ///<summary>Validate if passwords match in registration</summary>
        ///<param name="password">The first password.</param>
        ///<param name="validatePassword">The second password.</param>
        ///<exception cref = "Exception" > Thrown when password are null or not match.</exception>
        public void ValidatePassword(string password, string validatePassword)
        {
            log.Debug("Try to check if passwords are match.");
            if (password == null || validatePassword == null) 
                throw new Exception("Please enter a password in both fields");
            if(!password.Equals(validatePassword))
                throw new Exception("Passwords do not match");
        }

        ///<summary>This method registers a new user to the system.</summary>
        ///<param name="email">the user e-mail address, used as the username for logging the system.</param>
        ///<param name="password">the user password.</param>
        ///<exception cref = "Exception" > Thrown when email or password are null, and when email alredy exists</exception>
        public void Register(string email,string password)
        {
            log.Debug("Try to register.");
            if (email is null || email.Length == 0)
            {
                log.Error("User with null email attempted register.");
                throw new Exception("Please enter email.");
            }
            if (password is null || password.Length ==0)
            {
                log.Error("User with null password attempted register.");
                throw new Exception("Please enter password.");
            }
            if (IsEmailExists(email))
            {
                log.Error($"Email {email} already exists, can not register.");
                throw new Exception($"Email already exists.");
            }
            users[email]= new User(email, password, userController);
            log.Debug("User added to the server.");
        }

        ///<summary>Log in an existing user and logged off user.</summary>
        ///<param name="email">The email address of the user to login.</param>
        ///<param name="password">The password of the user to login.</param>
        ///<returns>the User </returns>
        ///<exception cref = "Exception" > Thrown when email is invalid, and when user is already logged in or another user in logged in.</exception>
        public User Login(string email, string password)
        {
            log.Debug("Try to Login.");
            if (IsEmailExists(email))
            {
                if (currentUser != null)
                {
                    if (IsUserLogged(email))
                    {
                        log.Error("User is already Logged in, can not login again.");
                        throw new Exception("User is already Logged in, can not login again.");
                    }
                    else
                    {
                        log.Error("Another user is logged in, should logout before login.");
                        throw new Exception("Another user is logged in, should logout before login.");
                    }
                }
                else
                {
                    users[email].Login(password);
                    currentUser = users[email];
                    log.Debug("User is logged in.");
                    return currentUser;
                }
            }
            else
            {
                log.Error($"Email {email} does not exists, can not login.");
                throw new Exception($"Email does not exists, can not login.");
            }
        }

        ///<summary>Log out an existing user and make the currentUser null.</summary>
        ///<param name="email">The email address of the user to logout.</param>
        ///<exception cref = "Exception" > if this user is already logged out or if email does not exist.</exception>
        public void Logout(string email)
        {
            log.Debug("Try to Logout.");
            if (IsEmailExists(email))
            {
                if (IsUserLogged(email))
                {
                    log.Info("Current user is loggged out.");
                    currentUser = null;
                }
                else
                {
                    log.Error("User is already logged out, can not logout.");
                    throw new Exception("User is already logged out, can not logout.");
                }
            }
            else
            {
                log.Error($"Email {email} does not exists, can not logout.");
                throw new Exception($"Email {email} does not exists, can not logout.");
            }
        }

        ///<summary>Checks if email is already registered.</summary>
        ///<param name="email">The email address of the user.</param>
        ///<returns>True if user registered.</returns>
        private bool IsEmailExists(string email)
        {
            log.Debug("Checking if email is already registered.");
            if (users.ContainsKey(email))
                return true;
            return false;
        }

        ///<summary>Checks if email is logged in.</summary>
        ///<param name="email">The email address of the user.</param>
        ///<returns>True if user is logged in.</returns>
        public bool IsUserLogged(string email)
        {
            log.Debug("Checking if user is logged in.");
            if (currentUser!=null&&currentUser.email.Equals(email))
                return true;
            return false;
        }

        ///<summary>Return a user that is logged in with this email.</summary>
        ///<param name="email">The email address of the user.</param>
        ///<returns>return the user.</returns>
        ///<exception cref = "Exception" > if user is not logged in or if there is no user with this email.</exception>
        public User GetUser(string email)
        {
            log.Debug("Try to get an user.");
            if (!IsEmailExists(email)) {
                log.Error($"Email {email} does not exists, can not return a user.");
                throw new Exception($"Email {email} does not exists, can not return a user.");
            }
            else if (IsUserLogged(email))
            {  
                log.Debug("User is returned.");
                return currentUser;
            }
            else
            {
                log.Error("User is not logged in, can not return a user.");
                throw new Exception("User is not logged in, can not return a user.");
            }
        }

        ///<summary>Loads users data from the database.</summary>
        public void LoadController()
        {
            log.Debug("Try to load users data.");
            users.Clear();
            List<DALUser> dataUsers = userController.SelectAllUsers();
            foreach(DALUser user in dataUsers)
            {
                users[user.Email] = new User(user, userController);
            }
            log.Debug("All users was loaded.");
        }

        ///<summary>Delete users data from UserController and from database.</summary>
        public void DeleteController()
        {
            log.Debug("Try to delete users data.");
            userController.Delete();
            log.Debug("All users data deleted.");
        }
    }
}
