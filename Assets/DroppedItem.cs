using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DroppedItem : MonoBehaviour
{

    public Item item;
    public int count;
    public float despawnTimeLeft;
    public Renderer quadRenderer; // Reference to the quad's renderer.

    public GameObject droppedItem;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void LoadImage(Texture2D tex)
    {
        //image.texture = tex;
        quadRenderer.material.mainTexture = tex;
        quadRenderer.material.shader = Shader.Find("Unlit/Transparent");
        float width = tex.width;
        float height = tex.height;

        // Scale the quad to match the texture's dimensions
        float scalingFactor = 0.007f; // Adjust this to change the overall size of the quad
        droppedItem.transform.localScale = new Vector3(width * scalingFactor, 1f, height * scalingFactor);
    }


}
