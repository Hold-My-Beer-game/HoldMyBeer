using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace CocaCopa.StateMachine.Editor {
    public partial class AISightStimulusEditor {
        private void DrawScriptField() {
            MonoScript script = MonoScript.FromMonoBehaviour((MonoBehaviour)target);
            using (new EditorGUI.DisabledScope(true)) { EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false); }
        }

        private static void DrawSubGroup(string title, System.Action content) {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField(title, EditorStyles.miniBoldLabel);

            EditorGUI.indentLevel++;
            content.Invoke();
            EditorGUI.indentLevel--;

            EditorGUILayout.EndVertical();
        }

        private static void DrawSightRangeSlider(string label, SerializedProperty minProperty, SerializedProperty maxProperty) {
            float min = -minProperty.floatValue;
            float max = maxProperty.floatValue;
            MinMaxSlider(new GUIContent(label), ref min, ref max, -180, 180);
            min = Mathf.Min(min, 0f);
            minProperty.floatValue = Mathf.Abs(min);
            maxProperty.floatValue = Mathf.Max(max, 0f);
        }

        private static void MinMaxSlider(GUIContent guiContent, ref float minValue, ref float maxValue, int rangeMin, int rangeMax) {
            Rect rowRect = EditorGUILayout.GetControlRect();

            Rect contentRect = EditorGUI.PrefixLabel(rowRect, guiContent);

            const float fieldWidth = 50f;
            const float spacing = 4f;

            var minFieldRect = new Rect(contentRect.x, contentRect.y, fieldWidth, contentRect.height);
            var maxFieldRect = new Rect(contentRect.xMax - fieldWidth, contentRect.y, fieldWidth, contentRect.height);
            var sliderRect = new Rect(minFieldRect.xMax + spacing, contentRect.y, contentRect.width - fieldWidth * 2 - spacing * 2, contentRect.height);

            int prevIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            float newMin = EditorGUI.FloatField(minFieldRect, minValue);
            float newMax = EditorGUI.FloatField(maxFieldRect, maxValue);

            newMin = Mathf.Clamp(newMin, rangeMin, rangeMax);
            newMax = Mathf.Clamp(newMax, rangeMin, rangeMax);

            if (newMin > newMax) { newMin = newMax; }

            minValue = newMin;
            maxValue = newMax;

            EditorGUI.MinMaxSlider(sliderRect, ref minValue, ref maxValue, rangeMin, rangeMax);

            minValue = Mathf.Clamp(minValue, rangeMin, rangeMax);
            maxValue = Mathf.Clamp(maxValue, rangeMin, rangeMax);

            if (minValue > maxValue) { minValue = maxValue; }

            EditorGUI.indentLevel = prevIndent;
        }
    }
}
#endif