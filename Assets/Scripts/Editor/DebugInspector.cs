using UnityEditor;
 
[CustomEditor(typeof(GradientScript))]
public class DebugInspector : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        // SerializedObject serializedGradient = new SerializedObject(target);
        // SerializedProperty colorGradient = serializedGradient.FindProperty("gradient");
        // EditorGUILayout.PropertyField(colorGradient, true, null);
        // if (EditorGUI.EndChangeCheck())
        //     serializedGradient.ApplyModifiedProperties();

        SerializedObject serializedGradients = new SerializedObject(target);
        SerializedProperty colorGradients = serializedGradients.FindProperty("gradients");
        EditorGUILayout.PropertyField(colorGradients, true, null);
        if (EditorGUI.EndChangeCheck())
            serializedGradients.ApplyModifiedProperties();
    }
}