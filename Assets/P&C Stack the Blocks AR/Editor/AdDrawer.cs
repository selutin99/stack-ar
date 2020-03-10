using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Ad))]
public class AdDrawer : PropertyDrawer
{
    float rows = 4;
    float verticalSpacing = 3;


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Rect foldoutRect = new Rect(position.x, position.y, position.width, base.GetPropertyHeight(property, label));
    
        EditorGUI.indentLevel = 1;

        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label);

        EditorGUI.indentLevel = 2;
        if (property.isExpanded)
        {
            var height = (position.height - (verticalSpacing * (rows))) / rows;
            var placementIDRect = new Rect(position.x, position.y + height + verticalSpacing, position.width, height);
            var delayRect = new Rect(position.x, placementIDRect.y + placementIDRect.height + verticalSpacing, position.width, height);
            var useFrequencyRect = new Rect(position.x, delayRect.y + delayRect.height + verticalSpacing, position.width / 2, height);
            var frequencyRect = new Rect(position.x + position.width / 2, delayRect.y + delayRect.height + verticalSpacing, position.width / 2, height);

            EditorGUI.PropertyField(placementIDRect, property.FindPropertyRelative("placementID"), new GUIContent("Placement ID"));
            EditorGUI.PropertyField(delayRect, property.FindPropertyRelative("delay"), new GUIContent("Delay"));
            EditorGUI.PropertyField(useFrequencyRect, property.FindPropertyRelative("useFrequency"), new GUIContent("Use Frequency"));

            //In case class/inspector GUI with this property drawer handles "GUI.enabled". If the parent has it true then only property drawer should be able to change it, else everything is disabled.
            if (GUI.enabled != false)
            {
                GUI.enabled = property.FindPropertyRelative("useFrequency").boolValue;
            }
            EditorGUI.PropertyField(frequencyRect, property.FindPropertyRelative("adFrequency"), new GUIContent("Frequency"));

        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = base.GetPropertyHeight(property, label) * (property.isExpanded ? rows : 1);
        float extra = property.isExpanded ? verticalSpacing * (rows) : 0;
        return height + extra;
    }
}
