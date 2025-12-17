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

    public DialogueObject GetCurrentDialogue() => window.GetCurrentDialogue();

    private bool isLoadingGraph;

    public DialogueGraphView(DialogueGraphWindow wnd, DialogueObject dlg)
    {
        window = wnd;

        // Grid background
        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        // Manipulators
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        //Add edge connector for dragging edges
        //var edgeConnector = new EdgeConnector<Edge>(new DialogueEdgeConnector(this, dialogue));
        //this.AddManipulator(edgeConnector);

        // Event to handle edges creation
        graphViewChanged += OnGraphViewChanged;
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange changes)
    {
        // Handle created edges
        if (changes.edgesToCreate != null)
        {
            foreach (var edge in changes.edgesToCreate)
            {
                ConnectEdge(edge);
            }
        }

       
        if (changes.elementsToRemove != null)
        {
            foreach (var element in changes.elementsToRemove)
            {
                // Handle removed edges
                if (element is Edge edge)
                {
                    DisconnectEdge(edge);
                }
                else if (element is DialogueNode node && changes.elementsToRemove.Count == 1)
                {
                    GetCurrentDialogue().lines.RemoveAt(node.dialogueLine.dialogueNumber);
                }
            }
        }


        return changes;
    }

    private void ConnectEdge(Edge edge)
    {
        if (edge == null || edge.output == null || edge.input == null) return;

        var outputNode = edge.output.node as DialogueNode;
        var inputNode = edge.input.node as DialogueNode;
        if (outputNode == null || inputNode == null) return;

        int portIndex = outputNode.outputPorts.IndexOf(edge.output);

        if (outputNode.dialogueLine.choices.Count > 0 && portIndex >= 0)
        {
            // This output corresponds to a choice
            Debug.Log("Choices: " + portIndex);
            outputNode.dialogueLine.choices[portIndex].nextLineIndex = inputNode.dialogueLine.dialogueNumber;
            outputNode.dialogueLine.choices[portIndex].nextDialogue = null;
        }
        else
        {
            // Default non-choice output
            
            outputNode.dialogueLine.NextLineIndex = inputNode.dialogueLine.dialogueNumber;
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
        if (isLoadingGraph)
            return;
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
            outputNode.dialogueLine.NextLineIndex = -1;
        }

#if UNITY_EDITOR
        var dlg = GetCurrentDialogue();
        if (dlg != null) EditorUtility.SetDirty(dlg);
#endif
    }

    public void LoadDialogue(DialogueObject dialogue)
    {
        if (dialogue == null) return;

        isLoadingGraph = true;

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
                int targetIndex = -1;
                Edge edge = null;
                if (node.dialogueLine.choices.Count > 0)
                {
                    var choice = i < node.dialogueLine.choices.Count ? node.dialogueLine.choices[i] : null;

                    targetIndex = choice != null ? choice.nextLineIndex : node.dialogueLine.NextLineIndexDefault;

                    if (targetIndex >= 0 && nodesByNumber.TryGetValue(targetIndex, out var targetNode))
                    {
                        edge = node.outputPorts[i].ConnectTo(targetNode.inputPort);
                    }
                } else if(node.dialogueLine.NextLineIndex != -1 && nodesByNumber.TryGetValue(node.dialogueLine.NextLineIndex, out var targetNode))
                {
                    edge = node.outputPorts[i].ConnectTo(targetNode.inputPort);
                }
                if(edge != null)
                {
                    AddElement(edge);
                }
            }
        }
        isLoadingGraph = false;
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
