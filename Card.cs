using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Card 
{
    private int attack;
    private int health;
    private int manaCost;
    private List<char> effects;
    private string name;
    //public string cardName;
    private int index;
    //private int ownerIndex;
    //change this to work with windfury
    private int numberOfAttacksAvailable = 0;
    public PlayerScript owner;


    public bool inHand = false;
    public bool onBoard = false;

    public bool AbleToAttack { get => numberOfAttacksAvailable > 0; }

    public int NumberOfAttacksAvailable { get => numberOfAttacksAvailable;  set => numberOfAttacksAvailable = value; }
    public int ManaCost { get => manaCost; set => manaCost = value; }
    public int Health { get => health; set => health = value; }

    public string Name { get => name; set => name = value; }

    public int Attack { get => attack; set => attack = value; }

    public List<char> Effects { get => effects;  }

    public int Index { get => index; set => index = value; }
    //public PlayerScript Owner { get => owner; set => owner = value; }

    public Card()
    {

    }
    
    public Card(string cardName, int cardAttack, int cardHealth, int cardManaCost, List<char> cardEffects) 
    {
        name = cardName;
        attack = cardAttack;
        health = cardHealth;
        manaCost = cardManaCost;
        effects = cardEffects;
    }
    public Card(string cardName, int cardAttack, int cardHealth, int cardManaCost, List<char> cardEffects, int cardIndex, int cardNumberOfAttacksAvailable, bool cardInHand)
    {
        name = cardName;
        attack = cardAttack;
        health = cardHealth;
        manaCost = cardManaCost;
        effects = cardEffects;
        index = cardIndex;
        numberOfAttacksAvailable = cardNumberOfAttacksAvailable;
        inHand = cardInHand;
    }

    public Card Duplicate(PlayerScript playerScriptOfNewOwner)
    {
        Card newCard = new Card(name, attack, health, manaCost, effects, index, numberOfAttacksAvailable, inHand);
        //we do not copy owner because problems 
        newCard.owner = playerScriptOfNewOwner; 
        return newCard;
    }
    public bool CheckPlayable(PlayerScript currentPlayer)
    {
        if (owner != currentPlayer) return false;
        if (!inHand) return false;
        if (manaCost > owner.ManaAvailable) return false;
        // 6 will be last slot 
        if (owner.FindFirstEmptySlot() == -1) return false;
        return true;
    }

    public bool CheckCanAttack()
    {
        if (!AbleToAttack) return false;
        if (inHand) return false;
        return true;
    }

    public bool CheckAbleToBeAttacked(PlayerScript currentPlayer)
    {
        if (owner == currentPlayer) return false;
        if (inHand) return false;
        else return true;
    }

    public void UpdateToBoard(int i)
    {
        index = i;
        inHand = false;
        onBoard = true;
        if (effects.Contains('C')) { RestoreAttacks(); }
    }

    public void RestoreAttacks()
    {
        if (effects.Contains('M'))
        {
            numberOfAttacksAvailable = 4;
        }
        else if (effects.Contains('W'))
        {
            numberOfAttacksAvailable = 2;
        }
        else
        {
            numberOfAttacksAvailable = 1;
        }
    }
}
