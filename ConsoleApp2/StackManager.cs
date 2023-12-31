using ConsoleTableExt;
using Flashcards.Models;
using System.Configuration;
using System.Text;
using QC = Microsoft.Data.SqlClient;

namespace Flashcards
{
    internal class StackManager
    {

        private static string connectionString = ConfigurationManager.AppSettings.Get("FlashcardsConnectionString");


        internal static void IndividualStackMenu(string stackName)
        {
            var exitStackMenu = false;

            while (!exitStackMenu)
            {
                Console.Clear();
                Console.WriteLine("-----------------------------------");
                Console.WriteLine($"Current working stack: {stackName}");
                Console.WriteLine("-----------------------------------");
                Console.WriteLine("\n0 to return to main menu");
                Console.WriteLine("X to change current stack");
                Console.WriteLine("V to view all flashcards in stack");
                Console.WriteLine("A to view X number of flashcards in stack");
                Console.WriteLine("C to Create a Flashcard in current stack");
                Console.WriteLine("E to Edit a Flashcard");
                Console.WriteLine();

                var userInput = Console.ReadLine().ToLower().Trim();

                var stackId = DatabaseManager.GetStackId(stackName);

                switch (userInput)
                {
                    case "0":
                        MainMenu.StartApplication();
                        exitStackMenu = true;
                        break;
                    case "x":
                        exitStackMenu = true;
                        break;
                    case "v":
                        // get Stack ID, then get all flashcards with that stack ID assigned to them, display in order by stack ID
                        StackManager.PrintFlashcardsInStack(stackId);
                        Console.WriteLine("Press any key to continue.\n\n");
                        Console.ReadKey();
                        break;
                    case "a":
                        var numFlashcards = Helpers.GetNumFlashcards(stackName);
                        StackManager.PrintFlashcardsInStack(stackId, numFlashcards);
                        Console.WriteLine("Press any key to continue.\n\n");
                        Console.ReadKey();
                        break;
                    case "c":
                        Flashcard flashcard = Helpers.CreateFlashcard(stackId, stackName);
                        DatabaseManager.PostFlashcard(flashcard);
                        Console.WriteLine("Press any key to continue.\n\n");
                        Console.ReadKey();
                        break;
                    case "e":
                        StackManager.EditFlashcard(stackId);
                        Console.WriteLine("Press any key to continue.\n\n");
                        Console.ReadKey();
                        break;


                }
            }
        }

        private static List<FlashcardDTO> PrintFlashcardsInStack(int stackId, int numFlashcards = -1)
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
                    Console.WriteLine("\n\n");
                    return flashcards;
                }
            }
        }
        private static void EditFlashcard(int stackId)
        {
            Console.Clear();
            var flashcards = StackManager.PrintFlashcardsInStack(stackId);
            Console.WriteLine("\n\nEnter ID of flashcard you want to edit.\n\n");

            var userIdInput = Console.ReadLine().Trim();

            while (!Helpers.ValidFlashcardID(userIdInput, flashcards))
            {
                Console.WriteLine("\n\nInvalid input. Please enter the ID of the flashcard you want to edit.\n\n");
                userIdInput = Console.ReadLine().Trim();
            }

            // formats flashcard Id into an int and also adjusts for offset
            var flashcardId = int.Parse(userIdInput) - 1;

            var currentFront = flashcards[flashcardId].Front;
            var currentBack = flashcards[flashcardId].Back;

            Console.Clear();
            Console.WriteLine(currentFront + "\n\nEnter new front for flashcard or press enter to keep it the same.\n\n");
            var newFront = Console.ReadLine().Trim();

            Console.Clear();
            Console.WriteLine(currentBack + "\n\nEnter new back for flashcard or press enter to keep it the same.\n\n");
            var newBack = Console.ReadLine().Trim();

            newFront = string.IsNullOrEmpty(newFront) ? currentFront : newFront;
            newBack = string.IsNullOrEmpty(newBack) ? currentBack : newBack;

            DatabaseManager.UpdateFlashcard(stackId, currentFront, newFront, newBack);
        }

    }
}
