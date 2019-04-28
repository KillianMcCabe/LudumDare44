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

    [SerializeField] Text _titleText;

    [SerializeField] Text _dialogueText;

    [SerializeField] Button _nextButton;

    int dialogueIndex = 0;

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
        Player.Instance.acceptingInput = false;
    }

    void HandleNextButtonClicked()
    {
        LoadText(dialogueIndex + 1);
    }

    private void LoadText(int index)
    {
        dialogueIndex = index;

        if (dialogueIndex < greedDialogue.Length)
        {
            _titleText.text = greedDialogue[dialogueIndex].Title;
            _dialogueText.text = greedDialogue[dialogueIndex].Message;
            _nextButton.GetComponentInChildren<Text>().text = greedDialogue[dialogueIndex].OptionText;
        }
        else
        {
            gameObject.SetActive(false);
            Player.Instance.acceptingInput = true;
        }
    }
}
