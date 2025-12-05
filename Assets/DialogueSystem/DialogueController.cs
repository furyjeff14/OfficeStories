using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public Dialogue dialogue;      // Reference to the Dialogue ScriptableObject
    public DialogueManager dialogueManager;  // Reference to the Dialogue Manager

    // Call this method when the player interacts with the NPC
    public void TriggerDialogue()
    {
        dialogueManager.StartDialogue(dialogue);
    }
}