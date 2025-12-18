using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[Serializable]
public class DialogueLine
{
    [ReadOnly]
    [Tooltip("We use this as reference to jump from different lines or a different dialogue")]
    public int dialogueNumber = 0;

    [Tooltip("This will be used as character number if we want to show multiple ones on the same dialogue - 0 shown on left - 1 shown on right and if it is -1 then we remove the shown character")]
    public int speakerSlot = 0;
    [Tooltip("This will be used for events such as other stuff happening to the world like *bump* *crashes* *falls*")]
    public bool isDialogueNoSpeaker = false;

    [Tooltip("Speaker name, will also change the speaker text name ui")]
    public string speaker;
    [Tooltip("Text shows on dialogue")]
    public string textKey;

    [SerializeField] public Sprite speakerImg;
    [SerializeField] public Sprite background;

    [SerializeField]
    public List<DialogueChoice> choices = new List<DialogueChoice>();

    [SerializeField]
    private Vector2 nodePosition;

    //Used for graph view
    [HideInInspector]
    public int nextLineIndex = -1;

    public int NextLineIndexDefault = -1;

    public event Action OnChangedNextLineIndex;
    public int NextLineIndex
    {
        get => nextLineIndex;
        set
        {
            if (nextLineIndex == value) return;
            nextLineIndex = value;
            OnChangedNextLineIndex?.Invoke();
        }
    }

    [Tooltip("Used for loading a new dialogue object after the current line, it is mandatory to leave this empty if we won't load a new file")]
    public string nextDialogueFile;

    public Vector2 NodePosition
    {
        get => nodePosition;
        set => nodePosition = value;
    }
}
