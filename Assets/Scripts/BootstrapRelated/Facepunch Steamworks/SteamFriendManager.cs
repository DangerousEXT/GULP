using Steamworks;
using Steamworks.Data;
using System;
using UnityEngine;
using Zenject;

public class SteamFriendManager : MonoBehaviour, IInitializable, IDisposable
{
    [Inject] private SteamLobbyManager lobbyManager;

    public void Initialize()
    {
        SteamFriends.OnGameLobbyJoinRequested += OnGameLobbyJoinRequested;
        SteamMatchmaking.OnLobbyInvite += OnLobbyInvite;
    }

    public void Dispose()
    {
        SteamFriends.OnGameLobbyJoinRequested -= OnGameLobbyJoinRequested;
        SteamMatchmaking.OnLobbyInvite -= OnLobbyInvite;
    }

    private void OnGameLobbyJoinRequested(Lobby lobby, SteamId steamId)
    {
        lobbyManager.JoinLobby(lobby.Id);
    }

    private void OnLobbyInvite(Friend friend, Lobby lobby)
    {
        Debug.Log($"Инвайт от {friend.Name}");
    }
}