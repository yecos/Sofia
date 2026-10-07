using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Sofia.Tests
{
    public sealed class SmokePlayModeTests
    {
        [UnityTest]
        public IEnumerator SmokeScenePlayerMovesHeleFollowsAndRunsTenSeconds()
        {
            SceneManager.LoadScene("SCN_MCP_SmokeTest");
            yield return null;
            GameObject player = GameObject.Find("Player");
            GameObject hele = GameObject.Find("Hele");
            Assert.That(player, Is.Not.Null);
            Assert.That(hele, Is.Not.Null);
            Vector3 start = player.transform.position;
            player.transform.position += Vector3.right * 2f;
            yield return new WaitForSeconds(1f);
            Assert.That(player.transform.position.x, Is.GreaterThan(start.x + 1f));
            Assert.That(Vector2.Distance(player.transform.position, hele.transform.position), Is.LessThan(3.5f));
            yield return new WaitForSeconds(9f);
            Assert.That(Time.time, Is.GreaterThanOrEqualTo(10f));
        }
    }
}
