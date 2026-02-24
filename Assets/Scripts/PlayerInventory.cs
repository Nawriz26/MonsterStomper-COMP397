using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    [SerializeField] private int maxSlots = 10;

    private List<InventoryItem> items = new List<InventoryItem>();

    public bool AddItem(InventoryItem item)
    {
        if (items.Count >= maxSlots)
        {
            Debug.Log("Inventory is full!");
            return false;
        }

        items.Add(item);
        Debug.Log($"Added {item.itemName} to inventory");
        return true;
    }

    public bool RemoveItem(InventoryItem item)
    {
        return items.Remove(item);
    }

    public bool HasItem(string itemName)
    {
        return items.Exists(i => i.itemName == itemName);
    }

    public int GetItemCount()
    {
        return items.Count;
    }

    public List<InventoryItem> GetAllItems()
    {
        return new List<InventoryItem>(items);
    }

    public void ClearInventory()
    {
        items.Clear();
    }
}

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public Sprite icon;
    public string description;
    public int quantity = 1;

    public InventoryItem(string name, Sprite itemIcon = null, string desc = "")
    {
        itemName = name;
        icon = itemIcon;
        description = desc;
    }
}
