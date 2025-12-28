using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Interactable
{
    public DialogueObject npcDialogue;

    public override void Interact(GameObject interactor)
    {
        DialogueRunner.Instance.StartDialogue(npcDialogue);
    }
}
