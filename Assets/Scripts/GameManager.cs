using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
public class GameManager : NetworkBehaviour
{
    [SyncVar][SerializeField] PlayerData[] players;

    [SerializeField] Transform[] spawn1;
    [SerializeField] Transform[] spawn2;
    public bool MouseLocked;
    [SyncVar] int team1Index;
    [SyncVar] int team2Index;

    [SyncVar] int team1Points;
    [SyncVar] int team2Points;

    [Header("UI")]
    public GameObject settingsUI;
    [SerializeField] GameObject tabMenu;
    [SerializeField] GameObject scoreboard1;
    [SerializeField] GameObject scoreboard2;

    [SerializeField] KeyCode inGameSettingsButton;
    [SerializeField] KeyCode scoreboardButton;
    // Start is called before the first frame update
    void Start()
    {
        if (IsHost)
        {
            SetTeamPoints();
        }
        MouseLocked = false;
    }
    void Update()
    {
        if (Input.GetKeyDown(inGameSettingsButton))
        {
            settingsUI.SetActive(!settingsUI.activeSelf);
        }
        if(settingsUI.activeSelf){
            Cursor.lockState = CursorLockMode.Confined;
        }
        else{
            Cursor.lockState = CursorLockMode.Locked;
        }

        int id = InstanceFinder.ClientManager.Connection.ClientId;

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
    }

    public void SetLockstate(bool b){
        MouseLocked = b;
    }

    public void MainMenu(){
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu Test");
    }
}
