using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Build.Player;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class TreeSearchNode : GameScript
{
    private TreeSearchNode parent;
    private Move parentMove;
    private List<TreeSearchNode> children = new List<TreeSearchNode>();
    private int numberOfVisits = 1;
    protected int[] score = { 0, 0, 0 }; //wins, losses, draws
    private List<Move> untriedMoves;
    private int numberOfMoves = 0;
    private const float c = 1.41f; //0.7f
    private const int numberOfMovesCap = 60;
    private const int numberOfRolloutsCap = 2;


    public int winCount { get => score[0]; }
    public int lossCount { get => score[1]; }
    public int drawCount { get => score[2]; }
    public Move ParentMove { get => parentMove;}



    public TreeSearchNode(TreeSearchNode parent, Move parentMove)
    {

        //Change 
        this.parent = parent;
        this.parentMove = parentMove;
        this.players[0] = parent.players[0].Duplicate();
        this.players[1] = parent.players[1].Duplicate();
        this.turn = parent.turn;
        this.currentPlayerIndex = parent.currentPlayerIndex;
        HandleMove(parentMove);
        //GetUntriedMoves();
    }
    public TreeSearchNode(GameScript currentGameState)
    {
        // may have to change bits here and make it horrible
        //idk how converting between player and player sprite will work may be problems we will see 
        //it was horrible but now it is fine 


        for (int i = 0; i < 2; i++)
        {
            this.players[i] = currentGameState.players[i].Duplicate();
        }
        this.turn = currentGameState.Turn;
        this.currentPlayerIndex = currentGameState.CurrentPlayerIndex;

    }
    public List<Move> GetMoves(int playerToMoveIndex)
    {
        //because moves can be made in different orders may want to check and discount parents childrens moves to improve efficiency
        PlayerScript playerToMove = players[playerToMoveIndex];
        PlayerScript opponent = players[1 - playerToMoveIndex];
        List<Move> validMoves = new List<Move>();
        //check all cards in hand
        for (int i =0; i<playerToMove.hand.Length; i++)
        {
            Card card = playerToMove.hand[i];
            if (card != null)
            {
                if (card.CheckPlayable(playerToMove))
                {
                    validMoves.Add(new Move('P', playerToMoveIndex, i));
                }
            }
        }

        //check all cards in board
        bool taunts = opponent.CheckTaunts();
        for (int i = 0; i < playerToMove.board.Length; i++)
        {
            Card card = playerToMove.board[i];
            if (card != null)
            {
                if (card.CheckCanAttack())
                {
                     for (int j = 0; j < opponent.board.Length; j++)
                        
                     {
                        if (opponent.board[j] != null)
                        {
                            // only adds the attack if there are no taunts on the opponents board, or if the target is one of the taunts 
                            if (!taunts | opponent.board[j].Effects.Contains('T'))
                            {
                                validMoves.Add(new Move('A', playerToMoveIndex, i, j));
                            }
                        }
                     }
                    //add hero attack
                    if (!taunts)
                    {
                        validMoves.Add(new Move('A', playerToMoveIndex, i, 7));
                    }
                }
            }
        }

        //add end of turn
        validMoves.Add(new Move('E', playerToMoveIndex));

        return validMoves;
    }
    public void GetUntriedMoves()
    {
        untriedMoves = GetMoves(currentPlayerIndex);
    }




    private void MakeRandomMove()
    {
        int randomMoveIndex = UnityEngine.Random.Range(0, untriedMoves.Count);
        Move move = untriedMoves[randomMoveIndex];
        untriedMoves.RemoveAt(randomMoveIndex);
        HandleMove(move);
    }
    private void Expand()
    {
        //maybe add in idea of not adding game states already stored somewhere else on tree 
        Move move = untriedMoves[untriedMoves.Count - 1];
        untriedMoves.RemoveAt(untriedMoves.Count - 1);
        TreeSearchNode childNode = new TreeSearchNode(this, move);
        children.Add(childNode);
        ////to make the check in selection work 
        childNode.GetUntriedMoves();
    }

    public void ExpandFully()
    {
        //GetUntriedMoves(); because added earlier
        while (untriedMoves.Count > 0)
        {
            Expand();
        }
    }

    public TreeSearchNode Select()
    {
        if (children.Count == 0 || untriedMoves.Count > 0)
        {
            return this;
        }
        //should check if node is terminal and not choose 
        else
        {
            float maxUCT = float.MinValue;
            int indexToSelect = 0;
            float currentUCT;
            for (int i = 0; i < children.Count; i++)
            {
                //that bad i think
                //currentUCT = (children[i].winCount + Mathf.Sqrt(numSimulations())) / (children[i].numSimulations());
                currentUCT = ((float)(children[i].winCount - children[i].lossCount) / (float)children[i].numberOfVisits) + c * Mathf.Sqrt(Mathf.Log(numberOfVisits) / children[i].numberOfVisits);
                if (currentUCT > maxUCT) 
                {
                    maxUCT = currentUCT;
                    indexToSelect = i;
                }
            }
            return children[indexToSelect].Select();
        }
    }

    public TreeSearchNode SelectHighestWinrateChild()
    {
        float highestWinrate = float.MinValue;
        int indexToSelect = 0;
        float currentWinrate;
        for (int i = 0;i < children.Count;i++)
        {
            currentWinrate = children[i].winrate();
            //if (children[i].ParentMove.type == 'E') { currentWinrate *= 0.5f; }
            if (currentWinrate > highestWinrate) 
            {
                highestWinrate = currentWinrate;
                indexToSelect = i;
            }
        }
        return children[indexToSelect];
    }

    public void SimulateRandomChild()
    {
        int randomIndex = UnityEngine.Random.Range(0, children.Count);

        
        for (int i = 0; i <= numberOfRolloutsCap; i++)
        {
            //make a copy so as not to change state of node
            TreeSearchNode copy = new TreeSearchNode(children[randomIndex]);
            int result = copy.Rollout();
            children[randomIndex].BackPropagate(result);
        }
    }

    public void SimulateAllChildren()
    {
        foreach (TreeSearchNode child in children)
        {
            for (int i = 0; i < numberOfRolloutsCap; i++)
            {
                TreeSearchNode copy = new TreeSearchNode(child);
                int result = copy.Rollout();
                child.BackPropagate(result);
            }
        }
    }
    public int Rollout()
    {
        GetUntriedMoves();
        while (numberOfMoves <= numberOfMovesCap)
        {
            MakeRandomMove();
            //a bit inefficient maybe improve 
            GetUntriedMoves();
            //returns code of losing player
            if (players[0].Health <= 0)
            {
                return 0;
            }
            if (players[1].Health <= 0)
            {
                return 1;
            }
            numberOfMoves++;
        }
        //Debug.Log("Draw");
        return 2;
    }

    public int numSimulations()
    { 
        return (score[0] + score[1] + score[2]);
    }

    public float winrate()
    {
        return (float)(score[0] - score[1]) / (float)numberOfVisits;
    }

    public void BackPropagate(int result)
    {
        score[result]++;
        numberOfVisits++;
        if (parent != null)
        {
            parent.BackPropagate(result);
        }
    }
    public void RemoveParent()
    {
        parent = null;
    }
    private void LogMoves()
    {
        Debug.Log("Logging moves in untried moves: ");
        foreach (Move move in untriedMoves)
        {
            Debug.Log($"Move: {move.type}, {move.currentPlayerIndex}, {move.index1}, {move.index2}");
        }
    }
    public void HandleMove(Move move)
    {
        if (move.type == 'P')
        {
            players[move.currentPlayerIndex].PlayCard(move.index1);

        }
        else if (move.type == 'A')
        {
            //we use 7 as code for hero 
            if (move.index2 == 7)
            {
                HandleAttackCardToHero(players[move.currentPlayerIndex].board[move.index1], players[1 - move.currentPlayerIndex]);
            }
            else
            {
                HandleAttackCardToCard(players[move.currentPlayerIndex].board[move.index1], players[1 - move.currentPlayerIndex].board[move.index2]);
            }
        }
        else if (move.type == 'E')
        {
            ChangeTurn();
        }
    }

    public void DisplayTree(int currentDepth)
    {
        if (currentDepth > 2) { return; }
        string log = "";
        for (int i = 0; i <currentDepth; i++)
        {
            log += "   ";
        }
        log += Convert.ToString(currentDepth);
        try
        {
            log += $"{parentMove.type} {parentMove.index1} {parentMove.index2}";
        }
        catch
        {
            log += "root";
        }
        log += $"| {winCount} {numberOfVisits} \n";
        Debug.Log(log);
        if (children != null)
        {
            foreach (TreeSearchNode child in children)
            {
                child.DisplayTree(currentDepth++);
            }
        }
        //return log;
    }

    public void LogChildren()
    {
        foreach (TreeSearchNode node in children)
        {
            Debug.Log($"child move: {node.parentMove.type}, {node.parentMove.currentPlayerIndex}, {node.parentMove.index1}, {node.parentMove.index2}\n child wr {node.winrate()}");
        }
    }


}