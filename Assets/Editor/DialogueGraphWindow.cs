#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class DialogueGraphWindow : EditorWindow
{
    DialogueGraphView graphView = null;
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
        Selection.selectionChanged += OnSelectionChanged;
        if (Selection.activeObject is DialogueObject asset)
        {
            OnSelectionChanged(); // load immediately if already selected
        } else
        {
            ConstructGraphView();
            GenerateToolbar();
        }
    }

    private void OnDisable()
    {
       // if (graphView != null && rootVisualElement.Contains(graphView)) 
            //rootVisualElement.Remove(graphView);

        Selection.selectionChanged -= OnSelectionChanged;
    }

    private void OnSelectionChanged()
    {
        if (Selection.activeObject is DialogueObject asset)
        {
            currentDialogue = asset;

            if (graphView == null)
            {
                ConstructGraphView();
                GenerateToolbar();
            }

            graphView.LoadDialogue(currentDialogue);
        }
    }

    private void ConstructGraphView()
    {
        if (graphView != null)
            return;

        graphView = new DialogueGraphView(this, currentDialogue);
        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);
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

        // Add new import button
        var importBtn = new Button(() =>
        {
            ImportStoryText();
        })
        { text = "Import Dialogue Text" };
        toolbar.Add(importBtn);


        rootVisualElement.Add(toolbar);
    }

    void ImportStoryText()
    {
        string txtPath = EditorUtility.OpenFilePanel("Import Story Script", Application.dataPath, "txt");
        if (string.IsNullOrEmpty(txtPath)) return;

        string defaultName = Path.GetFileNameWithoutExtension(txtPath);

        string assetPath = EditorUtility.SaveFilePanelInProject("Create Dialogue Asset", defaultName, "asset", "Choose location");
        if (string.IsNullOrEmpty(assetPath)) return;

        // Create DialogueObject asset
        DialogueObject dialogue = ScriptableObject.CreateInstance<DialogueObject>();
        AssetDatabase.CreateAsset(dialogue, assetPath);
        AssetDatabase.SaveAssets();

        // Import lines/choices into DialogueObject
        DialogueStoryImporter.Import(dialogue, txtPath);

        // Ensure GraphView exists
        if (graphView == null)
        {
            ConstructGraphView();
            GenerateToolbar();
        }

        // Load the imported dialogue AFTER GraphView is ready
        EditorApplication.delayCall += () =>
        {
            currentDialogue = dialogue;

            // Select and ping in Project
            Selection.activeObject = dialogue;
            EditorGUIUtility.PingObject(dialogue);

            // Safely populate GraphView
            if (graphView != null && graphView.parent != null)
            {
                graphView.LoadDialogue(dialogue);
            }
            else
            {
                // If parent not ready, schedule another delay
                EditorApplication.delayCall += () =>
                {
                    graphView?.LoadDialogue(dialogue);
                };
            }
        };
    }

    void RebuildGraphFromDialogue()
    {
        graphView.LoadDialogue(currentDialogue);
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
