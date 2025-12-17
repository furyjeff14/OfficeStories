using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEditor;

public class DialoguePort { }

public class DialogueNode : Node
{
    public DialogueLine dialogueLine;
    public DialogueGraphView graphView;
    public List<Port> outputPorts = new List<Port>();
    public Port inputPort;

    public DialogueNode(DialogueLine line, DialogueGraphView view)
    {
        dialogueLine = line;
        graphView = view;

        //title = string.IsNullOrEmpty(line.textKey) ? "New Line" : line.textKey;
        title = "Dialogue " + (line.dialogueNumber);

        // SetPosition(new Rect(100, 100, 350, 200));

        // Input port: allow multiple incoming connections
        inputPort = InstantiatePort(
            Orientation.Horizontal,
            Direction.Input,
            Port.Capacity.Multi, // important!
            typeof(DialoguePort)
        );
        inputPort.RegisterCallback<MouseDownEvent>(evt => evt.StopImmediatePropagation());
        inputPort.portName = "Input \n("+ inputPort.portType.ToString() + ")";
        inputContainer.Add(inputPort);

        // Output ports for each choice
        if (line.choices.Count > 0)
        {
            for (int i = 0; i < line.choices.Count; i++)
            {
                var port = CreateChoicePort(i, line.choices[i]);
                var choice = line.choices[i];
                string choiceName = string.IsNullOrEmpty(choice.choiceTextKey) ? "Choice" : choice.choiceTextKey;
                port.portName = string.IsNullOrEmpty(choice.choiceTextKey) ? choiceName + " \n(" + port.portType.ToString() + ")" : choice.choiceTextKey;
                outputContainer.Add(port);
                outputPorts.Add(port);
            }
        } else
        {
            var port = CreateChoicePort(0, new DialogueChoice());
            port.portName = "Next \n("+ port.portType.ToString() + ")";
            outputContainer.Add(port);
            outputPorts.Add(port);
        }

        // --- DialogueLine fields ---
        var dialogueNumberField = new IntegerField("Dialogue Number") { value = line.dialogueNumber };
        dialogueNumberField.SetEnabled(false); // read-only
        mainContainer.Add(dialogueNumberField);

        var nextLineIndexField = new IntegerField("Next Line Index") { value = line.NextLineIndex };
        nextLineIndexField.RegisterValueChangedCallback(evt => {line.NextLineIndex = evt.newValue; });
        mainContainer.Add(nextLineIndexField);

        line.OnChangedNextLineIndex += () =>
        {
            nextLineIndexField.SetValueWithoutNotify(line.NextLineIndex);
        };

        var speakerSlotField = new IntegerField("Speaker Slot") { value = line.speakerSlot };
        speakerSlotField.RegisterValueChangedCallback(evt => line.speakerSlot = evt.newValue);
        mainContainer.Add(speakerSlotField);

        var noSpeakerField = new Toggle("No Speaker") { value = line.isDialogueNoSpeaker };
        noSpeakerField.RegisterValueChangedCallback(evt => line.isDialogueNoSpeaker = evt.newValue);
        mainContainer.Add(noSpeakerField);

        var speakerField = new TextField("Speaker") { value = line.speaker };
        speakerField.RegisterValueChangedCallback(evt =>
        {
            line.speaker = evt.newValue;
            title = line.speaker; // optional: update node title
        });
        mainContainer.Add(speakerField);

        var textField = new TextField("Text") { value = line.textKey, multiline = true };
        textField.RegisterValueChangedCallback(evt => line.textKey = evt.newValue);
        mainContainer.Add(textField);

        var speakerImgField = new ObjectField("Speaker Image")
        {
            objectType = typeof(Sprite),
            value = line.speakerImg
        };
        speakerImgField.RegisterValueChangedCallback(evt => line.speakerImg = evt.newValue as Sprite);
        mainContainer.Add(speakerImgField);

        var backgroundField = new ObjectField("Background")
        {
            objectType = typeof(Sprite),
            value = line.background
        };
        backgroundField.RegisterValueChangedCallback(evt => line.background = evt.newValue as Sprite);
        mainContainer.Add(backgroundField);


        // Button to add a new choice
        var addChoiceBtn = new UnityEngine.UIElements.Button(() =>
        {
            var newChoice = new DialogueChoice { choiceTextKey = "New Choice", nextLineIndex = -1 };
            if(dialogueLine.choices.Count == 0)
            {
                outputContainer.Clear();
            }
            dialogueLine.choices.Add(newChoice);
            AddChoicePort(newChoice);
            RefreshPorts();
            RefreshExpandedState();
        })
        { text = "Add Choice" };
        extensionContainer.Add(addChoiceBtn);

        RefreshExpandedState();
        RefreshPorts();
    }

    private Port CreateChoicePort(int index, DialogueChoice choice)
    {
        var port = InstantiatePort(
            Orientation.Horizontal,
            Direction.Output,
            Port.Capacity.Single,
            typeof(DialoguePort)
        );

        port.RegisterCallback<MouseDownEvent>(evt => evt.StopImmediatePropagation());
        port.userData = index;
        port.portName = string.IsNullOrEmpty(choice.choiceTextKey)
            ? "Choice " + index
            : choice.choiceTextKey;

        return port;
    }

    public void AddChoicePort(DialogueChoice choice)
    {
        int index = outputPorts.Count;

        var port = InstantiatePort(
            Orientation.Horizontal,
            Direction.Output,
            Port.Capacity.Single,
            typeof(DialoguePort)
        );

        port.userData = index;
        port.portName = choice.choiceTextKey;

        outputContainer.Add(port);
        outputPorts.Add(port);
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        dialogueLine.NodePosition = newPos.position;

#if UNITY_EDITOR
        var dialogue = graphView.GetCurrentDialogue();
        if (dialogue != null)
            UnityEditor.EditorUtility.SetDirty(dialogue);
#endif
    }

    public void RefreshChoicePorts()
    {
        // Loop through all output ports
        for (int i = 0; i < dialogueLine.choices.Count; i++)
        {
            if (i >= outputPorts.Count) break;
            var choice = dialogueLine.choices[i];
            outputPorts[i].portName = string.IsNullOrEmpty(choice.choiceTextKey) ? "Choice" : choice.choiceTextKey;
        }

        RefreshPorts();
        RefreshExpandedState();
    }

#if UNITY_EDITOR
    public void OnInspectorChanged()
    {
        RefreshChoicePorts();
    }
#endif
}
