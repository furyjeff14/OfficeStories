using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;            // UI text for the dialogue
    public Image portraitImage;          // UI image for the character portrait
    public Button nextButton;            // Button to advance dialogue
    public GameObject dialoguePanel;     // Panel holding the dialogue UI elements

    private int currentLineIndex = 0;    // Index of the current dialogue line
    private Dialogue currentDialogue;    // The current dialogue being shown

    // Start is called before the first frame update
    void Start()
    {
        nextButton.onClick.AddListener(ShowNextLine);
        dialoguePanel.SetActive(false);  // Hide the panel initially
    }

    // Method to start dialogue
    public void StartDialogue(Dialogue dialogue)
    {
        currentDialogue = dialogue;
        currentLineIndex = 0;
        dialoguePanel.SetActive(true);
        ShowCurrentLine();
    }

    // Show the current line of dialogue
    private void ShowCurrentLine()
    {
        if (currentLineIndex < currentDialogue.lines.Length)
        {
            dialogueText.text = currentDialogue.lines[currentLineIndex];
            portraitImage.sprite = currentDialogue.characterPortrait;
        }
        else
        {
            EndDialogue();
        }
    }

    // Show next line of dialogue
    private void ShowNextLine()
    {
        currentLineIndex++;
        ShowCurrentLine();
    }

    // End the dialogue
    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);  // Hide the dialogue panel
        // You can trigger additional events here like starting a new scene, giving the player choices, etc.
    }
}
