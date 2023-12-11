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

    internal static void CreateFlashcardsTable(string flashcardsConnectionString)
    {
        using (var connection = new QC.SqlConnection(flashcardsConnectionString))
        {
            connection.Open();

            using (var command = new QC.SqlCommand())
            {
                command.Connection = connection;
                command.CommandType = DT.CommandType.Text;
                command.CommandText = @"IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = 'Flashcards')
                                        CREATE TABLE Flashcards(
                                        id int PRIMARY KEY IDENTITY(1,1),
                                        stack int NOT NULL FOREIGN KEY REFERENCES Stacks(id),
                                        question varchar(255) NOT NULL UNIQUE,
                                        answer varchar(255) NOT NULL
                                        )";

                command.ExecuteNonQuery();
            }
        }
    }

    internal static void CreateStacksTable(string flashcardsConnectionString)
    {
        using (var connection = new QC.SqlConnection(flashcardsConnectionString))
        {
            connection.Open();
            
            using (var command = new QC.SqlCommand())
            {
                command.Connection = connection;
                command.CommandType= DT.CommandType.Text;
                command.CommandText = @"IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = 'Stacks')
                                        CREATE TABLE Stacks(
                                        id int PRIMARY KEY IDENTITY(1,1),
                                        name varchar(50) NOT NULL UNIQUE
                                        )";

                command.ExecuteNonQuery();
            }
        }
    }
}
