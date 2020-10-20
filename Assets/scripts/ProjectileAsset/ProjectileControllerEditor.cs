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
        SerializedProperty penetrationEnabled;
        SerializedProperty penetration;

        public void OnEnable()
        {
            speed = serializedObject.FindProperty("_speed");
            gravityMultiplier = serializedObject.FindProperty("_gravityMultiplier");
            penetrationEnabled = serializedObject.FindProperty("_penetrationEnabled");
            penetration = serializedObject.FindProperty("_penetration");

        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();

            serializedObject.Update();

            EditorGUILayout.PropertyField(speed);
            EditorGUILayout.PropertyField(gravityMultiplier);
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Penetration", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold}, GUILayout.MinWidth(120));
            EditorGUILayout.PropertyField(penetrationEnabled, GUIContent.none, GUILayout.Width(Screen.width / 1.815f));
            EditorGUILayout.EndHorizontal();

            if (penetrationEnabled.boolValue)
            {
                EditorGUILayout.BeginHorizontal(new GUIStyle(GUI.skin.box) { padding = new RectOffset(10, 0, 0, 0) });
                EditorGUILayout.PropertyField(penetration);
                EditorGUILayout.LabelField("mm", GUILayout.Width(22));
                EditorGUILayout.EndHorizontal();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}