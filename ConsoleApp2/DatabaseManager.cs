using Flashcards.Models;
using System.Configuration;
using DT = System.Data;
using QC = Microsoft.Data.SqlClient;

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

    internal static void CreateStudySessionsTable(string flashcardsConnectionString)
    {
        using (var connection = new QC.SqlConnection(flashcardsConnectionString))
        {
            using (var command = new QC.SqlCommand())
            {
                connection.Open();

                command.Connection = connection;
                command.CommandType= DT.CommandType.Text;
                command.CommandText = @"IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = 'StudySessions')
                                        CREATE TABLE StudySessions (
                                        id int PRIMARY KEY IDENTITY(1, 1),
                                        stack varchar(50) NOT NULL,
                                        correct int NOT NULL,
                                        total int NOT NULL,
                                        date date NOT NULL
                                        )";
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
            using (var command = new QC.SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = DT.CommandType.Text;
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
                command.CommandType = DT.CommandType.Text;
                command.CommandText = @"IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = 'Stacks')
                                        CREATE TABLE Stacks(
                                        id int PRIMARY KEY IDENTITY(1,1),
                                        name varchar(50) NOT NULL UNIQUE
                                        )";

                command.ExecuteNonQuery();
            }
        }
    }

    internal static void DeleteFlashcard(int stackId, string front)
    {
        var connectionString = ConfigurationManager.AppSettings.Get("FlashcardsConnectionString");

        using (var connection = new QC.SqlConnection(connectionString))
        {
            using (var command = new QC.SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = DT.CommandType.Text;
                command.CommandText = $"DELETE FROM Flashcards WHERE stack = {stackId} AND front = '{front}'";

                var success = command.ExecuteNonQuery();

                if (success == 1)
                {
                    Console.WriteLine("\n\nFlashcard successfully deleted.");
                }
            }
        }
    }

    internal static void DeleteStack(int stackId)
    {
        var connectionString = ConfigurationManager.AppSettings.Get("FlashcardsConnectionString");

        using (var connection = new QC.SqlConnection(connectionString))
        {
            using ( var command = new QC.SqlCommand())
            {
                using (var secondCommand =  new QC.SqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;
                    command.CommandType = DT.CommandType.Text;
                    command.CommandText = $"DELETE FROM Stacks WHERE id = {stackId}";

                    secondCommand.Connection = connection;
                    secondCommand.CommandType = DT.CommandType.Text;
                    secondCommand.CommandText = $"DELETE FROM Flashcards WHERE stack = {stackId}";

                    
                    var result2 = secondCommand.ExecuteNonQuery();
                    var result1 = command.ExecuteNonQuery();


                    if (result1 != -1 && result2 != -1)
                    {
                        Console.WriteLine("\n\nStack deleted successfully.");
                    }
                    else
                    {
                        Console.WriteLine("\n\nStack was not able to be deleted.");
                    }
                }
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

    // posts flashcard object to Flashcards table
    internal static void PostFlashcard(Flashcard flashcard)
    {
        var flashcardConnectionString = ConfigurationManager.AppSettings.Get("FlashcardsConnectionString");
        using (var connection = new QC.SqlConnection(flashcardConnectionString))
        {
            using (var command = new QC.SqlCommand())
            {

                connection.Open();
                command.Connection = connection;
                command.CommandType = DT.CommandType.Text;
                command.CommandText = $"INSERT INTO Flashcards (stack, front, back) VALUES ({flashcard.Stack}, '{flashcard.Front}', '{flashcard.Back}')";

                var success = command.ExecuteNonQuery() == 1;

                if (success)
                {
                    Console.WriteLine("\n\nFlashcard successfully added.");
                }
            }
        }
    }

    internal static void UpdateFlashcard(int stackId, string oldFront, string front = "", string back = "")
    {
        var flashcardConnectionString = ConfigurationManager.AppSettings.Get("FlashcardsConnectionString");

        using (var connection = new QC.SqlConnection(flashcardConnectionString))
        {
            using (var command = new QC.SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = DT.CommandType.Text;
                command.CommandText = $"UPDATE Flashcards SET front = '{front}', back = '{back}' WHERE stack = {stackId} AND front = '{oldFront}'";

                var success = command.ExecuteNonQuery();

                if (success == 1)
                {
                    Console.WriteLine("\nFlashcard successfully updated.");
                }
            }
        }
    }

    internal static void PostStudySession(StudySession session, string connectionString)
    {   
        using (var connection = new QC.SqlConnection(connectionString))
        {
            using (var command = new QC.SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType= DT.CommandType.Text;

                command.CommandText = $"INSERT INTO StudySession (stack, correct, total, date) VALUES ()";
            }
        }
    }

    internal static string GenerateQuestion(List<string> questions, int stackId)
    {
        string question;

        using (var connection = new QC.SqlConnection(ConfigurationManager.AppSettings.Get("FlashcardsConnectionString")))
        {
            connection.Open();
            using (var command =  connection.CreateCommand())
            {
                command.Connection = connection;
                command.CommandType = DT.CommandType.Text;

                command.CommandText = $"SELECT COUNT(*) FROM Flashcards WHERE stack = {stackId}";
                var uniqueFlashcardCount = Convert.ToInt32(command.ExecuteScalar());

                if (uniqueFlashcardCount == questions.Count())
                {
                    question = "Study session complete.";
                }
                else
                {
                    command.CommandText = $"SELECT TOP 1 front FROM Flashcards WHERE stack = {stackId} ORDER BY NEWID()";

                    question = Convert.ToString(command.ExecuteScalar());

                    while (questions.Contains(question))
                    {
                        question = Convert.ToString(command.ExecuteScalar());
                    }
                }
                
            }
        }

        return question;
    }
}
