using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue System/Variable Database")]
public class DialogueVariableDatabase : ScriptableObject
{
    public Dictionary<string, bool> boolVars = new();
    public Dictionary<string, float> floatVars = new();
    public Dictionary<string, int> intVars = new();
    public Dictionary<string, string> stringVars = new();

    public bool GetBool(string key) => boolVars.ContainsKey(key) && boolVars[key];
    public void SetBool(string key, bool value) => boolVars[key] = value;

    public float GetFloat(string key) => floatVars.ContainsKey(key) ? floatVars[key] : 0;
    public void SetFloat(string key, float value) => floatVars[key] = value;

    public int GetInt(string key) => intVars.ContainsKey(key) ? intVars[key] : 0;
    public void SetInt(string key, int value) => intVars[key] = value;

    public string GetString(string key) => stringVars.ContainsKey(key) ? stringVars[key] : "";
    public void SetString(string key, string value) => stringVars[key] = value;
}
