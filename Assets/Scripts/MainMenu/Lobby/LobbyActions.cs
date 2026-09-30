using FishNet;
using FishNet.Managing.Scened;
using Steamworks;
using UnityEngine;
using Zenject;
public class LobbyActions : MonoBehaviour
{
    [Inject] private SteamLobbyManager lobbyManager;

    public void CreateLobby() => lobbyManager.CreateLobby();
    public void LeaveLobby() => lobbyManager.LeaveLobby();
    public void OpenInviteOverlay()
    {
        if (!lobbyManager.CurrentLobby.HasValue)
        {
            Debug.LogWarning("Лобби не создано");
            return;
        }
        SteamFriends.OpenGameInviteOverlay(lobbyManager.CurrentLobby.Value.Id);
    }
    public void StartGame()
    {
        if (!InstanceFinder.IsServerStarted) return;
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("MainMenu");
        SceneLoadData sld = new SceneLoadData("Gameplay");
        InstanceFinder.SceneManager.LoadGlobalScenes(sld);
    }
}