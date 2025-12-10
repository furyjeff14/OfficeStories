using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueRunner : MonoBehaviour
{
    public static DialogueRunner Instance { get; private set; }

    [SerializeField]
    private Stack<DialogueLine> history = new Stack<DialogueLine>();

    [Header("UI References")]
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogueText;
    public Transform choicesContainer;
    public Button choiceButtonPrefab;
    public Button nextButton;
    public Button previousButton;

    [Header("Dialogue Settings")]
    public DialogueObject startingDialogue;

    private DialogueLine currentLine;
    [SerializeField] private DialogueObject currentDialogue;

    [SerializeField]
    private Image dialogueBackground;
    [SerializeField]
    private Image dialogueCharacter1;

    private int nextDialogueLine = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            GameObject.Destroy(gameObject);
        }
    }

    void Start()
    {
        if (startingDialogue != null)
            StartDialogue(startingDialogue);

        nextButton.onClick.AddListener(NextLine);
        previousButton.onClick.AddListener(ShowPreviousLine);
    }

    public Stack<DialogueLine> GetHistory()
    {
        return history;
    }

    private void NextLine()
    {
        if (nextDialogueLine < currentDialogue.lines.Count)
        {
            history.Push(currentLine);
            ShowLine(currentDialogue.lines[nextDialogueLine]);
        } else
        {
            EndDialogue();
        }
    }

    private void ShowPreviousLine()
    {

        if (history.Count == 0)
            return;

        currentLine = history.Pop();
        ShowLine(currentLine);
    }

    public void StartDialogue(DialogueObject dialogue)
    {
        history.Clear();
        currentDialogue = dialogue;
        if (dialogue.lines.Count > 0)
            ShowLine(dialogue.lines[0]);
        else
            EndDialogue();
    }

    void ShowLine(DialogueLine line)
    {
        currentLine = line;

        dialogueBackground.sprite = line.background;
        speakerText.text = line.speaker;
        dialogueText.text = line.textKey;
        dialogueCharacter1.sprite = line.speakerImg;

        foreach (Transform child in choicesContainer)
            Destroy(child.gameObject);

        if (line.choices.Count == 0)
        {
            nextButton.gameObject.SetActive(true);

            nextDialogueLine += 1;
            // No choices → proceed to next line automatically if set
            /* if (line.choices.Count == 0 && currentLine != null)
             {
                 Invoke(nameof(EndDialogue), 2f); // optional auto-end delay
             }*/
        }
        else
        {
            nextButton.gameObject.SetActive(false);
            foreach (var choice in line.choices)
            {
                if (!CheckConditions(choice)) continue;

                var btn = Instantiate(choiceButtonPrefab, choicesContainer);
                btn.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceTextKey;
                btn.onClick.AddListener(() => OnChoiceSelected(choice));
            }
        }
    }

    public void OnChoiceSelected(DialogueChoice choice)
    {
        // Go to a line inside the same dialogue
        if (choice.nextLineIndex >= 0 &&
            choice.nextLineIndex < currentDialogue.lines.Count)
        {
            history.Push(currentLine);
            nextDialogueLine = choice.nextLineIndex;
            ShowLine(currentDialogue.lines[choice.nextLineIndex]);
            return;
        }
        // Go to another DialogueObject
        if (choice.nextDialogue != null && choice.nextDialogue.lines.Count > 0)
        {
            nextDialogueLine = 0;
            StartDialogue(choice.nextDialogue);
            return;
        }

        EndDialogue();
    }

    bool CheckConditions(DialogueChoice choice)
    {
        foreach (var cond in choice.conditions)
        {
            // Basic string comparison example
            string value = PlayerPrefs.GetString(cond.variableName, "");
            switch (cond.comparison)
            {
                case "==": if (value != cond.value) return false; break;
                case "!=": if (value == cond.value) return false; break;
                default: break;
            }
        }
        return true;
    }

    void EndDialogue()
    {
        speakerText.text = "";
        dialogueText.text = "";
        foreach (Transform child in choicesContainer)
            Destroy(child.gameObject);
        currentLine = null;
        currentDialogue = null;
    }
}
