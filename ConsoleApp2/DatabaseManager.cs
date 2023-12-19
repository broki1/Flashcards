﻿using System;
using System.Configuration;
using QC = Microsoft.Data.SqlClient;
using DT = System.Data;
using Flashcards.Models;

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
                                        front varchar(255) NOT NULL UNIQUE,
                                        back varchar(255) NOT NULL
                                        )";

                command.ExecuteNonQuery();
            }
        }
    }

    internal static void CreateStack(string stackName, string flashcardsConnectionString)
    {
        using (var connection = new QC.SqlConnection(flashcardsConnectionString))
        {
            using (var command = new QC. SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType= DT.CommandType.Text;
                command.CommandText = @$"INSERT INTO Stacks (name) VALUES ('{stackName}')";

                command.ExecuteNonQuery();
            }
        }

        Console.WriteLine($"\n\nStack '{stackName}' created.\n\n");
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

    // queries Stacks table for ID of stack whose name matches the argument passed in
    internal static int GetStackId(string stackName)
    {
        int stackId;
        using (var connection = new QC.SqlConnection(ConfigurationManager.AppSettings.Get("FlashcardsConnectionString")))
        {
            using (var command = new QC.SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = DT.CommandType.Text;
                command.CommandText = $"SELECT id FROM Stacks WHERE name = '{stackName}'";

                stackId = Convert.ToInt32(command.ExecuteScalar());
            }
        }

        return stackId;
    }
}
