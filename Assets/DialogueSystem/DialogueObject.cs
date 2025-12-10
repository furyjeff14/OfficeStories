using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.Progress;

[CreateAssetMenu(
    fileName = "NewDialogue",
    menuName = "Dialogue System/Dialogue Object"
)]
public class DialogueObject : ScriptableObject
{
    [Header("Dialogue Lines")]
    [SerializeField]
    public List<DialogueLine> lines = new List<DialogueLine>();

    [Header("Behaviour")]
    public bool autoAdvance = true;

    [Header("Events")]
    public UnityEvent onDialogueStart;
    public UnityEvent onDialogueEnd;

    private void OnValidate()
    {
   
        for (int i  = 0; i < lines.Count; i++)
        {
            lines[i].dialogueNumber = i;
            int choicesCount = lines[i].choices.Count;
            if (choicesCount > 0)
            {
                for (int j = 0; j < choicesCount; j++)
                {
                    if (j < choicesCount)
                    {
                        lines[i].choices[j].nextLineIndex = i + 1;
                    } else
                    {
                        lines[i].choices[j].nextLineIndex = -1;
                    }
                }
            }
        }

    }
}
