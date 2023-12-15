using ConsoleTableExt;
using Flashcards.Models;
using System.Configuration;
using QC = Microsoft.Data.SqlClient;

namespace Flashcards;

internal class StacksManager
{
    // connection string for Flashcards DB
    private static string connectionString = ConfigurationManager.AppSettings.Get("FlashcardsConnectionString");

    // creates string list to hold Stack names that are printed to the console, to validate if user input correlates to existing stack
    private static List<string> stackNames = new List<string>();

    internal static void ManageStacksMenu()
    {
        bool exitManageStacksMenu = false;

        while (!exitManageStacksMenu)
        {
            // calls method that displays Stacks to user
            StacksManager.PrintStacks();

            Console.WriteLine("------------------------------------");
            Console.WriteLine("Input current stack name\nInput 1 to create new stack\nOr input 0 to exit input");
            Console.WriteLine("------------------------------------\n\n");

            var userInput = Console.ReadLine().Trim().ToLower();

            switch (userInput)
            {
                case "0":
                    exitManageStacksMenu = true;
                    break;
                case "1":
                    StacksManager.CreateNewStack();
                    break;
                default:

                    if (!Helpers.StackNameAlreadyExists(userInput))
                    {
                        Console.WriteLine("\n\nStack doesn't exist. Input current stack name, input 1 to create new stack, or input 0 to exit input.\n\n");
                    }
                    else
                    {
                        Console.WriteLine("Success!");
                    }

                    break;
            }
        }

    }

    private static void CreateNewStack()
    {
        Console.WriteLine("Input the stack name:");
        var stackName = Console.ReadLine().Trim().ToLower();

        while (Helpers.StackNameAlreadyExists(stackName))
        {
            Console.WriteLine("\n\nInvalid input. Stack name already exists. Please try again.\n\n");
            stackName = Console.ReadLine();
        }

        stackName = Helpers.FormatStackName(stackName);

        DatabaseManager.CreateStack(stackName, connectionString);
    }

    internal static void PrintStacks()
    {
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

                        if (!StacksManager.stackNames.Contains(reader.GetString(0).Trim().ToLower()))
                        {
                            StacksManager.stackNames.Add(reader.GetString(0).Trim().ToLower());
                        }
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
