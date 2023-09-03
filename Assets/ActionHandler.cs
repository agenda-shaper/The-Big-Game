using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Examples.Basic;
using UnityEngine.EventSystems;
using UnityEngine.Networking;


public class ActionHandler : MonoBehaviour
{
    public Inventory inventory;
    public bool actionContinous = true;
    public bool isActing;
    public SlotItem currentSlotItem;  

    public GameObject projectilePrefab;

    public void exitLastAction() {
            switch (currentSlotItem.item.selection_type) {
                case 21:
                    inventory.player.localPlayer.buildingManager.exitBuilding();
                    break;
                default:
                    // deselect holding item
                    break;
            }
        }


    public void HandleSelection(SlotItem slotItem){
        if (isActing) exitLastAction();

        if (isActing && currentSlotItem == slotItem){
            // firstly deselect slotItem if its the same and in action
            isActing = false;
            currentSlotItem = null;
            inventory.holdingItem.gameObject.SetActive(false);
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

        inventory.holdingItem.LoadImage(inventory.LoadTextureFromPath(slotItem.item.ground_img));

        inventory.holdingItem.gameObject.SetActive(true);
        isActing = true;
        currentSlotItem = slotItem;
    }

    public void HandleUseAction()
    {
        switch (currentSlotItem.item.selection_type) {
            case 1:
                HandleStonePick(currentSlotItem);
                break;
            case 2:
                HandleMetalPick(currentSlotItem);
                break;
            case 3:
                HandleHatchet(currentSlotItem);
                break;
            case 4:
                HandleMetalAxe(currentSlotItem);
                break;
            case 5:
                HandleWoodSpear(currentSlotItem);
                break;
            case 6:
                HandleWoodBow(currentSlotItem);
                break;
            case 7:
                HandleShotgun(currentSlotItem);
                break;
            case 8:
                Handle9MM(currentSlotItem);
                break;
            case 9:
                HandleDesertEagle(currentSlotItem);
                break;
            case 10:
                HandleAK47(currentSlotItem);
                break;
            case 11:
                HandleSniper(currentSlotItem);
                break;
            case 12:
                HandleRawSteak(currentSlotItem);
                break;
            case 13:
                HandleCookedSteak(currentSlotItem);
                break;
            case 14:
                HandleRottenSteak(currentSlotItem);
                break;
            case 15:
                HandleOrange(currentSlotItem);
                break;
            case 16:
                HandleRottenOrange(currentSlotItem);
                break;
            case 17:
                HandleMedkit(currentSlotItem);
                break;
            case 18:
                HandleBandage(currentSlotItem);
                break;
            case 19:
                HandleSoda(currentSlotItem);
                break;
            case 20:
                HandleMP5(currentSlotItem);
                break;
            case 21:
                // building
                // cant pick in hands
                break;
            case 22:
                HandleSulfurPick(currentSlotItem);
                break;
            case 23:
                HandleHammer(currentSlotItem);
                break;
            case 24:
                HandleRepairHammer(currentSlotItem);
                break;
            case 25:
                HandleTomatoSoup(currentSlotItem);
                break;
            default:
                // Handle default case
                // for now exit
                isActing = false;
                currentSlotItem = null;
                return;
        }
    }


    public void shootProjectile(int pDamage, int bDamage, float projSpeed) {
        Quaternion adjustedRotation = Quaternion.Euler(0, inventory.player.transform.rotation.eulerAngles.y+ 90, 0);
        Vector3 spawnPosition = inventory.player.transform.position + (inventory.player.transform.right * 0.75f);
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, adjustedRotation); // Instantiate projectile
        Projectile projScript = projectile.GetComponent<Projectile>();
        projScript.Init(pDamage, bDamage, projSpeed);
        projScript.owner = inventory.player;
        NetworkServer.Spawn(projectile);
        inventory.player.projectiles.Add(projScript); // Add to player's list
    }


    public void HandleAK47(SlotItem item) {
        // add bullet image 
        shootProjectile(12, 24, 0.2f);
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