using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Test")]
namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOs
{
    /// <summary>
    /// Represents a user in User table.
    /// </summary>
    internal class User : DTO
    {
        ///<summary>Gets column name Email.</summary>
        ///<value>Column Email name.</value>
        public const string EmailColumn = "Email";
        ///<summary>Gets column name Password.</summary>
        ///<value>Column Password name.</value>
        public const string PasswordColumn = "Password";

        private string email; 
        ///<summary>Set and get of email.</summary>
        ///<param name="value">The new email.</param>
        ///<returns>Return email.</returns>
        public string Email
        { 
            get => email;
            set
            {
                controller.Update(Id, EmailColumn, value);
                email = value;
            } 
        }

        private string password;
        ///<summary>Set and get of password.</summary>
        ///<param name="value">The new password.</param>
        ///<returns>Return password.</returns>
        public string Password
        { 
            get => password;
            set {
                controller.Update(Id, PasswordColumn, value);
                password = value;
            } 
        }

        ///<summary>Constructor of a new user in User table.</summary>
        ///<param name="userController"> userController</param>
        ///<param name="email">The email address of the new user.</param>
        ///<param name="password">The password of the new user.</param>
        ///<returns>Return the user.</returns>
        public User(UserController userController,int id, string email, string password)
            : base(userController)
        {
            Id = id;
            this.email = email;
            this.password = password;
        }
    }
}
