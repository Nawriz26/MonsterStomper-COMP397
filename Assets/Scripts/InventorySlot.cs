using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Represents a single inventory slot with drag-and-drop support.
/// Uses PointerEventData.position so dragging works on both mouse and touch.
/// Drop targets must implement IDropHandler to consume dragged items.
/// </summary>
public class InventorySlot : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Header("UI References")]
    public Image icon;
    public TMP_Text quantityText;

    [Header("Slot Data")]
    public string itemID;
    public int quantity;

    private Transform    originalParent;
    private Vector3      originalPosition;
    private Canvas       canvas;
    private CanvasGroup  canvasGroup;

    void Awake()
    {
        canvas      = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    /// <summary>Populates this slot with item data.</summary>
    public void SetItem(string id, Sprite sprite, int amount)
    {
        itemID   = id;
        quantity = amount;

        icon.sprite  = sprite;
        icon.enabled = true;

        quantityText.text = amount > 1 ? amount.ToString() : string.Empty;
    }

    /// <summary>Clears this slot back to empty.</summary>
    public void ClearSlot()
    {
        itemID   = null;
        quantity = 0;

        icon.sprite   = null;
        icon.enabled  = false;
        quantityText.text = string.Empty;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (string.IsNullOrEmpty(itemID)) return;

        originalParent   = transform.parent;
        originalPosition = transform.localPosition;

        // Reparent to canvas root so it renders on top of everything
        transform.SetParent(canvas.transform, true);
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha          = 0.75f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Use eventData.position — works for both mouse and touch input
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                canvas.GetComponent<RectTransform>(),
                eventData.position,
                eventData.pressEventCamera,
                out Vector3 worldPoint))
        {
            transform.position = worldPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Return to original slot if not accepted by a drop target
        transform.SetParent(originalParent, true);
        transform.localPosition        = originalPosition;
        canvasGroup.blocksRaycasts     = true;
        canvasGroup.alpha              = 1f;
    }

    /// <summary>
    /// Accepts items dropped onto this slot from another slot (slot swapping).
    /// </summary>
    public void OnDrop(PointerEventData eventData)
    {
        InventorySlot dragged = eventData.pointerDrag?.GetComponent<InventorySlot>();
        if (dragged == null || dragged == this) return;

        // Swap slot contents
        (dragged.itemID,   itemID)   = (itemID,   dragged.itemID);
        (dragged.quantity, quantity) = (quantity, dragged.quantity);
        (dragged.icon.sprite, icon.sprite) = (icon.sprite, dragged.icon.sprite);

        dragged.quantityText.text = dragged.quantity > 1 ? dragged.quantity.ToString() : string.Empty;
        quantityText.text         = quantity         > 1 ? quantity.ToString()         : string.Empty;

        dragged.icon.enabled = dragged.icon.sprite != null;
        icon.enabled         = icon.sprite         != null;
    }
}