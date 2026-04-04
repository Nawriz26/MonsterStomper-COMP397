using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public InventorySlot[] slots;   // Assign in Inspector
    public PlayerInventory playerInventory;

    void Start()
    {
        playerInventory.OnInventoryChanged += UpdateUI;
        UpdateUI();
    }

    void UpdateUI()
    {
        var items = playerInventory.GetAllItems();

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < items.Count)
            {
                var item = items[i];
                slots[i].SetItem(item.itemName, item.icon, item.quantity);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
}