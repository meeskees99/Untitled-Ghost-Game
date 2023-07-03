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
    [SerializeField] int currentRes;
    int lastRes;
    List<Resolution> resolutions = new();
    [Header("Fullscreen")]
    [SerializeField] TMP_Dropdown fullScreenDrowdown;
    [SerializeField] int currentFullscreen;
    int lastFullscreen;


    [Header("FPS")]
    [SerializeField] Slider fpsSlider;
    [SerializeField] TMP_InputField fpsInput;
    [SerializeField] float currentFpsLimit;
    float lastFpsLimit;
    [SerializeField] bool currentLimitFPS;
    bool lastLimitFPS;

    int FPSIndex;
    [SerializeField] GameObject limitFPSYes;
    [SerializeField] GameObject limitFPSNo;

    [Header("vSync")]
    [SerializeField] GameObject doVsyncYes;
    [SerializeField] GameObject doVsyncNo;
    [SerializeField] bool currentVSync;
    [SerializeField] int vSyncIndex;
    bool lastVSync;


    [Header("FOV")]
    [SerializeField] Slider fovSlider;
    [SerializeField] TMP_InputField fovInput;
    [SerializeField] int currentFOV;
    [SerializeField] int lastFOV;


    [Header("Apply Settings")]
    [SerializeField] GameObject applySettingsUI;
    [Tooltip("Time until it automatically reverts changes")]
    [SerializeField] float revertTime;
    [SerializeField] float timer;
    [SerializeField] TMP_Text timerTxt;

    [SerializeField] bool _restoreTimer;

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
            StartVsync(0);
            currentVSync = false;
            lastVSync = false;
        }
        else if (PlayerPrefs.GetInt("vSync") == 1)
        {
            StartVsync(1);
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
        StartSettingsChange();
        #endregion
    }
    private void Update()
    {
        if (timer == 0)
        {

        }

        if (_restoreTimer && timer > 0)
        {
            timer -= Time.deltaTime;
            timerTxt.text = "Restoring in: " + timer.ToString("0");
        }
        else if (_restoreTimer && timer <= 0)
        {
            RestoreDefault();
            timer = 5;
            _restoreTimer = false;
        }
        ///////////////////////////Dit werkt NIET
        // if (!SettingsChanged())
        // {
        //     appliedSettings = false;
        // }
        // if (timer > 0 && !appliedSettings)
        // {
        //     timer -= Time.deltaTime;
        //     timerTxt.text = "Restoring in: " + timer.ToString("0");
        // }
        // else if (timer <= 0 && !appliedSettings)
        // {
        //     print("Reverted Settings");
        //     applySettingsUI.SetActive(false);
        //     RevertLastSettings();
        // }
        // else if (appliedSettings)
        // {
        //     print("Settings Are Up To Date");
        // }
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

    void SetResolution(int index)
    {
        PlayerPrefs.SetInt("ResolutionIndex", index);
        Screen.SetResolution(resolutions[index].width, resolutions[index].height, Screen.fullScreen);
        resDropDown.value = index;
        resDropDown.RefreshShownValue();
        SetScreenOptions(PlayerPrefs.GetInt("FullscreenIndex"));
        currentRes = index;
    }

    void GetResolution()
    {
        if (PlayerPrefs.HasKey("ResolutionIndex"))
        {
            SetResolution(PlayerPrefs.GetInt("ResolutionIndex"));
        }
        else
        {
            SetResolution(0);
        }
        storedRes = currentRes;
    }

    public void NewResolution(int index)
    {
        resDropDown.value = index;
        resDropDown.RefreshShownValue();
        storedRes = index;
    }

    #endregion

    #region FullScreen
    public void NewFullscreen(int index)
    {
        storedFullscreen = index;
        fullScreenDrowdown.value = index;
        fullScreenDrowdown.RefreshShownValue();
    }
    void SetScreenOptions(int index)
    {
        print(index);
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

        currentFullscreen = index;
    }

    void RestoreFullScreen(int index)
    {
        fullScreenDrowdown.value = index;
        fullScreenDrowdown.RefreshShownValue();
        SetScreenOptions(index);
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
    void LimitFPS(float fps)
    {
        print(fps);

        Application.targetFrameRate = (int)fps;
        fpsInput.text = fps.ToString("0");
        fpsSlider.value = fps;
        PlayerPrefs.SetFloat("targetFPS", fps);

        // fpsSlider.value = fps;
        currentFpsLimit = fps;
    }

    void StartLimitFps(float fps)
    {
        print(fps);
        Application.targetFrameRate = (int)fps;
        fpsInput.text = fps.ToString("0");
        fpsSlider.value = fps;
        currentFpsLimit = fps;
    }
    public void InputFPS()
    {
        float f;

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
        storedFpsLimit = f;

    }
    public void FPSSlider(float f)
    {
        storedFpsLimit = f;
        fpsInput.text = f.ToString("0");
    }

    void ToggleLimitFPS(bool toggle)
    {
        if (toggle)
        {
            PlayerPrefs.SetInt("LimitFps", 1);
            FPSIndex = 1;
            limitFPSYes.SetActive(true);
            limitFPSNo.SetActive(false);
        }
        else
        {
            if (!currentVSync)
                Application.targetFrameRate = 0;
            PlayerPrefs.SetInt("LimitFps", 0);
            FPSIndex = 0;
            limitFPSYes.SetActive(false);
            limitFPSNo.SetActive(true);
        }
        currentLimitFPS = toggle;
    }

    void StartToggleLimitFPS(int toggleIndex)
    {
        if (toggleIndex == 0)
        {
            ToggleLimitFPS(false);
        }
        else if (toggleIndex == 1)
        {
            ToggleLimitFPS(true);
        }
    }

    public void NewLimitFPS(bool toggle)
    {
        storedLimitFPS = toggle;
        if (toggle)
        {
            limitFPSYes.SetActive(true);
            limitFPSNo.SetActive(false);
            FPSIndex = 1;
        }
        else
        {
            limitFPSYes.SetActive(false);
            limitFPSNo.SetActive(true);
            FPSIndex = 0;
        }

    }
    #endregion
    #region vSync
    void StartVsync(int index)
    {
        QualitySettings.vSyncCount = index;
        if (index == 0)
        {
            doVsyncYes.SetActive(false);
            doVsyncNo.SetActive(true);
        }
        else
        {
            doVsyncYes.SetActive(true);
            doVsyncNo.SetActive(false);
        }
        vSyncIndex = index;
    }

    void ToggleVSync(bool toggle)
    {
        if (toggle)
        {
            QualitySettings.vSyncCount = 1;
            PlayerPrefs.SetInt("vSync", 1);
            doVsyncYes.SetActive(true);
            doVsyncNo.SetActive(false);
            vSyncIndex = 1;
        }
        else if (!toggle)
        {
            QualitySettings.vSyncCount = 0;
            PlayerPrefs.SetInt("vSync", 0);
            Application.targetFrameRate = 0;
            doVsyncYes.SetActive(false);
            doVsyncNo.SetActive(true);
            vSyncIndex = 0;
        }
        currentVSync = toggle;
        if (currentVSync)
            Application.targetFrameRate = Screen.resolutions[PlayerPrefs.GetInt("ResolutionIndex")].refreshRate;
        lastVSync = currentVSync;
    }
    public void NewVSync(bool toggle)
    {
        storedVSync = toggle;
        if (toggle)
        {
            doVsyncYes.SetActive(true);
            doVsyncNo.SetActive(false);
        }
        else
        {
            doVsyncYes.SetActive(false);
            doVsyncNo.SetActive(true);
        }
    }
    #endregion
    #region fov
    void StartFOV(int value)
    {
        ChangeFov(PlayerPrefs.GetInt("fov"));
        fovSlider.value = PlayerPrefs.GetInt("fov");
    }
    void ChangeFov(float value)
    {
        PlayerPrefs.SetInt("fov", (int)value);
        currentFOV = (int)value;
    }

    void RestoreFov(float value)
    {
        fovInput.text = value.ToString();
        fovSlider.value = value;
        ChangeFov(value);
    }

    public void FovInput()
    {
        float fov;
        float.TryParse(fovInput.text, out fov);
        if (fov > fovSlider.maxValue)
            fov = (int)fovSlider.maxValue;
        else if (fov < fovSlider.minValue)
            fov = (int)fovSlider.minValue;
        storedFOV = (int)fov;
        fovSlider.value = fov;
        fovInput.text = fov.ToString("0");
    }

    public void NewFov(float value)
    {
        fovInput.text = value.ToString();
        storedFOV = (int)value;
    }
    #endregion

    public void ApplySettings()
    {
        if (!SettingsChanged())
        {
            print("Settings not changed");
            return;
        }
        else
        {
            applySettingsUI.SetActive(true);
            timer = revertTime;
            ChangeSettings();
        }
    }
    public void AppliedSettings(bool value)
    {
        _restoreTimer = true;
    }
    public void FinalApply()
    {
        _restoreTimer = false;
    }
    bool SettingsChanged()
    {
        return !(storedRes == currentRes && storedFullscreen == currentFullscreen && storedFpsLimit == currentFpsLimit && storedLimitFPS == currentLimitFPS && storedFOV == currentFOV && storedVSync == currentVSync);
    }
    void ChangeSettings()
    {
        SetResolution(storedRes);
        lastRes = storedRes;
        ToggleVSync(storedVSync);
        lastVSync = storedVSync;
        ToggleLimitFPS(storedLimitFPS);
        lastLimitFPS = storedLimitFPS;
        LimitFPS(storedFpsLimit);
        lastFpsLimit = storedFpsLimit;
        SetScreenOptions(storedFullscreen);
        lastFullscreen = storedFullscreen;
        ChangeFov(storedFOV);
        lastFOV = storedFOV;
    }

    void StartSettingsChange()
    {
        SetResolution(PlayerPrefs.GetInt("ResolutionIndex"));
        StartVsync(PlayerPrefs.GetInt("vSync"));
        StartToggleLimitFPS(PlayerPrefs.GetInt("LimitFps"));
        StartLimitFps(PlayerPrefs.GetFloat("targetFPS"));
    }
    public void RevertLastSettings()
    {
        storedRes = lastRes;
        storedFullscreen = lastFullscreen;
        storedFpsLimit = lastFpsLimit;
        storedLimitFPS = lastLimitFPS;
        storedFOV = lastFOV;
        storedVSync = lastVSync;
        ChangeSettings();
    }

    public void RestoreDefault()
    {
        print("RestoreDefault");
        lastRes = defaultRes;
        currentRes = defaultRes;
        storedRes = defaultRes;

        lastFullscreen = defaultFullscreen;
        currentFullscreen = defaultFullscreen;
        storedFullscreen = defaultFullscreen;

        lastFpsLimit = defaultFpsLimit;
        currentFpsLimit = defaultFpsLimit;
        storedFpsLimit = defaultFpsLimit;

        lastLimitFPS = defaultLimitFPS;
        currentLimitFPS = defaultLimitFPS;
        storedLimitFPS = defaultLimitFPS;

        lastFOV = defaultFOV;
        currentFOV = defaultFOV;
        storedFOV = defaultFOV;

        lastVSync = defaultVSync;
        currentVSync = defaultVSync;
        storedVSync = defaultVSync;

        // ChangeSettings();
        SetResolution(defaultRes);
        ToggleVSync(defaultVSync);
        ToggleLimitFPS(defaultLimitFPS);
        LimitFPS(defaultFpsLimit);
        RestoreFullScreen(defaultFullscreen);
        RestoreFov(defaultFOV);
    }
}
