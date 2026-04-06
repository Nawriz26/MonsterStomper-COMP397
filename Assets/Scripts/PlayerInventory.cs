using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    [SerializeField] private int maxSlots = 10;

    private List<InventoryItem> items = new List<InventoryItem>();

    public Action OnInventoryChanged;

    [SerializeField] private Sprite coinIcon;

    void Start()
    {
        AddItem(new InventoryItem("Coin", coinIcon, "Collectible currency.", 5));
    }

    /// <summary>Adds an item to the inventory, stacking if it already exists.</summary>
    public bool AddItem(InventoryItem item)
    {
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
        OnInventoryChanged?.Invoke();
        return true;
    }

    /// <summary>Removes one item by name and triggers its use effect.</summary>
    public bool UseItem(string itemName)
    {
        InventoryItem item = items.Find(i => i.itemName == itemName);
        if (item == null) return false;

        ApplyItemEffect(item);

        item.quantity--;
        if (item.quantity <= 0)
            items.Remove(item);

        OnInventoryChanged?.Invoke();
        return true;
    }

    /// <summary>Applies the gameplay effect of a consumed item.</summary>
    private void ApplyItemEffect(InventoryItem item)
    {
        switch (item.itemName)
        {
            case "HealthPotion":
                PlayerHealth health = GetComponent<PlayerHealth>();
                if (health != null)
                    health.Heal(30);
                break;

            default:
                Debug.Log($"Used: {item.itemName} — no effect defined.");
                break;
        }
    }

    public bool RemoveItem(InventoryItem item)
    {
        bool removed = items.Remove(item);
        if (removed)
            OnInventoryChanged?.Invoke();
        return removed;
    }

    public bool HasItem(string itemName)
        => items.Exists(i => i.itemName == itemName);

    public int GetItemCount()
        => items.Count;

    public List<InventoryItem> GetAllItems()
        => items;

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
    public int    quantity = 1;

    public InventoryItem(string name, Sprite itemIcon = null, string desc = "", int qty = 1)
    {
        itemName    = name;
        icon        = itemIcon;
        description = desc;
        quantity    = qty;
    }
}