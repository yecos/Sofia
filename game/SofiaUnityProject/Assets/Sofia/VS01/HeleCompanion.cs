using UnityEngine;
namespace Sofia.VS01
{
    public sealed class HeleCompanion : MonoBehaviour
    {
        public enum Behaviour { Hidden, Curious, Follow }
        public Transform Father;
        public Renderer[] Visuals;
        public Behaviour State { get; private set; }
        public float RevealX = 125f;
        float phase;
        Vector3 velocity, origin;
        public void Reveal()
        { if (State != Behaviour.Hidden) return; State = Behaviour.Curious; phase = 0; origin = new Vector3(RevealX + 2f, .4f, -.3f); transform.position = origin; Show(true); }
        void Start() { if (State == Behaviour.Hidden) Show(false); }
        void Show(bool visible) { foreach (var r in Visuals) if (r) r.enabled = visible; }
        public void SetReviewState(bool visible)
        { State = visible ? Behaviour.Follow : Behaviour.Hidden; velocity = Vector3.zero; Show(visible); if (Father) transform.position = Father.position + new Vector3(2.5f, 1.5f, -.3f); }
        void LateUpdate()
        {
            if (!Father) return;
            if (State == Behaviour.Hidden) { if (Father.position.x >= RevealX) Reveal(); else return; }
            phase += Time.deltaTime;
            Vector3 desired;
            if (State == Behaviour.Curious)
            {
                // Two circles, a shy retreat, then a pause before guiding.
                if (phase < 4f) desired = origin + new Vector3(Mathf.Sin(phase * Mathf.PI) * .8f, 1f + Mathf.Cos(phase * Mathf.PI) * .55f, 0);
                else if (phase < 6f) desired = origin + new Vector3(Father.position.x > RevealX + 1 ? 2f : 1f, 1.4f, 0);
                else { State = Behaviour.Follow; desired = Father.position + new Vector3(2.5f, 1.5f, -.3f); }
            }
            else
            {
                float lead = Mathf.Clamp(2.8f - Mathf.Abs(Father.position.x - transform.position.x) * .15f, 1.3f, 2.8f);
                desired = Father.position + new Vector3(lead, 1.5f + Mathf.Sin(phase * 2.5f) * .16f, -.3f);
            }
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, .32f);
        }
    }
}
