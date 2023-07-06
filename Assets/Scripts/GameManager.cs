using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FishNet;
using TMPro;
using UnityEngine.SceneManagement;
public class GameManager : NetworkBehaviour
{
    [Header("Players")]
    [SyncVar][SerializeField] PlayerData[] players;
    [SerializeField] Transform[] spawn1;
    [SerializeField] Transform[] spawn2;

    public static bool MouseLocked;


    [Header("Spawning")]
    [SyncVar] int team1Index;
    [SyncVar] int team2Index;


    [SyncVar] int team1Points;
    [SyncVar] int team2Points;

    [Header("UI")]
    public GameObject settingsUI;
    [SerializeField] GameObject tabMenu;
    [SerializeField] GameObject scoreboard1;
    [SerializeField] GameObject scoreboard2;
    [SerializeField] Slider team1Slider;
    [SerializeField] Slider team2Slider;
    [SerializeField] GameObject halfWayWarning;
    [SerializeField] GameObject friendlyHalfwayUI;
    [SerializeField] AudioSource halfWayFriendlySound;
    [SerializeField] AudioSource halfWayEnemySound;

    [Header("Keybinds")]
    [SerializeField] KeyCode inGameSettingsButton;
    [SerializeField] KeyCode scoreboardButton;

    [Header("Game Settings")]
    [SyncVar][SerializeField] int pointLimit;
    [SerializeField] int timeLimit;
    [SyncVar][SerializeField] float timeLeft;
    [SerializeField] TMP_Text timeText;

    [SerializeField] bool team1Halfway;
    [SerializeField] bool team2Halfway;

    LoadManager loader;

    int id;

