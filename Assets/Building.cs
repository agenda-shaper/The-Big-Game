using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour {
    public bool isBuilding;
    public SlotItem slotItem;
    public int rotation;
    public int x;
    public int y;

    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    public void startBuilding(SlotItem item) {
        // when building - just check if slotitem matches with server slotitem
        // or just when a thing is called pass the slotitem and other info of building
        // when actually building - then in server check if that slotitem exists - and update


        gameObject.SetActive(true);
        isBuilding = true;
        slotItem = item;

        string meshSource = item.item.blockMeshes.building;


        // Load the mesh from Resources folder
        Mesh loadedMesh = Resources.Load<Mesh>("Meshes/" + meshSource);
        meshFilter.mesh = loadedMesh;

        transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);


        // Load the material for the mesh if needed
        // Material loadedMaterial = Resources.Load<Material>("Path/To/" + item.materialName);
        // meshRenderer.material = loadedMaterial;
    }

    public void exitBuilding(){
        isBuilding = false;
        slotItem = null;
        gameObject.SetActive(false);

    }

    public void RotateObject()
    {
        rotation = (rotation + 90) % 360; // Update the rotation variable
        transform.rotation = Quaternion.Euler(0, rotation, 0); // Rotate the GameObject
    }

    public void updatePosition(Character player)
    {
        
        // Offset by 22.5 to align with the middle of the tile for 0 degrees
        int section = Mathf.RoundToInt(player.rotation / 45.0f) % 8;

        int tempX = 0;
        int tempY = 0;



        switch (section)
        {
            case 0: tempX = 1;  tempY = 0;  break; // Right
            case 1: tempX = 1;  tempY = 1;  break; // Forward-Right
            case 2: tempX = 0;  tempY = 1;  break; // Forward
            case 3: tempX = -1; tempY = 1;  break; // Forward-Left
            case 4: tempX = -1; tempY = 0;  break; // Left
            case 5: tempX = -1; tempY = -1; break; // Backward-Left
            case 6: tempX = 0;  tempY = -1; break; // Backward
            case 7: tempX = 1;  tempY = -1; break; // Backward-Right
        }

        Vector3 playerPosition = player.character.transform.position;
        int roundedX = Mathf.RoundToInt(playerPosition.x);
        int roundedY = Mathf.RoundToInt(playerPosition.z);

        Vector3 blockPosition = new Vector3(roundedX + tempX, 0.5f, roundedY + tempY); 
        x = Mathf.RoundToInt(blockPosition.x);
        y = Mathf.RoundToInt(blockPosition.z);
        transform.position = blockPosition;
    }



}
