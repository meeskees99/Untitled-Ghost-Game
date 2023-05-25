using UnityEngine;
using UnityEngine.Audio;
using TMPro;

public class Mixer : MonoBehaviour
{
    [SerializeField] AudioMixer masterMixer;
    [SerializeField] TMP_Text masterPerc;
    [SerializeField] TMP_Text musicPerc;
    [SerializeField] TMP_Text sfxPerc;

    public void SetMasterVol(float masterLvl)
    {
        masterMixer.SetFloat("MasterVol", Mathf.Log10(masterLvl) * 20);
        masterPerc.text = (masterLvl * 100).ToString("0") + "%";
    }

    public void SetSFXVol(float sfxLvl)
    {
        masterMixer.SetFloat("SFXVol", Mathf.Log10(sfxLvl) * 20);
        sfxPerc.text = (sfxLvl * 100).ToString("0") + "%";
    }

    public void SetMusicVol(float musicLvl)
    { 
        masterMixer.SetFloat("MusicVol", Mathf.Log10(musicLvl) * 20);
        musicPerc.text = (musicLvl * 100).ToString("0") + "%";
    }
}