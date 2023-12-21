﻿using ConsoleTableExt;
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
                        StacksManager.IndividualStackMenu(Helpers.FormatStackName(userInput));
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

    private static void PrintStacks()
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

    internal static void IndividualStackMenu(string stackName)
    {
        Console.WriteLine("-----------------------------------");
        Console.WriteLine($"Current working stack: {stackName}");
        Console.WriteLine("-----------------------------------");
        Console.WriteLine("\n0 to return to main menu");
        Console.WriteLine("X to change current stack");
        Console.WriteLine("V to view all flashcards in stack");
        Console.WriteLine("A to view X number of flashcards in stack");
        Console.WriteLine("C to Create a Flashcard in current stack");
        Console.WriteLine();

        var userInput = Console.ReadLine().ToLower().Trim();

        var stackId = DatabaseManager.GetStackId(stackName);

        switch (userInput)
        {
            case "0":
                MainMenu.StartApplication();
                break;
            case "x":
                break;
            case "v":
                // get Stack ID, then get all flashcards with that stack ID assigned to them, display in order by stack ID
                StacksManager.PrintFlashcardsInStack(stackId);
                break;
            case "a":
                var numFlashcards = Helpers.GetNumFlashcards(stackName);
                StacksManager.PrintFlashcardsInStack(stackId, numFlashcards);
                break;
            case "c":
                Flashcard flashcard = Helpers.CreateFlashcard(stackId, stackName);
                DatabaseManager.PostFlashcard(flashcard);
                break;


        }
    }

    private static void PrintFlashcardsInStack(int stackId, int numFlashcards = -1)
    {
        Console.Clear();
        // holds all Stacks to display
        List<FlashcardDTO> flashcards = new List<FlashcardDTO>();

        using (var connection = new QC.SqlConnection(connectionString))
        {
            using (var command = new QC.SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = $"SELECT * FROM Flashcards WHERE stack = {stackId} ORDER BY stack";

                // executes query that returns a SQL Data Reader obj
                var reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    // while reader obj has rows to read, add that Stack to the Stacks list, but only the name
                    while (reader.Read())
                    {
                        if (numFlashcards == 0)
                        {
                            break;
                        }

                        flashcards.Add(new FlashcardDTO
                        {
                            Front = reader.GetString(2),
                            Back = reader.GetString(3)
                        });

                        numFlashcards--;
                    }
                }
                else
                {
                    Console.WriteLine("\n\nNo rows found.");
                }

                var index = 1;

                foreach (var flashcard in flashcards)
                {
                    flashcard.Id = index;
                    index++;
                }

                // uses ConsoleTableExt NuGet package library to display List of Stacks as a table
                ConsoleTableBuilder.From(flashcards).ExportAndWriteLine();
                Console.WriteLine("\n\nPress any key to continue.");

                Console.ReadKey();
                Console.WriteLine("\n\n");
            }
        }
    }
}
