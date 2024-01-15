using Flashcards.Models;
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
        var studySession = new StudySession();

        List<string> questions = new List<string>();
        var stackId = DatabaseManager.GetStackId(stackName);

        studySession.Stack = stackId;

        var exitSession = false;

        while (!exitSession)
        {
            Console.Clear();

            if (questions.Count == Helpers.TotalFlashcardsInStack(stackName))
            {
                questions.Clear();
            }

            var studyFlashcard = new List<StudyFlashcardDTO>();

            var question = DatabaseManager.GenerateQuestion(questions, stackId);


            questions.Add(question);

            Helpers.DisplayQuestion(question, studyFlashcard, stackName);

            var answer = Helpers.GetFlashcardAnswer();

            if (answer == "0")
            {
                exitSession = true;
            }

            else
            {
                var correct = Study.ProcessAnswer(question, answer);

                studySession.Total++;

                if (correct)
                {
                    studySession.Correct++;
                }

                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
            }
        }

        Console.WriteLine($"\n\nExiting Study session\nYou got {studySession.Correct} right out of {studySession.Total}\nPress any key to continue");
        Console.ReadKey();
        DatabaseManager.PostStudySession(studySession, connectionString);

    }

    private static bool ProcessAnswer(string question, string userAnswer)
    {
        bool correct;

        using (var connection = new QC.SqlConnection(connectionString))
        {
            using (var command = new QC.SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = $"SELECT back FROM Flashcards WHERE front = '{question}'";

                var answer = Convert.ToString(command.ExecuteScalar());

                if (answer.Equals(userAnswer, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("\n\nCorrect!");
                    correct = true;
                }

                else
                {
                    Console.WriteLine($"\n\nYour answer was wrong.\nYou answered {userAnswer}\nThe correct answer was {answer}\n");
                    correct = false;
                }
            }
        }

        return correct;
    }
}
