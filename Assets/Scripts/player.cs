using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class player 
{
    
    public string playerdID;
    public string playerNAME;
    public List<title> hand;
    public player(List<title> hand) {
        this.hand = hand;
    }

    public void showHand()
    {
        string result = "HAND : ";
        for (int i = 0; i < this.hand.Count; i++)
        {
            result += this.hand[i].number + " ";
        }
        Debug.Log(result);

    }

    public title play_title(int index,bool human)
    {
        if (!human)
        {
            string t_num = this.hand[index].number;
            Sprite tempspr = this.hand[index].dmn_spr;
            title temp = this.hand[index];
            this.hand.RemoveAt(index);

            return temp;
        }
        else
        {
            string t_num = this.hand[index].number;
            Sprite tempspr = this.hand[index].dmn_spr;
            title temp = this.hand[index];
            this.hand.RemoveAt(index);
            return temp;
        }
    }
}
