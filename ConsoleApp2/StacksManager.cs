using QC = Microsoft.Data.SqlClient;
using System.Configuration;
using Flashcards.Models;
using ConsoleTableExt;

namespace Flashcards;

internal class StacksManager
{
    // connection string for Flashcards DB
    private static string connectionString = ConfigurationManager.AppSettings.Get("FlashcardsConnectionString");
    private static List<string> stackNames = new List<string>();
    internal static void ManageStacks()
    {
        Console.Clear();
        // calls method that displays Stacks to user
        StacksManager.PrintStacks();

        Console.WriteLine("------------------------------------");
        Console.WriteLine("Input current stack name\nInput 1 to create new stack\nOr input 0 to exit input");
        Console.WriteLine("------------------------------------\n\n");

        var userInput = Console.ReadLine().Trim().ToLower();

        var validStackName = StacksManager.ValidateInput(userInput);

        Console.WriteLine(validStackName);

    }

    private static bool ValidateInput(string input)
    {
        if (StacksManager.stackNames.Contains(input.Trim().ToLower()))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    internal static void PrintStacks()
    {
        Console.Clear();

        // holds all Stacks to display
        List<Stack> stacks = new List<Stack>();

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

                        stackNames.Add(reader.GetString(0).Trim().ToLower());
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
