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
    public void HandleButtonUp(ButtonHandler buttonHandler){

        buttonHandler.UpdateButtonImage(1);

        int block_type = (int)buttonHandler.buttonType;

        Debug.Log(block_type);

        List<Item> items = localPlayer.player.engine.blockManager.GetItemsByType(block_type);
        Debug.Log(buttonHandler.buttonType);
        Debug.Log(items);

        localPlayer.craftingMenu.FillWithItems(items,localPlayer.craftingMenu.craftSlots,ItemSlotType.CraftingMenuItems);

        
    }

}
