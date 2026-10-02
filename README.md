# Dash Arena

A top-down 3D arena survival game made in Unity, where **dashing through enemies is your only attack**.
Survive escalating waves for as long as you can.

![Gameplay](docs/gameplay.gif)
<!-- Add a 20–30 s GIF here: a dash kill, a heart pickup, the wave counter going up -->

## How to play
| Input | Action |
|---|---|
| **WASD** / Arrow keys | Move |
| **Left click** | Dash toward the mouse (2 s cooldown). Dashing damages enemies and makes you briefly invulnerable |

- Your score is how long you survive. Your best time is shown top-right.
- Grab the **heart pickup** to restore health. It disappears after 12 s.
- At 0 HP the run restarts instantly.

## Enemies
| Enemy | Behaviour |
|---|---|
| **Chaser** (grey) | Runs straight at you. 1 hit |
| **Heavy** (purple) | Slow and big. Takes 3 dash hits |
| **Ranged** (orange) | Keeps its distance and shoots. Appears from wave 2 |

Waves grow in size, mix in tougher enemies over time, and are capped at 25 enemies alive.

## How it's built
- **Unity 6 (6000.3.23f1), URP, C#**
- **`IDamageable` interface.** Enemies and the player all take damage through one interface. The dash, bullets and enemy contact don't need to know what they hit.
- **Dash hit detection.** `Physics.OverlapCapsule` sweeps the whole path each frame, so fast dashes can't skip past enemies. A `HashSet` makes sure each enemy is hit once per dash.
- **Wave spawner.** A coroutine spawns waves at the arena edges, a safe distance from the player.
- **Separate invulnerability timers** for the dash and for after taking a hit.
- **Self-wiring components.** The HUD and pickups build themselves in code (`RequireComponent` + `Awake`), which removed a whole class of "forgot to set it in the Inspector" bugs.
- **Input fix.** Holding opposite keys (A+D) used to cancel movement to zero for a moment. I found it by logging input every frame with a small probe script; now the latest key pressed wins.
- **Blender → Unity asset pipeline.** The heart pickup was modelled in Blender and exported as FBX. In game, its ring spins, the heart beats, and its materials glow under bloom.

## Run it
1. Clone the repo and open the folder in **Unity Hub** with Unity 6000.3.23f1.
2. Open `Assets/Scenes/SampleScene` and press **Play**.

## Credits
Design, building and testing by **Muammar Mayaz**.
Built with **Claude (Anthropic)** as tutor and pair programmer, including help with code, debugging and the Blender heart model.

## Part of
A 14-week Unity roadmap: **Prototype 01** (top-down arena) → **Prototype 02** (2D platformer) → **Dash Arena** → AR (coming next).
