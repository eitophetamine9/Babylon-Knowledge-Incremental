namespace BabylonKnowledgeIncremental.Models;

public class TowerProgression
{
    public int CurrentLevel { get; private set; } = 24;
    public int CurrentEnergy { get; private set; } = 760;
    public int EnergyRequiredForNextLevel { get; private set; } = 1000;

    public double EnergyPercent =>
        Math.Min(100.0, (double)CurrentEnergy / EnergyRequiredForNextLevel * 100.0);

    public string FloorLabel => $"FLOOR {CurrentLevel}";

    /// <summary>
    /// Adds energy to the tower and checks for a level-up.
    /// Returns the number of capsules earned from this offer.
    /// </summary>
    public int OfferEnergy(int amount = 50)
    {
        int capsulesEarned;
        CurrentEnergy += amount;

        if (CurrentEnergy >= EnergyRequiredForNextLevel)
        {
            CurrentLevel++;
            CurrentEnergy -= EnergyRequiredForNextLevel;
            EnergyRequiredForNextLevel = (int)(EnergyRequiredForNextLevel * 1.2);
            capsulesEarned = 10; // level-up bonus
        }
        else
        {
            capsulesEarned = 1;
        }

        return capsulesEarned;
    }
}
