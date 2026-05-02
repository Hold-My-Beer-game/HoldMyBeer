using System;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace HoldMyBeer.Zombies.Unity {
    [CustomEditor(typeof(AIHealth))]
    internal sealed class AIHealthEditor : Editor {
        private SerializedProperty maxHealth;
        private SerializedProperty currentHealth;

        private void OnEnable() {
            FindProperties();
        }

        private void FindProperties() {
            maxHealth = serializedObject.FindProperty(nameof(maxHealth));
            currentHealth = serializedObject.FindProperty(nameof(currentHealth));
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            DrawScriptField();
            DrawHealth();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawScriptField() {
            MonoScript script = MonoScript.FromMonoBehaviour((MonoBehaviour)target);
            using (new EditorGUI.DisabledScope(true)) { EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false); }
        }

        private void DrawHealth() {
            EditorGUILayout.PropertyField(maxHealth);
            using (new EditorGUI.DisabledGroupScope(true)) { EditorGUILayout.PropertyField(currentHealth); }
        }
    }
}

#endif