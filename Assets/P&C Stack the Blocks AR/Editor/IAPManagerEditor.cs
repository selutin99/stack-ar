using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(IAPManager))]
public class IAPManagerEditor : Editor
{
    string IAPDefine = "IAPENABLED";
    IAPManager mIAPManager;

    BuildTarget activeBuildTarget = BuildTarget.NoTarget;

    public override void OnInspectorGUI()
    {
        mIAPManager = (IAPManager)target;

        if (EditorUserBuildSettings.activeBuildTarget != activeBuildTarget)
        {
            EditorUtil.ToggleDefine(mIAPManager.isIAPEnabled, IAPDefine);
            activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;
        }

        GUILayout.BeginVertical("HelpBox");

        GUILayout.Label("Requirement:");

        EditorGUILayout.HelpBox("• To use IAP, you must enable and import IAP from services window. Check documentation or below link for more details" +
                                    "\n**IMPORTANT** Enabling IAP without doing so will produce compile errors. **IMPORTANT**\n" +
                                    "\n• Platform must be Android/iOS", MessageType.Info);

        if (GUILayout.Button("IAP Guide"))
        {
            Application.OpenURL("https://docs.unity3d.com/Manual/UnityIAP.html");
        }
		mIAPManager.isIAPEnabled = EditorGUILayout.Toggle("Enable IAP", mIAPManager.isIAPEnabled);

        GUILayout.EndVertical();

        GUI.enabled = mIAPManager.isIAPEnabled;

        EditorUtil.DrawDefaultInspector(serializedObject);

        if (GUI.changed)
        {
            EditorUtil.ToggleDefine(mIAPManager.isIAPEnabled, IAPDefine);
            EditorUtility.SetDirty(mIAPManager);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(mIAPManager.gameObject.scene);
        }
    }

}
