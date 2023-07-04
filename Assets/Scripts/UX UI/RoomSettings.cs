using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using FishNet.Object;

public class RoomSettings : NetworkBehaviour
{
    [Header("UI Options")]
    [SerializeField] TMP_Dropdown MapChoice;
    [SerializeField] int[] timeOptions;
    [SerializeField] TMP_Text timeTxt;
    [SerializeField] int defaultTimeIndex;
    int timeIndex;
    [SerializeField] int[] pointOptions;
    [SerializeField] TMP_Text pointText;
    [SerializeField] int defaultPointIndex;
    int pointIndex;
    [Header("Settings")]
    [SerializeField] TeamManager tManager;

    bool notHost;
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!IsHost)
        {
            MapChoice.enabled = false;
            notHost = true;
        }
    }
    void Start()
    {
        if (notHost)
            return;
        if (PlayerPrefs.HasKey("PlayTime"))
        {
            timeTxt.text = PlayerPrefs.GetInt("PlayTime").ToString();
        }
        else
        {
            timeTxt.text = timeOptions[defaultTimeIndex].ToString();
            PlayerPrefs.SetInt("PlayTime", timeOptions[defaultTimeIndex]);
        }

        if (PlayerPrefs.HasKey("PointGoal"))
        {
            pointText.text = PlayerPrefs.GetInt("PointGoal").ToString();
        }
        else
        {
            pointText.text = pointOptions[defaultPointIndex].ToString();
            PlayerPrefs.SetInt("PointGoal", pointOptions[defaultPointIndex]);
        }
        tManager.LobbyToLoad = MapChoice.options[MapChoice.value].ToString();
        ChangeMap();
    }
    public void ChangeMap()
    {
        if (notHost)
            return;
        tManager.LobbyToLoad = MapChoice.options[MapChoice.value].text;
        // print(MapChoice.options[MapChoice.value].text);
    }

    public void ChangeTime(int value)
    {
        if (notHost)
            return;
        if (value > 0)
        {
            if (timeIndex == timeOptions.Length - 1)
            {
                timeIndex = 0;
            }
            else
            {
                timeIndex++;
            }
        }
        else
        {
            if (timeIndex == 0)
            {
                timeIndex = timeOptions.Length - 1;
            }
            else
            {
                timeIndex--;
            }
        }
        timeTxt.text = timeOptions[timeIndex].ToString();
        PlayerPrefs.SetInt("PlayTime", timeOptions[timeIndex]);
    }

    public void ChangePoints(int value)
    {
        if (notHost)
            return;
        if (value > 0)
        {
            if (pointIndex == pointOptions.Length - 1)
            {
                pointIndex = 0;
            }
            else
            {
                pointIndex++;
            }
        }
        else
        {
            if (pointIndex == 0)
            {
                pointIndex = pointOptions.Length - 1;
            }
            else
            {
                pointIndex--;
            }
        }
        pointText.text = pointOptions[pointIndex].ToString();
        PlayerPrefs.SetInt("PointGoal", pointOptions[pointIndex]);
    }
}
