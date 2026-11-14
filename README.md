# Sword Duel Game

> **🎮 Playable Windows Build:** [Download & Play (Google Drive)](https://drive.google.com/file/d/1tPINg-hVEMeK8sRvw3Z5kf_jpcvjXhwX/view?usp=sharing)  
> **🎬 Demonstration Video:** [Watch Gameplay Demo (Google Drive)](https://drive.google.com/file/d/1qI8Q3gehVDYwa0m7eb7PhTNkTRArb87B/view?usp=sharing)
>
> *(Download the zip, extract it, and run `Sword Duel.exe` to play without needing Unity!)*

## Game overview

Sword Duel Game is a single-scene Unity prototype. The player's goal is to defeat four skeleton villains while keeping the player's health above zero.

> The mid-term brief describes a playable obstacle course. This prototype currently focuses on sword combat in a Viking village rather than a purpose-built obstacle course. Confirm with the instructor that this project direction is acceptable, or add a clear obstacle route and finish goal.

## What the player needs to do

1. Start the game and move around the level.
2. Approach the skeletons. They stay still until the player is within about 4.5 Unity units, then chase and attack.
3. Attack the skeletons when they are within about 2.7 Unity units. Each player hit deals 25 damage.
4. Defeat all four skeletons to win. Each skeleton has 100 health points.
5. Avoid enemy attacks. Each enemy hit deals 15 damage; the player starts with 200 health points and loses when it reaches zero.
6. After a win or loss, choose **Play Again** to reload the level and start over.

Health bars show the player's and enemies' remaining health. The HUD also shows how many skeletons have been defeated.

## Controls

| Action | Control |
| --- | --- |
| Move | W, A, S, D |
| Look around | Move the mouse |
| Jump | Space |
| Sprint | Left Shift |
| Attack | Left mouse button |

## Game rules

- Defeat all four skeleton villains to win.
- The player starts with 350 HP; each of the four skeletons starts with 100 HP.
- A player hit deals 25 damage. An enemy hit deals 15 damage.
- Skeletons wait until the player comes within about 4.5 Unity units, then approach and attack at close range.
- The player loses when their health reaches zero.
- Press **Play Again** on the victory or defeat screen to reset the level.

## Unity version

Unity 6.3, Editor version **6000.3.8f1**.

## Repository

[GitHub repository](https://github.com/maoposovandeth/sword-duel-game-old)

## Open and play in the Unity Editor

1. Clone or download this repository.
2. Open it with Unity Hub using Unity **6000.3.8f1** and allow Unity to import the assets.
3. Open `Assets/Viking Village/Scenes/The_Viking_Village.unity`.
4. Press **Play** in the Unity Editor.
5. Use the controls above to fight all four skeletons. Test both a victory and a defeat, then test **Play Again**.

## Windows standalone build

The project is configured with `Assets/Viking Village/Scenes/The_Viking_Village.unity` in the build scene list. In Unity 6, open **File > Build Profiles**, select **Windows**, switch to that platform if needed, and build the game.

Test the generated Windows build without the Unity Editor. Include the complete build output folder (not only the `.exe`) in a ZIP archive. Upload the ZIP to Google Drive or GitHub Releases and add its accessible, non-expiring link below.

**Windows build ZIP:** [Download Playable Windows Build (Google Drive)](https://drive.google.com/file/d/1tPINg-hVEMeK8sRvw3Z5kf_jpcvjXhwX/view?usp=sharing)

## Gameplay recording

Record an MP4 with all four members presenting their parts. Show:

1. The game objective and controls: WASD, mouse look, Space to jump, Left Shift to sprint, and left-click to attack.
2. The player using the sword and shield, and the boundary walls stopping the player from leaving the village.
3. All four skeletons waiting, approaching, and attacking; show the player and enemy health bars changing.
4. A win by defeating all four skeletons, then use **Play Again**.
5. A loss when the player's health reaches zero, then use **Play Again** again.
6. The team members' contributions and the game's result.

The brief also asks for purposeful obstacles and physics interactions. The current level is combat-focused; enemy damage is distance-based. Confirm that the instructor accepts the current game, or add an obstacle route and a clear collision/trigger interaction before claiming those requirements are met. Upload the recording to YouTube or Google Drive and make sure the link is accessible to the lecturer.

**Gameplay video:** [Watch Demonstration Video (Google Drive)](https://drive.google.com/file/d/1qI8Q3gehVDYwa0m7eb7PhTNkTRArb87B/view?usp=sharing)

## Team members and contributions

| Team member | Contribution |
| --- | --- |
| Mao Posovandeth | Set up the Viking Village scene and player; added walking, running, jumping, mouse-controlled camera movement, sword-swing animation, and player/enemy health bars. |
| Chan Siekfong | Added invisible boundary walls to keep the player in the village, and equipped the player with a sword and shield. |
| Nuy Bunhou | Added Villain 1 and Villain 2 and their close-range behavior: they approach and attack when the player gets near. The game now includes four skeleton enemies. |
| Kim Seyha | Added the RPG Animations Pack and player animation work; added the on-screen objective, victory/defeat messages, and the Play Again button. |

## Asset and tutorial references

List assets used in the project with their Asset Store source and publisher. Check each listing's license terms before redistribution.

- [RPG Animations Pack FREE](https://assetstore.unity.com/packages/3d/animations/rpg-animations-pack-free-288783) - DoubleL. Standard Unity Asset Store EULA.
- [Sword and Shield](https://assetstore.unity.com/packages/3d/props/weapons/sword-and-shield-358880) - Danvil. Standard Unity Asset Store EULA.
- [Viking Village | 3D Sample Project](https://assetstore.unity.com/packages/3d/environments/viking-village-3d-sample-project-29140) - Unity Technologies. Standard Unity Asset Store EULA.
- [First Person + Third Person | Character Controllers](https://assetstore.unity.com/packages/3d/characters/first-person-third-person-character-controllers-196526) - Unity Technologies. Non-standard EULA; review the listing's terms.
- [Dungeon Skeletons Demo](https://assetstore.unity.com/packages/3d/characters/creatures/dungeon-skeletons-demo-71087) - Polygon Blacksmith. Standard Unity Asset Store EULA.

- Tutorials used: [Add each tutorial's title, creator, and link, or state that no external tutorials were used]

## Current project notes

- The player uses Unity's `CharacterController` for movement. Combat hits are currently determined by attack distance rather than collision or trigger events. Review this against the course physics requirement and add or demonstrate an appropriate collision/trigger interaction if needed.
- The brief is worth 15 percent of the course mark. Its evaluation is 20 percent game design and level layout, 20 percent player controls, 20 percent physics and interactions, 20 percent game logic and playability, 10 percent repository and documentation, and 10 percent teamwork and contribution.
- Confirm that the sword-duel project direction is acceptable for the obstacle-course brief before final submission.
