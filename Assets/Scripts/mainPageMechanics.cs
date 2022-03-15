using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class mainPageMechanics : MonoBehaviour
{
    [SerializeField] GameObject game_type;
    [SerializeField] GameObject number_of_players_panel;
    [SerializeField] GameObject game_mode;
    [SerializeField] GameObject blur_background;

    int number_of_players;

    public void Start()
    {
        number_of_players = 4;
    }

    public void startOnlineGame()
    {
        SceneManager.LoadScene("Room");
    }
    public void startSinglePlayerGame()
    {
        SceneManager.LoadScene("SinglePlayer");
    }

    public void openGameType()
    {
        SceneManager.LoadScene("GameType");
    }

    public void backToLogin()
    {
        SceneManager.LoadScene("login");
    }

    public void backToCharacter()
    {
        SceneManager.LoadScene("Character");
    }

    public void openNumberofPlayersPanel()
    {
        game_type.SetActive(false);
        number_of_players_panel.SetActive(true);
    }

    public void openGameMode()
    {
        switch (EventSystem.current.currentSelectedGameObject.name)
        {
            case "2Players":
                number_of_players = 2;
                break;
            case "3Players":
                number_of_players = 3;
                break;
            case "4Players":
                number_of_players = 4;
                break;
        }
        SinglePlayerGameData.singleplayer_number_of_players = number_of_players;
        blur_background.SetActive(true);
        game_mode.SetActive(true);
    }

    public void closeGameMode()
    {
        game_mode.SetActive(false);
        blur_background.SetActive(false);
    }
}
