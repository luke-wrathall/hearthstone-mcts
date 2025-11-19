using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour


{

    //these have to be public for unity 
    public GameScript gameScript;
    public PlayerSprite[] playerSprites = new PlayerSprite[2];
    public TMP_Text winMessage;




    protected virtual void Start()
    {
        SetUpGame();
     
    }

    public void SetUpGame()
    {
        //creates a new game were players load decks from file 
        gameScript = new GameScript(true);
        gameScript.players[0].ManaAvailable = 1;
        gameScript.players[0].ManaTotal = 1;

        for (int i = 0; i < 2; i++)
        {
            //sets up hands at start of game 
            for (int j = 0; j < 3 + i; j++)
            {
                gameScript.players[i].DrawCard();

                playerSprites[i].DrawCard(gameScript.players[i].hand[gameScript.players[i].HandCount - 1], gameScript.players[i].HandCount - 1);

            }
            playerSprites[i].UpdateHeatlh(gameScript.players[i].Health);
            playerSprites[i].UpdateDeckCount(gameScript.players[i].deckCount);
            playerSprites[i].UpdateMana(gameScript.players[i].ManaAvailable, gameScript.players[i].ManaTotal);

        }
        playerSprites[1].HideHand();
    }

    public void UpdateFromAttack(int playerIndex, int boardIndex, Card card)
    {
        if (card.Health <= 0)
        {
            //sets the card sprite to inactive if it is destroyed 
            playerSprites[playerIndex].board[boardIndex].GameObject().SetActive(false);                
        }
        playerSprites[playerIndex].board[boardIndex].UpdateStats(card);

        if (playerIndex == gameScript.CurrentPlayerIndex)
        {
            playerSprites[playerIndex].board[boardIndex].UpdateHighlight(card.AbleToAttack) ;
        }

    }

    public void UpdateFromCardDestroyed(int playerIndex, int boardIndex)
    {
        playerSprites[playerIndex].DestroyCard(boardIndex);
    }

    public void UpdateFromPlayCard(int playerIndex, int playedCardIndex, int boardIndex, int newMana, bool ableToAttack)
    {
        playerSprites[playerIndex].PlayCard(playedCardIndex, boardIndex, ableToAttack);
        playerSprites[playerIndex].UpdateMana(newMana);
    }



    public bool HandleHeroAttackAttempt(int heroPlayerIndex, Card selectedCard)
    {
        if (heroPlayerIndex == gameScript.CurrentPlayerIndex)
        {
            return false;
        }
        if (gameScript.players[heroPlayerIndex].CheckTaunts())
        {
            return false;
        }
        
        PlayerScript heroPlayer = gameScript.players[heroPlayerIndex];
        gameScript.HandleAttackCardToHero(selectedCard, heroPlayer);

        playerSprites[gameScript.CurrentPlayerIndex].ResetCardPositions();
        StartCoroutine(playerSprites[gameScript.CurrentPlayerIndex].AnimateMovement(playerSprites[gameScript.CurrentPlayerIndex].board[selectedCard.Index], playerSprites[1 - gameScript.CurrentPlayerIndex].GetComponentInChildren<SpriteRenderer>().transform.position, true));
        playerSprites[heroPlayerIndex].UpdateHeatlh(heroPlayer.Health);
        playerSprites[1 - heroPlayerIndex].board[selectedCard.Index].UpdateHighlight(selectedCard.AbleToAttack);
        //Debug.Log($"Health: {heroPlayer.Health}");
        if (heroPlayer.Health <= 0)
        {
            //Debug.Log("win");
            winMessage.text = $"Player {(1 - heroPlayerIndex) + 1} wins! Click here to continue";
            winMessage.gameObject.SetActive(true);
            winMessage.enabled = true;
        }

        return true;
        
    }

    public void HandleCardAttack(Card selectedCard, Card card)
    {
        gameScript.HandleAttackCardToCard(selectedCard, card);
        //updating sprites
        StartCoroutine(playerSprites[gameScript.CurrentPlayerIndex].AnimateMovement(playerSprites[gameScript.CurrentPlayerIndex].board[selectedCard.Index], playerSprites[1 - gameScript.CurrentPlayerIndex].board[card.Index].transform.position, true));
        UpdateFromAttack(gameScript.CurrentPlayerIndex, selectedCard.Index, selectedCard);
        UpdateFromAttack(1 - gameScript.CurrentPlayerIndex, card.Index, card);
    }

    public void HandleCardPlay(Card card)
    {

        //saving index here because it gets changed when moves to board 
        int cardIndex = card.Index;

        int boardIndex = card.owner.PlayCard(card.Index);
        UpdateFromPlayCard(gameScript.CurrentPlayerIndex, cardIndex, boardIndex, card.owner.ManaAvailable, card.AbleToAttack);
    }

    public bool CheckCardAbleToBeAttacked(Card card)
    {

        bool taunts = card.owner.CheckTaunts();
        if (taunts)
        {
            if (!card.Effects.Contains('T'))
            {
                return false;
            }
        }
        return card.CheckAbleToBeAttacked(gameScript.CurrentPlayer);
    }

    public bool CheckCardAbleToBePlayed(Card card)
    {

        return card.CheckPlayable(gameScript.CurrentPlayer);
    }
    public virtual void  HandleEndTurn()
    {

    }

    public void UpdateFromChangeTurn(int currentPlayerIndex)
    {
        playerSprites[currentPlayerIndex].UpdateMana(gameScript.CurrentPlayer.ManaAvailable, gameScript.CurrentPlayer.ManaTotal);
        playerSprites[currentPlayerIndex].UpdateBoardCardHighlights(true);
        playerSprites[1 - currentPlayerIndex].UpdateBoardCardHighlights(false);
        if (playerSprites[currentPlayerIndex].hand[9] == null) //stops bug when hand is full a little janky but works 
        {
            UpdateFromCardDrawn(currentPlayerIndex, gameScript.CurrentPlayer.hand[gameScript.CurrentPlayer.HandCount - 1], gameScript.CurrentPlayer.HandCount - 1);
        }
        playerSprites[currentPlayerIndex].UpdateDeckCount(gameScript.players[currentPlayerIndex].deckCount);
    }

    public void UpdateFromCardDrawn(int playerIndex, Card card, int cardIndex)
    {
        playerSprites[playerIndex].DrawCard(card, cardIndex);
        playerSprites[playerIndex].UpdateDeckCount(gameScript.players[playerIndex].deckCount);
    }

    public PlayerScript PlayerFromIndex(int playerIndex)
    {
        return gameScript.players[playerIndex];
    }

    public bool AnyWinner()
    {
        return gameScript.CheckWin(0) || gameScript.CheckWin(1);
    }

    public virtual void StartNextTurn()
    {

    }
}
