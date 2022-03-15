using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class title
{
    public string number = "";
    public Sprite dmn_spr;
    public Sprite test;
    public GameObject obj_title;
    public Button domino_img;
    public string obj_name="";
    public float rotation = 0f;
    public bool isDisplayed;
    public int sprite_index = 0;

    public title(string number, Sprite spr) {
        this.number = number;
        this.dmn_spr = spr;
        isDisplayed = false;
    }
    public void set_obj(Button btn, GameObject obj)
    {
        this.domino_img = btn;
        this.obj_title = obj;
    }
    public void reset()
    {
        number = "";
        dmn_spr = null;
        obj_title = null;
        obj_name = "";
        rotation = 0f;
        isDisplayed = false;
        sprite_index = 0;
    }
}
