using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using TMPro;
public class DeckDisplay : MonoBehaviour
{

    public CardSnippetSprite[] deckCardSnippets = new CardSnippetSprite[30];
    public List<int> cardCounts;
    public List<Card> sortedDeck; //no duplicates in this
    public CardSnippetSprite defaultCardSnippet;
    private const float maxSpace = -1f;

    public TMP_Text deckNameText;

    public Transform deckTopMarker;
    public Transform deckBottomMarker;


    protected string folderLocation =  Application.dataPath + "/SavedData";

    void Start()
    {
        cardCounts = new List<int>();
        sortedDeck = new List<Card>();
        for (int i = 0; i < deckCardSnippets.Length; i++)
        {
            CardSnippetSprite newCardSnip = Instantiate(defaultCardSnippet);

            newCardSnip.gameObject.SetActive(false);
            deckCardSnippets[i] = newCardSnip;
        }
        LoadDeck(1);
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        Vector2 start = deckTopMarker.position;
        Vector2 end = deckBottomMarker.position;
        float stepY = (end.y - start.y) / (float)sortedDeck.Count;
        if (stepY < maxSpace)
        {
            stepY = maxSpace;
        }
        int i = 0;
        while (i < sortedDeck.Count)
        {
            deckCardSnippets[i].gameObject.SetActive(true);
            deckCardSnippets[i].transform.position = new Vector2(start.x, start.y + i * stepY);
            deckCardSnippets[i].Populate(sortedDeck[i], cardCounts[i]);
            deckCardSnippets[i].index = i;
            i++;
        }
        while (i < deckCardSnippets.Length)
        {
            deckCardSnippets[i].gameObject.SetActive(false);
            i++;
        }
    }


    public bool LoadDeck(int deckIndex)
    {
        deckNameText.text = $"Deck {deckIndex}";
        string filename = folderLocation + $"/Deck{deckIndex}";
        sortedDeck.Clear();
        cardCounts.Clear();
        if (!File.Exists(filename))
        {
            return false;
        }
        StreamReader reader = new StreamReader(filename);
        string line = reader.ReadLine();
        while (line != null)
        {
            string[] values = line.Split(',');
            List<char> effects = new();
            foreach (char c in values[4])
            {
                effects.Add(c);
            }
            Card newCard = new Card(values[0], Convert.ToInt32(values[1]), Convert.ToInt32(values[2]), Convert.ToInt32(values[3]), effects);
            sortedDeck.Add(newCard);
            cardCounts.Add(Convert.ToInt32(values[5]));
            line = reader.ReadLine();
        }
        reader.Close();
        return true;
    }
}
