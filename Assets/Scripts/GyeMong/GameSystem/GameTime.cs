using UnityEngine;

namespace GyeMong.GameSystem
{
    // Options pause overlays the time scale requested by gameplay and tutorial events.
    public static class GameTime
    {
        private static float scale = 1f;
        public static bool IsPaused { get; private set; }
        public static float Scale
        {
            get => scale;
            set { scale = value; Apply(); }
        }
        public static void SetPaused(bool paused)
        {
            IsPaused = paused;
            Apply();
        }
        private static void Apply() { Time.timeScale = IsPaused ? 0f : scale; }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset()
        {
            scale = 1f;
            IsPaused = false;
            Apply();
        }
    }
}
