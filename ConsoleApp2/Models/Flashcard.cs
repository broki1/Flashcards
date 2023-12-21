namespace Flashcards.Models;

internal class Flashcard
{

    int Id { get; set; }
    public int Stack {  get; set; }
    public string Front { get; set; }
    public string Back { get; set; }

}
