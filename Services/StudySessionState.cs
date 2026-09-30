using BabylonKnowledgeIncremental.Models;
using Microsoft.JSInterop;

namespace BabylonKnowledgeIncremental.Services;

/// <summary>
/// Central observable state store for the entire application.
/// Registered as Scoped — all components share one instance per session.
/// Subscribe to OnChange to trigger re-renders; always unsubscribe on Dispose.
/// </summary>
public class StudySessionState
{
    private readonly IJSRuntime _js;

    public StudySessionState(IJSRuntime js)
    {
        _js = js;
        InitializeSampleData();
    }

    // ─── Theme ───────────────────────────────────────────────────────────────
    public bool IsDarkMode { get; private set; }

    public async Task ToggleThemeAsync()
    {
        IsDarkMode = !IsDarkMode;
        await _js.InvokeVoidAsync("babylonTheme.toggle", IsDarkMode);
        NotifyStateChanged();
    }

    public async Task InitializeThemeAsync()
    {
        var saved = await _js.InvokeAsync<string>("babylonTheme.getSaved");
        IsDarkMode = saved == "dark";
        await _js.InvokeVoidAsync("babylonTheme.apply", IsDarkMode);
        NotifyStateChanged();
    }

    // ─── Currency ────────────────────────────────────────────────────────────
    public int TotalCapsules { get; private set; } = 1280;

    private void AwardCapsules(int amount)
    {
        TotalCapsules += amount;
    }

    // ─── Streak ──────────────────────────────────────────────────────────────
    public StreakState Streak { get; private set; } = new();

    public void IncrementStreak()
    {
        Streak.IncrementDaily();
        if (Streak.IsBonusTrigger)
            AwardCapsules(50); // 7-day cycle bonus
        NotifyStateChanged();
    }

    // ─── Todos ───────────────────────────────────────────────────────────────
    public List<TodoItem> Todos { get; private set; } = new();
    public int CompletedCount => Todos.Count(t => t.IsCompleted);
    public double TodayProgress => Todos.Count == 0 ? 0 : (double)CompletedCount / Todos.Count * 100;

    public void AddTodo(string title)
    {
        if (string.IsNullOrWhiteSpace(title)) return;
        Todos.Add(new TodoItem { Title = title.Trim() });
        NotifyStateChanged();
    }

    public void ToggleTodo(Guid id)
    {
        var todo = Todos.FirstOrDefault(t => t.Id == id);
        if (todo is null) return;
        todo.IsCompleted = !todo.IsCompleted;
        NotifyStateChanged();
    }

    public void RemoveTodo(Guid id)
    {
        Todos.RemoveAll(t => t.Id == id);
        NotifyStateChanged();
    }

    // ─── Tower ───────────────────────────────────────────────────────────────
    public TowerProgression Tower { get; private set; } = new();

    public void OfferEnergy()
    {
        var earned = Tower.OfferEnergy(50);
        AwardCapsules(earned);
        NotifyStateChanged();
    }

    // ─── Study Content ───────────────────────────────────────────────────────
    public string? LoadedFileName { get; private set; }
    public long LoadedFileSizeBytes { get; private set; }
    public bool HasLoadedFile => LoadedFileName is not null;
    public string LoadedFileSizeLabel =>
        LoadedFileSizeBytes >= 1_048_576
            ? $"{LoadedFileSizeBytes / 1_048_576.0:F1} MB"
            : $"{LoadedFileSizeBytes / 1024} KB";

    public List<Flashcard> Flashcards { get; private set; } = new();
    public List<QuizQuestion> QuizQuestions { get; private set; } = new();
    public int CurrentCardIndex { get; private set; }
    public bool IsFlashcardMode { get; private set; } = true;

    public Flashcard? CurrentFlashcard =>
        Flashcards.Count > 0 && CurrentCardIndex < Flashcards.Count
            ? Flashcards[CurrentCardIndex] : null;

