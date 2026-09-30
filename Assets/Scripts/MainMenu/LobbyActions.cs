using Steamworks;
using UnityEngine;
using Zenject;

public class LobbyActions : MonoBehaviour
{
    [Inject] private SteamLobbyManager lobbyManager;

    public void CreateLobby() => lobbyManager.CreateLobby();
    public void LeaveLobby() => lobbyManager.LeaveLobby();
    public void OpenInviteOverlay() => SteamFriends.OpenGameInviteOverlay(lobbyManager.CurrentLobby.Value.Id);
}