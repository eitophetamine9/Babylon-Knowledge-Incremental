namespace BabylonKnowledgeIncremental.Models;

public class Flashcard
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // Foreign Key to parent scroll
    public Guid? ScrollItemId { get; set; }
    public ScrollItem? Scroll { get; set; }

    public string Tag { get; set; } = string.Empty;
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public bool IsFlipped { get; set; }
    public int OrderIndex { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public void Toggle() => IsFlipped = !IsFlipped;
    public void ResetFlip() => IsFlipped = false;
}
