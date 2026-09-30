namespace BabylonKnowledgeIncremental.Models;

public enum Rarity { Common, Rare, Epic }

public class GachaItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public Rarity Rarity { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Emoji { get; set; } = "✨";

    public string RarityColor => Rarity switch
    {
        Rarity.Epic   => "#D4AF37",
        Rarity.Rare   => "#4A90D9",
        Rarity.Common => "#8B7355",
        _             => "#8B7355"
    };

    public string RarityLabel => Rarity.ToString().ToUpper();

    public string RarityBadgeClass => Rarity switch
    {
        Rarity.Epic   => "badge-epic",
        Rarity.Rare   => "badge-rare",
        Rarity.Common => "badge-common",
        _             => "badge-common"
    };
}
