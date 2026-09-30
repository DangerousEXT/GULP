using Steamworks;
using Steamworks.Data;
using System.Linq;
using UnityEngine;
using Zenject;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private PlayerSlotUI[] slots;

    [Inject] private SteamLobbyManager lobbyManager;

    private void Start()
    {
        Debug.Log($"LobbyUI Start, lobbyManager = {(lobbyManager == null ? "NULL" : "OK")}");
        lobbyManager.LobbyEntered += OnLobbyEntered;
        lobbyManager.MemberJoined += OnMemberJoined;
        lobbyManager.MemberLeft += OnMemberLeft;
        Debug.Log($"Slots count: {slots.Length}");
        if (lobbyManager.CurrentLobby.HasValue)
            RefreshAll(lobbyManager.CurrentLobby.Value);
    }

    private void OnDestroy()
    {
        if (lobbyManager == null) 
            return;
        lobbyManager.LobbyEntered -= OnLobbyEntered;
        lobbyManager.MemberJoined -= OnMemberJoined;
        lobbyManager.MemberLeft -= OnMemberLeft;
    }

    private void OnLobbyEntered(Lobby lobby) => RefreshAll(lobby);
    private void OnMemberJoined(Friend friend) => RefreshAll(lobbyManager.CurrentLobby.Value);
    private void OnMemberLeft(Friend friend) => RefreshAll(lobbyManager.CurrentLobby.Value);

    private void RefreshAll(Lobby lobby)
    {
        Debug.Log($"RefreshAll вызван, Members: {lobby.Members.Count()}");
        foreach (var slot in slots)
            slot.gameObject.SetActive(false);

        int i = 0;
        foreach (var member in lobby.Members)
        {
            if (i >= slots.Length) break;
            slots[i].gameObject.SetActive(true);
            slots[i].Setup(member, member.Id == lobby.Owner.Id);
            i++;
        }
    }
}