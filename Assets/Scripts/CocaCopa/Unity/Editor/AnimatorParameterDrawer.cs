using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// ReSharper disable Unity.PerformanceCriticalCodeInvocation

namespace CocaCopa.Unity.EditorUtils {
    [CustomPropertyDrawer(typeof(AnimatorParameterAttribute))]
    public sealed class AnimatorParameterDrawer : PropertyDrawer {
        private const float HelpBoxSpacing = 2f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            if (property.propertyType != SerializedPropertyType.String) { return EditorGUIUtility.singleLineHeight; }

            float height = EditorGUIUtility.singleLineHeight * 2f;

            if (TryGetValidationMessage(property, out _)) { height += HelpBoxSpacing + EditorGUIUtility.singleLineHeight * 2f; }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            if (property.propertyType != SerializedPropertyType.String) {
                EditorGUI.LabelField(position, label.text, "Use [AnimatorParameter] only on string fields.");
                EditorGUI.EndProperty();
                return;
            }

            var attr = (AnimatorParameterAttribute)attribute;
            Animator animator = AnimatorDrawerUtility.ResolveAnimator(property, attr.AnimatorFieldName, attr.HasAnimatorFieldName);

            var fieldRect = new Rect(
                position.x,
                position.y,
                position.width,
                EditorGUIUtility.singleLineHeight
            );

            var controllerRect = new Rect(
                position.x,
                fieldRect.yMax,
                position.width,
                EditorGUIUtility.singleLineHeight
            );

            var helpRect = new Rect(
                position.x,
                controllerRect.yMax + HelpBoxSpacing,
                position.width,
                position.height - EditorGUIUtility.singleLineHeight * 2f - HelpBoxSpacing
            );

            List<string> parameterNames = GetFilteredParameterNames(animator, attr);

            DrawPopup(fieldRect, property, label, parameterNames);
            AnimatorDrawerUtility.DrawControllerLabel(controllerRect, animator);

            if (TryGetValidationMessage(property, out string validationMessage)) { EditorGUI.HelpBox(helpRect, validationMessage, MessageType.Warning); }

            EditorGUI.EndProperty();
        }

        private static void DrawPopup(Rect rect, SerializedProperty property, GUIContent label, List<string> parameterNames) {
            string currentValue = property.stringValue;

            var options = new List<string> { "<None>" };
            options.AddRange(parameterNames);

            int selectedIndex = 0;

            if (!string.IsNullOrEmpty(currentValue)) {
                int foundIndex = parameterNames.IndexOf(currentValue);
                if (foundIndex >= 0) { selectedIndex = foundIndex + 1; }
            }

            int newIndex = EditorGUI.Popup(rect, label.text, selectedIndex, options.ToArray());

            property.stringValue = newIndex == 0
                ? string.Empty
                : options[newIndex];
        }

        private static List<string> GetFilteredParameterNames(Animator animator, AnimatorParameterAttribute attr) {
            var result = new List<string>();

            if (!animator) { return result; }

            AnimatorControllerParameter[] parameters = animator.parameters;
            if (parameters == null || parameters.Length == 0) { return result; }

            for (int i = 0; i < parameters.Length; i++) {
                AnimatorControllerParameter parameter = parameters[i];

                if (attr.FilterByType && parameter.type != attr.ParameterType) { continue; }

                result.Add(parameter.name);
            }

            result.Sort(StringComparer.Ordinal);
            return result;
        }

        private bool TryGetValidationMessage(SerializedProperty property, out string message) {
            var attr = (AnimatorParameterAttribute)attribute;
            Animator animator = AnimatorDrawerUtility.ResolveAnimator(property, attr.AnimatorFieldName, attr.HasAnimatorFieldName);

            if (!animator) {
                message = attr.HasAnimatorFieldName
                    ? $"Could not find Animator from field '{attr.AnimatorFieldName}'."
                    : "Could not find Animator on this GameObject or its children.";
                return true;
            }

            if (animator.runtimeAnimatorController == null) {
                message = "Animator has no Runtime Animator Controller assigned.";
                return true;
            }

            string currentValue = property.stringValue;

            if (string.IsNullOrWhiteSpace(currentValue)) {
                message = string.Empty;
                return false;
            }

            AnimatorControllerParameter[] parameters = animator.parameters;

            for (int i = 0; i < parameters.Length; i++) {
                AnimatorControllerParameter parameter = parameters[i];

                if (!string.Equals(parameter.name, currentValue, StringComparison.Ordinal)) { continue; }

                if (attr.FilterByType && parameter.type != attr.ParameterType) {
                    message = $"Parameter '{currentValue}' exists, but it is not of type {attr.ParameterType}.";
                    return true;
                }

                message = string.Empty;
                return false;
            }

            message = $"Parameter '{currentValue}' no longer exists on this Animator.";
            return true;
        }
    }
}