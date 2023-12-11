using System.Configuration;
using SSClient = Microsoft.Data.SqlClient;

namespace Flashcards;

internal class Program
{
    static void Main(string[] args)
    {
        // stores connection string to master DB
        var connectionString = ConfigurationManager.AppSettings.Get("MasterConnectionString");

        // calls method that creates Flashcard DB if it doesn't already exist on the server
        DatabaseManager.CreateDatabase(connectionString);
    }
}
