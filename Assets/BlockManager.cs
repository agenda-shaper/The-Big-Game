using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BlockManager : NetworkBehaviour
{

    [SyncVar]
    public List<Item> ItemDatabase = new List<Item>(); // Initialize an empty list

    [SyncVar]
    public GameObject blockPrefab; // Assign your Block prefab in the Inspector


    [Server]
    public void SpawnBlock(float x, float z, int itemId)
    {

        Item item = GetItemById(itemId);

        Vector3 position = new Vector3(x, 1, z);

        // spawn blank prefab
        GameObject blockInstance = Instantiate(blockPrefab, position, Quaternion.identity);
        
        // load actual block
        SetupBlock(blockInstance, item);
        

        NetworkServer.Spawn(blockInstance);
    }

    [Server]
    public void SetupBlock(GameObject blockInstance, Item item)
    {
        Debug.Log(item);
        // Set up collisions based on width and height
        SetupCollisions(blockInstance,item.width, item.height);

        // Assuming you are using a simple texture to represent the block's appearance
        Texture2D texture = LoadTextureFromPath(item.putableimg.source);

        // Now let's create a material to apply this texture
        Material newMat = new Material(Shader.Find("Standard"));

        if(texture != null)
        {
            newMat.mainTexture = texture;
        }
        else
        {
            Debug.LogError("Texture not loaded!");
            return;
        }


        // Finally, apply this material to the block's MeshRenderer
        MeshRenderer meshRenderer = blockInstance.GetComponent<MeshRenderer>();
        if(meshRenderer != null)
        {
            meshRenderer.material = newMat;
            meshRenderer.enabled = true;  // Explicitly enable the MeshRenderer.

        }
        else
        {
            Debug.LogError("No MeshRenderer found on block instance!");
        }
                
        
    }

    public Item GetItemById(int id)
    {
        return ItemDatabase.Find(item => item.id == id);
    }


    public Texture2D LoadTextureFromPath(string pathWithExtension)
    {
        // Strip off the .png extension if it exists
        string pathWithoutExtension = pathWithExtension.EndsWith(".png") ? pathWithExtension.Substring(0, pathWithExtension.Length - 4) : pathWithExtension;

        Texture2D texture = Resources.Load<Texture2D>(pathWithoutExtension);
        if (texture == null)
        {
            Debug.LogError($"Failed to load texture at path: {pathWithExtension}");
        }

        return texture;
    }



    [Server]
    void SetupCollisions(GameObject blockInstance, int[] widths, int[] heights)
    {

        Debug.Log("collisions setuping");


        if (widths.Length != 4 || heights.Length != 4)
        {
            Debug.LogError("Widths and heights arrays should both have 4 elements.");
            return;
        }

        // Convert values since 100 = 1 unit
        float[] floatWidths = new float[4];
        float[] floatHeights = new float[4];
        for (int i = 0; i < 4; i++)
        {
            floatWidths[i] = widths[i] / 100f;
            floatHeights[i] = heights[i] / 100f;
        }

        CreateBoxColliderChild(blockInstance, "BoxCollider1", new Vector3(0, 0, 0), new Vector3(floatWidths[0], 1, floatHeights[0]));
        CreateBoxColliderChild(blockInstance, "BoxCollider2", new Vector3(0, 0, 0), new Vector3(floatWidths[1], 1, floatHeights[1]));
    }

    [Server]
    void CreateBoxColliderChild(GameObject blockInstance, string childName, Vector3 localPosition, Vector3 size)
    {
        GameObject colliderChild = new GameObject(childName);
        colliderChild.transform.SetParent(blockInstance.transform);
        colliderChild.transform.localPosition = localPosition;

        BoxCollider collider = colliderChild.AddComponent<BoxCollider>();
        collider.size = size;
    }

    [Server]
    public void LoadItemDatabase()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("items");
        //Debug.Log(jsonFile.text);
        if (jsonFile != null)
        {
            ItemDatabaseWrapper itemDatabaseWrapper = JsonUtility.FromJson<ItemDatabaseWrapper>(jsonFile.text);
            if (itemDatabaseWrapper != null && itemDatabaseWrapper.items != null)
            {
                ItemDatabase.AddRange(itemDatabaseWrapper.items); // Add the items to the List
                Debug.Log("Item database loaded.");
            }
            else
            {
                Debug.LogError("Failed to parse JSON data.");
            }
        }
        else
        {
            Debug.LogError("Failed to load JSON file.");
        }
    }
}

