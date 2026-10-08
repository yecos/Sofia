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

## Layered cutout source v01

A first independently editable cutout kit now lives in `Assets/Sofia/VS01/Art/Layered/Father/`:

- `Father_RigParts_Layers.ora`: editable OpenRaster master with one isolated layer per component, preserving the contact-sheet layout.
- `Parts/SPR_Father_*.png`: 20 transparent cutouts for Unity, split into head/hair, collar, torso, pelvis, near/far arm segments, near/far trouser and boot pieces, and six cape panels.
- `Father_RigParts_ContactSheet.png`: transparent checkerboard review sheet.
- `Father_RigParts_Manifest.json`: source rectangles, provisional pivots, PPU and notes.
- `tools/Art/build_father_layered_assets.py`: reproducible extraction, fringe cleanup, OpenRaster packing and contact-sheet generation.
- `Assets/Sofia/VS01/Editor/FatherLayeredArtImporter.cs`: imports the cutouts as single-sprite assets at 520 PPU with per-piece custom pivots, alpha transparency and uncompressed texture data.

The source atlas `Father_RigParts_Atlas_v01.png` is preserved unchanged. The clean atlas and per-part PNGs are derived copies. The file `Father_RigParts_Layers.ora` opens as layers in OpenRaster-compatible paint software; it is not a PSD/PSB.

This is a cutout source kit, not yet a finished assembled puppet. Several generated pieces overlap by design (for example, hair over the head, collar over torso, and cape panels behind/in front of the body), and the pivots still need tuning against a neutral assembled pose. I have kept the tested `PF_Father` prefab and its animation state machine intact rather than replacing it with an unreviewed assembly. The next rig pass should assemble a neutral side view in a separate prefab, tune overlaps and pivots in Unity, then transfer the existing locomotion, reach IK and cape timing after visual review.

## Rebuild and verification

Run `Sofia.VS01.Editor.FatherRigAssetBuilder.RebuildCharacterPrefab()` to refresh the generated pose meshes, clips, controller, and father prefab without rebuilding the environment scene. Run `Sofia.VS01.Editor.FatherAnimationLabBuilder.Build()` to create or refresh the animation review scene. Use `Sofia.VS01.Editor.VS01Builder.Build()` only when the VS01 environment itself needs regeneration.

Unity imported the updated rig without compiler errors. The targeted PlayMode test `FatherVisualUsesSpriteSkinAnimatorAndReachIk` passed (1/1, 3.84 seconds), including the 22-bone SpriteSkin readiness check and reach-chain validation. The editor-rendered character was visually checked in its full-body idle, walk, and run poses. A separate checkpoint capture test did not return from the Unity test executor within two minutes, so that run is not counted as a pass; the existing first-block evidence remains under `docs/evidence/vs01/`.
