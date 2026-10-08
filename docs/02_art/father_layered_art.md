# Father layered art kit v01

## Intent

The target for SOFIA is a painted, cinematic side-view character whose silhouette and cloth remain readable while moving. The tutorial reference [Create an Entire 2D Game with AI — Characters, Assets, Animation & Parallax](https://www.youtube.com/watch?v=Gb6zy40WojA) reinforces a locked side profile, consistent character design, transparent cutouts, and layered parallax. Its demonstrated character pipeline uses simpler sprite poses; this kit keeps the consistent side-view guidance while preparing separate parts for a cutout rig.

## Current files

- `game/SofiaUnityProject/Assets/Sofia/VS01/Art/Layered/Father/Father_RigParts_Atlas_v01.png` — generated input atlas; keep as the immutable source for this extraction pass.
- `Father_RigParts_Atlas_Clean.png` — same contact layout with neon edge contamination cleared.
- `Father_RigParts_Layers.ora` — editable OpenRaster master; one named layer per part, still laid out as the contact sheet.
- `Father_RigParts_ContactSheet.png` — review image on a checkerboard.
- `Parts/SPR_Father_*.png` — 20 transparent, cropped Unity sprites.
- `Father_RigParts_Manifest.json` — extracted bounds and provisional bottom-left normalized pivots.
- `game/SofiaUnityProject/Assets/Sofia/VS01/Editor/FatherLayeredArtImporter.cs` — importer settings utility.
- `tools/Art/build_father_layered_assets.py` — deterministic extraction and packaging script.

## Layer inventory

| Rig group | Cutouts | Animation use |
| --- | --- | --- |
| Head and hair | Head_Profile, Hair_Back, Hair_Front | Head tilt, turn silhouette, restrained hair follow-through |
| Upper body | Scarf_Collar, Torso_Robe, Pelvis_Belt_Robe | Breath, hunch/recovery, torso lean and weight shift |
| Arms | Near/Far Upper, Near/Far Forearm+Hand | Opposing arm swing, reach to Hele, planted hand on recovery |
| Legs | Near/Far Pants, Near/Far Boot | Step timing, takeoff, landing and planted foot |
| Cape | Back/Mid/Front × Upper/Lower | Six layered cloth panels with delayed rotation and controlled spring |

The separated atlas contains some overlapping visual roles by design: the head already carries its hair silhouette, the robe torso carries some collar fabric, and the six cape panels are separate folds. The art is therefore not composited into a character automatically. The provisional pivots are starting anchors and must be adjusted in an assembled neutral side pose.

## Import settings

Run `Sofia.VS01.Editor.FatherLayeredArtImporter.ConfigureAll()` from the Unity MCP C# script tool or the Unity menu `SOFIA > VS01 > Animation > Configure layered Father cutouts`. Each part is imported as one Sprite at 520 pixels per unit, with a custom pivot from the manifest, bilinear filtering, clamp wrapping, no mipmaps, alpha transparency, and uncompressed texture data.

The extraction script removes only highly saturated red/yellow fringe near transparency, keeps the generated atlas untouched, extends edge RGB into transparent texels to reduce bilinear fringes, and refuses to export unless all 20 substantial cutouts are present.

## Rigging and animation sequence

1. Assemble one neutral side-view puppet in a new prefab; keep `PF_Father` as the known-good reference.
2. Align the actual overlap at the neck, shoulders, waist, knees, boots, and cape roots; refine every pivot by comparing it with the contact sheet.
3. Add a small number of joint transforms, not one bone per fold: pelvis/root, chest, head, both upper/lower arms, both thighs/feet, and three cape chains.
4. Animate the existing first-block actions—Awakening, Idle, Walk, Run, Jump, Fall, Land, LookHele, and Turn—using pose keys and smooth interpolation.
5. Keep body translation and collision on the gameplay controller. Use hand-reach IK only after the hand and forearm silhouette stays connected through the reach.
6. Drive cape lag with bounded spring response and hand-authored key poses. Do not let independent physics pull the cloth outside its painted panels.
7. Review idle, walk, run, jump, landing, reach, and turn in the animation lab at gameplay camera scale; reject visible seams, floating pieces, foot skating, or flickering edge pixels before replacing the current prefab.

## Neutral cutout prototype (2026-10-07)

The first assembled cutout rig is available separately from the existing production reference:

- Prefab: `game/SofiaUnityProject/Assets/Sofia/VS01/Prefabs/PF_Father_LayeredPrototype.prefab`
- Review scene: `game/SofiaUnityProject/Assets/Sofia/Scenes/Dev/SCN_FatherLayeredReview.unity`
- Rebuild command: Unity menu `SOFIA > VS01 > Animation > Build layered Father prototype`
- Builder: `game/SofiaUnityProject/Assets/Sofia/VS01/Editor/FatherLayeredPuppetBuilder.cs`

The hierarchy exposes `Pelvis`, `Spine`, `Chest`, `Neck/Head`, separate near/far upper arms and forearms, thigh/foot chains, and independent back/mid/front upper/lower cape chains. All 20 cutouts are rendered as painterly sprites; the prototype stays on the normal game layer, while the review camera isolates temporary capture copies.

The Animator controller `AC_Father_LayeredPrototype` has a default `Idle` state and `Walk` and `LookHele` states. Its `Move` float selects walking above 0.1; `Curious` selects the Hele look. The clips are an initial motion probe, with restrained breathing, alternating limb motion and delayed cape movement. Their bindings were checked against the saved rig hierarchy. They are not connected to the gameplay motor or Hele behavior yet.

Review captures are saved under `docs/evidence/animation/`: `father-layered-neutral.png`, `father-layered-idle.png`, `father-layered-walk.png`, and `father-layered-lookhele.png`. Before this prefab replaces `PF_Father`, review the loops in Play Mode at game scale, then refine elbow overlap, planted-foot timing, cape silhouettes and the reach pose.

## Known limits in v01

- This is an editable ORA plus PNG layers; there is no generated PSD/PSB.
- The current PNGs now have an initial transform hierarchy and Animator controller, but do not yet form a production-ready puppet; the provisional joints and overlap extensions still need motion review.
- Forearms and hands are combined in each sprite; a separate hand layer will be needed if the reach/gesture tests show wrist deformation.
- Pants and boots are separate, but thigh and shin are not split into additional pieces yet.
- The existing pose-based 22-bone SpriteSkin animation rig remains the validated in-game implementation until the layered puppet passes Play Mode review and is connected to gameplay.
