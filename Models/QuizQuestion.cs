namespace BabylonKnowledgeIncremental.Models;

public class QuizQuestion
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Prompt { get; set; } = string.Empty;
    public List<string> Options { get; set; } = new();
    public int CorrectIndex { get; set; }
    public int SelectedAnswerIndex { get; set; } = -1;

    public bool IsAnswered => SelectedAnswerIndex >= 0;
    public bool IsCorrect => IsAnswered && SelectedAnswerIndex == CorrectIndex;

    public void Reset() => SelectedAnswerIndex = -1;
}
