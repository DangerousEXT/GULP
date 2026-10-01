using Steamworks;
using System;
using UnityEngine;
using Zenject;

public class SteamManager : MonoBehaviour, IInitializable, IDisposable
{
    [SerializeField] private uint appId = 480; //Spacewar

    public void Initialize()
    {
        if (SteamClient.IsValid) return;
        try
        {
            SteamClient.Init(appId, false);
            Debug.Log("Steam инициализирован");
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void Dispose()
    {
        try
        {
            SteamClient.Shutdown();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    private void Update()
    {
        if(SteamClient.IsValid)
            SteamClient.RunCallbacks();
    }
}