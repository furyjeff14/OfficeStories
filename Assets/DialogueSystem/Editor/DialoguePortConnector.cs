#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;

public class DialoguePortConnector : EdgeConnector<Edge>
{
    public DialoguePortConnector(IEdgeConnectorListener listener) : base(listener)
    {
    }
}
#endif
