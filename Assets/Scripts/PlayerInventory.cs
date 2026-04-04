using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    [SerializeField] private int maxSlots = 10;

    private List<InventoryItem> items = new List<InventoryItem>();

    // ADD THIS (Observer Pattern)
    public Action OnInventoryChanged;

   [SerializeField] private Sprite coinIcon;

    void Start()
    {
        AddItem(new InventoryItem("Coin", coinIcon, "", 5));
    }

    // ADD ITEM (with stacking)
    public bool AddItem(InventoryItem item)
    {
        // Check if item already exists → STACK
        InventoryItem existing = items.Find(i => i.itemName == item.itemName);

        if (existing != null)
        {
            existing.quantity += item.quantity;
        }
        else
        {
            if (items.Count >= maxSlots)
            {
                Debug.Log("Inventory is full!");
                return false;
            }

            items.Add(item);
        }

        Debug.Log($"Added {item.itemName} x{item.quantity}");

        OnInventoryChanged?.Invoke(); // IMPORTANT

        return true;
    }

    public bool RemoveItem(InventoryItem item)
    {
        bool removed = items.Remove(item);

        if (removed)
            OnInventoryChanged?.Invoke();

        return removed;
    }

    public bool HasItem(string itemName)
    {
        return items.Exists(i => i.itemName == itemName);
    }

    public int GetItemCount()
    {
        return items.Count;
    }

    // KEEP THIS (UI will use it)
    public List<InventoryItem> GetAllItems()
    {
        return items;
    }

    public void ClearInventory()
    {
        items.Clear();
        OnInventoryChanged?.Invoke();
    }
    
}

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public Sprite icon;
    public string description;
    public int quantity = 1;

    public InventoryItem(string name, Sprite itemIcon = null, string desc = "", int qty = 1)
    {
        itemName = name;
        icon = itemIcon;
        description = desc;
        quantity = qty;
    }
}