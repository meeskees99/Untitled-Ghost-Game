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
    [SerializeField] GameObject _canvas;

    [SerializeField] string _sceneToLoad;
    public string SceneToLoad
    {
        get
        {
            return _sceneToLoad;
        }
        set
        {
            _sceneToLoad = value;
        }
    }

    [SerializeField] bool _isloading;
    public bool Isloading
    {
        get
        {
            return _isloading;
        }
        set
        {
            _isloading = value;
        }
    }

    [SerializeField] string _sceneToUnload;
    public string SceneToUnload
    {
        get
        {
            return _sceneToUnload;
        }
        set
        {
            _sceneToUnload = value;
        }
    }

    [SerializeField] bool _startLoading;
    public bool StartLoading
    {
        get
        {
            return _startLoading;
        }
        set
        {
            _startLoading = value;
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
        if (_isloading)
        {
            print("Is loading at " + obj.Percent * 100 + " %");

            if (obj.Percent != 1)
            {
                if (_loadUI != null)
                    _loadUI.SetActive(true);
                if (_canvas != null)
                    _canvas.SetActive(false);
            }
            else if (obj.Percent == 1)
            {
                StartLoading = false;
                Isloading = false;

                if (_loadUI != null)
                    _loadUI.SetActive(false);
                if (_canvas != null)
                    _canvas.SetActive(true);

                SceneUnloadData lastScene = new SceneUnloadData(_sceneToUnload);
                base.SceneManager.UnloadGlobalScenes(lastScene);
            }
        }
    }
    void StartLoad()
    {
        SceneLoadData sld = new SceneLoadData(_sceneToLoad);
        base.SceneManager.LoadGlobalScenes(sld);
        Isloading = true;
    }

    bool _startedLoad;
    private void Update()
    {
        if (_startLoading)
        {
            StartLoad();
        }
        if (_startLoading && !_startedLoad)
        {
            _startedLoad = true;
            StartCoroutine(Loading());
        }
    }
}
