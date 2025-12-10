#if UNITY_EDITOR
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;

public class DialogueNode : Node
{
    public DialogueLine dialogueLine;
    private DialogueObject parentDialogue;
    private DialogueGraphView graphView;

    public DialogueNode(DialogueLine line, DialogueObject parent, DialogueGraphView graphView)
    {
        this.dialogueLine = line;
        this.parentDialogue = parent;
        this.graphView = graphView;

        title = line.speaker;
        RefreshExpandedState();
        RefreshPorts();

        CreateInputPort();
        CreateChoicePorts();
    }

    private void CreateInputPort()
    {
        var inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
        inputPort.portName = "Input";
        inputContainer.Add(inputPort);
    }

    private void CreateChoicePorts()
    {
        if (dialogueLine == null) return;

        // Ensure choices list exists
        if (dialogueLine.choices == null)
            dialogueLine.choices = new System.Collections.Generic.List<DialogueChoice>();

        foreach (var choice in dialogueLine.choices)
        {
            string choiceName = string.IsNullOrEmpty(choice.choiceTextKey) ? "Choice" : choice.choiceTextKey;

            var outputPort = InstantiatePort(
                Orientation.Horizontal,  // horizontal layout
                Direction.Output,        // output port
                Port.Capacity.Single,    // single connection per port
                typeof(float)            // type used for connections (can be float)
            );

            outputPort.portName = choiceName;

            // Add port to the output container of the node
            outputContainer.Add(outputPort);
        }

        // Refresh node to show the ports
        RefreshPorts();
        RefreshExpandedState();
    }
}
#endif
