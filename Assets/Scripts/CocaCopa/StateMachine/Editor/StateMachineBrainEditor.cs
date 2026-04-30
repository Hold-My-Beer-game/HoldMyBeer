#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace CocaCopa.StateMachine.Editor {
    [CustomEditor(typeof(StateMachineBrain))]
    public class StateMachineBrainEditor : UnityEditor.Editor {
        private StateMachineBrain brain;

        private void OnEnable() {
            brain = (StateMachineBrain)target;
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            DrawScriptField();
            DrawActiveStates();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawActiveStates() {
            string activeMovementState = EditorApplication.isPlaying ? brain.CurrentMovementStateID : "None";
            string activeCombatState = EditorApplication.isPlaying ? brain.CurrentCombatStateID : "None";

            using (new EditorGUI.DisabledGroupScope(true)) {
                EditorGUILayout.LabelField("Active States", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                {
                    EditorGUILayout.TextField("Movement", activeMovementState);
                    EditorGUILayout.TextField("Combat", activeCombatState);
                }
                EditorGUI.indentLevel--;
            }
        }

        private void DrawScriptField() {
            MonoScript script = MonoScript.FromMonoBehaviour((MonoBehaviour)target);
            using (new EditorGUI.DisabledScope(true)) { EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false); }
        }
    }
}
#endif