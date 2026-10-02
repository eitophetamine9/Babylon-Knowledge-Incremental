namespace BabylonKnowledgeIncremental.Models;

public class ScrollItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Title { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string Category { get; set; } = "General";
    public string Difficulty { get; set; } = "Novice"; // Novice, Adept, Master
    public string Icon { get; set; } = "📜";
    public string Description { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public int EstimatedMinutes { get; set; } = 15;
    public int FlashcardCount { get; set; } = 5;
    public int QuizQuestionCount { get; set; } = 3;
    public List<string> SampleQuestions { get; set; } = new();
}
