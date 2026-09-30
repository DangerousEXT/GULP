using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    [SerializeField] private SteamManager steamManager;
    [SerializeField] private SteamLobbyManager lobbyManager;
    [SerializeField] private SteamFriendManager friendManager;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<SteamManager>()
            .FromInstance(steamManager)
            .AsSingle();
        Container.BindInterfacesAndSelfTo<SteamLobbyManager>()
            .FromInstance(lobbyManager)
            .AsSingle();
        Container.BindInterfacesAndSelfTo<SteamFriendManager>()
            .FromInstance(friendManager)
            .AsSingle();
    }
}