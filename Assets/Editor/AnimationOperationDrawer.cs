using CustomUtilities;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AnimationOperation))]
public class AnimationOperationDrawer : PropertyDrawer {
    private const int BaseLineCount = 4;
    private const float LineSpacing = 4f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        SerializedProperty typeProperty = property.FindPropertyRelative("type");
        UIAnimationType selectedAnimationType = (UIAnimationType) typeProperty.enumValueIndex;
        SerializedProperty easingProperty = property.FindPropertyRelative("easing");
        EaseType easingType = (EaseType) easingProperty.enumValueIndex;

        EditorGUI.BeginProperty(position, label, property);

        float defaultLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = Utility.GetStringDimensions("Element ___", 5f).x;
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label, EditorStyles.boldLabel);

        float xOffset = Utility.GetStringDimensions("Element ___", 7f).x;
        position.x -= xOffset;
        position.width += xOffset;
        position.y += EditorGUIUtility.singleLineHeight + LineSpacing;

        EditorGUIUtility.labelWidth = Utility.GetStringDimensions("Target Object", 5f).x;
        Rect targetObjectsRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(targetObjectsRect, property.FindPropertyRelative("targetObject"));

        position.y += EditorGUIUtility.singleLineHeight + LineSpacing;

        Rect typeRect;
        if (selectedAnimationType != UIAnimationType.Activate) {
            typeRect = new Rect(position.x, position.y, position.width / 2 - 4f, EditorGUIUtility.singleLineHeight);
        } else {
            typeRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        }

        Rect easingRect = new Rect(position.x + position.width / 2 + 4f, position.y, position.width / 2 - 4f, EditorGUIUtility.singleLineHeight);

        position.y += EditorGUIUtility.singleLineHeight + LineSpacing;

        Rect delayRect = new Rect(position.x, position.y, position.width / 2 - 4f, EditorGUIUtility.singleLineHeight);
        Rect durationRect = new Rect(position.x + position.width / 2 + 4f, position.y, position.width / 2 - 4f, EditorGUIUtility.singleLineHeight);

        EditorGUI.PropertyField(typeRect, property.FindPropertyRelative("type"), GUIContent.none);
        if (selectedAnimationType != UIAnimationType.Activate) {
            EditorGUI.PropertyField(easingRect, property.FindPropertyRelative("easing"), GUIContent.none);
        }

        EditorGUIUtility.labelWidth = Utility.GetStringDimensions("Delay", 5f).x;
        EditorGUI.PropertyField(delayRect, property.FindPropertyRelative("delay"));
        if (selectedAnimationType != UIAnimationType.Activate && easingType != EaseType.None) {
            EditorGUIUtility.labelWidth = Utility.GetStringDimensions("Duration", 5f).x;
            EditorGUI.PropertyField(durationRect, property.FindPropertyRelative("duration"));
        }

        position.y += EditorGUIUtility.singleLineHeight + LineSpacing;

        switch (selectedAnimationType) {
            case UIAnimationType.Activate:
                position.y -= EditorGUIUtility.singleLineHeight + LineSpacing;
                Rect activateRect = new Rect(position.x + position.width / 2 + 4f, position.y, position.width / 2, EditorGUIUtility.singleLineHeight);
                EditorGUIUtility.labelWidth = Utility.GetStringDimensions("Activate", 5f).x;
                EditorGUI.PropertyField(activateRect, property.FindPropertyRelative("activate"));

                break;
            case UIAnimationType.Scale:
                Rect startScaleRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                position.y += EditorGUIUtility.singleLineHeight + LineSpacing;
                if (EditorGUIUtility.currentViewWidth < 345f) position.y += EditorGUIUtility.singleLineHeight + LineSpacing;
                Rect targetScaleRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

                EditorGUIUtility.labelWidth = Utility.GetStringDimensions("Target Scale", 5f).x;
                if (easingType != EaseType.None) {
                    EditorGUI.PropertyField(startScaleRect, property.FindPropertyRelative("scaleSettings").FindPropertyRelative("startScale"));
                    EditorGUI.PropertyField(targetScaleRect, property.FindPropertyRelative("scaleSettings").FindPropertyRelative("targetScale"));
                } else {
                    EditorGUI.PropertyField(startScaleRect, property.FindPropertyRelative("scaleSettings").FindPropertyRelative("targetScale"));
                }

                break;
            case UIAnimationType.Rotate:
                Rect startRotationRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                position.y += EditorGUIUtility.singleLineHeight + LineSpacing;
                if (EditorGUIUtility.currentViewWidth < 345f) position.y += EditorGUIUtility.singleLineHeight + LineSpacing;
                Rect targetRotationRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

                EditorGUIUtility.labelWidth = Utility.GetStringDimensions("Target Euler", 5f).x;
                if (easingType != EaseType.None) {
                    EditorGUI.PropertyField(startRotationRect, property.FindPropertyRelative("rotateSettings").FindPropertyRelative("startEuler"));
                    EditorGUI.PropertyField(targetRotationRect, property.FindPropertyRelative("rotateSettings").FindPropertyRelative("targetEuler"));
                } else {
                    EditorGUI.PropertyField(startRotationRect, property.FindPropertyRelative("rotateSettings").FindPropertyRelative("targetEuler"));
                }

                break;
            case UIAnimationType.WorldPosition:
                Rect startPositionRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                position.y += EditorGUIUtility.singleLineHeight + LineSpacing;
                if (EditorGUIUtility.currentViewWidth < 345f) position.y += EditorGUIUtility.singleLineHeight + LineSpacing;
                Rect targetPositionRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

                EditorGUIUtility.labelWidth = Utility.GetStringDimensions("Target Position", 5f).x;
                if (easingType != EaseType.None) {
                    EditorGUI.PropertyField(startPositionRect, property.FindPropertyRelative("worldPositionSettings").FindPropertyRelative("startPosition"));
                    EditorGUI.PropertyField(targetPositionRect, property.FindPropertyRelative("worldPositionSettings").FindPropertyRelative("targetPosition"));
                } else {
                    EditorGUI.PropertyField(startPositionRect, property.FindPropertyRelative("worldPositionSettings").FindPropertyRelative("targetPosition"));
                }

                break;
            case UIAnimationType.AnchoredPosition:
                Rect startMinRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                position.y += EditorGUIUtility.singleLineHeight + LineSpacing;
                if (EditorGUIUtility.currentViewWidth < 345f) position.y += EditorGUIUtility.singleLineHeight + LineSpacing;
                Rect targetMinRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                position.y += EditorGUIUtility.singleLineHeight + LineSpacing;
                if (EditorGUIUtility.currentViewWidth < 345f) position.y += EditorGUIUtility.singleLineHeight + LineSpacing;
                Rect startMaxRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                position.y += EditorGUIUtility.singleLineHeight + LineSpacing;
                if (EditorGUIUtility.currentViewWidth < 345f) position.y += EditorGUIUtility.singleLineHeight + LineSpacing;
                Rect targetMaxRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

                EditorGUIUtility.labelWidth = Utility.GetStringDimensions("Target Max", 5f).x;
                if (easingType != EaseType.None) {
                    EditorGUI.PropertyField(startMinRect, property.FindPropertyRelative("anchoredPositionSettings").FindPropertyRelative("startMin"));
                    EditorGUI.PropertyField(targetMinRect, property.FindPropertyRelative("anchoredPositionSettings").FindPropertyRelative("targetMin"));
                    EditorGUI.PropertyField(startMaxRect, property.FindPropertyRelative("anchoredPositionSettings").FindPropertyRelative("startMax"));
                    EditorGUI.PropertyField(targetMaxRect, property.FindPropertyRelative("anchoredPositionSettings").FindPropertyRelative("targetMax"));
                } else {
                    EditorGUI.PropertyField(startMinRect, property.FindPropertyRelative("anchoredPositionSettings").FindPropertyRelative("targetMin"));
                    EditorGUI.PropertyField(targetMinRect, property.FindPropertyRelative("anchoredPositionSettings").FindPropertyRelative("targetMax"));
                }

                break;
            case UIAnimationType.Colour:
                Rect startColourRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                position.y += EditorGUIUtility.singleLineHeight + LineSpacing;
                Rect targetColourRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

                EditorGUIUtility.labelWidth = Utility.GetStringDimensions("Target Colour", 5f).x;
                if (easingType != EaseType.None) {
                    EditorGUI.PropertyField(startColourRect, property.FindPropertyRelative("colourSettings").FindPropertyRelative("startColour"));
                    EditorGUI.PropertyField(targetColourRect, property.FindPropertyRelative("colourSettings").FindPropertyRelative("targetColour"));
                } else {
                    EditorGUI.PropertyField(startColourRect, property.FindPropertyRelative("colourSettings").FindPropertyRelative("targetColour"));
                }

                break;
            case UIAnimationType.Fade:
                Rect startFadeRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                position.y += EditorGUIUtility.singleLineHeight + LineSpacing;
                Rect targetFadeRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

                EditorGUIUtility.labelWidth = Utility.GetStringDimensions("Target Fade", 5f).x;
                if (easingType != EaseType.None) {
                    EditorGUI.PropertyField(startFadeRect, property.FindPropertyRelative("fadeSettings").FindPropertyRelative("startAlpha"));
                    EditorGUI.PropertyField(targetFadeRect, property.FindPropertyRelative("fadeSettings").FindPropertyRelative("targetAlpha"));
                } else {
                    EditorGUI.PropertyField(startFadeRect, property.FindPropertyRelative("fadeSettings").FindPropertyRelative("targetAlpha"));
                }

                break;
        }

        EditorGUIUtility.labelWidth = defaultLabelWidth;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        SerializedProperty typeProperty = property.FindPropertyRelative("type");
        SerializedProperty easeProperty = property.FindPropertyRelative("easing");
        UIAnimationType selectedAnimationType = (UIAnimationType) typeProperty.enumValueIndex;
        EaseType easeType = (EaseType) easeProperty.enumValueIndex;

        int typeLinesToAdd = selectedAnimationType switch {
            UIAnimationType.Activate => 0,
            UIAnimationType.Scale => 2,
            UIAnimationType.Rotate => 2,
            UIAnimationType.WorldPosition => 2,
            UIAnimationType.AnchoredPosition => 4,
            UIAnimationType.Colour => 2,
            UIAnimationType.Fade => 2,
            _ => 0
        };

        if (selectedAnimationType != UIAnimationType.Colour && selectedAnimationType != UIAnimationType.Fade && easeType != EaseType.None && EditorGUIUtility.currentViewWidth < 345f) {
            typeLinesToAdd *= 2;
        }

        if (easeType == EaseType.None && EditorGUIUtility.currentViewWidth >= 345f) {
            typeLinesToAdd /= 2;
        }

        if ((selectedAnimationType == UIAnimationType.Colour || selectedAnimationType == UIAnimationType.Fade) && easeType == EaseType.None && EditorGUIUtility.currentViewWidth < 345f) {
            typeLinesToAdd = 1;
        }

        return (EditorGUIUtility.singleLineHeight + LineSpacing) * (BaseLineCount + typeLinesToAdd);
    }
}