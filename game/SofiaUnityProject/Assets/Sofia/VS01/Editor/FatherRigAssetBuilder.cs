using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.U2D.Animation;
using UnityEngine.U2D.IK;

namespace Sofia.VS01.Editor
{
    /// <summary>
    /// Builds clean, full-body pose sprites from the supplied sheets, rigs their
    /// meshes with the installed 2D Animation data providers, and plays keyed
    /// poses through Animator. The painted poses preserve the supplied anatomy;
    /// weighted cape, torso, head, arm and leg zones add controlled deformation.
    /// </summary>
    public static class FatherRigAssetBuilder
    {
        const string SheetRoot = "Assets/Sofia/VS01/Art/SpriteSheets";
        const string FramesRoot = "Assets/Sofia/VS01/Art/GeneratedFrames";
        const string AnimationRoot = "Assets/Sofia/VS01/Animation";
        const string ClipsRoot = AnimationRoot + "/Clips";
        const string ControllerPath = AnimationRoot + "/AC_Father.controller";
        const string MaterialPath = "Assets/Sofia/VS01/Materials/VS01CharacterCutout.mat";
        const int PixelsPerUnit = 110;
        const int AlphaThreshold = 48;
        const int MinimumComponentPixels = 2500;
        const string SkinDataVersion = "SOFIA_SPRITESKIN_V5";

        static readonly string[] Groups = { "Awakening", "Curiosity", "IdleWalkRun", "Jump" };
        static readonly int[] ExpectedCounts = { 26, 26, 34, 27 };
        const string RendererPath = "Facing/CharacterSprites/Father_Sprite";
        const int MeshColumns = 16;
        const int MeshRows = 24;

        sealed class RigBone
        {
            public readonly string name;
            public readonly int parent;
            public readonly Vector3 position;
            public readonly float length;

            public RigBone(string boneName, int parentIndex, Vector3 localPosition, float boneLength)
            {
                name = boneName; parent = parentIndex; position = localPosition; length = boneLength;
            }
        }

        static readonly RigBone[] RigBones =
        {
            new RigBone("SkeletonRoot", -1, Vector3.zero, 0f),
            new RigBone("Hips", 0, new Vector3(0f, .83f, 0f), .32f),
            new RigBone("Spine", 1, new Vector3(0f, .32f, 0f), .28f),
            new RigBone("Chest", 2, new Vector3(.04f, .28f, 0f), .28f),
            new RigBone("Neck", 3, new Vector3(0f, .22f, 0f), .12f),
            new RigBone("Head", 4, new Vector3(.02f, .12f, 0f), .18f),
            new RigBone("ArmFrontUpper", 3, new Vector3(.18f, .02f, 0f), .24f),
            new RigBone("ArmFrontForearm", 6, new Vector3(.22f, -.09f, 0f), .23f),
            new RigBone("ArmFrontHand", 7, new Vector3(.22f, -.05f, 0f), .12f),
            new RigBone("LegFrontUpper", 1, new Vector3(.04f, -.31f, 0f), .38f),
            new RigBone("LegFrontKnee", 9, new Vector3(.08f, -.38f, 0f), .30f),
            new RigBone("LegFrontFoot", 10, new Vector3(.12f, -.30f, 0f), .18f),
            new RigBone("CapeRoot", 3, new Vector3(-.20f, .02f, 0f), .16f),
            new RigBone("Cape_A_01", 12, new Vector3(-.08f, -.10f, 0f), .16f),
            new RigBone("Cape_A_02", 13, new Vector3(-.08f, -.12f, 0f), .16f),
            new RigBone("Cape_A_03", 14, new Vector3(-.08f, -.12f, 0f), .16f),
            new RigBone("Cape_B_01", 12, new Vector3(-.10f, -.16f, 0f), .18f),
            new RigBone("Cape_B_02", 16, new Vector3(-.09f, -.18f, 0f), .18f),
            new RigBone("Cape_B_03", 17, new Vector3(-.08f, -.16f, 0f), .18f),
            new RigBone("Cape_C_01", 12, new Vector3(-.12f, -.22f, 0f), .18f),
            new RigBone("Cape_C_02", 19, new Vector3(-.10f, -.20f, 0f), .18f),
            new RigBone("Cape_C_03", 20, new Vector3(-.08f, -.18f, 0f), .18f)
        };

        sealed class Component
        {
            public int id, area, minX, minY, maxX, maxY;
            public int Top(int height) => height - 1 - maxY;
        }

