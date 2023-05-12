using FishNet.Managing.Scened;
using FishNet;
using FishNet.Managing.Logging;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStack : MonoBehaviour
{
    public const string scene_name = "Gerlof test";
    [SerializeField] private int _stackedSceneHandle = 0;

    [Server(Logging = LoggingType.Off)]

    private void OnTriggerEnter(Collider other)
    {
        print("ba");
        NetworkObject nob = other.GetComponent<NetworkObject>();
        if (nob != null)
        {
            LoadScene(nob);
        }
    }
    private void LoadScene(NetworkObject nob)
    {
        if (!nob.Owner.IsActive)
        {
            return;
        }
        
        SceneLookupData lookup;
        Debug.Log("loading by handle ? " + (_stackedSceneHandle != 0));
        if (_stackedSceneHandle != 0)
        {
            lookup = new SceneLookupData(_stackedSceneHandle);
        }
        else
        {
            lookup = new SceneLookupData(scene_name);
        }
        lookup = new SceneLookupData(scene_name);
        SceneLoadData sld = new SceneLoadData(lookup);
        sld.Options.AllowStacking = true;

        sld.Options.LocalPhysics = LocalPhysicsMode.Physics3D;

        sld.MovedNetworkObjects = new NetworkObject[] { nob };
        sld.ReplaceScenes = ReplaceOption.All;
        InstanceFinder.SceneManager.LoadConnectionScenes(nob.Owner, sld);
    }

    public bool sceneStack = false;

    private void Start()
    {
        InstanceFinder.SceneManager.OnLoadEnd += SceneManager_OnloadEnd;
    }
    private void OnDestroy()
    {
        if (InstanceFinder.SceneManager != null)
        {
            InstanceFinder.SceneManager.OnLoadEnd -= SceneManager_OnloadEnd;
        }
    }
    private void SceneManager_OnloadEnd(SceneLoadEndEventArgs obj)
    {
        if (!obj.QueueData.AsServer)
        {
            return;
        }
        if (!sceneStack)
        {
            return;
        }
        if (_stackedSceneHandle != 0)
        {
            return;
        }
        if (obj.LoadedScenes.Length > 0 )
        {
            _stackedSceneHandle = obj.LoadedScenes[0].handle;
        }
    }
}
