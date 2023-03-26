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
    /// Represents a data transfer object.
    /// </summary>
    internal abstract class DTO
    {
        ///<summary>Gets column name Id.</summary>
        ///<value>Column Id name.</value>
        public const string IDColumnName = "Id";
        protected DOS controller;

        ///<summary>Set and get of Id.</summary>
        ///<param name="value">The new Id.</param>
        ///<returns>Return Id.</returns>
        public int Id { get; set; } = -1;

        ///<summary>Constructor of a new data transfer object.</summary>
        ///<param name="controller"> DOS controller</param>
        ///<returns>Return data transfer object.</returns>
        protected DTO(DOS controller)
        {
            this.controller = controller;
        }
    }
}
