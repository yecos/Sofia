using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

namespace Sofia.VS01.Editor
{
    /// <summary>Creates an isolated, button-driven room for reviewing first-block character animation.</summary>
    public static class FatherAnimationLabBuilder
    {
        const string ScenePath = "Assets/Sofia/Scenes/Dev/SCN_AnimationLab.unity";
        const string FatherPrefabPath = "Assets/Sofia/VS01/Prefabs/PF_Father.prefab";
        const string FloorMaterialPath = "Assets/Sofia/VS01/Materials/MAT_AnimationLabFloor.mat";

        public static void Build()
        {
            string absoluteScenePath = Path.Combine(Application.dataPath, "Sofia/Scenes/Dev/SCN_AnimationLab.unity");
            Directory.CreateDirectory(Path.GetDirectoryName(absoluteScenePath));
            AssetDatabase.Refresh();

            Scene lab = EditorSceneManager.GetSceneByPath(ScenePath);
            if (!lab.IsValid())
            {
                lab = File.Exists(absoluteScenePath)
                    ? EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Additive)
                    : EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            }

            foreach (GameObject root in lab.GetRootGameObjects())
            {
                if (!root.name.StartsWith("AnimationLab") && root.name != "FatherAnimationPreview")
                    throw new System.InvalidOperationException("Refusing to replace unexpected content in SCN_AnimationLab: " + root.name);
                Object.DestroyImmediate(root);
            }

            EnsureFloorMaterial();
            CreateCamera();
            CreateFloor();
            CreateFatherPreview(lab);

            EditorSceneManager.MarkSceneDirty(lab);
            if (!EditorSceneManager.SaveScene(lab, ScenePath))
                throw new IOException("Could not save the animation review scene at " + ScenePath);
            EditorSceneManager.SetActiveScene(lab);
            AssetDatabase.SaveAssets();
        }

        static void EnsureFloorMaterial()
        {
            if (AssetDatabase.LoadAssetAtPath<Material>(FloorMaterialPath)) return;
            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            if (!shader) throw new System.InvalidOperationException("No supported material shader was found for the lab floor.");
            var material = new Material(shader) { name = "MAT_AnimationLabFloor", color = new Color(.17f, .22f, .25f, 1f) };
            AssetDatabase.CreateAsset(material, FloorMaterialPath);
        }

        static void CreateCamera()
        {
            var cameraObject = new GameObject("AnimationLabCamera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 1.05f, -10f);
            var camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 2.45f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(.075f, .105f, .13f, 1f);
            camera.nearClipPlane = .1f;
            camera.farClipPlane = 50f;
        }

        static void CreateFloor()
        {
            var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "AnimationLabFloor";
            floor.transform.position = new Vector3(0f, -.12f, .3f);
            floor.transform.localScale = new Vector3(24f, .22f, .4f);
            Object.DestroyImmediate(floor.GetComponent<Collider>());
            var renderer = floor.GetComponent<MeshRenderer>();
            renderer.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>(FloorMaterialPath);
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;
        }

        static void CreateFatherPreview(Scene targetScene)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(FatherPrefabPath);
            if (!prefab) throw new FileNotFoundException("The built father prefab is missing.", FatherPrefabPath);
            var father = (GameObject)PrefabUtility.InstantiatePrefab(prefab, targetScene);
            father.name = "FatherAnimationPreview";
            father.transform.position = Vector3.zero;
            father.transform.rotation = Quaternion.identity;

            var body = father.GetComponent<Rigidbody2D>();
            if (body)
            {
                body.linearVelocity = Vector2.zero;
                body.gravityScale = 0f;
                body.simulated = false;
            }
            var motor = father.GetComponent<FatherMotor>();
            if (motor) motor.enabled = false;
            var driver = father.GetComponent<FatherAnimationDriver>();
            if (driver) driver.enabled = false;

            var animator = father.GetComponentInChildren<Animator>(true);
            if (!animator || !animator.runtimeAnimatorController)
                throw new System.InvalidOperationException("The father prefab has no AnimatorController to review.");
            var controls = father.AddComponent<FatherAnimationLabControls>();
            controls.VisualAnimator = animator;
            controls.IkManager = father.GetComponentInChildren<UnityEngine.U2D.IK.IKManager2D>(true);
        }
    }
}
