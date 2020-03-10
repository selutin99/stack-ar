
using UnityEditor;

[InitializeOnLoad]
public static class UnityLaunchEvent
{
    private const string k_UnityLaunched = "hasUnityLaunched";

    static UnityLaunchEvent()
    {
        EditorApplication.update += OpenHandler;
    }

    static void OpenHandler()
    {
        if (!SessionState.GetBool(k_UnityLaunched, false))
        {
            SessionState.SetBool(k_UnityLaunched, true);
            if (!EditorPrefs.HasKey("welcomeOpened"))
            {
                PnCWelcomeWindow.Init();
            }
        }
    }
}