    // Start is called before the first frame update
    void Start()
    {
        if (IsHost)
        {
            SetTeamPoints();
            timeLimit = PlayerPrefs.GetInt("PlayTime") * 60;
            timeLeft = timeLimit;
            timeText.text = timeLeft.ToString("0:00");
        }
        id = InstanceFinder.ClientManager.Connection.ClientId;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<PlayerData>().playerId == id)
            {
                if (players[i].GetComponent<PlayerData>().teamID == 0)
                {
                    team1Slider.gameObject.SetActive(true);
                    team2Slider.gameObject.SetActive(false);
                }
                else if (players[i].GetComponent<PlayerData>().teamID == 1)
                {
                    team1Slider.gameObject.SetActive(false);
                    team2Slider.gameObject.SetActive(true);
                }
            }
        }
        if (PlayerPrefs.HasKey("PointGoal") && IsHost)
        {
            SetPointGoal(PlayerPrefs.GetInt("PointGoal"));
        }
        else if (IsHost)
        {
            Debug.LogError("No Point Goal Found");
        }
    }
    void Update()
    {
        if (loader == null)
        {
            loader = FindObjectOfType<LoadManager>();
        }

        if (pointLimit == 0)
            return;
        if (timeLeft > 0 && team1Points < pointLimit && team2Points < pointLimit)
        {
            Timer();
        }
        else if (team1Points < pointLimit && team2Points < pointLimit)
            EndGame(true);
        else
            EndGame(false);
        team1Slider.value = team1Points;
        team2Slider.value = team2Points;
        if (team1Points >= pointLimit / 2 && !team1Halfway)
        {
            print("Team 1 is halfway!!");
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].GetComponent<PlayerData>().playerId == this.id)
                {
                    if (players[i].GetComponent<PlayerData>().teamID == 0)
                    {
                        halfWayFriendlySound.Play();
                        friendlyHalfwayUI.SetActive(true);
                    }
                    else if (players[i].GetComponent<PlayerData>().teamID == 1)
                    {
                        halfWayEnemySound.Play();
                        halfWayWarning.SetActive(true);
                    }
                }
            }
            team1Halfway = true;
        }

        if (team2Points >= pointLimit / 2 && !team2Halfway)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].GetComponent<PlayerData>().playerId == this.id)
                {
                    if (players[i].GetComponent<PlayerData>().teamID == 0)
                    {
                        halfWayEnemySound.Play();
                        halfWayWarning.SetActive(true);
                    }
                    else if (players[i].GetComponent<PlayerData>().teamID == 1)
                    {
                        halfWayFriendlySound.Play();
                        friendlyHalfwayUI.SetActive(true);
                    }
                }
            }
            team2Halfway = true;
        }

        if (Input.GetKeyDown(inGameSettingsButton))
        {
            settingsUI.SetActive(!settingsUI.activeSelf);
        }

        if (settingsUI.activeSelf)
        {
            Cursor.lockState = CursorLockMode.Confined;
            MouseLocked = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            MouseLocked = true;
        }

        int id = InstanceFinder.ClientManager.Connection.ClientId;

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<PlayerData>().playerId == id)
            {
                if (players[i].GetComponent<PlayerData>().teamID == 0)
                {
                    team1Slider.gameObject.SetActive(true);
                    team2Slider.gameObject.SetActive(false);
                }
                else if (players[i].GetComponent<PlayerData>().teamID == 1)
                {
                    team1Slider.gameObject.SetActive(false);
                    team2Slider.gameObject.SetActive(true);
                }
            }
        }
        if (Input.GetKey(scoreboardButton) && !settingsUI.activeSelf)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].GetComponent<PlayerData>().playerId == id)
                {
                    if (players[i].GetComponent<PlayerData>().teamID == 0)
                    {
                        tabMenu.SetActive(true);
                        scoreboard1.SetActive(true);
                        scoreboard2.SetActive(false);
                    }
                    else if (players[i].GetComponent<PlayerData>().teamID == 1)
                    {
                        tabMenu.SetActive(true);
                        scoreboard1.SetActive(false);
                        scoreboard2.SetActive(true);
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].GetComponent<PlayerData>().playerId == id)
                {
                    if (players[i].GetComponent<PlayerData>().teamID == 0)
                    {
                        tabMenu.SetActive(false);
                        scoreboard1.SetActive(false);
                    }
                    else if (players[i].GetComponent<PlayerData>().teamID == 1)
                    {
                        tabMenu.SetActive(false);
                        scoreboard2.SetActive(false);
                    }
                }
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    void SetPointGoal(int pointgoal)
    {
        team1Slider.maxValue = pointgoal;
        team2Slider.maxValue = pointgoal;
        pointLimit = pointgoal;
        ObserverPointgoal(pointgoal);
    }
    [ObserversRpc]
    void ObserverPointgoal(int pointgoal)
    {
        team1Slider.maxValue = pointgoal;
        team2Slider.maxValue = pointgoal;
        pointLimit = pointgoal;
    }
    [ServerRpc(RequireOwnership = false)]
    void Timer()
    {
        timeLeft -= Time.deltaTime;
        ObserverTimer(timeLeft);
    }
    bool started;
    [ObserversRpc]
    void ObserverTimer(float tijd)
    {
        if (!started)
        {
            started = true;
            tijd = timeLimit;
        }
        tijd -= Time.deltaTime;

        float minutes = (tijd / 60);
        float seconds = (tijd % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    [ServerRpc(RequireOwnership = false)]
    void SetTeamPoints()
    {
        players = FindObjectsOfType<PlayerData>();
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].teamID == 0)
            {
                players[i].transform.position = spawn1[team1Index].transform.position;
                team1Index++;
            }
            else if (players[i].teamID == 1)
            {
                players[i].transform.position = spawn1[team2Index].transform.position;
                team2Index++;
            }
        }
        SetTeamPointsObserver();
    }
    [ObserversRpc(BufferLast = true)]
    void SetTeamPointsObserver()
    {
        //print("a");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].teamID == 0)
            {
                players[i].transform.position = spawn1[team1Index].transform.position;
            }
            else if (players[i].teamID == 1)
            {
                players[i].transform.position = spawn2[team2Index].transform.position;
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void AddPoints(int teamInt, int Points)
    {
        if (teamInt == 0)
        {
            team1Points += Points;
        }
        else if (teamInt == 1)
        {
            team1Points += Points;
        }
        team1Slider.value = team1Points;
        team2Slider.value = team2Points;
    }

    public void MainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu Test");
    }

    public void EndGame(bool timeUp)
    {
        loader.SceneToUnload = "Game";
        NetworkHudCanvases networkHudCanvases = FindObjectOfType<NetworkHudCanvases>();
        networkHudCanvases.OnClick_Client();
        networkHudCanvases.OnClick_Server();
        loader.StartLoading = true;
        if (timeUp)
        {
            print("Game has ended. The time ran out");
        }
        else
        {
            print("Game has ended. Point Limit Reached");
        }

    }

    public void Halfway()
    {
        friendlyHalfwayUI.SetActive(false);
        halfWayWarning.SetActive(false);
    }
}
