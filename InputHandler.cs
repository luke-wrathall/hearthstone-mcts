using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class InputHandler : MonoBehaviour
{ 
    public Camera _mainCamera;
    protected bool cardSelected;
    public Card selectedCard;
    public GameManager manager;

    public SpriteRenderer endTurnButton;


    protected float lastPressedTime = 0f;
    protected float coolDownTime = 1f;
    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    public void OnClick(InputAction.CallbackContext context) 
    {
        //Debug.Log("Click");
        if (!context.started) return;

        var rayHit = Physics2D.GetRayIntersection(_mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()));
        if (!rayHit.collider) return;
        GameObject clickedObject = rayHit.collider.gameObject;
        HandleClick(clickedObject);

    }

    public virtual void HandleClick(GameObject clickedObject)
    {
        if (clickedObject.CompareTag("Finish"))
        {
            SceneManager.LoadScene("StartScene");
        }
        if (manager.AnyWinner())
        {
            return;
        }
        //Debug.Log(clickedObject.name);
        if (clickedObject.CompareTag("Card"))
        {
            Card card = clickedObject.GetComponent<CardSprite>().script;
            if (manager.CheckCardAbleToBePlayed(card))
            {
                manager.HandleCardPlay(card);
                return;
            }
            if (manager.CheckCardAbleToBeAttacked(card))
            {
                //Debug.Log("Attackable");
                if (cardSelected)
                {
                    manager.HandleCardAttack(selectedCard, card);
                    cardSelected = false;
                    selectedCard = null;

                }
                Debug.Log($"Card selected: {cardSelected}");
                return;
            }
            if (card.CheckCanAttack())
            {
                selectedCard = null;
                selectedCard = card;
                cardSelected = true;
                Debug.Log($"Card selected: {cardSelected}");
                return;
            }
            //else
            //{
            //    Debug.Log($"Card selected: {cardSelected}");
            //}
            cardSelected = false;
            selectedCard = null;
            Debug.Log($"Card selected: {cardSelected}");
            return;


        }
        

        if (clickedObject.CompareTag("Hero"))
        {
            if (cardSelected)
            {
                //Debug.Log($"Card selected: {cardSelected}");
                if (manager.HandleHeroAttackAttempt(clickedObject.GetComponentInParent<PlayerSprite>().Index, selectedCard))
                {
                    //Debug.Log("Attacked Hero");
                    cardSelected = false;
                    selectedCard = null;
                }
            }

        }
        if (clickedObject.CompareTag("EndTurn"))
        {
            if (Time.time - lastPressedTime < coolDownTime)
            {
                return;
            }
            lastPressedTime = Time.time;
            cardSelected = false;
            selectedCard = null;
            manager.HandleEndTurn();
        }



        else
        {
            cardSelected = false;
            selectedCard = null;
            return;
        }
    }



}
