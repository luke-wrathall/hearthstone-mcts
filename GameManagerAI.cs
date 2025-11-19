using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerAI : GameManager
{
    public int treeSearchIterations = 40;
    private Dictionary<string, int> iterationsFromDifficultyName = new() {
        { "Easy", 1000},
        {"Medium", 2000 },
        {"Hard", 3000 },
        {"Challenge", 5000 }
        };
    public override void HandleEndTurn()
    {
        gameScript.ChangeTurn();
        UpdateFromChangeTurn(gameScript.CurrentPlayerIndex);
        playerSprites[gameScript.CurrentPlayerIndex].HideHand();
        if (gameScript.CurrentPlayerIndex == 1)
        {
            StartCoroutine(HandleTreeSearch());
        }
        else
        {
            playerSprites[gameScript.CurrentPlayerIndex].ShowHand();
        }
    }

    protected override void Start()
    {
        treeSearchIterations = iterationsFromDifficultyName[PlayerPrefs.GetString("Difficulty")];
        SetUpGame();
    }
    public IEnumerator HandleTreeSearch()
    {

        TreeSearchNode rootNode = new TreeSearchNode(gameScript);
        // AI is always player 1 
        //to make work later
        rootNode.GetUntriedMoves();
        while (gameScript.CurrentPlayerIndex == 1)
        {

            for (int i = 0; i < treeSearchIterations; i++)
            {
                //select is a recursive method which carries out the selection part of MCTS
                TreeSearchNode selectedNode = rootNode.Select();
                selectedNode.ExpandFully();
                //backpropagation is done within this 
                selectedNode.SimulateRandomChild();

            }
            //moves root to best child
            rootNode = rootNode.SelectHighestWinrateChild();
            //makes into root
            rootNode.RemoveParent();
            Debug.Log($"Selected Move: {rootNode.ParentMove.type}, {rootNode.ParentMove.currentPlayerIndex}, {rootNode.ParentMove.index1}, {rootNode.ParentMove.index2}");
            yield return StartCoroutine(HandleMoveWithSprites(rootNode.ParentMove));
        }
        playerSprites[1].ResetCardPositions();
    }

    public IEnumerator HandleMoveWithSprites(Move move)
    {
        if (move.type == 'P')
        {
            HandleCardPlay(gameScript.players[move.currentPlayerIndex].hand[move.index1]);
        }
        else if (move.type == 'A')
        {
            if (move.index2 == 7)
            {
                HandleHeroAttackAttempt(move.currentPlayerIndex - 1, gameScript.players[move.currentPlayerIndex].board[move.index1]);
            }
            else
            {
                HandleCardAttack(gameScript.players[move.currentPlayerIndex].board[move.index1], gameScript.players[1 - move.currentPlayerIndex].board[move.index2]);
            }
        }
        else if (move.type == 'E')
        {
            HandleEndTurn();
        }
        yield return new WaitForSeconds(0.5f);
    }
}
