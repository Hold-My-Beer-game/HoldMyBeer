using UnityEditor;
using UnityEngine;

namespace CocaCopa.Unity.EditorUtils {
    internal static class AnimatorDrawerUtility {
        private const float ControllerLabelIndent = 14f;

        internal static Animator ResolveAnimator(
            SerializedProperty property,
            string animatorFieldName,
            bool hasAnimatorFieldName
        ) {
            Object targetObject = property.serializedObject.targetObject;

            if (targetObject is not Component component) { return null; }

            if (hasAnimatorFieldName) {
                SerializedProperty animatorProperty = property.serializedObject.FindProperty(animatorFieldName);

                if (animatorProperty != null &&
                    animatorProperty.propertyType == SerializedPropertyType.ObjectReference) { return animatorProperty.objectReferenceValue as Animator; }
            }

            return component.GetComponentInChildren<Animator>(true);
        }

        internal static void DrawControllerLabel(Rect rect, Animator animator) {
            string text;

            if (!animator) { text = "Animator: <none>"; }
            else if (!animator.runtimeAnimatorController) { text = "Controller: <none>"; }
            else { text = $"Controller: {animator.runtimeAnimatorController.name}"; }

            rect.x += ControllerLabelIndent;
            rect.width -= ControllerLabelIndent;

            Color color = GetControllerLabelColor(animator);

            var style = new GUIStyle(EditorStyles.miniLabel) {
                alignment = TextAnchor.MiddleLeft
            };

            style.normal.textColor = color;

            EditorGUI.LabelField(rect, text, style);
        }

        private static Color GetControllerLabelColor(Animator animator) {
            if (!animator) { return new Color(1f, 0.5f, 0.5f); }

            if (!animator.runtimeAnimatorController) { return new Color(1f, 0.75f, 0.4f); }

            return new Color(0.6f, 0.9f, 1f);
        }
    }
}