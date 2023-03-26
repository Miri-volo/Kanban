using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.Reflection;
using System.Text.RegularExpressions;
using DALUserController = IntroSE.Kanban.Backend.DataAccessLayer.UserController;
using DALUser = IntroSE.Kanban.Backend.DataAccessLayer.DTOs.User;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    /// <summary>
    /// Represents a registered User.
    /// </summary>
    class User
    {
        private readonly ILog log = LogManager.GetLogger("piza");
        private DALUserController dalUserController;
        private DALUser user;
        private readonly HashSet<string> commonPassword = new HashSet<string> { "123456", "123456789", "qwerty", "password", "1111111", "12345678", "abc123", "1234567", "password1", "12345",
            "1234567890", "123123", "000000", "Iloveyou", "1234", "1q2w3e4r5t", "Qwertyuiop", "123", "Monkey", "Dragon"};

        ///<summary>Gets the email of the user.</summary>
        ///<value>User name.</value>
        public readonly string email;

        private string password; //password
        ///<summary>Set and get of password, also update the password in DataAccessLayer.</summary>
        ///<param name="value">The new password.</param>
        ///<returns>return password.</returns>
        ///<exception cref = "Exception" > if password is not in correct format.</exception>
        private string Password 
        {
            get { return password; }
            set
            {
                if (!IsLegalPass(value))
                {
                    log.Error("Password is not in correct format, can not set password.");
                    throw new Exception("Password must be in length of 4 to 20 characters, include at least one uppercase letter, character and a number.");
                }
                else
                {
                    user.Password = value;
                    password = value;
                    log.Debug("Password is set.");
                }
            }
        }

        ///<summary>Constructor of a new user, also add the new user in DataAccessLayer.</summary>
        ///<param name="email">The email address of the new user.</param>
        ///<param name="password">The password of the new user.</param>
        ///<param name="dalUserController"> UserController from DataAccessLayer</param>
        ///<returns>return the user.</returns>
        ///<exception cref = "Exception" > if email or password are illegal.</exception>
        public User(string email,string password, DALUserController dalUserController)
        {
            if (!IsLegalEmail(email))
            {
                log.Error($"Email {email} is not in correct format, can not create a new user.");
                throw new Exception($"Email is not in correct format, can not create a new user.");
            }
            if (!IsLegalPass(password))
            {
                log.Error("Password is not in correct format, can not set password.");
                throw new Exception("Password must be in length of 4 to 20 characters, include at least one uppercase letter, character and a number.");
            }
            this.dalUserController = dalUserController;
            user = dalUserController.Create(email, password);
            this.email = email;
            this.password = password;
            log.Debug($"New user {email} was created and added to database.");
        }

        ///<summary>Constructor of a new user that loaded from data.</summary>
        ///<param name="user">User from data.</param>
        ///<param name="dalUserController"> UserController from DataAccessLayer</param>
        ///<returns>return the user.</returns>
        public User(DALUser user, DALUserController dalUserController)
        {
            this.dalUserController = dalUserController;
            this.email = user.Email;
            this.password = user.Password;
            this.user = user;
            log.Debug($"New user {email} was loaded.");
        }


        ///<summary>Check if password is user's password.</summary>
        ///<param name="password">The password of the user.</param>
        ///<returns>return true if password is User's password.</returns>
        private bool ValidatePasswordMatch(string password)
        {
            log.Debug("Checking if password is valid.");
            if (this.password.Equals(password))
                return true;
            return false;
        }

        ///<summary>Login to this user.</summary>
        ///<param name="password">The password of the user.</param>
        ///<exception cref = "Exception" > if password is invalid.</exception>
        public void Login(string checkPassword)
        {
            if (ValidatePasswordMatch(checkPassword))
            {
                log.Debug("Password is valid, user can login.");
            }
            else
            {
                log.Error("Invalid password for this user, can not log in.");
                throw new Exception("Invalid password for this user.");
            }     
        }

        ///<summary>Check if password is legal,in the right format.</summary>
        ///<param name="password">The password of the user.</param>
        ///<returns>return true if password is legal.</returns>
        private bool IsLegalPass(string password)
        {
            MostCommonPassword(password);
            log.Debug("Checking if password in the right format");
            Regex rx = new Regex(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?\d).{4,20}$");
            if (password==null||!rx.IsMatch(password))
                return false;
            else
                return true;
        }

        ///<summary>Check if not one of the top 20 passwords published by the National Cyber Security Centre.</summary>
        ///<param name="password">The password of the user.</param>
        ///<exception cref = "Exception" > If password is one of them.</exception>
        private void MostCommonPassword(string password)
        {
            log.Debug("Checking if password not one of the top 20 passwords published by the National Cyber Security Centre.");
            if (commonPassword.Contains(password))
            {
                log.Debug("Password one of the top 20 passwords published by the National Cyber Security Centre.");
                throw new Exception("Low password strength.");
            }  
        }

        ///<summary>Check if email is legal,in the right format.</summary>
        ///<param name="email">The email of the user.</param>
        ///<returns>return true if email is legal.</returns>
        //source: https://stackoverflow.com/questions/5342375/regex-email-validation
        private bool IsLegalEmail(string email)
        {
            log.Debug("Checking if email in the right format");
            if (email is not null&&Regex.IsMatch(email,
                 @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                 @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                 RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250))) 
                return true;
            else
                return false;
        }
    }
}
