using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Transporting;
using System.Collections;
using UnityEngine;

public class ShittySpawner : MonoBehaviour
{
    [SerializeField] private NetworkObject playerPrefab;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => InstanceFinder.NetworkManager != null);
        Debug.Log("ShittySpawner: NetworkManager найден");
        InstanceFinder.ServerManager.OnRemoteConnectionState += OnClientConnected;
        if (InstanceFinder.IsServerStarted && InstanceFinder.IsClientStarted)
        {
            Debug.Log("Спавним хоста");
            SpawnPlayer(InstanceFinder.ClientManager.Connection);
        }
    }

    private void OnDestroy()
    {
        if (InstanceFinder.ServerManager != null)
            InstanceFinder.ServerManager.OnRemoteConnectionState -= OnClientConnected;
    }

    private void OnClientConnected(NetworkConnection conn, RemoteConnectionStateArgs args)
    {
        Debug.Log($"Клиент подключился: {args.ConnectionState}");
        if (args.ConnectionState != RemoteConnectionState.Started) return;
        SpawnPlayer(conn);
    }

    private void SpawnPlayer(NetworkConnection conn)
    {
        Debug.Log("Спавним игрока");
        var player = Instantiate(playerPrefab,
            new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f)),
            Quaternion.identity);
        InstanceFinder.ServerManager.Spawn(player, conn);
    }
}