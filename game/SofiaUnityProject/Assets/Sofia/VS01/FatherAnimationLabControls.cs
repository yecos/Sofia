using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D.IK;

namespace Sofia.VS01
{
    /// <summary>In-scene review controls for the father's first-block animation states.</summary>
    public sealed class FatherAnimationLabControls : MonoBehaviour
    {
        public Animator VisualAnimator;
        public IKManager2D IkManager;
        public float PlaybackRate = 1f;

        static readonly string[] Labels = { "Awakening", "Idle", "Walk", "Run", "Run Start", "Run Stop", "Jump", "Fall", "Land", "Look Hele", "Turn" };
        static readonly string[] States = { "Awakening", "Locomotion", "Locomotion", "Locomotion", "RunStart", "RunStop", "Jump", "Fall", "Land", "LookHele", "Turn" };
        static readonly float[] Speeds = { 0f, 0f, 1.8f, 4.2f, 4.2f, 1f, 0f, 0f, 0f, 0f, 0f };
        static readonly string[] Hotkeys = { "1", "2", "3", "4", "0", "-", "5", "6", "7", "8", "9" };
        string selected = "Locomotion";
        float blendSpeed;
        float reachWeight;

        void Awake()
        {
            if (!VisualAnimator) VisualAnimator = GetComponentInChildren<Animator>();
            if (!IkManager) IkManager = GetComponentInChildren<IKManager2D>();
        }

        void Start() => Play(1);

        void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null) return;
            if (keyboard.digit1Key.wasPressedThisFrame) Play(0);
            if (keyboard.digit2Key.wasPressedThisFrame) Play(1);
            if (keyboard.digit3Key.wasPressedThisFrame) Play(2);
            if (keyboard.digit4Key.wasPressedThisFrame) Play(3);
            if (keyboard.digit0Key.wasPressedThisFrame) Play(4);
            if (keyboard.minusKey.wasPressedThisFrame) Play(5);
            if (keyboard.digit5Key.wasPressedThisFrame) Play(6);
            if (keyboard.digit6Key.wasPressedThisFrame) Play(7);
            if (keyboard.digit7Key.wasPressedThisFrame) Play(8);
            if (keyboard.digit8Key.wasPressedThisFrame) Play(9);
            if (keyboard.digit9Key.wasPressedThisFrame) Play(10);
            if (keyboard.f12Key.wasPressedThisFrame) CaptureSelected();
        }

        void OnGUI()
        {
            GUI.color = new Color(.91f, .94f, .96f, .96f);
            GUI.Box(new Rect(18, 18, 400, 414), GUIContent.none);
            GUILayout.BeginArea(new Rect(32, 30, 372, 390));
            GUILayout.Label("SOFIA · ANIMATION LAB", TitleStyle());
            GUILayout.Label("Primer bloque · revisión de poses y secondary motion", SubtitleStyle());
            GUILayout.Space(8);

            for (int row = 0; row < 4; row++)
            {
                GUILayout.BeginHorizontal();
                for (int column = 0; column < 3; column++)
                {
                    int index = row * 3 + column;
                    if (index < Labels.Length && GUILayout.Button(Hotkeys[index] + "  " + Labels[index], GUILayout.Height(34))) Play(index);
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(4);
            }

            GUILayout.Space(7);
            GUILayout.Label("Reproducción  " + PlaybackRate.ToString("0.0") + "×");
            PlaybackRate = GUILayout.HorizontalSlider(PlaybackRate, .35f, 1.5f);
            if (VisualAnimator) VisualAnimator.speed = PlaybackRate;
            GUILayout.Space(6);
            GUILayout.Label("Hele · peso del IK  " + reachWeight.ToString("0.00"));
            float nextWeight = GUILayout.HorizontalSlider(reachWeight, 0f, 1f);
            if (!Mathf.Approximately(nextWeight, reachWeight))
            {
                reachWeight = nextWeight;
                if (IkManager) IkManager.weight = reachWeight;
            }
            GUILayout.Space(6);
            GUILayout.Label("Activa: " + CurrentClipName() + "    ·    F12: guardar captura");
            GUILayout.EndArea();
            GUI.color = Color.white;
        }

        public void Play(int index)
        {
            if (!VisualAnimator) return;
            index = Mathf.Clamp(index, 0, States.Length - 1);
            selected = States[index];
            blendSpeed = Speeds[index];
            VisualAnimator.speed = PlaybackRate;
            VisualAnimator.SetFloat("Speed", blendSpeed);
            VisualAnimator.Play(selected, 0, 0f);
            VisualAnimator.Update(0f);
        }

        void CaptureSelected()
        {
            string directory = Path.GetFullPath(Path.Combine(Application.dataPath, "../../../docs/evidence/vs01/animationlab"));
            Directory.CreateDirectory(directory);
            string safeName = CurrentClipName().Replace(' ', '_').ToLowerInvariant();
            ScreenCapture.CaptureScreenshot(Path.Combine(directory, "father_" + safeName + ".png"));
        }

        string CurrentClipName()
        {
            if (!VisualAnimator) return selected;
            var clips = VisualAnimator.GetCurrentAnimatorClipInfo(0);
            return clips.Length > 0 && clips[0].clip ? clips[0].clip.name : selected;
        }

        static GUIStyle TitleStyle()
        {
            return new GUIStyle(GUI.skin.label) { fontSize = 20, fontStyle = FontStyle.Bold, normal = { textColor = new Color(.12f, .19f, .23f) } };
        }

        static GUIStyle SubtitleStyle()
        {
            return new GUIStyle(GUI.skin.label) { fontSize = 11, normal = { textColor = new Color(.24f, .33f, .38f) } };
        }
    }
}
