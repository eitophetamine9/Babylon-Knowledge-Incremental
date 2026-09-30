namespace BabylonKnowledgeIncremental.Models;

public class Flashcard
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public bool IsFlipped { get; set; }
    public string Tag { get; set; } = string.Empty;

    public void Toggle() => IsFlipped = !IsFlipped;
    public void ResetFlip() => IsFlipped = false;
}
