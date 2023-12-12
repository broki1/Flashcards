using QC = Microsoft.Data.SqlClient;
using System.Configuration;
using Flashcards.Models;
using ConsoleTableExt;

namespace Flashcards;

internal class StacksManager
{
    private static List<Stack> stacks = new List<Stack>();

    private static string connectionString = ConfigurationManager.AppSettings.Get("FlashcardsConnectionString");
    internal static void ManageStacks()
    {
        StacksManager.PrintStacks();
    }

    internal static void PrintStacks()
    {
        using (var connection = new QC.SqlConnection(connectionString))
        {
            using (var command = new QC.SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = @"SELECT name FROM Stacks ORDER BY id";

                var reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        stacks.Add(new Stack
                        {
                            Name = reader.GetString(0)
                        });
                    }
                }
                else
                {
                    Console.WriteLine("\n\nNo rows found.");
                }

                ConsoleTableBuilder.From(stacks).ExportAndWriteLine();
                Console.WriteLine("\n\n");

            }
        }
    }
}
