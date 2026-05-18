<div align="center">

# Brotato2

### Survive the waves. Build your synergy. Break the limits.

A fan-made roguelike arena survival game developed in Unity, inspired by Brotato and Vampire Survivors.

<br>

![Unity](https://img.shields.io/badge/Engine-Unity_2022_LTS-black?logo=unity)
![Language](https://img.shields.io/badge/Language-C%23-blue?logo=csharp)
![Genre](https://img.shields.io/badge/Genre-Roguelike-green)
![Platform](https://img.shields.io/badge/Platform-PC-lightgrey)
![Status](https://img.shields.io/badge/Status-Active_Development-success)

</div>

---

# Trailer / Gameplay Preview

> Add your gameplay GIF here

```md
![Gameplay](images/gameplay.gif)
```

---

# About The Project

Brotato2 is a fast-paced top-down roguelike survival game where players face continuous enemy waves, collect weapons, stack upgrades, and create powerful synergies to survive increasingly difficult battles.

The project focuses on:

- Replayability
- Modular architecture
- Dynamic stat balancing
- Expandable content systems
- Efficient performance under large enemy counts

Built as a personal game development project to practice scalable gameplay system design in Unity.

---

# Key Features

## Combat

- Auto-attack system
- Multi-weapon support
- Critical strike mechanics
- Attack speed scaling
- Damage formula balancing
- Boss battle system

## Progression

- Random upgrade selection
- Shop between waves
- Luck-based rarity generation
- Weapon synergy bonuses
- Passive item combinations
- Character stat growth

## Technical

- Object Pooling
- ScriptableObject-based data
- Manager pattern
- Runtime stat recalculation
- Modular enemy spawning
- Save/load system
- Scalable architecture

---

# Gameplay Loop

```text
Spawn Wave
↓
Fight Enemies
↓
Gain EXP
↓
Level Up
↓
Choose Upgrade
↓
Visit Shop
↓
Buy / Reroll
↓
Boss Wave
↓
Repeat
```

---

# Screenshots

## Combat

```md
![Combat](images/combat.png)
```

## Shop

```md
![Shop](images/shop.png)
```

## Upgrade System

```md
![Upgrade](images/upgrade.png)
```

---

# System Architecture

## Main Modules

| Module | Responsibility |
|---|---|
| GameManager | Core game flow |
| WaveManager | Wave progression |
| EnemySpawner | Spawn control |
| ShopManager | Shop generation |
| PlayerStatsManager | Stat system |
| UpgradeManager | Upgrade selection |
| WeaponSystem | Combat handling |
| UIManager | Interface updates |

---

# Technical Highlight

## Dynamic Stat Calculator

The project uses a layered stat system:

```csharp
FinalStat =
BaseStat
+ UpgradeAddend
+ ItemAddend
+ SynergyAddend;
```

This allows:

- isolated balancing
- flexible future expansion
- easy debugging
- scalable content creation

---

# Performance

### Stress Test Result

| Scenario | Result |
|---|---|
| 500 enemies | Stable |
| 1000 entities | 60 FPS |
| Object pool reuse | Success |
| GC spikes | Minimal |

Optimization techniques:

- object pooling
- event dispatching
- centralized update logic
- runtime caching

---

# Folder Structure

```text
Assets/
├── Scripts/
│   ├── Core
│   ├── Enemy
│   ├── Player
│   ├── Shop
│   ├── Weapons
│   └── UI
│
├── Prefabs
├── ScriptableObjects
├── Animations
├── Sprites
└── Scenes
```

---

# Future Roadmap

## Planned Features

- More playable characters
- More bosses
- New weapon classes
- Item rarity expansion
- Achievement system
- Skill evolution
- Multiplayer prototype
- Voice command ultimate system

---

# Development Goals

This project is used to practice:

## Programming

- Unity architecture
- C# gameplay systems
- optimization
- clean code
- modular design

## Game Design

- balancing
- progression loop
- replayability
- enemy design
- reward systems

---

# Challenges Solved

## Enemy Optimization

Large enemy counts were optimized using object pooling and reduced Update() calls.

## Balancing

Critical chance + attack speed stacking required formula redesign to prevent exponential scaling.

## Content Expansion

All item/weapon data is separated via ScriptableObject for future scalability.

---

# Installation

Clone repository:

```bash
git clone https://github.com/Homge/Brotato2.git
```

Open with:

- Unity 2022 LTS

Run:

- MainScene.unity

---

# Inspiration

Main references:

- Brotato
- Vampire Survivors
- 20 Minutes Till Dawn

---

# Author

## Nguyễn Hoàng Gia Bảo

Computer Science Student  
Unity Developer  
Gameplay Programmer

---

# Repository

:contentReference[oaicite:0]{index=0}

---

# License

This project is for educational and portfolio purposes.

All rights belong to respective original inspirations.
