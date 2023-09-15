using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class ButtonManager : MonoBehaviour
{
    public LocalPlayer localPlayer;

    public GameObject buttonPrefab;

    public GameObject menuNavigationObject;



    public void HandleButtonEnter(ButtonHandler buttonHandler){
       buttonHandler.UpdateButtonImage(1);
    }
    public void HandleButtonExit(ButtonHandler buttonHandler){
       buttonHandler.UpdateButtonImage(0);
    }
    public void HandleButtonDown(ButtonHandler buttonHandler){
      buttonHandler.UpdateButtonImage(2);
    }

  public void HandleButtonUp(ButtonHandler buttonHandler)
  {
    buttonHandler.UpdateButtonImage(1);

    switch (buttonHandler.buttonType)
    {
      case ButtonType.CraftItem:
        // Handle crafting item
        int slot = localPlayer.player.inventory.FindEmptySlot();
        Item highlighted_item = localPlayer.craftingMenu.highlightedItem.item;
        if (slot >= 0){
          localPlayer.player.inventory.AddItemToSlot(slot,highlighted_item,highlighted_item.stack);
        } else {
          // all slots filled - drop the item outside
          // for now first slot
          localPlayer.player.inventory.AddItemToSlot(0,highlighted_item,highlighted_item.stack);
        }
        break;
      case ButtonType.Hidden:
      case ButtonType.Misc:
      case ButtonType.Skill:
      case ButtonType.Survival:
      case ButtonType.Armor:
      case ButtonType.Buildings:
      case ButtonType.Tools:
      case ButtonType.Guns:
      case ButtonType.Nature:
      case ButtonType.Medicine:
      case ButtonType.Materials:
      case ButtonType.Cables:
        // Handle other button types
        // crafting menu list items
        List<Item> items = localPlayer.player.engine.blockManager.GetItemsByType((int)buttonHandler.buttonType);
        localPlayer.craftingMenu.FillWithItems(items, localPlayer.craftingMenu.craftSlots, ItemSlotType.CraftingMenuItems);
        break;
      default:
        // Handle default case
        break;
    }
  }

}
