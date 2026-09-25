using UnityEngine;
using System;

public class SteamManager : MonoBehaviour
{
    [SerializeField] private uint appId = 480; //spacewar
    public bool Init()
    {
        try
        {
            Steamworks.SteamClient.Init(appId, true);
            Debug.Log("steam works");
            return true;
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }
    }

    private void OnApplicationQuit()
    {
        try
        {
            Steamworks.SteamClient.Shutdown();
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }
    }
}
