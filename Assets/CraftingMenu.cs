using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingMenu : MonoBehaviour
{
    public GameObject firstCraftPrefab;

    public LocalPlayer localPlayer;

    public HighlightedItem highlightedItem;

    public List<CraftSlot> craftSlots = new List<CraftSlot>();

    private float starting_pos_x = -485f;
    private float starting_pos_y = 287.5f;

    private float slotSpacingX = 116f; // The space between slots in the X direction.
    private float slotSpacingY = 116f; // The space between slots in the Y direction.
    private const int maxSlotsPerRow = 5;



    // Start is called before the first frame update
    void Start()
    {
        int numberOfSlotsToFill = 30;

        for(int i = 0; i < numberOfSlotsToFill; i++)
        {
            CreateNewCraftSlot();
        }
        List<Item> items = localPlayer.player.engine.blockManager.GetItemsByType(3);
        FillWithItems(items);
    }

    public void CreateNewCraftSlot()
    {
        GameObject newSlot = Instantiate(firstCraftPrefab, transform);
        CraftSlot craftSlotComponent = newSlot.GetComponent<CraftSlot>();
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
    public void FillWithItems(List<Item> items)
    {
        ClearAllCraftSlots();
        for (int i = 0; i < items.Count && i < craftSlots.Count; i++)
        {
            CraftSlot currentSlot = craftSlots[i];

            // Enable the slot's game object
            currentSlot.gameObject.SetActive(true);

            // Load the item into the slot
            LoadItemInSlot(currentSlot, items[i]);
        }
        // highlight first
        highlightedItem.LoadInfo(craftSlots[0]);
    }

    public void ClearAllCraftSlots()
    {
        foreach (CraftSlot slot in craftSlots)
        {
            slot.item = null;  // Remove the item reference

            // If you want to clear the slot's visual representation, do something like:
            slot.slotImage.texture = null;  // Clear the texture
            slot.slotImage.enabled = false; // Optionally disable the slot image if you want it hidden

            // If you want to deactivate the slot's game object, do:
            slot.gameObject.SetActive(false);
        }

        
    }




    public void LoadItemInSlot(CraftSlot slot, Item item)
    {
        slot.item = item;
        LoadItemTexture(slot, slot.item.img.source[0]);
    }

    public void LoadItemTexture(CraftSlot slot,string image_source){
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
    public void HandleItemEnter(CraftSlot slot){
        LoadItemTexture(slot, slot.item.img.source[1]);
    }
    public void HandleItemExit(CraftSlot slot){
        LoadItemTexture(slot, slot.item.img.source[0]);
    }
    public void HandleItemDown(CraftSlot slot){
        LoadItemTexture(slot, slot.item.img.source[2]);
    }
    public void HandleItemUp(CraftSlot slot){
        LoadItemTexture(slot, slot.item.img.source[1]);

        // add loading item
        highlightedItem.LoadInfo(slot);
    }


}
