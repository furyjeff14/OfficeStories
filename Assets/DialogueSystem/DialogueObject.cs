using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

        // Ensure dialogueNumber and sensible nextLineIndex defaults
        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i] == null) continue;
            lines[i].dialogueNumber = i;

            // If choices exist, set default nextLineIndex only when it is currently -1
            for (int j = 0; j < lines[i].choices.Count; j++)
            {
                if (lines[i].choices[j] == null) continue;

                // Default behaviour: point to next line (i + 1) if in range, else -1.
                int defaultNext = (i + 1 < lines.Count) ? i + 1 : -1;

                // Only override if nextLineIndex is -1 (i.e. not manually set).
                if (lines[i].choices[j].nextLineIndex == -1)
                    lines[i].choices[j].nextLineIndex = defaultNext;
            }
        }

    }
}
