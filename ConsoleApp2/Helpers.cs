using ConsoleTableExt;
using Flashcards.Models;
using System.Configuration;

namespace Flashcards;

internal class Helpers
{

    internal static void CreateDatabaseAndTables()
    {
        // stores connection string to master DB
        var masterConnectionString = ConfigurationManager.AppSettings.Get("MasterConnectionString");

        // calls method that creates Flashcard DB if it doesn't already exist on the server
        DatabaseManager.CreateDatabase(masterConnectionString);

        // store connection to Flashcards DB
        var flashcardsConnectionString = ConfigurationManager.AppSettings.Get("FlashcardsConnectionString");

        // calls method that creates Stacks and Flashcards tables if they do not already exist in the DB
        DatabaseManager.CreateStacksTable(flashcardsConnectionString);
        DatabaseManager.CreateFlashcardsTable(flashcardsConnectionString);
        DatabaseManager.CreateStudySessionsTable(flashcardsConnectionString);
    }

    internal static Flashcard CreateFlashcard(int stackId, string stackName)
    {
        Console.Clear();
        Console.WriteLine($"Enter the front (question) of the {stackName} flashcard:\n");

        var front = Console.ReadLine().Trim();

        Console.WriteLine($"\n\nEnter the back (answer) of the {stackName} flashcard:\n");

        var back = Console.ReadLine().Trim();

        return new Flashcard
        {
            Stack = stackId,
            Front = front,
            Back = back
        };

        Console.Clear();
    }

    internal static void DisplayQuestion(string question, List<StudyFlashcardDTO> studyFlashcard, string stackName)
    {
        var flashcard = new StudyFlashcardDTO
        {
            Front = question
        };

        studyFlashcard.Add(flashcard);

        ConsoleTableBuilder.From(studyFlashcard).WithTitle(stackName).WithFormat(ConsoleTableBuilderFormat.Alternative).ExportAndWriteLine();

        Console.WriteLine("\n\nInput your answer to this card (use the same punctuation)\nOr 0 to exit\n");
    }

    // formats parameter string so that first letter is capitalized and the rest of the characters are lowercase
    internal static string FormatStackName(string stackName)
    {
        return char.ToUpper(stackName[0]) + stackName.Substring(1).ToLower();
    }

    internal static string GetFlashcardAnswer()
    {
        var answer = Console.ReadLine().Trim();

        return answer;
    }

    internal static int GetNumFlashcards(string stackName)
    {
        Console.WriteLine($"\nHow many flashcards from the {stackName} stack do you want to display?\nInput a number 0 or higher, or press enter to see all flashcards.");

        var userInput = Console.ReadLine().Trim();

        if (string.IsNullOrEmpty(userInput))
        {
            return -1;
        }

        while (!int.TryParse(userInput, out _) || int.Parse(userInput) < 0)
        {
            Console.WriteLine("\n\nInvalid input. Please input a number 0 or higher, or press enter to see all flashcards.");
            userInput = Console.ReadLine().Trim();

            if (string.IsNullOrEmpty(userInput))
            {
                return -1;
            }
        }

        return int.Parse(userInput);
    }

    // checks StackNames list if stack name inputted by the user already exists
    internal static bool StackNameAlreadyExists(string userInput)
    {
        bool validInput = false;

        var connectionString = ConfigurationManager.AppSettings.Get("FlashcardsConnectionString");


        using (var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString))
        {
            using (var command = new Microsoft.Data.SqlClient.SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "SELECT name FROM Stacks";

                var reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var stackName = reader.GetString(0).Trim().ToLower();

                        if (userInput.Equals(stackName))
                        {
                            validInput = true;
                        }
                    }
                }

            }
        }

        return validInput;
    }

    internal static int TotalFlashcardsInStack(string stackName)
    {
        int stackId;

        using (var connection = new Microsoft.Data.SqlClient.SqlConnection(ConfigurationManager.AppSettings.Get("FlashcardsConnectionString")))
        {
            using (var command = new Microsoft.Data.SqlClient.SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = $"SELECT COUNT(*) FROM Flashcards WHERE stack = {DatabaseManager.GetStackId(stackName)}";

                stackId = Convert.ToInt32(command.ExecuteScalar());
            }
        }

        return stackId;
    }

    internal static bool ValidFlashcardID(string userInput, List<FlashcardDTO> flashcards)
    {
        try
        {
            flashcards[int.Parse(userInput) - 1].Front.Trim();
        }
        catch (Exception ex)
        {
            return false;
        }

        return true;
    }
}
