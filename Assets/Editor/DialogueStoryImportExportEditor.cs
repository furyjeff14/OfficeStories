#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DialogueObject))]
public class DialogueStoryImportExportEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);
        GUILayout.Label("Storyteller Export", EditorStyles.boldLabel);

        if (GUILayout.Button("Export Story Script (.txt)"))
        {
            var dialogue = (DialogueObject)target;

            string path = EditorUtility.SaveFilePanel(
                "Export Story Script",
                Application.dataPath,
                dialogue.name + "_Story.txt",
                "txt"
            );

            if (!string.IsNullOrEmpty(path))
            {
                DialogueStoryExporter.Export(dialogue, path);
            }
        }
    }
}
#endif
