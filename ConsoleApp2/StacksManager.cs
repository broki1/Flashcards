using QC = Microsoft.Data.SqlClient;
using System.Configuration;
using Flashcards.Models;
using ConsoleTableExt;

namespace Flashcards;

internal class StacksManager
{
    // holds all Stacks to display
    private static List<Stack> stacks = new List<Stack>();

    // connection string for Flashcards DB
    private static string connectionString = ConfigurationManager.AppSettings.Get("FlashcardsConnectionString");
    internal static void ManageStacks()
    {
        // calls method that displays Stacks to user
        StacksManager.PrintStacks();
    }

    internal static void PrintStacks()
    {
        using (var connection = new QC.SqlConnection(connectionString))
        {
            using (var command = new QC.SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = @"SELECT name FROM Stacks ORDER BY id";

                // executes query that returns a SQL Data Reader obj
                var reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    // while reader obj has rows to read, add that Stack to the Stacks list, but only the name
                    while (reader.Read())
                    {
                        stacks.Add(new Stack
                        {
                            Name = reader.GetString(0)
                        });
                    }
                }
                else
                {
                    Console.WriteLine("\n\nNo rows found.");
                }

                // uses ConsoleTableExt NuGet package library to display List of Stacks as a table
                ConsoleTableBuilder.From(stacks).ExportAndWriteLine();
                Console.WriteLine("\n\n");

            }
        }
    }
}
