using UnityEngine;
using UnityEngine.EventSystems;

public class RawImageHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler,IDragHandler,IBeginDragHandler, IEndDragHandler
{
    public SlotHandler slotHandler; // Reference to the SlotHandler

    public void OnBeginDrag(PointerEventData eventData)
    {
        slotHandler.mainInventory.HandleItemDragBegin(slotHandler.slotNumber);    
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        slotHandler.mainInventory.HandleItemDragEnd(slotHandler.slotNumber);
    }
    public void OnDrag(PointerEventData eventData)
    {
        slotHandler.mainInventory.HandleItemDrag(slotHandler.slotNumber,eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        slotHandler.mainInventory.HandleItemEnter(slotHandler.slotNumber);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        slotHandler.mainInventory.HandleItemExit(slotHandler.slotNumber);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        slotHandler.mainInventory.HandleItemDown(slotHandler.slotNumber);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        slotHandler.mainInventory.HandleItemUp(slotHandler.slotNumber);
    }
}
