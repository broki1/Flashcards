using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flashcards;

internal class MainMenu
{

    internal static void StartApplication()
    {
        var endApp = false;

        while (!endApp)
        {
            Console.WriteLine("------------------------");
            Console.WriteLine("exit");
            Console.WriteLine("Manage Stacks");
            Console.WriteLine("Manage Flashcards");
            Console.WriteLine("Study");
            Console.WriteLine("view study session data");
            Console.WriteLine("------------------------\n\n");

            var userInput = Console.ReadLine().Trim().ToLower();

            switch (userInput)
            {
                case "exit":
                    Console.WriteLine("\n\nGoodbye.");
                    Environment.Exit(0);
                    endApp = true;
                    break;
                case "manage stacks":
                    StacksManager.ManageStacks();
                    break;
                default:
                    Console.WriteLine("\n\nInvalid input. Please try again.\n\n");
                    break;
            }
        }
    }

}
