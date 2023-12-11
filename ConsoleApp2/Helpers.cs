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
    }

}
