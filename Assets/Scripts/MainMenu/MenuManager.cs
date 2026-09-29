using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup[] groups;

    void Awake()
    {
        OpenPanel(0);
    }
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
