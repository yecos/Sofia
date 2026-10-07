using UnityEngine;

namespace Sofia.Core
{
    public static class SofiaRuntimeSettings
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize() => Application.targetFrameRate = 60;
    }
}
