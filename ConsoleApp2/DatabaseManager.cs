using System;
using System.Configuration;
using QC = Microsoft.Data.SqlClient;
using DT = System.Data;

namespace Flashcards;

internal class DatabaseManager
{

    internal static void CreateDatabase(string masterConnectionString)
    {
        // creates Microsoft.Data.SqlClient connection object using the master connection string
        using (var connection = new QC.SqlConnection(masterConnectionString))
        {
            connection.Open();
            
            // creates new MDSC SQL command object
            using (var command = new QC.SqlCommand())
            {
                // sets its connection field to the MDSC connection object, the command's type to 'text' and command set to the query
                // executes non-query
                command.Connection = connection;
                command.CommandType = DT.CommandType.Text;
                command.CommandText = @"IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = 'Flashcards') CREATE DATABASE Flashcards";
                command.ExecuteNonQuery();
            }
        }
    }

}
