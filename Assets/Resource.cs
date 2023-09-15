using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public enum ResourceType{
    Wood,
    Branch,
    Stone,
    Sulfur,
    Uranium
}
public class Resource : NetworkBehaviour
{

    [SyncVar]
    public ResourceType resourceType;

    public Block parentblock;

    public void InitResource(ResourceType type) {
        parentblock.isResource = true;
        // resource Item
        parentblock.item = Engine.Instance.blockManager.GetItemById(200);

        gameObject.SetActive(true);
        resourceType = type;
        StartCoroutine(DestroyAfterLifetime());
    }

    public Mesh PickMesh()
    {
        // Get the name of the resource type
        string resourceName = resourceType.ToString();

        // Generate a random number between 1 and 4
        int randomNumber = UnityEngine.Random.Range(1, 5);

        // Form the path to the mesh
        string meshPath = "Meshes/" + resourceName + "Resource_" + randomNumber;

        Debug.Log(meshPath);

        // Load the mesh from the Resources folder
        Mesh mesh = Resources.Load<Mesh>(meshPath);
        return mesh;
    }



   
    IEnumerator DestroyAfterLifetime()
    {
        float lifetime = GetLifetime();
        yield return new WaitForSeconds(lifetime);
        parentblock.Despawn();
    }

    float GetLifetime()
    {
        switch (resourceType)
        {
            case ResourceType.Wood:
                return 900f; // Lifetime for Wood   
            case ResourceType.Stone:
                return 1200f; // Lifetime for Stone
            case ResourceType.Sulfur:
                return 1500f; // Lifetime for Sulfur
            case ResourceType.Uranium:
                return 2000f; // Lifetime for Uranium
            default:
                return 500f; // Default lifetime
        }
    }
}
