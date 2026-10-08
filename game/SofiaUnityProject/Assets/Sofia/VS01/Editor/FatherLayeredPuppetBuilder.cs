using System;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Sofia.VS01.Editor
{
    /// <summary>
    /// Assembles the extracted painted cutouts as an isolated neutral-pose rig.
    /// PF_Father remains the validated production reference until this prototype
    /// passes animation and silhouette review.
    /// </summary>
    public static class FatherLayeredPuppetBuilder
    {
        const string ScenePath = "Assets/Sofia/Scenes/Dev/SCN_FatherLayeredReview.unity";
        const string PartsPath = "Assets/Sofia/VS01/Art/Layered/Father/Parts/";
        const string PrefabPath = "Assets/Sofia/VS01/Prefabs/PF_Father_LayeredPrototype.prefab";
        const string AnimationFolder = "Assets/Sofia/VS01/Animation/Layered";
        const string ControllerPath = AnimationFolder + "/AC_Father_LayeredPrototype.controller";
        const string IdleClipPath = AnimationFolder + "/AN_Father_Layered_Idle.anim";
        const string WalkClipPath = AnimationFolder + "/AN_Father_Layered_Walk.anim";
        const string LookClipPath = AnimationFolder + "/AN_Father_Layered_LookHele.anim";
        const float ArtScale = 0.68f;
        const int ReviewLayer = 2; // Ignore Raycast; keeps this review camera isolated from additive scenes.

        [MenuItem("SOFIA/VS01/Animation/Build layered Father prototype")]
        public static void BuildForReview()
        {
            Scene scene = EditorSceneManager.GetSceneByPath(ScenePath);
            if (!scene.IsValid() || !scene.isLoaded)
                throw new InvalidOperationException("Open the Father layered review scene first: " + ScenePath);

            SceneManager.SetActiveScene(scene);
            DestroyNamed("PF_Father_LayeredPrototype");
            DestroyNamed("FatherParts_Inspection");

            var root = new GameObject("PF_Father_LayeredPrototype");
            SceneManager.MoveGameObjectToScene(root, scene);
            root.AddComponent<SortingGroup>();

            Transform visual = NewBone(root.transform, "Visual", Vector3.zero);
            visual.localScale = Vector3.one * ArtScale;
            Transform rig = NewBone(visual, "Rig", Vector3.zero);

            Transform pelvis = NewBone(rig, "Pelvis", new Vector3(0f, 1.75f, 0f));
            AddSprite(pelvis, "Pelvis_Belt_Robe", -3);

            Transform farLeg = NewBone(pelvis, "LegFar_Upper", new Vector3(0.03f, 0f, 0f));
            AddSprite(farLeg, "Leg_Far_Pants", -12);
            Transform farFoot = NewBone(farLeg, "LegFar_Foot", new Vector3(0.02f, -0.865f, 0f));
            AddSprite(farFoot, "Leg_Far_Boot", -11);

            Transform nearLeg = NewBone(pelvis, "LegNear_Upper", new Vector3(0.20f, 0f, -0.01f));
            AddSprite(nearLeg, "Leg_Near_Pants", -10);
            Transform nearFoot = NewBone(nearLeg, "LegNear_Foot", new Vector3(0.17f, -0.865f, 0f));
            AddSprite(nearFoot, "Leg_Near_Boot", -9);

            Transform spine = NewBone(pelvis, "Spine", new Vector3(0f, 0.03f, 0f));
            AddSprite(spine, "Torso_Robe", -2);
            Transform chest = NewBone(spine, "Chest", new Vector3(0f, 0.55f, 0f));

            Transform capeRoot = NewBone(chest, "CapeRoot", new Vector3(-0.04f, 0.30f, 0.02f));
            AddCapeChain(capeRoot, "Cape_Back", "Cape_Back_Upper", "Cape_Back_Lower", -30, 0f);
            AddCapeChain(capeRoot, "Cape_Mid", "Cape_Mid_Upper", "Cape_Mid_Lower", -29, -0.075f);
            AddCapeChain(capeRoot, "Cape_Front", "Cape_Front_Upper", "Cape_Front_Lower", -28, -0.15f);

            Transform farShoulder = NewBone(chest, "ArmFar_Upper", new Vector3(0.18f, 0.24f, 0.01f));
            AddSprite(farShoulder, "Arm_Far_Upper", -4);
            Transform farElbow = NewBone(farShoulder, "ArmFar_Forearm", new Vector3(0.015f, -0.67f, 0f));
            AddSprite(farElbow, "Arm_Far_Forearm_Hand", -4);

            Transform nearShoulder = NewBone(chest, "ArmNear_Upper", new Vector3(0.31f, 0.24f, -0.02f));
            AddSprite(nearShoulder, "Arm_Near_Upper", 1);
            Transform nearElbow = NewBone(nearShoulder, "ArmNear_Forearm", new Vector3(0.015f, -0.67f, 0f));
            AddSprite(nearElbow, "Arm_Near_Forearm_Hand", 1);

            Transform collar = NewBone(chest, "ScarfCollar", new Vector3(0.10f, 0.11f, -0.03f));
            AddSprite(collar, "Scarf_Collar", 2);

            Transform neck = NewBone(chest, "Neck", new Vector3(0.30f, 0.40f, 0f));
            Transform head = NewBone(neck, "Head", Vector3.zero);
            AddSprite(head, "Hair_Back", 4);
            AddSprite(head, "Head_Profile", 5);
            AddSprite(head, "Hair_Front", 6);

            Animator animator = root.AddComponent<Animator>();
            animator.applyRootMotion = false;
            animator.runtimeAnimatorController = CreateAnimatorController();

            Camera camera = FindCamera(scene);
            if (camera)
            {
                camera.orthographic = true;
                camera.orthographicSize = 1.38f;
                camera.aspect = 1f;
                camera.transform.position = new Vector3(0.08f, 1.13f, -10f);
                camera.transform.rotation = Quaternion.identity;
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = new Color32(24, 31, 39, 255);
                camera.transparencySortMode = TransparencySortMode.Orthographic;
                camera.cullingMask = ~0;
            }

            EditorUtility.SetDirty(root);
            EditorSceneManager.MarkSceneDirty(scene);
            PrefabUtility.SaveAsPrefabAssetAndConnect(root, PrefabPath, InteractionMode.AutomatedAction);
            AssetDatabase.SaveAssets();
            EditorSceneManager.SaveScene(scene);

            if (camera)
            {
                SavePoseEvidence(root, camera, null, 0f, "father-layered-neutral.png");
                SavePoseEvidence(root, camera, IdleClipPath, 1.0f, "father-layered-idle.png");
                SavePoseEvidence(root, camera, WalkClipPath, 0.20f, "father-layered-walk.png");
                SavePoseEvidence(root, camera, LookClipPath, 0.72f, "father-layered-lookhele.png");
            }
            Selection.activeGameObject = root;
            Debug.Log("Built the neutral cutout rig, Idle/Walk/LookHele animation assets, and review evidence.");
        }

        static void DestroyNamed(string name)
        {
            GameObject old = GameObject.Find(name);
            if (old) UnityEngine.Object.DestroyImmediate(old);
        }

        static Transform NewBone(Transform parent, string name, Vector3 localPosition)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            return go.transform;
        }

        static void AddCapeChain(Transform capeRoot, string chainName, string upperSprite, string lowerSprite, int order, float xOffset)
        {
            Transform upper = NewBone(capeRoot, chainName + "_Upper", new Vector3(xOffset, 0f, 0f));
            AddSprite(upper, upperSprite, order);
            Transform lower = NewBone(upper, chainName + "_Lower", new Vector3(0f, -1.16f, 0f));
            AddSprite(lower, lowerSprite, order);
        }

        static void AddSprite(Transform parent, string spriteName, int sortingOrder)
        {
            string path = PartsPath + "SPR_Father_" + spriteName + ".png";
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (!sprite) throw new FileNotFoundException("Layered Father Sprite was not imported: " + path);

            var go = new GameObject("Sprite_" + spriteName);
            go.transform.SetParent(parent, false);
            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = sortingOrder;
            renderer.sortingLayerName = "Default";
            renderer.drawMode = SpriteDrawMode.Simple;
            renderer.maskInteraction = SpriteMaskInteraction.None;
        }

        static AnimatorController CreateAnimatorController()
        {
            EnsureAssetFolder(AnimationFolder);
            DeleteAssetIfPresent(IdleClipPath);
            DeleteAssetIfPresent(WalkClipPath);
            DeleteAssetIfPresent(LookClipPath);
            DeleteAssetIfPresent(ControllerPath);

            AnimationClip idle = BuildIdleClip();
            AnimationClip walk = BuildWalkClip();
            AnimationClip look = BuildLookClip();
            AssetDatabase.CreateAsset(idle, IdleClipPath);
            AssetDatabase.CreateAsset(walk, WalkClipPath);
            AssetDatabase.CreateAsset(look, LookClipPath);

            AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
            controller.AddParameter("Move", AnimatorControllerParameterType.Float);
            controller.AddParameter("Curious", AnimatorControllerParameterType.Bool);
            AnimatorStateMachine machine = controller.layers[0].stateMachine;

            AnimatorState idleState = machine.AddState("Idle");
            AnimatorState walkState = machine.AddState("Walk");
            AnimatorState lookState = machine.AddState("LookHele");
            idleState.motion = idle;
            walkState.motion = walk;
            lookState.motion = look;
            machine.defaultState = idleState;

            AddTransition(idleState, walkState, AnimatorConditionMode.Greater, "Move", 0.1f);
            AddTransition(walkState, idleState, AnimatorConditionMode.Less, "Move", 0.1f);
            AddTransition(idleState, lookState, AnimatorConditionMode.If, "Curious", 0f);
            AddTransition(walkState, lookState, AnimatorConditionMode.If, "Curious", 0f);
            AddTransition(lookState, idleState, AnimatorConditionMode.IfNot, "Curious", 0f);

            EditorUtility.SetDirty(controller);
            return controller;
        }

        static void AddTransition(AnimatorState from, AnimatorState to, AnimatorConditionMode mode, string parameter, float threshold)
        {
            AnimatorStateTransition transition = from.AddTransition(to);
            transition.hasExitTime = false;
            transition.duration = 0.16f;
            transition.AddCondition(mode, threshold, parameter);
        }

        static AnimationClip BuildIdleClip()
        {
            var clip = NewClip("Idle_Breath", 2f, true);
            AddCurve(clip, "Visual/Rig/Pelvis/Spine", "m_LocalPosition.y",
                K(0f, 0.03f), K(0.5f, 0.042f), K(1f, 0.03f), K(1.5f, 0.042f), K(2f, 0.03f));
            AddCurve(clip, "Visual/Rig/Pelvis/Spine/Chest", "localEulerAnglesRaw.z",
                K(0f, 0f), K(0.5f, -0.4f), K(1f, 0f), K(1.5f, 0.35f), K(2f, 0f));
            AddCurve(clip, "Visual/Rig/Pelvis/Spine/Chest/Neck/Head", "localEulerAnglesRaw.z",
                K(0f, 0f), K(0.5f, 0.25f), K(1f, 0f), K(1.5f, -0.2f), K(2f, 0f));
            AddCapeCurves(clip, 2f, 0.7f);
            return clip;
        }

        static AnimationClip BuildWalkClip()
        {
            var clip = NewClip("Walk", 0.8f, true);
            AddCurve(clip, "Visual/Rig/Pelvis/Spine", "m_LocalPosition.y",
                K(0f, 0.03f), K(0.2f, 0.052f), K(0.4f, 0.03f), K(0.6f, 0.052f), K(0.8f, 0.03f));
            AddCurve(clip, "Visual/Rig/Pelvis/LegNear_Upper", "localEulerAnglesRaw.z",
                K(0f, 8f), K(0.2f, 0f), K(0.4f, -8f), K(0.6f, 0f), K(0.8f, 8f));
            AddCurve(clip, "Visual/Rig/Pelvis/LegFar_Upper", "localEulerAnglesRaw.z",
                K(0f, -8f), K(0.2f, 0f), K(0.4f, 8f), K(0.6f, 0f), K(0.8f, -8f));
            AddCurve(clip, "Visual/Rig/Pelvis/Spine/Chest/ArmNear_Upper", "localEulerAnglesRaw.z",
                K(0f, -7f), K(0.2f, 0f), K(0.4f, 7f), K(0.6f, 0f), K(0.8f, -7f));
            AddCurve(clip, "Visual/Rig/Pelvis/Spine/Chest/ArmFar_Upper", "localEulerAnglesRaw.z",
                K(0f, 7f), K(0.2f, 0f), K(0.4f, -7f), K(0.6f, 0f), K(0.8f, 7f));
            AddCurve(clip, "Visual/Rig/Pelvis/Spine/Chest/ArmNear_Upper/ArmNear_Forearm", "localEulerAnglesRaw.z",
                K(0f, 2f), K(0.2f, 0f), K(0.4f, -2f), K(0.6f, 0f), K(0.8f, 2f));
            AddCurve(clip, "Visual/Rig/Pelvis/Spine/Chest/ArmFar_Upper/ArmFar_Forearm", "localEulerAnglesRaw.z",
                K(0f, -2f), K(0.2f, 0f), K(0.4f, 2f), K(0.6f, 0f), K(0.8f, -2f));
            AddCurve(clip, "Visual/Rig/Pelvis/Spine/Chest", "localEulerAnglesRaw.z",
                K(0f, 0f), K(0.2f, -0.6f), K(0.4f, 0f), K(0.6f, 0.6f), K(0.8f, 0f));
            AddCapeCurves(clip, 0.8f, 4f);
            return clip;
        }

        static AnimationClip BuildLookClip()
        {
            var clip = NewClip("LookHele", 1.2f, false);
            AddCurve(clip, "Visual/Rig/Pelvis/Spine/Chest", "localEulerAnglesRaw.z",
                K(0f, 0f), K(0.55f, -1.6f), K(0.9f, -0.8f), K(1.2f, 0f));
            AddCurve(clip, "Visual/Rig/Pelvis/Spine/Chest/Neck/Head", "localEulerAnglesRaw.z",
                K(0f, 0f), K(0.55f, 6f), K(0.9f, 8f), K(1.2f, 0f));
            AddCapeCurves(clip, 1.2f, 2.8f);
            return clip;
        }

        static void AddCapeCurves(AnimationClip clip, float duration, float amplitude)
        {
            string root = "Visual/Rig/Pelvis/Spine/Chest/CapeRoot/";
            foreach (string chain in new[] { "Cape_Back", "Cape_Mid", "Cape_Front" })
            {
                float phase = chain == "Cape_Back" ? 0f : chain == "Cape_Mid" ? 0.22f : 0.44f;
                AddCurve(clip, root + chain + "_Upper", "localEulerAnglesRaw.z",
                    K(0f, amplitude * Mathf.Sin(phase)), K(duration * 0.25f, amplitude * Mathf.Sin(phase + 1.57f)),
                    K(duration * 0.5f, amplitude * Mathf.Sin(phase + 3.14f)), K(duration * 0.75f, amplitude * Mathf.Sin(phase + 4.71f)),
                    K(duration, amplitude * Mathf.Sin(phase)));
                AddCurve(clip, root + chain + "_Upper/" + chain + "_Lower", "localEulerAnglesRaw.z",
                    K(0f, -amplitude * 0.45f * Mathf.Sin(phase)), K(duration * 0.25f, -amplitude * 0.45f * Mathf.Sin(phase + 1.57f)),
                    K(duration * 0.5f, -amplitude * 0.45f * Mathf.Sin(phase + 3.14f)), K(duration * 0.75f, -amplitude * 0.45f * Mathf.Sin(phase + 4.71f)),
                    K(duration, -amplitude * 0.45f * Mathf.Sin(phase)));
            }
        }

        static AnimationClip NewClip(string name, float duration, bool loop)
        {
            var clip = new AnimationClip { name = name, frameRate = 30f, wrapMode = loop ? WrapMode.Loop : WrapMode.Once };
            AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = loop;
            AnimationUtility.SetAnimationClipSettings(clip, settings);
            return clip;
        }

        static Keyframe K(float time, float value) => new Keyframe(time, value);

        static void AddCurve(AnimationClip clip, string path, string property, params Keyframe[] keys)
        {
            var curve = new AnimationCurve(keys);
            for (int i = 1; i < keys.Length - 1; i++) curve.SmoothTangents(i, 0f);
            AnimationUtility.SetEditorCurve(clip, EditorCurveBinding.FloatCurve(path, typeof(Transform), property), curve);
        }

        static void EnsureAssetFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = Path.GetDirectoryName(path).Replace('\\', '/');
            string name = Path.GetFileName(path);
            EnsureAssetFolder(parent);
            AssetDatabase.CreateFolder(parent, name);
        }

        static void DeleteAssetIfPresent(string path)
        {
            if (AssetDatabase.LoadMainAssetAtPath(path)) AssetDatabase.DeleteAsset(path);
        }

        static void SetLayerRecursively(GameObject root, int layer)
        {
            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
                child.gameObject.layer = layer;
        }

        static Camera FindCamera(Scene scene)
        {
            foreach (GameObject sceneRoot in scene.GetRootGameObjects())
            {
                Camera camera = sceneRoot.GetComponentInChildren<Camera>(true);
                if (camera) return camera;
            }
            return null;
        }

        static string EvidencePath(string fileName)
        {
            string repoRoot = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "..", ".."));
            string folder = Path.Combine(repoRoot, "docs", "evidence", "animation");
            Directory.CreateDirectory(folder);
            return Path.Combine(folder, fileName);
        }

        static void SavePoseEvidence(GameObject root, Camera camera, string clipPath, float time, string fileName)
        {
            AnimationClip clip = string.IsNullOrEmpty(clipPath) ? null : AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
            if (!string.IsNullOrEmpty(clipPath) && !clip)
                throw new FileNotFoundException("Animation clip was not created: " + clipPath);

            GameObject preview = UnityEngine.Object.Instantiate(root);
            preview.name = "EvidencePose";
            int previousMask = camera.cullingMask;
            camera.cullingMask = 1 << ReviewLayer;
            try
            {
                SetLayerRecursively(preview, ReviewLayer);
                if (clip)
                {
                    AnimationMode.StartAnimationMode();
                    AnimationMode.BeginSampling();
                    AnimationMode.SampleAnimationClip(preview, clip, time);
                    AnimationMode.EndSampling();
                }
                SaveEvidence(camera, fileName);
            }
            finally
            {
                if (AnimationMode.InAnimationMode()) AnimationMode.StopAnimationMode();
                camera.cullingMask = previousMask;
                UnityEngine.Object.DestroyImmediate(preview);
            }
        }

        static void SaveEvidence(Camera camera, string fileName)
        {
            const int resolution = 1024;
            var renderTexture = new RenderTexture(resolution, resolution, 24, RenderTextureFormat.ARGB32);
            RenderTexture previousActive = RenderTexture.active;
            RenderTexture previousTarget = camera.targetTexture;
            Texture2D image = null;
            try
            {
                camera.targetTexture = renderTexture;
                RenderTexture.active = renderTexture;
                camera.Render();

                image = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
                image.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
                image.Apply(false, false);
                File.WriteAllBytes(EvidencePath(fileName), image.EncodeToPNG());
            }
            finally
            {
                camera.targetTexture = previousTarget;
                RenderTexture.active = previousActive;
                renderTexture.Release();
                UnityEngine.Object.DestroyImmediate(renderTexture);
                if (image) UnityEngine.Object.DestroyImmediate(image);
            }
        }
    }
}
