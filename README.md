# Babylon Knowledge Incremental

Babylon Knowledge Incremental is a Blazor WebAssembly study-app prototype built with C# and .NET 10. Its academy-inspired workspace brings flashcards, quizzes, study tasks, streaks, and a gamified progression system together in one interface. (Prototype)

## Features

- **Study workspace:** Switch between flashcard and multiple-choice quiz modes, reveal answers, submit responses, and navigate study content.
- **PDF study-set interface:** Choose a PDF from the file browser or select one of the built-in sample documents.
- **Study planning:** Track a daily task list and study streak.
- **Progression and rewards:** Offer energy to progress the Tower of Babel and spend earned capsules on collectible items in the Oracle's Treasury.
- **Theme control:** Switch between light and dark themes.
- **Login and registration screens:** Explore the academy-themed account interface.

## Current prototype limitations

This project is a front-end prototype. It does not currently include a server, account authentication, or persistent storage. The PDF interface records the selected file's name and size, then loads built-in sample study content based on the filename; it does not parse PDF contents or export study materials to PDF. The account forms are visual/demo flows and do not create or authenticate accounts.

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

## Run locally

From the repository root, restore dependencies and start the Blazor development server:

```powershell
dotnet restore
dotnet run --project .\BabylonKnowledgeIncremental.csproj
```

Open the local URL printed by the .NET CLI in your browser.

To build without starting the development server:

```powershell
dotnet build .\BabylonKnowledgeIncremental.csproj
```

## Application routes

- `/` or `/workspace` — study workspace
- `/login` — login and registration interface (to be added)
- `/treasury` — reward banners and collection pulls (to be added)

## Technology

- Blazor WebAssembly
- C# with nullable reference types and implicit usings enabled
- .NET 10
