using UnityEditor;
using UnityEngine;

public class PnCWelcomeWindow : EditorWindow
{
    private readonly static float width = 600;
    private readonly static float height = 465;

    private readonly Rect logoRect = new Rect(0, 30, width, 60);
    private readonly Rect docRect = new Rect(20, 120, width - 40, 50);


    private readonly Rect labelrect = new Rect(0, 185, width, 20);
    private readonly Rect fbRect = new Rect(width / 2 - 70, 210, 60, 60);
    private readonly Rect instaRect = new Rect(width / 2 + 10, 210, 60, 60);


    private readonly Rect rateRect = new Rect(20, 290, width - 40, 80);
    private readonly Rect nevershowRect = new Rect(100, 385, width - 200, 50);

    private readonly string assetsPath = "Assets/P&C Stack the Blocks AR/Editor/Images/";
    Texture PnClogo, fbLogo, instaLogo;

    GUIStyle rateButtonStyle;

    GUIStyle labelstyle;
    [MenuItem("Window/Poly and Code/Welcome")]
    public static void Init()
    {
        PnCWelcomeWindow window = (PnCWelcomeWindow)EditorWindow.GetWindow(typeof(PnCWelcomeWindow), true, "Welcome");
        window.position = new Rect(0, 0, width, height);
        window.minSize = new Vector2(width, height);
        window.maxSize = new Vector2(width, height);
        window.Show();
    }

    private void OnEnable()
    {
        PnClogo = (Texture)EditorGUIUtility.Load(assetsPath + "PnCLogo.png");
        fbLogo = (Texture)EditorGUIUtility.Load(assetsPath + "FBlogo.png");
        instaLogo = (Texture)EditorGUIUtility.Load(assetsPath + "IGlogo.png");
    }

    private void OnGUI()
    {
        rateButtonStyle = new GUIStyle("Button")
        {
            fontSize = 32
        };
        GUI.DrawTexture(logoRect, PnClogo, ScaleMode.ScaleToFit);
        GUI.Box(docRect, "Documentation: Assets\\P&C Stack the Blocks AR\\Documentation - P & C Stack the blocks AR" +
                "\n" +
                "\n For support email at polyandcode@gmail.com");

        labelstyle = new GUIStyle()
        {
            fontSize = 15,
            alignment = TextAnchor.MiddleCenter
        };

        GUI.Label(labelrect, "Follow us on", labelstyle);

        if (GUI.Button(fbRect, fbLogo))
        {
            Application.OpenURL("https://www.facebook.com/Polyandcode");
        }

        if (GUI.Button(instaRect, instaLogo))
        {
            Application.OpenURL("https://www.instagram.com/polyandcode");
        }

        if (GUI.Button(rateRect, "Rate this Asset", rateButtonStyle))
        {
            Application.OpenURL("http://u3d.as/1yhR");
        }

        if (GUI.Button(nevershowRect, "Don't show this again"))
        {
            EditorPrefs.SetInt("welcomeOpened", 1);
            Close();
        }
    }
}
