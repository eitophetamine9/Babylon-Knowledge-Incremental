namespace BabylonKnowledgeIncremental.Models;

public class StreakState
{
    public int CurrentStreak { get; private set; } = 3;
    public int TotalDays { get; private set; } = 42;
    public int HighestStreak { get; private set; } = 14;
    public bool IsBonusTrigger { get; private set; }

    /// <summary>
    /// Increments the daily streak, loops at 7 and fires a bonus trigger.
    /// </summary>
    public void IncrementDaily()
    {
        TotalDays++;
        CurrentStreak++;
        IsBonusTrigger = false;

        if (CurrentStreak > HighestStreak)
            HighestStreak = CurrentStreak;

        if (CurrentStreak >= 7)
        {
            IsBonusTrigger = true;
            CurrentStreak = 1; // loop back to start of new week
        }
    }

    /// <summary>
    /// Resets current streak to zero (e.g. missed a day).
    /// </summary>
    public void Reset()
    {
        CurrentStreak = 0;
        IsBonusTrigger = false;
    }
}
