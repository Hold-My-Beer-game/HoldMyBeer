#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace CocaCopa.StateMachine.Editor {
    public partial class AISightStimulusEditor : UnityEditor.Editor {
        private SerializedProperty originOffset;
        private SerializedProperty sightDistance;
        private SerializedProperty leftAngle;
        private SerializedProperty rightAngle;
        private SerializedProperty upAngle;
        private SerializedProperty downAngle;
        private SerializedProperty visionDepth;
        private SerializedProperty visionPointsCount;
        private SerializedProperty visionPointSize;
        private SerializedProperty sightMask;
        private SerializedProperty triggerInteraction;

        private void OnEnable() {
            FindProperties();
        }

        private void FindProperties() {
            originOffset = serializedObject.FindProperty(nameof(originOffset));
            sightDistance = serializedObject.FindProperty(nameof(sightDistance));
            leftAngle = serializedObject.FindProperty(nameof(leftAngle));
            rightAngle = serializedObject.FindProperty(nameof(rightAngle));
            upAngle = serializedObject.FindProperty(nameof(upAngle));
            downAngle = serializedObject.FindProperty(nameof(downAngle));
            visionDepth = serializedObject.FindProperty(nameof(visionDepth));
            visionPointsCount = serializedObject.FindProperty(nameof(visionPointsCount));
            visionPointSize = serializedObject.FindProperty(nameof(visionPointSize));
            sightMask = serializedObject.FindProperty(nameof(sightMask));
            triggerInteraction = serializedObject.FindProperty(nameof(triggerInteraction));
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            DrawScriptField();
            DrawSightConeProperties();
            DrawVisionProperties();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawSightConeProperties() {
            EditorGUILayout.LabelField("Sight Cone", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(sightDistance, new GUIContent("Max Distance"));
            DrawSightRangeSlider("Horizontal Angles", leftAngle, rightAngle);
            DrawSightRangeSlider("Vertical Angles", downAngle, upAngle);
            EditorGUILayout.Space(10f);
        }

        private void DrawVisionProperties() {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.Space(2f);
            EditorGUILayout.LabelField("Vision", EditorStyles.boldLabel);

            EditorGUILayout.Space(4f);

            DrawSubGroup("Detection", () => {
                EditorGUILayout.PropertyField(originOffset, new GUIContent("Origin Offset"));
                EditorGUILayout.PropertyField(sightMask, new GUIContent("Layer Mask"));
                EditorGUILayout.PropertyField(triggerInteraction);
            });

            EditorGUILayout.Space(6f);

            DrawSubGroup("Sampling", () => {
                EditorGUILayout.PropertyField(visionDepth, new GUIContent("Depth"));
                EditorGUILayout.PropertyField(visionPointsCount, new GUIContent("Grid"));
                EditorGUILayout.PropertyField(visionPointSize, new GUIContent("Size (%)"));
            });

            EditorGUILayout.EndVertical();
        }
    }
}
#endif