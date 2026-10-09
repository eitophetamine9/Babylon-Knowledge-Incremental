namespace BabylonKnowledgeIncremental.Models;

public class StreakState
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int CurrentStreak { get; set; } = 3;
    public int TotalDays { get; set; } = 42;
    public int HighestStreak { get; set; } = 14;
    public bool IsBonusTrigger { get; set; }
    public DateTime? LastAscentDate { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Increments the daily streak, loops at 7 and fires a bonus trigger.
    /// </summary>
    public void IncrementDaily()
    {
        TotalDays++;
        CurrentStreak++;
        IsBonusTrigger = false;
        LastAscentDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        if (CurrentStreak > HighestStreak)
            HighestStreak = CurrentStreak;

        if (CurrentStreak >= 7)
        {
            IsBonusTrigger = true;
            CurrentStreak = 1; // cycle restart
        }
    }

    /// <summary>
    /// Resets current streak to zero (e.g. missed a day).
    /// </summary>
    public void Reset()
    {
        CurrentStreak = 0;
        IsBonusTrigger = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
