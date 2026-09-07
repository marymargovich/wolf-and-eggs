# 🐉 Little Dragon Treasure Hunt

A fast-paced arcade-style casual game built with Unity. Catch falling treasures while avoiding obstacles with your 5 hearts in a thrilling 60-second challenge!

**Status**: ✅ **v1.0 STABLE** - Fully tested & production-ready  
**Play online:** [Little Dragon WebGL Build](https://marymargovich.github.io/wolf-and-eggs/)

---

## 🎮 Gameplay Overview

**Objective:** Help the Little Dragon catch as many treasures as possible while avoiding hazards within 60 seconds. Survive with at least 1 heart!

### Core Mechanics

- **🎯 Catch Treasures**: Collect falling gems, keys, money, bottles, shamrocks, and stars (+10 points each)
- **⚠️ Avoid Obstacles**: Dodge bombs, meteors, stones, and viruses (lose 1 heart, no point penalty)
- **❤️ Hearts System**: Start with **5 hearts** per game; hit an obstacle to lose one
- **⏱️ Time Pressure**: The clock is ticking—score as much as possible in 60 seconds
- **📈 Difficulty Ramps**: Spawn rate and fall speed increase over time, keeping the challenge intense
- **🎵 Dynamic Audio**: Background music during gameplay, victory music on game end

### Game End Conditions

| Condition | Result |
|-----------|--------|
| **Timer expires (00:00)** with lives remaining | ✅ VICTORY - Display final score |
| **All 5 hearts lost (0 lives)** | ❌ GAME OVER - Display final score |

### Scoring & Bonuses

| Action | Points |
|--------|--------|
| Catch treasure | +10 |
| Hit obstacle | 0 penalty (lose 1 heart instead) |
| Consecutive catches (5 in a row, no hits) | +100 bonus |
| Combo reset | On obstacle collision |

### Controls

| Action | Input |
|--------|-------|
| Move Left | ← Arrow or A |
| Move Right | → Arrow or D |
| (Mobile) | Tap/Swipe left or right |
| Pause Info | Info button (during gameplay) |
| Exit to Menu | Exit button (during gameplay) |

---

## 🎵 Audio System

- **Background Music (BGM)**: Plays during active gameplay
- **Victory Music**: Plays when game ends (both victory by timer & game over by 0 hearts)
- **Sound Effects**: Good item pickup SFX, bad item collision SFX
- **Audio Mute Controls**: Toggles for Music and SFX in settings

---

## 📁 Project Structure

```
wolf and eggs/
├── Assets/
│   ├── Scripts/
│   │   ├── GameManager.cs       # Game state & 5-lives management
│   │   ├── UIManager.cs         # UI display & state transitions
│   │   ├── TimerManager.cs      # 60-second countdown
│   │   ├── ScoreManager.cs      # Points & combo tracking
│   │   ├── DragonController.cs  # Player movement & animation
│   │   ├── ObjectSpawner.cs     # Treasure & obstacle spawning
│   │   ├── FallingObject.cs     # Physics for falling items
│   │   ├── Collectible.cs       # Treasure pickup logic
│   │   ├── Obstacle.cs          # Hazard collision & damage
│   │   ├── AudioManager.cs      # Audio playback control
│   │   ├── ClickToChangeScene.cs # Scene navigation
│   │   └── QuitGameController.cs # App quit handling
│   ├── Scenes/
│   │   ├── MainMenu.unity       # Start screen
│   │   └── GameScene.unity      # Main gameplay scene
│   ├── prefabs/                 # 15+ collectible & obstacle prefabs
│   ├── sprites/                 # 2D sprite assets
│   ├── animation/               # Dragon animation clips
│   ├── Audio/                   # BGM & SFX clips
│   ├── Resources/               # Runtime-loaded assets
│   └── TextMesh Pro/            # UI font assets
├── ProjectSettings/             # Unity project configuration
├── docs/                        # Published WebGL build
└── README.md                    # This file
```

---

## 🛠️ Technical Details

### Built With

- **Engine**: Unity 6000.5.4f1 LTS
- **Language**: C# (.NET)
- **Platform**: Windows, macOS, Linux, WebGL
- **Graphics**: 2D sprites with real-time visual feedback

### Key Components

| Component | Responsibility |
|-----------|-----------------|
| **GameManager** | Game state machine (Start→Playing→GameOver), 5-lives tracking, damage system |
| **UIManager** | HUD display, hearts UI, win/game-over screens, audio transitions, restart logic |
| **TimerManager** | 60-second countdown, end-game trigger on 00:00 |
| **ScoreManager** | Score tracking, combo system (5 consecutive catches = +100 bonus) |
| **DragonController** | Player movement (keyboard/touch), animation, clamping to screen |
| **ObjectSpawner** | Weighted random spawning, difficulty ramping, fall speed scaling |
| **FallingObject** | Gravity simulation, cleanup when off-screen |
| **Collectible** | Treasure detection & score addition, audio feedback |
| **Obstacle** | Collision detection, TakeDamage() call, combo reset, SFX |
| **AudioManager** | Centralized audio control, BGM/SFX/Win Music playback, mute state |

### Architecture

- **Event-Driven**: OnLivesChanged, OnGameStateChanged, OnScoreChanged events
- **Manager Pattern**: Singleton managers for audio, score, timer, game state
- **Auto-Find References**: Components auto-locate each other via FindAnyObjectByType
- **State Management**: Active UI state management via UpdateGameUI() in Update() loop

### Difficulty Progression

- **Spawn Interval**: 1.5s → 0.6s (faster objects appear)
- **Fall Speed**: 2 → 6 units/sec (objects fall faster)
- **Progression**: Dynamic over 60 seconds of gameplay

---

## 🚀 Getting Started

### Requirements

- **Unity**: 6000.5.4f1 or compatible version
- **Platform**: Windows, macOS, Linux (or Web Browser for WebGL build)

### Open & Play

1. Clone or download this repository
2. Open the project in Unity Hub or directly via Unity Editor
3. Load the **MainMenu** scene from `Assets/Scenes/MainMenu.unity`
4. Press **Play** to test gameplay
5. Navigate to **GameScene** to play the full game

### Running the WebGL Build

The WebGL build is published through GitHub Pages. Open the [online build](https://marymargovich.github.io/wolf-and-eggs/) to play in a modern browser.

To create a new build, select **File → Build Profiles → Web → WebGL** in Unity and build into the `docs/` directory. GitHub Pages serves the contents of `docs/` from the `main` branch.

---

## 📊 Game States & Flow

```
MainMenu 
   ↓ (Click Play)
GameScene (Countdown: "Get Ready!")
   ↓ (Countdown expires)
Playing (60 seconds active)
   ├─ 🎵 BGM plays
   ├─ Objects spawn & fall
   ├─ Dragon moves & catches/hits
   └─ Timer counts down
   ↓ (Timer expires OR lives reach 0)
GameOver
   ├─ 🎵 Victory Music plays
   ├─ Final score displayed
   └─ Buttons: "Play Again" or "Exit to Menu"
   ↓
MainMenu or Playing (restart)
```

**Victory Condition**: Timer reaches 00:00 with ≥1 heart remaining  
**Defeat Condition**: All 5 hearts lost before timer expires

---

## ✨ Features

✅ **5-Hearts System**: Visual heart indicators with SetActive + alpha control  
✅ **Full Audio Integration**: BGM during play, Victory music on end, SFX on actions  
✅ **Progressive Difficulty**: Challenge scales over 60 seconds  
✅ **Combo System**: Bonus points for consecutive catches (5 in a row = +100)  
✅ **Mobile-Optimized**: Touch controls for phones & tablets  
✅ **Browser Playable**: WebGL build is published through GitHub Pages
✅ **Visual Feedback**: Splash effects on pickups & collisions  
✅ **Instant State Transitions**: Win/GameOver screens with proper audio  
✅ **Clean Restart**: "Play Again" button resets all state & music  

---

## 🧪 Testing Checklist (v1.0)

All features verified working:

- ✅ Start with exactly 5 hearts displayed
- ✅ Hearts decrease on obstacle hit (one at a time)
- ✅ Hearts display left→right removal pattern
- ✅ Combo resets on obstacle collision
- ✅ Timer counts down from 60 to 00 correctly
- ✅ Game ends at 0:00 with victory screen (if hearts remaining)
- ✅ Game ends at 0 hearts with game-over screen
- ✅ BGM plays during gameplay
- ✅ Victory music plays on game end
- ✅ Play Again button restarts with fresh BGM
- ✅ Exit to Menu returns to start screen
- ✅ Objects spawn and fall correctly
- ✅ Collision detection accurate
- ✅ Score calculation correct
- ✅ No console errors or warnings
- ✅ WebGL build runs through GitHub Pages

---

## 🐛 Known Issues

**None** — v1.0 stable is fully tested with no known bugs.

### Potential Future Enhancements

- Difficulty levels (Easy/Normal/Hard)
- Leaderboard system with high-score persistence
- Additional collectible types & obstacle variations
- Sound volume slider (currently toggle-based)
- Mobile app versions (iOS/Android)
- Visual themes/cosmetics

---

## 👩‍💻 Development Notes

### Code Quality

- **Clean Architecture**: Separation of concerns with dedicated manager classes
- **Debug Logging**: Extensive logging for audio transitions & state changes
- **Extensible Design**: Easy to add new collectible/obstacle types via prefabs
- **Performance**: Optimized spawning, object pooling on the roadmap

### Building for WebGL

```bash
# In Unity Editor:
File → Build Settings → Switch Platform → WebGL
Build into "Little Dragon/" directory
```

Then deploy `Little Dragon/index.html` to a web server or open locally.

---

## 📈 Version History

| Version | Date | Changes |
|---------|------|---------|
| **v1.0-stable** | 2026-09-07 | ✅ Full 5-hearts system, audio transitions, game-over logic, all features tested |
| v1.2.0-web-music-ready | 2026-08-09 | Audio manager toggles & UI sprites |
| v1.2.0-web-ready | 2026-08-09 | WebGL exit freeze fix |

---

## 📝 License

Created for personal/educational purposes.

---

## 🤝 Support

Issues or suggestions? Review the scripts in `Assets/Scripts/` to understand the implementation. The codebase is well-commented and structured for easy modification!

**Enjoy playing Little Dragon! 🐉✨**

*Built with ❤️ using Unity*
