namespace Flashcards.Models;

internal class StudySession
{
    public int Stack { get; set; }
    public string Date { get; set; }
    public int Correct {  get; set; }
    public int Total { get; set; }

    public StudySession()
    {
        Date = DateTime.Now.ToString("yyyy-MM-dd");
        Correct = 0;
        Total = 0;
    }

}
