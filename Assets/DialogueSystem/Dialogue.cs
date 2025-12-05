using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue 
{
    public string id;
    public string characterName;
    public int characterSprite;
    public int phraseId;
    public string phrase;

    public int? response = 0;

    public Dialogue()
    {

    }

    public Dialogue(DialogueCSVModel model)
    {
        this.id = model.id; 
        this.characterName = model.characterName;
        this.characterSprite = model.characterSprite;
        this.phraseId = model.phraseId;
        this.phrase = model.phrase; 
    }

}