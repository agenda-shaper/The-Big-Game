using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HighlightedItem : MonoBehaviour
{
    public CraftingMenu craftingMenu; // Reference
    public RawImage itemImage;
    public TextMeshPro itemName;
    public TextMeshPro description;
    public TextMeshPro LowestHeader;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<Item> getRequiredResources(Item itemToCheck){
        List<Item> resources = new List<Item>();

        Debug.Log(itemToCheck.detail.name);
        Debug.Log(itemToCheck.detail.requiredResources);

        foreach (RequiredResources resource in itemToCheck.detail.requiredResources)
        {
            Debug.Log(resource);
            int id = resource.itemId;
            int count = resource.count;
            
            Item resourceItem = craftingMenu.localPlayer.player.engine.blockManager.GetItemById(id);
            resources.Add(resourceItem);
        }
        return resources;

    }

    public void LoadInfo(ItemSlot slot){
        Texture2D loadedTexture = craftingMenu.localPlayer.player.inventory.LoadTextureFromPath(slot.item.img.source[0]);
        if (loadedTexture != null)
        {
            itemImage.texture = loadedTexture;
            itemImage.enabled = true;
        }
            else
        {
            Debug.LogError($"Failed to load texture from path: {slot.item.img.source[0]}");
        }
        itemName.text = slot.item.detail.name;
        description.text = slot.item.detail.description;
        if (slot.item.health > 0){
            LowestHeader.text = $"Life: {slot.item.health}";
        } else {
            LowestHeader.text = $"";
            // for now nothing
        }
        

        craftingMenu.FillWithItems(getRequiredResources(slot.item),craftingMenu.requiredResourcesSlots,ItemSlotType.RequiredResources);


    }
}
