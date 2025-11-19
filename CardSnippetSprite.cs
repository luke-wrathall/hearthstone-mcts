using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

public class CardSnippetSprite : MonoBehaviour
{
    public TMP_Text numberOfText;
    public TMP_Text manaText;
    public TMP_Text nameText;
    public int index;

    public void Populate(Card card, int number)
    {
        nameText.text = card.Name;
        manaText.text = Convert.ToString(card.ManaCost);
        numberOfText.text = Convert.ToString(number);
    }

    public void UpdateNumber(int number)
    {
        numberOfText.text = Convert.ToString(number);
    }
}
