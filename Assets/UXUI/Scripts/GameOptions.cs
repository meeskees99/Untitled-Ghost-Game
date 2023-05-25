using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameOptions : MonoBehaviour
{
    [SerializeField] TMP_InputField username;
    [SerializeField] Sprite[] characters;
    [SerializeField] GameObject selectedCharacter;

    int characterIndex;
    private void Start()
    {
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
}