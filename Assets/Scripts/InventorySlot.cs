using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI References")]
    public Image icon;
    public TMP_Text quantityText;

    [Header("Slot Data")]
    public string itemID;
    public int quantity;

    private Transform originalParent;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void SetItem(string id, Sprite sprite, int amount)
    {
        itemID = id;
        quantity = amount;

        icon.sprite = sprite;
        icon.enabled = true;

        quantityText.text = amount > 1 ? amount.ToString() : "";
    }

    public void ClearSlot()
    {
        itemID = null;
        quantity = 0;

        icon.sprite = null;
        icon.enabled = false;
        quantityText.text = "";
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (string.IsNullOrEmpty(itemID)) return;

        originalParent = transform.parent;
        transform.SetParent(canvas.transform);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(originalParent);
        canvasGroup.blocksRaycasts = true;
        transform.localPosition = Vector3.zero;
    }
}