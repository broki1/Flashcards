namespace Flashcards.Models;

internal class StudySession
{

    int Id { get; set; }
    public string Stack { get; set; }
    public DateTime Date { get; set; }
    public int Correct {  get; set; }
    public int Total { get; set; }

}
