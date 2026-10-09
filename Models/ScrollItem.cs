namespace BabylonKnowledgeIncremental.Models;

public enum DifficultyLevel
{
    Novice,
    Adept,
    Master
}

public class ScrollItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string Category { get; set; } = "General";
    public string Difficulty { get; set; } = "Novice";
    public string Icon { get; set; } = "📜";
    public string Description { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public int EstimatedMinutes { get; set; } = 15;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation collections for relational database mapping
    public List<Flashcard> Flashcards { get; set; } = new();
    public List<QuizQuestion> QuizQuestions { get; set; } = new();
    public List<string> SampleQuestions { get; set; } = new();

    private int _flashcardCount = 5;
    public int FlashcardCount
    {
        get => Flashcards.Count > 0 ? Flashcards.Count : _flashcardCount;
        set => _flashcardCount = value;
    }

    private int _quizQuestionCount = 3;
    public int QuizQuestionCount
    {
        get => QuizQuestions.Count > 0 ? QuizQuestions.Count : _quizQuestionCount;
        set => _quizQuestionCount = value;
    }
}
