using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Examples.Basic;
using UnityEngine.EventSystems;
using UnityEngine.Networking;


public class ActionHandler : MonoBehaviour
{
    public Inventory inventory;

    public GameObject projectilePrefab;

    public void exitLastAction() {
            switch (inventory.actionManager.currentSlotItem.item.selection_type) {
                case 21:
                    inventory.player.localPlayer.buildingManager.exitBuilding();
                    break;
                default:
                    // deselect holding item
                    break;
            }
        }


    public void HandleSelection(SlotItem slotItem){
        if (inventory.actionManager.isActing) exitLastAction();

        if (inventory.actionManager.isActing && inventory.actionManager.currentSlotItem == slotItem){
            // firstly deselect slotItem if its the same and in action
            inventory.actionManager.isActing = false;
            inventory.actionManager.currentSlotItem = null;
            return;
        }


        switch (slotItem.item.selection_type) {
            case 21:
                inventory.player.localPlayer.buildingManager.startBuilding(slotItem);
                break;
            default:
                // pick up in hand
                break;
        }
        inventory.actionManager.isActing = true;
        inventory.actionManager.currentSlotItem = slotItem;
    }

    public void HandleUseAction(SlotItem slotItem)
    {
        switch (slotItem.item.selection_type) {
            case 1:
                HandleStonePick(slotItem);
                break;
            case 2:
                HandleMetalPick(slotItem);
                break;
            case 3:
                HandleHatchet(slotItem);
                break;
            case 4:
                HandleMetalAxe(slotItem);
                break;
            case 5:
                HandleWoodSpear(slotItem);
                break;
            case 6:
                HandleWoodBow(slotItem);
                break;
            case 7:
                HandleShotgun(slotItem);
                break;
            case 8:
                Handle9MM(slotItem);
                break;
            case 9:
                HandleDesertEagle(slotItem);
                break;
            case 10:
                HandleAK47(slotItem);
                break;
            case 11:
                HandleSniper(slotItem);
                break;
            case 12:
                HandleRawSteak(slotItem);
                break;
            case 13:
                HandleCookedSteak(slotItem);
                break;
            case 14:
                HandleRottenSteak(slotItem);
                break;
            case 15:
                HandleOrange(slotItem);
                break;
            case 16:
                HandleRottenOrange(slotItem);
                break;
            case 17:
                HandleMedkit(slotItem);
                break;
            case 18:
                HandleBandage(slotItem);
                break;
            case 19:
                HandleSoda(slotItem);
                break;
            case 20:
                HandleMP5(slotItem);
                break;
            case 21:
                // building
                // cant pick in hands
                break;
            case 22:
                HandleSulfurPick(slotItem);
                break;
            case 23:
                HandleHammer(slotItem);
                break;
            case 24:
                HandleRepairHammer(slotItem);
                break;
            case 25:
                HandleTomatoSoup(slotItem);
                break;
            default:
                // Handle default case
                // for now exit
                inventory.actionManager.isActing = false;
                inventory.actionManager.currentSlotItem = null;
                return;
        }
    }


    public void shootProjectile(int pDamage, int bDamage, float projSpeed) {
        GameObject projectile = Instantiate(projectilePrefab, inventory.player.transform.position, transform.rotation); // Instantiate projectile
        Projectile projScript = projectile.GetComponent<Projectile>();
        projScript.Init(pDamage,bDamage,projSpeed);
        inventory.player.projectiles.Add(projScript); // Add to player's list
    }

    public void HandleAK47(SlotItem item) {
        // add bullet image 
        shootProjectile(1, 2, 0.1f);
    }
    public void HandleSniper(SlotItem item){

    }
    public void HandleMedkit(SlotItem item){

    }
    public void HandleBandage(SlotItem item){

    }
    public void HandleSoda(SlotItem item){

    }
    public void HandleMP5(SlotItem item){

    }
    public void HandleSulfurPick(SlotItem item){

    }
    public void HandleHammer(SlotItem item){

    }
    public void HandleRepairHammer(SlotItem item){

    }
    public void HandleTomatoSoup(SlotItem item){

    }
    public void HandleStonePick(SlotItem item){

    }
    public void HandleMetalPick(SlotItem item){

    }
    public void HandleHatchet(SlotItem item){

    }
    public void HandleMetalAxe(SlotItem item){

    }
    public void HandleWoodSpear(SlotItem item){

    }
    public void HandleWoodBow(SlotItem item){

    }
    public void HandleShotgun(SlotItem item){

    }
    public void Handle9MM(SlotItem item){

    }
    public void HandleDesertEagle(SlotItem item){

    }
    public void HandleRawSteak(SlotItem item){

    }
    public void HandleCookedSteak(SlotItem item){

    }
    public void HandleRottenSteak(SlotItem item){

    }
    public void HandleOrange(SlotItem item){

    }
    public void HandleRottenOrange(SlotItem item){

    }

}