#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEditor;

public class DialogueGraphView : GraphView
{
    private DialogueGraphWindow window;
    private DialogueObject dialogue;

    public DialogueObject GetCurrentDialogue() => window.GetCurrentDialogue();
    private HashSet<Edge> existingEdges = new HashSet<Edge>();

    public DialogueGraphView(DialogueGraphWindow wnd, DialogueObject dlg)
    {
        window = wnd;
        dialogue = dlg;

        // Grid background
        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        // Manipulators
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        // Add edge connector for dragging edges
        var edgeConnector = new EdgeConnector<Edge>(new DialogueEdgeConnector(this, dialogue));
        this.AddManipulator(edgeConnector);

        // Event to handle edges creation
        graphViewChanged += OnGraphViewChanged;
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange changes)
    {
        // Handle new edges
        if (changes.edgesToCreate != null)
        {
            foreach (var edge in changes.edgesToCreate)
            {
                ConnectEdge(edge);
                existingEdges.Add(edge); // track new edge
            }
        }

        // Detect deleted edges
        var currentEdges = new HashSet<Edge>(edges.ToList().Cast<Edge>());
        foreach (var oldEdge in existingEdges)
        {
            if (!currentEdges.Contains(oldEdge))
            {
                DisconnectEdge(oldEdge);
            }
        }
        existingEdges = currentEdges;

        return changes;
    }

    private void ConnectEdge(Edge edge)
    {
        if (edge == null || edge.output == null || edge.input == null) return;

        var outputNode = edge.output.node as DialogueNode;
        var inputNode = edge.input.node as DialogueNode;
        if (outputNode == null || inputNode == null) return;

        int portIndex = (int)(edge.output.userData ?? -1);

        if (outputNode.dialogueLine.choices.Count > 0 && portIndex >= 0)
        {
            // This output corresponds to a choice
            outputNode.dialogueLine.choices[portIndex].nextLineIndex = inputNode.dialogueLine.dialogueNumber;
            outputNode.dialogueLine.choices[portIndex].nextDialogue = null;
        }
        else
        {
            // Default non-choice output
            outputNode.dialogueLine.nextLineIndex = inputNode.dialogueLine.dialogueNumber;
            outputNode.dialogueLine.nextDialogue = null;
        }

#if UNITY_EDITOR
        var dlg = outputNode.graphView.GetCurrentDialogue();
        if (dlg != null)
            UnityEditor.EditorUtility.SetDirty(dlg);
#endif
    }

    private void DisconnectEdge(Edge edge)
    {
        if (edge == null || edge.output == null) return;

        var outputNode = edge.output.node as DialogueNode;
        if (outputNode == null) return;

        int portIndex = outputNode.outputPorts.IndexOf(edge.output);
        if (portIndex < 0) return;

        if (portIndex < outputNode.dialogueLine.choices.Count)
        {
            outputNode.dialogueLine.choices[portIndex].nextLineIndex = -1;
            outputNode.dialogueLine.choices[portIndex].nextDialogue = null;
        }
        else
        {
            outputNode.dialogueLine.nextLineIndex = -1;
        }

#if UNITY_EDITOR
        var dlg = GetCurrentDialogue();
        if (dlg != null) EditorUtility.SetDirty(dlg);
#endif
    }

    public void LoadDialogue(DialogueObject dialogue)
    {
        if (dialogue == null) return;

        DeleteElements(graphElements.ToList());

        Dictionary<int, DialogueNode> nodesByNumber = new Dictionary<int, DialogueNode>();

        // Create nodes
        foreach (var line in dialogue.lines)
        {
            var node = new DialogueNode(line, this);
            AddElement(node);

            // Restore saved node position
            if (line.NodePosition != Vector2.zero)
                node.SetPosition(new Rect(line.NodePosition, node.GetPosition().size));

            nodesByNumber.Add(line.dialogueNumber, node);
        }

        // Reconnect edges based on nextLineIndex
        foreach (var node in nodesByNumber.Values)
        {
            for (int i = 0; i < node.outputPorts.Count; i++)
            {
                var choice = i < node.dialogueLine.choices.Count ? node.dialogueLine.choices[i] : null;
                int targetIndex = choice != null ? choice.nextLineIndex : node.dialogueLine.NextLineIndexDefault;

                if (targetIndex >= 0 && nodesByNumber.TryGetValue(targetIndex, out var targetNode))
                {
                    var edge = node.outputPorts[i].ConnectTo(targetNode.inputPort);
                    AddElement(edge);
                }
            }
        }
    }

    public void AddDialogueNode(DialogueLine line)
    {
        var node = new DialogueNode(line, this);
        AddElement(node);
    }

    public void AddDialogueNode(DialogueLine line, int nextLine)
    {
        line.dialogueNumber = nextLine;
        var node = new DialogueNode(line, this);
        AddElement(node);
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();

        foreach (var port in ports)
        {
            // Exclude the same port
            if (port == startPort) continue;

            // Only allow connections between ports of the same type
            if (port.portType == startPort.portType &&
                port.direction != startPort.direction)
            {
                compatiblePorts.Add(port);
            }
        }

        return compatiblePorts;
    }
}
#endif
