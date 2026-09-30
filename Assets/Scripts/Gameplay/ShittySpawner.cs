using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Transporting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShittySpawner : MonoBehaviour
{
    [SerializeField] private NetworkObject playerPrefab;
    [SerializeField] private List<Transform> spawnPoints = new();

    private int nextIndex = 0;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => InstanceFinder.NetworkManager != null);
        Debug.Log("ShittySpawner: NetworkManager найден");

        InstanceFinder.ServerManager.OnRemoteConnectionState += OnClientConnected;

        // Ждём, пока хост подключится и как сервер, и как клиент
        yield return new WaitUntil(() =>
            InstanceFinder.IsServerStarted && InstanceFinder.IsClientStarted);

        Debug.Log("Спавним хоста");
        SpawnPlayer(InstanceFinder.ClientManager.Connection);
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
        var point = GetNextSpawnPoint();
        if (point == null)
        {
            Debug.LogError("ShittySpawner: нет точек спавна!");
            return;
        }

        Debug.Log($"Спавним игрока в {nextIndex}");
        var player = Instantiate(playerPrefab, point.position, point.rotation);
        InstanceFinder.ServerManager.Spawn(player, conn);
    }

    private Transform GetNextSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Count == 0)
            return null;

        var point = spawnPoints[nextIndex];
        nextIndex = (nextIndex + 1) % spawnPoints.Count;
        return point;
    }
}