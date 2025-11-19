using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Move
{
    public char type;
    // P is play card from hand, A is attack, E is end turn 
    public int index1;
    public int index2;
    public int currentPlayerIndex; 
    // index 7 will correspond to opponent hero 
    public Move(char Type, int CurrentPlayerIndex, int Index1 = -1, int Index2 = -1)
    {
        type = Type;
        index1 = Index1;
        index2 = Index2;
        currentPlayerIndex = CurrentPlayerIndex;
    }
}
