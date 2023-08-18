using UnityEngine;
using UnityEngine.EventSystems;

public class RawImageHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public SlotHandler slotHandler; // Reference to the SlotHandler

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
