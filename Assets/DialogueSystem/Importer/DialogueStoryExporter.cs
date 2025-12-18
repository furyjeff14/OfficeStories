#if UNITY_EDITOR
using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;

public static class DialogueStoryExporter
{
    public static void Export(DialogueObject dialogue, string path)
    {
        if (dialogue == null || string.IsNullOrEmpty(path))
        {
            Debug.LogError("Invalid DialogueObject or path for export.");
            return;
        }

        var sb = new StringBuilder();

        sb.AppendLine($"# DIALOGUE: {dialogue.name}");
        sb.AppendLine($"# AutoAdvance: {dialogue.autoAdvance}");
        sb.AppendLine();

        foreach (var line in dialogue.lines)
        {
            if (line == null) continue;

            sb.AppendLine($"[LINE {line.dialogueNumber}]");
            sb.AppendLine($"speakerSlot: {line.speakerSlot}");
            sb.AppendLine($"noSpeaker: {line.isDialogueNoSpeaker}");
            sb.AppendLine($"speaker: {line.speaker}");
            sb.AppendLine($"text: {line.textKey}");
            sb.AppendLine();

            sb.AppendLine("assets:");
            sb.AppendLine($"  speakerImage: {(line.speakerImg ? line.speakerImg.name : "")}");
            sb.AppendLine($"  background: {(line.background ? line.background.name : "")}");
            sb.AppendLine();

            sb.AppendLine($"defaultNext: {line.nextLineIndex}");
            sb.AppendLine($"position: ({line.NodePosition.x}, {line.NodePosition.y})");
            sb.AppendLine();

            // If line jumps to another dialogue file
            if (!string.IsNullOrEmpty(line.nextDialogueFile))
            {
                sb.AppendLine($"-> DIALOGUE {line.nextDialogueFile}");
            }
            // Otherwise export choices
            else if (line.choices != null && line.choices.Count > 0)
            {
                sb.AppendLine("CHOICES:");
                foreach (var choice in line.choices)
                {
                    if (choice == null) continue;

                    sb.AppendLine($"- ({choice.choiceTextKey})");

                    if (!string.IsNullOrEmpty(choice.nextDialogueFile))
                        sb.AppendLine($"    -> DIALOGUE {choice.nextDialogueFile}");
                    else if (choice.nextLineIndex >= 0)
                        sb.AppendLine($"    -> LINE {choice.nextLineIndex}");
                    else
                        sb.AppendLine($"    -> END");

                    if (choice.conditions != null)
                    {
                        foreach (var cond in choice.conditions)
                        {
                            sb.AppendLine(
                                $"    [IF {cond.variableName} {cond.comparison} {cond.value}]"
                            );
                        }
                    }

                    sb.AppendLine();
                }
            }
            else
            {
                sb.AppendLine("-> END");
            }

            sb.AppendLine();
        }

        // Ensure directory exists
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllText(path, sb.ToString());
        AssetDatabase.Refresh();

        Debug.Log($"Dialogue exported to: {path}");
    }
}
#endif
