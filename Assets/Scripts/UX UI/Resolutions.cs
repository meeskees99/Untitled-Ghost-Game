using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Resolutions : MonoBehaviour
{
    List <Resolution> resolutions = new();
    [SerializeField] TMP_Dropdown resDropDown;

    [SerializeField] TMP_Dropdown fullScreenDrowdown;
    // Start is called before the first frame update
    void Start()
    {
        GetAndSetResolution();
        if (!PlayerPrefs.HasKey("FullscreenIndex"))
        {
            SetScreenOptions(0);
        }      
    }
    #region Resolution 
    void GetAndSetResolution()
    {
        int currentResolutionIndex = 0;
        if (PlayerPrefs.HasKey("ResolutionIndex"))
        {
            currentResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex");
        }
        else
        {
            currentResolutionIndex = 0;
        }

        Resolution[] tempRes = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height}).Distinct().ToArray();

        for (int i = tempRes.Length -1; i > 0; i--)
        {
            resolutions.Add(tempRes[i]);
        }      
        resDropDown.ClearOptions();

        List<string> options = new();

        for (int i = 0; i < resolutions.Count; i++)
        {
            string Option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(Option);
        }
        //options.Reverse();
        resDropDown.AddOptions(options);
        resDropDown.value = currentResolutionIndex;
        resDropDown.RefreshShownValue();
       
        Screen.SetResolution(resolutions[currentResolutionIndex].width, resolutions[currentResolutionIndex].height, true);

    }

    public void SetResolution(int index)
    {
        PlayerPrefs.SetInt("ResolutionIndex", index);
        Screen.SetResolution(resolutions[index].width, resolutions[index].height, Screen.fullScreen);
        resDropDown.value = index;
        //dropDown.RefreshShownValue();
    }

    public void GetResolution()
    {
        if (PlayerPrefs.HasKey("ResolutionIndex"))
        {
            SetResolution(PlayerPrefs.GetInt("ResolutionIndex"));
        }
        else
        {
            SetResolution(0);
        }
    }

    #endregion

    #region FullScreen
    public void SetScreenOptions(int index)
    {
        int currentFullscreenIndex = 0;
        if (PlayerPrefs.HasKey("FullscreenIndex"))
        {
            currentFullscreenIndex = PlayerPrefs.GetInt("FullscreenIndex");
        }
        else { currentFullscreenIndex = 0; }

        currentFullscreenIndex = index;
        switch (currentFullscreenIndex)
        {
            case 0:
                {
                    FullScreen();
                }
                break;
                case 1:
                {
                    Borderless();
                }
                break;
                case 2:
                {
                    Windowed();
                }
                break;
        }
        fullScreenDrowdown.value = index;
        fullScreenDrowdown.RefreshShownValue();
    }
    void FullScreen()
    {
        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
    }
    void Borderless()
    {
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
    }
    void Windowed()
    {
        Screen.fullScreenMode = FullScreenMode.Windowed;
    }
    #endregion
}
