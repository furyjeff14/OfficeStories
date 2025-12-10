#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

public class DialogueEditorWindow : EditorWindow
{
    public DialogueGraphView graphView;
    public DialogueObject currentDialogue;

    [MenuItem("Window/Dialogue System/Dialogue Editor")]
    public static void Open() => GetWindow<DialogueEditorWindow>("Dialogue Editor");

    private void OnEnable()
    {
        // 1️⃣ Initialize GraphView
        if (graphView == null)
        {
            graphView = new DialogueGraphView(this);
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);
        }

        // 2️⃣ Toolbar
        ConstructToolbar();

        // 3️⃣ Subscribe to selection changes
        Selection.selectionChanged += OnSelectionChanged;

        // 4️⃣ Auto-load currently selected DialogueObject
        TryLoadSelectedDialogue();
    }

    private void OnDisable()
    {
        Selection.selectionChanged -= OnSelectionChanged;

        if (graphView != null)
        {
            rootVisualElement.Remove(graphView);
            graphView = null;
        }
    }

    private void OnSelectionChanged()
    {
        TryLoadSelectedDialogue();
    }

    private void TryLoadSelectedDialogue()
    {
        if (Selection.activeObject is DialogueObject dialogue)
        {
            if (currentDialogue != dialogue)
            {
                LoadDialogue(dialogue);
            }
        }
    }

    private void ConstructToolbar()
    {
        var toolbar = new VisualElement();
        toolbar.style.flexDirection = FlexDirection.Row;
        toolbar.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
        toolbar.style.paddingLeft = 4;
        toolbar.style.paddingTop = 4;
        toolbar.style.paddingBottom = 4;

        // Load Dialogue button (manual reload still works)
        var loadBtn = new Button(() =>
        {
            if (Selection.activeObject is DialogueObject dialogue)
                LoadDialogue(dialogue);
        })
        { text = "Load Dialogue" };

        // Create Node button
        var createNodeBtn = new Button(() =>
        {
            if (graphView != null && currentDialogue != null)
                graphView.CreateNodeFromUI();
        })
        { text = "Create Node" };

        toolbar.Add(loadBtn);
        toolbar.Add(createNodeBtn);

        rootVisualElement.Insert(0, toolbar);
    }

    public void LoadDialogue(DialogueObject dialogue)
    {
        if (dialogue == null) return;

        currentDialogue = dialogue;
        titleContent.text = $"Dialogue Editor — {dialogue.name}";

        if (graphView != null)
            graphView.Populate(dialogue);
    }
}
#endif
