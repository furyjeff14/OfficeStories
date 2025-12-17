#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DialogueEdgeConnector : IEdgeConnectorListener
{
    private DialogueGraphView graphView;
    private DialogueObject dialogue;

    public DialogueEdgeConnector(DialogueGraphView gv, DialogueObject dlg)
    {
        graphView = gv;
        dialogue = dlg;
    }

    public void OnDropOutsidePort(Edge edge, Vector2 position)
    {
        // Nothing to do if dropped outside
        Debug.Log("444");
    }

    public void OnDrop(GraphView gv, Edge edge)
    {
        Debug.Log("333");
        if (edge == null || edge.output == null || edge.input == null)
            return;

        var outputNode = edge.output.node as DialogueNode;
        var inputNode = edge.input.node as DialogueNode;
        if (outputNode == null || inputNode == null)
            return;

        // Add edge to graph
        gv.AddElement(edge);
        // Get choice index from port's userData
        int portIndex = (int)(edge.output.userData ?? -1);

        if (portIndex >= 0 && portIndex < outputNode.dialogueLine.choices.Count)
        {
            // Normal choice port
            outputNode.dialogueLine.choices[portIndex].nextLineIndex = inputNode.dialogueLine.dialogueNumber;
            outputNode.dialogueLine.choices[portIndex].nextDialogue = null;
        }
        else if (portIndex == -1)
        {
            // Default "Next" output port
           
            outputNode.dialogueLine.NextLineIndex = inputNode.dialogueLine.dialogueNumber;
            outputNode.dialogueLine.nextDialogue = null;
        }

#if UNITY_EDITOR
        // Auto-save the dialogue
        if (dialogue != null)
            UnityEditor.EditorUtility.SetDirty(dialogue);
#endif
    }
}
#endif
