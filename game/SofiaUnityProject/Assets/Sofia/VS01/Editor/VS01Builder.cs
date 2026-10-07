using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Unity.Cinemachine;

namespace Sofia.VS01.Editor
{
    public static class VS01Builder
    {
        public const string ScenePath = "Assets/Sofia/Scenes/VerticalSlice/SCN_VS01_Despertar.unity";
        const string AssetsRoot = "Assets/Sofia/VS01";
        static Material stone, distant, far, father, yellow, halo;
        static Transform environment;
        [MenuItem("SOFIA/VS01/Build Despertar 0-90s")]
        public static void Build()
        {
            if (EditorApplication.isPlaying) throw new System.InvalidOperationException("Stop Play Mode before building.");
            foreach (var folder in new[] { "Art", "Materials", "Prefabs", "Meshes" }) Directory.CreateDirectory(AssetsRoot + "/" + folder);
            Directory.CreateDirectory(Path.GetDirectoryName(ScenePath)); AssetDatabase.Refresh();
            ConfigureArt();
            stone = Material("Stone", new Color(.78f, .85f, .9f));
            distant = Material("MistArchitecture", new Color(.52f, .6f, .65f));
            far = Material("FarArchitecture", new Color(.62f, .68f, .71f));
            father = Material("FatherIvory", new Color(.86f, .86f, .8f));
            yellow = Material("HeleCore", new Color(1f, .78f, .06f));
            halo = Material("HeleHalo", new Color(.9f, .75f, .32f));
            EnsureRenderer();
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            environment = new GameObject("Environment_Despertar").transform;
            var stoneTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetsRoot + "/Art/TEX_ColdStone.png");
            stone.SetTexture("_BaseMap", stoneTexture); stone.SetTextureScale("_BaseMap", Vector2.one); EditorUtility.SetDirty(stone);
            var foreground = Material("Foreground", new Color(.43f, .52f, .61f));
            foreground.SetTexture("_BaseMap", stoneTexture); foreground.SetTextureScale("_BaseMap", Vector2.one); EditorUtility.SetDirty(foreground);
            Backdrop("Awakening_Distance", 0, "BG_ReferenceAwakening");
            Backdrop("FirstSteps_Distance_A", 45, "BG_ReferenceFirstSteps");
            Backdrop("FirstSteps_Distance_B", 90, "BG_ReferenceFirstSteps");
            Backdrop("Hele_Chamber", 135, "BG_ReferenceHele");
            Backdrop("Follow_Distance", 180, "BG_ReferenceFollow");
            // Playable plane: small step, stone, one short gap, three changes of height.
            Block("Awakening_CircularPlatform", new Vector3(2, -1.1f, 0), new Vector3(24, 2.2f, 3), true);
            Block("BrokenRim", new Vector3(-9.5f, -.7f, -1f), new Vector3(1f, .7f, 2), false, distant);
            Block("FirstSteps_Colonnade", new Vector3(33, -1.1f, 0), new Vector3(38, 2.2f, 3), true);
            Block("FirstSteps_Rise", new Vector3(63, -.8f, 0), new Vector3(22, 2.8f, 3), true);
            Block("JumpStone", new Vector3(64, 1.05f, 0), new Vector3(.8f, .7f, 2), true);
            Block("HeleToSafeEnd", new Vector3(133.75f, -1f, 0), new Vector3(116.5f, 2, 3), true);
            Block("ArcStep_A", new Vector3(140, -.45f, 0), new Vector3(4, 3.1f, 3), true);
            Block("ArcStep_B", new Vector3(155, .1f, 0), new Vector3(4, 4.2f, 3), true);
            Block("ArcStep_C", new Vector3(170, -.2f, 0), new Vector3(4, 3.6f, 3), true);
            Block("EndRim_90s", new Vector3(191, -.1f, -.2f), new Vector3(.35f, .6f, 2), false);
            // Soft physical limits; no route toward the 2:00 bridge yet.
            InvisibleWall("LeftLimit", -10.5f); InvisibleWall("EndOfImplementedBlock", 192.5f);
            // The painted architecture carries the silhouette; avoid synthetic arches over it.
            Block("HeleCrack", new Vector3(127, .025f, -.1f), new Vector3(.15f, .045f, 2), false, Material("Crack", new Color(.15f, .21f, .25f)));

