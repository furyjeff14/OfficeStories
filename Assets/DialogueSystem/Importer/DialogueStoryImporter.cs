#if UNITY_EDITOR
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Globalization;
using UnityEditor;

public static class DialogueStoryImporter
{
    static Regex LineHeader = new(@"\[LINE (\d+)\]");
    static Regex KeyValue = new(@"^(\w+):\s*(.+)$");
    static Regex Position = new(@"\((.+),\s*(.+)\)");
    static Regex ChoiceHeader = new(@"-\s*\((.+)\)");
    static Regex LineJump = new(@"->\s*LINE\s*(\d+)");
    static Regex DialogueJump = new(@"->\s*DIALOGUE\s*(.+)");
    static Regex ConditionLine = new(@"\[IF\s+(.+?)\s*(==|!=|>=|<=|>|<)\s*(.+)\]");

    /// <summary>
    /// Import text into an existing DialogueObject.
    /// </summary>
    public static void Import(DialogueObject dialogue, string path)
    {
        if (dialogue == null || string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            Debug.LogError("Invalid DialogueObject or file path for import.");
            return;
        }

        dialogue.lines.Clear();

        DialogueLine currentLine = null;
        DialogueChoice currentChoice = null;
        bool inAssets = false;

        foreach (var raw in File.ReadAllLines(path))
        {
            var line = raw.Trim();
            if (string.IsNullOrEmpty(line)) continue;

            var lm = LineHeader.Match(line);
            if (lm.Success)
            {
                currentLine = new DialogueLine
                {
                    dialogueNumber = dialogue.lines.Count
                };
                dialogue.lines.Add(currentLine);
                currentChoice = null;
                continue;
            }

            if (currentLine == null) continue;

            if (line == "assets:")
            {
                inAssets = true;
                continue;
            }

            var kv = KeyValue.Match(line);
            if (kv.Success)
            {
                string key = kv.Groups[1].Value;
                string value = kv.Groups[2].Value;

                switch (key)
                {
                    case "speakerSlot":
                        currentLine.speakerSlot = int.Parse(value);
                        break;
                    case "noSpeaker":
                        currentLine.isDialogueNoSpeaker = bool.Parse(value);
                        break;
                    case "speaker":
                        currentLine.speaker = value;
                        break;
                    case "text":
                        currentLine.textKey = value;
                        break;
                    case "defaultNext":
                        currentLine.nextLineIndex = int.Parse(value);
                        break;
                    case "position":
                        var pm = Position.Match(value);
                        if (pm.Success)
                            currentLine.NodePosition = new Vector2(
                                float.Parse(pm.Groups[1].Value, CultureInfo.InvariantCulture),
                                float.Parse(pm.Groups[2].Value, CultureInfo.InvariantCulture)
                            );
                        break;
                }

                if (inAssets)
                {
                    if (key == "speakerImage") currentLine.speakerImg = FindSprite(value);
                    if (key == "background") currentLine.background = FindSprite(value);
                }

                continue;
            }

            var cm = ChoiceHeader.Match(line);
            if (cm.Success)
            {
                currentChoice = new DialogueChoice
                {
                    choiceTextKey = cm.Groups[1].Value
                };
                currentLine.choices.Add(currentChoice);
                continue;
            }

            if (currentChoice == null) continue;

            var lj = LineJump.Match(line);
            if (lj.Success)
            {
                currentChoice.nextLineIndex = int.Parse(lj.Groups[1].Value);
                continue;
            }

            var dj = DialogueJump.Match(line);
            if (dj.Success)
            {
                currentLine.nextDialogueFile = dj.Groups[1].Value.Trim();
                continue;
            }

            var cond = ConditionLine.Match(line);
            if (cond.Success)
            {
                currentChoice.conditions.Add(new DialogueCondition
                {
                    variableName = cond.Groups[1].Value,
                    comparison = cond.Groups[2].Value,
                    value = cond.Groups[3].Value
                });
            }
        }

        // Mark asset dirty and save
        EditorUtility.SetDirty(dialogue);
        AssetDatabase.SaveAssets();
    }

    static Sprite FindSprite(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        var guids = AssetDatabase.FindAssets($"{name} t:Sprite");
        if (guids.Length == 0) return null;
        return AssetDatabase.LoadAssetAtPath<Sprite>(
            AssetDatabase.GUIDToAssetPath(guids[0])
        );
    }
}
#endif
