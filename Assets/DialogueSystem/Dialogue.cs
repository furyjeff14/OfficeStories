using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue")]
public class Dialogue : ScriptableObject
{
    public string characterName;    // Character name
    public Sprite characterPortrait; // Character portrait
    [TextArea] public string[] lines; // Array of dialogue lines
}