            var player = new GameObject("Father"); player.layer = 2; player.transform.position = new Vector3(0, .91f, 0);
            var collider = player.AddComponent<BoxCollider2D>(); collider.size = new Vector2(.55f, 1.8f);
            var body = player.AddComponent<Rigidbody2D>(); body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            var friction = new PhysicsMaterial2D("VS01_NoFriction") { friction = 0, bounciness = 0 };
            SaveAsset(friction, AssetsRoot + "/Materials/NoFriction.physicsMaterial2D"); collider.sharedMaterial = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(AssetsRoot + "/Materials/NoFriction.physicsMaterial2D");
            var motor = player.AddComponent<FatherMotor>();
            var visual = new GameObject("Visual").transform; visual.SetParent(player.transform, false);
            var fatherSprite = new GameObject("Father_PaintedCutout").AddComponent<SpriteRenderer>();
            fatherSprite.transform.SetParent(visual, false); fatherSprite.transform.localPosition = new Vector3(0, .3f, -1);
            fatherSprite.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetsRoot + "/Art/SPR_Father_Profile.png"); fatherSprite.sortingOrder = 10;
            PrefabUtility.SaveAsPrefabAssetAndConnect(player, AssetsRoot + "/Prefabs/PF_Father.prefab", InteractionMode.AutomatedAction);

