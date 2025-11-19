using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScript 
{
    public PlayerScript[] players = new PlayerScript[2];
    protected int turn = -1;
    protected int currentPlayerIndex = 0;
    //protected PlayerScript currentPlayer;

    public PlayerScript CurrentPlayer { get => players[currentPlayerIndex];}
    public int Turn { get => turn; set => turn = value; }
    public int CurrentPlayerIndex { get => currentPlayerIndex;}




    

    
    public GameScript()
    {

    }

    

    public GameScript(bool loadDecks)
    {
        if (loadDecks)
        {
            players[0] = new PlayerScript(PlayerPrefs.GetInt("Player0Deck"));
            players[1] = new PlayerScript(PlayerPrefs.GetInt("Player1Deck"));
        }
        Debug.Log("Player 1 deck:");
        players[0].LogDeck();
        //else
        //{
        //    players[0] = new PlayerScript(11);
        //    players[1] = new PlayerScript(11);
        //}
    }

    public void ChangeTurn()
    {
        
        if (currentPlayerIndex == 1)
        {
            turn++;
        }
        currentPlayerIndex = 1 - currentPlayerIndex;
        //currentPlayer = players[currentPlayerIndex];
        //Debug.Log($"Deck length {currentPlayer.deckCount}");
        players[currentPlayerIndex].StartTurn();
    }
    public void HandleAttackCardToCard(Card attacker, Card target)
    {

        // add checks for poison 
        if (target.Effects.Contains('P'))
        {
            attacker.Health = 0;
        }
        else attacker.Health -= target.Attack;
        if (attacker.Effects.Contains('P')) 
        {      
            target.Health = 0; 
        }
        else target.Health -= attacker.Attack;

        attacker.NumberOfAttacksAvailable--;
        if (attacker.Health <= 0)
        {   
            attacker.owner.RemoveCard(attacker.Index);
        }
        if (target.Health <= 0)
        {
            target.owner.RemoveCard(target.Index);
            
        }
    }
    public void HandleAttackCardToHero(Card attacker, PlayerScript target)
    {
        target.Health -= attacker.Attack;
        attacker.NumberOfAttacksAvailable--;
    }

    public bool CheckWin(int index)
    {
        if (players[index].Health <= 0)
        {
            return true;
        }
        return false;
    }

   

}
