using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

    public enum ItemSlotType{
        CraftingMenuItems,
        RequiredResources,
        RequiredMachines,
        ItemsInChest,

    }
public class ItemSlot : MonoBehaviour
{

    public CraftingMenu craftingMenu; // Reference
    public RawImage slotImage;
    public Item item;

    public ItemSlotType type;

    // 0 - C menu | 1 - required resources for craft | 2 - Item IN chest | 3 - highlight mesh of crafting menu selected item 



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
