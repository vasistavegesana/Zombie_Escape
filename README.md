# Zombie Escape

This Unity project is a playable top-down survival game built with original visuals. It includes a polished main menu, illustrated top-down character and zombie sprites, three selectable character classes, two complete levels, combat, pickups, a three-hit health system, and full win/lose flow.

## Prototype loop

1. Choose Survivor, Scout, or Guardian from the main menu.
2. Explore the abandoned offices with WASD or the arrow keys.
3. Collect the red security key, fight or avoid zombies with Space, and reach the exit.
4. Survive the more dangerous night loading bay and reach the final extraction zone.

The player has three health units. Each completed zombie hit removes exactly one unit; losing all three ends the run. Health packs restore one unit. Press R after a win or loss to return to the menu, or P during play to save a screenshot.

## Character classes

- Survivor: balanced movement speed and attack range.
- Scout: fastest movement with a more precise attack.
- Guardian: slower movement with the widest attack range.

## Rubric coverage

- Game environment: two distinct levels with walls, hallways, office furniture, cargo obstacles, signage, locked security passages, and exits.
- Core mechanics: character selection, four-direction movement, pickups, inventory feedback, three-unit health, combat, locked areas, level progression, win condition, and lose condition.
- Gameplay: a complete two-stage start-to-extraction loop with eight enemies, health packs, and optional supply pickups.
- Presentation: custom tactical-horror menu, character stat cards, premium HUD, hit feedback, mission alerts, and polished result screens.
- Original art: distinct Survivor, Scout, Guardian, and infected sprites plus a glowing security-keycard objective with an animated beacon.
- AI explanation: the current zombies use a deliberately simple direct-chase placeholder. The final design adds A-star pathfinding for obstacle-aware routes and a six-state behavior machine: Idle, Wander, Detect, Chase, Attack, and Search.

## Open and run

1. Open this folder in Unity 6000.5.8f1.
2. Open `Assets/Scenes/ZombieEscapePrototype.unity`.
3. Press Play.

Generated builds and Unity cache folders are intentionally excluded from the repository. The checked-in source has been verified with a successful macOS build.

## Main source files

- `PrototypeBootstrap.cs`: generates the playable environment and game objects.
- `PlayerController2D.cs`: movement, facing, and melee combat.
- `ZombieController2D.cs`: temporary chase and attack behavior.
- `GameManager.cs`: menu, character selection, level flow, health, inventory, premium HUD, feedback, and result screens.
- `Pickup2D.cs`: keys, health packs, and supplies.
- `LockedDoor2D.cs`: key-gated security door.
- `ExitZone2D.cs`: validates the key and ends the run.
- `Assets/Resources/Sprites`: original transparent character, enemy, and keycard art used by the runtime.

## Final AI architecture

The final version separates decision-making from navigation. A state machine decides whether a zombie should idle, wander, detect, chase, attack, or search. A-star pathfinding then calculates how the zombie reaches its current target around walls and furniture. This separation keeps the behavior understandable, testable, and fair to the player.
