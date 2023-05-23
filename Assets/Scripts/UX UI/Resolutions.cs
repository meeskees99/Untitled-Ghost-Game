using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Resolutions : MonoBehaviour
{
    [SerializeField]List <Resolution> resolutions = new();
    [SerializeField]TMP_Dropdown dropDown;
    // Start is called before the first frame update
    void Start()
    {
        GetAndSetResolution();
        dropDown = GetComponent<TMP_Dropdown>();
    }
    #region Resolution 
    public void GetAndSetResolution()
    {
        int currentResolutionIndex = 0;
        if (PlayerPrefs.HasKey("ResolutionIndex"))
        {
            currentResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex");
        }
        else
        {
            currentResolutionIndex = 2;
        }

        Resolution[] tempRes = Screen.resolutions;

        for (int i = tempRes.Length -1; i > 0; i--)
        {
            resolutions.Add(tempRes[i]);
        }      
        dropDown.ClearOptions();

        List<string> options = new();

        for (int i = 0; i < resolutions.Count; i++)
        {
            string Option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(Option);
        }
        //options.Reverse();
        dropDown.AddOptions(options);
        dropDown.value = currentResolutionIndex;
        dropDown.RefreshShownValue();
       
        Screen.SetResolution(resolutions[currentResolutionIndex].width, resolutions[currentResolutionIndex].height, true);

    }

    public void SetResolution(int index)
    {
        PlayerPrefs.SetInt("ResolutionIndex", index);
        Screen.SetResolution(resolutions[index].width, resolutions[index].height, Screen.fullScreen);
        dropDown.value = index;
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
}
