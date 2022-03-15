using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class domino : MonoBehaviour
{
    [SerializeField] static List<title> titles = new List<title>();
    [SerializeField] static player p1;
    [SerializeField] static player[] bots;
    [SerializeField] static List<title> game_line = new List<title>();
    [SerializeField] GameObject title_prefab;
    [SerializeField] Sprite[] domino_sprites;
    [SerializeField] Sprite test;
    [SerializeField] Image[] player_pass_images;
    [SerializeField] GameObject game_over_UI;
    [SerializeField] Image blur_background;
    [SerializeField] GameObject center_of_titles;
    [SerializeField] Button domino_btn;
    [SerializeField] GameObject player_hand_horizontal_layout;
    [SerializeField] Image empty_title;
    [SerializeField] GameObject bazar;
    [SerializeField] GameObject tile_holder_left;
    [SerializeField] GameObject tile_holder_right;
    [SerializeField] GameObject main_player_data;
    [SerializeField] GameObject other_players_data;
    

    float center_of_titles_X = 960;
    float center_of_titles_Y = 540;
    float center_of_titles_X_scale = 350;

    public UIFunctions UI_functions;
    bool human_player_turn = true;
    bool isGameActive = false;

    int active_player = -1;
    DateTime turn_start_time;
    int bot_turn_interval = 1;
    int human_turn_interval = 3;
    int number_of_passes = 0;
    int pass_wait_interval = 2;
    public int game_blocker = -1;

    static int numberOFLeftTiles = 0;
    static int numberOFRightTiles = 0;

    private float gameline_left_X;
    private float gameline_right_X;
    private float gameline_up_Y;
    private float gameline_down_Y;


    public static bool isOnline = false;
    public int number_of_players;

    int[] Scores = new int[] { 0, 0, 0, 0 };
    public int[] total_scores = new int[] { 0, 0, 0, 0 };

    public float time_bar_speed;
    public int number_of_rounds = 1;

    int previous_winner_index = -1;

    string first_playable_twin;

    protected void Start()
    {
        // Initialization of Variables
        number_of_players = SinglePlayerGameData.singleplayer_number_of_players != 0 ? SinglePlayerGameData.singleplayer_number_of_players : 4;

        UI_functions = GameObject.FindGameObjectWithTag("UIFunctions").GetComponent<UIFunctions>();

        titles = new List<title>();
        bots = new player[number_of_players - 1];
        game_line = new List<title>();
        human_player_turn = true;
        isGameActive = false;

        active_player = -1;
        bot_turn_interval = 1;
        human_turn_interval = 10;
        pass_wait_interval = 2;

        numberOFLeftTiles = 0;
        numberOFRightTiles = 0;

        gameline_left_X = 1920 / 2;
        gameline_right_X = 1920 / 2;
        gameline_up_Y = 1080 / 2;
        gameline_down_Y = 1080 / 2;

        Scores = new int[] { 0, 0, 0, 0 };
        total_scores = new int[] { 0, 0, 0, 0 };

        time_bar_speed = 0.05f;

        // create titles
        int index_spr = 0;
        for (int i = 0; i <= 6; i++)
        {
            for (int j = i; j <= 6; j++)
            {
                title temp = new title(i + "#" + j, domino_sprites[index_spr]); index_spr++;
                temp.test = test;
                Button dmn_btn = domino_btn;
                GameObject t_obj = title_prefab;
                temp.obj_name = i + "#" + j;
                temp.set_obj(dmn_btn, t_obj);
                titles.Add(temp);
            }

        }

        // making hands
        List<title> temp_hand = new List<title>();

        for (int i = 0; i < 7; i++)
        {
            int index = UnityEngine.Random.Range(0, titles.Count);
            temp_hand.Add(titles[index]);
            titles.RemoveAt(index);
        }

        // prepearing hand for player
        p1 = new player(temp_hand);
        p1.showHand();
        display_player_hand();

        for (int j = 0; j < number_of_players-1; j++)
        {

            temp_hand = new List<title>();

            for (int i = 0; i < 7; i++)
            {
                int index = UnityEngine.Random.Range(0, titles.Count);
                temp_hand.Add(titles[index]);
                titles.RemoveAt(index);
            }
            bots[j] = new player(temp_hand);
            bots[j].showHand();
        }

        // check who starts first
        int which_Bot_Has_Ones = check_ones();
        //Debug.Log(which_Bot_Has_Ones);
        if (which_Bot_Has_Ones != -1)
        {
            active_player = which_Bot_Has_Ones;
        }
        else
        {
            active_player = number_of_players-1;
        }
        game_blocker = active_player;
        // Show titles in game line
        //show_consoleGameLine();

        // Define first start time for first turn
        turn_start_time = DateTime.Now;

        // Funcntions that must be initiated with Start
        disableAllNotVisibleUI();
        fillBazar();
        showPlayerData();

        isGameActive = true;
    }

    void Update()
    {
        if (isGameActive == true)
        {
            if (active_player != number_of_players - 1) 
            {
                if (DateTime.Now >= turn_start_time.AddSeconds(bot_turn_interval))
                {
                    other_players_data.transform.GetChild(active_player).GetChild(0).
                        GetChild(0).GetChild(0).GetChild(0).GetChild(0).
                        GetComponent<Image>().fillAmount -= time_bar_speed * Time.deltaTime*2;

                    int previous_player = active_player == 0 ? number_of_players - 1 : active_player - 1;                                                  

                    disablePassUI(previous_player);

                    if (DateTime.Now >= turn_start_time.AddSeconds(1))
                    {                      
                        if (number_of_passes == number_of_players) gameOver();

                        if (!cpuPlays(active_player))
                        {
                            if (!getAvailableTitle(active_player))
                            {
                                number_of_passes++;
                                showPassUI(active_player);
                            }
                            else
                            {
                                number_of_passes = 0;
                            }
                        }
                        else
                        {
                            number_of_passes = 0;
                        }
                        other_players_data.transform.GetChild(active_player).GetChild(0).
                        GetChild(0).GetChild(0).GetChild(0).GetChild(0).
                        GetComponent<Image>().fillAmount = 1;

                        active_player++;
                        if (active_player > number_of_players - 1)
                        {
                            active_player = 0;
                        }
                        turn_start_time = DateTime.Now;

                        
                    }
                }
            }
            else
            {
                main_player_data.transform.GetChild(0).gameObject.SetActive(true);
                if (DateTime.Now <= turn_start_time.AddSeconds(human_turn_interval))
                {
                    main_player_data.transform.GetChild(0).GetComponent<Image>().fillAmount -= time_bar_speed * Time.deltaTime;
                    disableHandTiles();

                    if (DateTime.Now >= turn_start_time.AddSeconds(1))
                    {
                        //Debug.Log(DateTime.Now);
                        if (number_of_passes == number_of_players) gameOver();

                        int previous_player = active_player == 0 ? number_of_players - 1 : active_player - 1;
                        disablePassUI(previous_player);

                        if (!checkPlayerHand())
                        {
                            human_turn_interval = 100;
                            if (titles.Count == 0)
                            {
                                bazar.SetActive(false);
                                number_of_passes++;
                                showPassUI(active_player);
                                active_player = 0;
                                turn_start_time = DateTime.Now;
                            }
                            else
                            {
                                bazar.SetActive(true);
                                getAvailableTitle(active_player);
                                turn_start_time = DateTime.Now;
                            }
                        }
                        else
                        {
                            bazar.SetActive(false);
                            human_turn_interval = 20;
                            pass_wait_interval = 2;
                        }
                    }
                }
                else
                {
                    bazar.SetActive(false);
                    string playable_title_idx = autoplay();
                    
                    bool can_destroy = get_input("left", playable_title_idx);
                    Debug.Log(playable_title_idx);
                    if (can_destroy)
                    {
                        Destroy(GameObject.Find("Horizontal Layout 2 1/"+playable_title_idx));
                    }
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                for (int i = 0; i < number_of_players; i++)
                {
                    if (total_scores[i] > 100)
                    {
                        UI_functions.backToCharacter();
                    }
                    restartRound();
                }
            }
        }
    }

    // human player play title
    static bool play(int index, string side)
    {
        if (game_line.Count == 0)
        {
            title title_played_by_human = null;
            title_played_by_human = p1.play_title(index - 1, true);
            game_line.Add(title_played_by_human);
        }
        else
        {
            int gl_left = int.Parse(game_line[0].number[0].ToString());
            int gl_right = int.Parse(game_line[game_line.Count - 1].number[2].ToString());

            int played_left = int.Parse(p1.hand[index - 1].number[0].ToString());
            int played_right = int.Parse(p1.hand[index - 1].number[2].ToString());


            if ((gl_right == played_left && gl_left == played_right) ||
                (gl_right == played_right && gl_left == played_left) ||
                (gl_right == played_right && gl_left == played_right) ||
                (gl_right == played_left && gl_left == played_left) )
            {
                if (side == "left")
                {
                    if (gl_left == played_right)
                    {
                        title title_played_by_human = null;
                        title_played_by_human = p1.play_title(index - 1, true);
                        game_line.Insert(0, title_played_by_human);
                        numberOFLeftTiles++;
                    }
                    else if (gl_left == played_left)
                    {
                        title title_played_by_human = null;
                        title_played_by_human = p1.play_title(index - 1, true);
                        title_played_by_human.number = Reverse(title_played_by_human.number);
                        title_played_by_human.rotation = 180f;
                        game_line.Insert(0, title_played_by_human);
                        numberOFLeftTiles++;
                    }
                }
                else
                {
                    if (gl_right == played_left)
                    {
                        title title_played_by_human = null;
                        title_played_by_human = p1.play_title(index - 1, true);
                        game_line.Add(title_played_by_human);
                        numberOFRightTiles++;
                    }

                    else if (gl_right == played_right)
                    {
                        title title_played_by_human = null;
                        title_played_by_human = p1.play_title(index - 1, true);
                        title_played_by_human.number = Reverse(title_played_by_human.number);
                        title_played_by_human.rotation = 180f;
                        game_line.Add(title_played_by_human);
                        numberOFRightTiles++;
                    }
                }
            }
            else
            {
                if (gl_right == played_left)
                {
                    title title_played_by_human = null;
                    title_played_by_human = p1.play_title(index - 1, true);
                    game_line.Add(title_played_by_human);
                    numberOFRightTiles++;
                }
                else if (gl_left == played_right)
                {
                    title title_played_by_human = null;
                    title_played_by_human = p1.play_title(index - 1, true);
                    game_line.Insert(0, title_played_by_human);
                    numberOFLeftTiles++;
                }
                else if (gl_right == played_right)
                {
                    title title_played_by_human = null;
                    title_played_by_human = p1.play_title(index - 1, true);
                    title_played_by_human.number = Reverse(title_played_by_human.number);
                    title_played_by_human.rotation = 180f;
                    game_line.Add(title_played_by_human);
                    numberOFRightTiles++;

                }
                else if (gl_left == played_left)
                {
                    title title_played_by_human = null;
                    title_played_by_human = p1.play_title(index - 1, true);
                    title_played_by_human.number = Reverse(title_played_by_human.number);
                    title_played_by_human.rotation = 180f;
                    game_line.Insert(0, title_played_by_human);
                    numberOFLeftTiles++;
                }
                else
                {
                    return false;
                }
            }
            
        }

        return true;
    }


    //get input from human player
    public bool get_input(string side, string number)
    {
        if (active_player != number_of_players - 1 || isGameActive == false) return false;
        int idx;
        for (idx = 0; idx < p1.hand.Count; idx++)
        {
            if (p1.hand[idx].number == number)
                break;
        }

        if (!play(idx + 1, side)) return false;

        p1.showHand();

        //show gameline
        displayGameLine();
        check_win(p1);

        // Variable modification after human player turn finish

        disableHandTiles(true);
        main_player_data.transform.GetChild(0).gameObject.SetActive(false);
        main_player_data.transform.GetChild(0).GetComponent<Image>().fillAmount = 1;
        turn_start_time = DateTime.Now;
        active_player = 0;
        number_of_passes = 0;
        return true;
    }

    //reverse given string
    public static string Reverse(string s)
    {
        char[] charArray = s.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }

    // bot play turn
    public bool cpuPlays(int index)
    {
        bool is_cpu_player_has_tile = false;
        //Debug.Log("Bot -> " + index.ToString() + " Start");
        int j = index;
        int idx = j;
        int t_index = -1;

        title cpu_playable_tile = null;

        foreach (title t in bots[idx].hand)
        {
            t_index++;
            if (game_line.Count == 0)
            {
                if (number_of_rounds == 1)
                {
                    if (t.number == first_playable_twin)
                    {
                        title title_played_by_cpu = null;
                        title_played_by_cpu = bots[idx].play_title(t_index, true);
                        game_line.Add(title_played_by_cpu);
                        is_cpu_player_has_tile = true;
                        cpu_playable_tile = title_played_by_cpu;
                        break;
                    }
                }
                else
                {
                    title title_played_by_cpu = null;
                    title_played_by_cpu = bots[idx].play_title(t_index, true);
                    game_line.Add(title_played_by_cpu);
                    is_cpu_player_has_tile = true;
                    cpu_playable_tile = title_played_by_cpu;
                    break;
                }
                    
            }
            else
            {

                int gl_left = int.Parse(game_line[0].number[0].ToString());
                int gl_right = int.Parse(game_line[game_line.Count - 1].number[2].ToString());

                int played_left = int.Parse(t.number[0].ToString());
                int played_right = int.Parse(t.number[2].ToString());

                if (gl_right == played_left)
                {
                    title title_played_by_cpu = null;
                    title_played_by_cpu = bots[idx].play_title(t_index, true);
                    game_line.Add(title_played_by_cpu);
                    numberOFRightTiles++;
                    is_cpu_player_has_tile = true;
                    cpu_playable_tile = title_played_by_cpu;
                    break;
                }
                else if (gl_left == played_right)
                {
                    title title_played_by_cpu = null;
                    title_played_by_cpu = bots[idx].play_title(t_index, true);
                    game_line.Insert(0, title_played_by_cpu);
                    numberOFLeftTiles++;
                    is_cpu_player_has_tile = true;
                    cpu_playable_tile = title_played_by_cpu;
                    break;
                }
                else if (gl_right == played_right)
                {
                    title title_played_by_cpu = null;
                    title_played_by_cpu = bots[idx].play_title(t_index, true);
                    title_played_by_cpu.number = Reverse(title_played_by_cpu.number);
                    title_played_by_cpu.rotation = 180f;
                    game_line.Add(title_played_by_cpu);
                    numberOFRightTiles++;
                    is_cpu_player_has_tile = true;
                    cpu_playable_tile = title_played_by_cpu;
                    break;
                }
                else if (gl_left == played_left)
                {
                    title title_played_by_cpu = null;
                    title_played_by_cpu = bots[idx].play_title(t_index, true);
                    title_played_by_cpu.number = Reverse(title_played_by_cpu.number);
                    title_played_by_cpu.rotation = 180f;
                    game_line.Insert(0, title_played_by_cpu);
                    numberOFLeftTiles++;
                    is_cpu_player_has_tile = true;
                    cpu_playable_tile = title_played_by_cpu;
                    break;
                }
            }
        }
        if (is_cpu_player_has_tile == false)
        {
            return false;
        }
        else
        {
            other_players_data.transform.GetChild(index).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>().text = bots[index].hand.Count.ToString();

            displayGameLine();

            if (cpu_playable_tile.obj_name[0] != cpu_playable_tile.obj_name[2])
            {
                game_blocker = active_player;
            }
            
            //show_consoleGameLine();
            check_win(bots[j]);


            return true;
        }

    }

    //check if selected player played all hiss titles
    public void check_win(player pla)
    {
        if (pla.hand.Count == 0)
        {
            Debug.Log("GAME OVER");
            gameOver();
        }
    }

    //Game Over function
    public void gameOver()
    {
        isGameActive = false;
        calculateScores();
        int sum_of_scores = 0;
        int min_index = 0;
        for (int i = 0; i < number_of_players; i++)
        {
            sum_of_scores += Scores[i];
            if (Scores[i] < Scores[min_index])
            {
                min_index = i;
            }
        }
        sum_of_scores -= Scores[min_index];
        total_scores[min_index] += sum_of_scores;
        previous_winner_index = min_index; 
        min_index = (min_index + 1) % number_of_players;


        showGameOverUI(min_index, sum_of_scores);
    }

    // check which player has 1#1 and plays first at the first round
    public int check_ones()
    {


        for (int searched_twin = 1; searched_twin < 6; searched_twin++)
        {
            for (int j = 0; j < p1.hand.Count; j++)
            {
                if (p1.hand[j].number == searched_twin.ToString() + "#" + searched_twin.ToString())
                {
                    first_playable_twin = searched_twin.ToString() + "#" + searched_twin.ToString();
                    return number_of_players-1;
                }
            }

            for (int i = 0; i < bots.Length; i++)
            {
                //Debug.Log(i);
                Debug.Log(bots[i].hand);
                for (int j = 0; j < bots[i].hand.Count; j++)
                {
                    if (bots[i].hand[j].number == searched_twin.ToString() + "#" + searched_twin.ToString())
                    {
                        first_playable_twin = searched_twin.ToString() + "#" + searched_twin.ToString();
                        return i;
                    }
                }
            }
        }

       
        return -1;
    }

    //calculate scores of each player at the end of each round
    public void calculateScores()
    {
        for (int i = 0; i < p1.hand.Count; i++)
        {
            Scores[number_of_players - 1] += int.Parse(p1.hand[i].number[0].ToString()) + int.Parse(p1.hand[i].number[2].ToString());

        }

        for (int i = 0; i < bots.Length; i++)
        {
            Debug.Log(bots[i].hand);
            for (int j = 0; j < bots[i].hand.Count; j++)
            {
                Scores[i] += int.Parse(bots[i].hand[j].number[0].ToString()) + int.Parse(bots[i].hand[j].number[2].ToString());
            }
        }
    }

   
    public void display_player_hand(int start_index = 0)
    {
        // initiate titles in players hand as an object
        float hand_X = 750f;
        float hand_Y = 203f;
        float hand_Z = -2f;
        float distance_between_titles = 110f;
        Vector3 title_size = new Vector3(8f, 8f, 8f);

        for (int i = start_index; i < p1.hand.Count; i++)
        {
            p1.hand[i].domino_img = Instantiate(p1.hand[i].domino_img, new Vector3(hand_X, hand_Y, hand_Z), Quaternion.identity);
            p1.hand[i].domino_img.GetComponent<Image>().sprite = p1.hand[i].dmn_spr;
            p1.hand[i].domino_img.transform.parent = player_hand_horizontal_layout.transform;
            p1.hand[i].domino_img.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

            //p1.hand[i].obj_title.transform.localScale = title_size;
            p1.hand[i].domino_img.name = p1.hand[i].obj_name;
            hand_X = hand_X + distance_between_titles;
        }
    }

    // show titles in game line
    float dX = 140f;
    float twin_dX = 110f;
    float twin_backword = 30f;
    float hor_to_vert_dX = 35f;
    public void displayGameLine()
    {
        Vector3 line_title_size = new Vector3(6f, 6f, 6f);
        //Orta das
        float Mx = 1920 / 2;
        float My = 1080 / 2;
        title Mt = game_line[numberOFLeftTiles];
        if (Mt.isDisplayed == false)
        {
            Mt.obj_title = Instantiate(Mt.obj_title, new Vector3(Mx, My, -2), Quaternion.identity);
            Mt.obj_title.GetComponent<SpriteRenderer>().sprite = Mt.dmn_spr;
            Mt.obj_title.name = Mt.obj_name;
            Mt.obj_title.transform.localScale = line_title_size;
            if (Mt.obj_name[0] != Mt.obj_name[2])
            {
                Mt.rotation += 90f;
            }
            Mt.obj_title.transform.Rotate(0, 0, Mt.rotation);
            Mt.isDisplayed = true;

            if (Mt.obj_name[0] == Mt.obj_name[2])
            {
                gameline_left_X -= twin_dX;
                gameline_right_X += twin_dX;
            }
            else
            {
                gameline_left_X -= dX;
                gameline_right_X += dX;
            }
                
            
        }

        //Left Side
        for (int i = 0; i < numberOFLeftTiles; i++)
        {
            if (numberOFLeftTiles - i <= 5)
            {
                float x = -1 * (numberOFLeftTiles - i) * dX + Mx;
                float y = My;
                title t = game_line[i];
                if (t.isDisplayed == false)
                {
                    if (t.obj_name[0] == t.obj_name[2])
                        gameline_left_X = gameline_left_X + twin_backword;

                    t.obj_title = Instantiate(t.obj_title, new Vector3(gameline_left_X, gameline_down_Y, -2), Quaternion.identity);
                    t.obj_title.GetComponent<SpriteRenderer>().sprite = t.dmn_spr;
                    t.obj_title.name = t.obj_name;
                    t.obj_title.transform.localScale = line_title_size;
                    if (t.obj_name[0] != t.obj_name[2])
                    {
                        t.rotation += 90f;
                    }
                    t.obj_title.transform.Rotate(0, 0, t.rotation);


                    t.isDisplayed = true;

                    center_of_titles_X -= dX / 2;

                    if (numberOFLeftTiles - i != 5)
                    {
                        if (t.obj_name[0] != t.obj_name[2])
                            gameline_left_X = gameline_left_X - dX;
                        else
                            gameline_left_X = gameline_left_X - twin_dX;
                    }
                    else
                    {
                        if (t.obj_name[0] != t.obj_name[2])
                        {
                            gameline_left_X = gameline_left_X - hor_to_vert_dX;
                            gameline_down_Y = gameline_down_Y - twin_dX;
                        }
                        else
                        {
                            gameline_down_Y = gameline_down_Y - dX;
                        }
                    }
                }
            }
            else if (numberOFLeftTiles - i <= 7)
            {
                float x = -5 * dX + Mx;
                float y = -1 * dX * (numberOFLeftTiles - i - 5) + My;
                title t = game_line[i];
                if (t.isDisplayed == false)
                {
                    if (t.obj_name[0] == t.obj_name[2])
                        gameline_down_Y = gameline_down_Y + twin_backword;

                    t.obj_title = Instantiate(t.obj_title, new Vector3(gameline_left_X, gameline_down_Y, -2), Quaternion.identity);
                    t.obj_title.GetComponent<SpriteRenderer>().sprite = t.dmn_spr;
                    t.obj_title.name = t.obj_name;
                    t.obj_title.transform.localScale = line_title_size;
                    if (t.obj_name[0] == t.obj_name[2])
                    {
                        t.rotation += 90f;
                    }
                    if (t.obj_name[0] != t.obj_name[2])
                    {
                        t.rotation += 180f;
                    }
                    t.obj_title.transform.Rotate(0, 0, t.rotation);


                    t.isDisplayed = true;
                    center_of_titles_Y -= dX / 2;

                    if (numberOFLeftTiles - i != 7)
                    {
                        if (t.obj_name[0] != t.obj_name[2])
                            gameline_down_Y = gameline_down_Y - dX;
                        else
                            gameline_down_Y = gameline_down_Y - twin_dX;
                    }
                    else
                    {
                        if (t.obj_name[0] != t.obj_name[2])
                        {
                            gameline_left_X = gameline_left_X + twin_dX;
                            gameline_down_Y = gameline_down_Y - twin_backword;
                        }
                        else
                        {
                            gameline_left_X = gameline_left_X + dX;
                        }
                    }
                }
            }
            else
            {
                float x = -5 * dX + (numberOFLeftTiles - 7 - i) * dX + Mx;
                float y = -2 * dX + My;
                title t = game_line[i];
                if (t.isDisplayed == false)
                {
                    if (t.obj_name[0] == t.obj_name[2])
                        gameline_left_X = gameline_left_X - twin_backword;

                    t.obj_title = Instantiate(t.obj_title, new Vector3(gameline_left_X, gameline_down_Y, -2), Quaternion.identity);
                    t.obj_title.GetComponent<SpriteRenderer>().sprite = t.dmn_spr;
                    t.obj_title.name = t.obj_name;
                    t.obj_title.transform.localScale = line_title_size;
                    if (t.obj_name[0] != t.obj_name[2])
                    {
                        t.rotation += 270f;
                    }

                    t.obj_title.transform.Rotate(0, 0, t.rotation);


                    t.isDisplayed = true;

                    if (t.obj_name[0] != t.obj_name[2])
                        gameline_left_X = gameline_left_X + dX;
                    else
                        gameline_left_X = gameline_left_X + twin_dX;
                }
            }
        }

        //Right Side
        for (int i = 0; i < numberOFRightTiles; i++)
        {
            if (i <= 4)
            {
                float x = (i + 1) * dX + Mx;
                float y = My;
                title t = game_line[numberOFLeftTiles + i + 1];
                if (t.isDisplayed == false)
                {
                    if (t.obj_name[0] == t.obj_name[2])
                        gameline_right_X = gameline_right_X - twin_backword;

                    t.obj_title = Instantiate(t.obj_title, new Vector3(gameline_right_X, gameline_up_Y, -2), Quaternion.identity);
                    t.obj_title.GetComponent<SpriteRenderer>().sprite = t.dmn_spr;
                    t.obj_title.name = t.obj_name;
                    t.obj_title.transform.localScale = line_title_size;
                    if (t.obj_name[0] != t.obj_name[2])
                    {
                        t.rotation += 90f;
                    }
                    t.obj_title.transform.Rotate(0, 0, t.rotation);

                    t.isDisplayed = true;

                    center_of_titles_X += dX / 2;

                    if (i != 4)
                    {
                        if (t.obj_name[0] != t.obj_name[2])
                            gameline_right_X = gameline_right_X + dX;
                        else
                            gameline_right_X = gameline_right_X + twin_dX;
                    }
                    else
                    {
                        if (t.obj_name[0] != t.obj_name[2])
                        {
                            gameline_right_X = gameline_right_X + hor_to_vert_dX;
                            gameline_up_Y = gameline_up_Y + twin_dX;
                        }
                        else
                        {
                            gameline_up_Y = gameline_up_Y + dX;
                        }
                    }
                }

            }
            else if (i <= 6)
            {
                float x = 5 * dX + Mx;
                float y = (i - 4) * dX + My;
                title t = game_line[numberOFLeftTiles + i + 1];
                if (t.isDisplayed == false)
                {
                    if (t.obj_name[0] == t.obj_name[2])
                        gameline_up_Y = gameline_up_Y - twin_backword;

                    t.obj_title = Instantiate(t.obj_title, new Vector3(gameline_right_X, gameline_up_Y, -2), Quaternion.identity);
                    t.obj_title.GetComponent<SpriteRenderer>().sprite = t.dmn_spr;
                    t.obj_title.name = t.obj_name;
                    t.obj_title.transform.localScale = line_title_size;
                    if (t.obj_name[0] == t.obj_name[2])
                    {
                        t.rotation += 90f;
                    }
                    if (t.obj_name[0] != t.obj_name[2])
                    {
                        t.rotation += 180f;
                    }
                    t.obj_title.transform.Rotate(0, 0, t.rotation);


                    t.isDisplayed = true;
                    center_of_titles_Y += dX / 2;

                    if (i != 6)
                    {
                        if (t.obj_name[0] != t.obj_name[2])
                            gameline_up_Y = gameline_up_Y + dX;
                        else
                            gameline_up_Y = gameline_up_Y + twin_dX;
                    }
                    else
                    {
                        if (t.obj_name[0] != t.obj_name[2])
                        {
                            gameline_right_X = gameline_right_X - twin_dX;
                            gameline_up_Y = gameline_up_Y + twin_backword;
                        }
                        else
                        {
                            gameline_right_X = gameline_right_X - dX;
                        }
                    }
                }

            }
            else
            {
                float x = 5 * dX - (i - 7) * dX + Mx;
                float y = 2 * dX + My;
                title t = game_line[numberOFLeftTiles + i + 1];
                if (t.isDisplayed == false)
                {
                    if (t.obj_name[0] == t.obj_name[2])
                        gameline_right_X = gameline_right_X + twin_backword;

                    t.obj_title = Instantiate(t.obj_title, new Vector3(gameline_right_X, gameline_up_Y, -2), Quaternion.identity);
                    t.obj_title.GetComponent<SpriteRenderer>().sprite = t.dmn_spr;
                    t.obj_title.name = t.obj_name;
                    t.obj_title.transform.localScale = line_title_size;
                    if (t.obj_name[0] != t.obj_name[2])
                    {
                        t.rotation += 270f;
                    }
                    t.obj_title.transform.Rotate(0, 0, t.rotation);

                    t.isDisplayed = true;

                    if (t.obj_name[0] != t.obj_name[2])
                        gameline_right_X = gameline_right_X - dX;
                    else
                        gameline_right_X = gameline_right_X - twin_dX;

                }

            }
        }

        if (game_line.Count > 16)
        {
            center_of_titles_X_scale = 770;
        }
        else
        if (game_line.Count > 13)
        {
            center_of_titles_X_scale = 600;
        }
        else
        if (game_line.Count > 9)
        {
            center_of_titles_X_scale = 550;
        }
        else
        if (game_line.Count > 5)
        {
            center_of_titles_X_scale = 450;
        }
        
        center_of_titles.transform.position = new Vector3(center_of_titles_X, center_of_titles_Y, -10);
        center_of_titles.transform.localScale = new Vector3(center_of_titles_X_scale, 1, 1);

        //display_tile_holder();
    }

    public void display_tile_holder(string number, bool invisible)
    {
        if (game_line.Count == 0) return;

        int gl_left = int.Parse(game_line[0].number[0].ToString());
        int gl_right = int.Parse(game_line[game_line.Count - 1].number[2].ToString());

        bool left_is_available = false;
        bool right_is_available = false;

        int played_left = int.Parse(number[0].ToString());
        int played_right = int.Parse(number[2].ToString());

        if (gl_right == played_left || gl_right == played_right)
        {
            right_is_available = true;
        }

        if (gl_left == played_left || gl_left == played_right)
        {
            left_is_available = true;
        }



        if (invisible == false)
        {
            tile_holder_left.GetComponent<SpriteRenderer>().enabled = false;
            tile_holder_right.GetComponent<SpriteRenderer>().enabled = false;
        }
        else
        {
            if(left_is_available)
                tile_holder_left.GetComponent<SpriteRenderer>().enabled = true;

            if(right_is_available)
                tile_holder_right.GetComponent<SpriteRenderer>().enabled = true;

            if (left_is_available == true && right_is_available == false)
            {
                tile_holder_left.GetComponent<SpriteRenderer>().color = Color.green;
            }

            if (right_is_available == true && left_is_available == false)
            {
                tile_holder_right.GetComponent<SpriteRenderer>().color = Color.green;
            }

            if (left_is_available == true && right_is_available == true)
            {
                tile_holder_left.GetComponent<SpriteRenderer>().color = Color.green;
                tile_holder_right.GetComponent<SpriteRenderer>().color = Color.green;
            }

        }
        //Left Side
        if (numberOFLeftTiles <= 4)
        {
            tile_holder_left.transform.position = new Vector3(gameline_left_X, gameline_down_Y, -2);
        }
        else if (numberOFLeftTiles <= 6)
        {
            tile_holder_left.transform.position = new Vector3(gameline_left_X, gameline_down_Y, -2);
            Vector3 rotationVector = tile_holder_left.transform.rotation.eulerAngles;
            rotationVector.z = 90;
            tile_holder_left.transform.rotation = Quaternion.Euler(rotationVector);
        }
        else
        {
            tile_holder_left.transform.position = new Vector3(gameline_left_X, gameline_down_Y, -2);
            Vector3 rotationVector = tile_holder_left.transform.rotation.eulerAngles;
            rotationVector.z = 0;
            tile_holder_left.transform.rotation = Quaternion.Euler(rotationVector);

        }

        //Right Side

        if (numberOFRightTiles <= 4)
        {
            tile_holder_right.transform.position = new Vector3(gameline_right_X, gameline_up_Y, -2);
        }
        else if (numberOFRightTiles <= 6)
        {
            tile_holder_right.transform.position = new Vector3(gameline_right_X, gameline_up_Y, -2);
            Vector3 rotationVector = tile_holder_right.transform.rotation.eulerAngles;
            rotationVector.z = 90;
            tile_holder_right.transform.rotation = Quaternion.Euler(rotationVector);
        }
        else
        {
            tile_holder_right.transform.position = new Vector3(gameline_right_X, gameline_up_Y, -2);
            Vector3 rotationVector = tile_holder_right.transform.rotation.eulerAngles;
            rotationVector.z = 0;
            tile_holder_right.transform.rotation = Quaternion.Euler(rotationVector);
        }

    }

    //show console game Line
    public static void show_consoleGameLine()
    {
        string gl = "GameLine : ";
        foreach (title t in game_line)
        {
            gl += t.number + " ";
        }
        Debug.Log(gl);
    }

    public void showPassUI(int player_index)
    {
        player_pass_images[player_index].GetComponent<Image>().enabled = true;
        player_pass_images[player_index].GetComponentInChildren<Text>().enabled = true;
       // Debug.Log("Bot " + player_index + " PASS!!!");
    }

    public void showGameOverUI(int winner_index, int current_score)
    {
        game_over_UI.SetActive(true);

        for (int i = 0; i < 4; i++)
        {
            
            if (i >= number_of_players)
            {
                game_over_UI.transform.GetChild(0).GetChild((i)).GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                game_over_UI.transform.GetChild(0).GetChild((i + 1) % number_of_players).GetChild(0).GetChild(2).GetComponentInChildren<Text>().text = total_scores[i].ToString();
                game_over_UI.transform.GetChild(0).GetChild((i + 1) % number_of_players).GetChild(0).GetChild(3).GetComponentInChildren<Text>().text = "+" + 0;
                game_over_UI.transform.GetChild(0).GetChild((i + 1) % number_of_players).GetChild(0).GetChild(4).GetComponentInChildren<Text>().text = Scores[i].ToString();
            }
        }
        game_over_UI.transform.GetChild(0).GetChild(winner_index).GetChild(0).GetChild(3).GetComponentInChildren<Text>().text = "+" + current_score.ToString();


        blur_background.enabled = true;
    }

    public void disablePassUI(int player_index)
    {
        player_pass_images[player_index].GetComponent<Image>().enabled = false;
        player_pass_images[player_index].GetComponentInChildren<Text>().enabled = false;
    }

    public void disableAllNotVisibleUI()
    {
        for (int i = 0; i < player_pass_images.Length; i++)
        {
            disablePassUI(i);
        }
        game_over_UI.SetActive(false);
        blur_background.enabled = false;
    }

    public void disableHandTiles(bool isCpuTurn = false)
    {
        if (isCpuTurn == true)
        {
            foreach (title t in p1.hand)
            {
                t.domino_img.GetComponent<Image>().color = Color.gray;                           
            }
            return;
        }

        if (game_line.Count == 0)
        {
            if (number_of_rounds == 1)
            {
                foreach (title t in p1.hand)
                {
                    if (t.number != first_playable_twin)
                    {
                        t.domino_img.GetComponent<Image>().color = Color.gray;
                    }
                    else
                    {
                        t.domino_img.GetComponent<Image>().color = Color.white;
                    }

                }
                return;
            }
            else
            {
                return;
            }
        } 

        int gl_left = int.Parse(game_line[0].number[0].ToString());
        int gl_right = int.Parse(game_line[game_line.Count - 1].number[2].ToString());

        foreach (title t in p1.hand)
        {
            int played_left = int.Parse(t.number[0].ToString());
            int played_right = int.Parse(t.number[2].ToString());

            if (gl_right == played_left || gl_left == played_right ||
                gl_right == played_right || gl_left == played_left)
            {
                t.domino_img.GetComponent<Image>().color = Color.white;
            }
            else
            {
                t.domino_img.GetComponent<Image>().color = Color.gray;
            }
        }
    }

    public bool checkPlayerHand()
    {
        if (game_line.Count == 0) return true;

        int gl_left = int.Parse(game_line[0].number[0].ToString());
        int gl_right = int.Parse(game_line[game_line.Count - 1].number[2].ToString());

        foreach (title t in p1.hand)
        {
            int played_left = int.Parse(t.number[0].ToString());
            int played_right = int.Parse(t.number[2].ToString());

            if (gl_right == played_left || gl_left == played_right ||
                gl_right == played_right || gl_left == played_left)
            {
                return true;
            }
        }
        return false;
    }

    public string autoplay()
    {
        if (game_line.Count == 0) return p1.hand[0].number;

        int gl_left = int.Parse(game_line[0].number[0].ToString());
        int gl_right = int.Parse(game_line[game_line.Count - 1].number[2].ToString());

        foreach (title t in p1.hand)
        {
            int title_index = 0;
            int played_left = int.Parse(t.number[0].ToString());
            int played_right = int.Parse(t.number[2].ToString());

            if (gl_right == played_left || gl_left == played_right ||
                gl_right == played_right || gl_left == played_left)
            {
                return t.number;
            }

            title_index++;
        }
        return "Error";
    }

    public bool getAvailableTitle(int player_index)
    {
        if (titles.Count < 2)
        {
            return false;
        }
        if (player_index == number_of_players - 1)
        {
            int index = UnityEngine.Random.Range(0, titles.Count);
            p1.hand.Add(titles[index]);
            titles.RemoveAt(index);
            display_player_hand(p1.hand.Count - 1);
            Destroy(bazar.transform.GetChild(UnityEngine.Random.Range(0, bazar.transform.childCount)).gameObject);
            p1.showHand();

            return true;
        }

        while (titles.Count != 1)
        {
            int index = UnityEngine.Random.Range(0, titles.Count);
            bots[active_player].hand.Add(titles[index]);
            titles.RemoveAt(index);
            Destroy(bazar.transform.GetChild(UnityEngine.Random.Range(0, bazar.transform.childCount)).gameObject);
            bots[active_player].showHand();
            if (cpuPlays(active_player))
            {
                return true;
            }       
        }
       
        return false;
    }

    public void fillBazar()
    {
        float bazar_x = 50f;
        float bazar_y = -70f;
        for (int i = 0; i < titles.Count; i++)
        {
            float tile_x = bazar_x + (i % 4) * 65;
            float tile_y = bazar_y - (i / 4) * 100;
            Image bazar_temp_tile = Instantiate(empty_title, new Vector3(0, 0, 0), Quaternion.identity);
            bazar_temp_tile.GetComponent<RectTransform>().anchoredPosition = new Vector2(tile_x, tile_y);
            bazar_temp_tile.transform.SetParent(bazar.transform, false);
            
        }
    }

    public void showPlayerData()
    {
        
        for (int i = 4; i > number_of_players; i--)
        {
            Debug.Log(i);
            other_players_data.transform.GetChild(i-2).gameObject.SetActive(false);
        }
    }

    public void restartRound()
    {
        // Destroy previous round objects
        for (int i = 0; i < p1.hand.Count; i++)
        {
            Destroy(p1.hand[i].domino_img.gameObject);
        }

        for (int i = 0; i < game_line.Count; i++)
        {
            Destroy(game_line[i].obj_title);
        }

        titles = new List<title>();
        bots = new player[number_of_players - 1];
        game_line = new List<title>();

        human_player_turn = true;

        // reset domino numbers UI for bots

        for (int i = 0; i < 3; i++)
        {
            other_players_data.transform.GetChild(i).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>().text = "7";
        }

        // set other players score on UI

        for (int i = 0; i < 3; i++)
        {
            other_players_data.transform.GetChild(i).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(1).GetComponent<Text>().text = total_scores[i].ToString();
        }

        //active_player = -1;
        bot_turn_interval = 1;
        human_turn_interval = 10;

        numberOFLeftTiles = 0;
        numberOFRightTiles = 0;
        Scores = new int[] { 0, 0, 0, 0 };

        // Initialization of Variables
        gameline_left_X = 1920 / 2;
        gameline_right_X = 1920 / 2;
        gameline_up_Y = 1080 / 2;
        gameline_down_Y = 1080 / 2;
        center_of_titles_X = 960;
        center_of_titles_Y = 540;
        center_of_titles_X_scale = 350;

        // create titles
        int index_spr = 0;
        for (int i = 0; i <= 6; i++)
        {
            for (int j = i; j <= 6; j++)
            {
                title temp = new title(i + "#" + j, domino_sprites[index_spr]); index_spr++;
                temp.test = test;
                Button dmn_btn = domino_btn;
                GameObject t_obj = title_prefab;
                temp.obj_name = i + "#" + j;
                temp.set_obj(dmn_btn, t_obj);
                titles.Add(temp);
            }

        }

        // making hands
        List<title> temp_hand = new List<title>();

        for (int i = 0; i < 7; i++)
        {
            int index = UnityEngine.Random.Range(0, titles.Count);
            temp_hand.Add(titles[index]);
            titles.RemoveAt(index);
        }

        // prepearing hand for player
        p1 = new player(temp_hand);
        p1.showHand();
        display_player_hand();

        for (int j = 0; j < number_of_players - 1; j++)
        {
            temp_hand = new List<title>();

            for (int i = 0; i < 7; i++)
            {
                int index = UnityEngine.Random.Range(0, titles.Count);
                //Debug.Log(index);

                temp_hand.Add(titles[index]);
                titles.RemoveAt(index);
            }
            bots[j] = new player(temp_hand);
            bots[j].showHand();
        }

        if (number_of_passes == number_of_players)
        {
            // situation where nobody finishes

            if (number_of_rounds == 1)
            {
                // check who starts first
                int which_Bot_Has_Ones = check_ones();
                //Debug.Log(which_Bot_Has_Ones);
                if (which_Bot_Has_Ones != -1)
                {
                    active_player = which_Bot_Has_Ones;
                }
                else
                {
                    active_player = number_of_players - 1;
                }
            }
            else
            {
                // it means previous rounds last player must player
                active_player = game_blocker;
                number_of_rounds++;
            }

        }
        else
        {
            // situation where somebody won

            active_player = previous_winner_index;
            number_of_rounds++;

        }


        // Show titles in game line
        //show_consoleGameLine();

        // Define first start time for first turn
        turn_start_time = DateTime.Now;

        // Funcntions that must be initiated with Start
        disableAllNotVisibleUI();
        isGameActive = true;
    }
}