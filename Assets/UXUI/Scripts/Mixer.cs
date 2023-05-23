using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;

public class Mixer : MonoBehaviour
{
    [SerializeField] AudioMixer masterMixer;
    [SerializeField] TMP_Text masterPerc;
    [SerializeField] TMP_Text musicPerc;
    [SerializeField] TMP_Text sfxPerc;

    // Zet de sliders van -60 tot 20
    public void SetMasterVol(float masterLvl)
    {
        masterMixer.SetFloat("MasterVol", Mathf.Log10(masterLvl)*20);
        masterPerc.text = (masterLvl * 1.25 + 75).ToString("0") + "%";
    }

    public void SetSFXVol(float sfxLvl)
    {
        masterMixer.SetFloat("SFXVol", Mathf.Log10(sfxLvl) * 20);
        sfxPerc.text = (sfxLvl * 1.25 + 75).ToString("0") + "%";
    }

    public void SetMusicVol(float musicVol)
    { 
        masterMixer.SetFloat("MusicVol", Mathf.Log10(musicVol) * 20);
        musicPerc.text = (musicVol * 1.25 + 75).ToString("0") + "%";
    }
}
