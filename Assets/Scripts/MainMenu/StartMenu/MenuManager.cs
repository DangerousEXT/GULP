using Steamworks.Data;
using UnityEngine;
using Zenject;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup[] groups;
    [Inject] private SteamLobbyManager lobby;

    void Awake()
    {
        OpenPanel(0);
    }

    void Start()
    {
        lobby.LobbyEntered += OnLobbyEntered;
        if (lobby.CurrentLobby.HasValue) OpenPanel(1);
    }

    void OnDestroy()
    {
        if (lobby != null) lobby.LobbyEntered -= OnLobbyEntered;
    }

    private void OnLobbyEntered(Lobby _) => OpenPanel(1);

    public void OpenPanel(int index)
    {
        for (int i = 0; i < groups.Length; i++)
        {
            var active = i == index;
            groups[i].alpha = active ? 1f : 0f;
            groups[i].interactable = active;
            groups[i].blocksRaycasts = active;
        }
    }
}
