using UnityEngine;

namespace Core
{
    [DefaultExecutionOrder(10000)]
    public sealed class EditorApplicationHook : MonoBehaviour
    {
        private static EditorApplicationHook _instance;

        public static void Init()
        {
            if (_instance != null) return;
            var go = new GameObject("[SaveSystem.Hook]");
            DontDestroyOnLoad(go);
            go.hideFlags = HideFlags.HideAndDontSave;
            _instance = go.AddComponent<EditorApplicationHook>();
        }

        private void Update() => SaveSystem.TickFlush();
        private void OnApplicationQuit() => SaveSystem.Flush();
        private void OnApplicationPause(bool pause) { if (pause) SaveSystem.Flush(); }
    }
}
