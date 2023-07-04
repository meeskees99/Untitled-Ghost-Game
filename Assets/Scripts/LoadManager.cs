using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using System;
using FishNet.Managing.Scened;
using TMPro;
public class LoadManager : NetworkBehaviour
{
    [Header("Ui elements")]
    [SerializeField] GameObject _loadUI;
    [SerializeField] TMP_Text _loadingTxt;

    [SerializeField] string _lobbyToUnload;
    public string SceneToUnload
    {
        get
        {
            return _lobbyToUnload;
        }
        set
        {
            _lobbyToUnload = value;
        }
    }

    [SerializeField] bool _isLoading;
    public bool IsLoading
    {
        get
        {
            return _isLoading;
        }
        set
        {
            _isLoading = value;
        }
    }

    public event Action<SceneLoadPercentEventArgs> OnLoadPercentChange;
    private void OnEnable()
    {
        if (SceneManager != null)
            SceneManager.OnLoadPercentChange += Load;
    }

    IEnumerator Loading()
    {
        yield return new WaitForSeconds(0.5f);
        if (_loadingTxt.text.Contains("Loading..."))
        {
            _loadingTxt.text = "Loading.";
        }
        else
        {
            _loadingTxt.text += ".";
        }
        _startedLoad = false;
    }
    void Load(FishNet.Managing.Scened.SceneLoadPercentEventArgs obj)
    {
        print("OnloadChange");
        if (_isLoading)
        {
            print("Is loading at " + obj.Percent * 100 + " %");

            if (obj.Percent != 1)
            {
                _loadUI.SetActive(true);
            }
            else if (obj.Percent == 1)
            {
                //_loadUI.SetActive(false);
                SceneUnloadData lastScene = new SceneUnloadData(_lobbyToUnload);
                base.SceneManager.UnloadGlobalScenes(lastScene);
            }
        }
    }
    bool _startedLoad;
    private void Update()
    {
        if (_isLoading && !_startedLoad)
        {
            _startedLoad = true;
            StartCoroutine(Loading());
        }
    }
}
