using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
namespace Sofia.VS01
{
    public sealed class VS01Director : MonoBehaviour
    {
        public FatherMotor Father;
        public HeleCompanion Hele;
        public CinemachineCamera Rig;
        public CinemachinePositionComposer Composer;
        public Transform CameraAnchor;
        public Transform[] Checkpoints;
        public Transform FatherVisual;
        public SpriteRenderer FatherSprite;
        public bool IsAwakening => awakening;
        public int CurrentCheckpoint { get; private set; }
        public float Elapsed { get; private set; }
        float fade, lookAhead = 2.5f;
        bool awakening = true;
        void Start() { Application.targetFrameRate = 60; Time.fixedDeltaTime = 1f / 60f; Father.ControlEnabled = false; }
        void Update()
        {
            Elapsed += Time.deltaTime;
            if (awakening)
            {
                Father.ControlEnabled = Elapsed >= 2.5f;
                FatherVisual.localScale = Vector3.Lerp(new Vector3(1.1f, .78f, 1), Vector3.one, Mathf.SmoothStep(0, 1, (Elapsed - .4f) / 2.1f));
                FatherVisual.localPosition = Vector3.down * (.9f * (1f - FatherVisual.localScale.y));
                if (Elapsed >= 2.5f) awakening = false;
            }
            for (int i = CurrentCheckpoint + 1; i < Checkpoints.Length; i++)
                if (Father.transform.position.x >= Checkpoints[i].position.x) CurrentCheckpoint = i;
            if (Father.transform.position.y < -6f) { fade = 1; GoToCheckpoint(CurrentCheckpoint); }
            fade = Mathf.MoveTowards(fade, 0, Time.deltaTime * 1.5f);
            var k = Keyboard.current;
            if (k != null && k.rKey.wasPressedThisFrame) { fade = 1; GoToCheckpoint(CurrentCheckpoint); }
#if UNITY_EDITOR
            if (k != null && k.f1Key.wasPressedThisFrame) GoToCheckpoint(0);
            if (k != null && k.f2Key.wasPressedThisFrame) GoToCheckpoint(1);
            if (k != null && k.f3Key.wasPressedThisFrame) GoToCheckpoint(2);
            if (k != null && k.f4Key.wasPressedThisFrame) GoToCheckpoint(3);
#endif
        }
        void LateUpdate()
        {
            float dt = Time.deltaTime;
            lookAhead = Mathf.Lerp(lookAhead, Father.Axis < -.1f ? -2.5f : 2.5f, 1 - Mathf.Exp(-dt * 2));
            Vector3 desired = awakening ? new Vector3(0, 2, 0) : new Vector3(Father.transform.position.x + lookAhead, 3f + Mathf.Max(0, Father.transform.position.y - 1f) * .3f, 0);
            CameraAnchor.position = desired;
            if (!awakening)
            {
                FatherVisual.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(Elapsed * (Father.Running ? 10f : 6f)) * Mathf.Abs(Father.Axis) * 2f);
                if (FatherSprite && Mathf.Abs(Father.Axis) > .1f) FatherSprite.flipX = Father.Axis < 0;
            }
            float size = Hele.State == HeleCompanion.Behaviour.Curious ? 7.3f : 8f;
            var lens = Rig.Lens; lens.OrthographicSize = awakening ? 6.5f : Mathf.Lerp(lens.OrthographicSize, size, 1 - Mathf.Exp(-dt * .8f)); Rig.Lens = lens;
        }
        public void GoToCheckpoint(int index)
        {
            index = Mathf.Clamp(index, 0, Checkpoints.Length - 1); CurrentCheckpoint = index;
            Vector3 old = CameraAnchor.position;
            Father.Warp(Checkpoints[index].position); Father.ControlEnabled = true;
            awakening = false; FatherVisual.localScale = Vector3.one; FatherVisual.localPosition = Vector3.zero;
            Hele.SetReviewState(index >= 2);
            CameraAnchor.position = new Vector3(Father.transform.position.x + 2.5f, 3f + Mathf.Max(0, Father.transform.position.y - 1f) * .3f, 0);
            Rig.OnTargetObjectWarped(CameraAnchor, CameraAnchor.position - old);
            Rig.PreviousStateIsValid = false;
            var lens = Rig.Lens; lens.OrthographicSize = 8f; Rig.Lens = lens;
        }
        void OnGUI()
        {
            if (fade <= 0) return;
            GUI.color = new Color(.35f, .42f, .5f, fade); GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture); GUI.color = Color.white;
        }
    }
}

