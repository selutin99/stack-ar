using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameMode))]
public class GameModeEditor : Editor
{
    string ARDefine = "ARENABLED";

    GameMode gameMode;

    BuildTarget activeBuildTarget = BuildTarget.NoTarget;

    public override void OnInspectorGUI()
    {
        gameMode = (GameMode)target;

        if (EditorUserBuildSettings.activeBuildTarget != activeBuildTarget)
        {
            EditorUtil.ToggleDefine(gameMode.isAREnabled, ARDefine);
            activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;
        }

        GUILayout.BeginVertical(EditorStyles.helpBox);

        GUILayout.Label("Requirement:");

        EditorGUILayout.HelpBox("• To enable AR, you must import the AR packages. Complete the following steps: " +
                                    "\n1. Go to Window --> Package Manager" +
                                "\n2. Check \"show preview packages\" from the \"advanced option\" (2nd option on menu bar)" +
                                "\n3. Import \"AR Foundation\"" +
                                "\n4. If on Android, import \"ARCore XR Plugin\"" +
                                "\n5. If on iOS, import \"ARkit XR Plugin\"\n" +
                                "\n**IMPORTANT** Enabling AR without importing the mentioned packages will produce compile errors. **IMPORTANT**\n", MessageType.Info);

        GUILayout.Label("Check Documentation for a Detailed guide");
       
        gameMode.isAREnabled = EditorGUILayout.Toggle("Enable AR Mode", gameMode.isAREnabled);

        GUILayout.EndVertical();

        GUI.enabled = gameMode.isAREnabled;
        EditorUtil.DrawDefaultInspector(serializedObject);

        if (GUI.changed)
        {
            EditorUtil.ToggleDefine(gameMode.isAREnabled, ARDefine);
            gameMode.StopAllCoroutines();
            EditorUtility.SetDirty(gameMode);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameMode.gameObject.scene);
        }
    }
}
