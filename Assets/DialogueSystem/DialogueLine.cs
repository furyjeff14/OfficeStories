using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[Serializable]
public class DialogueLine
{
    [ReadOnly]
    public int dialogueNumber = 0;
    public string speaker;
    public string textKey;

    [SerializeField] public Sprite speakerImg;
    [SerializeField] public Sprite background;

    [SerializeField]
    public List<DialogueChoice> choices = new List<DialogueChoice>();
}
