using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AdsManager))]
public class AdsManagerEditor : Editor
{
    string adsDefine = "ADSENABLED";

    AdsManager adsManager;

    BuildTarget activeBuildTarget = BuildTarget.NoTarget;

    public override void OnInspectorGUI()
    {
         adsManager = (AdsManager)target;

        if (EditorUserBuildSettings.activeBuildTarget != activeBuildTarget)
        {
            EditorUtil.ToggleDefine(adsManager.isAdEnabled, adsDefine);
            activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;
        }

        GUILayout.BeginVertical(EditorStyles.helpBox);

        GUILayout.Label("Requirement:");

        EditorGUILayout.HelpBox("• To use Ads, you must import the Unity Monetization sdk from the Asset Store (link given below)." +
                                    "\n**IMPORTANT** Enabling ad without doing so will produce compile errors. **IMPORTANT**\n" +
                                    "\n• Do not enable ads in services (Legacy)\n"+
                                    "\n• Platform must be Android/iOS", MessageType.Info);

        if (GUILayout.Button("Click here for Unity Monetization package"))
        {
            Application.OpenURL("https://assetstore.unity.com/packages/add-ons/services/unity-monetization-3-0-1-66123");
        }

        adsManager.isAdEnabled = EditorGUILayout.Toggle("Enable Ad", adsManager.isAdEnabled);

        GUILayout.EndVertical();

        GUI.enabled = adsManager.isAdEnabled;
        EditorUtil.DrawDefaultInspector(serializedObject);
 
        if (GUI.changed) {
            EditorUtil.ToggleDefine(adsManager.isAdEnabled, adsDefine);
            EditorUtility.SetDirty(adsManager);  
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty (adsManager.gameObject.scene);
        }
    }
}
