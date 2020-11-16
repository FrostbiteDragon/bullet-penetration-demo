using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace ProjectileAsset
{
    [CustomEditor(typeof(ProjectileController), true)]
    public class ProjectileControllerEditor : Editor
    {
        SerializedProperty speed;
        SerializedProperty gravityMultiplier;
        SerializedProperty layerMask;

        SerializedProperty penetrationEnabled;
        SerializedProperty penetration;

        SerializedProperty ricochetEnabled;
        SerializedProperty ricochetAngle;

        public void OnEnable()
        {
            speed = serializedObject.FindProperty("_speed");
            gravityMultiplier = serializedObject.FindProperty("_gravityMultiplier");
            layerMask = serializedObject.FindProperty("_layerMask");

            penetrationEnabled = serializedObject.FindProperty("_penetrationEnabled");
            penetration = serializedObject.FindProperty("_penetration");

            ricochetEnabled = serializedObject.FindProperty("_ricochetEnabled");
            ricochetAngle = serializedObject.FindProperty("_ricochetAngle");
        }

        public override void OnInspectorGUI()
        {
           

            serializedObject.Update();

            EditorGUILayout.PropertyField(speed);
            EditorGUILayout.PropertyField(gravityMultiplier);
            EditorGUILayout.PropertyField(layerMask);

            //penetration
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Penetration", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold}, GUILayout.MinWidth(120));
            EditorGUILayout.PropertyField(penetrationEnabled, GUIContent.none, GUILayout.Width(Screen.width * 0.55f));
            EditorGUILayout.EndHorizontal();

            if (penetrationEnabled.boolValue)
            {
                EditorGUILayout.BeginHorizontal(new GUIStyle(GUI.skin.box) { padding = new RectOffset(10, 0, 0, 0) });
                EditorGUILayout.LabelField("Penetration", new GUIStyle(GUI.skin.label), GUILayout.Width(Mathf.Max(Screen.width * 0.45f - 50, 110)));
                EditorGUILayout.PropertyField(penetration, GUIContent.none);
                EditorGUILayout.LabelField("mm", GUILayout.Width(22));
                EditorGUILayout.EndHorizontal();
            }

            //ricochet
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Ricochet", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold }, GUILayout.MinWidth(120));
            EditorGUILayout.PropertyField(ricochetEnabled, GUIContent.none, GUILayout.Width(Screen.width * 0.55f));
            EditorGUILayout.EndHorizontal();

            if (ricochetEnabled.boolValue)
            {
                EditorGUILayout.BeginHorizontal(new GUIStyle(GUI.skin.box) { padding = new RectOffset(10, 0, 0, 0) });
                EditorGUILayout.LabelField("Ricochet Angle", new GUIStyle(GUI.skin.label), GUILayout.Width(Mathf.Max(Screen.width * 0.45f - 50, 110)));
                EditorGUILayout.PropertyField(ricochetAngle, GUIContent.none);
                EditorGUILayout.LabelField("°", GUILayout.Width(22));
                EditorGUILayout.EndHorizontal();
            }
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            DrawDefaultInspector();

        }
    }
}