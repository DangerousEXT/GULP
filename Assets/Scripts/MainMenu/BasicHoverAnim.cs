using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BasicHoverAnim : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //TODO: решить проблему мерцания при наведении на самую левую часть кнопки (анимация бесконечно начинается и заканчивается)

    [SerializeField] private float moveDuration;
    [SerializeField] private Ease moveEase;
    [SerializeField] private float offsetX;

    private Vector3 originalPosition;

    private void Awake()
    {
        originalPosition = transform.localPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOLocalMoveX(originalPosition.x + offsetX, moveDuration).SetEase(moveEase);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOLocalMoveX(originalPosition.x, moveDuration).SetEase(moveEase);
    }
}