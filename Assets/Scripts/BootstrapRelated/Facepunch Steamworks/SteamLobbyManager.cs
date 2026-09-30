using FishNet;
using Steamworks;
using Steamworks.Data;
using System;
using UnityEngine;
using Zenject;

public class SteamLobbyManager : MonoBehaviour, IInitializable, IDisposable
{
    [SerializeField] private int maxPlayers = 4;
    private Lobby? currentLobby;
    public Lobby? CurrentLobby => currentLobby;

    public event Action<Lobby> LobbyEntered;
    public event Action<Friend> MemberJoined;
    public event Action<Friend> MemberLeft;

    public void Initialize()
    {
        SteamMatchmaking.OnLobbyCreated += OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave += OnLobbyMemberLeave;
        SteamMatchmaking.OnLobbyMemberDisconnected += OnLobbyMemberDisconnected;
    }

    public void Dispose()
    {
        SteamMatchmaking.OnLobbyCreated -= OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered -= OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined -= OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave -= OnLobbyMemberLeave;
        SteamMatchmaking.OnLobbyMemberDisconnected -= OnLobbyMemberDisconnected;
    }

    public async void CreateLobby()
    {
        try
        {
            var result = await SteamMatchmaking.CreateLobbyAsync(maxPlayers);
            if (!result.HasValue)
            {
                Debug.LogError("Не удалось создать лобби");
                return;
            }
            currentLobby = result.Value;
            currentLobby.Value.SetFriendsOnly();
            currentLobby.Value.SetJoinable(true);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    public async void JoinLobby(SteamId lobbyId)
    {
        try
        {
            var result = await SteamMatchmaking.JoinLobbyAsync(lobbyId);
            if (!result.HasValue)
            {
                Debug.LogError("Не удалось присоединиться");
                return;
            }
            currentLobby = result.Value;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    public void LeaveLobby()
    {
        if (!currentLobby.HasValue) return;
        currentLobby.Value.Leave();
        currentLobby = null;
    }

    private void OnLobbyCreated(Result result, Lobby lobby)
    {
        if (result != Result.OK) return;
        Debug.Log("Запускаем сервер");
        InstanceFinder.ServerManager.StartConnection();
        InstanceFinder.ClientManager.StartConnection(SteamClient.SteamId.ToString());
    }

    private void OnLobbyEntered(Lobby lobby)
    {
        currentLobby = lobby;
        LobbyEntered?.Invoke(lobby);
        if (lobby.Owner.Id == SteamClient.SteamId) 
            return;
        InstanceFinder.ClientManager.StartConnection(lobby.Owner.Id.ToString());
    }

    private void OnLobbyMemberJoined(Lobby lobby, Friend friend)
    {
        MemberJoined?.Invoke(friend);
    }

    private void OnLobbyMemberLeave(Lobby lobby, Friend friend)
    {
        MemberLeft?.Invoke(friend);
    }

    private void OnLobbyMemberDisconnected(Lobby lobby, Friend friend)
    {
        MemberLeft?.Invoke(friend);
    }
}