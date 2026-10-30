using UnityEditor;

// Awake에서 만든 게임과 SDK의 런타임 객체는 script reload로 복원되지 않는다.
// 재생 중에는 기존 assembly를 유지하고, 재생을 끝낸 뒤 컴파일 결과를 적용한다.
[InitializeOnLoad]
internal static class PlayModeAssemblyReloadGuard
{
    private static bool reloadLocked;

    static PlayModeAssemblyReloadGuard()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        EditorApplication.quitting += Unlock;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        // 진입할 때 필요한 domain reload는 허용한다.
        if (state == PlayModeStateChange.EnteredPlayMode && !reloadLocked)
        {
            EditorApplication.LockReloadAssemblies();
            reloadLocked = true;
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            Unlock();
        }
    }

    private static void Unlock()
    {
        if (!reloadLocked) return;
        reloadLocked = false;
        EditorApplication.UnlockReloadAssemblies();
    }
}
