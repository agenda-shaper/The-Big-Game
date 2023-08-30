using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


[System.Serializable]
public class ItemDatabaseWrapper
{
  public Item[] items;
}

public class BlockManager : NetworkBehaviour
{

    [SyncVar]
    public List<Item> ItemDatabase = new List<Item>(); // Initialize an empty list

    [SyncVar]
    public GameObject blockPrefab; // Assign your Block prefab in the Inspector

    public Dictionary<Vector2, List<Block>> blockMap = new Dictionary<Vector2, List<Block>>();



    [Server]
    public void SpawnBlock(float x, float z, int itemId)
    {

        Vector2 coord = new Vector2(x, z);

        Item item = GetItemById(itemId);

         // Check if the coordinate is already occupied by any block.
        if (blockMap.ContainsKey(coord))
        {
            // Coordinate is occupied, exit the function.
            return;
        }

        // add a check if its a spawnable item


        // check if block is not occupied ( there will be layers )

        Vector3 position = new Vector3(x, 1, z);

        // spawn blank prefab
        GameObject blockInstance = Instantiate(blockPrefab, position, Quaternion.identity);

        Block block = blockInstance.GetComponent<Block>();  

        block.item = item;  

        if (blockMap.ContainsKey(coord))
        {
            blockMap[coord].Add(block);
        }
        else
        {
            blockMap[coord] = new List<Block> { block };
        }

        //Debug.Log(blockMap[coord]);

        // load actual block in map collisions + meshes
        SetupBlock(block);

        NetworkServer.Spawn(blockInstance);

        
    }

    [Server]
    public void SetupBlock(Block block)
    {
        Debug.Log(block.item);
        // Set up collisions based on width and height
        SetupCollisions(block.blockInstance,block.item.width, block.item.height);

        // Assuming you are using a simple texture to represent the block's appearance
        //Debug.Log(block.item.instation);

        UpdateBlock(block);

        Material material = Resources.Load<Material>("Materials/YellowMat");
        block.meshRenderer.material = material;
        block.meshRenderer.enabled = true;
        Texture2D texture = LoadTextureFromPath("wooden");
                  
        if(texture != null)
        {
            block.LoadImageTexture(texture);
        }
        else
        {
            Debug.LogError("Texture not loaded!");
            return;
        }
                
        
    }

    [Server]
    public void UpdateBlock(Block block)
    {

        List<Block> neighbors = GetNeighboringBlocks(block);
        CheckAndUpdateMesh(block);

        foreach (var neighbor in neighbors)
        {
            //Debug.Log("updating neighbour");
            CheckAndUpdateMesh(neighbor);
            // update all neighbours too
        }
    }

    [Server]
    public void CheckAndUpdateMesh(Block block)
    {
        float x = block.transform.position.x;
        float z = block.transform.position.z;

        MeshFilter meshFilter = block.GetComponent<MeshFilter>();
        meshFilter.mesh.Clear(); // Clear the existing mesh data

        CombineInstance[] combine = new CombineInstance[4];

        Vector2[][] cornerOffsets = {
            new Vector2[] {new Vector2(0, 1), new Vector2(1, 0)  },  // Top-Right
            new Vector2[] { new Vector2(-1, 0), new Vector2(0, 1) }, // Top-Left
            new Vector2[] { new Vector2(0, -1), new Vector2(-1, 0)  }, // Bottom-Left
            new Vector2[] { new Vector2(1, 0), new Vector2(0, -1) } // Bottom-Right
        };

        float[] baseRotationAngles = { 180.0f, 90.0f, 0.0f, 270.0f };  // One for each corner

        for (int i = 0; i < 4; i++)
        {
            Vector3 translation;

            Vector2[] offsets = cornerOffsets[i];
    
            if (i == 0 || i == 2) {  // 1st and 3rd
                translation = new Vector3(offsets[0].y, -1f, offsets[1].x);
            } else {  // 2nd and 4th
                translation = new Vector3(offsets[0].x, -1f, offsets[1].y);
            }
            Vector2 diagonalOffset = offsets[0] + offsets[1];
            bool side1 = blockMap.ContainsKey(new Vector2(x + offsets[0].x, z + offsets[0].y));
            bool side2 = blockMap.ContainsKey(new Vector2(x + offsets[1].x, z + offsets[1].y));
            bool diag = blockMap.ContainsKey(new Vector2(x + diagonalOffset.x, z + diagonalOffset.y));


            (string meshSource, float rotationAngle) = SelectMesh(block.item, side1, side2, diag);
            //Debug.Log("source: " + meshSource);
            Mesh cornerMesh = LoadBlockMesh(meshSource);  // This will load the Mesh
            float finalRotation = rotationAngle + baseRotationAngles[i] + block.rotation;  // Combine with base rotation
            


            combine[i].mesh = cornerMesh;
            Matrix4x4 scaleMatrix = Matrix4x4.Scale(new Vector3(0.25f, 0.25f, 0.25f));
            combine[i].transform = scaleMatrix * Matrix4x4.Translate(translation) * Matrix4x4.Rotate(Quaternion.Euler(0, finalRotation, 0));
        }

        meshFilter.mesh = new Mesh();
        meshFilter.mesh.CombineMeshes(combine, true, true);


        

    }
    private Mesh LoadBlockMesh(string meshSource)
    {
        return Resources.Load<Mesh>("BlockMeshes/" + meshSource);
    }


    private (string, float) SelectMesh(Item item, bool side1, bool side2, bool diag)
    {
        float rotationAngle = 0.0f;

        if (side1 && side2 && !diag)
        {
            return (item.blockMeshes.inward, rotationAngle);
        }
        else if (side1 && side2 && diag)
        {
            return (item.blockMeshes.enclosed, rotationAngle);
        }
        else if (side1 && !side2)
        {
            return (item.blockMeshes.side, rotationAngle);
        }
        else if (!side1 && side2)
        {
            rotationAngle = 270.0f;
            return (item.blockMeshes.side, rotationAngle);
        }
        else
        {
            return (item.blockMeshes.outward, rotationAngle);
        }
    }




    public List<Block> GetNeighboringBlocks(Block block)
    {
        float x = block.transform.position.x;
        float z = block.transform.position.z;
        List<Block> neighbors = new List<Block>();
        
        Vector2[] offsets = { 
            new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(0, -1),
            new Vector2(1, 1), new Vector2(-1, -1), new Vector2(-1, 1), new Vector2(1, -1)

        };

        foreach (Vector2 offset in offsets)
        {
            Vector2 neighborCoord = new Vector2(x + offset.x, z + offset.y);
            if (blockMap.TryGetValue(neighborCoord, out List<Block> neighborBlocks))
            {
                neighbors.AddRange(neighborBlocks);
            }
        }
        return neighbors;
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

