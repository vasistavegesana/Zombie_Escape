# Zombie Escape Prototype

This Unity project implements Deliverable 2 from the Zombie Escape game proposal. It is a playable top-down survival demo built with original, code-generated visuals so every required mechanic can be reproduced without third-party assets.

## Prototype loop

1. Explore the abandoned building with WASD or the arrow keys.
2. Collect the red security key on the west side of the building.
3. Fight or avoid zombies with the Space key.
4. Pick up health packs and supplies.
5. Use the security key to open the red center door.
6. Reach the green exit zone on the east side to win.

If the player's health reaches zero, the run is lost. Press R after a win or loss to restart.
Press P at any time to save a clean prototype screenshot for the presentation.

## Rubric coverage

- Game environment: two connected rooms, walls, hallways, furniture obstacles, signs, a locked security passage, and an exit.
- Core mechanics: four-direction movement, pickups, inventory feedback, health, combat, locked areas, win condition, and lose condition.
- Limited gameplay: a complete start-to-escape loop with three enemies and optional supply pickups.
- AI explanation: the current zombies use a deliberately simple direct-chase placeholder. The final design adds A-star pathfinding for obstacle-aware routes and a six-state behavior machine: Idle, Wander, Detect, Chase, Attack, and Search.

## Open and run

1. Open this folder in Unity 6000.5.8f1.
2. Open `Assets/Scenes/ZombieEscapePrototype.unity`.
3. Press Play.

The project also includes `Builds/Mac/Zombie Escape.app` when the included build step succeeds.

## Main source files

- `PrototypeBootstrap.cs`: generates the playable environment and game objects.
- `PlayerController2D.cs`: movement, facing, and melee combat.
- `ZombieController2D.cs`: temporary chase and attack behavior.
- `GameManager.cs`: health, inventory, HUD, feedback, and win or lose state.
- `Pickup2D.cs`: keys, health packs, and supplies.
- `LockedDoor2D.cs`: key-gated security door.
- `ExitZone2D.cs`: validates the key and ends the run.

## Final AI architecture

The final version separates decision-making from navigation. A state machine decides whether a zombie should idle, wander, detect, chase, attack, or search. A-star pathfinding then calculates how the zombie reaches its current target around walls and furniture. This separation keeps the behavior understandable, testable, and fair to the player.
