using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Examples.Basic;

[System.Serializable]
public class SlotItem
{
    [SyncVar]
    public Item item; // If this is null, it means the slot is empty.

    [SyncVar]
    public GameObject slotObject;

    [SyncVar]
    public int count;

    [SyncVar]
    public SlotHandler slotHandler;

    // Add more slot-specific data if needed.
}


public class Inventory : NetworkBehaviour
{
    public Dictionary<int, SlotItem> slots = new Dictionary<int, SlotItem>();

    public Character player;

    public GameObject droppedItemPrefab;

    public GameObject slotPrefab; // Drag your slot prefab here in the inspector.

    public Transform centerSlots; // Drag the centerSlots transform here in the inspector.

    public void SpawnNewSlot()
    {
        Debug.Log("spwaning");
        GameObject newSlotGO = Instantiate(slotPrefab, centerSlots);
        SlotHandler slotScript = newSlotGO.GetComponent<SlotHandler>();
        slotScript.slotNumber = slots.Count; // Using the current count of slots as the next slot number.
        slotScript.mainInventory = this;

        SlotItem newSlot = new SlotItem();
        newSlot.slotObject = newSlotGO;
        newSlot.slotHandler = slotScript;
        slots.Add(slotScript.slotNumber, newSlot); // Add the new slot to the dictionary.
        OrganizeSlots();
        
    }

    public void OrganizeSlots()
    {
        if (centerSlots.childCount == 0) return;

        float slotWidth = slotPrefab.GetComponent<RectTransform>().rect.width;
        float spacing = 12f; // Fixed spacing between slots. Adjust as needed.

        float totalWidth = (centerSlots.childCount - 1) * spacing + centerSlots.childCount * slotWidth;
        float startX = -totalWidth / 2 + slotWidth / 2;

        for (int i = 0; i < centerSlots.childCount; i++)
        {
            RectTransform rect = centerSlots.GetChild(i).GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(startX + i * (spacing + slotWidth), 0);
        }
    }


    public Texture2D LoadTextureFromPath(string pathWithExtension)
    {
        // Strip off the .png extension if it exists
        string pathWithoutExtension = pathWithExtension.EndsWith(".png") ? pathWithExtension.Substring(0, pathWithExtension.Length - 4) : pathWithExtension;

        Texture2D texture = Resources.Load<Texture2D>(pathWithoutExtension);
        if (texture == null)
        {
            Debug.LogError($"Failed to load texture at path: {pathWithExtension}");
        }

        return texture;
    }

    public void changeInventoryImage(SlotItem slotItem, string image_source){
        Texture2D loadedTexture = LoadTextureFromPath(image_source);
        if (loadedTexture != null)
        {
            slotItem.slotHandler.slotImage.texture = loadedTexture;
            slotItem.slotHandler.slotImage.gameObject.SetActive(true); // Ensure it's visible
        }
            else
        {
            Debug.LogError($"Failed to load texture from path: {image_source}");
        }
    }

    [Server]
    public void DropItem(int slotNumber){
        SlotItem slotItem = slots[slotNumber];

        GameObject droppedItem = Instantiate(droppedItemPrefab, player.transform.position, Quaternion.identity);
        DroppedItem script = droppedItem.GetComponent<DroppedItem>();
        script.count = slotItem.count;
        script.item = slotItem.item;
        script.LoadImage(LoadTextureFromPath(slotItem.item.img.source[0]));

    }

    [Server]
    public void AddItemToSlot(int slotNumber, Item item)
    {
        if (slots.ContainsKey(slotNumber))
        {
            slots[slotNumber].item = item;

            changeInventoryImage(slots[slotNumber],item.img.source[0]);

            
            
            // handle if theres existing items switch their places or drop it
        }
        else
        {
            // Slot does not exist. Handle this situation as you see fit.
        }
    }

    [Server]
    public void RemoveItemFromSlot(int slotNumber)
    {
        if (slots.ContainsKey(slotNumber))
        {
            slots[slotNumber].item = null;
            // drop it in map
        }
        else
        {
            // Slot does not exist. Handle this situation as you see fit.
        }
    }

    [Server]
    public bool ContainsItem(Item targetItem)
    {
        foreach (KeyValuePair<int, SlotItem> slotEntry in slots)
        {
            if (slotEntry.Value.item != null && slotEntry.Value.item.detail.name == targetItem.detail.name)
            {
                return true;
            }
        }
        return false;
    }




    public void HandleItemEnter(int slotNumber)
    {
        // highlight item
        changeInventoryImage(slots[slotNumber],slots[slotNumber].item.img.source[1]);
        // Logic for when the mouse pointer enters an item in slotNumber.
    }

    public void HandleItemExit(int slotNumber)
    {
        // unhighlight item
        changeInventoryImage(slots[slotNumber],slots[slotNumber].item.img.source[0]);
        // Logic for when the mouse pointer exits an item in slotNumber.
    }

    public void HandleItemDown(int slotNumber)
    {
        // Logic for when an item in slotNumber is pressed.

        // select pressed item
        changeInventoryImage(slots[slotNumber],slots[slotNumber].item.img.source[2]);
    }

    public void HandleItemUp(int slotNumber)
    {
        // unselect item back to simple highlight
        changeInventoryImage(slots[slotNumber],slots[slotNumber].item.img.source[1]);
        // Logic for when an item in slotNumber is released.

        // do the selection
    }

    // Method to display the items in the inventory
    public void DisplayInventory()
    {
        Debug.Log("Items in inventory:");
        foreach (KeyValuePair<int, SlotItem> slotEntry in slots)
        {
            if (slotEntry.Value.item != null)
            {
                Debug.Log("Slot " + slotEntry.Key + ": " + slotEntry.Value.item.detail.name);
            }
            else
            {
                Debug.Log("Slot " + slotEntry.Key + " is empty.");
            }
        }
    }

}
