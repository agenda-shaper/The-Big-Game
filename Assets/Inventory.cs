using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Examples.Basic;
using UnityEngine.EventSystems;

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
        Debug.Log("spawning");
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


    [Server]
    public void DropItemInMap(Item item, int count) {

        Vector3 startPosition = new Vector3(player.transform.position.x,0.6f,player.transform.position.z);

        // add end position for animations
        // or setup some unity thing

        GameObject droppedItem = Instantiate(droppedItemPrefab, startPosition, player.transform.rotation);
        DroppedItem script = droppedItem.GetComponent<DroppedItem>();
        script.count = count;
        script.item = item;
        script.LoadImage(LoadTextureFromPath(item.ground_img));

        NetworkServer.Spawn(droppedItem);
        // add the night day switching

    }

    [Server]
    public void AddItemToSlot(int slotNumber, Item item, int count)
    {
        if (slots.ContainsKey(slotNumber))
        {
            slots[slotNumber].item = item;
            slots[slotNumber].count = count;
            LoadSlotItemImage(slotNumber,item.img.source[0]);

        }
        else
        {
            // Slot does not exist. Handle this situation as you see fit.
        }
    }


    public void LoadSlotItemImage(int slotNumber, string image_source)
    {

        Texture2D loadedTexture = LoadTextureFromPath(image_source);
        if (loadedTexture != null)
        {
            slots[slotNumber].slotHandler.slotImage.texture = loadedTexture;
            slots[slotNumber].slotHandler.emptyImage.enabled = false;
            slots[slotNumber].slotHandler.slotImage.enabled = true;
        }
            else
        {
            Debug.LogError($"Failed to load texture from path: {image_source}");
        }


    }

    public void EmptySlotImage(int slotNumber) {
        if (slots.ContainsKey(slotNumber))
        {
            slots[slotNumber].slotHandler.emptyImage.enabled = true;
            slots[slotNumber].slotHandler.slotImage.enabled = false;
        }
    }


    [Command]
    public void DropItemFromSlot(int slotNumber)
    {
        if (slots.ContainsKey(slotNumber))
        {
            
            // drop it in map

            DropItemInMap(slots[slotNumber].item,slots[slotNumber].count);

            slots[slotNumber].item = null;
            slots[slotNumber].count = 0;
            EmptySlotImage(slotNumber);
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


    public void HandleItemDrag(int slotNumber,PointerEventData eventData)
    {
        slots[slotNumber].slotHandler.emptyImage.enabled = true;
         // Move the image with the mouse
        // slots[slotNumber].slotHandler.slotImage.rectTransform.localPosition = new Vector3(eventData.position.x, eventData.position.y, 0);
        // Debug.Log(slots[slotNumber].slotHandler.slotImage.rectTransform.localPosition);

        RectTransform centerSlotsRect = slots[slotNumber].slotHandler.slotImage.transform.parent.parent.GetComponent<RectTransform>();
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(centerSlotsRect, eventData.position, eventData.pressEventCamera, out localPoint);

        // Get the local position of the slot to subtract its offset
        Vector3 slotOffset = slots[slotNumber].slotHandler.transform.localPosition;

        // Set the local position of the image after negating the slot's offset
        slots[slotNumber].slotHandler.slotImage.rectTransform.localPosition = new Vector3(localPoint.x - slotOffset.x, localPoint.y - slotOffset.y, 0);


    }



    public void HandleItemDragBegin(int slotNumber)
    {
        // Enable the empty-inv image
        slots[slotNumber].slotHandler.emptyImage.enabled = true;
    }

    public void HandleItemDragEnd(int slotNumber)
    {
        // Disable the empty-inv image
        slots[slotNumber].slotHandler.emptyImage.enabled = false;

        // Move the main image to the local position (0,0,0)
        slots[slotNumber].slotHandler.slotImage.rectTransform.localPosition = Vector3.zero;

        DropItemFromSlot(slotNumber);
    }

    public void HandleItemEnter(int slotNumber)
    {
        // highlight item
        LoadSlotItemImage(slotNumber,slots[slotNumber].item.img.source[1]);
        // Logic for when the mouse pointer enters an item in slotNumber.
    }

    public void HandleItemExit(int slotNumber)
    {
        // unhighlight item
        LoadSlotItemImage(slotNumber,slots[slotNumber].item.img.source[0]);
        // Logic for when the mouse pointer exits an item in slotNumber.
    }

    public void HandleItemDown(int slotNumber)
    {
        // Logic for when an item in slotNumber is pressed.

        // select pressed item
        LoadSlotItemImage(slotNumber,slots[slotNumber].item.img.source[2]);
    }

    public void HandleItemUp(int slotNumber)
    {
        // unselect item back to simple highlight
        LoadSlotItemImage(slotNumber,slots[slotNumber].item.img.source[1]);
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
