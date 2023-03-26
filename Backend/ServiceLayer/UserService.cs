using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using System.IO;
using IntroSE.Kanban.Backend.BusinessLayer;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    /// <summary>
    /// Represents a User Service.
    /// </summary>
    class UserService
    {
        private readonly ILog log = LogManager.GetLogger("piza");
        private readonly UserController userController; //UserController

        ///<summary>Constructor of UserService.</summary>
        ///<param name="userController">UserController.</param>
        public UserService(UserController userController)
        {
            this.userController = userController;
        }

        ///<summary>This method registers a new user to the system.</summary>
        ///<param name="email">the user e-mail address, used as the username for logging the system.</param>
        ///<param name="password">the user password.</param>
        ///<returns cref="Response">The response of the action</returns>
        public Response Register(string email,string password)
        {
            try
            {
                userController.Register(email, password);
                log.Debug("User is created successfully.");
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        ///<summary>Validate if passwords match in registration</summary>
        ///<param name="password">The first password.</param>
        ///<param name="validatePassword">The second password.</param>
        /// <returns>A response object. The response should contain a error message in case of an error<returns>
        public Response ValidatePassword(string password, string validatePassword)
        {
            try
            {
                userController.ValidatePassword(password, validatePassword);
                log.Debug("Passwords are match.");
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        /// <summary>Log in an existing user</summary>
        /// <param name="email">The email address of the user to login</param>
        /// <param name="password">The password of the user to login</param>
        /// <returns>A response object with a value set to the user, instead the response should contain a error message in case of an error</returns>
        public Response<User> Login(string email, string password)
        {
            try
            {
                BusinessLayer.User user = userController.Login(email, password);
                User su = new User(user.email);
                log.Debug("User is logged in successfully.");
                return Response<User>.FromValue(su);
            }
            catch (Exception e)
            {
                return Response<User>.FromError(e.Message);
            }
        }

        /// <summary>Log out an logged in user.</summary>
        /// <param name="email">The email of the user to log out</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response Logout(string email)
        {
            try
            {
                userController.Logout(email);
                log.Debug("User is logged out successfully.");
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }





    }
}
