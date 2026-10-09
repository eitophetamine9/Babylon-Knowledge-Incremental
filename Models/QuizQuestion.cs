namespace BabylonKnowledgeIncremental.Models;

public class QuizQuestion
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // Foreign Key to parent scroll
    public Guid? ScrollItemId { get; set; }
    public ScrollItem? Scroll { get; set; }

    public string Prompt { get; set; } = string.Empty;
    public List<string> Options { get; set; } = new();
    public int CorrectIndex { get; set; }
    public int SelectedAnswerIndex { get; set; } = -1;
    public int OrderIndex { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsAnswered => SelectedAnswerIndex >= 0;
    public bool IsCorrect => IsAnswered && SelectedAnswerIndex == CorrectIndex;

    public void SelectAnswer(int index) => SelectedAnswerIndex = index;
    public void Reset() => SelectedAnswerIndex = -1;
}
