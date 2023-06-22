using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class VideoSettings : MonoBehaviour
{
    [Header("Defaults")]
    [SerializeField] int defaultRes = 0;
    [SerializeField] int defaultFullscreen = 0;
    [SerializeField] float defaultFpsLimit = 60;
    [SerializeField] bool defaultLimitFPS = false;
    [SerializeField] bool defaultVSync = true;
    [SerializeField] int defaultFOV = 60;


    [Header("Settings To Store")]
    [SerializeField] int storedRes = 0;
    [SerializeField] int storedFullscreen = 0;
    [SerializeField] float storedFpsLimit = 60;
    [SerializeField] bool storedLimitFPS = false;
    [SerializeField] bool storedVSync = true;
    [SerializeField] int storedFOV = 60;

    [Header("Resolution")]
    [SerializeField] TMP_Dropdown resDropDown;
    int currentRes;
    int lastRes;
    List<Resolution> resolutions = new();
    [Header("Fullscreen")]
    [SerializeField] TMP_Dropdown fullScreenDrowdown;
    int currentFullscreen;
    int lastFullscreen;


    [Header("FPS")]
    [SerializeField] Slider fpsSlider;
    [SerializeField] TMP_InputField fpsInput;
    float currentFpsLimit;
    float lastFpsLimit;
    bool currentLimitFPS;
    bool lastLimitFPS;

    int FPSIndex;
    [SerializeField] GameObject limitFPSYes;
    [SerializeField] GameObject limitFPSNo;

    [Header("vSync")]
    [SerializeField] GameObject doVsyncYes;
    [SerializeField] GameObject doVsyncNo;
    bool currentVSync;
    int vSyncIndex;
    bool lastVSync;


    [Header("FOV")]
    [SerializeField] Slider fovSlider;
    [SerializeField] TMP_InputField fovInput;
    int currentFOV;
    int lastFOV;


    [Header("Apply Settings")]
    [SerializeField] GameObject applySettingsUI;
    [Tooltip("Time until it automatically reverts changes")]
    [SerializeField] float revertTime;
    [SerializeField] float timer;
    bool appliedSettings;


    void Start()
    {
        #region Fullscreen, Vcync, Fov PlayerPrefs
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
        if (PlayerPrefs.GetInt("vSync") == 0)
        {
            doVsyncNo.SetActive(true);
            doVsyncYes.SetActive(false);
            currentVSync = false;
            lastVSync = false;
        }
        else if (PlayerPrefs.GetInt("vSync") == 1)
        {
            doVsyncNo.SetActive(false);
            doVsyncYes.SetActive(true);
            currentVSync = true;
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
            defaultFOV = 60;
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
        if (PlayerPrefs.GetInt("LimitFps") == 1)
        {
            limitFPSYes.SetActive(true);
            limitFPSNo.SetActive(false);
            currentLimitFPS = true;
            lastLimitFPS = true;
        }
        else
        {
            limitFPSYes.SetActive(false);
            limitFPSNo.SetActive(true);
            currentLimitFPS = false;
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
        //fpsSlider.GetComponent<Slider>().enabled = (limitFPS || !doVSync);
        fpsInput.text = PlayerPrefs.GetFloat("targetFPS").ToString("0");
        fpsSlider.value = PlayerPrefs.GetFloat("targetFPS");
        if (!currentVSync && !currentLimitFPS)
            Application.targetFrameRate = 0;
        else
        {
            Application.targetFrameRate = (int)PlayerPrefs.GetFloat("targetFPS");
        }
        GetAndSetResolution();

        #endregion
    }
    private void Update()
    {
        appliedSettings = (lastRes == currentRes || lastFullscreen == currentFullscreen || lastFpsLimit == currentFpsLimit || lastLimitFPS == currentLimitFPS || lastFOV == currentFOV || lastVSync == currentVSync);
        if (timer > 0 && !appliedSettings)
        {
            timer -= Time.deltaTime;
        }
        else if (timer <= 0 && !appliedSettings)
        {
            RestoreDefault();
        }
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
            PlayerPrefs.SetInt("ResolutionIndex", currentRes);
        }

        Resolution[] tempRes = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();

        for (int i = tempRes.Length - 1; i > 0; i--)
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
        if (currentVSync)
        {
            Application.targetFrameRate = Screen.resolutions[currentRes].refreshRate;
            fpsSlider.value = fpsSlider.maxValue;
            fpsInput.text = fpsSlider.value.ToString("0");
        }
    }

    public void SetResolution(int index)
    {
        PlayerPrefs.SetInt("ResolutionIndex", index);
        Screen.SetResolution(resolutions[index].width, resolutions[index].height, Screen.fullScreen);
        resDropDown.value = index;
        resDropDown.RefreshShownValue();
        SetScreenOptions(PlayerPrefs.GetInt("FullscreenIndex"));
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
        if (currentVSync || !currentLimitFPS)
        {
            fpsSlider.GetComponent<Slider>().enabled = false;
            return;
        }
        else if (!currentVSync && currentLimitFPS)
        {
            fpsSlider.GetComponent<Slider>().enabled = true;
            Application.targetFrameRate = (int)fps;
            fpsInput.text = fps.ToString("0");
            PlayerPrefs.SetFloat("targetFPS", fps);
        }
    }
    public void InputFPS()
    {
        float f;
        if (!currentVSync && currentLimitFPS)
        {
            float.TryParse(fpsInput.text, out f);
            if (f < fpsSlider.minValue)
            {
                f = fpsSlider.minValue;
                fpsSlider.value = f;
            }
            else if (f > fpsSlider.maxValue)
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
        currentLimitFPS = toggle;
        if (toggle && !currentVSync)
        {
            fpsSlider.GetComponent<Slider>().enabled = true;
            PlayerPrefs.SetInt("LimitFps", 1);
            LimitFPS(PlayerPrefs.GetFloat("targetFPS"));
            FPSIndex = 1;
            limitFPSYes.SetActive(true);
            limitFPSNo.SetActive(false);
        }
        else
        {
            if (!currentVSync)
                Application.targetFrameRate = 0;
            fpsSlider.GetComponent<Slider>().enabled = false;
            PlayerPrefs.SetInt("LimitFps", 0);
            FPSIndex = 0;
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
            fpsInput.text = fpsSlider.value.ToString("0");
            QualitySettings.vSyncCount = 1;
            currentVSync = true;
            PlayerPrefs.SetInt("vSync", 1);
            print("PlayerPrefs vSync Set to 1");
            doVsyncYes.SetActive(true);
            doVsyncNo.SetActive(false);
            limitFPSNo.SetActive(true);
            limitFPSYes.SetActive(false);
        }
        else if (!toggle)
        {
            if (currentLimitFPS)
                fpsSlider.GetComponent<Slider>().enabled = true;
            QualitySettings.vSyncCount = 0;
            currentVSync = false;
            PlayerPrefs.SetInt("vSync", 0);
            Application.targetFrameRate = 0;
            doVsyncYes.SetActive(false);
            doVsyncNo.SetActive(true);
        }
        if (currentVSync)
            Application.targetFrameRate = Screen.resolutions[PlayerPrefs.GetInt("ResolutionIndex")].refreshRate;
        lastVSync = currentVSync;
        print("doVsync = " + currentVSync);
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
        if (lastRes == currentRes || lastFullscreen == currentFullscreen || lastFpsLimit == currentFpsLimit || lastLimitFPS == currentLimitFPS || lastFOV == currentFOV || lastVSync == currentVSync)
        {
            return;
        }
        else
        {
            applySettingsUI.SetActive(true);
            timer = revertTime;
            ChangeSettings();
        }
    }
    void ChangeSettings()
    {
        SetResolution(currentRes);
        SetScreenOptions(currentFullscreen);
        LimitFPS(currentFpsLimit);
        ToggleLimitFPS(currentLimitFPS);
        ChangeFov(currentFOV);
        ToggleVSync(currentVSync);
    }
    public void RevertLastSettings()
    {
        currentRes = lastRes;
        currentFullscreen = lastFullscreen;
        currentFpsLimit = lastFpsLimit;
        currentLimitFPS = lastLimitFPS;
        currentFOV = lastFOV;
        currentVSync = lastVSync;
        ChangeSettings();
    }

    public void RestoreDefault()
    {
        lastRes = defaultRes;
        currentRes = defaultRes;
        lastFullscreen = defaultFullscreen;
        currentFullscreen = defaultFullscreen;
        lastFpsLimit = defaultFpsLimit;
        currentFpsLimit = defaultFpsLimit;
        lastLimitFPS = defaultLimitFPS;
        currentLimitFPS = defaultLimitFPS;
        lastFOV = defaultFOV;
        currentFOV = defaultFOV;
        lastVSync = defaultVSync;
        currentVSync = defaultVSync;
        ChangeSettings();
    }
}
