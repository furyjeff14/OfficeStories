using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class DialogueUtility
{
    public static DialogueObject FindDialogue(string name)
    {
        var guids = AssetDatabase.FindAssets($"{name} t:DialogueObject");
        if (guids.Length == 0) return null;
        return AssetDatabase.LoadAssetAtPath<DialogueObject>(
            AssetDatabase.GUIDToAssetPath(guids[0])
        );
    }
}
