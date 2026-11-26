# ClonePuzzle

**ClonePuzzle** is a grid-based puzzle game developed with Unity. The goal is to reach the finish tile while carefully managing your movements and the clones you leave behind.

## 🎮 Gameplay

The player moves across a grid by swiping in one of four directions. The character slides until it encounters a wall or a clone.

Every time the player moves, a clone is created at their previous position. These clones remain on the grid and act as obstacles, making each move an important part of the puzzle.

Plan your movements carefully, avoid getting trapped, and find your way to the finish tile before running out of moves!

## ✨ Features

- **Grid-Based Movement:** Swipe in four directions to move across the level.
- **Clone Mechanic:** Leave a clone behind every time you move.
- **Obstacle System:** Walls and clones block the player's movement.
- **Move Limit:** Complete each level within the available number of moves.
- **Win Condition:** Reach the designated finish tile to complete the level.

## 🗺️ Level Design

Levels are designed directly in the Unity Editor using Tilemaps.

- **Paint-Based Level Creation:** Design levels by painting tiles directly onto the grid.
- **Flexible Level Design:** Add walls, ground tiles, and finish positions without manually editing code.
- **Easy Iteration:** Create and modify levels quickly using Unity's Tilemap tools.

This approach separates level design from gameplay code, making it easier to create new levels and experiment with different layouts.

## 🕹️ Controls

| Input | Action |
|---|---|
| Swipe Up | Move Up |
| Swipe Down | Move Down |
| Swipe Left | Move Left |
| Swipe Right | Move Right |

## 🛠️ Built With

- Unity
- C#
- Unity Input System
- DOTween
- Tilemap System

## 📱 Platform

- Android (Mobile)

## 🚧 Development Status

ClonePuzzle is currently in development. New levels, mechanics, and improvements may be added over time.
