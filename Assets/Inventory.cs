using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Examples.Basic;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

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

[System.Serializable]
public class ActionManager
{
    public bool isActing;
    public SlotItem currentSlotItem;    
}

public class Inventory : NetworkBehaviour
{
    public Dictionary<int, SlotItem> slots = new Dictionary<int, SlotItem>();
    public List<GameObject> actionSpaceObjects = new List<GameObject>();

    public int highlightedSlot;
    public Character player;

    public ActionManager actionManager;

    public ActionHandler actionHandler;

    public GameObject droppedItemPrefab;

    public GameObject slotPrefab; // Drag your slot prefab here in the inspector.

    public Transform centerSlots; // Drag the centerSlots transform here in the inspector.


    

    public void SpawnNewSlot()
    {
        //Debug.Log("spawning");
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
            slots[slotNumber].slotHandler.emptyImage.enabled = false;
            slots[slotNumber].slotHandler.slotImage.rectTransform.localPosition = Vector3.zero;

        }
        else
        {
            // Slot does not exist. Handle this situation as you see fit.
        }
    }

    [Server]
    public void PickUpItem(DroppedItem droppedObject) {
        int slotNumber = FindEmptySlot();
        if (slotNumber == -1) return;

        AddItemToSlot(slotNumber,droppedObject.item,droppedObject.count);

        NetworkServer.Destroy(droppedObject.gameObject);

    }

    public int FindEmptySlot()
    {
        foreach (var slot in slots)
        {
            if (slot.Value.item == null)
            {
                return slot.Key;
            }
        }
        return -1; // Return -1 or any suitable value if no empty slot is found
    }
    [Command]
    public void HandleActionSpace(int objectNumber)
    {
        if (objectNumber <= 0 || objectNumber > actionSpaceObjects.Count)
        {
            //Debug.LogError($"No object at position {objectNumber} in the action space.");
            return;
        }

        GameObject objectToHandle = actionSpaceObjects[objectNumber - 1];

        if (objectToHandle.GetComponent<DroppedItem>() != null){
            PickUpItem(objectToHandle.GetComponent<DroppedItem>());

            if (player.inventory.actionSpaceObjects.Contains(objectToHandle)){
                actionSpaceObjects.Remove(objectToHandle);
            }
            
        }
        // Perform other actions with objectToHandle...
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
            if (slots[slotNumber].item == null){
                return;
                // no item in slot
            }
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

    public void SwitchSlotItems(int fromSlot, int toSlot)
    {
        Debug.Log("switching");
        Item tempItem = slots[toSlot].item;
        int tempCount = slots[toSlot].count;

        // Handle item swapping or moving
        slots[toSlot].item = slots[fromSlot].item;
        slots[toSlot].count = slots[fromSlot].count;

        slots[fromSlot].item = tempItem;
        slots[fromSlot].count = tempCount;

        // Update UI for fromSlot
        if (slots[fromSlot].item != null)
        {
            LoadSlotItemImage(fromSlot, slots[fromSlot].item.img.source[0]);
        }
        else
        {
            EmptySlotImage(fromSlot);
            // Remove or hide image for empty slot
        }

        // Update UI for toSlot
        if (slots[toSlot].item != null)
        {
            LoadSlotItemImage(toSlot, slots[toSlot].item.img.source[0]);
        }
        else
        {
            EmptySlotImage(toSlot);

            // Remove or hide image for empty slot
        }
    }





    public void HandleItemDragBegin(int slotNumber)
    {
        if (slots[slotNumber].item == null){
            return;
            // no item in slot
        }

        // Enable the empty-inv image
        slots[slotNumber].slotHandler.emptyImage.enabled = true;
    }

    public void HandleItemDragEnd(int slotNumber, PointerEventData eventData)
    {
        if (slots[slotNumber].item == null){
            return;
            // no item in slot
        }       
        Debug.Log("ended drag");
        
        // Disable the empty-inv image
        slots[slotNumber].slotHandler.emptyImage.enabled = false;

        // Move the main image to the local position (0,0,0)
        slots[slotNumber].slotHandler.slotImage.rectTransform.localPosition = Vector3.zero;



        if (highlightedSlot != -1)
        {
            SwitchSlotItems(slotNumber, highlightedSlot);
        }
        else
        {
            DropItemFromSlot(slotNumber);
        }
    }

    public void HandleItemEnter(int slotNumber)
    {
        highlightedSlot = slotNumber;
        if (slots[slotNumber].item == null){
            return;
            // no item in slot
        }


        // highlight item
        LoadSlotItemImage(slotNumber,slots[slotNumber].item.img.source[1]);
        // Logic for when the mouse pointer enters an item in slotNumber.
    }

    public void HandleItemExit(int slotNumber)
    {
        highlightedSlot = -1;
        if (slots[slotNumber].item == null){
            return;
            // no item in slot
        }
        
        // unhighlight item
        LoadSlotItemImage(slotNumber,slots[slotNumber].item.img.source[0]);
        // Logic for when the mouse pointer exits an item in slotNumber.
    }

    public void HandleItemDown(int slotNumber)
    {
        if (slots[slotNumber].item == null){
            return;
            // no item in slot
        }

        // Logic for when an item in slotNumber is pressed.

        // select pressed item
        LoadSlotItemImage(slotNumber,slots[slotNumber].item.img.source[2]);
    }

    public void HandleItemUp(int slotNumber, PointerEventData eventData)
    {
        if (slots[slotNumber].item == null)
        {
            return; // No item in slot
        }

        // Unselect item back to simple highlight
        LoadSlotItemImage(slotNumber, slots[slotNumber].item.img.source[1]);

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if(slots[slotNumber].item.selection_type > 0 ) actionHandler.HandleSelection(slots[slotNumber]);
            // Do the selection (Code for left-click functionality here)
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            // Logic for dropping item (Code for right-click functionality here)
            DropItemFromSlot(slotNumber);
        }
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
