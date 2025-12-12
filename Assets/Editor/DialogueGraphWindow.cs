#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraphWindow : EditorWindow
{
    DialogueGraphView graphView;
    DialogueObject currentDialogue;

    [MenuItem("Window/Dialogue/Node Dialogue Editor")]
    public static void OpenWindow()
    {
        var wnd = GetWindow<DialogueGraphWindow>();
        wnd.titleContent = new GUIContent("Dialogue Graph");
        wnd.minSize = new Vector2(900, 500);
    }

    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolbar();
    }

    private void OnDisable()
    {
        if (graphView != null)
            rootVisualElement.Remove(graphView);
    }

    private void ConstructGraphView()
    {
        if (graphView == null)
        {
            graphView = new DialogueGraphView(this, currentDialogue);
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);
        }

        // If a DialogueObject is already selected, load it
        if (currentDialogue != null)
            graphView.LoadDialogue(currentDialogue);
    }

    private void GenerateToolbar()
    {
        var toolbar = new Toolbar();

        // DialogueObject field
        var objField = new ObjectField("Dialogue Object")
        {
            objectType = typeof(DialogueObject),
            allowSceneObjects = false
        };
        objField.RegisterValueChangedCallback(evt =>
        {
            currentDialogue = evt.newValue as DialogueObject;

            // Make sure GraphView exists
            if (graphView == null)
            {
                graphView = new DialogueGraphView(this, currentDialogue);
                graphView.StretchToParentSize();
                rootVisualElement.Add(graphView);
            }

            // Load the new DialogueObject
            graphView.LoadDialogue(currentDialogue);
        });
        toolbar.Add(objField);

        // Add new line button
        var addLineBtn = new Button(() =>
        {
            if (currentDialogue != null)
            {
                var line = new DialogueLine();
                currentDialogue.lines.Add(line);
                graphView.AddDialogueNode(line, currentDialogue.lines.Count - 1);
            }
        })
        { text = "Add Line" };
        toolbar.Add(addLineBtn);

        rootVisualElement.Add(toolbar);
    }

#if UNITY_EDITOR
    public void OnInspectorChanged()
    {
        foreach (var node in graphView.nodes.ToList().Cast<DialogueNode>())
        {
            node.RefreshChoicePorts();
        }
    }
#endif

    public DialogueObject GetCurrentDialogue() => currentDialogue;
}
#endif