        public static void BuildAssets()
        {
            Directory.CreateDirectory(Path.Combine(Application.dataPath, "Sofia/VS01/Animation/Clips"));
            Directory.CreateDirectory(Path.Combine(Application.dataPath, "Sofia/VS01/Art/GeneratedFrames"));
            Directory.CreateDirectory(Path.Combine(Application.dataPath, "Sofia/VS01/Materials"));
            ExtractPoseFrames();
            CreateAnimationClips();
            CreateController();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void RebuildCharacterPrefab()
        {
            BuildAssets();
            const string prefabPath = "Assets/Sofia/VS01/Prefabs/PF_Father.prefab";
            GameObject prefab = PrefabUtility.LoadPrefabContents(prefabPath);
            if (!prefab) throw new FileNotFoundException("Father prefab was not found.", prefabPath);
            try
            {
                var motor = prefab.GetComponent<FatherMotor>();
                Transform visual = prefab.transform.Find("Visual");
                if (!motor || !visual)
                    throw new InvalidOperationException("Father prefab must contain its gameplay motor and Visual root.");

                DestroyIfPresent(prefab.GetComponent<FatherAnimationDriver>());
                DestroyIfPresent(prefab.GetComponent<FatherCapeSecondaryMotion>());
                DestroyIfPresent(visual.GetComponent<Animator>());
                DestroyIfPresent(visual.GetComponent<IKManager2D>());
                Attach(visual, motor);
                PrefabUtility.SaveAsPrefabAsset(prefab, prefabPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(prefab);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        static void DestroyIfPresent(UnityEngine.Component component)
        {
            if (component) UnityEngine.Object.DestroyImmediate(component, true);
        }

        static void ExtractPoseFrames()
        {
            bool generated = true;
            for (int g = 0; g < Groups.Length; g++)
                for (int i = 0; i < ExpectedCounts[g]; i++)
                    generated &= File.Exists(AbsoluteFramePath(Groups[g], i));
            if (generated)
            {
                for (int g = 0; g < Groups.Length; g++)
                    for (int i = 0; i < ExpectedCounts[g]; i++) PrepareFrameImporter(FrameAssetPath(Groups[g], i));
                return;
            }

            for (int g = 0; g < Groups.Length; g++)
            {
                string sheetPath = SheetRoot + "/SHT_Father_" + Groups[g] + ".png";
                var sourceImporter = AssetImporter.GetAtPath(sheetPath) as TextureImporter;
                if (!sourceImporter) throw new FileNotFoundException("Required father pose sheet was not found.", sheetPath);
                if (!sourceImporter.isReadable)
                {
                    sourceImporter.isReadable = true;
                    sourceImporter.SaveAndReimport();
                }
                var source = AssetDatabase.LoadAssetAtPath<Texture2D>(sheetPath);
                if (!source) throw new InvalidOperationException("Could not read pose sheet: " + sheetPath);
                Color32[] sourcePixels = source.GetPixels32();
                var frames = FindSilhouetteComponents(sourcePixels, source.width, source.height);
                if (frames.Count != ExpectedCounts[g])
                    throw new InvalidOperationException("Expected " + ExpectedCounts[g] + " character poses in " + sheetPath + ", found " + frames.Count + ".");

                var rows = new List<List<Component>>();
                foreach (Component frame in frames.OrderBy(x => x.Top(source.height)).ThenBy(x => x.minX))
                {
                    if (rows.Count == 0 || frame.Top(source.height) - rows[rows.Count - 1][0].Top(source.height) > 64)
                        rows.Add(new List<Component>());
                    rows[rows.Count - 1].Add(frame);
                }
                var ordered = rows.SelectMany(row => row.OrderBy(frame => frame.minX)).ToArray();
                if (ordered.Length != ExpectedCounts[g]) throw new InvalidOperationException("Pose row grouping failed for " + sheetPath);
                for (int i = 0; i < ordered.Length; i++)
                    WriteCleanFrame(Groups[g], i, ordered[i], sourcePixels, source.width, source.height);
            }

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            for (int g = 0; g < Groups.Length; g++)
                for (int i = 0; i < ExpectedCounts[g]; i++) PrepareFrameImporter(FrameAssetPath(Groups[g], i));
        }

        static List<Component> FindSilhouetteComponents(Color32[] pixels, int width, int height)
        {
            int[] labels = new int[pixels.Length];
            int[] stack = new int[pixels.Length];
            int nextId = 0;
            var components = new List<Component>();
            for (int seed = 0; seed < pixels.Length; seed++)
            {
                if (labels[seed] != 0 || pixels[seed].a <= AlphaThreshold) continue;
                int id = ++nextId, size = 0, minX = width, minY = height, maxX = 0, maxY = 0;
                int top = 0;
                labels[seed] = id;
                stack[top++] = seed;
                while (top > 0)
                {
                    int p = stack[--top], x = p % width, y = p / width;
                    size++; minX = Mathf.Min(minX, x); maxX = Mathf.Max(maxX, x);
                    minY = Mathf.Min(minY, y); maxY = Mathf.Max(maxY, y);
                    int y0 = Mathf.Max(0, y - 1), y1 = Mathf.Min(height - 1, y + 1);
                    int x0 = Mathf.Max(0, x - 1), x1 = Mathf.Min(width - 1, x + 1);
                    for (int ny = y0; ny <= y1; ny++)
                    for (int nx = x0; nx <= x1; nx++)
                    {
                        int n = ny * width + nx;
                        if (labels[n] != 0 || pixels[n].a <= AlphaThreshold) continue;
                        labels[n] = id;
                        stack[top++] = n;
                    }
                }
                if (size >= MinimumComponentPixels)
                    components.Add(new Component { id = id, area = size, minX = minX, minY = minY, maxX = maxX, maxY = maxY });
            }
            // Store labels for the matching crop pass without retaining temporary work on the component objects.
            lastLabels = labels;
            return components;
        }

        static int[] lastLabels;

        static void WriteCleanFrame(string group, int index, Component component, Color32[] source,
            int sourceWidth, int sourceHeight)
        {
            int x0 = Mathf.Max(0, component.minX - 2), y0 = Mathf.Max(0, component.minY - 2);
            int x1 = Mathf.Min(sourceWidth - 1, component.maxX + 2), y1 = Mathf.Min(sourceHeight - 1, component.maxY + 2);
            int width = x1 - x0 + 1, height = y1 - y0 + 1;
            var cropped = new Color32[width * height];
            for (int y = y0; y <= y1; y++)
            for (int x = x0; x <= x1; x++)
            {
                int sourceIndex = y * sourceWidth + x;
                if (lastLabels[sourceIndex] == component.id)
                {
                    cropped[(y - y0) * width + x - x0] = source[sourceIndex];
                    continue;
                }
                // Keep the original soft antialias just outside the alpha threshold; discard detached sheet debris.
                if (source[sourceIndex].a > 0 && IsNearLabel(lastLabels, x, y, sourceWidth, sourceHeight, component.id))
                    cropped[(y - y0) * width + x - x0] = source[sourceIndex];
            }
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false, true);
            texture.SetPixels32(cropped); texture.Apply(false, false);
            byte[] png = texture.EncodeToPNG();
            UnityEngine.Object.DestroyImmediate(texture);
            string absolute = AbsoluteFramePath(group, index);
            if (!File.Exists(absolute) || !File.ReadAllBytes(absolute).SequenceEqual(png))
                File.WriteAllBytes(absolute, png);
        }

        static bool IsNearLabel(int[] labels, int x, int y, int width, int height, int id)
        {
            for (int dy = -1; dy <= 1; dy++)
            for (int dx = -1; dx <= 1; dx++)
            {
                int nx = x + dx, ny = y + dy;
                if (nx >= 0 && nx < width && ny >= 0 && ny < height && labels[ny * width + nx] == id) return true;
            }
            return false;
        }

        static string FrameName(string group, int index) => group + "_" + index.ToString("00");
        static string FrameAssetPath(string group, int index) => FramesRoot + "/" + FrameName(group, index) + ".png";
        static string AbsoluteFramePath(string group, int index) => Path.Combine(Application.dataPath, "Sofia/VS01/Art/GeneratedFrames/" + FrameName(group, index) + ".png");

        static void PrepareFrameImporter(string path)
        {
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (!importer) throw new FileNotFoundException("Generated pose frame has not imported.", path);
            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            bool changed = importer.textureType != TextureImporterType.Sprite || importer.spriteImportMode != SpriteImportMode.Multiple ||
                importer.spritePixelsPerUnit != PixelsPerUnit || !importer.isReadable || importer.mipmapEnabled ||
                importer.textureCompression != TextureImporterCompression.Uncompressed || settings.spriteMeshType != SpriteMeshType.FullRect;
            if (changed)
            {
                settings.spriteMeshType = SpriteMeshType.FullRect;
                importer.SetTextureSettings(settings);
                // SetTextureSettings can restore the texture type and sprite mode from its snapshot.
                // Apply the sprite-specific fields after it so the import remains a Sprite asset.
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Multiple;
                importer.spritePixelsPerUnit = PixelsPerUnit;
                importer.alphaIsTransparency = true;
                importer.isReadable = true;
                importer.mipmapEnabled = false;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.filterMode = FilterMode.Bilinear;
                importer.userData = string.Empty;
                importer.SaveAndReimport();
                importer = AssetImporter.GetAtPath(path) as TextureImporter;
            }

            var factory = new SpriteDataProviderFactories(); factory.Init();
            var data = factory.GetSpriteEditorDataProviderFromObject(importer);
            data.InitSpriteEditorDataProvider();
            var rects = data.GetSpriteRects();
            if (importer.userData == SkinDataVersion) return;
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            SpriteRect rect = rects.Length == 1 ? rects[0] : default;
            rect.name = Path.GetFileNameWithoutExtension(path);
            if (rect.spriteID == default) rect.spriteID = GUID.Generate();
            rect.rect = new Rect(0, 0, texture.width, texture.height);
            rect.alignment = SpriteAlignment.Custom;
            rect.pivot = new Vector2(.5f, .015f);
            rects = new[] { rect };
            data.SetSpriteRects(rects);
            GUID spriteId = rects[0].spriteID;
            var bones = data.GetDataProvider<ISpriteBoneDataProvider>();
            var mesh = data.GetDataProvider<ISpriteMeshDataProvider>();
            if (bones == null || mesh == null)
                throw new InvalidOperationException("The installed 2D Sprite data providers are not available for " + path);
            bones.SetBones(spriteId, BuildSpriteBones(texture.width, texture.height, rect.pivot));
            BuildSpriteMesh(mesh, spriteId, texture.width, texture.height, rect.pivot);
            data.GetDataProvider<ISpriteNameFileIdDataProvider>()?.SetNameFileIdPairs(
                new[] { new SpriteNameFileIdPair(rects[0].name, spriteId) });
            data.Apply();
            importer.userData = SkinDataVersion;
            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();
        }

        static Sprite[] Frames(string group, int[] indices)
        {
            return indices.Select(index =>
            {
                string path = FrameAssetPath(group, index);
                var sprite = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().FirstOrDefault();
                if (!sprite) throw new InvalidOperationException("Missing imported character pose: " + path);
                return sprite;
            }).ToArray();
        }

        static void CreateAnimationClips()
        {
            // The sheet has no lying-down art: wake begins on the low kneel and rises into the first idle pose.
            CreatePoseClip("AN_Father_Awakening", Frames("Awakening", new[] { 20, 19, 18, 17, 21, 22, 23, 24, 25 }), false, 2.5f);
            CreatePoseClip("AN_Father_Idle", Frames("IdleWalkRun", new[] { 0, 1, 2, 3, 4, 5 }), true, 1.2f);
            CreatePoseClip("AN_Father_Walk", Frames("IdleWalkRun", new[] { 14, 15, 16, 17, 18, 19, 20, 21 }), true, .78f);
            CreatePoseClip("AN_Father_Run", Frames("IdleWalkRun", new[] { 23, 24, 25, 26, 27, 30, 31, 32, 33 }), true, .58f);
            CreatePoseClip("AN_Father_RunStart", Frames("IdleWalkRun", new[] { 3, 4, 5, 12, 14, 17, 19, 21, 22, 23 }), false, .52f);
            CreatePoseClip("AN_Father_RunStop", Frames("IdleWalkRun", new[] { 33, 32, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 19, 17, 14, 12, 5, 4, 3 }), false, .72f);
            CreatePoseClip("AN_Father_Jump", Frames("Jump", new[] { 0, 1, 2, 3, 4, 5, 6, 7 }), false, .48f);
            CreatePoseClip("AN_Father_Fall", Frames("Jump", new[] { 8, 9, 10, 11, 12, 13, 14 }), true, .63f);
            CreatePoseClip("AN_Father_Land", Frames("Jump", new[] { 18, 19, 20, 21 }), false, .38f);
            CreatePoseClip("AN_Father_LookHele", Frames("Curiosity", new[] { 0, 1, 2, 3 }), true, .9f);
            CreatePoseClip("AN_Father_Turn", Frames("Curiosity", new[] { 0, 4, 0 }), false, .24f);
        }

        static AnimationClip CreatePoseClip(string name, Sprite[] frames, bool loop, float duration)
        {
            string path = ClipsRoot + "/" + name + ".anim";
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (!clip)
            {
                clip = new AnimationClip { name = name };
                AssetDatabase.CreateAsset(clip, path);
            }
            foreach (EditorCurveBinding binding in AnimationUtility.GetCurveBindings(clip)) AnimationUtility.SetEditorCurve(clip, binding, null);
            foreach (EditorCurveBinding binding in AnimationUtility.GetObjectReferenceCurveBindings(clip)) AnimationUtility.SetObjectReferenceCurve(clip, binding, null);
            var bindingSprite = EditorCurveBinding.PPtrCurve(RendererPath, typeof(SpriteRenderer), "m_Sprite");
            var keys = new ObjectReferenceKeyframe[frames.Length + (loop ? 1 : 0)];
            for (int i = 0; i < frames.Length; i++)
                keys[i] = new ObjectReferenceKeyframe { time = duration * i / Mathf.Max(1, frames.Length - (loop ? 0 : 1)), value = frames[i] };
            if (loop) keys[keys.Length - 1] = new ObjectReferenceKeyframe { time = duration, value = frames[0] };
            AnimationUtility.SetObjectReferenceCurve(clip, bindingSprite, keys);
            clip.frameRate = frames.Length / duration;
            var settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = loop; settings.loopBlend = false;
            AnimationUtility.SetAnimationClipSettings(clip, settings);
            EditorUtility.SetDirty(clip);
            AddPoseBoneCurves(name, clip, duration);
            return clip;
        }

        static List<SpriteBone> BuildSpriteBones(int width, int height, Vector2 pivot)
        {
            var bones = new List<SpriteBone>(RigBones.Length);
            for (int i = 0; i < RigBones.Length; i++)
            {
                RigBone bone = RigBones[i];
                bones.Add(new SpriteBone
                {
                    name = bone.name,
                    guid = Hash128.Compute("SOFIA_VS01_BONE_" + bone.name).ToString(),
                    position = bone.parent < 0
                        ? new Vector3(pivot.x * width, pivot.y * height, 0f)
                        : bone.position * PixelsPerUnit,
                    rotation = Quaternion.identity,
                    length = bone.length * PixelsPerUnit,
                    parentId = bone.parent,
                    color = Color.white
                });
            }
            return bones;
        }

        static void BuildSpriteMesh(ISpriteMeshDataProvider provider, GUID spriteId, int width, int height, Vector2 pivot)
        {
            var vertices = new Vertex2DMetaData[(MeshColumns + 1) * (MeshRows + 1)];
            for (int y = 0; y <= MeshRows; y++)
            for (int x = 0; x <= MeshColumns; x++)
            {
                float u = x / (float)MeshColumns;
                float v = y / (float)MeshRows;
                int index = y * (MeshColumns + 1) + x;
                vertices[index] = new Vertex2DMetaData
                {
                    position = new Vector2(u * width, v * height),
                    boneWeight = WeightAt(u, v)
                };
            }

            var indices = new int[MeshColumns * MeshRows * 6];
            int cursor = 0;
            for (int y = 0; y < MeshRows; y++)
            for (int x = 0; x < MeshColumns; x++)
            {
                int a = y * (MeshColumns + 1) + x;
                int b = a + 1;
                int c = a + MeshColumns + 1;
                int d = c + 1;
                indices[cursor++] = a; indices[cursor++] = b; indices[cursor++] = c;
                indices[cursor++] = b; indices[cursor++] = d; indices[cursor++] = c;
            }

            provider.SetVertices(spriteId, vertices);
            provider.SetIndices(spriteId, indices);
            provider.SetEdges(spriteId, Array.Empty<Vector2Int>());
        }

        static BoneWeight WeightAt(float u, float v)
        {
            // The supplied paintings already carry the detailed body poses. This
            // shared mesh gives the same rig light, smooth influence zones across
            // the sequence, with the cape receiving the strongest follow-through.
            int core = v > .56f ? 3 : v > .32f ? 2 : 1;
            float cape = SmoothRange(.55f - u, .02f, .24f) * SmoothRange(v, .05f, .16f) * (1f - SmoothRange(v, .80f, .91f));
            if (cape > .015f)
            {
                int branch = v > .58f ? 13 : v > .34f ? 16 : 19;
                float depth = SmoothRange(.48f - u, .01f, .27f);
                int segment = depth > .68f ? 2 : depth > .34f ? 1 : 0;
                int capeBone = branch + segment;
                int innerBone = segment == 0 ? 12 : capeBone - 1;
                return Blend(capeBone, innerBone, Mathf.Lerp(.64f, .94f, cape));
            }

            if (v > .68f && u > .44f)
                return Blend(5, 4, SmoothRange(v, .69f, .85f) * SmoothRange(u, .43f, .59f));

            if (u > .64f && v > .32f && v < .79f)
            {
                int arm = v > .62f ? 6 : v > .47f ? 7 : 8;
                int parent = arm == 6 ? 3 : arm - 1;
                float strength = SmoothRange(u, .64f, .82f) * (1f - SmoothRange(v, .72f, .80f));
                return Blend(arm, parent, strength);
            }

            if (v < .39f && u > .42f)
            {
                int leg = v < .12f ? 11 : v < .23f ? 10 : 9;
                int parent = leg == 11 ? 10 : leg == 10 ? 9 : 1;
                float strength = (1f - SmoothRange(v, .30f, .40f)) * SmoothRange(u, .41f, .55f);
                return Blend(leg, parent, strength);
            }

            return Blend(core, 0, 1f);
        }

        static BoneWeight Blend(int primary, int secondary, float primaryWeight)
        {
            float amount = Mathf.Clamp01(primaryWeight);
            return new BoneWeight
            {
                boneIndex0 = primary, weight0 = amount,
                boneIndex1 = secondary, weight1 = 1f - amount,
                boneIndex2 = 0, weight2 = 0f,
                boneIndex3 = 0, weight3 = 0f
            };
        }

        static float SmoothRange(float value, float min, float max)
        {
            return Smooth01((value - min) / Mathf.Max(.0001f, max - min));
        }

        static float Smooth01(float value)
        {
            float t = Mathf.Clamp01(value);
            return t * t * (3f - 2f * t);
        }

        static void AddPoseBoneCurves(string clipName, AnimationClip clip, float duration)
        {
            string chest = "Facing/SkeletonRoot/Hips/Spine/Chest";
            string head = chest + "/Neck/Head";
            string capeA1 = chest + "/CapeRoot/Cape_A_01";
            string capeA2 = capeA1 + "/Cape_A_02";
            string capeA3 = capeA2 + "/Cape_A_03";
            string capeB1 = chest + "/CapeRoot/Cape_B_01";
            string capeB2 = capeB1 + "/Cape_B_02";
            string capeB3 = capeB2 + "/Cape_B_03";
            string capeC1 = chest + "/CapeRoot/Cape_C_01";
            string capeC2 = capeC1 + "/Cape_C_02";
            string capeC3 = capeC2 + "/Cape_C_03";

            switch (clipName)
            {
                case "AN_Father_Awakening":
                    AddZCurve(clip, chest, duration, 0f, -3f, -1f, 1f, 0f);
                    AddZCurve(clip, head, duration, -6f, -3f, 0f, 1f);
                    AddZCurve(clip, capeB2, duration, 0f, -5f, -2f, 2f, 0f);
                    AddZCurve(clip, capeC3, duration, 0f, -7f, -2f, 3f, 0f);
                    break;
                case "AN_Father_Idle":
                    AddZCurve(clip, chest, duration, -.5f, .6f, -.5f, .6f, -.5f);
                    AddZCurve(clip, head, duration, .5f, -.6f, .5f, -.6f, .5f);
                    AddZCurve(clip, capeA3, duration, 0f, 1.1f, 0f, -1.1f, 0f);
                    AddZCurve(clip, capeB3, duration, 0f, 1.6f, 0f, -1.6f, 0f);
                    AddZCurve(clip, capeC3, duration, 0f, 2f, 0f, -2f, 0f);
                    break;
                case "AN_Father_Walk":
                    AddZCurve(clip, chest, duration, -1.2f, 1.2f, -1.2f, 1.2f, -1.2f);
                    AddZCurve(clip, head, duration, .4f, -.4f, .4f, -.4f, .4f);
                    AddZCurve(clip, capeA3, duration, 0f, -2f, 0f, 2f, 0f);
                    AddZCurve(clip, capeB3, duration, 0f, -3.4f, 0f, 3.4f, 0f);
                    AddZCurve(clip, capeC3, duration, 0f, -4.2f, 0f, 4.2f, 0f);
                    break;
                case "AN_Father_Run":
                    AddZCurve(clip, chest, duration, -2f, 2f, -2f, 2f, -2f);
                    AddZCurve(clip, head, duration, .7f, -.7f, .7f, -.7f, .7f);
                    AddZCurve(clip, capeA2, duration, 0f, -2.8f, 0f, 2.8f, 0f);
                    AddZCurve(clip, capeA3, duration, 0f, -4f, 0f, 4f, 0f);
                    AddZCurve(clip, capeB3, duration, 0f, -6.2f, 0f, 6.2f, 0f);
                    AddZCurve(clip, capeC3, duration, 0f, -7f, 0f, 7f, 0f);
                    break;
                case "AN_Father_RunStart":
                    AddZCurve(clip, chest, duration, 0f, -2f, 1f, -1f, 1.4f, 0f);
                    AddZCurve(clip, head, duration, 0f, 1f, -.5f, .5f, 0f);
                    AddZCurve(clip, capeB3, duration, 0f, -5f, -2f, 2f, 0f);
                    AddZCurve(clip, capeC3, duration, 0f, -7f, -3f, 3f, 0f);
                    break;
                case "AN_Father_RunStop":
                    AddZCurve(clip, chest, duration, 0f, 2f, -3f, -6f, 1f, 0f);
                    AddZCurve(clip, head, duration, 0f, -1f, 2f, 3f, -.5f, 0f);
                    AddZCurve(clip, capeA3, duration, 0f, 4f, 2f, -5f, -1f, 0f);
                    AddZCurve(clip, capeB3, duration, 0f, 7f, 3f, -7f, -2f, 0f);
                    AddZCurve(clip, capeC3, duration, 0f, 9f, 4f, -8f, -2f, 0f);
                    break;
                case "AN_Father_Jump":
                    AddZCurve(clip, chest, duration, 0f, -5f, 3f, 5f, 0f);
                    AddZCurve(clip, head, duration, -1f, -2f, 1f, 2f, 0f);
                    AddZCurve(clip, capeB3, duration, 0f, -6f, -9f, -3f, 0f);
                    AddZCurve(clip, capeC3, duration, 0f, -8f, -12f, -4f, 0f);
                    break;
                case "AN_Father_Fall":
                    AddZCurve(clip, chest, duration, 1.5f, -1.5f, 1.5f, -1.5f, 1.5f);
                    AddZCurve(clip, capeB3, duration, -4f, -1f, 4f, 1f, -4f);
                    AddZCurve(clip, capeC3, duration, -6f, -2f, 6f, 2f, -6f);
                    break;
                case "AN_Father_Land":
                    AddZCurve(clip, chest, duration, 0f, -7f, 2f, 0f);
                    AddZCurve(clip, head, duration, 0f, 3f, -1f, 0f);
                    AddZCurve(clip, capeA3, duration, 0f, -4f, 0f, 1f);
                    AddZCurve(clip, capeB3, duration, 0f, -7f, -2f, 1f);
                    AddZCurve(clip, capeC3, duration, 0f, -10f, -3f, 1f);
                    break;
                case "AN_Father_LookHele":
                    AddZCurve(clip, head, duration, 0f, -3f, 1f, 0f);
                    AddZCurve(clip, capeC3, duration, 0f, 2f, -1f, 0f);
                    break;
                case "AN_Father_Turn":
                    AddZCurve(clip, chest, duration, 0f, -4f, 4f, 0f);
                    AddZCurve(clip, head, duration, 0f, 2f, -2f, 0f);
                    break;
            }
        }

        static void AddZCurve(AnimationClip clip, string path, float duration, params float[] values)
        {
            var keys = new Keyframe[values.Length];
            for (int i = 0; i < values.Length; i++)
                keys[i] = new Keyframe(duration * i / Mathf.Max(1, values.Length - 1), values[i]);
            AnimationUtility.SetEditorCurve(clip, EditorCurveBinding.FloatCurve(path, typeof(Transform), "localEulerAnglesRaw.z"), new AnimationCurve(keys));
        }

        static void CreateController()
        {
            if (AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath)) AssetDatabase.DeleteAsset(ControllerPath);
            var controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
            controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
            controller.AddParameter("IsGrounded", AnimatorControllerParameterType.Bool);
            controller.AddParameter("IsFalling", AnimatorControllerParameterType.Bool);
            controller.AddParameter("LookHele", AnimatorControllerParameterType.Bool);
            controller.AddParameter("Jump", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Land", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Turn", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("RunStart", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("RunStop", AnimatorControllerParameterType.Trigger);

            var sm = controller.layers[0].stateMachine;
            var wake = sm.AddState("Awakening", new Vector3(0, 0));
            var move = sm.AddState("Locomotion", new Vector3(300, 0));
            var runStart = sm.AddState("RunStart", new Vector3(300, 180));
            var runStop = sm.AddState("RunStop", new Vector3(300, 320));
            var jump = sm.AddState("Jump", new Vector3(560, -120));
            var fall = sm.AddState("Fall", new Vector3(790, -120));
            var land = sm.AddState("Land", new Vector3(1020, -120));
            var look = sm.AddState("LookHele", new Vector3(560, 150));
            var turn = sm.AddState("Turn", new Vector3(790, 150));
            sm.defaultState = wake;
            wake.motion = LoadClip("AN_Father_Awakening"); jump.motion = LoadClip("AN_Father_Jump");
            runStart.motion = LoadClip("AN_Father_RunStart"); runStop.motion = LoadClip("AN_Father_RunStop");
            fall.motion = LoadClip("AN_Father_Fall"); land.motion = LoadClip("AN_Father_Land");
            look.motion = LoadClip("AN_Father_LookHele"); turn.motion = LoadClip("AN_Father_Turn");
            var tree = new BlendTree { name = "BT_Father_Locomotion", blendType = BlendTreeType.Simple1D,
                blendParameter = "Speed", useAutomaticThresholds = false };
            AssetDatabase.AddObjectToAsset(tree, controller);
            tree.AddChild(LoadClip("AN_Father_Idle"), 0f);
            tree.AddChild(LoadClip("AN_Father_Walk"), 2.6f);
            tree.AddChild(LoadClip("AN_Father_Run"), 4.3f);
            move.motion = tree;
            sm.AddEntryTransition(wake);
            var wakeExit = wake.AddTransition(move); wakeExit.hasExitTime = true; wakeExit.exitTime = .98f; wakeExit.duration = .08f;
            AddAny(sm, jump, AnimatorConditionMode.If, "Jump");
            AddAny(sm, runStart, AnimatorConditionMode.If, "RunStart");
            AddAny(sm, runStop, AnimatorConditionMode.If, "RunStop");
            AddAny(sm, turn, AnimatorConditionMode.If, "Turn");
            AddAny(sm, land, AnimatorConditionMode.If, "Land");
            AddAny(sm, fall, AnimatorConditionMode.If, "IsFalling");
            AddTransition(jump, fall, false, 0f, AnimatorConditionMode.If, "IsFalling");
            AddTransition(fall, move, true, .98f);
            AddTransition(land, move, true, .98f);
            AddTransition(runStart, move, true, .98f);
            AddTransition(runStop, move, true, .98f);
            AddTransition(turn, move, true, .95f);
            AddTransition(move, look, false, 0f, AnimatorConditionMode.If, "LookHele");
            AddTransition(look, move, false, 0f, AnimatorConditionMode.IfNot, "LookHele");
            EditorUtility.SetDirty(controller);
        }

        static void AddAny(AnimatorStateMachine sm, AnimatorState destination, AnimatorConditionMode mode, string parameter)
        {
            var transition = sm.AddAnyStateTransition(destination);
            transition.hasExitTime = false; transition.duration = .07f; transition.canTransitionToSelf = false;
            transition.AddCondition(mode, 0f, parameter);
        }

        static void AddTransition(AnimatorState from, AnimatorState to, bool exitTime, float exit,
            AnimatorConditionMode? mode = null, string parameter = null)
        {
            var transition = from.AddTransition(to); transition.hasExitTime = exitTime;
            transition.exitTime = exit; transition.duration = .07f;
            if (mode.HasValue) transition.AddCondition(mode.Value, 0f, parameter);
        }

        static AnimationClip LoadClip(string name)
        {
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(ClipsRoot + "/" + name + ".anim");
            if (!clip) throw new FileNotFoundException("Expected generated animation clip is missing.", name);
            return clip;
        }

        public static FatherAnimationDriver Attach(Transform visual, FatherMotor motor)
        {
            visual.localPosition = new Vector3(0f, -.90f, 0f);
            for (int i = visual.childCount - 1; i >= 0; i--) UnityEngine.Object.DestroyImmediate(visual.GetChild(i).gameObject);
            var facing = Child(visual, "Facing");
            var rig = new Transform[RigBones.Length];
            for (int i = 0; i < RigBones.Length; i++)
            {
                RigBone bone = RigBones[i];
                Transform parent = bone.parent < 0 ? facing : rig[bone.parent];
                rig[i] = Bone(bone.name, parent, bone.position);
            }
            var skeleton = rig[0];
            var sprites = Child(facing, "CharacterSprites");
            var rendererObject = Bone("Father_Sprite", sprites, Vector3.zero);
            var renderer = rendererObject.gameObject.AddComponent<SpriteRenderer>();
            renderer.sprite = Frames("IdleWalkRun", new[] { 0 })[0];
            renderer.sharedMaterial = GetSpriteMaterial();
            renderer.sortingOrder = 20;
            rendererObject.transform.localScale = Vector3.one * .94f;
            var skin = rendererObject.gameObject.AddComponent<SpriteSkin>();
            skin.alwaysUpdate = true;
            skin.SetRootBone(skeleton);
            SpriteSkinState skinState = skin.SetBoneTransforms(rig);
            if (skinState != SpriteSkinState.Ready)
                throw new InvalidOperationException("Father SpriteSkin is not ready: " + skinState);
            Transform armHand = rig[8];
            Transform[] capeBones = rig.Where((bone, index) => index >= 13).ToArray();

            var animator = visual.gameObject.AddComponent<Animator>();
            animator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
            animator.applyRootMotion = false; animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            var manager = visual.gameObject.AddComponent<IKManager2D>();
            var solverObject = new GameObject("IK_HeleReach"); solverObject.transform.SetParent(visual, false);
            var solver = solverObject.AddComponent<LimbSolver2D>();
            Transform target = Bone("ReachTarget_Hele", visual, new Vector3(.90f, 1.08f, 0f));
            var chain = solver.GetChain(0); chain.transformCount = 3; chain.effector = armHand; chain.target = target;
            solver.Initialize(); manager.AddSolver(solver); manager.weight = 0f;

            var driver = motor.gameObject.AddComponent<FatherAnimationDriver>();
            driver.Motor = motor; driver.VisualAnimator = animator; driver.FacingRoot = facing;
            driver.IkManager = manager; driver.ReachSolver = solver; driver.ReachTarget = target;
            var capeMotion = motor.gameObject.AddComponent<FatherCapeSecondaryMotion>();
            capeMotion.Configure(motor.Body, capeBones);
            EditorUtility.SetDirty(visual.gameObject);
            return driver;
        }

        static Material GetSpriteMaterial()
        {
            var material = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
            if (material) return material;
            Shader shader = Shader.Find("SOFIA/SpriteCleanup");
            if (!shader) shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
            if (!shader) shader = Shader.Find("Sprites/Default");
            if (!shader) throw new InvalidOperationException("No supported SpriteRenderer shader is available.");
            material = new Material(shader) { name = "VS01CharacterCutout" };
            AssetDatabase.CreateAsset(material, MaterialPath);
            return material;
        }

        static Transform Child(Transform parent, string name)
        {
            var existing = parent.Find(name);
            if (existing) UnityEngine.Object.DestroyImmediate(existing.gameObject);
            var child = new GameObject(name).transform; child.SetParent(parent, false); return child;
        }

        static Transform Bone(string name, Transform parent, Vector3 position)
        {
            var existing = parent.Find(name);
            if (existing) UnityEngine.Object.DestroyImmediate(existing.gameObject);
            var bone = new GameObject(name).transform; bone.SetParent(parent, false); bone.localPosition = position; return bone;
        }
    }
}