    public QuizQuestion? CurrentQuizQuestion =>
        QuizQuestions.Count > 0 && CurrentCardIndex < QuizQuestions.Count
            ? QuizQuestions[CurrentCardIndex] : null;

    public int TotalCards => IsFlashcardMode ? Flashcards.Count : QuizQuestions.Count;

    public void SimulateFileLoad(string filename, long sizeBytes)
    {
        LoadedFileName = filename;
        LoadedFileSizeBytes = sizeBytes;
        LoadStudySetForFile(filename);
        NotifyStateChanged();
    }

    public void ClearFile()
    {
        LoadedFileName = null;
        LoadedFileSizeBytes = 0;
        NotifyStateChanged();
    }

    public void ActivateFlashcards()
    {
        IsFlashcardMode = true;
        CurrentCardIndex = 0;
        foreach (var card in Flashcards) card.ResetFlip();
        NotifyStateChanged();
    }

    public void ActivateQuiz()
    {
        IsFlashcardMode = false;
        CurrentCardIndex = 0;
        NotifyStateChanged();
    }

    public void SwitchToFlashcards()
    {
        IsFlashcardMode = true;
        CurrentCardIndex = 0;
        foreach (var card in Flashcards) card.ResetFlip();
        NotifyStateChanged();
    }

    public void SwitchToQuiz()
    {
        IsFlashcardMode = false;
        CurrentCardIndex = 0;
        NotifyStateChanged();
    }

    public void FlipCurrentCard()
    {
        CurrentFlashcard?.Toggle();
        NotifyStateChanged();
    }

    public void NextCard()
    {
        if (IsFlashcardMode && Flashcards.Count > 0)
        {
            Flashcards[CurrentCardIndex].ResetFlip();
            CurrentCardIndex = (CurrentCardIndex + 1) % Flashcards.Count;
        }
        else if (!IsFlashcardMode && QuizQuestions.Count > 0)
        {
            CurrentCardIndex = (CurrentCardIndex + 1) % QuizQuestions.Count;
        }
        NotifyStateChanged();
    }

    public void SelectQuizAnswer(int index)
    {
        if (CurrentQuizQuestion is not null && !CurrentQuizQuestion.IsAnswered)
        {
            CurrentQuizQuestion.SelectedAnswerIndex = index;
            NotifyStateChanged();
        }
    }

    // ─── Gacha ───────────────────────────────────────────────────────────────
    public List<GachaItem> Collection { get; private set; } = new();
    public GachaItem? LastPullResult { get; private set; }
    public bool ShowPullModal { get; private set; }

    public void PullGacha(string category, int count)
    {
        var pool = _gachaPool.Where(g => g.Category == category).ToList();
        if (pool.Count == 0) return;

        // Deduct cost
        int cost = count * 100;
        if (TotalCapsules < cost) return;
        TotalCapsules -= cost;

        GachaItem? lastItem = null;
        for (int i = 0; i < count; i++)
        {
            var roll = Random.Shared.NextDouble();
            Rarity rarity = roll < 0.045 ? Rarity.Epic
                          : roll < 0.225 ? Rarity.Rare
                          : Rarity.Common;

            var candidates = pool.Where(g => g.Rarity == rarity).ToList();
            if (candidates.Count == 0) candidates = pool;

            lastItem = candidates[Random.Shared.Next(candidates.Count)];
            if (!Collection.Any(c => c.Id == lastItem.Id))
                Collection.Add(lastItem);
        }

        LastPullResult = lastItem;
        ShowPullModal = true;
        NotifyStateChanged();
    }

    public void ClosePullModal()
    {
        ShowPullModal = false;
        LastPullResult = null;
        NotifyStateChanged();
    }

    // ─── Notifications ───────────────────────────────────────────────────────
    public event Action? OnChange;
    private void NotifyStateChanged() => OnChange?.Invoke();

