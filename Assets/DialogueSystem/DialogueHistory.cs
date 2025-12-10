using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueHistory : MonoBehaviour
{
    public static DialogueHistory Instance;
    [SerializeField] Transform dialogueHistoryContainer;
    [SerializeField] TextMeshProUGUI historyText;
    [SerializeField] Button showHistory;
    [SerializeField] Button hideHistory;


    private void Awake()
    {
        if (DialogueHistory.Instance == null)
        {
            Instance = this;
        } else
        {
            GameObject.Destroy(gameObject);
        }
    }

    private void Start()
    {
        hideHistory.onClick.AddListener(HideHistory);
        showHistory.onClick.AddListener(ShowHistory);
    }

    public void ShowHistory()
    {
        Stack<DialogueLine> lines = DialogueRunner.Instance.GetHistory();
        dialogueHistoryContainer.gameObject.SetActive(true);
        DialogueLine[] dialogueLines = lines.ToArray();
        for (int i = 0; i < lines.Count; i++)
        {
            historyText.text = historyText.text + dialogueLines[i].textKey + "\n\n";
        }
    }

    public void HideHistory()
    {
        dialogueHistoryContainer.gameObject.SetActive(false);
        historyText.text = "";
    }
}
