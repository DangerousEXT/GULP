using System.Collections;
using System.Linq;
using FishNet;
using FishNet.Managing.Scened;
using UnityEngine;
using Unity.Multiplayer.PlayMode;

#if UNITY_EDITOR

#endif

public class DevNetworkStarter : MonoBehaviour
{
#if UNITY_EDITOR
    IEnumerator Start()
    {
        yield return new WaitUntil(() => InstanceFinder.NetworkManager != null);

        bool isClient = CurrentPlayer.Tags.Contains("Client");

        if (!isClient)
        {
            InstanceFinder.ServerManager.StartConnection();
            InstanceFinder.ClientManager.StartConnection();
        }
        else
        {
            yield return new WaitForSeconds(1.5f); // дать хосту подняться
            InstanceFinder.ClientManager.StartConnection("localhost");
        }
    }

    void Update()
    {
        // F3 — старт геймплея, жать в окне хоста
        if (Input.GetKeyDown(KeyCode.F3) && InstanceFinder.IsServerStarted)
            InstanceFinder.SceneManager.LoadGlobalScenes(new SceneLoadData("Gameplay"));
    }
#endif
}