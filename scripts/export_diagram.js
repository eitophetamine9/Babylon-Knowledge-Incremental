const fs = require('fs');
const path = require('path');

const plantumlCode = `@startuml Babylon_Knowledge_Incremental_ERD
skinparam backgroundColor #FFFFFF
skinparam shadowing false
skinparam roundcorner 8
skinparam defaultFontName Arial
skinparam dpi 150

skinparam class {
    BackgroundColor #FFFFFF
    BorderColor #B8960C
    HeaderBackgroundColor #F4EEDD
    FontSize 12
    FontColor #1A1A2E
    AttributeFontColor #3D3825
    AttributeFontSize 11
}

skinparam arrow {
    Color #1B4965
    FontColor #1B4965
    FontSize 11
}

enum Rarity {
    Common
    Rare
    Epic
}

enum DifficultyLevel {
    Novice
    Adept
    Master
}

class User {
    + Guid Id [PK]
    + string Username
    + string Email
    + int TotalCapsules
    + DateTime CreatedAt
    + DateTime LastActiveAt
}

class StreakState {
    + Guid Id [PK]
    + Guid UserId [FK]
    + int CurrentStreak
    + int TotalDays
    + int HighestStreak
    + bool IsBonusTrigger
    + DateTime LastAscentDate
    + DateTime UpdatedAt
    + IncrementDaily() void
    + Reset() void
}

class TowerProgression {
    + Guid Id [PK]
    + Guid UserId [FK]
    + int CurrentLevel
    + int CurrentEnergy
    + int EnergyRequiredForNextLevel
    + DateTime LastOfferedAt
    + double EnergyPercent
    + string FloorLabel
    + OfferEnergy(int) int
}

class TodoItem {
    + Guid Id [PK]
    + Guid UserId [FK]
    + string Title
    + bool IsCompleted
    + DateTime CreatedAt
    + DateTime CompletedAt
    + ToggleCompletion() void
}

class ScrollItem {
    + Guid Id [PK]
    + Guid UserId [FK]
    + string Title
    + string FileName
    + long FileSizeBytes
    + string Category
    + DifficultyLevel Difficulty
    + string Icon
    + string Description
    + DateTime CreatedAt
}

class Flashcard {
    + Guid Id [PK]
    + Guid ScrollItemId [FK]
    + string Tag
    + string Question
    + string Answer
    + bool IsFlipped
    + int OrderIndex
    + DateTime CreatedAt
    + Toggle() void
    + ResetFlip() void
}

class QuizQuestion {
    + Guid Id [PK]
    + Guid ScrollItemId [FK]
    + string Prompt
    + List<string> Options
    + int CorrectIndex
    + int SelectedAnswerIndex
    + int OrderIndex
    + DateTime CreatedAt
    + SelectAnswer(int) void
    + Reset() void
}

class UserCollectionItem {
    + Guid Id [PK]
    + Guid UserId [FK]
    + Guid GachaItemId [FK]
    + DateTime UnlockedAt
}

class GachaItem {
    + Guid Id [PK]
    + string Name
    + Rarity Rarity
    + string Category
    + string Description
    + string Emoji
}

' Fully Connected Relational Architecture
User "1" *-- "1" StreakState : climbs & tracks >
User "1" *-- "1" TowerProgression : offers to >
User "1" *-- "0..*" TodoItem : manages >
User "1" *-- "0..*" ScrollItem : studies >
User "1" *-- "0..*" UserCollectionItem : owns >

UserCollectionItem "0..*" --> "1" GachaItem : references >

ScrollItem "1" *-- "0..*" Flashcard : contains >
ScrollItem "1" *-- "0..*" QuizQuestion : contains >

ScrollItem ..> DifficultyLevel : classified by
GachaItem ..> Rarity : categorized by

@enduml
`;

async function exportDiagram() {
    console.log("Generating connected ERD/Class Diagram with solid white background...");

    // Fetch PNG
    console.log("Requesting PNG with solid #FFFFFF background...");
    const pngRes = await fetch('https://kroki.io/plantuml/png', {
        method: 'POST',
        headers: { 'Content-Type': 'text/plain' },
        body: plantumlCode
    });

    if (!pngRes.ok) {
        throw new Error(`Kroki PNG error: ${pngRes.status} ${pngRes.statusText}`);
    }
    const pngBuffer = Buffer.from(await pngRes.arrayBuffer());

    // Fetch SVG
    console.log("Requesting vector SVG...");
    const svgRes = await fetch('https://kroki.io/plantuml/svg', {
        method: 'POST',
        headers: { 'Content-Type': 'text/plain' },
        body: plantumlCode
    });

    if (!svgRes.ok) {
        throw new Error(`Kroki SVG error: ${svgRes.status} ${svgRes.statusText}`);
    }
    const svgText = await svgRes.text();

    const targets = [
        path.join(__dirname, '..', 'Babylon_ERD_Class_Diagram.png'),
        'C:\\Users\\Erick\\.gemini\\antigravity-ide\\brain\\f9cc9bb8-f693-40bd-9a83-14d136c558de\\Babylon_ERD_Class_Diagram.png'
    ];

    for (const target of targets) {
        fs.mkdirSync(path.dirname(target), { recursive: true });
        fs.writeFileSync(target, pngBuffer);
        console.log(`Saved PNG (${pngBuffer.length} bytes) to: ${target}`);
    }

    const svgTargets = [
        path.join(__dirname, '..', 'Babylon_ERD_Class_Diagram.svg'),
        'C:\\Users\\Erick\\.gemini\\antigravity-ide\\brain\\f9cc9bb8-f693-40bd-9a83-14d136c558de\\Babylon_ERD_Class_Diagram.svg'
    ];

    for (const target of svgTargets) {
        fs.mkdirSync(path.dirname(target), { recursive: true });
        fs.writeFileSync(target, svgText, 'utf8');
        console.log(`Saved SVG to: ${target}`);
    }

    console.log("Export complete!");
}

exportDiagram().catch(err => {
    console.error("Export error:", err);
    process.exit(1);
});
