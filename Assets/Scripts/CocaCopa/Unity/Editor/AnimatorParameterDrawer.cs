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

            bool shouldShowHelpBox = TryGetValidationMessage(property, out _);
            if (!shouldShowHelpBox) { return EditorGUIUtility.singleLineHeight; }

            float helpBoxHeight = EditorGUIUtility.singleLineHeight * 2f;
            return EditorGUIUtility.singleLineHeight + HelpBoxSpacing + helpBoxHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            if (property.propertyType != SerializedPropertyType.String) {
                EditorGUI.LabelField(position, label.text, "Use [AnimatorParameter] only on string fields.");
                EditorGUI.EndProperty();
                return;
            }

            var fieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            var helpRect = new Rect(position.x, fieldRect.yMax + HelpBoxSpacing, position.width, position.height - EditorGUIUtility.singleLineHeight - HelpBoxSpacing);

            Animator animator = ResolveAnimator(property, (AnimatorParameterAttribute)attribute);
            List<string> parameterNames = GetFilteredParameterNames(animator, (AnimatorParameterAttribute)attribute);

            DrawPopup(fieldRect, property, label, parameterNames);

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

            if (newIndex == 0) {
                property.stringValue = string.Empty;
                return;
            }

            property.stringValue = options[newIndex];
        }

        private static Animator ResolveAnimator(SerializedProperty property, AnimatorParameterAttribute attr) {
            UnityEngine.Object targetObject = property.serializedObject.targetObject;

            if (targetObject is not Component component) { return null; }

            if (attr.HasAnimatorFieldName) {
                SerializedProperty animatorProperty = property.serializedObject.FindProperty(attr.AnimatorFieldName);

                if (animatorProperty != null && animatorProperty.propertyType == SerializedPropertyType.ObjectReference) { return animatorProperty.objectReferenceValue as Animator; }
            }

            return component.GetComponent<Animator>();
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
            Animator animator = ResolveAnimator(property, attr);

            if (!animator) {
                message = attr.HasAnimatorFieldName
                    ? $"Could not find Animator from field '{attr.AnimatorFieldName}'."
                    : "Could not find Animator on the same GameObject.";
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