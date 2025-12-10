#if UNITY_EDITOR
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;
using System.Linq;

public class DialogueGraphView : GraphView
{
    private DialogueEditorWindow window;

    public DialogueGraphView(DialogueEditorWindow window)
    {
        this.window = window;

        // Load USS safely
        var styleSheet = Resources.Load<StyleSheet>("DialogueGraphStyle");
        if (styleSheet != null) styleSheets.Add(styleSheet);

        Insert(0, new GridBackground());

        // Safe manipulators
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        // Context menu
        this.AddManipulator(new ContextualMenuManipulator(evt =>
        {
            evt.menu.AppendAction("Create Node", action =>
            {
                if (window != null && window.currentDialogue != null)
                    CreateNodeFromUI(evt.localMousePosition);
            });
        }));
    }

    public void Populate(DialogueObject dialogue)
    {
        if (dialogue == null)
        {
            Debug.LogWarning("Populate called with null dialogue.");
            return;
        }

        // Ensure lines list exists
        if (dialogue.lines == null)
            dialogue.lines = new System.Collections.Generic.List<DialogueLine>();

        // Remove existing nodes safely
        foreach (var element in graphElements.ToList())
            RemoveElement(element);

        // Create nodes
        for (int i = 0; i < dialogue.lines.Count; i++)
        {
            var line = dialogue.lines[i];
            if (line == null)
            {
                Debug.LogWarning($"Dialogue line at index {i} is null. Skipping.");
                continue;
            }

            // Ensure choices list exists
            if (line.choices == null)
                line.choices = new System.Collections.Generic.List<DialogueChoice>();

            CreateNode(dialogue, i, new UnityEngine.Vector2(250 * i, 200));
        }
    }


    private void DeleteAllNodes()
    {
        foreach (var element in graphElements.ToList())
            RemoveElement(element);
    }

    public void CreateNodeFromUI(Vector2? pos = null)
    {
        if (window == null || window.currentDialogue == null)
        {
            Debug.LogWarning("Cannot create node: DialogueObject not loaded.");
            return;
        }

        var dialogue = window.currentDialogue;

        // Ensure lines list exists
        if (dialogue.lines == null)
            dialogue.lines = new System.Collections.Generic.List<DialogueLine>();

        // Create new line
        var newLine = new DialogueLine
        {
            speaker = "Speaker",
            textKey = "text_key"
        };

        Undo.RecordObject(dialogue, "Add Dialogue Line");
        newLine.dialogueNumber = dialogue.lines.Count;
        dialogue.lines.Add(newLine);
        EditorUtility.SetDirty(dialogue);

        // Safely get index of new line
        int newIndex = dialogue.lines.Count - 1;
        if (newIndex < 0) newIndex = 0;

        // Add node to graph
        Vector2 position = pos ?? new Vector2(200, 200);
        CreateNode(dialogue, newIndex, position);
    }

    private DialogueNode CreateNode(DialogueObject parent, int index, Vector2 pos)
    {
        var line = parent.lines[index];
        var node = new DialogueNode(line, parent, this);
        node.SetPosition(new Rect(pos, new Vector2(350, 240)));
        AddElement(node);
        return node;
    }
}
#endif
