# Babylon Knowledge Incremental

Babylon Knowledge Incremental is a gamified study web application built with **Blazor WebAssembly (.NET 10)** and styled using **Tailwind CSS**. Its academy-inspired workspace combines active recall flashcards, multiple-choice quizzes, daily streak progression with an animated Sisyphus character, an ancient hourglass focus timer, and an interactive scroll library catalog into a unified Hellenistic-Babylonian learning realm.

---

## Key Features

- **🏛️ The Great Scroll Library (`/library`):**
  - Search and filter curated codices across Computer Science, Ancient Mesopotamia, Algorithms & Logic, and Philosophy.
  - Interactive preview modal with sample questions.
  - One-click study loading into your active workspace.

- **⛰️ Sisyphus Daily Ascent:**
  - Dynamic vector character sprite of Sisyphus pushing the great stone boulder along the mountain incline.
  - Smooth tracking across 7 milestone notches (from *Foot of the Mountain* to the *Altar of Mastery*).
  - Milestone particle effects and 7-day streak cycle capsule bonuses.

- **⏳ Ancient Hourglass Focus Timer:**
  - Built-in Pomodoro focus timer with presets for 25-minute deep study, 15-minute sprint, and 5-minute review.
  - Live sand-ticking animation and bonus capsule bounty rewards upon completing focus intervals.

- **🗃️ Active Study Deck & Quiz Evaluation:**
  - 3D interactive flip cards with keyboard navigation (`Space` / `Enter` to reveal, `Arrow Right` for next).
  - Multiple-choice quiz trial with instant answer validation, comprehensive score evaluations, accuracy calculation, and claimable capsule bounties.

- **📄 Core PDF Study-Set Utility:**
  - File picker accepting `.pdf` documents from your local file system.
  - One-click preset scrolls (*C# Basic Syntax*, *Babylonian History & Architecture*, *Algorithms & Complexity*) with dynamic study-set generation.
  - Quick-switch and clear file actions.

- **💎 The Oracle's Treasury (`/treasury`):**
  - Gacha pull banners across Aesthetics, Study Tools, and Audio Packs.
  - Multi-tier rarity system (Common, Rare, Epic) with ceremonial pull animations.

- **🌓 Hellenistic-Babylonian Design System:**
  - Custom color palette: Lapis Lazuli (`#1B4965`), Temple Gold (`#D4AF37`), Sandstone (`#F4EEDD`), and Marble White (`#F9F8F6`).
  - Typography: Classical `Cinzel` serif headings with modern `Inter` body text.
  - Seamless zero-flash light and dark theme toggling.

---

## Application Routes

- `/` or `/workspace` — Primary study workspace (Sisyphus Ascent, Todo List, PDF Dropzone, Study Deck, and Tower Progression)
- `/library` — The Great Scroll Library with real-time search, category filters, and one-click study loading
- `/treasury` — Oracle's Treasury for gacha capsule pulls and collection inventory
- `/login` — Classical academy scholar login and registration interface

---

## Technology Stack

- **Framework:** Blazor WebAssembly (.NET 10)
- **Language:** C# 13 (nullable reference types, scoped dependency injection)
- **Styling:** Tailwind CSS v3 with custom Hellenistic-Babylonian theme extensions
- **Assets:** Custom SVG vector character sprites, 3D CSS transforms, and CSS grid layouts

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js](https://nodejs.org/) (v18+ for compiling Tailwind CSS)

### Running Locally

1. **Restore and run the Blazor WebAssembly app:**
   ```powershell
   dotnet restore
   dotnet run --project .\BabylonKnowledgeIncremental.csproj
   ```

2. Open the URL printed in the terminal (typically `http://localhost:5000`) in your browser.

### Rebuilding Tailwind CSS (Optional)

```powershell
npm install
npm run build:css
```

To watch for changes during development:
```powershell
npm run watch:css
```

---

## License & Academic Notice

Developed as an educational prototype exploring gamified cognitive learning environments and incremental progression systems.
