using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueChoice
{
    public string choiceTextKey;

    [Header("Branching")]
    public int nextLineIndex = -1;      // -1 = no next line in this DialogueObject
    public string nextLineId;           // optional, prefer GUID approach for robustness
    public DialogueObject nextDialogue; // still allowed (safe)

    [Header("Conditions (optional)")]
    public List<DialogueCondition> conditions = new List<DialogueCondition>();

    public void Initialize(int _nextLineIndex)
    {
        nextLineIndex = _nextLineIndex;
    }
}
