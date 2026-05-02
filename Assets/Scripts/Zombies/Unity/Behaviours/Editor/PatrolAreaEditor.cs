#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace HoldMyBeer.Zombies.Unity.Editor {
    [InitializeOnLoad]
    public static class PatrolAreaEditor {
        static PatrolAreaEditor() {
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private static void OnSceneGUI(SceneView sceneView) {
            PatrolArea[] patrolAreas = Object.FindObjectsByType<PatrolArea>(
                FindObjectsInactive.Exclude,
                FindObjectsSortMode.None
            );

            foreach (PatrolArea patrolArea in patrolAreas) {
                if (patrolArea == null || !patrolArea.DrawGizmos) { continue; }

                DrawPatrolAreaBox(patrolArea);
                DrawPatrolPointButtons(patrolArea);
            }
        }

        private static void DrawPatrolAreaBox(PatrolArea patrolArea) {
            Transform t = patrolArea.transform;
            if (t.childCount == 0) { return; }

            float minX = float.MaxValue;
            float minZ = float.MaxValue;
            float maxX = float.MinValue;
            float maxZ = float.MinValue;

            for (int i = 0; i < t.childCount; i++) {
                Vector3 pos = t.GetChild(i).position;

                if (pos.x < minX) { minX = pos.x; }
                if (pos.z < minZ) { minZ = pos.z; }

                if (pos.x > maxX) { maxX = pos.x; }
                if (pos.z > maxZ) { maxZ = pos.z; }
            }

            float y = t.position.y;
            const float height = 2f;

            Vector3 p0 = new(minX, y, minZ);
            Vector3 p1 = new(maxX, y, minZ);
            Vector3 p2 = new(maxX, y, maxZ);
            Vector3 p3 = new(minX, y, maxZ);

            Vector3 p0Top = p0 + Vector3.up * height;
            Vector3 p1Top = p1 + Vector3.up * height;
            Vector3 p2Top = p2 + Vector3.up * height;
            Vector3 p3Top = p3 + Vector3.up * height;

            Color fill = patrolArea.AreaColor;

            Color outline = patrolArea.AreaColor;
            outline.a = 1f;

            Handles.DrawSolidRectangleWithOutline(new[] { p0, p1, p2, p3 }, fill, Color.clear);
            Handles.DrawSolidRectangleWithOutline(new[] { p0Top, p1Top, p2Top, p3Top }, fill, Color.clear);

            Handles.DrawSolidRectangleWithOutline(new[] { p0, p1, p1Top, p0Top }, fill, Color.clear);
            Handles.DrawSolidRectangleWithOutline(new[] { p1, p2, p2Top, p1Top }, fill, Color.clear);
            Handles.DrawSolidRectangleWithOutline(new[] { p2, p3, p3Top, p2Top }, fill, Color.clear);
            Handles.DrawSolidRectangleWithOutline(new[] { p3, p0, p0Top, p3Top }, fill, Color.clear);

            Color previousColor = Handles.color;
            Handles.color = outline;

            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p1, p2);
            Handles.DrawLine(p2, p3);
            Handles.DrawLine(p3, p0);

            Handles.DrawLine(p0Top, p1Top);
            Handles.DrawLine(p1Top, p2Top);
            Handles.DrawLine(p2Top, p3Top);
            Handles.DrawLine(p3Top, p0Top);

            Handles.DrawLine(p0, p0Top);
            Handles.DrawLine(p1, p1Top);
            Handles.DrawLine(p2, p2Top);
            Handles.DrawLine(p3, p3Top);

            Handles.color = previousColor;
        }

        private static void DrawPatrolPointButtons(PatrolArea patrolArea) {
            Transform areaTransform = patrolArea.transform;

            Color previousColor = Handles.color;
            Handles.color = patrolArea.PointColor;

            for (int i = 0; i < areaTransform.childCount; i++) {
                Transform child = areaTransform.GetChild(i);
                float size = patrolArea.PointRadius;

                if (!Handles.Button(child.position, Quaternion.identity, size, size, Handles.SphereHandleCap)) { continue; }

                Selection.activeGameObject = child.gameObject;
                EditorGUIUtility.PingObject(child.gameObject);
                SceneView.RepaintAll();
            }

            Handles.color = previousColor;
        }
    }
}
#endif