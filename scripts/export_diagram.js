const fs = require('fs');
const path = require('path');

const mermaidCode = `classDiagram
    direction TB

    class Rarity {
        <<enumeration>>
        Common
        Rare
        Epic
    }

    class DifficultyLevel {
        <<enumeration>>
        Novice
        Adept
        Master
    }

    class ScrollItem {
        +Guid Id [PK]
        +string Title
        +string FileName
        +long FileSizeBytes
        +string Category
        +string Difficulty
        +string Icon
        +string Description
        +List~string~ Tags
        +int EstimatedMinutes
        +DateTime CreatedAt
        +List~Flashcard~ Flashcards
        +List~QuizQuestion~ QuizQuestions
    }

    class Flashcard {
        +Guid Id [PK]
        +Guid ScrollItemId [FK]
        +string Tag
        +string Question
        +string Answer
        +bool IsFlipped
        +int OrderIndex
        +DateTime CreatedAt
        +ScrollItem Scroll
        +Toggle() void
        +ResetFlip() void
    }

    class QuizQuestion {
        +Guid Id [PK]
        +Guid ScrollItemId [FK]
        +string Prompt
        +List~string~ Options
        +int CorrectIndex
        +int SelectedAnswerIndex
        +int OrderIndex
        +DateTime CreatedAt
        +ScrollItem Scroll
        +bool IsAnswered
        +bool IsCorrect
        +SelectAnswer(int) void
        +Reset() void
    }

    class TodoItem {
        +Guid Id [PK]
        +string Title
        +bool IsCompleted
        +DateTime CreatedAt
        +DateTime CompletedAt
        +ToggleCompletion() void
    }

    class StreakState {
        +Guid Id [PK]
        +int CurrentStreak
        +int TotalDays
        +int HighestStreak
        +bool IsBonusTrigger
        +DateTime LastAscentDate
        +DateTime UpdatedAt
        +IncrementDaily() void
        +Reset() void
    }

    class TowerProgression {
        +Guid Id [PK]
        +int CurrentLevel
        +int CurrentEnergy
        +int EnergyRequiredForNextLevel
        +DateTime LastOfferedAt
        +double EnergyPercent
        +string FloorLabel
        +OfferEnergy(int) int
    }

    class GachaItem {
        +Guid Id [PK]
        +string Name
        +Rarity Rarity
        +string Category
        +string Description
        +string Emoji
        +DateTime UnlockedAt
        +string RarityColor
        +string RarityLabel
        +string RarityBadgeClass
    }

    ScrollItem "1" *-- "0..*" Flashcard : contains
    ScrollItem "1" *-- "0..*" QuizQuestion : contains
    GachaItem ..> Rarity : uses
    ScrollItem ..> DifficultyLevel : uses
`;

async function exportDiagram() {
    console.log("Encoding Mermaid Class Diagram...");
    const payload = JSON.stringify({
        code: mermaidCode,
        mermaid: {
            theme: "default"
        }
    });

    const b64 = Buffer.from(payload).toString('base64url');
    const pngUrl = `https://mermaid.ink/img/${b64}?type=png`;
    const svgUrl = `https://mermaid.ink/svg/${b64}`;

    console.log("Fetching rendered PNG from mermaid.ink...");
    const pngRes = await fetch(pngUrl);
    if (!pngRes.ok) {
        throw new Error(`Failed to fetch PNG: ${pngRes.status} ${pngRes.statusText}`);
    }
    const pngBuffer = Buffer.from(await pngRes.arrayBuffer());

    console.log("Fetching rendered SVG from mermaid.ink...");
    const svgRes = await fetch(svgUrl);
    const svgText = await svgRes.text();

    const targets = [
        path.join(__dirname, '..', 'Babylon_ERD_Class_Diagram.png'),
        'C:\\Users\\Erick\\.gemini\\antigravity-ide\\brain\\f9cc9bb8-f693-40bd-9a83-14d136c558de\\Babylon_ERD_Class_Diagram.png'
    ];

    for (const target of targets) {
        fs.mkdirSync(path.dirname(target), { recursive: true });
        fs.writeFileSync(target, pngBuffer);
        console.log(`Saved PNG to: ${target} (${pngBuffer.length} bytes)`);
    }

    const svgTarget = path.join(__dirname, '..', 'Babylon_ERD_Class_Diagram.svg');
    fs.writeFileSync(svgTarget, svgText, 'utf8');
    console.log(`Saved SVG to: ${svgTarget}`);
    console.log("Diagram generation complete!");
}

exportDiagram().catch(err => {
    console.error("Export error:", err);
    process.exit(1);
});
