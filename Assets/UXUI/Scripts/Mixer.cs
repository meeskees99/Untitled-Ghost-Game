using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class Mixer : MonoBehaviour
{
    [SerializeField] AudioMixer masterMixer;

    [Header("Sliders")]
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    [Header("Input Fields")]
    [SerializeField] TMP_InputField masterInput;
    [SerializeField] TMP_InputField musicInput;
    [SerializeField] TMP_InputField sfxInput;

    private void Start()
    {
        if (PlayerPrefs.HasKey("MasterVol"))
        {
            masterSlider.value = PlayerPrefs.GetFloat("MasterVol");
            SetMasterVol(PlayerPrefs.GetFloat("MasterVol"));
        }
        else
        {
            PlayerPrefs.SetFloat("MasterVol", masterSlider.value);
        }
        if (PlayerPrefs.HasKey("MusicVol"))
        {
            musicSlider.value = PlayerPrefs.GetFloat("MusicVol");
            SetMusicVol(PlayerPrefs.GetFloat("MusicVol"));
        }
        else
        {
            PlayerPrefs.SetFloat("MusicVol", musicSlider.value);
        }
        if (PlayerPrefs.HasKey("SfxVol"))
        {
            sfxSlider.value = PlayerPrefs.GetFloat("SfxVol");
            SetSFXVol(PlayerPrefs.GetFloat("SfxVol"));
        }
        else
        {
            PlayerPrefs.SetFloat("SfxVol", sfxSlider.value);
        }
    }

    public void SetMasterVol(float masterLvl)
    {
        masterMixer.SetFloat("MasterVol", Mathf.Log10(masterLvl) * 20);
        PlayerPrefs.SetFloat("MasterVol", masterLvl);
        masterInput.text = (masterLvl * 100).ToString("0") + "%";
    }

    public void SetMusicVol(float musicLvl)
    { 
        masterMixer.SetFloat("MusicVol", Mathf.Log10(musicLvl) * 20);
        PlayerPrefs.SetFloat("MusicVol", musicLvl);
        musicInput.text = (musicLvl * 100).ToString("0") + "%";
    }
    public void SetSFXVol(float sfxLvl)
    {
        masterMixer.SetFloat("SFXVol", Mathf.Log10(sfxLvl) * 20);
        PlayerPrefs.SetFloat("SfxVol", sfxLvl);
        sfxInput.text = (sfxLvl * 100).ToString("0") + "%";
    }

    public void InputMasterVol()
    {
        float f;
        float.TryParse(masterInput.text, out f);
        if (f < masterSlider.minValue)
            f = masterSlider.minValue;
        if (f > masterSlider.maxValue)
            f = masterSlider.maxValue;
        masterSlider.value = f;
        SetMasterVol(f);
    }

    public void InputMusicVol()
    {
        float f;
        float.TryParse(musicInput.text, out f);
        if (f < musicSlider.minValue)
            f = musicSlider.minValue;
        if (f > musicSlider.maxValue)
            f = musicSlider.maxValue;
        musicSlider.value = f;
        SetMusicVol(f);
    }

    public void InputSfxVol()
    {
        float f;
        float.TryParse(sfxInput.text, out f);
        if (f < sfxSlider.minValue)
            f = sfxSlider.minValue;
        if (f > sfxSlider.maxValue)
            f = sfxSlider.maxValue;
        sfxSlider.value = f;
        SetSFXVol(f);
    }
}