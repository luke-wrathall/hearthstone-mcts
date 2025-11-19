using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandlerPVP : InputHandler
{
    public override void HandleClick(GameObject clickedObject)
    {
        if (clickedObject.CompareTag("Start"))
        {
            manager.StartNextTurn();
        }
        base.HandleClick(clickedObject);
    }
}
