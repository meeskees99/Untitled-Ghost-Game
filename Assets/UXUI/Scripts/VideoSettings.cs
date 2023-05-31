using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class VideoSettings : MonoBehaviour
{
    [Header("Resolution")]
    List <Resolution> resolutions = new();
    [SerializeField] TMP_Dropdown resDropDown;
    int currentRes;
    int lastRes;
    int defaultRes;

    [SerializeField] TMP_Dropdown fullScreenDrowdown;
    int currentFullscreen;
    int lastFullscreen;
    int defaultFullscreen = 0;


    [Header("FPS")]
    [SerializeField] Slider fpsSlider;
    [SerializeField] TMP_Text fpsText;
    [SerializeField] InputField fpsInput;
    float lastFpsLimit;
    float defaultFpsLimit;
    bool lastLimitFPS;
    bool defaultLimitFPS = false;
    
    bool limitFPS;
    int FPSIndex;
    [SerializeField] GameObject limitFPSYes;
    [SerializeField] GameObject limitFPSNo;

    [Header("vSync")]
    [SerializeField] GameObject doVsyncYes;
    [SerializeField] GameObject doVsyncNo;
    bool doVSync;
    int vSyncIndex;
    bool lastVSync;
    bool defaultVSync = true;


    [Header("FOV")]
    [SerializeField] Slider fovSlider;
    [SerializeField] InputField fovInput;
    int lastFOV;
    int defaultFOV = 60;

    bool appliedSettings;
    

    // Start is called before the first frame update
    void Start()
    {
        #region PlayerPrefs
        if (!PlayerPrefs.HasKey("FullscreenIndex"))
        {
            PlayerPrefs.SetInt("FullscreenIndex", defaultFullscreen);
            currentFullscreen = defaultFullscreen;
            lastFullscreen = currentFullscreen;
        }
        else
        {
            PlayerPrefs.SetInt("FullscreenIndex", PlayerPrefs.GetInt("FullscreenIndex"));
            lastFullscreen = PlayerPrefs.GetInt("FullscreenIndex");
            switch (PlayerPrefs.GetInt("FullscreenIndex"))
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
            fullScreenDrowdown.value = PlayerPrefs.GetInt("FullscreenIndex");
            fullScreenDrowdown.RefreshShownValue();
        }
        if (PlayerPrefs.HasKey("vSync"))
        {
            vSyncIndex = PlayerPrefs.GetInt("vSync");
        }
        else
        {
            PlayerPrefs.SetInt("vSync", vSyncIndex);
        }
        if(PlayerPrefs.GetInt("vSync") == 0)
        {
            doVsyncNo.SetActive(true);
            doVsyncYes.SetActive(false);
            doVSync = false;
            lastVSync = false;
        }
        else if (PlayerPrefs.GetInt("vSync") == 1)
        {
            doVsyncNo.SetActive(false);
            doVsyncYes.SetActive(true);
            doVSync = true;
            lastVSync = true;
        }
        if (PlayerPrefs.HasKey("fov"))
        {
            fovSlider.value = PlayerPrefs.GetInt("fov");
            fovInput.text = PlayerPrefs.GetInt("fov").ToString("0");
            lastFOV = PlayerPrefs.GetInt("fov");
        }
        else
        {
            PlayerPrefs.SetInt("fov", defaultFOV);
            fovSlider.value = PlayerPrefs.GetInt("fov");
            fovInput.text = PlayerPrefs.GetInt("fov").ToString("0");
            lastFOV = defaultFOV;
        }
        #endregion
        #region FPS prefs
        if (PlayerPrefs.HasKey("LimitFps"))
        {
            FPSIndex = PlayerPrefs.GetInt("LimitFps");
        }
        else
        {
            PlayerPrefs.SetInt("LimitFps", 0);
            FPSIndex = 0;
        }
        if(PlayerPrefs.GetInt("LimitFps") == 1)
        {
            limitFPSYes.SetActive(true);
            limitFPSNo.SetActive(false);
            limitFPS = true;
            lastLimitFPS = true;
        }
        else
        {
            limitFPSYes.SetActive(false);
            limitFPSNo.SetActive(true);
            limitFPS = false;
            lastLimitFPS = false;
        }
        if (PlayerPrefs.HasKey("targetFPS"))
        {
            PlayerPrefs.SetFloat("targetFPS", PlayerPrefs.GetFloat("targetFPS"));
            lastFpsLimit = PlayerPrefs.GetFloat("targetFPS");
        }
        else
        {
            PlayerPrefs.SetFloat("targetFPS", fpsSlider.maxValue);
            defaultFpsLimit = PlayerPrefs.GetFloat("targetFPS");
            lastFpsLimit = PlayerPrefs.GetFloat("targetFPS");
        }
        #endregion
        #region StartFunctions
        QualitySettings.vSyncCount = PlayerPrefs.GetInt("vSync");
        Application.targetFrameRate = (int)PlayerPrefs.GetFloat("targetFPS");
        fpsSlider.GetComponent<Slider>().enabled = (limitFPS && !doVSync);
        fpsText.text = PlayerPrefs.GetFloat("targetFPS").ToString("0");
        //fpsInput.text = PlayerPrefs.GetFloat("targetFPS").ToString("0");
        bool doFpsSlider = (limitFPS && !doVSync);
        fpsSlider.GetComponent<Slider>().enabled = doFpsSlider;
       
        fpsSlider.value = PlayerPrefs.GetFloat("targetFPS");
        if (!doVSync && !limitFPS)
            Application.targetFrameRate = 0;
        if (!PlayerPrefs.HasKey("ResolutionIndex"))
            GetAndSetResolution();
        #endregion
    }
    #region Resolution 
    void GetAndSetResolution()
    {
        currentRes = 0;
        if (PlayerPrefs.HasKey("ResolutionIndex"))
        {
            currentRes = PlayerPrefs.GetInt("ResolutionIndex");
        }
        else
        {
            currentRes = 0;
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
        resDropDown.value = currentRes;
        resDropDown.RefreshShownValue();
       
        Screen.SetResolution(resolutions[currentRes].width, resolutions[currentRes].height, true);
        fpsSlider.maxValue = Screen.resolutions[currentRes].refreshRate;
        if (doVSync)
        {
            Application.targetFrameRate = Screen.resolutions[currentRes].refreshRate;
            fpsSlider.value = fpsSlider.maxValue;
            fpsText.text = fpsSlider.value.ToString("0");
        }
    }

    public void SetResolution(int index)
    {
        PlayerPrefs.SetInt("ResolutionIndex", index);
        int i = PlayerPrefs.GetInt("FullscreenMode");
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
        PlayerPrefs.SetInt("FullscreenIndex", index);
        switch (index)
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
    #region LimitFPS
    public void LimitFPS(float fps)
    {
        if (doVSync)
        {
            fpsSlider.GetComponent<Slider>().enabled = false;
            return;
        }
        else if (!doVSync && limitFPS)
        {
            Application.targetFrameRate = (int)fps;
            fpsText.text = fps.ToString("0");
            PlayerPrefs.SetFloat("targetFPS", fps);
        }
    }
    public void InputFPS()
    {
        float f;
        if (!doVSync && limitFPS)
        {
            float.TryParse(fpsText.text, out f);
            if(f < fpsSlider.minValue)
            {
                f = fpsSlider.minValue;
                fpsSlider.value = f;
            }
            else if(f > fpsSlider.maxValue)
            {
                f = fpsSlider.maxValue;
                fpsSlider.value = f;
            }
            fpsSlider.value = f;
            LimitFPS(f);
        }
    }

    public void ToggleLimitFPS(bool toggle)
    {
        limitFPS = toggle;
        if(toggle && !doVSync)
        {
            fpsSlider.GetComponent<Slider>().enabled = true;
            PlayerPrefs.SetInt("LimitFps", FPSIndex = 1);
            limitFPSYes.SetActive(true);
            limitFPSNo.SetActive(false);
            LimitFPS(PlayerPrefs.GetFloat("targetFPS"));
        }
        else
        {
            if(!doVSync)
                Application.targetFrameRate = 0;
            fpsSlider.GetComponent<Slider>().enabled = false;
            PlayerPrefs.SetInt("LimitFps", FPSIndex = 0);
            limitFPSYes.SetActive(false);
            limitFPSNo.SetActive(true);
        }
    }
    #endregion
    #region vSync
    public void ToggleVSync(bool toggle)
    {
        if (toggle)
        {
            fpsSlider.GetComponent<Slider>().enabled = false;
            fpsSlider.value = fpsSlider.maxValue;
            fpsText.text = fpsSlider.value.ToString("0");
            QualitySettings.vSyncCount = 1;
            doVSync = true;
            PlayerPrefs.SetInt("vSync", 1);
            print("PlayerPrefs vSync Set to 1");
        }
        else if (!toggle)
        {
            if(limitFPS)
                fpsSlider.GetComponent<Slider>().enabled = true;
            QualitySettings.vSyncCount = 0;
            doVSync = false;
            PlayerPrefs.SetInt("vSync", 0);
            Application.targetFrameRate = 0;
        }
        if (doVSync)
            Application.targetFrameRate = Screen.resolutions[PlayerPrefs.GetInt("ResolutionIndex")].refreshRate;
        doVsyncNo.SetActive(!toggle);
        doVsyncYes.SetActive(toggle);
        print("doVsync = " + doVSync);
        print("vSync playerprefs = " + PlayerPrefs.GetInt("vSync"));
    }
    #endregion
    #region fov
    public void ChangeFov(float value)
    {
        PlayerPrefs.SetInt("fov", (int)value);
        fovInput.text = PlayerPrefs.GetInt("fov").ToString("0");
    }

    public void FovInput()
    {
        int fov = fovInput.text.ConvertTo<int>();
        if (fov > fovSlider.maxValue)
              fov = (int)fovSlider.maxValue;
        else if (fov < fovSlider.minValue)
            fov = (int)fovSlider.minValue;
        PlayerPrefs.SetInt("fov", fovInput.text.ConvertTo<int>());
        fovSlider.value = PlayerPrefs.GetInt("fov");
        fovInput.text = PlayerPrefs.GetInt("fov").ToString("0");
    }
    #endregion
    public void ApplySettings()
    {
        if(lastRes == currentRes || lastFullscreen == currentFullscreen)
        {
            return;
        }
        else
        {
            //Enable ApplySettings Panel
        }
    }

    public void RestoreDefault()
    {
        lastRes = defaultRes;
        lastFullscreen = defaultFullscreen;
        lastFpsLimit = defaultFpsLimit;
        lastLimitFPS = defaultLimitFPS;
        lastFOV = defaultFOV;
        lastVSync = defaultVSync;
        //Do the settings
    }
}
