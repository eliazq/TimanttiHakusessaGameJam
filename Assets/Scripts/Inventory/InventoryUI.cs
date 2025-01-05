using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryUI : MonoBehaviour
{
    Inventory inventory;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject itemSlotParent;
    [SerializeField] public static Sprite emptyInventoryIcon;
    private List<ItemSlot> itemSlots = new List<ItemSlot>();
    public bool isVisible { get { return inventoryUI.activeSelf; } }

    private void Awake()
    {
        inventory = GetComponent<Inventory>();
        for (int i = 0; i < itemSlotParent.transform.childCount; i++)
        {
            itemSlots.Add(itemSlotParent.transform.GetChild(i).GetComponent<ItemSlot>());
        }

        GetComponent<Inventory>().OnInventoryChanged += InventoryUI_OnInventoryChanged;

        emptyInventoryIcon = itemSlots[0].itemSprite;

        ItemSlot.OnAnyItemSlotSelected += ItemSlot_OnAnyItemSlotSelected;

        Item.OnAnyItemAmountChanged += Item_OnAnyItemAmountChanged;
    }

    private void Item_OnAnyItemAmountChanged(object sender, EventArgs e)
    {
        UpdateInventoryUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    private void ItemSlot_OnAnyItemSlotSelected(object sender, EventArgs e)
    {
        // Ensure the sender is a valid ItemSlot
        if (sender is not ItemSlot selectedItemSlot) return;

        foreach (ItemSlot itemSlot in itemSlots)
        {
            // Deselect all item slots except the one that triggered the event
            if (itemSlot.Selected && itemSlot != selectedItemSlot)
            {
                ItemSlot.DeSelect(itemSlot);
            }
        }
    }

    private void OnDestroy()
    {
        ItemSlot.OnAnyItemSlotSelected -= ItemSlot_OnAnyItemSlotSelected;
        Item.OnAnyItemAmountChanged -= Item_OnAnyItemAmountChanged;
    }

    private void InventoryUI_OnInventoryChanged(object sender, System.EventArgs e)
    {
        UpdateInventoryUI();        
    }

    private void ToggleInventory()
    {
        inventoryUI.SetActive(!inventoryUI.activeSelf);
    }

    private void UpdateInventoryUI()
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            ItemSlot itemSlot = itemSlots[i];
            if (inventory.TryGetItem(i, out Item item))
            {
                itemSlot.item = item;
            }
            else
            {
                itemSlot.ClearItem();
            }
        }
    }

}
