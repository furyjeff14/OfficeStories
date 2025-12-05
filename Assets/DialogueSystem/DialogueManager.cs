using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;            // UI text for the dialogue
    public Image portraitImage;          // UI image for the character portrait
    public Button nextButton;            // Button to advance dialogue
    public GameObject dialoguePanel;     // Panel holding the dialogue UI elements

    private int currentLineIndex = 0;    // Index of the current dialogue line
    List<Dialogue> dialogueScenario = new List<Dialogue>();
    private Dialogue currentDialogue;    // The current dialogue being shown

    // Start is called before the first frame update
    void Start()
    {

        nextButton.onClick.AddListener(ShowNextLine);
        dialoguePanel.SetActive(false);  // Hide the panel initially
    }

    // Method to start dialogue
    public void SetupDialogue(List<Dialogue> dialogue)
    {
        currentLineIndex = 0;
        dialogueScenario = dialogue;
        currentDialogue = dialogue[currentLineIndex];
    }

    public void ShowDialogue()
    {
        dialogueText.text = currentDialogue.phrase;
        Sprite characterSprite = CharacterSpriteManager.Instance.GetSprite(currentDialogue.characterSprite);
        portraitImage.sprite = characterSprite;
        dialoguePanel.SetActive(true);
    }

    // Show next line of dialogue
    private void ShowNextLine()
    {
        currentLineIndex++;
        currentDialogue = dialogueScenario[currentLineIndex];
        dialogueText.text = currentDialogue.phrase;
        Sprite characterSprite = CharacterSpriteManager.Instance.GetSprite(currentDialogue.characterSprite);
        portraitImage.sprite = characterSprite;

        if(currentLineIndex >= dialogueScenario.Count - 1)
        {
            EndDialogue();
        }
    }

    // End the dialogue
    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);  // Hide the dialogue panel
        portraitImage.sprite = null;
        // You can trigger additional events here like starting a new scene, giving the player choices, etc.
    }
}
