const fs = require('fs');
const path = require('path');

const umlClassDiagramCode = `@startuml Babylon_UML_Class_Diagram
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

skinparam package {
    BackgroundColor #FDFCFB
    BorderColor #1B4965
    FontColor #1B4965
    FontSize 13
}

skinparam arrow {
    Color #1B4965
    FontColor #1B4965
    FontSize 11
}

package "Domain Models" {
    enum Rarity {
        + Common
        + Rare
        + Epic
    }

    enum DifficultyLevel {
        + Novice
        + Adept
        + Master
    }

    class User {
        + Guid Id
        + string Username
        + string Email
        + int TotalCapsules
        + DateTime CreatedAt
        + DateTime LastActiveAt
    }

    class ScrollItem {
        + Guid Id
        + Guid? UserId
        + string Title
        + string FileName
        + long FileSizeBytes
        + string Category
        + string Difficulty
        + string Icon
        + string Description
        + List<string> Tags
        + int EstimatedMinutes
        + DateTime CreatedAt
        + List<Flashcard> Flashcards
        + List<QuizQuestion> QuizQuestions
        + int FlashcardCount
        + int QuizQuestionCount
    }

    class Flashcard {
        + Guid Id
        + Guid? ScrollItemId
        + ScrollItem Scroll
        + string Tag
        + string Question
        + string Answer
        + bool IsFlipped
        + int OrderIndex
        + DateTime CreatedAt
        + Toggle() : void
        + ResetFlip() : void
    }

    class QuizQuestion {
        + Guid Id
        + Guid? ScrollItemId
        + ScrollItem Scroll
        + string Prompt
        + List<string> Options
        + int CorrectIndex
        + int SelectedAnswerIndex
        + int OrderIndex
        + DateTime CreatedAt
        + bool IsAnswered
        + bool IsCorrect
        + SelectAnswer(int index) : void
        + Reset() : void
    }

    class TodoItem {
        + Guid Id
        + Guid? UserId
        + string Title
        + bool IsCompleted
        + DateTime CreatedAt
        + DateTime? CompletedAt
        + ToggleCompletion() : void
    }

    class StreakState {
        + Guid Id
        + Guid? UserId
        + int CurrentStreak
        + int TotalDays
        + int HighestStreak
        + bool IsBonusTrigger
        + DateTime? LastAscentDate
        + DateTime UpdatedAt
        + IncrementDaily() : void
        + Reset() : void
    }

    class TowerProgression {
        + Guid Id
        + Guid? UserId
        + int CurrentLevel
        + int CurrentEnergy
        + int EnergyRequiredForNextLevel
        + DateTime LastOfferedAt
        + double EnergyPercent
        + string FloorLabel
        + OfferEnergy(int amount) : int
    }

    class GachaItem {
        + Guid Id
        + string Name
        + Rarity Rarity
        + string Category
        + string Description
        + string Emoji
        + DateTime? UnlockedAt
        + string RarityColor
        + string RarityLabel
        + string RarityBadgeClass
    }

    class UserCollectionItem {
        + Guid Id
        + Guid UserId
        + Guid GachaItemId
        + DateTime UnlockedAt
    }
}

package "Services & Application State" {
    class StudySessionState {
        - IJSRuntime _js
        + bool IsDarkMode
        + int TotalCapsules
        + StreakState Streak
        + TowerProgression Tower
        + List<TodoItem> Todos
        + List<Flashcard> Flashcards
        + List<QuizQuestion> QuizQuestions
        + List<GachaItem> Collection
        + string LoadedFileName
        + long LoadedFileSizeBytes
        + bool HasLoadedFile
        + int CurrentCardIndex
        + bool IsFlashcardMode
        + event Action OnChange
        + ToggleThemeAsync() : Task
        + AwardCapsules(int amount) : void
        + IncrementStreak() : void
        + AddTodo(string title) : void
        + ToggleTodo(Guid id) : void
        + OfferEnergy() : void
        + SimulateFileLoad(string filename, long sizeBytes) : void
        + ClearFile() : void
        + ActivateFlashcards() : void
        + ActivateQuiz() : void
        + PullGacha(string category, int count) : void
        + ResetQuiz() : void
    }
}

' Structural Associations & Connectivity
User "1" *-- "1" StreakState : tracks >
User "1" *-- "1" TowerProgression : climbs >
User "1" *-- "0..*" TodoItem : manages >
User "1" *-- "0..*" ScrollItem : studies >
User "1" *-- "0..*" UserCollectionItem : owns >

UserCollectionItem "0..*" --> "1" GachaItem : unlocks >

ScrollItem "1" *-- "0..*" Flashcard : aggregates >
ScrollItem "1" *-- "0..*" QuizQuestion : aggregates >

StudySessionState o-- "1" StreakState : coordinates
StudySessionState o-- "1" TowerProgression : coordinates
StudySessionState o-- "0..*" TodoItem : synchronizes
StudySessionState o-- "0..*" Flashcard : loads
StudySessionState o-- "0..*" QuizQuestion : loads
StudySessionState o-- "0..*" GachaItem : inventory

ScrollItem ..> DifficultyLevel : classified by
GachaItem ..> Rarity : typed by

@enduml
`;

async function renderAndSave(pumlCode, baseFileName) {
    console.log(`Generating ${baseFileName} (PNG & SVG with white background)...`);

    const pngRes = await fetch('https://kroki.io/plantuml/png', {
        method: 'POST',
        headers: { 'Content-Type': 'text/plain' },
        body: pumlCode
    });
    if (!pngRes.ok) throw new Error(`PNG fetch error: ${pngRes.status} ${pngRes.statusText}`);
    const pngBuf = Buffer.from(await pngRes.arrayBuffer());

    const svgRes = await fetch('https://kroki.io/plantuml/svg', {
        method: 'POST',
        headers: { 'Content-Type': 'text/plain' },
        body: pumlCode
    });
    if (!svgRes.ok) throw new Error(`SVG fetch error: ${svgRes.status} ${svgRes.statusText}`);
    const svgText = await svgRes.text();

    const artifactDir = 'C:\\Users\\Erick\\.gemini\\antigravity-ide\\brain\\f9cc9bb8-f693-40bd-9a83-14d136c558de';
    const workspaceDir = path.join(__dirname, '..');

    const pngTargets = [
        path.join(workspaceDir, `${baseFileName}.png`),
        path.join(artifactDir, `${baseFileName}.png`)
    ];

    for (const target of pngTargets) {
        fs.mkdirSync(path.dirname(target), { recursive: true });
        fs.writeFileSync(target, pngBuf);
        console.log(`Saved PNG (${pngBuf.length} bytes): ${target}`);
    }

    const svgTargets = [
        path.join(workspaceDir, `${baseFileName}.svg`),
        path.join(artifactDir, `${baseFileName}.svg`)
    ];

    for (const target of svgTargets) {
        fs.mkdirSync(path.dirname(target), { recursive: true });
        fs.writeFileSync(target, svgText, 'utf8');
        console.log(`Saved SVG: ${target}`);
    }
}

async function main() {
    await renderAndSave(umlClassDiagramCode, 'Babylon_UML_Class_Diagram');
    console.log("UML Class Diagram generated successfully!");
}

main().catch(err => {
    console.error("Export error:", err);
    process.exit(1);
});
