using System.IO;
using Sofia.CameraSystem;
using Sofia.Gameplay;
using Sofia.Hele;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace Sofia.Editor
{
    public static class SofiaProjectBuilder
    {
        private const string Root = "Assets/Sofia";
        private const string ScenePath = Root + "/Scenes/Dev/SCN_MCP_SmokeTest.unity";

        [MenuItem("SOFIA/Build MCP Smoke Test")]
        public static void Build()
        {
            foreach (string folder in new[] { "Art/Characters", "Art/Environments", "Art/Props", "Art/UI", "Animation/Father", "Animation/Hele", "Animation/Sofi", "Audio", "Characters", "Environments", "Materials", "Prefabs/Characters", "Prefabs/Environment", "Prefabs/Gameplay", "Scenes/Bootstrap", "Scenes/Dev", "Scenes/VerticalSlice", "Scripts/Core", "Scripts/Character", "Scripts/Camera", "Scripts/Gameplay", "Scripts/Hele", "Scripts/Narrative", "Scripts/Audio", "Scripts/Editor", "Scripts/Tests", "Shaders", "Settings", "VFX" })
                Directory.CreateDirectory(Path.Combine(Application.dataPath, "Sofia/" + folder));

            ApplyProjectSettings();
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            CreateCamera();
            CreateBlock("Ground", new Vector3(0, -2.2f, 0), new Vector3(18, 0.6f, 1), new Color(0.18f, 0.23f, 0.32f));
            CreateBlock("Platform_A", new Vector3(-4, 0.2f, 0), new Vector3(3, 0.45f, 1), new Color(0.25f, 0.35f, 0.48f));
            CreateBlock("Platform_B", new Vector3(0, 1.4f, 0), new Vector3(3, 0.45f, 1), new Color(0.32f, 0.42f, 0.55f));
            CreateBlock("Platform_C", new Vector3(4.5f, 0.1f, 0), new Vector3(3, 0.45f, 1), new Color(0.25f, 0.35f, 0.48f));
            GameObject player = CreateBlock("Player", new Vector3(-5, -1.1f, 0), new Vector3(0.8f, 1.4f, 1), new Color(0.25f, 0.65f, 1f));
            Rigidbody2D body = player.AddComponent<Rigidbody2D>();
            body.freezeRotation = true;
            player.AddComponent<PlayerController>();
            GameObject groundCheck = new("GroundCheck");
            groundCheck.transform.SetParent(player.transform);
            groundCheck.transform.localPosition = new Vector3(0, -0.72f, 0);
            var controller = player.GetComponent<PlayerController>();
            var groundField = typeof(PlayerController).GetField("groundCheck", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            groundField?.SetValue(controller, groundCheck.transform);
            GameObject hele = CreateBlock("Hele", new Vector3(-3.2f, 0.4f, -0.1f), new Vector3(0.55f, 0.55f, 1), new Color(1f, 0.82f, 0.08f));
            HeleFollow follow = hele.AddComponent<HeleFollow>();
            follow.Target = player.transform;
            Camera.main.GetComponent<CameraFollow>().Target = player.transform;
            RenderSettings.ambientLight = new Color(0.22f, 0.25f, 0.35f);
            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("SOFIA MCP smoke scene built: " + ScenePath);
        }

        public static void CaptureSmokeScreenshot()
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            Camera camera = Camera.main;
            if (!camera) throw new System.Exception("Main Camera not found for smoke screenshot.");
            const int width = 1280;
            const int height = 720;
            RenderTexture target = new(width, height, 24, RenderTextureFormat.ARGB32);
            Texture2D image = new(width, height, TextureFormat.RGB24, false);
            camera.targetTexture = target;
            camera.Render();
            RenderTexture.active = target;
            image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            image.Apply();
            string output = Path.GetFullPath(Path.Combine(Application.dataPath, "../../docs/evidence/setup/mcp-smoke-test.png"));
            Directory.CreateDirectory(Path.GetDirectoryName(output));
            File.WriteAllBytes(output, image.EncodeToPNG());
            camera.targetTexture = null;
            RenderTexture.active = null;
            Object.DestroyImmediate(image);
            Object.DestroyImmediate(target);
            Debug.Log("SOFIA smoke screenshot saved: " + output);
        }

        private static GameObject CreateBlock(string name, Vector3 position, Vector3 scale, Color color)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.position = position;
            go.transform.localScale = scale;
            Shader shader = Shader.Find("Sprites/Default") ?? Shader.Find("Unlit/Color") ?? Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Standard");
            Material material = new(shader) { color = color };
            go.GetComponent<Renderer>().sharedMaterial = material;
            Object.DestroyImmediate(go.GetComponent<BoxCollider>());
            go.AddComponent<BoxCollider2D>();
            return go;
        }

        private static void CreateCamera()
        {
            GameObject go = new("Main Camera");
            go.tag = "MainCamera";
            go.transform.position = new Vector3(0, 1, -10);
            Camera camera = go.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 5.5f;
            camera.backgroundColor = new Color(0.035f, 0.05f, 0.1f);
            camera.clearFlags = CameraClearFlags.SolidColor;
            go.AddComponent<CameraFollow>();
        }

        private static void ApplyProjectSettings()
        {
            PlayerSettings.colorSpace = ColorSpace.Linear;
            EditorSettings.serializationMode = SerializationMode.ForceText;
            EditorSettings.externalVersionControl = "Visible Meta Files";
            PlayerSettings.runInBackground = true;
            Application.targetFrameRate = 60;
            const string urpPath = Root + "/Settings/SofiaURP.asset";
            UniversalRenderPipelineAsset urp = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(urpPath);
            if (!urp)
            {
                urp = ScriptableObject.CreateInstance<UniversalRenderPipelineAsset>();
                AssetDatabase.CreateAsset(urp, urpPath);
            }
            GraphicsSettings.defaultRenderPipeline = urp;
            QualitySettings.renderPipeline = urp;
        }
    }
}
