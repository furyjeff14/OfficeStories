using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance;

    public Dialogue dialogue;      // Reference to the Dialogue ScriptableObject
    public DialogueManager dialogueManager;  // Reference to the Dialogue Manager
    public List<DialogueCSVModel> dialogues = new List<DialogueCSVModel>();

    public DialogueObject dialogueToPlay;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        } else
        {
            GameObject.Destroy(gameObject);
        }
        
    }

    private void Start()
    {
        DialogueRunner.Instance.StartDialogue(dialogueToPlay);
    }

    // Call this method when the player interacts with the NPC
    public void SetupDialogue(List<DialogueCSVModel> dialogues)
    {
        List<Dialogue> toShow = new List<Dialogue>();
        foreach (DialogueCSVModel csv in dialogues)
        {
            toShow.Add(new Dialogue(csv));
        }
       // dialogueManager.SetupDialogue(toShow);
    }

    public void StartDialogue(string index)
    {
       // dialogueManager.ShowDialogue();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            Debug.Log("Start dialogue");
            StartDialogue("0");
        }

    }

}
