using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapEntryPoint : MonoBehaviour
{
    [SerializeField] private SteamManager steamManager;
    void Start()
    {
        //паттерн единая точка входа, не переходим на новую сцену, пока все не инициализируем
        steamManager.Init();
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
    }
}
