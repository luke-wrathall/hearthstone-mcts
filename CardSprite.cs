using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Xml.Serialization;

public class CardSprite : MonoBehaviour
{
    public Sprite front;
    public Sprite back;
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer highlight;
    //this may be stupid
    public Card script;

    public TMP_Text attackText;
    public TMP_Text healthText;
    public TMP_Text effectText;
    public TMP_Text manaText;
    public TMP_Text nameText;


    private Dictionary<char, string> effectNameDict = new Dictionary<char, string>
    {
        {'W', "Windfury" },
        {'C', "Charge" },
        {'T', "Taunt" },
        {'P' , "Poisonous"},
        {'M', "Megawindfury" }
    };


    public void Populate(Card card)
    {
        nameText.text = card.Name;
        UpdateStats(card);
        script = card;
        tag = "Card";
        name = card.Name;
        effectText.text = DescriptionFromEffects(card.Effects);
    }


    public void Populate(string name, int attack, int health, int manaCost, List<char> effects)
    {
        nameText.text = name;
        attackText.text = Convert.ToString(attack);
        healthText.text = Convert.ToString(health);
        manaText.text = Convert.ToString(manaCost);
        effectText.text = DescriptionFromEffects(effects);

    }

    public void UpdateStats(Card card)
    {
        attackText.text = Convert.ToString(card.Attack);
        healthText.text = Convert.ToString(card.Health);
        manaText.text = Convert.ToString(card.ManaCost);

    }


    public void UpdateHighlight (bool ableToAttack)
    {
        highlight.gameObject.SetActive(ableToAttack);
    }


    public  void Hide()
    {
        spriteRenderer.sprite = back;
        transform.localScale = new Vector3(1.0f, 1.0f, 2.0f);
        attackText.enabled = false;
        healthText.enabled = false;
        effectText.enabled = false;
        manaText.enabled = false;
        nameText.enabled = false;
    }

    public void Show()
    {
        spriteRenderer.sprite = front;
        transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        attackText.enabled = true;
        healthText.enabled = true;
        effectText.enabled = true;
        manaText.enabled = true;
        nameText.enabled = true;
    }

    private string DescriptionFromEffects(List<char> effects)
    {
        string s = "";
        foreach (char c in effects) { s += effectNameDict[c] + " "; }
        s.TrimEnd(' ');
        return s;
    }

}