            var hele = new GameObject("Hele"); var companion = hele.AddComponent<HeleCompanion>();
            companion.Father = player.transform;
            var heleSprite = new GameObject("Hele_PaintedLight").AddComponent<SpriteRenderer>();
            heleSprite.transform.SetParent(hele.transform, false); heleSprite.transform.localPosition = new Vector3(0, 0, -.5f);
            heleSprite.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetsRoot + "/Art/SPR_Hele.png"); heleSprite.sortingOrder = 20;
            companion.Visuals = new Renderer[] { heleSprite };
            PrefabUtility.SaveAsPrefabAssetAndConnect(hele, AssetsRoot + "/Prefabs/PF_Hele.prefab", InteractionMode.AutomatedAction);

            var cameraObject = new GameObject("Main Camera"); cameraObject.tag = "MainCamera";
            var camera = cameraObject.AddComponent<Camera>(); camera.orthographic = true; camera.orthographicSize = 6.5f;
            camera.backgroundColor = new Color(.69f, .74f, .77f); camera.clearFlags = CameraClearFlags.SolidColor;
            ForegroundFrame(cameraObject.transform);
            Atmosphere(cameraObject.transform);
            cameraObject.AddComponent<AudioListener>();
            var brain = cameraObject.AddComponent<CinemachineBrain>(); brain.UpdateMethod = CinemachineBrain.UpdateMethods.LateUpdate;
            var rig = new GameObject("CM_VS01").AddComponent<CinemachineCamera>(); rig.transform.position = new Vector3(0, 2, -30);
            var lens = rig.Lens; lens.OrthographicSize = 6.5f; lens.ModeOverride = LensSettings.OverrideModes.Orthographic; lens.NearClipPlane = .1f; lens.FarClipPlane = 100; rig.Lens = lens;
            var anchor = new GameObject("CameraAnchor").transform; anchor.position = new Vector3(0, 2, 0); rig.Follow = anchor;
            var composer = rig.gameObject.AddComponent<CinemachinePositionComposer>(); composer.CameraDistance = 30; composer.Damping = new Vector3(.45f, .8f, 0);
            var director = new GameObject("VS01_Director").AddComponent<VS01Director>();
            director.Father = motor; director.Hele = companion; director.Rig = rig; director.Composer = composer; director.CameraAnchor = anchor; director.FatherVisual = visual; director.FatherSprite = fatherSprite;
            var checkpointRoot = new GameObject("Checkpoints").transform;
            director.Checkpoints = new Transform[4];
            Vector3[] points = { new Vector3(0, .91f, 0), new Vector3(60, 1.51f, 0), new Vector3(125, .91f, 0), new Vector3(180, .91f, 0) };
            for (int i = 0; i < 4; i++) { var cp = new GameObject("CP_" + (i * 30).ToString("000")).transform; cp.SetParent(checkpointRoot); cp.position = points[i]; director.Checkpoints[i] = cp; }
            PrefabUtility.SaveAsPrefabAssetAndConnect(environment.gameObject, AssetsRoot + "/Prefabs/PF_DespertarGreybox.prefab", InteractionMode.AutomatedAction);
            EditorSceneManager.SaveScene(scene, ScenePath);
            var scenes = EditorBuildSettings.scenes.Where(x => x.path != ScenePath).ToList(); scenes.Add(new EditorBuildSettingsScene(ScenePath, true)); EditorBuildSettings.scenes = scenes.ToArray();
            AssetDatabase.SaveAssets(); Debug.Log("VS01 built: " + ScenePath);
        }
        static Material Material(string name, Color color)
        {
            string path = AssetsRoot + "/Materials/" + name + ".mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (!mat) { mat = new Material(Shader.Find("Universal Render Pipeline/Unlit")); AssetDatabase.CreateAsset(mat, path); }
            mat.SetColor("_BaseColor", color); EditorUtility.SetDirty(mat); return mat;
        }
        static void ConfigureArt()
        {
            foreach (string name in new[] { "IdleWalkRun", "Jump", "Curiosity", "Awakening" })
            {
                string path = AssetsRoot + "/Art/SpriteSheets/SHT_Father_" + name + ".png";
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null) throw new FileNotFoundException("Missing father animation sheet", path);
                if (importer.textureType == TextureImporterType.Default && !importer.mipmapEnabled) continue;
                importer.textureType = TextureImporterType.Default; importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false; importer.maxTextureSize = 2048; importer.SaveAndReimport();
            }
            foreach (string name in new[] { "SPR_Father", "SPR_Father_Profile", "SPR_Hele", "SPR_StoneWalkway", "SPR_IvyBridge", "SPR_ForegroundIvy", "SPR_DriftingMist", "SPR_Waterfall" })
            {
                string path = AssetsRoot + "/Art/" + name + ".png";
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null) throw new FileNotFoundException("Missing painted VS01 sprite", path);
                float ppu = name == "SPR_Father_Profile" ? 650 : name == "SPR_Father" ? 768 : name == "SPR_StoneWalkway" ? 198.3f : name == "SPR_IvyBridge" ? 120 : name == "SPR_ForegroundIvy" ? 100 : name == "SPR_DriftingMist" ? 100 : name == "SPR_Waterfall" ? 200 : 1600;
                if (importer.textureType == TextureImporterType.Sprite && Mathf.Approximately(importer.spritePixelsPerUnit, ppu)) continue;
                importer.textureType = TextureImporterType.Sprite; importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = ppu; importer.spritePivot = name == "SPR_Father_Profile" ? new Vector2(.5f, .02f) : name == "SPR_Father" ? new Vector2(.5f, .04f) : new Vector2(.5f, .5f);
                importer.alphaIsTransparency = true; importer.mipmapEnabled = false; importer.maxTextureSize = 2048;
                importer.SaveAndReimport();
            }
        }
        static void Backdrop(string name, float x, string imageName)
        {
            string matPath = AssetsRoot + "/Materials/" + imageName + ".mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            if (!mat) { mat = new Material(Shader.Find("SOFIA/SoftBackdrop")); AssetDatabase.CreateAsset(mat, matPath); }
            mat.SetTexture("_BaseMap", AssetDatabase.LoadAssetAtPath<Texture2D>(AssetsRoot + "/Art/" + imageName + ".png"));
            mat.SetColor("_BaseColor", Color.white); EditorUtility.SetDirty(mat);
            Primitive(name, PrimitiveType.Quad, new Vector3(x, 3, 20), new Vector3(50, 28, 1), mat, environment);
        }
        static void ForegroundFrame(Transform camera)
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetsRoot + "/Art/SPR_ForegroundIvy.png");
            if (!sprite) return;
            var renderer = new GameObject("Near_Ivy_CameraFrame").AddComponent<SpriteRenderer>();
            renderer.transform.SetParent(camera, false);
            renderer.transform.localPosition = new Vector3(0, 0, 5);
            renderer.transform.localScale = new Vector3(30f / (sprite.rect.width / sprite.pixelsPerUnit), 17f / (sprite.rect.height / sprite.pixelsPerUnit), 1);
            renderer.sprite = sprite;
            renderer.sortingOrder = 100;
            renderer.color = new Color(1, 1, 1, .65f);
        }
        static void Atmosphere(Transform camera)
        {
            var mist = AssetDatabase.LoadAssetAtPath<Sprite>(AssetsRoot + "/Art/SPR_DriftingMist.png");
            var waterfall = AssetDatabase.LoadAssetAtPath<Sprite>(AssetsRoot + "/Art/SPR_Waterfall.png");
            string flowPath = AssetsRoot + "/Materials/FlowWater.mat";
            var flow = AssetDatabase.LoadAssetAtPath<Material>(flowPath);
            if (!flow) { flow = new Material(Shader.Find("SOFIA/FlowWater")); AssetDatabase.CreateAsset(flow, flowPath); }
            flow.shader = Shader.Find("SOFIA/FlowWater"); flow.SetColor("_Color", Color.white); EditorUtility.SetDirty(flow);
            float[] bands = { 0, 45, 90, 135, 180 };
            for (int i = 0; i < bands.Length; i++)
            {
                var layer = new GameObject("Mid_MistBand_" + i).AddComponent<SpriteRenderer>();
                layer.transform.SetParent(environment, false);
                layer.sprite = mist; layer.sortingOrder = 1;
                layer.color = new Color(.88f, .94f, 1f, .16f);
                layer.transform.localScale = new Vector3(36f / (mist.rect.width / mist.pixelsPerUnit), 4.7f / (mist.rect.height / mist.pixelsPerUnit), 1);
                var motion = layer.gameObject.AddComponent<VS01AtmosphericMotion>();
                motion.CameraTarget = camera; motion.Origin = new Vector3(bands[i], -2.8f, 7);
                motion.Parallax = .14f; motion.DriftAmplitude = .55f; motion.DriftRate = .16f; motion.VerticalAmplitude = .12f; motion.Phase = i * 1.4f;
            }
            float[] falls = { -1, 46, 122, 166 };
            for (int i = 0; i < falls.Length; i++)
            {
                var layer = new GameObject("Mid_FlowingWater_" + i).AddComponent<SpriteRenderer>();
                layer.transform.SetParent(environment, false);
                layer.sprite = waterfall; layer.sortingOrder = 0; layer.sharedMaterial = flow;
                layer.color = new Color(.83f, .94f, 1f, .25f);
                layer.transform.localScale = new Vector3(3.4f / (waterfall.rect.width / waterfall.pixelsPerUnit), 11f / (waterfall.rect.height / waterfall.pixelsPerUnit), 1);
                var motion = layer.gameObject.AddComponent<VS01AtmosphericMotion>();
                motion.CameraTarget = camera; motion.Origin = new Vector3(falls[i], -.8f, 9);
                motion.Parallax = .015f; motion.DriftAmplitude = .035f; motion.DriftRate = 1.8f; motion.VerticalAmplitude = .02f; motion.Phase = i * 1.2f;
            }
        }
        static void SaveAsset(Object obj, string path)
        { if (!AssetDatabase.LoadAssetAtPath<Object>(path)) AssetDatabase.CreateAsset(obj, path); else Object.DestroyImmediate(obj); }
        static GameObject Primitive(string name, PrimitiveType type, Vector3 position, Vector3 scale, Material mat, Transform parent)
        {
            var go = GameObject.CreatePrimitive(type); go.name = name; go.transform.SetParent(parent, false); go.transform.localPosition = position; go.transform.localScale = scale;
            Object.DestroyImmediate(go.GetComponent<Collider>()); go.GetComponent<Renderer>().sharedMaterial = mat; return go;
        }
        static GameObject Block(string name, Vector3 pos, Vector3 scale, bool solid, Material mat = null)
        {
            var go = new GameObject(name); go.transform.SetParent(environment); go.transform.position = pos;
            int segments = Mathf.Max(2, Mathf.CeilToInt(scale.x / 1.5f));
            var vertices = new Vector3[(segments + 1) * 2]; var uv = new Vector2[vertices.Length]; var triangles = new int[segments * 6];
            for (int i = 0; i <= segments; i++)
            {
                float t = i / (float)segments, x = Mathf.Lerp(-scale.x / 2, scale.x / 2, t);
                float chip = i == 0 || i == segments ? .16f : Mathf.PerlinNoise(i * 1.37f, scale.x * .17f) * .13f;
                float underside = Mathf.PerlinNoise(i * .79f, scale.x * .31f) * .23f;
                if (name == "Awakening_CircularPlatform") underside += (Mathf.Abs(t - .5f) * 2f) * .65f;
                vertices[2 * i] = new Vector3(x, scale.y / 2 - chip, 0);
                float visualDepth = solid ? Mathf.Min(scale.y, .72f) : scale.y;
                vertices[2 * i + 1] = new Vector3(x, scale.y / 2 - visualDepth + underside * .35f, 0);
                uv[2 * i] = new Vector2(x / 6f, 1); uv[2 * i + 1] = new Vector2(x / 6f, 0);
                if (i == segments) continue;
                int p = i * 2, k = i * 6;
                triangles[k] = p; triangles[k + 1] = p + 2; triangles[k + 2] = p + 1;
                triangles[k + 3] = p + 1; triangles[k + 4] = p + 2; triangles[k + 5] = p + 3;
            }
            var mesh = new Mesh { vertices = vertices, uv = uv, triangles = triangles }; mesh.RecalculateNormals(); mesh.RecalculateBounds();
            string path = AssetsRoot + "/Meshes/Block_" + name + ".asset"; SaveAsset(mesh, path);
            go.AddComponent<MeshFilter>().sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            var renderer = go.AddComponent<MeshRenderer>(); renderer.sharedMaterial = mat ? mat : stone;
            if (name == "BrokenRim" || name == "EndRim_90s") renderer.enabled = false;
            if (solid)
            {
                renderer.enabled = false;
                int count = Mathf.Max(1, Mathf.CeilToInt(scale.x / 16f));
                float step = scale.x / count;
                for (int i = 0; i < count; i++)
                {
                    bool arch = name == "Awakening_CircularPlatform" || name == "FirstSteps_Colonnade" || name.StartsWith("ArcStep_") || (name == "HeleToSafeEnd" && i % 3 == 1);
                    var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetsRoot + (arch ? "/Art/SPR_IvyBridge.png" : "/Art/SPR_StoneWalkway.png"));
                    var layer = new GameObject("PaintedStone_" + i).AddComponent<SpriteRenderer>();
                    layer.transform.SetParent(go.transform, false);
                    layer.sprite = sprite;
                    layer.sortingOrder = 2;
                    float verticalScale = scale.x < 2f ? .3f : scale.x < 6f ? .48f : arch ? .72f : 1f;
                    float topInset = (arch ? 1.61f / .72f : .59f) * verticalScale;
                    layer.transform.localPosition = new Vector3(-scale.x / 2f + step * (i + .5f), scale.y / 2f - topInset + (arch ? .16f : 0), -.1f);
                    layer.transform.localScale = new Vector3((step + (scale.x < 6f ? .2f : .9f)) / (sprite.rect.width / sprite.pixelsPerUnit), verticalScale, 1);
                }
            }
            if (solid) go.AddComponent<BoxCollider2D>().size = new Vector2(scale.x, scale.y);
            return go;
        }
        static void InvisibleWall(string name, float x)
        { var go = new GameObject(name); go.transform.SetParent(environment); go.transform.position = new Vector3(x, 3, 0); go.AddComponent<BoxCollider2D>().size = new Vector2(1, 20); }
        static void Arc(string name, float x, float y, float radius, float z, Material mat)
        {
            var root = new GameObject(name).transform; root.SetParent(environment); root.position = new Vector3(x, y, z);
            float width = radius * .12f;
            Primitive("LeftPillar", PrimitiveType.Cube, new Vector3(-radius, radius * .6f, 0), new Vector3(width, radius * 1.2f, .6f), mat, root);
            Primitive("RightPillar", PrimitiveType.Cube, new Vector3(radius, radius * .6f, 0), new Vector3(width, radius * 1.2f, .6f), mat, root);
            var verts = new Vector3[66]; var triangles = new int[32 * 6];
            for (int i = 0; i <= 32; i++) { float a = i * Mathf.PI / 32; var v = new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0); verts[i * 2] = v * radius + Vector3.up * radius * 1.2f; verts[i * 2 + 1] = v * (radius - width) + Vector3.up * radius * 1.2f; }
            for (int i = 0; i < 32; i++) { int k = i * 6, p = i * 2; triangles[k] = p; triangles[k+1] = p+1; triangles[k+2] = p+2; triangles[k+3] = p+1; triangles[k+4] = p+3; triangles[k+5] = p+2; }
            var mesh = new Mesh { vertices = verts, triangles = triangles }; mesh.RecalculateNormals(); mesh.RecalculateBounds();
            string path = AssetsRoot + "/Meshes/" + name + ".asset"; SaveAsset(mesh, path);
            var top = new GameObject("Arch"); top.transform.SetParent(root, false); top.AddComponent<MeshFilter>().sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(path); top.AddComponent<MeshRenderer>().sharedMaterial = mat;
        }
        static void EnsureRenderer()
        {
            // Bootstrap asset had no renderer data. Persist a valid URP renderer for scene rendering.
            const string path = AssetsRoot + "/Materials/VS01Renderer.asset";
            var data = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(path);
            if (!data) { data = ScriptableObject.CreateInstance<UniversalRendererData>(); AssetDatabase.CreateAsset(data, path); }
            var pipeline = GraphicsSettings.defaultRenderPipeline as UniversalRenderPipelineAsset;
            if (pipeline) { var so = new SerializedObject(pipeline); var list = so.FindProperty("m_RendererDataList"); if (list.arraySize == 0) list.arraySize = 1; list.GetArrayElementAtIndex(0).objectReferenceValue = data; so.ApplyModifiedPropertiesWithoutUndo(); }
        }
    }
}