    // ─── Sample Data ─────────────────────────────────────────────────────────
    private void InitializeSampleData()
    {
        Todos = new List<TodoItem>
        {
            new() { Title = "Review basic syntax notes",       IsCompleted = true  },
            new() { Title = "Complete interface flashcards",   IsCompleted = true  },
            new() { Title = "Take the C# fundamentals quiz",   IsCompleted = false },
            new() { Title = "Summarize generics chapter",      IsCompleted = false },
            new() { Title = "Share questions with mentor",     IsCompleted = false },
        };

        Flashcards = new List<Flashcard>
        {
            new() { Tag = "C# · INTERFACES", Question = "What is an interface in C#?",
                    Answer = "An interface defines a contract that classes or structs must implement. It declares method signatures, properties, events, and indexers but contains no implementation." },
            new() { Tag = "C# · OOP", Question = "Abstract class vs interface — key difference?",
                    Answer = "Abstract classes can have implementation, constructors, and fields. Interfaces only define contracts. A class can implement multiple interfaces but inherit only one abstract class." },
            new() { Tag = "C# · DELEGATES", Question = "What is a delegate in C#?",
                    Answer = "A delegate is a type-safe function pointer that holds a reference to a method with a specific signature. It enables callbacks, events, and higher-order functions." },
            new() { Tag = "C# · LINQ", Question = "Explain LINQ and its purpose.",
                    Answer = "LINQ (Language Integrated Query) provides a unified, SQL-like syntax to query collections, databases, XML, and more directly within C# code." },
            new() { Tag = "C# · ASYNC", Question = "What does async/await do in C#?",
                    Answer = "async marks a method as asynchronous; await suspends execution until a Task completes without blocking the thread. This enables responsive, non-blocking code." },
        };

        QuizQuestions = new List<QuizQuestion>
        {
            new() { Prompt = "Which symbol implements an interface in C#?",
                    Options = new() { "extends", "implements", ":", "inherits" }, CorrectIndex = 2 },
            new() { Prompt = "What is the return type of an async method that returns nothing useful?",
                    Options = new() { "void", "Task", "async", "ValueTask" }, CorrectIndex = 1 },
            new() { Prompt = "Which of the following is NOT a valid C# access modifier?",
                    Options = new() { "private", "internal", "package", "protected" }, CorrectIndex = 2 },
        };

        LoadedFileName = "C# Basic Syntax.pdf";
        LoadedFileSizeBytes = 2_400_000;
    }

