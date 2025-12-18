using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueChoice
{
    public string choiceTextKey;

    [Header("Branching")]
    [Tooltip("This is the next dialogue line id to be called when we choose this answer")]
    public int nextLineIndex = -1;      // -1 = no next line in this DialogueObject
    [Tooltip("We use this if we want to jump to a different dialogue")]
    public string nextDialogueFile; // still allowed (safe)

    [Header("Conditions (optional)")]
    public List<DialogueCondition> conditions = new List<DialogueCondition>();

    public void Initialize(int _nextLineIndex)
    {
        nextLineIndex = _nextLineIndex;
    }
}
