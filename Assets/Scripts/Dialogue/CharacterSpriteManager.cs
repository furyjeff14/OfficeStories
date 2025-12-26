using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpriteManager : MonoBehaviour
{
    public static CharacterSpriteManager Instance;
    [SerializeField] private Sprite[] characters;

    private void Awake()
    {
        Instance = this;
    }

    public Sprite GetSprite(int index)
    {
        return characters[index];
    }
}
