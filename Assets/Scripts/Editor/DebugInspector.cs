using UnityEditor;
 
[CustomEditor(typeof(GradientScript))]
public class DebugInspector : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        SerializedObject serializedGradient = new SerializedObject(target);
        SerializedProperty colorGradient = serializedGradient.FindProperty("gradient");
        EditorGUILayout.PropertyField(colorGradient, true, null);
        if (EditorGUI.EndChangeCheck())
            serializedGradient.ApplyModifiedProperties();
    }
}