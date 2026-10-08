# VS01 character animation

## What is built

The first 90-second block now uses the supplied painted character art instead of a geometric placeholder. The editor builder extracts 113 intact poses from the four transparent sheets, removes detached fragments, and preserves antialiased edges. The source sheets remain unchanged.

The father prefab uses Unity 2D Animation with a 22-bone skeleton, a weighted SpriteSkin mesh on each extracted pose, and three cape chains. Sprite frames carry the main silhouette and anatomy; bone keys add restrained torso, head, limb, and cape movement. The controller covers Awakening, Idle, Walk, RunStart, Run, RunStop, Jump, Fall, Land, LookHele, and Turn. Grounded speed thresholds trigger the one-shot run transitions. The gameplay motor still owns movement, collision, coyote time, buffered jumps, and world position; Animator root motion is off.

The installed 2D IK package drives the father's reach toward Hele. The cape secondary-motion component adds a spring-like response. `SCN_AnimationLab` is a separate review room with buttons and number-key shortcuts for all eleven states, playback speed and IK-weight sliders, and F12 screenshot capture.

## Review captures

The lab captures are saved under `docs/evidence/vs01/animationlab/`:

| Action | Capture |
| --- | --- |
| Awakening | `father_awakening.png` |
| Idle | `father_idle.png` |
| Walk | `father_walk.png` |
| Run | `father_run.png` |
| Run start | `father_run_start.png` |
| Run stop | `father_run_stop.png` |
| Jump | `father_jump.png` |
| Fall | `father_fall.png` |
| Land | `father_land.png` |
| Look at Hele | `father_look_hele.png` |
| Turn | `father_turn.png` |

## Source-art boundary

The repository has no layered `Father_MASTER.psd` or `.psb`. PSD Importer is installed, but a flattened full-body painting does not contain independent arm, leg, torso, hair, or cape layers for the importer to recover. This rig therefore combines the painted key poses with controlled mesh deformation; it is a useful first pass, not a final separated-layer rig. When the layered master is ready, import it with PSD Importer and transfer the tested state machine, IK setup, timing, and cape curves onto those layers.

## Rebuild and verification

Run `Sofia.VS01.Editor.FatherRigAssetBuilder.RebuildCharacterPrefab()` to refresh the generated pose meshes, clips, controller, and father prefab without rebuilding the environment scene. Run `Sofia.VS01.Editor.FatherAnimationLabBuilder.Build()` to create or refresh the animation review scene. Use `Sofia.VS01.Editor.VS01Builder.Build()` only when the VS01 environment itself needs regeneration.

Unity imported the updated rig without compiler errors. The targeted PlayMode test `FatherVisualUsesSpriteSkinAnimatorAndReachIk` passed (1/1, 3.84 seconds), including the 22-bone SpriteSkin readiness check and reach-chain validation. The editor-rendered character was visually checked in its full-body idle, walk, and run poses. A separate checkpoint capture test did not return from the Unity test executor within two minutes, so that run is not counted as a pass; the existing first-block evidence remains under `docs/evidence/vs01/`.
