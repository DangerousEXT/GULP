using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Object;
using UnityEngine;

public class ShittySpawner : MonoBehaviour
{
    [SerializeField] private NetworkObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints;

    private NetworkManager nm;
    private readonly HashSet<NetworkConnection> spawned = new();

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => InstanceFinder.NetworkManager != null);
        nm = InstanceFinder.NetworkManager;

        // на чистом клиенте спавнить нечего
        if (!nm.IsServerStarted) yield break;

        nm.SceneManager.OnClientPresenceChangeEnd += OnPresenceChanged;

        // те, кто уже в этой сцене
        foreach (var conn in nm.ServerManager.Clients.Values)
        {
            if (conn.Scenes.Contains(gameObject.scene))
                SpawnFor(conn);
        }
    }

    private void OnDestroy()
    {
        if (nm != null)
            nm.SceneManager.OnClientPresenceChangeEnd -= OnPresenceChanged;
    }

    private void OnPresenceChanged(ClientPresenceChangeEventArgs args)
    {
        if (!args.Added) return;
        if (args.Scene != gameObject.scene) return;
        SpawnFor(args.Connection);
    }

    private void SpawnFor(NetworkConnection conn)
    {
        if (!spawned.Add(conn)) return; // защита от дублей

        var point = spawnPoints[(spawned.Count - 1) % spawnPoints.Length];
        var nob = Instantiate(playerPrefab, point.position, point.rotation);
        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(nob.gameObject, gameObject.scene);
        nm.ServerManager.Spawn(nob, conn);
    }
}