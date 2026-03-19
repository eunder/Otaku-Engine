using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
public class SteamIntegration : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        try
        {
            Steamworks.SteamClient.Init(2002540);
            Debug.Log(Steamworks.SteamClient.Name);
        }
        catch (System.Exception e)
        {
            
            Debug.Log(e);
        }

    }


    public void UnlockAchivement(string id)
    {
        var ach = new Steamworks.Data.Achievement(id);
        ach.Trigger();
    }

    void Update()
    {
        Steamworks.SteamClient.RunCallbacks();
    }


    void OnApplicationQuit()
    {
        Steamworks.SteamClient.Shutdown();
    }

}
