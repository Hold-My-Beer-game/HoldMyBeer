using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

// ReSharper disable Unity.PerformanceCriticalCodeInvocation

namespace CocaCopa.Unity.EditorUtils {
    [CustomPropertyDrawer(typeof(AnimatorStateAttribute))]
    public sealed class AnimatorStateDrawer : PropertyDrawer {
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
                EditorGUI.LabelField(position, label.text, "Use [AnimatorState] only on string fields.");
                EditorGUI.EndProperty();
                return;
            }

            var attr = (AnimatorStateAttribute)attribute;
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

            List<string> stateNames = GetStateNames(animator, attr.UseFullPath);

            DrawPopup(fieldRect, property, label, stateNames);
            AnimatorDrawerUtility.DrawControllerLabel(controllerRect, animator);

            if (TryGetValidationMessage(property, out string validationMessage)) { EditorGUI.HelpBox(helpRect, validationMessage, MessageType.Warning); }

            EditorGUI.EndProperty();
        }

        private static void DrawPopup(Rect rect, SerializedProperty property, GUIContent label, List<string> stateNames) {
            string currentValue = property.stringValue;

            var options = new List<string> { "<None>" };
            options.AddRange(stateNames);

            int selectedIndex = 0;

            if (!string.IsNullOrEmpty(currentValue)) {
                int foundIndex = stateNames.IndexOf(currentValue);
                if (foundIndex >= 0) { selectedIndex = foundIndex + 1; }
            }

            int newIndex = EditorGUI.Popup(rect, label.text, selectedIndex, options.ToArray());

            property.stringValue = newIndex == 0
                ? string.Empty
                : options[newIndex];
        }

        private static List<string> GetStateNames(Animator animator, AnimatorStateAttribute attr) {
            return GetStateNames(animator, attr.UseFullPath);
        }

        private static List<string> GetStateNames(Animator animator, bool useFullPath) {
            var result = new List<string>();

            if (!animator || animator.runtimeAnimatorController == null) { return result; }

            if (animator.runtimeAnimatorController is not AnimatorController controller) { return result; }

            foreach (AnimatorControllerLayer layer in controller.layers) {
                CollectStates(
                    layer.stateMachine,
                    layer.name,
                    useFullPath,
                    result
                );
            }

            result.Sort(StringComparer.Ordinal);
            return result;
        }

        private static void CollectStates(AnimatorStateMachine stateMachine, string currentPath, bool useFullPath, List<string> result) {
            foreach (ChildAnimatorState childState in stateMachine.states) {
                string stateName = childState.state.name;

                result.Add(useFullPath
                    ? $"{currentPath}.{stateName}"
                    : stateName);
            }

            foreach (ChildAnimatorStateMachine childMachine in stateMachine.stateMachines) {
                string nextPath = $"{currentPath}.{childMachine.stateMachine.name}";

                CollectStates(
                    childMachine.stateMachine,
                    nextPath,
                    useFullPath,
                    result
                );
            }
        }

        private bool TryGetValidationMessage(SerializedProperty property, out string message) {
            var attr = (AnimatorStateAttribute)attribute;
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

            List<string> stateNames = GetStateNames(animator, attr);

            if (!stateNames.Contains(currentValue)) {
                message = $"State '{currentValue}' no longer exists on this Animator Controller.";
                return true;
            }

            message = string.Empty;
            return false;
        }
    }
}