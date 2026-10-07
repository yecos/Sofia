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
            foreach (var folder in new[] { "Materials", "Prefabs", "Meshes" }) Directory.CreateDirectory(AssetsRoot + "/" + folder);
            Directory.CreateDirectory(Path.GetDirectoryName(ScenePath)); AssetDatabase.Refresh();
            stone = Material("Stone", new Color(.31f, .38f, .44f));
            distant = Material("MistArchitecture", new Color(.52f, .6f, .65f));
            far = Material("FarArchitecture", new Color(.62f, .68f, .71f));
            father = Material("FatherIvory", new Color(.86f, .86f, .8f));
            yellow = Material("HeleCore", new Color(1f, .78f, .06f));
            halo = Material("HeleHalo", new Color(.9f, .75f, .32f));
            EnsureRenderer();
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            environment = new GameObject("Environment_Despertar").transform;
            // Playable plane: small step, stone, one short gap, three changes of height.
            var disk = Primitive("Awakening_CircularPlatform", PrimitiveType.Cylinder, new Vector3(2, -1.1f, 0), new Vector3(24, 1.1f, 24), stone, environment);
            var diskCollider = disk.AddComponent<BoxCollider2D>(); diskCollider.size = new Vector2(1, 2);
            Block("BrokenRim", new Vector3(-9.5f, -.7f, -1f), new Vector3(1f, .7f, 2), false, distant);
            Block("FirstSteps_Rise", new Vector3(20, -.75f, 0), new Vector3(12, 2.7f, 3), true);
            Block("JumpStone", new Vector3(22, .95f, 0), new Vector3(.8f, .7f, 2), true);
            Block("Landing_HeleCrack", new Vector3(38, -1f, 0), new Vector3(20, 2, 3), true);
            Block("ArcStep_A", new Vector3(50, -.45f, 0), new Vector3(4, 3.1f, 3), true);
            Block("ArcStep_B", new Vector3(54, .1f, 0), new Vector3(4, 4.2f, 3), true);
            Block("ArcStep_C", new Vector3(58, -.55f, 0), new Vector3(4, 2.9f, 3), true);
            Block("SafeEnd_90s", new Vector3(65, -1f, 0), new Vector3(10, 2, 3), true);
            // Soft physical limits; no route toward the 2:00 bridge yet.
            InvisibleWall("LeftLimit", -10.5f); InvisibleWall("EndOfImplementedBlock", 70.5f);
            for (int i = 0; i < 9; i++)
            { Arc("DistantArc_" + i, -24 + i * 14, -1, 10 + (i % 3) * 4, 12, i % 2 == 0 ? distant : far); }
            for (int i = 0; i < 4; i++) Arc("RouteArc_" + i, 46 + i * 7, i % 2, 4.5f + i % 2, 3, stone);
            for (int i = 0; i < 12; i++)
                Block("Foreground_Rubble_" + i, new Vector3(-10 + i * 7, -3.8f, -3), new Vector3(4f, 1.2f + i % 3, 1), false, Material("Foreground", new Color(.19f, .25f, .3f)));
            Block("HeleCrack", new Vector3(40, .025f, -.1f), new Vector3(.15f, .045f, 2), false, Material("Crack", new Color(.15f, .21f, .25f)));

            var player = new GameObject("Father"); player.layer = 2; player.transform.position = new Vector3(0, .91f, 0);
            var collider = player.AddComponent<BoxCollider2D>(); collider.size = new Vector2(.55f, 1.8f);
            var body = player.AddComponent<Rigidbody2D>(); body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            var friction = new PhysicsMaterial2D("VS01_NoFriction") { friction = 0, bounciness = 0 };
            SaveAsset(friction, AssetsRoot + "/Materials/NoFriction.physicsMaterial2D"); collider.sharedMaterial = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(AssetsRoot + "/Materials/NoFriction.physicsMaterial2D");
            var motor = player.AddComponent<FatherMotor>();
            var visual = new GameObject("Visual").transform; visual.SetParent(player.transform, false);
            var torso = Primitive("FatherBody", PrimitiveType.Capsule, new Vector3(0, -.1f, 0), new Vector3(.5f, .8f, .45f), father, visual);
            Primitive("FatherHead", PrimitiveType.Sphere, new Vector3(0, .78f, 0), Vector3.one * .32f, father, visual);
            PrefabUtility.SaveAsPrefabAssetAndConnect(player, AssetsRoot + "/Prefabs/PF_Father.prefab", InteractionMode.AutomatedAction);

            var hele = new GameObject("Hele"); var companion = hele.AddComponent<HeleCompanion>();
            companion.Father = player.transform;
            var outer = Primitive("Halo", PrimitiveType.Sphere, Vector3.zero, Vector3.one * .22f, halo, hele.transform);
            var core = Primitive("Core", PrimitiveType.Sphere, new Vector3(0, 0, -.12f), Vector3.one * .13f, yellow, hele.transform);
            companion.Visuals = new[] { outer.GetComponent<Renderer>(), core.GetComponent<Renderer>() };
            PrefabUtility.SaveAsPrefabAssetAndConnect(hele, AssetsRoot + "/Prefabs/PF_Hele.prefab", InteractionMode.AutomatedAction);

            var cameraObject = new GameObject("Main Camera"); cameraObject.tag = "MainCamera";
            var camera = cameraObject.AddComponent<Camera>(); camera.orthographic = true; camera.orthographicSize = 20;
            camera.backgroundColor = new Color(.69f, .74f, .77f); camera.clearFlags = CameraClearFlags.SolidColor;
            cameraObject.AddComponent<AudioListener>();
            var brain = cameraObject.AddComponent<CinemachineBrain>(); brain.UpdateMethod = CinemachineBrain.UpdateMethods.LateUpdate;
            var rig = new GameObject("CM_VS01").AddComponent<CinemachineCamera>(); rig.transform.position = new Vector3(0, 5, -30);
            var lens = rig.Lens; lens.OrthographicSize = 20; lens.ModeOverride = LensSettings.OverrideModes.Orthographic; lens.NearClipPlane = .1f; lens.FarClipPlane = 100; rig.Lens = lens;
            var anchor = new GameObject("CameraAnchor").transform; anchor.position = new Vector3(0, 5, 0); rig.Follow = anchor;
            var composer = rig.gameObject.AddComponent<CinemachinePositionComposer>(); composer.CameraDistance = 30; composer.Damping = new Vector3(.45f, .8f, 0);
            var director = new GameObject("VS01_Director").AddComponent<VS01Director>();
            director.Father = motor; director.Hele = companion; director.Rig = rig; director.Composer = composer; director.CameraAnchor = anchor; director.FatherVisual = visual;
            var checkpointRoot = new GameObject("Checkpoints").transform;
            director.Checkpoints = new Transform[4];
            Vector3[] points = { new Vector3(0, .91f, 0), new Vector3(18, 1.51f, 0), new Vector3(38, .91f, 0), new Vector3(56, 3.11f, 0) };
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
        static void SaveAsset(Object obj, string path)
        { if (!AssetDatabase.LoadAssetAtPath<Object>(path)) AssetDatabase.CreateAsset(obj, path); else Object.DestroyImmediate(obj); }
        static GameObject Primitive(string name, PrimitiveType type, Vector3 position, Vector3 scale, Material mat, Transform parent)
        {
            var go = GameObject.CreatePrimitive(type); go.name = name; go.transform.SetParent(parent, false); go.transform.localPosition = position; go.transform.localScale = scale;
            Object.DestroyImmediate(go.GetComponent<Collider>()); go.GetComponent<Renderer>().sharedMaterial = mat; return go;
        }
        static GameObject Block(string name, Vector3 pos, Vector3 scale, bool solid, Material mat = null)
        { var go = Primitive(name, PrimitiveType.Cube, pos, scale, mat ? mat : stone, environment); if (solid) go.AddComponent<BoxCollider2D>(); return go; }
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

