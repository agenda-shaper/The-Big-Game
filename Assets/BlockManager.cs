//using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Linq;

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

    public Dictionary<Tuple<int, int>, List<Block>> blockMap = new Dictionary<Tuple<int, int>, List<Block>>();

    [Server]
    public void SpawnResource(ResourceType type, int posX, int posZ, int rotation){
        rotation *= 90;


        Tuple<int, int> coord = new Tuple<int, int>(posX, posZ);
        
        // Check if the coordinate is already occupied by any blocks.
        if (blockMap.ContainsKey(coord))
        {
            List<Block> existingBlocks = blockMap[coord]; // Retrieve the list of blocks at the location

            foreach (Block existingBlock in existingBlocks)
            {
                // Check if the new block and the existing block have the same collision type
                if (existingBlock.item.collidesWithEntities)
                {
                    return; // Prevent spawning if collides
                }
            }
        }

        Vector3 position = new Vector3(posX, 1, posZ);

        // spawn blank prefab
        GameObject blockInstance = Instantiate(blockPrefab, position, Quaternion.Euler(0, 0, 0));

        Block block = blockInstance.GetComponent<Block>(); 

        block.mainBodyObj.transform.rotation = Quaternion.Euler(0, rotation, 0);

        //block.health = item.health;
        block.rotation = rotation;
        block.door.gameObject.SetActive(false);
        block.resource.InitResource(type);
        Mesh mesh = block.resource.PickMesh();

        block.meshFilter.mesh = mesh;
        float finalRotation = block.rotation;

        // Set rotation
        block.meshFilter.transform.rotation = Quaternion.Euler(0, finalRotation, 0);

        // Set scale
        block.meshFilter.transform.localScale = new Vector3(0.6f,  0.6f, 0.6f);

        block.meshFilter.transform.localPosition = new Vector3(0f, 0f, 0f);

        Vector3[] vertices = block.meshFilter.mesh.vertices;  // Get all vertices
        Vector2[] newUVs = new Vector2[vertices.Length]; // Initialize new UV array

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = vertices[i];
            newUVs[i] = new Vector2(vertex.x * 0.5f, vertex.z * 0.5f);  // Planar projection with tiling
        }

        block.meshFilter.mesh.uv = newUVs; // Set the mesh's UVs

        // Add a MeshCollider to the blockInstance
        MeshCollider meshCollider = block.meshObject.gameObject.AddComponent<MeshCollider>();

        // Set the sharedMesh property of the MeshCollider to the picked mesh
        meshCollider.sharedMesh = mesh;

        Texture2D texture;

        texture = LoadTextureFromPath("Materials/Resource_" + type);
        if(texture != null)
        {
            block.loadTexture(texture);
            return;
        }
        

        
    }

    [Server]
    public void SpawnBlock(int itemId, int x, int z, int rotation,Character owner=null)
    {

        rotation *= 90;

        Tuple<int, int> coord = new Tuple<int, int>(x, z);


        Item item = GetItemById(itemId);

        if (item == null) {
            Debug.Log("not found"+itemId);  
            return;
        }
        if (item.connection_type == "wall" || item.connection_type == "floor"){
            rotation = 0;
        }


        // Check if the coordinate is already occupied by any blocks.
        if (blockMap.ContainsKey(coord))
        {
            List<Block> existingBlocks = blockMap[coord]; // Retrieve the list of blocks at the location

            foreach (Block existingBlock in existingBlocks)
            {
                // Check if the new block and the existing block have the same collision type
                if (existingBlock.item == null){
                    return;
                    // probably resource or smh
                }
                if (item.collidesWithEntities == existingBlock.item.collidesWithEntities)
                {
                    return; // Prevent spawning if they are the same
                }
            }
        }

        // add a check if its a spawnable item


        // check if block is not occupied ( there will be layers )

        Vector3 position = new Vector3(x, 1, z);

        // spawn blank prefab
        GameObject blockInstance = Instantiate(blockPrefab, position, Quaternion.Euler(0, 0, 0));

        Block block = blockInstance.GetComponent<Block>(); 

        block.mainBodyObj.transform.rotation =  Quaternion.Euler(0, rotation, 0);

        block.resource.gameObject.SetActive(false);
        block.item = item;  
        block.health = item.health;
        block.rotation = rotation;
        block.owner = owner;

        if (block.item.isDoor){
            block.doorObj.SetActive(true);
            float offsetX, offsetZ;
            switch (rotation)
            {
                
                case 0:
                    offsetX = 0.5f;
                    offsetZ = 0.5f;
                    break;
                case 90:
                    offsetX = 0.5f;
                    offsetZ = -0.5f;
                    break;
                case 180:
                    offsetX = -0.5f;
                    offsetZ = -0.5f;
                    break;
                case 270:
                    offsetX = -0.5f;
                    offsetZ = 0.5f;
                    break;
                default:
                    offsetX = 0f;
                    offsetZ = 0f;
                    break;
            }
            block.objectNode.transform.position = new Vector3(position.x + offsetX, 1, position.z + offsetZ);
            block.mainBodyObj.transform.localPosition = new Vector3(-offsetX, 0, -offsetZ);
        } else {
            block.doorObj.SetActive(false);
        }


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
        //Debug.Log(block.item);
        // Set up collisions based on width and height
        SetupCollisions(block);


        UpdateBlock(block);

        // if (block.item.isDoor){
        //     block.meshObject.transform.Rotate(0, -90, 0);
        // }
        Texture2D texture;
        if (block.item.texture != null ){
            texture = LoadTextureFromPath("Materials/" + block.item.texture);
            if(texture != null)
            {
                block.loadTexture(texture);
                return;
            }
        } else {
            Debug.Log(block.item.inmap_image);
            texture = LoadTextureFromPath(block.item.inmap_image);
            if(texture != null)
            {
                block.loadImageTexture(texture,block.item);

                return;
            }
        }

        Debug.LogError("Texture not loaded!");
    }
    private Mesh LoadBlockMesh(string meshSource)
    {
        return Resources.Load<Mesh>("Meshes/" + meshSource);
    }

    [Server]
    public void UpdateBlock(Block block)
    {
        if (block.item.connection_type == null){
            //Debug.Log("null connection type of: "+ block.item.details.name);
            return;
        } else if (block.item.connection_type == "base_building") {
            Mesh mesh = LoadBlockMesh(block.item.blockMeshes.building);  // This will load the Mesh
            block.meshFilter.mesh = mesh;
            float finalRotation = block.rotation;
            if (block.door.isOpen){
                finalRotation+=180f;
            }

            // Set rotation
            block.meshFilter.transform.rotation = Quaternion.Euler(0, finalRotation, 0);

            // Set scale
            block.meshFilter.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);

            block.meshFilter.transform.localPosition = new Vector3(0f, -0.5f, 0f);


            Vector3[] vertices = block.meshFilter.mesh.vertices;  // Get all vertices
            Vector2[] newUVs = new Vector2[vertices.Length]; // Initialize new UV array

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 vertex = vertices[i];
                newUVs[i] = new Vector2(vertex.x, vertex.z);  // Planar projection
            }

            block.meshFilter.mesh.uv = newUVs; // Set the mesh's UVs
            return;
        }


        List<Block> neighbors = GetNeighboringBlocks(block);
        CheckAndUpdateMesh(block);
        foreach (var neighbor in neighbors)
        {
            CheckAndUpdateMesh(neighbor);
        }        
    }

    private bool IsConnected(Block block, Vector2 offset)
    {
        if (block.animating) {
            //Debug.Log("animating disconnected");
            return false;}
        Tuple<int, int> coord = new Tuple<int, int>((int)(block.transform.position.x + offset.x), (int)(block.transform.position.z + offset.y)); 
        if (blockMap.TryGetValue(coord, out List<Block> neighborBlocks))
        {
            return neighborBlocks.Any(b => b.item.id == block.item.id && !b.animating);
        }
        return false;
    }

    [Server]
    public void CheckAndUpdateMesh(Block block)
    {
        float x = block.transform.position.x;
        float z = block.transform.position.z;

        block.meshFilter.mesh.Clear(); // Clear the existing mesh data

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

            // if block is animating then treat it as not connected
            bool side1 = IsConnected(block, new Vector2(offsets[0].x, offsets[0].y));
            bool side2 = IsConnected(block, new Vector2(offsets[1].x, offsets[1].y));
            bool diag = IsConnected(block, new Vector2(diagonalOffset.x, diagonalOffset.y));


            (string meshSource, float rotationAngle) = SelectMesh(block.item, side1, side2, diag);
            //Debug.Log("source: " + meshSource);
            Mesh cornerMesh = LoadBlockMesh(meshSource);  // This will load the Mesh
            float finalRotation = rotationAngle + baseRotationAngles[i];// + block.rotation;  // Combine with base rotation
            


            combine[i].mesh = cornerMesh;
            Matrix4x4 scaleMatrix = Matrix4x4.Scale(new Vector3(0.25f, 0.25f, 0.25f));
            combine[i].transform = scaleMatrix * Matrix4x4.Translate(translation) * Matrix4x4.Rotate(Quaternion.Euler(0, finalRotation, 0));
        }

        block.meshFilter.mesh = new Mesh();
        block.meshFilter.mesh.CombineMeshes(combine, true, true);

        Vector3[] vertices = block.meshFilter.mesh.vertices;  // Get all vertices
        Vector2[] newUVs = new Vector2[vertices.Length]; // Initialize new UV array

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = vertices[i];
            newUVs[i] = new Vector2(vertex.x, vertex.z);  // Planar projection
        }

        block.meshFilter.mesh.uv = newUVs; // Set the mesh's UVs

    }
    


    private (string, float) SelectMesh(Item item, bool side1, bool side2, bool diag)
    {
        float rotationAngle = 0.0f;

        switch (item.connection_type){
            case "wall":
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
            case "floor":
                if (!side1 && !side2)
                {
                    rotationAngle = -90.0f;
                    return (item.blockMeshes.outward, rotationAngle);
                }
                else
                {
                    return (item.blockMeshes.enclosed, rotationAngle);
                }
            default:
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
            Tuple<int, int> neighborCoord = new Tuple<int, int>((int)(x + offset.x), (int)(z + offset.y)); 
            if (blockMap.TryGetValue(neighborCoord, out List<Block> neighborBlocks))
            {
                // Filter blocks with the same item.id
                var filteredNeighbors = neighborBlocks.Where(n => n.item.id == block.item.id).ToList();
                neighbors.AddRange(filteredNeighbors);
            }
        }
        return neighbors;
    }





    public Item GetItemById(int id)
    {
        return ItemDatabase.Find(item => item.id == id);
    }

    public List<Item> GetItemsByType(int type)
    {
        Debug.Log($"Fetching items of type: {type}");
        List<Item> foundItems = ItemDatabase.FindAll(item => item.detail.type == type);
        Debug.Log($"Found {foundItems.Count} items of type: {type}");

        foreach(var item in foundItems)
        {
            Debug.Log($"ID: {item.id}, Name: {item.detail.name}, First Image Source: {item.img?.source?[0] ?? "None"}");
        }


        return foundItems;
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
    BoxCollider CreateBoxColliderChild(Block block, string childName, Vector3 localPosition, Vector3 size, Vector3 rotation = new Vector3(), bool isTrigger = false)
    {
        GameObject colliderChild = new GameObject(childName);
        colliderChild.transform.SetParent(block.collidersNode.transform);
        colliderChild.transform.localPosition = localPosition;
        colliderChild.transform.localRotation = Quaternion.Euler(rotation); // Rotation added here

        BoxCollider collider = colliderChild.AddComponent<BoxCollider>();
        collider.size = size;
        collider.isTrigger = isTrigger; // Set the collider to be a trigger based on the passed value
        return collider;
    }

    [Server]
    void SetupCollisions(Block block)
    {
        GameObject blockInstance = block.blockInstance;
        int[] widths = block.item.width;
        int[] heights = block.item.height;

        if (widths.Length != 4 || heights.Length != 4)
        {
            Debug.LogError("Widths and heights arrays should both have 4 elements.");
            return;
        }

        float[] floatWidths = new float[4];
        float[] floatHeights = new float[4];
        for (int i = 0; i < 4; i++)
        {
            floatWidths[i] = widths[i] / 100f;
            floatHeights[i] = heights[i] / 100f;
        }

        // Determine if the colliders should be triggers
        bool shouldCollideWithEntities = block.item.collidesWithEntities;

        BoxCollider collider1 = CreateBoxColliderChild(block, "BoxCollider1", new Vector3(0, 0, 0), new Vector3(floatWidths[0], 1, floatHeights[0]), new Vector3(0, 0, 0), !shouldCollideWithEntities); // block.rotation
        BoxCollider collider2 = CreateBoxColliderChild(block, "BoxCollider2", new Vector3(0, 0, 0), new Vector3(floatWidths[1], 1, floatHeights[1]), new Vector3(0, 90, 0), !shouldCollideWithEntities); // Subtract 90-degree for the second collider
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

