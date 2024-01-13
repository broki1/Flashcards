using System.Configuration;
using QC = Microsoft.Data.SqlClient;

namespace Flashcards;

internal class Study
{

    private static string connectionString = ConfigurationManager.AppSettings.Get("FlashcardsConnectionString");

    internal static void StudyMenu()
    {
        Console.Clear();
        StacksManager.PrintStacks();

        Console.WriteLine("\nPlease enter the name of the stack you want to study, enter 0 to return to main menu:");
        var userInput = Console.ReadLine().Trim().ToLower();

        if (userInput == "0")
        {
            MainMenu.StartApplication();
        }

        while (!Helpers.StackNameAlreadyExists(userInput))
        {
            Console.Clear();
            StacksManager.PrintStacks();
            Console.WriteLine("Invalid input. Please enter the name of the stack you want to study. Enter 0 to return to the main menu.\n\n");
            userInput = Console.ReadLine().Trim().ToLower();
        }

        userInput = Helpers.FormatStackName(userInput);

        Study.CompleteSession(userInput);


    }

    private static void CompleteSession(string stackName)
    {
        List<string> questions = new List<string>();
        var stackId = DatabaseManager.GetStackId(stackName);

        var exitSession = false;

        while (!exitSession)
        {
            var question = DatabaseManager.GenerateQuestion(questions, stackId);

            if (question != "Study session complete.")
            {
                questions.Add(question);
            } else
            {
                exitSession = true;
            }

            Console.WriteLine(question);

            Console.ReadLine();
        }
        
    }
}
