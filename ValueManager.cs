using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using UnityEngine.Animations;
using UnityEngine.UIElements;
using System.IO;
using UnityEngine.SceneManagement;

public class ValueManager :MonoBehaviour
{
    public TMP_Text player0DropdownLabel;
    public TMP_Text player1DropdownLabel;
    public TMP_Text difficultyDropDown;

    public DeckDisplay player0Display;
    public DeckDisplay player1Display;

    protected string folderLocation = Application.dataPath + "/SavedData";





    public void SavePlayersDecksPVP()
    {
        if (BothDecksValid())
        {
            PlayerPrefs.SetInt("Player0Deck", ParseLabel(player0DropdownLabel.text));
            PlayerPrefs.SetInt("Player1Deck", ParseLabel(player1DropdownLabel.text));
            SceneManager.LoadScene("GameScene");
        }
    }

    public void SavePlayersDecksAI()
    {
        PlayerPrefs.SetString("Difficulty", difficultyDropDown.text);
        if (BothDecksValid())
        {
            PlayerPrefs.SetInt("Player0Deck", ParseLabel(player0DropdownLabel.text));
            PlayerPrefs.SetInt("Player1Deck", ParseLabel(player1DropdownLabel.text));
            SceneManager.LoadScene("GameSceneAI");
        }
    }

    public void UpdatePlayPrefsDeckEdit()
    {
        PlayerPrefs.SetInt("DeckToEdit", ParseLabel(player0DropdownLabel.text) );
        Debug.Log($"{PlayerPrefs.GetInt("DeckToEdit")}");
    }

    public void UpdatePlayer0Display()
    {
        PlayerPrefs.SetInt("Player0Deck", ParseLabel(player0DropdownLabel.text));
        player0Display.LoadDeck(ParseLabel(player0DropdownLabel.text));
        player0Display.UpdateDisplay();

    }

    public void UpdatePlayer1Display()
    {
        PlayerPrefs.SetInt("Player1Deck", ParseLabel(player1DropdownLabel.text));
        player1Display.LoadDeck(ParseLabel(player1DropdownLabel.text));
        player1Display.UpdateDisplay() ;

    }

    private int ParseLabel(string label)
    {
        return (Convert.ToInt32(label.Split(' ')[1]));
    }

    private bool BothDecksValid()
    {
        string filename0 = folderLocation + $"/Deck{ParseLabel(player0DropdownLabel.text)}";
        string filename1 = folderLocation + $"/Deck{ParseLabel(player1DropdownLabel.text)}";
        return (File.Exists(filename0) && File.Exists(filename1));

    }
}
