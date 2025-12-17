using System.IO;
using System.Text;
using UnityEngine;

public static class DialogueExporter
{
    public static void Export(DialogueObject dialogue, string path)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"# DIALOGUE: {dialogue.name}");
        sb.AppendLine($"# AutoAdvance: {dialogue.autoAdvance}");
        sb.AppendLine();

        foreach (var line in dialogue.lines)
        {
            if (line == null) continue;

            sb.AppendLine($"[LINE {line.dialogueNumber}] {line.textKey}");
            sb.AppendLine("--------------------------------");

            sb.AppendLine($"> NPC: \"{line.textKey}\"");
            sb.AppendLine();

            if (line.choices.Count == 0)
            {
                sb.AppendLine("-> END");
                sb.AppendLine();
                continue;
            }

            sb.AppendLine("CHOICES:");

            foreach (var choice in line.choices)
            {
                if (choice == null) continue;

                sb.AppendLine($"- ({choice.choiceTextKey})");

                // Destination
                if (choice.nextDialogue != null)
                    sb.AppendLine($"    -> DIALOGUE {choice.nextDialogue.name}");
                else if (choice.nextLineIndex >= 0)
                    sb.AppendLine($"    -> LINE {choice.nextLineIndex}");
                else
                    sb.AppendLine($"    -> END");

                // Conditions
                foreach (var cond in choice.conditions)
                {
                    sb.AppendLine(
                        $"    [IF {cond.variableName} {cond.comparison} {cond.value}]"
                    );
                }

                sb.AppendLine();
            }

            sb.AppendLine();
        }

        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllText(path, sb.ToString());

        Debug.Log($"Story script exported to:\n{path}");
    }
}