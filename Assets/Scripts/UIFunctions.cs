using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIFunctions : MonoBehaviour
{
    bool isSettingsOpen = false;
    public void openSettings()
    {
        if (isSettingsOpen)
        {
            EventSystem.current.currentSelectedGameObject.transform.parent.localPosition = new Vector3(682, 0, 0);
            isSettingsOpen = false;
        }
        else
        {
            EventSystem.current.currentSelectedGameObject.transform.parent.localPosition = new Vector3(400, 0, 0);
            isSettingsOpen = true;
        }
        Debug.Log(EventSystem.current.currentSelectedGameObject.transform.parent.localPosition);
    }
    public void backToCharacter()
    {
        SceneManager.LoadScene("Character");
    }
    public void dominoClick()
    {

        /*domino main_mechanics_script = GameObject.FindGameObjectWithTag("MainMechanics").GetComponent<domino>();
        string number = EventSystem.current.currentSelectedGameObject.name;
        Debug.Log(number);

        bool can_destroy = main_mechanics_script.get_input(number);

        if (can_destroy)
        {
            main_mechanics_script.display_tile_holder(number, false);
            Destroy(EventSystem.current.currentSelectedGameObject);
        }*/
    }
}
