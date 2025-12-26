using System.Collections.Generic;
using UnityEngine;

public class CSVCustomReader : MonoBehaviour
{
    public TextAsset csvFile; // Drag your CSV file here in the Inspector

    void Start()
    {
        if (csvFile != null)
        {
            ParseCSV(csvFile.text);
        }
        else
        {
            Debug.LogError("CSV file not assigned!");
        }
    }

    void ParseCSV(string csvText)
    {
        string[] lines = csvText.Split('\n');
        List<DialogueCSVModel> dialogues = new List<DialogueCSVModel>();
        for(int i = 0; i < lines.Length; i++) 
        {
            if(i == 0)
            {
                continue;
            }
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');
            // Process your values here, e.g., store them in a list of custom objects
            //Debug.Log("Row: " + string.Join(", ", values));

            if (values.Length < 5) {
                Debug.LogError("Values are not enough for id : " + values[0]);
                continue;
            }
            string _id = values[0].Trim();
            string _characterName = values[1].Trim();
            int _characterSprite = 0;
            int.TryParse(values[2].Trim(), out _characterSprite);
            int _phraseId = 0;
            int.TryParse(values[3].Trim(), out _phraseId);
            string _phrase = values[4].Trim();

            DialogueCSVModel parsedDialogue = new DialogueCSVModel
            {
                id = _id,
                characterName = _characterName,
                characterSprite = _characterSprite,
                phraseId = _phraseId,
                phrase = _phrase,
            };
            dialogues.Add(parsedDialogue);
        }


        DialogueController.Instance.SetupDialogue(dialogues);
    }
}