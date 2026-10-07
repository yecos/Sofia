using NUnit.Framework;
using UnityEngine;

namespace Sofia.Tests
{
    public sealed class ProjectSmokeEditModeTests
    {
        [Test]
        public void UnityVersionIsPinnedToSixPointSix()
        {
            Assert.That(Application.unityVersion, Does.StartWith("6000.6"));
        }

        [Test]
        public void SofiaRootFolderExists()
        {
            Assert.That(UnityEditor.AssetDatabase.IsValidFolder("Assets/Sofia"), Is.True);
        }
    }
}
