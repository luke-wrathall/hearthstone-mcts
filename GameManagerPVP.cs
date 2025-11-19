using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManagerPVP : GameManager
{
    //public SpriteRenderer startButton;
    public TMP_Text startMessage;

    protected override void Start()
    {
        SetUpGame();
        playerSprites[0].HideHand();
        
    }
    public override void HandleEndTurn()
    {
        playerSprites[gameScript.CurrentPlayerIndex].HideHand();
        playerSprites[gameScript.CurrentPlayerIndex].UpdateBoardCardHighlights(false);
        //startButton.enabled = true;
        startMessage.text = $"Click to start player {2 - gameScript.CurrentPlayerIndex}'s turn";
        startMessage.gameObject.SetActive(true);
        startMessage.enabled = true;
    }

    public override void StartNextTurn()
    {

        //startButton.enabled = false;
        startMessage.gameObject.SetActive(false);
        startMessage.enabled = false;
        if (gameScript.Turn != -1)
        {
            gameScript.ChangeTurn();
            UpdateFromChangeTurn(gameScript.CurrentPlayerIndex);
        }
        else
        {
            gameScript.Turn++;
        }
        playerSprites[gameScript.CurrentPlayerIndex].ShowHand();
    }
}
