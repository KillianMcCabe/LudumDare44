using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    struct Dialogue
    {
        public string Title;
        public string Message;
        public string OptionText;

        public Dialogue(string title, string message, string optionText)
        {
            Title = title;
            Message = message;
            OptionText = optionText;
        }
    }

    [SerializeField] Text _titleText = null;

    [SerializeField] Text _dialogueText = null;

    [SerializeField] Button _nextButton = null;

    int _dialogueIndex = 0;

    Dialogue[] greedDialogue =
    {
        new Dialogue("???", "Who is it that dares call on me?", "..."),
        new Dialogue("???", "A mortal? Do you have a deathwish?", "..."),
        new Dialogue("???", "You will submit to me or die. Make your choice mortal.", "Please spare me!"),
        new Dialogue("???", "As expected. From this moment on you will do my bidding.", "..."),
        new Dialogue("Greed", "You will serve in the name of Greed and gather gold and artifacts to fill my hoard.", "I will do as you wish."),
    };

    // Start is called before the first frame update
    void Start()
    {
        _nextButton.onClick.AddListener(HandleNextButtonClicked);
        LoadText(0);
        GameSetupController.LocalPlayer.acceptingInput = false;
    }

    void HandleNextButtonClicked()
    {
        LoadText(_dialogueIndex + 1);
    }

    private void LoadText(int index)
    {
        _dialogueIndex = index;

        if (_dialogueIndex < greedDialogue.Length)
        {
            _titleText.text = greedDialogue[_dialogueIndex].Title;
            _dialogueText.text = greedDialogue[_dialogueIndex].Message;
            _nextButton.GetComponentInChildren<Text>().text = greedDialogue[_dialogueIndex].OptionText;
        }
        else
        {
            gameObject.SetActive(false);
            GameSetupController.LocalPlayer.acceptingInput = true;
        }
    }
}
