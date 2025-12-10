using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public DialogueObject currentDialogue;
    public DialogueLine currentLine;

    private Stack<DialogueLine> history = new Stack<DialogueLine>();

    public void StartDialogue(DialogueObject dialogue)
    {
        currentDialogue = dialogue;

        if (dialogue.lines.Count == 0)
        {
            Debug.LogWarning("Dialogue has no lines.");
            return;
        }

        currentLine = dialogue.lines[0];

        history.Clear();
        DisplayCurrentLine();
    }

    public void ChooseOption(DialogueChoice choice)
    {
        if (currentLine != null)
            history.Push(currentLine);

        // --- SAME DIALOGUE BRANCH ---
        if (choice.nextLineIndex >= 0 &&
            choice.nextLineIndex < currentDialogue.lines.Count)
        {
            currentLine = currentDialogue.lines[choice.nextLineIndex];
            DisplayCurrentLine();
            return;
        }

        // --- SWITCH TO ANOTHER DIALOGUE ---
        if (choice.nextDialogue != null &&
            choice.nextDialogue.lines.Count > 0)
        {
            StartDialogue(choice.nextDialogue);
            return;
        }

        // --- END DIALOGUE ---
        currentLine = null;
        DisplayCurrentLine();
    }

    public void GoBack()
    {
        if (history.Count == 0)
            return;

        currentLine = history.Pop();
        DisplayCurrentLine();
    }

    private void DisplayCurrentLine()
    {
        // Update UI
        Debug.Log($"[{currentLine.speaker}] {currentLine.textKey}");
    }
}
