using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using log4net;


[assembly: InternalsVisibleTo("Test")]
namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    /// <summary>
    /// Represents tables.
    /// </summary>
    internal abstract class DOS
    {
        private readonly ILog log = LogManager.GetLogger("piza");
        protected readonly string connectionString = "Data Source=kanban.db;"; //connection string
        private readonly string tableName; //current table

        /// <summary>Converts reader to DTO.</summary>
        /// <param name="reader">Reader to convert from.</param>
        /// <returns>DTO created from the reader.</returns>
        protected abstract DTO ConvertReaderToObject(SQLiteDataReader reader);

        /// <summary>Creates a table if it does not exist.</summary>
        protected abstract void CreateTable(); 

        ///<summary>Constructor of a new DOS, create a new Table if does not exist.</summary>
        ///<param name="tableName"> Table name.</param>
        public DOS(string tableName)
        {
            this.tableName = tableName;
            CreateTable();
            log.Debug("Dos was created");
        }

        ///<summary>Update row in current table.</summary>
        ///<param name="id"> Id of the row that should be update.</param>
        ///<param name="attributeName"> Which column to update.</param>
        ///<param name="attributeValue"> Which value to set.</param>
        ///<exception cref = "Exception" > Fail to update.</exception>
        public void Update(int id, string attributeName, string attributeValue)
        {
            log.Debug($"Try to update {id} {attributeName} to {attributeValue} in {tableName}");
            using var connection = new SQLiteConnection(connectionString);
            log.Debug("Open connection");
            using var command = new SQLiteCommand(connection);
            connection.Open();
            command.CommandText = $"UPDATE {tableName} SET {attributeName} = @attributeValue WHERE {DTO.IDColumnName} = @id";
            command.Parameters.AddWithValue("@attributeValue", attributeValue);
            command.Parameters.AddWithValue("@id", id);
            command.Prepare();
            int rowsChanged = command.ExecuteNonQuery();
            if (rowsChanged == 0)
            {
                log.Error($"Fail to update {id} {attributeName} to {attributeValue}");
                throw new Exception($"Fail to update {id} {attributeName}");
            }
            log.Debug($"Update successfully");
        }

        ///<summary>Delete row in current table.</summary>
        ///<param name="dto"> Which row to delete.</param>
        ///<exception cref = "Exception" > Fail to delete.</exception>
        public void Delete(DTO dto)
        {
            log.Debug($"Try to delte {dto.Id} from {tableName}");
            using var connection = new SQLiteConnection(connectionString);
            log.Debug("Open connection");
            using var command = new SQLiteCommand(connection);
            connection.Open();
            command.CommandText = $"DELETE FROM {tableName} WHERE {DTO.IDColumnName} = @id";
            SQLiteParameter idParam = new SQLiteParameter(@"id", dto.Id);
            command.Parameters.Add(idParam);
            command.Prepare();
            int rowsChanged = command.ExecuteNonQuery();
            if (rowsChanged == 0)
            {
                log.Error($"Fail to delete {dto.Id}");
                throw new Exception($"Fail to delete {dto.Id}");
            }
            log.Debug($"Delete successfully");
        }

        ///<summary>Delete data in current table.</summary>
        public void Delete()
        {
            log.Debug($"Try to delte data from {tableName}");
            using var connection = new SQLiteConnection(connectionString);
            log.Debug("Open connection");
            using var command = new SQLiteCommand(connection);
            connection.Open();
            command.CommandText = $"DELETE FROM {tableName}";
            command.Prepare();
            command.ExecuteNonQuery();
            log.Debug($"Delete successfully");
        }

        ///<summary>List of data in current table.</summary>
        ///<returns>Return list.</returns>
        protected List<DTO> Select()
        {
            log.Debug($"Try to import data from {tableName}");
            List<DTO> results = new List<DTO>();
            using var connection = new SQLiteConnection(connectionString);
            log.Debug("Open connection");
            using var command = new SQLiteCommand(connection);
            command.CommandText = $"SELECT * FROM {tableName};";
            SQLiteDataReader dataReader = null;
            connection.Open();
            dataReader = command.ExecuteReader();

            while (dataReader.Read())
            {
                results.Add(ConvertReaderToObject(dataReader));

            }
            if (dataReader != null)
            {
                dataReader.Close();
            }
            log.Debug("Return list of data");
            return results;
        }
    }
}
