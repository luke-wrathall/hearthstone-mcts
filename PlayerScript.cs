using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class PlayerScript
{
    public Card[] deck = new Card[30];
    public int deckCount;
    public Card[] hand = new Card[10];
    public Card[] board = new Card[7];
    private bool[] fullBoardSlots = new bool[7];

    private int handCount = 0;
    private int manaTotal = 0;
    private int manaAvailable = 0;
    private int health = 30;

    public int ManaAvailable { get => manaAvailable; set => manaAvailable = value; }
    public int Health { get => health; set => health = value; }
    public bool[] FullBoardSlots { get => fullBoardSlots;}
    public int HandCount { get => handCount;}

    public Card RightMostCard { get => hand[handCount - 1]; }
    public int ManaTotal { get => manaTotal; set => manaTotal = value; }

    protected string folderLocation = Application.dataPath + "/SavedData";

    public PlayerScript()
    {
        
    }

    public PlayerScript(int index)
    {
        LoadDeck(index);
        deckCount = 30;
        ShuffleDeck();

    }


    public PlayerScript Duplicate()
    {
        PlayerScript newPlayerScript = new PlayerScript(); 
        for (int i = 0; i < hand.Length; i++)
        {
            if (hand[i] != null)
            {
                newPlayerScript.hand[i] = hand[i].Duplicate(newPlayerScript);
            }
        }
        for (int i = 0; i < board.Length; i++)
        {
            if (board[i] != null)
            {
                newPlayerScript.board[i] = board[i].Duplicate(newPlayerScript);
            }
        }
        for (int i = 0; i < deck.Length; i++)
        {
            if (deck[i] != null)
            {
                newPlayerScript.deck[i] = deck[i].Duplicate(newPlayerScript);
            }
        }

        newPlayerScript.deckCount = deckCount;

        for (int i = 0; i<FullBoardSlots.Length; i++)
        {
            newPlayerScript.fullBoardSlots[i] = fullBoardSlots[i];
        }
        newPlayerScript.handCount = handCount;
        newPlayerScript.manaAvailable = manaAvailable;
        newPlayerScript.manaTotal = manaTotal;
        newPlayerScript.health = health;
        return newPlayerScript;
    }

    

    public Card DrawCard()
    {
        Card drawnCard = null;
        if (deckCount > 0)
        {
            if (handCount < 10)
            {
                //Draw top card of deck
                drawnCard = deck[deckCount - 1];
                drawnCard.Index = handCount;
                drawnCard.inHand = true;
                hand[handCount] = drawnCard;
                handCount++;


            }
            deck[deckCount - 1] = null;
            deckCount--;

        }
        return drawnCard;

    }
    public int PlayCard(int cardIndex)
    {
        Card card = hand[cardIndex];
        manaAvailable -= card.ManaCost;
        int index = AddToBoard(hand[cardIndex]);
        UpdateHand(cardIndex);
        return index;
    }
    public int AddToBoard(Card card)
    {
        int index = FindFirstEmptySlot();
        board[index] = card;
        fullBoardSlots[index] = true;
        card.UpdateToBoard(index);
        return index;
    }

    public int FindFirstEmptySlot()
    {
        int i = 0;
        while (i < fullBoardSlots.Length - 1 && fullBoardSlots[i] == true)
        {
            i++;
        }
        // -1 is full case 
        if (fullBoardSlots[i] == true && i == fullBoardSlots.Length - 1) { i = -1; };
        return i;
    }

    public void StartTurn()
    {
        if (manaTotal < 10)
        {
            manaTotal++;
        }
        manaAvailable = manaTotal;

        foreach (Card card in board)
        {
            //Only restores attacks if not null
            card?.RestoreAttacks();
        }

        DrawCard();

    }

    public void UpdateHand(int index)
    {
        hand[index] = null;
        while (index < hand.Length - 1 && hand[index + 1] != null)
        {
            index++;
            hand[index].Index--;
            hand[index - 1] = hand[index];


        }
        hand[index] = null;
        handCount--;

    }
    public void RemoveCard(int index)
    {
        board[index] = null;
        fullBoardSlots[index] = false;
    }
    public void LoadDeck(int deckIndex)
    {
        string filename = folderLocation + $"/Deck{deckIndex}";
        StreamReader reader = new StreamReader(filename);
        int locationInDeck = 0;
        string line = reader.ReadLine();
        while (line != null)
        {
            //Parse each line in file
            string[] values = line.Split(',');
            List<char> effects = new();
            //Effects stored as contiguous string of characters, so we loop through 
            foreach( char c in values[4])
            {
                effects.Add(c);
            }
            //Create a new card with the read values 
            Card newCard = new Card(values[0], Convert.ToInt32(values[1]), Convert.ToInt32(values[2]), Convert.ToInt32(values[3]), effects);
            for (int i = 0; i < Convert.ToInt32(values[5]); i++)
            {
                
                deck[locationInDeck] = newCard.Duplicate(this);
                locationInDeck++;
            }
            line = reader.ReadLine();
        }
        reader.Close();
    }

    public void ShuffleDeck()
    {

        Card temp;

        for (int i = 0; i < deck.Length; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, deck.Length);
            temp = deck[randomIndex];
            
            deck[randomIndex] = deck[i];
            deck[i] = temp;

        }
    }

    public bool CheckTaunts()
    {
        bool flag = false;
        foreach (Card card in board)
        {
            if (card != null)
            {
                if (card.Effects.Contains('T'))
                    flag = true;
            }
        }
        return flag;
    }

    public void LogDeck()
    {
        foreach (Card card in deck)
        {
            Debug.Log(card.Name);
        }
    }

}