    private void LoadStudySetForFile(string filename)
    {
        CurrentCardIndex = 0;
        if (filename.Contains("Babylon", StringComparison.OrdinalIgnoreCase))
        {
            Flashcards = new List<Flashcard>
            {
                new() { Tag = "MESOPOTAMIA · ARCHITECTURE", Question = "What was the primary function of a Babylonian Ziggurat?",
                        Answer = "A ziggurat was a monumental terraced temple tower built to bridge heaven and earth, honoring the city's patron deity (such as Marduk in Babylon)." },
                new() { Tag = "BABYLON · CIVILIZATION", Question = "What was the significance of the Code of Hammurabi?",
                        Answer = "One of the earliest and most complete written legal codes, inscribed on a diorite stele, establishing the principle of lex talionis ('an eye for an eye')." },
                new() { Tag = "BABYLON · WONDERS", Question = "What were the Hanging Gardens of Babylon celebrated for?",
                        Answer = "One of the Seven Wonders of the Ancient World, described as ascending tiered gardens irrigated by an ingenious screw or bucket system from the Euphrates." },
                new() { Tag = "MESOPOTAMIA · SCRIPT", Question = "What writing system was developed in ancient Mesopotamia?",
                        Answer = "Cuneiform: wedge-shaped impressions pressed into wet clay tablets with a reed stylus, used for over three millennia." }
            };

            QuizQuestions = new List<QuizQuestion>
            {
                new() { Prompt = "Which river flowed through the heart of ancient Babylon?",
                        Options = new() { "Nile", "Euphrates", "Tigris", "Danube" }, CorrectIndex = 1 },
                new() { Prompt = "Who was the chief patron deity of Babylon?",
                        Options = new() { "Anu", "Enlil", "Marduk", "Ishtar" }, CorrectIndex = 2 },
                new() { Prompt = "The famous Ishtar Gate was adorned with glazed blue bricks and depictions of which creatures?",
                        Options = new() { "Lions, bulls, and dragons (Mušḫuššu)", "Eagles and serpents", "Horses and chariots", "Sphinxes" }, CorrectIndex = 0 }
            };
        }
        else if (filename.Contains("Algorithm", StringComparison.OrdinalIgnoreCase) || filename.Contains("Complexity", StringComparison.OrdinalIgnoreCase))
        {
            Flashcards = new List<Flashcard>
            {
                new() { Tag = "ALGORITHMS · COMPLEXITY", Question = "What is the time complexity of binary search on a sorted array?",
                        Answer = "O(log n) because the search space is halved at each comparison step." },
                new() { Tag = "DATA STRUCTURES · HASHING", Question = "What is average lookup time in a hash table?",
                        Answer = "O(1) average time complexity, assuming a good hash function and minimal collision clustering." },
                new() { Tag = "ALGORITHMS · SORTING", Question = "What is the worst-case time complexity of QuickSort?",
                        Answer = "O(n²) when the pivot chosen is consistently the smallest or largest element, e.g. on already sorted arrays without random pivots." }
            };

            QuizQuestions = new List<QuizQuestion>
            {
                new() { Prompt = "Which data structure follows First-In, First-Out (FIFO) ordering?",
                        Options = new() { "Stack", "Queue", "Binary Tree", "Heap" }, CorrectIndex = 1 },
                new() { Prompt = "What is the worst-case time complexity of Merge Sort?",
                        Options = new() { "O(n)", "O(n log n)", "O(n²)", "O(log n)" }, CorrectIndex = 1 },
            };
        }
        else if (filename.Contains("C#", StringComparison.OrdinalIgnoreCase) || filename.Contains("Syntax", StringComparison.OrdinalIgnoreCase))
        {
            Flashcards = new List<Flashcard>
            {
                new() { Tag = "C# · INTERFACES", Question = "What is an interface in C#?",
                        Answer = "An interface defines a contract that classes or structs must implement. It declares method signatures, properties, events, and indexers but contains no implementation." },
                new() { Tag = "C# · OOP", Question = "Abstract class vs interface — key difference?",
                        Answer = "Abstract classes can have implementation, constructors, and fields. Interfaces only define contracts. A class can implement multiple interfaces but inherit only one abstract class." },
                new() { Tag = "C# · DELEGATES", Question = "What is a delegate in C#?",
                        Answer = "A delegate is a type-safe function pointer that holds a reference to a method with a specific signature. It enables callbacks, events, and higher-order functions." },
                new() { Tag = "C# · LINQ", Question = "Explain LINQ and its purpose.",
                        Answer = "LINQ (Language Integrated Query) provides a unified, SQL-like syntax to query collections, databases, XML, and more directly within C# code." },
                new() { Tag = "C# · ASYNC", Question = "What does async/await do in C#?",
                        Answer = "async marks a method as asynchronous; await suspends execution until a Task completes without blocking the thread. This enables responsive, non-blocking code." }
            };

            QuizQuestions = new List<QuizQuestion>
            {
                new() { Prompt = "Which symbol implements an interface in C#?",
                        Options = new() { "extends", "implements", ":", "inherits" }, CorrectIndex = 2 },
                new() { Prompt = "What is the return type of an async method that returns nothing useful?",
                        Options = new() { "void", "Task", "async", "ValueTask" }, CorrectIndex = 1 },
                new() { Prompt = "Which of the following is NOT a valid C# access modifier?",
                        Options = new() { "private", "internal", "package", "protected" }, CorrectIndex = 2 },
            };
        }
        else
        {
            var cleanName = System.IO.Path.GetFileNameWithoutExtension(filename);
            Flashcards = new List<Flashcard>
            {
                new() { Tag = $"{cleanName.ToUpperInvariant()} · OVERVIEW", Question = $"What are the core concepts covered in {filename}?",
                        Answer = $"This study set was generated from your uploaded scroll '{filename}'. Use flashcards to drill key definitions and quiz mode to test your retention." },
                new() { Tag = $"{cleanName.ToUpperInvariant()} · ESSENTIALS", Question = $"How do you maximize retention when studying {cleanName}?",
                        Answer = "Utilize active recall, spaced repetition, and incrementally push your knowledge boulder up the mountain every day!" },
                new() { Tag = $"{cleanName.ToUpperInvariant()} · SYNTHESIS", Question = $"What are the foundational principles in this scroll?",
                        Answer = $"Review the primary arguments and structural paradigms outlined in '{filename}' to reinforce master-level comprehension." }
            };

            QuizQuestions = new List<QuizQuestion>
            {
                new() { Prompt = $"Have you reviewed all foundational chapters in '{filename}'?",
                        Options = new() { "Yes, ready for the trial", "Currently reviewing", "Need deeper analysis", "Just started" }, CorrectIndex = 0 },
                new() { Prompt = "Which study technique gives the highest incremental knowledge gain?",
                        Options = new() { "Passive reading", "Active recall & retrieval", "Highlighting text", "Cramming" }, CorrectIndex = 1 }
            };
        }
    }

