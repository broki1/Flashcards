using ConsoleTableExt;
using Flashcards.Models;
using System.Configuration;
using System.Threading.Channels;
using QC = Microsoft.Data.SqlClient;

namespace Flashcards;

internal class StacksManager
{
    // connection string for Flashcards DB
    private static string connectionString = ConfigurationManager.AppSettings.Get("FlashcardsConnectionString");

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
                        StacksManager.ManageStack(Helpers.FormatStackName(userInput));
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

    private static void ManageStack(string stackName)
    {
        Console.WriteLine($"Press 'D' to delete the {stackName} stack, 0 to exit to stack menu.");
        var userInput = Console.ReadLine().Trim().ToLower();

        while (userInput != "d" &&  userInput != "0")
        {
            Console.WriteLine($"\nInvalid input. Press 'D' to delete the {stackName} stack, 0 to exit to stack menu.");
            userInput = Console.ReadLine().Trim().ToLower();
        }

        switch (userInput)
        {
            case "d":
                Console.Clear();
                Console.WriteLine("\nAre you sure you wish to delete the stack? Press 'y' to confirm, press 'n' to cancel. (All flashcards associated with the stack will be deleted as well.)");

                var userConfirmation = Console.ReadLine().Trim().ToLower() == "y" ? true : false;

                if (userConfirmation)
                {
                    var stackId = DatabaseManager.GetStackId(stackName);

                    DatabaseManager.DeleteStack(stackId);
                }
                break;
            case "0":
                break;
        }
    }
}
