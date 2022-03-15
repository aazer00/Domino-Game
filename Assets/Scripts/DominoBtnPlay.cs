using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DominoBtnPlay : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private RectTransform rect_transform;
    private Vector3 initial_rect_transform_position;
    public domino main_mechanics_script;

    private void Awake()
    {
        rect_transform = GetComponent<RectTransform>();
        
        main_mechanics_script = GameObject.FindGameObjectWithTag("MainMechanics").GetComponent<domino>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (! (GetComponent<Image>().color == Color.gray))
        {
            main_mechanics_script.display_tile_holder(name, true);
            initial_rect_transform_position = rect_transform.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        
        if (!(GetComponent<Image>().color == Color.gray))
        {
            rect_transform.position = eventData.position;
            Debug.Log(transform.position.y);
        }

        if (transform.position.x < 1920 / 2)
        {
            Color temp_color = GameObject.Find("Tile Holder Right").GetComponent<SpriteRenderer>().color;
            temp_color.a = 0.7f;
            GameObject.Find("Tile Holder Right").GetComponent<SpriteRenderer>().color = temp_color;

            temp_color = GameObject.Find("Tile Holder Left").GetComponent<SpriteRenderer>().color;
            temp_color.a = 1f;
            GameObject.Find("Tile Holder Left").GetComponent<SpriteRenderer>().color = temp_color;

        }
        else
        {
            Color temp_color = GameObject.Find("Tile Holder Left").GetComponent<SpriteRenderer>().color;
            temp_color.a = 0.7f;
            GameObject.Find("Tile Holder Left").GetComponent<SpriteRenderer>().color = temp_color;

            temp_color = GameObject.Find("Tile Holder Right").GetComponent<SpriteRenderer>().color;
            temp_color.a = 1f;
            GameObject.Find("Tile Holder Right").GetComponent<SpriteRenderer>().color = temp_color;
        }
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        

        if (!(GetComponent<Image>().color == Color.gray))
        {
            if (transform.position.y < 150)
            {
                rect_transform.position = initial_rect_transform_position;
                main_mechanics_script.display_tile_holder(name, false);
                return;
            } 

            string side = "left";

            if (transform.position.x > 1920 / 2) side = "right";

            bool can_destroy = main_mechanics_script.get_input(side, name);

            if (can_destroy)
            {
                main_mechanics_script.display_tile_holder(name, false);
                Destroy(EventSystem.current.currentSelectedGameObject);
            }
        }
    }

    void Update()
    {
        
    }
}
