using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class GameOptions : MonoBehaviour
{
    [Header("Username Settings")]
    [SerializeField] TMP_InputField username;
    [Header("Character Settings")]
    [SerializeField] Sprite[] characters;
    [SerializeField] GameObject selectedCharacter;
    int characterIndex;
    [Header("Mouse Settings")]
    [SerializeField] Slider sensSlider;
    [SerializeField] TMP_InputField sensInput;
    [Tooltip("Multiplies Slider Value By this")]
    [SerializeField] float sensMultiplier;

    #region Player Settings
    private void Start()
    {
        #region Get and Set PlayerPrefs
        if (!PlayerPrefs.HasKey("Character"))
        {
            PlayerPrefs.SetInt("Character", characterIndex);
        }
        else
        {
            characterIndex = PlayerPrefs.GetInt("Character");
        }
        selectedCharacter.GetComponent<Image>().sprite = characters[characterIndex];
        if (!PlayerPrefs.HasKey("username"))
        {
            username.text = selectedCharacter.GetComponent<Image>().sprite.name;
        }
        else
        {
            username.text = PlayerPrefs.GetString("username");
        }

        if(PlayerPrefs.HasKey("Mouse Sensitivity"))
        {
            PlayerPrefs.SetFloat("Mouse Sensitivity", PlayerPrefs.GetFloat("Mouse Sensitivity"));
        }
        else if(!PlayerPrefs.HasKey("Mouse Sensitivity"))
        {
            PlayerPrefs.SetFloat("Mouse Sensitivity", 1 * sensMultiplier);
        }
        #endregion
        
        float z = PlayerPrefs.GetFloat("Mouse Sensitivity");
        sensSlider.value = z / sensMultiplier;
        float f = PlayerPrefs.GetFloat("Mouse Sensitivity");
        float v = (f / sensMultiplier * 1000);
        sensInput.text = v.ToString("0,###");
    }
    public void SetUsername()
    {
        if(username.text.Length == 0 || username.text.Contains(" "))
        {
            print("Spaces are not allowed!");
            return;
        }
        if (PlayerPrefs.HasKey("username"))
        {
            PlayerPrefs.SetString("username", username.text);
        }
        else
        {
            PlayerPrefs.SetString("username", username.text);
        }
        print(username.text);
    }
    public void CycleCharacter(int id)
    {
        if (id > 0)
        {
            if (characterIndex == characters.Length-1)
            {
                characterIndex = 0;
            }
            else
            {
                characterIndex++;
            }
            
        }
        else if(id < 0)
        {
            if (characterIndex == 0)
            {
                characterIndex = characters.Length-1;
            }
            else
            {
                characterIndex--;
            }
           
        }
        print(characterIndex);
        selectedCharacter.GetComponent<Image>().sprite = characters[characterIndex];
        PlayerPrefs.SetInt("Character", characterIndex);
    }
    #endregion

    public void ChangeMouseSens(float f)
    {
        PlayerPrefs.SetFloat("Mouse Sensitivity", f * sensMultiplier);
        float s = sensSlider.value;
        sensInput.text = s.ToString("0.###");
    }

    public void ChangeMouseSensInputField()
    {
        float f;
        float.TryParse(sensInput.text, out f);
        if (f < sensSlider.minValue)
            f = sensSlider.minValue; 
        if (f > sensSlider.maxValue)
            f = sensSlider.maxValue;
        sensSlider.value = f;
        ChangeMouseSens(f);
    }
}