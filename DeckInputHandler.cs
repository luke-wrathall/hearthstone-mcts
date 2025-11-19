using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class DeckInputHandler : MonoBehaviour
{
    public Camera _mainCamera;
    public CardManager cardManager;
    private void Awake()
    {
        _mainCamera = Camera.main;
    }
    public void OnClick(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        Debug.Log("click");
        var rayHit = Physics2D.GetRayIntersection(_mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()));

        if (!rayHit.collider) return;
        GameObject clickedObject = rayHit.collider.gameObject;
        Debug.Log(clickedObject.name);
        if (clickedObject.CompareTag("Card"))
        {
            cardManager.AddCard(clickedObject.GetComponent<CardSprite>().script);
        }
        if (clickedObject.CompareTag("LeftArrow"))
        {
            cardManager.HandleLeftArrowClick();
        }
        if (clickedObject.CompareTag("RightArrow"))
        {
            cardManager.HandleRightArrowClick();
        }
        if (clickedObject.CompareTag("CardSnippet"))
        {
            cardManager.HandleCardSnippetClick(clickedObject.GetComponent<CardSnippetSprite>().index);
        }
        if (clickedObject.CompareTag("Undo"))
        {
            cardManager.HandleUndo();
        }
        if (clickedObject.CompareTag("Save"))
        {
            cardManager.Save();
            SceneManager.LoadScene("DeckSelectionScene");
        }
    }
}
