# Zombie Escape Prototype Presentation

Target length: 12 to 13 minutes. Assign one speaker to every section before presenting.

## 1 Concept recap 1 minute

Zombie Escape is a top-down survival game set in an abandoned building during a zombie outbreak. The player's objective is to explore, collect the security key and supplies, survive the zombies, unlock the security passage, and escape.

## 2 Environment walkthrough 1.5 minutes

Show the two-room building, center security door, furniture obstacles, pickup locations, and the exit. Explain that the visuals are code-generated original content for a reproducible prototype.

## 3 Core mechanics demonstration 5 minutes

- Move with WASD or the arrow keys.
- Collect the red key and show the HUD update.
- Collect supplies and a health pack.
- Attack a zombie with Space and explain the short melee range.
- Touch the locked door before and after collecting the key.
- Point out the health loss when a zombie reaches the player.

## 4 Complete gameplay loop 1.5 minutes

Demonstrate a loss by allowing health to reach zero, restart with R, then complete a winning run by collecting the key and reaching the green exit.

## 5 Planned AI 3 minutes

The prototype uses direct chase behavior so the team could prioritize the six core systems. The final build will combine two layers:

- A state machine chooses Idle, Wander, Detect Player, Chase, Attack, or Search.
- A-star pathfinding finds a valid route around walls and furniture to the current target.

The state machine answers what the zombie should do; A-star answers how it should get there. Normal, fast, and smart zombie types can share the architecture while using different movement speeds, detection ranges, and route choices. Nearby zombies can later receive an alert when one zombie detects the player.

## 6 Wrap-up 1 minute

Summarize that the prototype demonstrates the environment, all proposed core mechanics, and a complete playable loop. Remaining final-build work includes A-star navigation, the full state machine, animations, sound, improved art, and balancing.
