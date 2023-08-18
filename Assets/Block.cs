using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Block : NetworkBehaviour
{
    public Renderer blockRenderer; // Reference to the quad's renderer.

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void LoadImage(Texture2D tex)
    {
        //image.texture = tex;
        blockRenderer.material.mainTexture = tex;
        blockRenderer.material.shader = Shader.Find("Sprites/Diffuse");
        blockRenderer.enabled = true;
        //image.rectTransform.sizeDelta = new Vector2(tex.width, tex.height);
    }
}
