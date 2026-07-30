# 🐉 Little Dragon

A fast-paced arcade-style casual game built with Unity. Catch falling treasures while avoiding obstacles in a thrilling 60-second challenge!

**Play online:** [Little Dragon WebGL Build](Little%20Dragon/index.html)

---

## 🎮 Gameplay Overview

**Objective:** Help the Little Dragon catch as many treasures as possible while avoiding hazards within 60 seconds.

### Core Mechanics

- **🎯 Catch Treasures**: Collect falling gems, keys, money, bottles, shamrocks, and stars (+10 points each)
- **⚠️ Avoid Obstacles**: Dodge bombs, meteors, stones, and viruses (-5 points each)
- **❤️ Lives System**: Start with 3 lives; hit an obstacle to lose one
- **⏱️ Time Pressure**: The clock is ticking—score as much as possible in 60 seconds
- **📈 Difficulty Ramps**: Spawn rate and fall speed increase over time, keeping the challenge intense

### Scoring & Bonuses

| Action | Points |
|--------|--------|
| Catch treasure | +10 |
| Hit obstacle | -5 |
| Consecutive catches (combo) | +100 bonus |

### Controls

| Action | Input |
|--------|-------|
| Move Left | ← Arrow or A |
| Move Right | → Arrow or D |
| (Mobile) | Tap/Swipe left or right |

---

## 📁 Project Structure

```
wolf and eggs/
├── Assets/
│   ├── Scripts/           # Core gameplay logic (12 C# scripts)
│   ├── Scenes/            # MainMenu & GameScene
│   ├── prefabs/           # Collectibles, obstacles, FX (15 prefabs)
│   ├── sprites/           # 2D sprite assets
│   ├── animation/         # Dragon animations
│   ├── Resources/         # Runtime-loaded assets
│   ├── Settings/          # Graphics & shader configuration
│   └── TextMesh Pro/      # UI font assets
├── Little Dragon/         # WebGL build output
├── ProjectSettings/       # Unity project configuration
└── README.md             # This file
```

---

## 🛠️ Technical Details

### Built With

- **Engine**: Unity 6000.5.4f1
- **Language**: C#
- **Platform**: Mobile-optimized (portrait, touch-ready) + WebGL
- **Graphics**: 2D sprites with visual feedback effects

### Key Components

| Component | Purpose |
|-----------|---------|
| **GameManager** | Manages game states (Start, Playing, GameOver) & lives |
| **DragonController** | Handles player movement & animation |
| **ObjectSpawner** | Spawns treasures & obstacles with difficulty scaling |
| **ScoreManager** | Tracks points & combo system |
| **TimerManager** | 60-second countdown & game end logic |
| **UIManager** | Displays score, timer, lives, & game-over screens |
| **FallingObject** | Physics for falling items (movement & destruction) |
| **Collectible** | Treasure pickup detection & score addition |
| **Obstacle** | Hazard collision & life penalty |

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

- Open `Little Dragon/index.html` in a modern web browser
- No installation required—play instantly in your browser

---

## 📊 Game States & Flow

```
MainMenu → GameScene (Countdown) → Playing (60s) → GameOver (Score Screen) → MainMenu
```

1. **MainMenu**: Title screen with start button
2. **GameScene**: Initialization & countdown before gameplay starts
3. **Playing**: Active 60-second challenge with falling objects
4. **GameOver**: Final score display & restart option

---

## 🎨 Art Style

The game features a **casual, cartoonish 2D aesthetic** with:

- Cute Little Dragon protagonist
- Colorful treasures (gems, keys, coins, magical items)
- Environmental & hazard obstacles
- Visual feedback: splash effects on pickups & collisions
- Mobile-friendly UI with large, readable fonts

---

## ✨ Features Highlights

✅ **Progressive Difficulty**: Challenge increases throughout the match  
✅ **Combo System**: Consecutive catches earn bonus points  
✅ **Mobile-Optimized**: Touch controls for phones & tablets  
✅ **Browser Playable**: No download needed—play on web  
✅ **Instant Feedback**: Animations & effects on every action  
✅ **Quick Sessions**: Perfect for casual gaming (1 minute per game)  

---

## 🐛 Known Limitations & Future Improvements

- Currently single-scene gameplay (MainMenu + GameScene)
- Scoring balancing could include difficulty levels (Easy/Hard)
- Leaderboard system not yet implemented
- Sound/music system not yet integrated
- Local high-score persistence could enhance replayability

---

## 👩‍💻 Development Notes

- **Code Style**: Clean separation of concerns with dedicated manager classes
- **Asset Organization**: Prefabs for collectibles & obstacles enable easy tweaking
- **Difficulty Tuning**: All difficulty parameters are exposed in `ObjectSpawner` for easy testing
- **Extensibility**: System is designed for adding new collectible/obstacle types

---

## 📦 Distribution

The game is published as a **WebGL build** for browser play. To rebuild:

1. Go to **File → Build Settings**
2. Set **WebGL** as the target platform
3. Build to the `Little Dragon/` directory
4. Host `index.html` on a web server or open locally

---

## 📝 License

This project is created for personal/educational purposes.

---

## 🤝 Support & Feedback

Found a bug? Have an idea for improvement? Check the scripts in `Assets/Scripts/` to understand the codebase and consider contributing!

**Enjoy playing Little Dragon! 🐉✨**
