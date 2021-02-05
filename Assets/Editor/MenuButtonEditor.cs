using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(MenuButton))]
public class MenuButtonEditor : ButtonEditor {
    public override void OnInspectorGUI() {
        MenuButton menuButtonComponent = (MenuButton) target;

        serializedObject.Update();

        EditorGUILayout.LabelField("Custom Properties", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("applicationEventRelay"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("buttonCanvas"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("selectImage"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("backgroundImage"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("buttonImage"));

        if (menuButtonComponent.transform.childCount > 3 && menuButtonComponent.transform.GetChild(3).childCount == 2) {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("headerText"));
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("descriptionText"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("selectOnShow"));
        if (menuButtonComponent.SelectOnShow) {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("selectDelay"));
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("showAnimation"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("hideAnimation"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("selectAnimation"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("deselectAnimation"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("pressAnimation"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("enableAnimation"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("disableAnimation"));

        // if (menuButtonComponent.transition == Selectable.Transition.ColorTint) {
        //     EditorGUILayout.PropertyField(serializedObject.FindProperty("colourTintTargetGraphics"));
        // }

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("onSelect"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("onDeselect"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("onPress"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("onEnable"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("onDisable"));

        if (EditorGUI.EndChangeCheck()) {
            serializedObject.ApplyModifiedProperties();
        }

        EditorGUILayout.Separator();

        EditorGUILayout.LabelField("Default Properties", EditorStyles.boldLabel);

        base.OnInspectorGUI();
    }
}