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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void LoadImage(Texture2D tex)
    {
        //image.texture = tex;
        quadRenderer.material.mainTexture = tex;

        //image.rectTransform.sizeDelta = new Vector2(tex.width, tex.height);
    }
}
