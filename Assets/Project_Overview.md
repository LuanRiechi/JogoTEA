# Technical Documentation: JogoTEA

## 1. Project Description
**JogoTEA** is an educational Unity project designed to support children with Autism Spectrum Disorder (ASD/TEA). The project features mini-games focused on cognitive development, including a Memory Game and a Maze Game. A central pillar of the experience is the Student Management system, which allows teachers or guardians to track individual progress, best times, and completed levels for multiple students.

## 2. Gameplay Flow / User Loop
1.  **Boot & Authentication**: The game starts at `MenuInicial`. Users must either select an existing student profile or create a new one to unlock access to the mini-games.
2.  **Mini-Game Selection**: Once a student is active, the user can choose between the Memory Game (`JogoMemoria`) or the Maze Game (`JogoLabirinto`).
3.  **Core Loop (Mini-Games)**:
    *   **Memory Game**: Select difficulty (3 levels), flip cards to find pairs, and complete the set.
    *   **Maze Game**: Navigate a character (Dinosaur) through a 2D maze to reach the finish line.
4.  **Completion & Feedback**: Upon finishing a game, a "Success" panel displays the completion time and updates the student's persistent data if a new record is achieved.
5.  **Return/Exit**: The user can return to the main menu to switch students, view progress reports, or exit the application.

## 3. Architecture
The project follows a **Manager-centric** architecture with a clear separation between data persistence and game logic.
*   **Data Persistence**: Handled by the static `DataManager`, which uses JSON serialization to save data to the disk.
*   **Scene Management**: `MenuController` acts as the primary orchestrator for UI transitions and scene loading.
*   **Mini-Game Isolation**: Each mini-game operates within its own scene with a dedicated Manager (`scriptJogoMemoria`, `LabirintoGameManager`) that communicates back to the `DataManager` only upon game completion.
*   **Data Flow**: `UI -> MenuController -> DataManager (Static) <- Mini-Game Managers`.

`Location: Assets/Scripts`

## 4. Game Systems & Domain Concepts

### Student Management System
*   `DataManager`: Static class managing the `GameData` container, handling JSON Save/Load logic.
*   `Aluno`: Data class representing a student, containing a list of `FaseProgresso`.
*   `FaseProgresso`: Stores specific stats (game name, level index, completion status, best time).
*   **Extension**: Add new fields to the `Aluno` class (e.g., age, specific needs) and update `DataManager.Salvar()` to include them in the JSON.
`Location: Assets/Scripts/DataManager.cs`

### Memory Game System
*   `scriptJogoMemoria`: Main logic for card shuffling, pair matching, and difficulty scaling (Grid size 2x2 to 4x4).
*   `scripCarta`: Individual card behavior, handling click events, animations, and texture swapping.
*   **Extension**: New difficulties can be added by extending the `switch` statement in `ConfigurarFase()` and providing more card sprites.
`Location: Assets/` (Main folder)

### Maze Navigation System
*   `LabirintoPlayerController`: Handles 2D movement using `Rigidbody2D.linearVelocity` (Unity 6) and input detection.
*   `LabirintoGameManager`: Tracks time and detects the "Finish" trigger to conclude the level.
*   **Extension**: Add obstacles or "pickups" by creating new classes that interact with the `LabirintoPlayerController` or modify speed variables.
`Location: Assets/MiniGames/Maze/Scripts/`

## 5. Scene Overview
*   `MenuInicial`: The entry point. Contains the student registry, profile selection, and the primary navigation hub.
*   `JogoMemoria`: A dynamic scene that adjusts its grid and card count based on the `FaseSelecionada` static variable set by the menu.
*   `JogoLabirinto`: A 2D top-down navigation level. Current implementation suggests a single-level structure or prefab-based layout.

## 6. UI System
The project uses **Unity UI (uGUI)** combined with **TextMesh Pro** for high-quality text rendering.
*   **Menu Panels**: Managed via `GameObject` toggling in `MenuController` (`painelPrincipal`, `painelSelecao`, etc.).
*   **Dynamic Lists**: The `PopularListaAlunos` method instantiates prefab buttons into a vertical/grid layout to display the student database.
*   **Feedback UI**: Mini-games use overlay Canvases for "Success" messages and real-time timers.
*   **UI Binding**: Standard Unity Events (OnClick) are used to link buttons to `MenuController` methods.

## 7. Asset & Data Model
*   **Persistence**: Data is stored in `game_data.json` located in `Application.persistentDataPath`.
*   **Visual Assets**: Uses 2D Sprites for cards and dinosaurs. Animations for card flipping are handled via the `Animator` component with `AnimacaoAbre` and `AnimacaoFecha`.
*   **Naming Convention**: A mix of English and Portuguese (e.g., `DataManager.cs` vs `scripCarta.cs`).
*   **Prefabs**: `Card.prefab` is the central unit for the Memory game, containing the sprite renderer and animator.

## 8. Notes, Caveats & Gotchas
*   **Static Variable Dependency**: The Memory Game relies on `scriptJogoMemoria.FaseSelecionada`. Ensure this is set before loading the scene, or it defaults to Fase 1.
*   **Unity 6 Physics**: `LabirintoPlayerController` uses `rb.linearVelocity`. If downgrading the Unity version, this must be changed to `rb.velocity`.
*   **Data Integrity**: `DataManager` is a static class. It does not exist in the hierarchy, but `DataManager.Carregar()` must be called (currently handled in `MenuController.Start()`).
*   **Audio**: The "Mudo" (Mute) setting is saved in `PlayerPrefs`, independent of the student JSON data.