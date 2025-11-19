using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public class CardManager : DeckDisplay
{

    public List<Card> allCards = new List<Card>();
    Vector2[,] cardPositions;
    public CardSprite defaultCard;
    private const int cardsPerPage = 12;
    private CardSprite[,] allPages;
    int numPages;
    private int currentPageIndex = 0;
    private bool[] loadedPages;

    public SpriteRenderer leftArrow;
    public SpriteRenderer rightArrow;

    private int currentDeckIndex;

    private const int deckSizeLimit = 30;

    public SpriteRenderer undoArrow;
    public SpriteRenderer saveIcon;
    
    private List<int> addedNegationStack = new List<int>();
    private List<Card> removedNegationStack = new List<Card>();
    private List<bool> actionWasAddStack = new List<bool>();


    private int totalCardsInDeck;

    void Start()
    {
        LoadAllCards();
        numPages = (allCards.Count / cardsPerPage) + 1;
        loadedPages = new bool[numPages];
        allPages = new CardSprite[numPages,cardsPerPage];

        CalculatePositions();
        
        LoadCurrentPage(currentPageIndex);
        DisplayCurrentPage();
        UpdateLRArrows();

        cardCounts = new List<int>();
        sortedDeck = new List<Card>();
        for (int i = 0; i < deckCardSnippets.Length; i++)
        {
            CardSnippetSprite newCardSnip = Instantiate(defaultCardSnippet);

            newCardSnip.gameObject.SetActive(false);
            deckCardSnippets[i] = newCardSnip;
        }
        currentDeckIndex = PlayerPrefs.GetInt("DeckToEdit");
        if (LoadDeck(currentDeckIndex)) totalCardsInDeck = deckSizeLimit;
        UpdateDisplay();
        UpdateIcons();


    }
    public void HandleCardSnippetClick(int index)
    {
        RemoveCard(index);
    }

    public void HandleLeftArrowClick()
    {

        if (currentPageIndex == 0)
        {
            return;
        }
        ToggleCurrentPage(false);
        currentPageIndex--;
        LoadCurrentPage(currentPageIndex);
        DisplayCurrentPage();
        UpdateLRArrows();

    }

    public void HandleRightArrowClick()
    {
        if ((currentPageIndex + 1) * cardsPerPage >= allCards.Count)
        {
            return;
        }

        ToggleCurrentPage(false);
        currentPageIndex++;
        LoadCurrentPage(currentPageIndex);
        DisplayCurrentPage();
        UpdateLRArrows();
    }

    public void HandleUndo()
    {
        Undo();
    }



    private void CalculatePositions()
    {
        cardPositions = new Vector2[4, 3];
        Vector2 start = this.transform.position;
        Vector2 end = new Vector2(4, -5);
        float stepX = (end.x - start.x) / cardPositions.GetLength(0);
        float stepY = (end.y - start.y) / cardPositions.GetLength(1);
        for (int i = 0; i < cardPositions.GetLength(0); i++)
        {
            for (int j = 0; j < cardPositions.GetLength(1); j++)
            {
                cardPositions[i, j] = start;
                cardPositions[i, j].x += stepX *i;
                cardPositions[i, j].y += stepY *j;
            }
        }
    }

    private void DisplayCurrentPage()
    {
        int k = 0;
        for (int i = 0; i < cardPositions.GetLength(1); i++)
        {
            for (int j = 0; j < cardPositions.GetLength(0); j++)
            {
                if (allPages[currentPageIndex, k] != null)
                {
                    allPages[currentPageIndex, k].gameObject.SetActive(true);
                    //j and i are reversed so that cards are displayed in the correct order
                    allPages[currentPageIndex, k].transform.position = cardPositions[j, i];
                }
                k++;
            }
        }
    }

    private void LoadCurrentPage(int pageIndex)
    {
        if (loadedPages[pageIndex])
        {
            return;
        }
        int allCardsIndex = pageIndex * cardsPerPage;
        for (int indexWithinPage = 0; indexWithinPage < cardsPerPage; indexWithinPage++)
        {    
            if (allCardsIndex + indexWithinPage < allCards.Count)
            {
                CardSprite newCardSprite = Instantiate(defaultCard);
                newCardSprite.Populate(allCards[allCardsIndex + indexWithinPage]);
                allPages[currentPageIndex, indexWithinPage] = newCardSprite;
            }
        }
        loadedPages[pageIndex] = true;
    }

    private void LoadAllCards()
    {
        StreamReader reader = new StreamReader(folderLocation + "/AllCards.txt");
        string line = reader.ReadLine();
        while (line != null)
        {

            string[] values = line.Split(',');
            if (values.Length >= 3)
            {
                List<char> effects = new();
                if (values.Length >= 4)
                {
                    foreach (char c in values[4])
                    {
                        effects.Add(c);
                    }
                }
                Card newCard = new Card(values[0], Convert.ToInt32(values[1]), Convert.ToInt32(values[2]), Convert.ToInt32(values[3]), effects);
                allCards.Add(newCard);
            }
            line = reader.ReadLine();
        }
        reader.Close();
        QuickSortCardSet(0, allCards.Count - 1);
    }

    public void ToggleCurrentPage(bool activityState)
    {
        int i = 0;
        while (i < cardsPerPage)
        {
            if (allPages[currentPageIndex, i] != null)
            {
                allPages[currentPageIndex, i].gameObject.SetActive(activityState);             
            }
            i++;
        }
    }
    public void UpdateLRArrows()
    {
        Debug.Log($"{currentPageIndex}");
        if (currentPageIndex + 1 >= numPages)
        {
            rightArrow.enabled = false;
        }
        else
        {
            rightArrow.enabled = true;
        }
        if (currentPageIndex == 0)
        {
            leftArrow.enabled = false;
        }
        else
        {
            leftArrow.enabled = true;
        }
    }

    public void AddCard(Card card, bool initiatedByUser = true)
    {
        if (totalCardsInDeck >= deckSizeLimit)
        {
            return;
        }
        totalCardsInDeck++;
        if (initiatedByUser) PushToActionWasAdd(true);
        for (int i = 0; i < sortedDeck.Count; i++)
        {
            if (sortedDeck[i].Name == card.Name)
            {
                cardCounts[i]++;
                deckCardSnippets[i].UpdateNumber(cardCounts[i]);
                saveIcon.gameObject.SetActive(totalCardsInDeck == deckSizeLimit);
                if (initiatedByUser) PushToAddedNegtation(i);
                return;
            }
        }
        int index = SortInCard(card);
        if (initiatedByUser) PushToAddedNegtation(index);
        UpdateDisplay();
        UpdateIcons();
    }

    public void RemoveCard(int index, bool initiatedByUser = true)
    {
        if (totalCardsInDeck <= 0)
        {
            return;
        }
        totalCardsInDeck--;
        if (initiatedByUser)
        {
            PushToActionWasAdd(false);
            PushToRemoveNegation(sortedDeck[index]);
            undoArrow.gameObject.SetActive(true);
        }
        if (cardCounts[index] > 1)
        {
            cardCounts[index]--;
            deckCardSnippets[index].UpdateNumber(cardCounts[index]);
        }
        else
        {
            sortedDeck.RemoveAt(index);
            cardCounts.RemoveAt(index);
        }
        UpdateDisplay();
        UpdateIcons();
    }

    public void Undo()
    {
        if (actionWasAddStack.Count == 0)
        {
            Debug.Log("Stack Empty");
            return;
        }
        bool lastActionWasAdd = actionWasAddStack[actionWasAddStack.Count - 1];
        actionWasAddStack.RemoveAt(actionWasAddStack.Count - 1);
        if (lastActionWasAdd)
        {
            int indexToRemove = PopAddedNegation();
            if (indexToRemove == -1)
            {
                Debug.Log("Added stack empty");
                return;
            }
            RemoveCard(indexToRemove, false);
        }
        else
        {
            Card cardToAdd = PopRemoveNegation();
            if (cardToAdd == null)
            {
                Debug.Log("Remove stack empty");
                return;
            }
            AddCard(cardToAdd, false);
        }
        undoArrow.gameObject.SetActive(actionWasAddStack.Count > 0);
    }

    public void Save()
    {
        if (totalCardsInDeck < deckSizeLimit) { return; }
        string filename = folderLocation + $"/Deck{currentDeckIndex}";
        StreamWriter writer = new StreamWriter(filename, false);
        for (int i = 0; i < sortedDeck.Count; i++)
        {
            string effectString = "";
            foreach (char c in sortedDeck[i].Effects) { effectString += c; }
            string line = $"{sortedDeck[i].Name},{sortedDeck[i].Attack},{sortedDeck[i].Health},{sortedDeck[i].ManaCost},{effectString},{cardCounts[i]}";
            writer.WriteLine(line);
        }
        writer.Close();
    }

    public int SortInCard(Card card)
    {
        sortedDeck.Add(card);
        cardCounts.Add(1);
        int j = sortedDeck.Count - 2;
        while (j >= 0 && CheckBefore(sortedDeck[j], card))
        {
            sortedDeck[j + 1] = sortedDeck[j];

            cardCounts[j + 1] = cardCounts[j];

             
            j--;
        }
        sortedDeck[j + 1] = card;
        cardCounts[j + 1] = 1;
        return j + 1;
    }

    public void QuickSortCardSet(int low, int high)
    {
        if (low < high)
        {
            int partitionIndex = Partition(low, high);
            QuickSortCardSet(low, partitionIndex - 1);
            QuickSortCardSet(partitionIndex + 1, high);
        }
    }

    public int Partition(int low, int high)
    {
        Card pivot = allCards[high];
        int i = low - 1;
        for (int j = low; j <= high - 1; j++)
        {
            
            if (CheckBefore(pivot, allCards[j]))
            {
                i++;
                (allCards[i], allCards[j]) = (allCards[j], allCards[i]);
            }
        }
        (allCards[i + 1], allCards[high]) = (allCards[high], allCards[i + 1]);
        return i + 1;
    }

    public bool CheckBefore(Card firstCard, Card secondCard)
    {
        if (firstCard.ManaCost > secondCard.ManaCost)
        {
            return true;
        }
        if (firstCard.ManaCost < secondCard.ManaCost)
        {
            return false;
        }
        int i = 0;
        int shorterLength = (firstCard.Name.Length < secondCard.Name.Length) ? firstCard.Name.Length : secondCard.Name.Length;
        while (i < shorterLength)
        {
            if (Convert.ToInt32(firstCard.Name[i]) > Convert.ToInt32(secondCard.Name[i]))
            {
                return true;
            }
            if (Convert.ToInt32(firstCard.Name[i]) < Convert.ToInt32(secondCard.Name[i]))
            {
                return false;
            }
            i++;
        }
        return false;
    }


    private void UpdateIcons()
    {
        undoArrow.gameObject.SetActive(actionWasAddStack.Count > 0);
        saveIcon.gameObject.SetActive(totalCardsInDeck == deckSizeLimit);

    }

    private void PushToAddedNegtation(int index)
    {
        addedNegationStack.Add(index);
    }

    private void PushToRemoveNegation(Card card)
    {
        removedNegationStack.Add(card);
    }

    private void PushToActionWasAdd(bool indicator)
    {
        actionWasAddStack.Add(indicator);
    }

    private int PopAddedNegation()
    {
        if (addedNegationStack.Count == 0)
        {
            return -1;
        }
        int poppedValue = addedNegationStack[addedNegationStack.Count - 1];
        addedNegationStack.RemoveAt(addedNegationStack.Count - 1);
        return poppedValue;
    }
    private Card PopRemoveNegation()
    {
        if (removedNegationStack.Count == 0)
        {
            return null;
        }
        Card poppedValue = removedNegationStack[removedNegationStack.Count - 1];
        removedNegationStack.RemoveAt(removedNegationStack.Count - 1);
        return poppedValue;
    }


}
