using System.Collections;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
namespace Sofia.VS01.Tests
{
    public sealed class VS01PlayTests
    {
        static string Evidence => Path.GetFullPath(Path.Combine(Application.dataPath, "../../../docs/evidence/vs01"));
        IEnumerator Load()
        { SceneManager.LoadScene("SCN_VS01_Despertar"); yield return null; }
        static VS01Director Director => Object.FindAnyObjectByType<VS01Director>();
        static IEnumerator Capture(string name)
        { Directory.CreateDirectory(Evidence); yield return new WaitForEndOfFrame(); ScreenCapture.CaptureScreenshot(Path.Combine(Evidence, name + ".png")); yield return new WaitForSeconds(.3f); }
        [UnityTest]
        public IEnumerator StoryboardFramesAndFallRecovery()
        {
            yield return Load(); var d = Director; var p = d.Father; p.ExternalInput = true;
            Assert.That(d.Checkpoints.Length, Is.EqualTo(4));
            yield return Capture("vs01_000");
            yield return new WaitForSeconds(3f); Assert.That(p.ControlEnabled, Is.True);
            d.GoToCheckpoint(1); yield return new WaitForSeconds(2f); yield return Capture("vs01_030");
            d.GoToCheckpoint(2); d.Hele.SetReviewState(false); yield return new WaitForSeconds(1.5f);
            Assert.That(d.Hele.State, Is.EqualTo(HeleCompanion.Behaviour.Curious)); yield return Capture("vs01_060");
            yield return new WaitForSeconds(6f); Assert.That(d.Hele.State, Is.EqualTo(HeleCompanion.Behaviour.Follow));
            d.GoToCheckpoint(3); yield return new WaitForSeconds(2f); yield return Capture("vs01_090");
            p.Warp(new Vector3(180, -8, 0)); yield return new WaitForSeconds(1f);
            Assert.That(p.transform.position.y, Is.GreaterThan(0)); Assert.That(d.CurrentCheckpoint, Is.EqualTo(3));
            LogAssert.NoUnexpectedReceived();
        }
        [UnityTest]
        public IEnumerator RouteTraversesWithRealMotorAndThreeArcJumps()
        {
            yield return Load(); var d = Director; var p = d.Father; p.ExternalInput = true;
            yield return new WaitForSeconds(3f); p.Axis = 1; p.Running = true;
            float started = Time.time, previousX = p.transform.position.x;
            float[] jumps = { 51.5f, 62.5f, 73.2f, 137.3f, 152.3f, 167.3f }; int next = 0;
            while (p.transform.position.x < 188 && Time.time - started < 60)
            {
                if (next < jumps.Length && p.transform.position.x >= jumps[next] && p.Grounded) { p.QueueJump(); next++; }
                yield return new WaitForFixedUpdate();
                Assert.That(p.transform.position.x, Is.GreaterThanOrEqualTo(previousX - .5f), "Unexpected fall recovery during route"); previousX = p.transform.position.x;
            }
            p.Axis = 0;
            Assert.That(p.transform.position.x, Is.GreaterThanOrEqualTo(188), "Route not traversable");
            Assert.That(d.CurrentCheckpoint, Is.EqualTo(3)); Assert.That(p.JumpCount, Is.GreaterThanOrEqualTo(6));
            yield return new WaitForSeconds(.5f); Assert.That(Mathf.Abs(p.Body.linearVelocity.x), Is.LessThan(.1f));
            LogAssert.NoUnexpectedReceived();
        }
        [UnityTest]
        public IEnumerator CameraIsStableAtRestAndMovesWithoutReversals()
        {
            yield return Load(); var d = Director; var p = d.Father; p.ExternalInput = true;
            yield return new WaitForSeconds(3f); d.GoToCheckpoint(2); yield return new WaitForSeconds(8f);
            Vector3 previous = Camera.main.transform.position; float maxRest = 0, maxReverse = 0, sumDt = 0, maxDt = 0;
            for (int i = 0; i < 90; i++) { yield return null; Vector3 now = Camera.main.transform.position; maxRest = Mathf.Max(maxRest, Vector3.Distance(now, previous)); previous = now; }
            Assert.That(maxRest, Is.LessThan(.002f), "Camera drifts at rest");
            p.Axis = 1;
            for (int i = 0; i < 120; i++) { yield return null; Vector3 now = Camera.main.transform.position; maxReverse = Mathf.Max(maxReverse, previous.x - now.x); previous = now; sumDt += Time.unscaledDeltaTime; maxDt = Mathf.Max(maxDt, Time.unscaledDeltaTime); }
            p.Axis = 0; Assert.That(maxReverse, Is.LessThan(.002f), "Camera reverses during steady movement");
            Directory.CreateDirectory(Evidence);
            File.WriteAllText(Path.Combine(Evidence, "camera_metrics.json"), "{\"frames\":120,\"meanFps\":" + (120f / sumDt).ToString("F2", System.Globalization.CultureInfo.InvariantCulture) + ",\"maxFrameSeconds\":" + maxDt.ToString("F5", System.Globalization.CultureInfo.InvariantCulture) + ",\"maxRestDisplacement\":" + maxRest.ToString("F6", System.Globalization.CultureInfo.InvariantCulture) + ",\"maxReverseDisplacement\":" + maxReverse.ToString("F6", System.Globalization.CultureInfo.InvariantCulture) + "}");
        }
        [UnityTest]
        public IEnumerator ContinuousWalkingRouteReachesStoryboardBeats()
        {
            yield return Load(); var d = Director; var p = d.Father; p.ExternalInput = true;
            float[] times = { 0, -1, -1, -1 };
            float[] jumps = { 51.5f, 62.5f, 73.2f, 137.3f, 152.3f, 167.3f };
            int nextJump = 0, lastCheckpoint = 0;
            bool curiousPause = false;
            p.Axis = 1; p.Running = false;
            while (d.Elapsed < 110f && d.CurrentCheckpoint < 3)
            {
                if (nextJump < jumps.Length && p.transform.position.x >= jumps[nextJump] && p.Grounded)
                { p.QueueJump(); nextJump++; }
                if (d.CurrentCheckpoint > lastCheckpoint)
                {
                    lastCheckpoint = d.CurrentCheckpoint; times[lastCheckpoint] = d.Elapsed;
                    if (lastCheckpoint == 2) { p.Axis = 0; curiousPause = true; }
                }
                if (curiousPause && d.Hele.State == HeleCompanion.Behaviour.Follow)
                { p.Axis = 1; curiousPause = false; }
                yield return new WaitForFixedUpdate();
            }
            p.Axis = 0;
            if (d.CurrentCheckpoint == 3 && times[3] < 0) times[3] = d.Elapsed;
            Directory.CreateDirectory(Evidence);
            string json = "{\"cp030\":" + times[1].ToString("F2", System.Globalization.CultureInfo.InvariantCulture) + ",\"cp060\":" + times[2].ToString("F2", System.Globalization.CultureInfo.InvariantCulture) + ",\"cp090\":" + times[3].ToString("F2", System.Globalization.CultureInfo.InvariantCulture) + ",\"jumpCount\":" + p.JumpCount + ",\"finalX\":" + p.transform.position.x.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) + ",\"heleState\":\"" + d.Hele.State + "\"}";
            File.WriteAllText(Path.Combine(Evidence, "route_timing.json"), json);
            Assert.That(d.CurrentCheckpoint, Is.EqualTo(3), "A first-time walking route did not finish: x=" + p.transform.position.x.ToString("F2") + ", jumps=" + nextJump + ", grounded=" + p.Grounded + ", Hele=" + d.Hele.State);
            Assert.That(times[1], Is.InRange(25f, 35f), "0:30 beat outside storyboard tolerance");
            Assert.That(times[2], Is.InRange(55f, 65f), "1:00 beat outside storyboard tolerance");
            Assert.That(times[3], Is.InRange(85f, 95f), "1:30 beat outside storyboard tolerance");
            LogAssert.NoUnexpectedReceived();
        }
        [UnityTest]
        public IEnumerator CoyoteAndBufferedLandingAcceptJump()
        {
            yield return Load(); var d = Director; var p = d.Father; p.ExternalInput = true;
            yield return new WaitForSeconds(3f);
            p.Warp(new Vector3(73.5f, 1.51f, 0)); yield return new WaitForSeconds(.3f); p.Axis = 1; p.Running = true;
            float deadline = Time.time + 2;
            while (p.Grounded && Time.time < deadline) yield return new WaitForFixedUpdate();
            Assert.That(p.Grounded, Is.False); p.QueueJump(); yield return new WaitForFixedUpdate();
            Assert.That(p.Body.linearVelocity.y, Is.GreaterThan(8), "Coyote jump rejected");
            p.Axis = 0; d.GoToCheckpoint(2); yield return new WaitForSeconds(.3f);
            p.Warp(new Vector3(125, 1.16f, 0)); p.Body.linearVelocity = new Vector2(0, -4);
            int count = p.JumpCount; p.QueueJump(); yield return new WaitForSeconds(.16f);
            Assert.That(p.JumpCount, Is.EqualTo(count + 1), "Buffered landing jump rejected");
            Assert.That(p.Body.linearVelocity.y, Is.GreaterThan(5)); LogAssert.NoUnexpectedReceived();
        }
    }
}

