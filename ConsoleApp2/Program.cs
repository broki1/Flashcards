using System.Configuration;
using SSClient = Microsoft.Data.SqlClient;

namespace Flashcards;

internal class Program
{
    static void Main(string[] args)
    {
        Helpers.CreateDatabaseAndTables();

        MainMenu.StartApplication();
    }
}
