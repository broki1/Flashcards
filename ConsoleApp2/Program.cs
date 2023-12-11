using System.Configuration;
using SSClient = Microsoft.Data.SqlClient;

namespace Flashcards;

internal class Program
{
    static void Main(string[] args)
    {
        var connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");

        using (var connection = new SSClient.SqlConnection(connectionString))
        {
            connection.Open();

            Console.WriteLine("Connection opened successfully...\n\nPress any key to finish...");

            Console.ReadKey();
        }
    }
}
