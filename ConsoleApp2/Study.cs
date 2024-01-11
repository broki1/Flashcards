namespace Flashcards;

internal class Study
{

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

        Console.WriteLine($"\nSUCCESS! You've chosen to study {userInput}");
        Console.ReadKey();


    }

}
