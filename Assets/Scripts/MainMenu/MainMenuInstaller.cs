using Zenject;

public class MainMenuInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<LobbyUI>().FromComponentInHierarchy().AsSingle();
        Container.Bind<LobbyActions>().FromComponentInHierarchy().AsSingle();
    }
}