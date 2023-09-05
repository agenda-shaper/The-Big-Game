using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingMenu : MonoBehaviour
{
    public GameObject firstCraftPrefab;

    public LocalPlayer localPlayer;

    public HighlightedItem highlightedItem;

    public List<ItemSlot> craftSlots = new List<ItemSlot>();

    private const int MaxCraftSlots = 30;

    public List<ItemSlot> requiredResourcesSlots = new List<ItemSlot>();

    private const int MaxrequiredResourcesSlots = 5;



    private float starting_pos_x = -485f;
    private float starting_pos_y = 287.5f;

    private float required_res_starting_pos_x = 158f;
    private float required_res_starting_pos_y = 150f;

    private float required_res_slotSpacingX = 90f;

    private float slotSpacingX = 116f; // The space between slots in the X direction.
    private float slotSpacingY = 116f; // The space between slots in the Y direction.
    private const int maxSlotsPerRow = 5;



    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < MaxCraftSlots; i++)
        {
            CreateNewCraftSlot();
        }

        for(int i = 0; i < MaxrequiredResourcesSlots; i++)
        {
            CreateRequiredResourceSlot();
        }

        List<Item> items = localPlayer.player.engine.blockManager.GetItemsByType(3);
        FillWithItems(items,craftSlots,ItemSlotType.CraftingMenuItems);
        
    }

    public void CreateNewCraftSlot()
    {

        GameObject newSlot = Instantiate(firstCraftPrefab, transform);
        ItemSlot craftSlotComponent = newSlot.GetComponent<ItemSlot>();
        craftSlotComponent.type = ItemSlotType.CraftingMenuItems;
        craftSlotComponent.craftingMenu = this;

        int currentTotalSlots = craftSlots.Count;
        int row = currentTotalSlots / maxSlotsPerRow;
        int col = currentTotalSlots % maxSlotsPerRow;

        float posX = starting_pos_x + (col * slotSpacingX);
        float posY = starting_pos_y - (row * slotSpacingY);

        newSlot.GetComponent<RectTransform>().localPosition = new Vector3(posX, posY, 0f);


        craftSlots.Add(craftSlotComponent);

        //// inactive for now
        craftSlotComponent.gameObject.SetActive(false);
        

    }
    public void CreateRequiredResourceSlot()
    {

        GameObject newSlot = Instantiate(firstCraftPrefab, transform);
        ItemSlot craftSlotComponent = newSlot.GetComponent<ItemSlot>();
        craftSlotComponent.type = ItemSlotType.RequiredResources;
        craftSlotComponent.craftingMenu = this;

        int currentTotalSlots = requiredResourcesSlots.Count;
        int col = currentTotalSlots % maxSlotsPerRow;

        float posX = required_res_starting_pos_x + (col * required_res_slotSpacingX);
        float posY = required_res_starting_pos_y;

        newSlot.GetComponent<RectTransform>().localPosition = new Vector3(posX, posY, 0f);
        newSlot.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 0f);


        requiredResourcesSlots.Add(craftSlotComponent);

        //// inactive for now
        craftSlotComponent.gameObject.SetActive(false);
        

    }
    public void FillWithItems(List<Item> items, List<ItemSlot> slotsList, ItemSlotType type)
    {
        ClearAllCraftSlots(type);
        for (int i = 0; i < items.Count && i < slotsList.Count; i++)
        {
            ItemSlot currentSlot = slotsList[i];

            // Enable the slot's game object
            currentSlot.gameObject.SetActive(true);

            // Load the item into the slot
            LoadItemInSlot(currentSlot, items[i]);
        }
        if (type == 0){
            // highlight first
            highlightedItem.LoadInfo(slotsList[0]);
        }
        
    }

    public void ClearAllCraftSlots(ItemSlotType type)
    {
        List<ItemSlot> slotsList;

        switch(type){
            case ItemSlotType.CraftingMenuItems:
                slotsList = craftSlots;
                break;
            case ItemSlotType.RequiredResources:
                slotsList = requiredResourcesSlots;
                break;
            default:
                return;
        }

        foreach (ItemSlot slot in slotsList)
        {
            slot.item = null;  // Remove the item reference

            // If you want to clear the slot's visual representation, do something like:
            slot.slotImage.texture = null;  // Clear the texture
            slot.slotImage.enabled = false; // Optionally disable the slot image if you want it hidden

            // If you want to deactivate the slot's game object, do:
            slot.gameObject.SetActive(false);
        }

        
    }




    public void LoadItemInSlot(ItemSlot slot, Item item)
{
    Debug.Log($"Slot: {slot}");
    Debug.Log($"Item: {item}");
    if (item != null)
    {
        Debug.Log($"Item Detail: {item.detail}");
        if (item.detail != null)
        {
            Debug.Log($"Item Detail Name: {item.detail.name}");
        }
        Debug.Log($"Item Img: {item.img}");
        if (item.img != null)
        {
            Debug.Log($"Item Img Source: {item.img.source}");
            if (item.img.source != null && item.img.source.Length > 0)
            {
                Debug.Log($"Item Img Source[0]: {item.img.source[0]}");
            }
        }
    }
    
    slot.item = item;
    LoadItemTexture(slot, slot.item.img.source[0]);
}


    public void LoadItemTexture(ItemSlot slot,string image_source){
        Texture2D loadedTexture = localPlayer.player.inventory.LoadTextureFromPath(image_source);
        if (loadedTexture != null)
        {
            slot.slotImage.texture = loadedTexture;
            slot.slotImage.enabled = true;
        }
            else
        {
            Debug.LogError($"Failed to load texture from path: {image_source}");
        }
    }
    public void HandleItemEnter(ItemSlot slot){
        LoadItemTexture(slot, slot.item.img.source[1]);
    }
    public void HandleItemExit(ItemSlot slot){
        LoadItemTexture(slot, slot.item.img.source[0]);
    }
    public void HandleItemDown(ItemSlot slot){
        LoadItemTexture(slot, slot.item.img.source[2]);
    }
    public void HandleItemUp(ItemSlot slot){

        LoadItemTexture(slot, slot.item.img.source[1]);

        if (slot.type == ItemSlotType.CraftingMenuItems){
            // add loading item
            highlightedItem.LoadInfo(slot);
        }

        
    }


}
