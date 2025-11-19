using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;

public class PlayerSprite : MonoBehaviour
{


    public SlotList ImportedHandSlotList;
    public Transform[] handSlotList;

    public SlotList ImportedBoardSlotList;
    public Transform[] boardSlotList;

    public TMP_Text manaText;
    public TMP_Text healthText;
    public TMP_Text deckCountText;

    public CardSprite[] board = new CardSprite[7];
    public CardSprite[] hand = new CardSprite[10];

    public CardSprite defaultCard;

    //has to be public so we can edit it in unity
    public int index;

    public float animationSpeed = 2f;

    public int Index { get => index; }




    void Awake()
    {
        handSlotList = new Transform[ImportedHandSlotList.slots.Count];
        ImportedHandSlotList.slots.CopyTo(handSlotList);
        boardSlotList = new Transform[ImportedBoardSlotList.slots.Count];
        ImportedBoardSlotList.slots.CopyTo(boardSlotList);
        

    }

    public void PlayCard(int handIndex, int boardIndex, bool ableToAttack)
    {
        CardSprite card = hand[handIndex];
        card.Show();
        StartCoroutine(AnimateMovement(card, boardSlotList[boardIndex].position));
        board[boardIndex] = card;
        card.UpdateHighlight(ableToAttack);
        UpdateHand(handIndex);
    }
    public void UpdateHand(int index)
    {
        hand[index] = null;
        while (index < hand.Length - 1 && hand[index + 1] != null)
        {
            index++;
            StartCoroutine(AnimateMovement(hand[index], handSlotList[index - 1].transform.position));
            hand[index - 1] = hand[index];


        }
        hand[index] = null;

    }

    public void DrawCard(Card card, int index)
    {
        CardSprite drawnCard = Instantiate(defaultCard);
        drawnCard.transform.position = handSlotList[index].transform.position;
        drawnCard.Populate(card);
        hand[index] = drawnCard;

        //not sure 
        drawnCard.transform.parent = this.transform;

    }


    public void DestroyCard(int index)
    {
        CardSprite toDestroy = board[index];
        toDestroy.gameObject.SetActive(false);
        board[index] = null;
        GameObject.Destroy(toDestroy.gameObject);
    }

    public void HideHand()
    {
        foreach (CardSprite card in hand)
        {
            if (card != null) card.Hide();
        }
    }
    public void ShowHand()
    {
        foreach (CardSprite card in hand)
        {
            if (card != null) card.Show();
        }
    }
    public void UpdateMana(int newAmountAvailable)
    {
        manaText.text = $"{newAmountAvailable}/{manaText.text.Split('/')[1]}";
    }

    public void UpdateMana(int newAmountAvailable, int newTotal )
    {
        manaText.text =  $"{newAmountAvailable}/{newTotal}";
    }


    public void UpdateDeckCount(int newAmount)
    {
        deckCountText.text = Convert.ToString(newAmount);
    }

    public void UpdateHeatlh(int newAmount)
    {
        healthText.text = Convert.ToString(newAmount);
    }

    public void UpdateBoardCardHighlights(bool ableToAttack)
    {
        foreach (CardSprite card in board)
        {
            if (card != null)
            {
                card.highlight.gameObject.SetActive(ableToAttack);
            }
        }
    }

    public System.Collections.IEnumerator AnimateMovement(CardSprite card, Vector2 endPoint, bool returnToInitial = false)
    {
        Vector2 startPoint = card.transform.position;
        float progress = 0;
        while (progress < 1.0f)
        {
            progress += Time.deltaTime * animationSpeed;
            card.transform.position = Vector2.Lerp(startPoint, endPoint, progress);
            yield return null;
        }
        if (returnToInitial )
        {
            progress = 0;
            while(progress < 1.0f)
            {
                progress += Time.deltaTime * animationSpeed;
                card.transform.position = Vector2.Lerp(endPoint, startPoint, progress);
                yield return null;
            }
            card.transform.position = startPoint;
        }
        else
        {
            card.transform.position = endPoint;
        }

    }

    public void ResetCardPositions()
    {
        for (int i =0; i<board.Length; i++)
        {
            if (board[i] != null)
            {
                board[i].transform.position = boardSlotList[i].transform.position;
            }
        }
    }
}