    // ─── Gacha Pool ──────────────────────────────────────────────────────────
    private static readonly List<GachaItem> _gachaPool = new()
    {
        // Aesthetics
        new() { Name = "Spartan Dark Mode",       Rarity = Rarity.Epic,   Category = "Aesthetics",   Emoji = "⚔️",  Description = "A midnight workspace forged in bronze, lapis, and disciplined contrast." },
        new() { Name = "Marble Light Theme",       Rarity = Rarity.Rare,   Category = "Aesthetics",   Emoji = "🏛️", Description = "Pure white marble with gold veining for a classic Hellenistic study hall." },
        new() { Name = "Lapis Lazuli Dark",        Rarity = Rarity.Epic,   Category = "Aesthetics",   Emoji = "💎",  Description = "Deep Babylonian blue inspired by royal lapis inlay mosaics." },
        new() { Name = "Papyrus Scroll",           Rarity = Rarity.Common, Category = "Aesthetics",   Emoji = "📜",  Description = "An aged papyrus texture for your study cards and panels." },
        new() { Name = "Sandstone Warm",           Rarity = Rarity.Common, Category = "Aesthetics",   Emoji = "🟫",  Description = "Warm desert tones from Mesopotamian sun-dried brick." },
        // Study Tools
        new() { Name = "Oracle Hints",             Rarity = Rarity.Epic,   Category = "Study Tools",  Emoji = "🔮",  Description = "AI-powered contextual hints that appear on difficult flashcards." },
        new() { Name = "Pomodoro Hourglass",       Rarity = Rarity.Rare,   Category = "Study Tools",  Emoji = "⏱️",  Description = "A classical focus timer to structure your study sessions." },
        new() { Name = "Speed Reader Mode",        Rarity = Rarity.Rare,   Category = "Study Tools",  Emoji = "⚡",  Description = "Flash individual words for rapid reading and comprehension practice." },
        new() { Name = "Scholar's Notebook",       Rarity = Rarity.Common, Category = "Study Tools",  Emoji = "📓",  Description = "An in-app freeform note-taking scroll for quick annotations." },
        // Audio Packs
        new() { Name = "Oracle's Voice",           Rarity = Rarity.Epic,   Category = "Audio Packs",  Emoji = "🎙️",  Description = "Ceremonial narration voice for your flashcard answers." },
        new() { Name = "Hanging Gardens Ambience", Rarity = Rarity.Rare,   Category = "Audio Packs",  Emoji = "🌿",  Description = "Ambient nature sounds from the legendary Babylonian gardens." },
        new() { Name = "Agora Marketplace",        Rarity = Rarity.Common, Category = "Audio Packs",  Emoji = "🎵",  Description = "Soft marketplace sounds of a bustling ancient Athenian agora." },
    };
}
