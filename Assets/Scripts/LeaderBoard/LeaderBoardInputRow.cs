using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderBoardInputRow : MonoBehaviour
{
    private const int CHAR_LIMIT = 20;
    private const string validCharacters = " 1234567890qwertyuiopasdfghjklzxcvbnm";

    [SerializeField] private TextMeshProUGUI rankTextMesh;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI scoreTextMesh;

    private LeaderBoard leaderBoard;
    private int score;

    private void Start()
    {
        inputField.onEndEdit.AddListener(Submit);
        inputField.onValueChanged.AddListener(BadWordFilter);
        inputField.onValidateInput = (string text, int charIndex, char addedChar) => { return ValidChar(addedChar); };
        inputField.characterLimit = CHAR_LIMIT;
    }


    public void Init(int rank, int score, LeaderBoard leaderBoard)
    {
        rankTextMesh.text = rank.ToString();
        this.score = score;
        scoreTextMesh.text = score.ToString();
        this.leaderBoard = leaderBoard;
    }


    private void Submit(string entryName)
    {
        leaderBoard.SubmitPlayerName(new LeaderBoardEntry { PlayerName = entryName, Score = score });
    }


    private void BadWordFilter(string textToCheck)
    {
        for (int i = 0; i < BadWordList.BadWords.Length; i++)
        {
            if (textToCheck.ToLower().Contains(BadWordList.BadWords[i]))
            {
                for(int j = 0; j < textToCheck.Length; j++)
                {
                    string temp = textToCheck.Substring(j, BadWordList.BadWords[i].Length);
                    if(temp.ToLower() == BadWordList.BadWords[i])
                    {
                        textToCheck = textToCheck.Remove(j, BadWordList.BadWords[i].Length);
                        if (textToCheck != null)
                        {
                            inputField.text = textToCheck.ToString();
                        }
                        else
                        {
                            inputField.text = "";
                        }
                        return;
                    }
                }
            }
        }
    }

    private char ValidChar(char addedChar)
    {
        if (validCharacters.IndexOf(addedChar) != -1)
        {
            //Valid
            return addedChar;
        }
        else
        {
            //Invalid
            return '\0';
        }
    }

}
