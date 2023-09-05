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
    public TextMeshPro life;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
        life.text = $"Life: {slot.item.health}";

    }
}
