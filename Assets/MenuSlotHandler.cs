using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuSlotHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{

    public CraftSlot craftSlot;
    public void OnPointerEnter(PointerEventData eventData)
    {
        craftSlot.craftingMenu.HandleItemEnter(craftSlot);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        craftSlot.craftingMenu.HandleItemExit(craftSlot);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        craftSlot.craftingMenu.HandleItemDown(craftSlot);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        craftSlot.craftingMenu.HandleItemUp(craftSlot);
    }
}
